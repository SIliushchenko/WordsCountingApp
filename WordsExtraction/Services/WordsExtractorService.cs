using System.Text;
using WordsExtraction.Models;

namespace WordsExtraction.Services;

public class WordsExtractorService : IWordsExtractorService
{
    ///<inheritdoc/>
    public async Task<IEnumerable<WordCountModel>?> CountWordsAsync(
        string? filePath,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return default;
       
        const int bufferSize = 1024 * 1024; // 1MB buffer
        
        var fileSize = new FileInfo(filePath).Length;
        long bytesProcessed = 0;
        var wordCounts = new Dictionary<string, uint>();

        await using (var fileStream = new FileStream(filePath, FileMode.Open))
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
        {
            var buffer = new char[bufferSize];
            var remainingCharsAsString = string.Empty;

            while (true)
            {
                //Read asynchronously to a buffer
                var bytesRead = await streamReader.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);
                bytesProcessed += bytesRead;

                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                //Check if nothing is read or the end of stream is reached
                if (bytesRead == 0)
                {   
                    if (!string.IsNullOrEmpty(remainingCharsAsString))
                        CountWordsInLine(remainingCharsAsString, wordCounts);
                    break;
                }

                var bufferOffset = 0;
                
                while (true)
                {
                    var newlineIndex = Array.IndexOf(buffer, '\n', bufferOffset, bytesRead - bufferOffset);
                    if (newlineIndex == -1)
                        break;

                    // Get the line from the buffer
                    int lineLength = newlineIndex - bufferOffset + 1;
                    string line = remainingCharsAsString + new string(buffer, bufferOffset, lineLength);
                    remainingCharsAsString = string.Empty;

                    // Process the line here
                    CountWordsInLine(line, wordCounts);
                    
                    bufferOffset = newlineIndex + 1;
                }

                //  Determine the remaining partial line to append it with a new line from the next buffer
                if (bufferOffset < bytesRead)
                {
                    var remainingBytes = bytesRead - bufferOffset;
                    remainingCharsAsString += new string(buffer, bufferOffset, remainingBytes);
                }

                //Report about the progress
                if (progress == null) continue;
                var percentComplete = (int)(bytesProcessed * 100 / fileSize);
                progress.Report(percentComplete);
            }
        }

        progress?.Report(100);
        return wordCounts.Select(_ => new WordCountModel(_.Key, _.Value));
    }

    private static void CountWordsInLine(string line, IDictionary<string, uint> wordCounts)
    {
        var words = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (!wordCounts.ContainsKey(word))
                wordCounts[word] = 1;
            else
                wordCounts[word]++;
        }
    }
}