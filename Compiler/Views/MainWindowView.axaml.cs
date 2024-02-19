using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Compiler.ViewModels;
using ReactiveUI;

namespace Compiler.Views;

public partial class MainWindowView : ReactiveWindow<MainWindowViewModel>
{
    private readonly FileManager _fileManager;

    public MainWindowView()
    {
        Closing += OnClosing;

        InitializeComponent();

        _fileManager = new FileManager(StorageProvider);

        this.WhenActivated(d =>
        {
            this.BindInteraction(ViewModel, vm => vm.OpenFile, OpenFile).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.CloseProgram, CloseProgram).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.OpenDocs, OpenDocs).DisposeWith(d);
            this.BindInteraction(ViewModel, vm => vm.OpenAboutProgram, OpenAboutProgram).DisposeWith(d);
        });
    }

    private void OnClosing(object? sender, WindowClosingEventArgs windowClosingEventArgs)
    {
        Task.Run(async () =>
        {
            if (ViewModel != null) await ViewModel.CloseAll();
        }).Wait();
    }

    private async Task OpenFile(IInteractionContext<Unit, string?> context)
    {
        var filePath = await _fileManager.TryOpen();
        context.SetOutput(filePath);
    }

    private async Task CloseProgram(IInteractionContext<Unit, Unit> context)
    {
        Close();
        context.SetOutput(Unit.Default);
    }

    private async Task OpenDocs(IInteractionContext<Unit, Unit> context)
    {
        // TODO
        context.SetOutput(Unit.Default);
    }

    private async Task OpenAboutProgram(IInteractionContext<Unit, Unit> context)
    {
        // TODO
        context.SetOutput(Unit.Default);
    }
}