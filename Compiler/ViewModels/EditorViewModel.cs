using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using CodeAnalysis;
using Compiler.ArithmeticExpr;
using Compiler.ArithmeticExpr.InfixNotation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Unit = System.Reactive.Unit;

namespace Compiler.ViewModels;

public interface IFileSaver
{
    Task AsyncSave(string path, string content);
}

public interface IDocument : IEquatable<IDocument>
{
    string Text { get; }

    IDocument CreateSnapshot();
}

public interface IDocumentChangedEventArgs
{
    // IDocument? OldDocument { get; }
    //
    IDocument NewDocument { get; }
}

public struct CaretPos
{
    public int Row { get; init; }
    public int Column { get; init; }

    // public string AsString => $"row {Row} col {Column}";
}

public interface ITextEditor
{
    IDocument Document { get; }

    void UpdateText(string text);

    event EventHandler<IDocumentChangedEventArgs> DocumentChanged;

    void Undo();

    void Redo();

    void Copy();

    void Cut();

    void Paste();

    void Delete();

    void SelectAll();

    void Select(int start, int end);

    CaretPos OffsetToCaretPos(int offset);
}

public interface IEditorsSet
{
    public int FontSize { get; set; }

    void RemoveEditor(EditorViewModel editor);
}

public enum ConfirmSaveResult
{
    Save,
    Cancel,
    DontSave,
    Failed
}

public enum EditAction
{
    Undo,
    Redo,
    Cut,
    Copy,
    Paste,
    Delete,
    SelectAll
}

public struct ConfirmSaveParams
{
    public bool Cancellable { get; init; }
}

public partial class EditorViewModel : ViewModelBase
{
// static string EXAMPLE_TEXT = """const id: &str = """;
    private static readonly string TextExample = """const ba d: &str = "bad2";""";

    public IEditorsSet EditorsSet { get; }

    private readonly IFileSaver _fileSaver;

    private ITextEditor? _textEditor;

    public ITextEditor? TextEditor
    {
        get => _textEditor;
        set
        {
            LatestSavedDocument = value?.Document.CreateSnapshot() ?? null;
            this.RaiseAndSetIfChanged(ref _textEditor, value);
        }
    }

    [Reactive] public CaretPos CaretPos { get; set; }

    [Reactive] private IDocument? LatestSavedDocument { get; set; }

    [Reactive] public string? FilePath { get; private set; }

    [ObservableAsProperty] public string? FileName { get; }

    public int? UntitledFileIndex { get; }

    [ObservableAsProperty] public IDocument? CurrentDocument { get; }

    [ObservableAsProperty] public bool MayBeSaved { get; }

    public ReactiveCommand<Unit, Unit> Close { get; }

    public Interaction<Unit, string?> RequestFilePath { get; } = new();

    public Interaction<ConfirmSaveParams, ConfirmSaveResult> ConfirmSave { get; } = new();

    private readonly Subject<Unit> _activated = new();
    public IObservable<Unit> Activated => _activated.AsQbservable();

    [Reactive] public int SelectedVariant { get; set; } = 0;

    public int SelectedMatchingIndex
    {
        get => _selectedMatchingIndex;
        set
        {
            if (value >= 0 && value < Matches.Count)
            {
                var span = Matches[value].Span;
                TextEditor?.Select(span.Start, span.End);
            }

            this.RaiseAndSetIfChanged(ref _selectedMatchingIndex, value);
        }
    }

    private int _selectedMatchingIndex = -1;

    public ObservableCollection<EditorMatchViewModel> Matches { get; } = new();

    private readonly Regex _regex5 = Regex5();

    private readonly Regex _regex22 = Regex22();

    private readonly Regex _regex28 = Regex28();

    public EditorViewModel(IEditorsSet editorsSet, IFileSaver fileSaver,
        string? filePath, int? untitledIndex)
    {
        EditorsSet = editorsSet;
        FilePath = filePath;
        _fileSaver = fileSaver;
        UntitledFileIndex = untitledIndex;

        Close = ReactiveCommand.CreateFromTask(OnClose);

        this
            .WhenAnyValue(vm => vm.FilePath)
            .Select(Path.GetFileName)
            .ToPropertyEx(this, vm => vm.FileName);

        this.WhenAnyValue(vm => vm.TextEditor)
            .SelectMany(te => te != null
                ? Observable
                    .FromEventPattern<IDocumentChangedEventArgs>(
                        ev => te.DocumentChanged += ev,
                        ev => te.DocumentChanged -= ev
                    )
                    .Select(p => p.EventArgs.NewDocument)
                    .Cast<IDocument?>()
                : Observable
                    .Return<IDocument?>(null)
            ).ToPropertyEx(this, vm => vm.CurrentDocument);

        this
            .WhenAnyValue(vm => vm.CurrentDocument, vm => vm.LatestSavedDocument, vm => vm.FilePath,
                (currentDocument, latestSavedDocument, _) =>
                    currentDocument != null && latestSavedDocument != null &&
                    currentDocument.Text != latestSavedDocument.Text
            ).ToPropertyEx(this, vm => vm.MayBeSaved);
    }

