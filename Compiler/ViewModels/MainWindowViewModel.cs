using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.VisualBasic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Compiler.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // File
    public ReactiveCommand<Unit, Unit> Create { get; }
    public ReactiveCommand<Unit, Unit> Open { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> SaveAs { get; }
    public ReactiveCommand<Unit, Unit> Exit { get; }

    // Edit
    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }
    public ReactiveCommand<Unit, Unit> Cut { get; }
    public ReactiveCommand<Unit, Unit> Copy { get; }
    public ReactiveCommand<Unit, Unit> Paste { get; }
    public ReactiveCommand<Unit, Unit> Delete { get; }
    public ReactiveCommand<Unit, Unit> SelectAll { get; }

    // Text
    // public IReactiveCommand<Unit, Unit> ProblemStatement;
    // public IReactiveCommand<Unit, Unit> Grammar;
    // public IReactiveCommand<Unit, Unit> GrammarClassification;
    // public IReactiveCommand<Unit, Unit> AnalysisMethod;
    // public IReactiveCommand<Unit, Unit> ErrorDiagnosisAndNeutralization;
    // public IReactiveCommand<Unit, Unit> TextExample;
    // public IReactiveCommand<Unit, Unit> ReferencesList;
    // public IReactiveCommand<Unit, Unit> SourceCode;

    // Docs
    public ReactiveCommand<Unit, Unit> CallDocs { get; }
    public ReactiveCommand<Unit, Unit> ShowAboutProgram { get; }

    public Interaction<Unit, string?> OpenFile { get; }
    public Interaction<Unit, Unit> CloseProgram { get; }
    public Interaction<Unit, Unit> OpenDocs { get; }
    public Interaction<Unit, Unit> OpenAboutProgram { get; }

    private readonly ObservableCollection<EditorViewModel> _editors = [];
    public ReadOnlyObservableCollection<EditorViewModel> Editors => new(_editors);

    [Reactive] public int CurrentEditorIndex { get; set; } = -1;

    public EditorViewModel? CurrentEditor => CurrentEditorIndex != -1 ? Editors[CurrentEditorIndex] : null;

    // public bool HasEditors => CurrentEditor != null;

    public MainWindowViewModel()
    {
        OpenFile = new Interaction<Unit, string?>();
        CloseProgram = new Interaction<Unit, Unit>();
        OpenDocs = new Interaction<Unit, Unit>();
        OpenAboutProgram = new Interaction<Unit, Unit>();

        Create = ReactiveCommand.Create(ExecuteCreateFile);
        Open = ReactiveCommand.CreateFromTask(ExecuteOpenFile);
        Exit = ReactiveCommand.CreateFromTask(ExecuteExit);

        Save = ReactiveCommand.CreateFromTask(ExecuteEditorSave);
        SaveAs = ReactiveCommand.CreateFromTask(ExecuteEditorSaveAs);
        Undo = ReactiveCommand.Create(ExecuteEditorUndo);
        Redo = ReactiveCommand.Create(ExecuteEditorRedo);
        Cut = ReactiveCommand.Create(ExecuteEditorCut);
        Copy = ReactiveCommand.Create(ExecuteEditorCopy);
        Paste = ReactiveCommand.Create(ExecuteEditorPaste);
        Delete = ReactiveCommand.Create(ExecuteEditorDelete);
        SelectAll = ReactiveCommand.Create(ExecuteEditorSelectAll);

        CallDocs = ReactiveCommand.CreateFromTask(ExecuteCallDocs);
        ShowAboutProgram = ReactiveCommand.CreateFromTask(ExecuteShowAboutProgram);
    }

    public void CloseEditor(EditorViewModel editor)
    {
        // Use lambda or comparator
        var editorIndex = _editors.IndexOf(editor);
        if (editorIndex < 0) return;

        if (editor.FilePath == CurrentEditor?.FilePath)
        {
            CurrentEditorIndex = editorIndex - 1 < 0 ? 0 : editorIndex - 1;
            _editors.RemoveAt(editorIndex);
        }
        else if (CurrentEditor != null)
        {
            var currentEditor = CurrentEditor;
            _editors.RemoveAt(editorIndex);
            CurrentEditorIndex = _editors.IndexOf(currentEditor);
        }
    }

    private void ExecuteCreateFile()
    {
        _editors.Add(new EditorViewModel(this));
    }

    private async Task ExecuteOpenFile()
    {
        var fileName = await OpenFile.Handle(Unit.Default);
        _editors.Add(new EditorViewModel(this, fileName));
    }

    private async Task ExecuteEditorSave()
    {
        if (CurrentEditor is { } editor) await editor.Save();
    }

    private async Task ExecuteEditorSaveAs()
    {
        if (CurrentEditor is { } editor) await editor.SaveAs();
    }

    private async Task ExecuteExit()
    {
        await CloseProgram.Handle(Unit.Default);
    }

    private void ExecuteEditorUndo()
    {
        CurrentEditor?.Undo();
    }

    private void ExecuteEditorRedo()
    {
        CurrentEditor?.Redo();
    }

    private void ExecuteEditorCut()
    {
        CurrentEditor?.Cut();
    }

    private void ExecuteEditorCopy()
    {
        CurrentEditor?.Copy();
    }

    private void ExecuteEditorPaste()
    {
        CurrentEditor?.Paste();
    }

    private void ExecuteEditorDelete()
    {
        CurrentEditor?.Delete();
    }

    private void ExecuteEditorSelectAll()
    {
        CurrentEditor?.SelectAll();
    }

    private async Task ExecuteCallDocs()
    {
        await OpenDocs.Handle(Unit.Default);
    }

    private async Task ExecuteShowAboutProgram()
    {
        await OpenAboutProgram.Handle(Unit.Default);
    }
}