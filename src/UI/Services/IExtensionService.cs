using System.Collections.Generic;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public interface IExtensionService
    {
        void Initialize();
        IEnumerable<Extension> GetInstalledExtensions();
        IEnumerable<Extension> SearchExtensions(string query);
        bool InstallExtension(string extensionId, string version = null);
        bool UninstallExtension(string extensionId);
        bool IsExtensionInstalled(string extensionId);
        Extension GetExtension(string extensionId);
    }
}