using System;
using System.IO;
using System.IO.Enumeration;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData.Kernel;
using Microsoft.VisualBasic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Compiler.ViewModels;

public class EditorViewModel : ViewModelBase
{
    // FIX 
    private readonly MainWindowViewModel _parent;

    public string? FilePath { get; protected set; }

    public string? FileName => Path.GetFileName(FilePath);

    public ReactiveCommand<Unit, Unit> Close { get; }

    public readonly Interaction<string, Unit?> SaveFile;

    public readonly Interaction<string?, string?> SaveFileAs;

    private readonly Subject<Unit> _undo;
    public readonly IObservable<Unit> UndoAction;

    private readonly Subject<Unit> _redo;
    public readonly IObservable<Unit> RedoAction;

    private readonly Subject<Unit> _cut;
    public readonly IObservable<Unit> CutAction;

    private readonly Subject<Unit> _copy;
    public readonly IObservable<Unit> CopyAction;

    private readonly Subject<Unit> _paste;
    public readonly IObservable<Unit> PasteAction;

    private readonly Subject<Unit> _delete;
    public readonly IObservable<Unit> DeleteAction;

    private readonly Subject<Unit> _selectAll;
    public readonly IObservable<Unit> SelectAllAction;

    public EditorViewModel(MainWindowViewModel parent) : this(parent, null)
    {
    }

    public EditorViewModel(MainWindowViewModel parent, string? filePath)
    {
        _parent = parent;
        FilePath = filePath;

        Close = ReactiveCommand.Create(() => _parent.CloseEditor(this));

        SaveFile = new Interaction<string, Unit?>();
        SaveFileAs = new Interaction<string?, string?>();

        _undo = new Subject<Unit>();
        UndoAction = _undo.AsObservable();

        _redo = new Subject<Unit>();
        RedoAction = _undo.AsObservable();

        _cut = new Subject<Unit>();
        CutAction = _cut.AsObservable();

        _copy = new Subject<Unit>();
        CopyAction = _copy.AsObservable();

        _paste = new Subject<Unit>();
        PasteAction = _paste.AsObservable();

        _delete = new Subject<Unit>();
        DeleteAction = _delete.AsObservable();

        _selectAll = new Subject<Unit>();
        SelectAllAction = _selectAll.AsObservable();
    }

    public async Task Save()
    {
        if (FilePath == null)
        {
            await SaveAs();
            return;
        }

        var result = await SaveFile.Handle(FilePath);

        if (result == null)
        {
            // TODO
        }
    }

    public async Task SaveAs()
    {
        var result = await SaveFileAs.Handle(FilePath);

        if (result != null)
        {
            FilePath = result;
        }
        else
        {
            // TODO
        }
    }

    public void Undo()
    {
        _undo.OnNext(Unit.Default);
    }

    public void Redo()
    {
        _redo.OnNext(Unit.Default);
    }

    public void Cut()
    {
        _cut.OnNext(Unit.Default);
    }

    public void Copy()
    {
        _copy.OnNext(Unit.Default);
    }

    public void Paste()
    {
        _paste.OnNext(Unit.Default);
    }

    public void Delete()
    {
        _delete.OnNext(Unit.Default);
    }

    public void SelectAll()
    {
        _selectAll.OnNext(Unit.Default);
    }
}