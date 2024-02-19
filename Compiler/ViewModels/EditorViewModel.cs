using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Compiler.ViewModels;

public class EditorViewModel : ViewModelBase
{
    // FIX 
    private readonly MainWindowViewModel _parent;

    private ITextSource? _latestSavedDocument;

    private TextDocument? _document = null;

    public TextDocument? Document
    {
        get => _document;
        set
        {
            _latestSavedDocument = value?.CreateSnapshot();
            this.RaiseAndSetIfChanged(ref _document, value);
        }
    }

    [Reactive] public string? FilePath { get; private set; }

    public string? FileName => Path.GetFileName(FilePath);

    public ReactiveCommand<Unit, Unit> Close { get; }

    public readonly Interaction<string, Unit?> SaveFile;

    public readonly Interaction<string?, string?> SaveFileAs;

    public Interaction<Unit, Unit> DoCut { get; } = new();

    public Interaction<Unit, Unit> DoCopy { get; } = new();

    public Interaction<Unit, Unit> DoPaste { get; } = new();

    public Interaction<Unit, Unit> DoDelete { get; } = new();

    public Interaction<Unit, Unit> DoSelectAll { get; } = new();

    public Interaction<Unit, bool?> ConfirmSave { get; } = new();

    public EditorViewModel(MainWindowViewModel parent) : this(parent, null)
    {
    }

    public EditorViewModel(MainWindowViewModel parent, string? filePath)
    {
        _parent = parent;
        FilePath = filePath;

        Close = ReactiveCommand.CreateFromTask(OnClose);

        SaveFile = new Interaction<string, Unit?>();
        SaveFileAs = new Interaction<string?, string?>();
    }

    private async Task OnClose()
    {
        if (Document?.Text == _latestSavedDocument?.Text && FileName != null)
        {
            _parent.CloseEditor(this);
            return;
        }

        var shouldBeSaved = await ConfirmSave.Handle(Unit.Default);

        if (shouldBeSaved == true)
        {
            await Save();
            _parent.CloseEditor(this);
        }
    }


    public async Task Save()
    {
        if (FilePath == null)
        {
            await SaveAs();
            return;
        }

        var result = await SaveFile.Handle(FilePath);
        if (result != null) _latestSavedDocument = Document?.CreateSnapshot();
    }

    public async Task SaveAs()
    {
        var result = await SaveFileAs.Handle(FilePath);

        if (result != null)
        {
            FilePath = result;
            _latestSavedDocument = Document?.CreateSnapshot();
        }
    }

    public void Undo()
    {
        if (Document == null) return;
        if (!Document.UndoStack.CanUndo)
            // TODO
            return;

        Document.UndoStack.Undo();
    }

    public void Redo()
    {
        if (Document == null) return;
        if (!Document.UndoStack.CanRedo)
            // TODO
            return;

        Document.UndoStack.Redo();
    }

    public async Task Cut()
    {
        await DoCut.Handle(Unit.Default);
    }

    public async Task Copy()
    {
        await DoCopy.Handle(Unit.Default);
    }

    public async Task Paste()
    {
        await DoPaste.Handle(Unit.Default);
    }

    public async Task Delete()
    {
        await DoDelete.Handle(Unit.Default);
    }

    public async Task SelectAll()
    {
        await DoSelectAll.Handle(Unit.Default);
    }
}