using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Compiler.Views;

public partial class ConfirmWindowView : ReactiveWindow<ConfirmWindowView>
{
    public string? FileName { get; }
    public ReactiveCommand<Unit, Unit> Confirm { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }

    public ConfirmWindowView() : this(null)
    {
    }

    public ConfirmWindowView(string? fileName)
    {
        ViewModel = this;

        FileName = fileName;
        Confirm = ReactiveCommand.Create(() => Close(true));
        Cancel = ReactiveCommand.Create(() => Close(false));

        InitializeComponent();

        this.WhenActivated(d =>
        {
            Confirm.DisposeWith(d);
            Cancel.DisposeWith(d);
        });
    }
}