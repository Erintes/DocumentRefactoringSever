namespace DocumentRefactoringServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<UploadedDocument> UploadedDocuments { get; set; }
        public List<EditedDocument> EditedDocuments { get; set; }
    }
}
