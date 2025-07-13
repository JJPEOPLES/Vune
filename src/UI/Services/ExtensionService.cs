using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public class ExtensionService : IExtensionService
    {
        private readonly ILogger<ExtensionService> _logger;
        private List<Extension> _installedExtensions = new List<Extension>();
        
        // P/Invoke for the native Core API
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool InstallExtension(string extensionId, string version);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UninstallExtension(string extensionId);
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetInstalledExtensions();
        
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeStringArray(IntPtr array);

        public ExtensionService(ILogger<ExtensionService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing ExtensionService");
            
            // For now, add some sample extensions
            _installedExtensions.Add(new Extension
            {
                Id = "ms-vscode.cpptools",
                Name = "C/C++ Tools",
                Description = "C/C++ IntelliSense, debugging, and code browsing.",
                Version = "1.14.5",
                Publisher = "Microsoft",
                IsEnabled = true
            });
            
            _installedExtensions.Add(new Extension
            {
                Id = "ms-dotnettools.csharp",
                Name = "C# Dev Kit",
                Description = "Official C# extension from Microsoft.",
                Version = "1.25.2",
                Publisher = "Microsoft",
                IsEnabled = true
            });
            
            _installedExtensions.Add(new Extension
            {
                Id = "vscode-icons-team.vscode-icons",
                Name = "vscode-icons",
                Description = "Icons for Visual Studio Code",
                Version = "12.2.0",
                Publisher = "VSCode Icons Team",
                IsEnabled = true
            });
        }

        public IEnumerable<Extension> GetInstalledExtensions()
        {
            return _installedExtensions;
        }

        public IEnumerable<Extension> SearchExtensions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Extension>();
            }
            
            // Mock implementation - in a real app, this would search an extension marketplace
            var mockResults = new List<Extension>
            {
                new Extension
                {
                    Id = "ms-python.python",
                    Name = "Python",
                    Description = "IntelliSense, linting, debugging, code formatting, refactoring, unit tests, and more.",
                    Version = "2023.6.0",
                    Publisher = "Microsoft",
                    IsEnabled = false
                },
                new Extension
                {
                    Id = "esbenp.prettier-vscode",
                    Name = "Prettier - Code formatter",
                    Description = "Code formatter using prettier",
                    Version = "9.10.4",
                    Publisher = "Prettier",
                    IsEnabled = false
                },
                new Extension
                {
                    Id = "ms-azuretools.vscode-docker",
                    Name = "Docker",
                    Description = "Makes it easy to create, manage, and debug containerized applications.",
                    Version = "1.25.1",
                    Publisher = "Microsoft",
                    IsEnabled = false
                }
            };
            
            return mockResults.Where(e => 
                e.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                e.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                e.Publisher.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public bool InstallExtension(string extensionId, string version = null)
        {
            _logger.LogInformation($"Installing extension: {extensionId}, version: {version ?? "latest"}");
            
            // Check if already installed
            if (IsExtensionInstalled(extensionId))
            {
                _logger.LogWarning($"Extension {extensionId} is already installed");
                return false;
            }
            
            // Mock implementation - in a real app, this would call the Core API
            var extension = new Extension
            {
                Id = extensionId,
                Name = extensionId.Split('.').Last(),
                Description = "Extension description",
                Version = version ?? "1.0.0",
                Publisher = extensionId.Split('.').First(),
                IsEnabled = true
            };
            
            _installedExtensions.Add(extension);
            return true;
        }

        public bool UninstallExtension(string extensionId)
        {
            _logger.LogInformation($"Uninstalling extension: {extensionId}");
            
            var extension = GetExtension(extensionId);
            if (extension == null)
            {
                _logger.LogWarning($"Extension {extensionId} is not installed");
                return false;
            }
            
            return _installedExtensions.Remove(extension);
        }

        public bool IsExtensionInstalled(string extensionId)
        {
            return _installedExtensions.Any(e => e.Id == extensionId);
        }

        public Extension GetExtension(string extensionId)
        {
            return _installedExtensions.FirstOrDefault(e => e.Id == extensionId);
        }
    }
}