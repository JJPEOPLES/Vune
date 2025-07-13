namespace Vune.ExtensionManager
{
    public class ExtensionInfo
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public bool IsEnabled { get; set; }
    }
}