    public void SetExample()
    {
        TextEditor?.UpdateText(TextExample);
    }

    public void Fix()
    {
        // TextEditor?.UpdateText(Parser.Fix(TextEditor.Document.Text));
    }

    public void Run()
    {
        if (TextEditor is not { } editor) return;

        MatchCollection regexMatches;
        if (SelectedVariant == 0)
        {
            regexMatches = _regex5.Matches(editor.Document.Text);
        }
        else if (SelectedVariant == 1)
        {
            regexMatches = _regex22.Matches(editor.Document.Text);
        }
        else if (SelectedVariant == 2)
        {
            regexMatches = _regex28.Matches(editor.Document.Text);
        }
        else
        {
            return;
        }

        var matches = new List<EditorMatchViewModel>();
        foreach (Match regexMatching in regexMatches)
        {
            if (regexMatching.Groups.Count > 0)
            {
                matches.Add(new EditorMatchViewModel(editor, regexMatching.Value,
                    new Span(regexMatching.Index, regexMatching.Index + regexMatching.Length)));
            }
        }

        Matches.Clear();
        Matches.AddRange(matches);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not EditorViewModel editor) return false;

        return (editor.FilePath == FilePath && FilePath != null) ||
               (editor.UntitledFileIndex == UntitledFileIndex && FilePath == null);
    }

    public void Edit(EditAction action)
    {
        if (TextEditor == null) return;

        switch (action)
        {
            case EditAction.Undo:
                TextEditor.Undo();
                break;
            case EditAction.Redo:
                TextEditor.Redo();
                break;
            case EditAction.Copy:
                TextEditor.Copy();
                break;
            case EditAction.Cut:
                TextEditor.Cut();
                break;
            case EditAction.Paste:
                TextEditor.Paste();
                break;
            case EditAction.Delete:
                TextEditor.Delete();
                break;
            case EditAction.SelectAll:
                TextEditor.SelectAll();
                break;
        }
    }

    public void NotifyActive()
    {
        _activated.OnNext(Unit.Default);
    }

    public async Task SaveWithConfirmation()
    {
        if (!MayBeSaved) return;

        var confirmSaveResult = await ConfirmSave.Handle(new ConfirmSaveParams { Cancellable = false });
        if (confirmSaveResult == ConfirmSaveResult.Save)
            await Save();
    }

    private async Task OnClose()
    {
        if (MayBeSaved)
        {
            var confirmSaveResult = await ConfirmSave.Handle(new ConfirmSaveParams() { Cancellable = true });

            if (confirmSaveResult == ConfirmSaveResult.Cancel) return;
            if (confirmSaveResult == ConfirmSaveResult.Failed)
                // TODO
                return;

            if (MayBeSaved && confirmSaveResult == ConfirmSaveResult.Save)
                await Save();
        }

        EditorsSet.RemoveEditor(this);
    }

    public async Task Save()
    {
        if (TextEditor == null)
            return;

        if (FilePath == null)
        {
            await SaveAs();
            return;
        }

        await _fileSaver.AsyncSave(FilePath, TextEditor.Document.Text);
        LatestSavedDocument = TextEditor.Document.CreateSnapshot();
    }

    public async Task SaveAs()
    {
        if (TextEditor == null)
            return;

        var filePath = await RequestFilePath.Handle(Unit.Default);
        if (filePath == null)
            return;

        await _fileSaver.AsyncSave(filePath, TextEditor.Document.Text);

        LatestSavedDocument = TextEditor.Document.CreateSnapshot();
        FilePath = filePath;
    }

    [GeneratedRegex(@"#.*$", RegexOptions.Multiline)]
    private static partial Regex Regex28();

    [GeneratedRegex(@"[\+-]?[0-9]+(\.[0-9]+)?", RegexOptions.Multiline)]
    private static partial Regex Regex22();

    [GeneratedRegex(@"(\b4\d{3}-\d{4}-\d{4}-\d{4})|(\b4\d{3} \d{4} \d{4} \d{4})|(\b4\d{15})",
        RegexOptions.Multiline)]
    private static partial Regex Regex5();
}