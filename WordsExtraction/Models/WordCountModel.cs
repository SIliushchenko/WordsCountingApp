namespace WordsExtraction.Models;

public struct WordCountModel
{
    public WordCountModel(string word, uint count)
    {
        Word = word;
        Count = count;
    }

    public string Word { get; set; }

    public uint Count { get; set; }
}