using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordsExtraction.Models;
using WordsExtraction.Services;

namespace WordsExtractionTests.Services;

[TestClass]
public class WordsExtractorServiceTests
{
    private static string? _filePath;
    private static string? _emptyFilePath;

    [TestInitialize]
    public void ClassSetup()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
        _emptyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "empty.txt");
       
        var lines = new[]
        {
            "This is a test.",
            "A test file for testing.",
            "Testing, testing testing 1-2-3."
        };

        File.WriteAllLines(_filePath, lines, Encoding.ASCII);
        File.WriteAllLines(_emptyFilePath, new[] { "" });
    }

    [TestCleanup]
    public void ClassCleanup()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
        if (File.Exists(_emptyFilePath))
            File.Delete(_emptyFilePath);
    }
    
    [TestMethod]
    public async Task CountWordsAsync_FileExists_ReturnsExpectedWordCounts()
    {
        // Arrange
        var wordsExtractorService = new WordsExtractorService();

        // Act
        IEnumerable<WordCountModel>? res = await wordsExtractorService.CountWordsAsync(_filePath);

        var expected = new List<WordCountModel>
        {
            new("This", 1),
            new("is", 1),
            new("a", 1),
            new("test.", 1),
            new("A", 1),
            new("test", 1),
            new("file", 1),
            new("for", 1),
            new("testing.", 1),
            new("Testing,", 1),
            new("testing", 2),
            new("1-2-3.", 1)
        };
        
        // Assert
        Assert.IsNotNull(res);
        CollectionAssert.AreEquivalent(expected, res.ToList());
    }

    [TestMethod]
    public async Task CountWordsAsync_FileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var wordsExtractorService = new WordsExtractorService();

        // Act
        var actual = await wordsExtractorService.CountWordsAsync("notExistedPath.txt");

        // Assert
        Assert.IsNull(actual);
    }

    [TestMethod]
    public async Task CountWordsAsync_FileIsEmpty_ReturnsEmptyResult()
    {
        // Arrange

        var wordsExtractorService = new WordsExtractorService();

        // Act
        var actual = await wordsExtractorService.CountWordsAsync(_emptyFilePath);

        // Assert
        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.Any());
    }

    [TestMethod]
    public async Task CountWordsAsync_ProgressIsReported()
    {
        // Arrange
        var wordsExtractorService = new WordsExtractorService();
        var reportedCount = 0;
        var progress = new Progress<int>();

        // Act
        var task =  wordsExtractorService.CountWordsAsync(_filePath, progress);
        progress.ProgressChanged += (_, _) => reportedCount++;
        await Task.Delay(100);
        await task;
        // Assert
        Assert.IsTrue(reportedCount > 0);
    }

    [TestMethod]
    public async Task CountWordsAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var wordsExtractorService = new WordsExtractorService();
        var cts = new CancellationTokenSource();
        var task = wordsExtractorService.CountWordsAsync(_filePath, cancellationToken: cts.Token);

        // Act
        cts.Cancel();

        // Assert
        await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => task);
    }
}