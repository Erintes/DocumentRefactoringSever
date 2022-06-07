using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    public class Standart7_32_2017 : IDocumentStandard
    {
        private const int BorderAllowance = 5;

        public IDocumentError FindErrorInBibliographicList(Paragraph paragraph)
        {
            throw new NotImplementedException();
        }

        public IDocumentError FindErrorInBorders(PageMargin borders)
        {
            var leftBorder = int.Parse(borders.Left);
            var rightBorder = int.Parse(borders.Right);
            var topBorder = int.Parse(borders.Top);
            var bottomBorder = int.Parse(borders.Bottom);

            if (Math.Abs(leftBorder - 1701) > BorderAllowance ||
                Math.Abs(rightBorder - 850) > BorderAllowance ||
                Math.Abs(topBorder - 1134)  > BorderAllowance ||
                Math.Abs(bottomBorder - 1134) > BorderAllowance)
                return new ErrorS7_32_2017(
                    $"Вы используете поля: левое – {leftBorder / 56} мм, правое – {rightBorder / 56} мм, " +
                    $"верхнее – {topBorder / 56} мм, нижнее – {bottomBorder / 56} мм. " +
                    "Следует соблюдать следующие размеры полей: левое – 30 мм, правое – 15 мм, верхнее и нижнее – 20 мм",
                    "6.1.1",
                    Prioritys.Error,
                    Categories.Border);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInFontColor(Color color)
        {
            if (color.Val != "000000")
                return new ErrorS7_32_2017("Цвет шрифта должен быть чёрным.", "6.1.1", Prioritys.Error, Categories.FontColor);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInFontName(RunFonts font)
        {
            if (font.Ascii != "Times New Roman")
                return new ErrorS7_32_2017(
                    $"Вы используете шрифт {font.Ascii}, рекомендуемый тип шрифта основного текста Times New Roman. " +
                    "Использование иного начертания шрифта может применяться для выделения текста.", 
                    "6.1.1", 
                    Prioritys.Warning,
                    Categories.FontName);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInFontSize(FontSize fontSize)
        {
            int fontSizeInPt;
            int.TryParse(fontSize.Val, out fontSizeInPt);
            fontSizeInPt /= 2;
            if (fontSize.Val != "28")
                return new ErrorS7_32_2017(
                    $"Размер шрифта равен {fontSizeInPt} пт, требуемый размер шрифта 14 пт.", 
                    "6.1.1", 
                    Prioritys.Error,
                    Categories.FontSize);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInFontStyle(RunProperties runProperties)
        {
            var errorMessage = string.Empty;
            if (runProperties.Descendants<Bold>().Any())
                errorMessage += "Полужирный шрифт применяют только для заголовков разделов и подразделов. ";
            if (runProperties.Descendants<Italic>().Any())
                errorMessage += "Использование курсива допускается для обозначения объектов и написания терминов.";
            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, "6.1.1", Prioritys.Info, Categories.FontStyle);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInFooters(Footer footer)
        {
            throw new NotImplementedException();
        }

        public IDocumentError FindErrorInHeader(Paragraph paragraph)
        {
            var errorMessage = string.Empty;
            var jc = paragraph.ParagraphProperties?.Justification;
            var al = paragraph.ParagraphProperties?.TextAlignment;
            if (jc != null && jc.Val != "left" || 
                jc == null && al != null && al.Val != "baseline")
                errorMessage += "В заголовках и подзаголовках, кроме заголовков структурных элементов, должно использоваться выравнивание по левому краю. ";

            var nextParagraph = paragraph.ElementsAfter().FirstOrDefault(x => x is Paragraph);
            if (nextParagraph != null)
            {
                var nextParagrapRun = nextParagraph.Descendants<Run>().FirstOrDefault();
                if (nextParagrapRun != null)
                {
                    var nextParagraphText = nextParagrapRun.Descendants<Text>().FirstOrDefault();
                    if (nextParagraphText != null)
                        errorMessage += "После заголовка перед подзаголовком или текстом следует оставлять одну пустую строку. ";
                }
            }

            var spacing = paragraph.ParagraphProperties?.SpacingBetweenLines;
            if (spacing != null && spacing.Line != "240")
                errorMessage += "Междустрочный интервал в заголовках и подзаголовках должен быть равен 1. ";

            var indentation = paragraph.ParagraphProperties?.Indentation;
            if (indentation != null && indentation.FirstLine != "709")
                errorMessage += "Требуемый отступ строки 1.25 см. ";

            var run = paragraph.Descendants<Run>().LastOrDefault();
            if (run != null)
            {
                var text = run.Descendants<Text>().LastOrDefault();
                if (text != null && text.Text.EndsWith('.'))
                    errorMessage += "Заголовки и подзаголовки оформляются без точки в конце. ";
            }

            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, " ", Prioritys.Warning, Categories.Paragraph);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInImage(Paragraph paragraph)
        {
            var errorMessage = string.Empty;            
            
            var prevParagraph = paragraph.ElementsBefore().LastOrDefault(x => x is Paragraph);
            if (prevParagraph != null)
            {
                var prevParagraphRun = prevParagraph.Descendants<Run>().LastOrDefault();
                if (prevParagraphRun != null)
                {
                    var prevParagraphText = prevParagraphRun.Descendants<Text>().LastOrDefault();
                    if (prevParagraphText != null)
                        errorMessage += "Перед иллюстрацией следует оставлять одну пустую строку. ";
                }
            }

            var imageDesc = paragraph.ElementsAfter().FirstOrDefault(x => x is Paragraph);
            if (imageDesc != null)
            {
                var descParagraph = (Paragraph)imageDesc;
                var descJc = descParagraph.ParagraphProperties?.Justification;
                if (descJc != null && descJc.Val != "center")
                    errorMessage += "Название иллюстрации следует размещать по центру. ";
                var descRun = descParagraph.Descendants<Run>().LastOrDefault();
                if (descRun != null)
                {
                    var descRunProp = descRun.RunProperties;
                    if (descRunProp != null && descRunProp.FontSize.Val != "28")
                        errorMessage += "Размер шрифта в названии иллюстрации должен быть равен 14 пунктам. ";
                    var descText = descRun.Descendants<Text>().LastOrDefault();
                    if (descText != null && descText.Text.EndsWith('.'))
                        errorMessage += "Точку в конце названия рисунка ставить не следует. ";
                }
            }

            var nextParagraph = imageDesc.ElementsAfter().FirstOrDefault(x => x is Paragraph);
            if (nextParagraph != null)
            {
                var nextParagrapRun = nextParagraph.Descendants<Run>().FirstOrDefault();
                if (nextParagrapRun != null)
                {
                    var nextParagraphText = nextParagrapRun.Descendants<Text>().FirstOrDefault();
                    if (nextParagraphText != null)
                        errorMessage += "После названия иллюстрации следует оставлять одну пустую строку. ";
                }
            }

            if (errorMessage != string.Empty)
            {
                errorMessage += "После иллюстрации должно располагаться её название и номер в формате \"Рисунок 1 - Название рисунка\". ";
                return new ErrorS7_32_2017(errorMessage, " ", Prioritys.Error, Categories.Image);
            }
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInList(ParagraphProperties paragraphProperties, Text text)
        {
            var errorMessage = "Перед каждым элементом перечисления должно быть тире или строчная буква русского алфавита" + 
                ", если требуется указать ссылку на пункт перечисления. ";

            if (!text.Text.Trim().EndsWith(',') || !text.Text.Trim().EndsWith(';'))
                errorMessage += "Каждый элемент перечисления, кроме последнего, должен оканчиваться на \",\" или \";\" если это сложное перечисление.";
            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, "6.4.6", Prioritys.Info, Categories.Lists);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInParagraph(ParagraphProperties paragraphProperties)
        {
            var errorMessage = string.Empty;
            if (paragraphProperties.SpacingBetweenLines?.Line != null && paragraphProperties.SpacingBetweenLines?.LineRule != null)
            {
                double lineSpDouble = int.Parse(paragraphProperties.SpacingBetweenLines.Line) / 240.0;
                if (paragraphProperties.SpacingBetweenLines.Line != "360")
                    errorMessage += $"Используемый вами междустрочный интервал: {lineSpDouble}. Требуемый интервал: 1.5. ";
            }
            else errorMessage += $"Требуемый междустрочный интервал: 1.5. ";

            if (paragraphProperties.Indentation?.FirstLine != null)
            {
                double firstLineIndDouble = int.Parse(paragraphProperties.Indentation.FirstLine) / 567.2;
                if (paragraphProperties.Indentation.FirstLine != "709")
                    errorMessage += $"Используемый вами отступ первой строки: {firstLineIndDouble} см. Требуемый отступ: 1.25 см.";
            }
            else errorMessage += $"Требуемый отступ первой строки: 1.25 см.";

            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, "6.1.1", Prioritys.Error, Categories.Paragraph);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInTable(Paragraph paragraph)
        {
            var errorMessage = string.Empty;
            var tc = paragraph.Parent;
            var tr = tc?.Parent;
            var tbl = tr?.Parent;
            var firstRun = paragraph.Elements<Run>().FirstOrDefault();
            var firstText = firstRun?.Elements<Text>().FirstOrDefault();
            var lastRun = paragraph.Elements<Run>().LastOrDefault();
            var lastText = lastRun?.Elements<Text>().LastOrDefault();

            if (tr != null && !tr.ElementsBefore().Any(x => x is TableRow))
            {
                if (firstText != null && char.IsLower(firstText.Text.First()))
                    errorMessage += "Названия столбцов должны начинаться с цифры или прописной буквы. ";
                if (lastText != null && lastText.Text.EndsWith('.'))
                    errorMessage += "В конце названия столбцов не следует ставить точку. ";
            }

            if (tc != null && !tc.ElementsBefore().Any(x => x is TableCell))
            {
                if (firstText != null && char.IsLower(firstText.Text.First()))
                    errorMessage += "Названия строк должны начинаться с цифры или прописной буквы. ";
                if (lastText != null && lastText.Text.EndsWith('.'))
                    errorMessage += "В конце названия строк не следует ставить точку. ";
            }

            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, " ", Prioritys.Warning, Categories.Table);
            return NotAnError.GetError();
        }

        public IDocumentError FindErrorInTableName(Paragraph paragraph)
        {
            var errorMessage = string.Empty;

            if (paragraph != null)
            {
                var paragraphProp = paragraph.ParagraphProperties;
                if (paragraphProp != null && paragraphProp.Justification != null && paragraphProp.Justification?.Val != "left")
                    errorMessage += "Название таблицы следует располагать над таблицей слева. ";
                if (paragraphProp != null && paragraphProp.Indentation != null && paragraphProp.Indentation.FirstLine != "0")
                    errorMessage += "Название таблицы пишется без абзацного отступа. ";
                var paragraphRun = paragraph.Descendants<Run>().LastOrDefault();
                if (paragraphRun != null)
                {
                    var paragraphText = paragraphRun.Descendants<Text>().LastOrDefault();
                    if (paragraphText != null && paragraphText.Text.EndsWith('.'))
                        errorMessage += "В конце названия таблицы точка не ставится. ";
                }
            }

            if (errorMessage != string.Empty)
                return new ErrorS7_32_2017(errorMessage, " ", Prioritys.Warning, Categories.Table);
            return NotAnError.GetError();
        }
    }
}
