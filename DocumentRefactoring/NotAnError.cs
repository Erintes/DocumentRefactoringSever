using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentRefactoring
{
    public class NotAnError : IDocumentError
    {
        public string Message { get; set; }
        public string Standart { get; set; }
        public string StandartItem { get; set; }
        public Prioritys Priority { get; set; }
        public Categories Category { get; set; }

        public Comment GetComment()
        {
            throw new InvalidOperationException();
        }

        public static NotAnError GetError()
        {
            return new NotAnError() 
            { 
                Message = string.Empty,
                Standart = string.Empty,
                StandartItem = string.Empty,
                Priority = Prioritys.Info,
                Category = Categories.WithoutErrors 
            };
        }
    }
}
