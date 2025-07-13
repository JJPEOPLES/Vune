namespace Vune.UI.Services
{
    public interface IVSCodeImportService
    {
        bool ImportSettings(string vscodePath);
        bool ImportExtensions(string vscodePath);
        bool ImportThemes(string vscodePath);
        bool ImportAll(string vscodePath);
        string DetectVSCodePath();
    }
}