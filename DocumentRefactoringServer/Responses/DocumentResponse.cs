using System.Text.Json.Serialization;

namespace DocumentRefactoringServer.Responses
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
