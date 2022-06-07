using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    public interface IDocumentError
    {
        string Message { get; set; }
        string Standart { get; set; }
        string StandartItem { get; set; }
        Prioritys Priority { get; set; }
        Categories Category { get; set; }

        Comment GetComment();
    }
}
