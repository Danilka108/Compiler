using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Compiler.ViewModels;
using ReactiveUI;

namespace Compiler.Views;

public partial class MainWindowView : ReactiveWindow<MainWindowViewModel>, IProgramCloser
{
    public MainWindowView()
    {
        InitializeComponent();

        Closing += HandleClosing;

        this.WhenActivated(d =>
        {
            this
                .BindInteraction(ViewModel, vm => vm.RequestFilePath, RequestFilePath)
                .DisposeWith(d);

            this
                .BindInteraction(ViewModel, vm => vm.OpenDocs, async context =>
                {
                    var docsWindow = new DocsWindowView();
                    docsWindow.DataContext = docsWindow;
                    await docsWindow.ShowDialog(this);
                    context.SetOutput(Unit.Default);
                })
                .DisposeWith(d);

            this
                .BindInteraction(ViewModel, vm => vm.OpenAboutProgram, async context =>
                {
                    // await ShowDialog(new DocsWindowView());
                    context.SetOutput(Unit.Default);
                })
                .DisposeWith(d);
        });
    }

    private void OpenAboutPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("About.html");
    }

    private void OpenDocsPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("Docs.html");
    }

    public void OpenFormulationOfTheProblemPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("FormulationOfTheProblem.html");
    }

    private void OpenGrammarPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("Grammar.html");
    }

    private void OpenGrammarClassificationPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("GrammarClassification.html");
    }

    private void OpenAnalysisMethodPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("AnalysisMethod.html");
    }

    private void OpenDiagnosticAndErrorNeutralization(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("DiagnosticsAndErrorNeutralization.html");
    }

    private void OpenSourcesListPage(object sender, RoutedEventArgs e)
    {
        OpenHtmlPage("SourcesList.html");
    }

    private void OpenSourceCodePage(object sender, RoutedEventArgs e)
    {
        OpenBrowserWithUrl(HtmlPages.SourceCodeUrl);
    }

    private async void HandleClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        if (ViewModel != null) await ViewModel.CloseAllEditors();

        Closing -= HandleClosing;
        Close();
    }

    private async Task RequestFilePath(IInteractionContext<Unit, string?> context)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false });

        if (files.Count == 0)
        {
            context.SetOutput(null);
            return;
        }

        context.SetOutput(files[0].Path.AbsolutePath);
    }

    public void CloseProgram()
    {
        Close();
    }

    private void SetEnglish(object? sender, RoutedEventArgs e)
    {
        // Lang.Resources.Culture = new CultureInfo("");
    }

    private void SetRussian(object? sender, RoutedEventArgs e)
    {
        // Lang.Resources.Culture = new CultureInfo("ru");
        // InvalidateVisual();
    }

    private void OpenBrowserWithUrl(string url)
    {
        try
        {
            // Открываем браузер с временным файлом
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при открытии браузера: {ex.Message}");
        }
    }

    private void OpenHtmlPage(string name)
    {
        var resourceName = $"Compiler.HtmlPages.{name}";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        var fileContent = reader.ReadToEnd();

        OpenBrowserFromHtmlFile(fileContent);
    }

    private void OpenBrowserFromHtmlFile(string htmlContent)
    {
        // Создаем временный файл с расширением .html
        var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".html");

        // Записываем HTML в файл
        File.WriteAllText(tempFilePath, htmlContent);

        try
        {
            // Открываем браузер с временным файлом
            Process.Start(new ProcessStartInfo
            {
                FileName = tempFilePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при открытии браузера: {ex.Message}");
        }
    }
}