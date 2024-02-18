using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using AvaloniaEdit;
using Compiler.ViewModels;
using ReactiveUI;

namespace Compiler.Views;

internal static class ViewsExtensions
{
    public static IObservable<T> WhenAction<VM, T>(this IViewFor<VM> control, Func<VM, T> f)
        where VM : class where T : class
    {
        return WhenAnyMixin.WhenAnyValue(control, v => v.ViewModel, vm => vm != null ? f(vm) : null).WhereNotNull();
    }
}

public partial class EditorView : ReactiveUserControl<EditorViewModel>
{
    private readonly TextEditor _textEditor;
    private readonly FileManager _fileManager;

    public EditorView()
    {
        InitializeComponent();

        _textEditor = this.FindControl<TextEditor>("TextEditor")!;

        var storageProvider = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            .MainWindow.StorageProvider;
        _fileManager = new FileManager(storageProvider);

        this.WhenActivated((d) =>
        {
            this.WhenAnyValue(v => v.ViewModel).WhereNotNull().SelectMany(async vm =>
                {
                    if (vm.FilePath != null)
                        return await _fileManager.TryRead(vm.FilePath);
                    return null;
                }).ObserveOn(RxApp.MainThreadScheduler).Subscribe(content =>
                {
                    _textEditor.Document.UndoStack.ClearAll();
                    _textEditor.Text = content;
                })
                .DisposeWith(d);

            this.BindInteraction(ViewModel, vm => vm.SaveFile, SaveFile).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.SaveFileAs, SaveFileAs).DisposeWith(d);

            this.WhenAction(vm => vm.RedoAction).Subscribe(_ => { _textEditor.Redo(); });
            this.WhenAction(vm => vm.UndoAction).Subscribe(_ => { _textEditor.Undo(); });
            this.WhenAction(vm => vm.CutAction).Subscribe(_ => { _textEditor.Cut(); });
            this.WhenAction(vm => vm.CopyAction).Subscribe(_ => { _textEditor.Copy(); });
            this.WhenAction(vm => vm.PasteAction).Subscribe(_ => { _textEditor.Paste(); });
            this.WhenAction(vm => vm.DeleteAction).Subscribe(_ => { _textEditor.Delete(); });
            this.WhenAction(vm => vm.SelectAllAction).Subscribe(_ => { _textEditor.SelectAll(); });
        });
    }


    private async Task SaveFile(IInteractionContext<string, Unit?> context)
    {
        var fileName = await _fileManager.TrySave(context.Input, _textEditor.Text);
        context.SetOutput(fileName);
    }

    private async Task SaveFileAs(IInteractionContext<string?, string?> context)
    {
        var newFileName = await _fileManager.TrySaveAs(context.Input, _textEditor.Text);
        context.SetOutput(newFileName);
    }
}