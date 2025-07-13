namespace Vune.UI.Models
{
    public class Document
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
        public bool IsModified { get; set; }
    }
}