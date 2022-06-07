using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    internal class ErrorS7_32_2017 : IDocumentError
    {
        public string Message { get; set; }
        public string Standart { get; set; }
        public string StandartItem { get; set; }
        public Prioritys Priority { get; set; }
        public Categories Category { get; set; }

        public ErrorS7_32_2017(string message, string standartItem, Prioritys priority, Categories category)
        {
            Message = message;
            Standart = "ГОСТ 7.32–2017";
            StandartItem = standartItem;
            Priority = priority;
            Category = category;
        }

        public Comment GetComment()
        {
            var firstP= new Paragraph(new Run(new Text("Статус: " + GetErrorStatus())));
            var secondP = new Paragraph(new Run(new Text("Сообщение: " + Message)));
            var thirdP = new Paragraph(new Run(new Text("В соответствии со стандартом: " + Standart)));
            var fourthP = new Paragraph(new Run(new Text("Подпунктом: " + StandartItem)));

            var comment = new Comment()
            {
                Author = "Нормконтроль бот",
                Initials = "НК бот",
                Date = DateTime.UtcNow.AddHours(5)
            };

            comment.AppendChild(firstP);
            comment.AppendChild(secondP);
            comment.AppendChild(thirdP);
            comment.AppendChild(fourthP);

            return comment;
        }

        private string GetErrorStatus()
        {
            switch (Priority)
            {
                case Prioritys.Error:
                    return "Ошибка";
                case Prioritys.Warning:
                    return "Предупреждение";
                case Prioritys.Info:
                    return "Информация";
                default:
                    return "Без статуса";
            }
        }
    }
}
