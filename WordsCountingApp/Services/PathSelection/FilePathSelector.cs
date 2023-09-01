using Microsoft.Win32;

namespace WordsCountingApp.Services.PathSelection;

public class FilePathSelector : IFilePathSelector
{
    public string GetFilePath()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt"
        };

        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
    }
}