namespace DocumentRefactoringServer.Models
{
    public class EditedDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public User User { get; set; }
    }
}
