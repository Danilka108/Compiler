using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Compiler.ConstExpr;
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

public class EditorViewModel : ViewModelBase
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

    public ObservableCollection<EditorErrorViewModel> Errors { get; }

    [Reactive] public bool HasErrors { get; private set; }

    public ObservableCollection<EditorTokenViewModel> Tokens { get; }

    private readonly Subject<Unit> _activated = new();
    public IObservable<Unit> Activated => _activated.AsQbservable();

    private int _selectedTokenIndex = -1;

    private readonly Lexer _lexer = new();

    private readonly Analyzer _analyzer = new();

    public int SelectedTokenIndex
    {
        get => _selectedTokenIndex;
        set
        {
            if (value >= 0 && value < Tokens.Count)
            {
                var span = Tokens[value].Lexeme.Span;
                TextEditor?.Select(span.Start, span.End);
            }

            this.RaiseAndSetIfChanged(ref _selectedTokenIndex, value);
        }
    }

    private int _selectedErrorIndex = -1;

    public int SelectedErrorIndex
    {
        get => _selectedErrorIndex;
        set
        {
            if (value >= 0 && value < Errors.Count)
            {
                var span = Errors[value].Error.Span;
                TextEditor?.Select(span.Start, span.End);
            }

            this.RaiseAndSetIfChanged(ref _selectedErrorIndex, value);
        }
    }

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

        Errors = new ObservableCollection<EditorErrorViewModel>();
        Tokens = new ObservableCollection<EditorTokenViewModel>();

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

        var lexemes = _lexer.Scan(editor.Document.Text).ToArray();
        var errors = _analyzer
            .Analyze(lexemes)
            .Select(error => new EditorErrorViewModel(editor, error))
            .ToArray();

        var tokens = lexemes
            .Select(lexeme => new EditorTokenViewModel(editor, lexeme)).ToArray();

        HasErrors = errors.Length != 0;

        Tokens.Clear();
        Tokens.AddRange(tokens);

        Errors.Clear();
        Errors.AddRange(errors);


        //         CaretPos = editor.OffsetToCaretPos(error.Span.Start),
        //         Error = error,
        //         Span = error.Span

        // // var scanner = new Scanner<TokenType, TokenError>(editor.Document.Text, TokensScanners.TokenScanners);
        // var lexemes = Lexer.Scan(editor.Document.Text).ToArray();
        //
        // // var parseErrors = Parser.Parser.Scan(editor.Document.Text).ToArray();
        // var lexemes2 = Parsing.Lexing.Lexer.Scan(editor.Document.Text).ToArray();
        //
        // var tokenViewModels = lexemes.Select(lexeme =>
        //     new EditorTokenViewModel(editor.OffsetToCaretPos(lexeme.Span.Start), lexeme, editor.Document.Text));
        //
        // var parseErrors = Parsing.Parser<Parsing.State>.Parse(lexemes2);
        //
        // // var errorViewModels = lexemes
        // //     .OfType<Lexeme.Invalid>()
        // var errorViewModels = parseErrors
        //     .Select(error => new EditorErrorViewModel(editor.Document.Text)
        //     {
        //         CaretPos = editor.OffsetToCaretPos(error.Span.Start),
        //         Error = error,
        //         Span = error.Span
        //     }).ToArray();
        //
        // // var parseErrorViewModels = parseErrors.Select(error =>
        // //     new EditorParseErrorViewModel
        // //         { ErrorType = error.Type, CaretPos = editor.OffsetToCaretPos(error.Span.Start), Span = error.Span }
        // // );

        // HasErrors = errorViewModels.Length != 0;
        //
        // Tokens.Clear();
        // Tokens.AddRange(tokenViewModels);
        //
        // Errors.Clear();
        // Errors.AddRange(errors);

        // ParseErrors.Clear();
        // ParseErrors.AddRange(parseErrorViewModels);
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
}