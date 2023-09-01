using WordsExtraction.Models;

namespace WordsExtraction.Services;

public interface IWordsExtractorService
{
    /// <summary>
    /// Parses given file text file and count each words occurrence
    /// </summary>
    /// <param name="filePath">File path to process</param>
    /// <param name="progress">Progress callback of <see cref="IProgress{T}"/> type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<IEnumerable<WordCountModel>?> CountWordsAsync(
        string? filePath,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
}