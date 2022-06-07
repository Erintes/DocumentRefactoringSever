using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    public class Editor : IDisposable
    {
        private string currentCommentId;
        private Comments comments;
        private WordprocessingDocument document;
        private IDocumentStandard standard;
        private Dictionary<Paragraph, List<IDocumentError>> errors; 

        public Editor(WordprocessingDocument document, IDocumentStandard standard)
        {
            this.document = document;
            this.standard = standard;
            errors = new Dictionary<Paragraph, List<IDocumentError>>();
            SetCommentsAndCurrentId();
        }

        public void EditDocument()
        {
            var errs = new List<IDocumentError>();
            var skipParagraphCount = 0;

            var sectionProperties = document.MainDocumentPart.Document.Descendants<SectionProperties>().First();
            CheckBorders(sectionProperties, errs);

            foreach (var paragraph in document.MainDocumentPart.Document.Descendants<Paragraph>())
            {
                if (skipParagraphCount > 0)
                {
                    skipParagraphCount--;
                    continue;
                }
                if (paragraph.ParagraphProperties == null || !paragraph.Descendants<Run>().Any()) continue;
                if (CheckImage(paragraph, errs))
                {
                    SaveErrs(paragraph, errs);
                    skipParagraphCount++;
                    continue;
                }
                if (CheckTableName(paragraph, errs))
                {
                    SaveErrs(paragraph, errs);
                    continue;
                }
                if (!CheckTableCell(paragraph, errs))
                    CheckParagraph(paragraph, errs);

                foreach (var run in paragraph.Descendants<Run>())
                {
                    if (run.RunProperties == null) continue;
                    if (CheckHeader(paragraph, run, errs)) continue;
                    CheckFontFamily(run, errs);
                    CheckFontSize(paragraph, run, errs);
                    CheckFontColor(run, errs);
                    CheckFontStyle(run, errs);
                    CheckList(paragraph, run, errs);
                }
                SaveErrs(paragraph, errs);
            }
        }

        private bool CheckTableCell(Paragraph paragraph, List<IDocumentError> errs)
        {
            if (paragraph.Parent is TableCell)
            {
                errs.Add(standard.FindErrorInTable(paragraph));
                return true;
            }
            return false;
        }

        private bool CheckTableName(Paragraph paragraph, List<IDocumentError> errs)
        {
            if (paragraph.ElementsAfter().FirstOrDefault() is Table)
            {
                errs.Add(standard.FindErrorInTableName(paragraph));
                return true;
            }
            return false;
        }

        private void SaveErrs(Paragraph paragraph, List<IDocumentError> errs)
        {
            if (!errors.ContainsKey(paragraph))
                errors.Add(paragraph, errs.Where(x => x.Category != Categories.WithoutErrors).ToList());
            errs.Clear();
        }

        private bool CheckImage(Paragraph paragraph, List<IDocumentError> errs)
        {
            if (paragraph.Descendants<Drawing>().Any())
            {
                errs.Add(standard.FindErrorInImage(paragraph));
                return true;
            }
            return false;
        }

        private bool CheckHeader(Paragraph paragraph, Run run, List<IDocumentError> errs)
        {                    
            if (paragraph.ParagraphProperties.NumberingProperties != null && 
                run.RunProperties.Descendants<Bold>().Any())
            {
                errs.RemoveAll(x => x.Category == Categories.Paragraph);
                errs.Add(standard.FindErrorInHeader(paragraph));
                return true;
            }
            return false;
        }

        private void CheckList(Paragraph paragraph, Run run, List<IDocumentError> errs)
        {
            if (paragraph.ParagraphProperties != null && paragraph.ParagraphProperties.Descendants<NumberingProperties>().Any())
                errs.Add(standard.FindErrorInList(paragraph.ParagraphProperties, run.Descendants<Text>().First()));
        }

        private void CheckParagraph(Paragraph paragraph, List<IDocumentError> errs)
        {
            if (paragraph.ParagraphProperties != null && !errs.Any(x => x.Category == Categories.Paragraph))
                errs.Add(standard.FindErrorInParagraph(paragraph.ParagraphProperties));
        }

        private void CheckBorders(SectionProperties sp, List<IDocumentError> errs)
        {
            var pageMargin = sp.Descendants<PageMargin>().First();
            if (!errs.Any(x => x.Category == Categories.Border))
                errs.Add(standard.FindErrorInBorders(pageMargin));
        }

        private void CheckFontFamily(Run run, List<IDocumentError> errs)
        {
            if (run.RunProperties.RunFonts != null && !errs.Any(x => x.Category == Categories.FontName))
                errs.Add(standard.FindErrorInFontName(run.RunProperties.RunFonts));
        }

        private void CheckFontSize(Paragraph paragraph, Run run, List<IDocumentError> errs)
        {
            if (!(paragraph.Parent is TableCell) && run.RunProperties.FontSize != null && !errs.Any(x => x.Category == Categories.FontSize)) 
                errs.Add(standard.FindErrorInFontSize(run.RunProperties.FontSize));
        }

        private void CheckFontColor(Run run, List<IDocumentError> errs)
        {
            if (run.RunProperties.Color != null && !errs.Any(x => x.Category == Categories.FontColor)) 
                errs.Add(standard.FindErrorInFontColor(run.RunProperties.Color));
        }

        private void CheckFontStyle(Run run, List<IDocumentError> errs)
        {
            if (!errs.Any(x => x.Category == Categories.FontStyle))
                errs.Add(standard.FindErrorInFontStyle(run.RunProperties));
        }

        public void AddComments()
        {
            foreach (var paragraph in errors.Keys)
            {
                foreach (var error in errors[paragraph].DistinctBy(x => x.Category))
                {
                    var cmt = error.GetComment();
                    cmt.Id = currentCommentId;
                    comments.AppendChild(cmt);
                    comments.Save();

                    paragraph.InsertBefore(new CommentRangeStart()
                    { Id = cmt.Id }, paragraph.GetFirstChild<Run>());

                    var cmtEnd = paragraph.InsertAfter(new CommentRangeEnd()
                    { Id = cmt.Id }, paragraph.Elements<Run>().Last());

                    paragraph.InsertAfter(new Run(new CommentReference() { Id = cmt.Id }), cmtEnd);
                    IncreaseCurrentId();
                }
            }
        }

        private void SetCommentsAndCurrentId()
        {
            currentCommentId = "0";
            if (document.MainDocumentPart.GetPartsCountOfType<WordprocessingCommentsPart>() > 0)
            {
                comments = document.MainDocumentPart.WordprocessingCommentsPart.Comments;
                if (comments.HasChildren)
                    currentCommentId = comments.Descendants<Comment>().Select(e => e.Id.Value).Max();
            }
            else
            {
                var commentPart = document.MainDocumentPart.AddNewPart<WordprocessingCommentsPart>();
                commentPart.Comments = new Comments();
                comments = commentPart.Comments;
            }
        }

        private void IncreaseCurrentId()
        {
            var currIdInt = int.Parse(currentCommentId);
            currIdInt++;
            currentCommentId = currIdInt.ToString();
        }

        public void Dispose()
        {
            document.Dispose();
        }
    }
}
