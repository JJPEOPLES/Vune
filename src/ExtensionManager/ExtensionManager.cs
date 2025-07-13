using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vune.ExtensionManager
{
    public class ExtensionManager
    {
        private readonly ILogger<ExtensionManager> _logger;
        private readonly string _extensionsDirectory;
        private readonly Dictionary<string, ExtensionInfo> _extensions = new Dictionary<string, ExtensionInfo>();

        public ExtensionManager(ILogger<ExtensionManager> logger, string extensionsDirectory)
        {
            _logger = logger;
            _extensionsDirectory = extensionsDirectory;
            
            // Create extensions directory if it doesn't exist
            if (!Directory.Exists(_extensionsDirectory))
            {
                Directory.CreateDirectory(_extensionsDirectory);
            }
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing ExtensionManager");
            
            // Load installed extensions
            LoadInstalledExtensions();
        }

        public IEnumerable<ExtensionInfo> GetInstalledExtensions()
        {
            return _extensions.Values;
        }

        public ExtensionInfo GetExtension(string extensionId)
        {
            if (_extensions.TryGetValue(extensionId, out var extension))
            {
                return extension;
            }
            
            return null;
        }

        public async Task<bool> InstallExtensionAsync(string extensionId, string version = null)
        {
            _logger.LogInformation($"Installing extension: {extensionId}, version: {version ?? "latest"}");
            
            try
            {
                // Check if already installed
                if (_extensions.ContainsKey(extensionId))
                {
                    _logger.LogWarning($"Extension {extensionId} is already installed");
                    return false;
                }
                
                // TODO: Download extension from marketplace
                
                // Create extension directory
                string extensionDir = Path.Combine(_extensionsDirectory, extensionId);
                Directory.CreateDirectory(extensionDir);
                
                // Create package.json
                var packageJson = new
                {
                    name = extensionId,
                    version = version ?? "1.0.0",
                    publisher = extensionId.Split('.').First(),
                    engines = new
                    {
                        vune = "^1.0.0"
                    },
                    main = "./extension.js"
                };
                
                string packageJsonPath = Path.Combine(extensionDir, "package.json");
                await File.WriteAllTextAsync(packageJsonPath, JsonSerializer.Serialize(packageJson, new JsonSerializerOptions { WriteIndented = true }));
                
                // Create extension.js
                string extensionJsPath = Path.Combine(extensionDir, "extension.js");
                await File.WriteAllTextAsync(extensionJsPath, "// Extension entry point");
                
                // Add to installed extensions
                var extension = new ExtensionInfo
                {
                    Id = extensionId,
                    Version = version ?? "1.0.0",
                    Path = extensionDir,
                    IsEnabled = true
                };
                
                _extensions[extensionId] = extension;
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error installing extension: {extensionId}");
                return false;
            }
        }

        public bool UninstallExtension(string extensionId)
        {
            _logger.LogInformation($"Uninstalling extension: {extensionId}");
            
            try
            {
                // Check if installed
                if (!_extensions.TryGetValue(extensionId, out var extension))
                {
                    _logger.LogWarning($"Extension {extensionId} is not installed");
                    return false;
                }
                
                // Remove extension directory
                if (Directory.Exists(extension.Path))
                {
                    Directory.Delete(extension.Path, true);
                }
                
                // Remove from installed extensions
                _extensions.Remove(extensionId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uninstalling extension: {extensionId}");
                return false;
            }
        }

        public bool EnableExtension(string extensionId)
        {
            if (!_extensions.TryGetValue(extensionId, out var extension))
            {
                return false;
            }
            
            extension.IsEnabled = true;
            return true;
        }

        public bool DisableExtension(string extensionId)
        {
            if (!_extensions.TryGetValue(extensionId, out var extension))
            {
                return false;
            }
            
            extension.IsEnabled = false;
            return true;
        }

        private void LoadInstalledExtensions()
        {
            _extensions.Clear();
            
            try
            {
                // Get all subdirectories in extensions directory
                var extensionDirs = Directory.GetDirectories(_extensionsDirectory);
                
                foreach (var extensionDir in extensionDirs)
                {
                    string packageJsonPath = Path.Combine(extensionDir, "package.json");
                    
                    if (File.Exists(packageJsonPath))
                    {
                        try
                        {
                            string packageJsonContent = File.ReadAllText(packageJsonPath);
                            var packageJson = JsonSerializer.Deserialize<JsonElement>(packageJsonContent);
                            
                            string extensionId = packageJson.GetProperty("name").GetString();
                            string version = packageJson.GetProperty("version").GetString();
                            
                            var extension = new ExtensionInfo
                            {
                                Id = extensionId,
                                Version = version,
                                Path = extensionDir,
                                IsEnabled = true
                            };
                            
                            _extensions[extensionId] = extension;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error loading extension from: {extensionDir}");
                        }
                    }
                }
                
                _logger.LogInformation($"Loaded {_extensions.Count} extensions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading installed extensions");
            }
        }
    }
}