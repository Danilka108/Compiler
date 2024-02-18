using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Compiler;

public class FileManager(IStorageProvider storageProvider)
{
    public async Task<string?> TryRead(string filePath)
    {
        var file = await storageProvider.TryGetFileFromPathAsync(filePath);
        if (file == null) return null;

        try
        {
            return await TryReadFile(file);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string?> TryOpen()
    {
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            { AllowMultiple = false });

        return files.Count == 0 ? null : files[0].Path.AbsolutePath;
    }

    public async Task<Unit?> TrySave(string filePath, string content)
    {
        var file = await storageProvider.TryGetFileFromPathAsync(filePath);
        if (file == null) return null;

        try
        {
            await TryWriteFile(file, content);
        }
        catch (Exception)
        {
            return null;
        }

        return Unit.Default;
    }

    public async Task<string?> TrySaveAs(string? filePath, string content)
    {
        var pickerOptions = await GetSavePickerOptions(filePath, storageProvider);
        var file = await storageProvider.SaveFilePickerAsync(pickerOptions);
        if (file == null) return null;

        try
        {
            await TryWriteFile(file, content);
        }
        catch (Exception)
        {
            return null;
        }

        return file.Path.AbsolutePath;
    }

    private static async Task<FilePickerSaveOptions> GetSavePickerOptions(string? prevFilePath,
        IStorageProvider storageProvider)
    {
        var prevDirName = Path.GetDirectoryName(prevFilePath);
        var startLocation = prevDirName != null
            ? await storageProvider.TryGetFolderFromPathAsync(prevDirName)
            : null;

        var prevFile = prevFilePath != null
            ? await storageProvider.TryGetFileFromPathAsync(prevFilePath)
            : null;

        return new FilePickerSaveOptions
        {
            Title = "Save file", SuggestedStartLocation = startLocation, SuggestedFileName = prevFile?.Name
        };
    }

    private static async Task TryWriteFile(IStorageFile file, string content)
    {
        var fileStream = await file.OpenWriteAsync();

        await using var streamWriter = new StreamWriter(fileStream);
        await streamWriter.WriteAsync(content);
        await streamWriter.FlushAsync();
    }

    private static async Task<string> TryReadFile(IStorageFile file)
    {
        var fileStream = await file.OpenReadAsync();

        using var streamWriter = new StreamReader(fileStream);
        var content = await streamWriter.ReadToEndAsync();

        return content;
    }
}