using Async.Mvvm;
using Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WordsCountingApp.Services.PathSelection;
using WordsExtraction.Models;
using WordsExtraction.Services;

namespace WordsCountingApp.ViewModels;

public class WordsCounterViewModel : BindableBase
{
    private readonly IFilePathSelector _filePathSelector;
    private readonly IWordsExtractorService _wordsExtractorService;
    private IAsyncCommand? _processFileCommand;
    private ICommand? _selectFilePathCommand;
    private string? _selectedFilePath;
    private int _progress;

    public WordsCounterViewModel(IFilePathSelector filePathSelector, IWordsExtractorService wordsExtractorService)
    {
        _filePathSelector       = filePathSelector;
        _wordsExtractorService  = wordsExtractorService;
    }

    public string? SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            _selectedFilePath = value;
            OnPropertyChanged();
        }
    }

    public int Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            OnPropertyChanged();
        }
    }

    #region Select file path command

    public ICommand SelectFilePathCommand =>
        _selectFilePathCommand ??= new RelayCommand(SelectFilePathCmdExecute);

    private void SelectFilePathCmdExecute(object param)
    {
        SelectedFilePath = _filePathSelector.GetFilePath();
    }

    #endregion

    #region Process file commnad

    public IAsyncCommand ProcessFileCommand =>
        _processFileCommand ??= new AsyncCommand<IEnumerable<WordCountModel>?>(ProcessFile);

    private async Task<IEnumerable<WordCountModel>?> ProcessFile(CancellationToken ct)
    {
        Progress = 0;
        IProgress<int> progress = new Progress<int>(i => Progress = i);
        var res = await _wordsExtractorService.CountWordsAsync(SelectedFilePath, progress, ct).ConfigureAwait(false);
        return res?.OrderByDescending(_ => _.Count);
    }

    #endregion
}