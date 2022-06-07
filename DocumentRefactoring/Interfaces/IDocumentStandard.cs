using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    public interface IDocumentStandard
    {
        IDocumentError FindErrorInBorders(PageMargin borders);
        IDocumentError FindErrorInFooters(Footer footer);
        IDocumentError FindErrorInFontName(RunFonts font);
        IDocumentError FindErrorInFontSize(FontSize fontSize);
        IDocumentError FindErrorInFontStyle(RunProperties runProperties);
        IDocumentError FindErrorInFontColor(Color color);
        IDocumentError FindErrorInParagraph(ParagraphProperties paragraphProperties);
        IDocumentError FindErrorInList(ParagraphProperties paragraphProperties, Text text);
        IDocumentError FindErrorInHeader(Paragraph paragraph);
        IDocumentError FindErrorInTable(Paragraph paragraph);
        IDocumentError FindErrorInTableName(Paragraph paragraph);
        IDocumentError FindErrorInImage(Paragraph paragraph);
        IDocumentError FindErrorInBibliographicList(Paragraph paragraph);
    }
}
