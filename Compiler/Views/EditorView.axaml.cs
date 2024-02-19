using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using AvaloniaEdit;
using AvaloniaEdit.Document;
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

        var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            .MainWindow;
        var storageProvider = mainWindow.StorageProvider;
        _fileManager = new FileManager(storageProvider);

        this.WhenActivated((d) =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Where(vm => vm.Document == null)
                .SelectMany(ReadFile)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(content =>
                {
                    var document = new TextDocument();
                    document.BeginUpdate();
                    document.Text = content ?? "";
                    document.EndUpdate();

                    ViewModel!.Document = document;
                }).Subscribe().DisposeWith(d);

            this.BindInteraction(ViewModel, vm => vm.SaveFile, SaveFile).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.SaveFileAs, SaveFileAs).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.DoCut, async ctx =>
            {
                _textEditor.Cut();
                ctx.SetOutput(Unit.Default);
            });
            this.BindInteraction(ViewModel, vm => vm.DoCopy, async ctx =>
            {
                _textEditor.Copy();
                ctx.SetOutput(Unit.Default);
            });
            this.BindInteraction(ViewModel, vm => vm.DoPaste, async ctx =>
            {
                _textEditor.Paste();
                ctx.SetOutput(Unit.Default);
            });
            this.BindInteraction(ViewModel, vm => vm.DoDelete, async ctx =>
            {
                _textEditor.Delete();
                ctx.SetOutput(Unit.Default);
            });
            this.BindInteraction(ViewModel, vm => vm.DoSelectAll, async ctx =>
            {
                _textEditor.SelectAll();
                ctx.SetOutput(Unit.Default);
            });
            this.BindInteraction(ViewModel, vm => vm.ConfirmSave, async ctx =>
            {
                var confirmWindow = new ConfirmWindowView(ViewModel.FileName);
                var result = await confirmWindow.ShowDialog<bool?>(mainWindow);
                ctx.SetOutput(result);
            });
        });
    }

    private async Task<string?> ReadFile(EditorViewModel? vm)
    {
        if (vm?.FilePath != null)
            return await _fileManager.TryRead(vm.FilePath);
        return null;
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