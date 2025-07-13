namespace Vune.UI.Models
{
    public enum FileType
    {
        Code,
        Config,
        Document,
        Image,
        Other
    }

    public class WorkspaceFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public FileType Type { get; set; }
    }
}