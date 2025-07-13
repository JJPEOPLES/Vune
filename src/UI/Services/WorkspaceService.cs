using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly ILogger<WorkspaceService> _logger;
        private Workspace _currentWorkspace;
        private List<WorkspaceFile> _workspaceFiles = new List<WorkspaceFile>();

        public WorkspaceService(ILogger<WorkspaceService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing WorkspaceService");
        }

        public Workspace OpenWorkspace(string path)
        {
            _logger.LogInformation($"Opening workspace: {path}");
            
            try
            {
                if (!Directory.Exists(path))
                {
                    _logger.LogWarning($"Workspace directory does not exist: {path}");
                    return null;
                }
                
                // Close current workspace if any
                CloseWorkspace();
                
                // Create new workspace
                _currentWorkspace = new Workspace
                {
                    Path = path,
                    Name = Path.GetFileName(path)
                };
                
                // Load workspace files
                LoadWorkspaceFiles(path);
                
                return _currentWorkspace;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error opening workspace: {path}");
                return null;
            }
        }

        public void CloseWorkspace()
        {
            if (_currentWorkspace == null)
            {
                return;
            }
            
            _logger.LogInformation($"Closing workspace: {_currentWorkspace.Path}");
            
            _currentWorkspace = null;
            _workspaceFiles.Clear();
        }

        public Workspace GetCurrentWorkspace()
        {
            return _currentWorkspace;
        }

        public IEnumerable<WorkspaceFile> GetWorkspaceFiles()
        {
            return _workspaceFiles;
        }

        public WorkspaceFile GetFile(string path)
        {
            return _workspaceFiles.FirstOrDefault(f => f.Path == path);
        }
        
        private void LoadWorkspaceFiles(string rootPath)
        {
            _workspaceFiles.Clear();
            
            try
            {
                // Get all files in the workspace
                var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
                
                foreach (var file in files)
                {
                    // Skip hidden files and directories
                    if (IsHiddenPath(file))
                    {
                        continue;
                    }
                    
                    var relativePath = file.Substring(rootPath.Length).TrimStart('\\', '/');
                    
                    _workspaceFiles.Add(new WorkspaceFile
                    {
                        Path = file,
                        Name = Path.GetFileName(file),
                        RelativePath = relativePath,
                        Type = GetFileType(file)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading workspace files from: {rootPath}");
            }
        }
        
        private bool IsHiddenPath(string path)
        {
            var fileName = Path.GetFileName(path);
            if (fileName.StartsWith("."))
            {
                return true;
            }
            
            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            return directoryInfo.Attributes.HasFlag(FileAttributes.Hidden);
        }
        
        private FileType GetFileType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            
            switch (extension)
            {
                case ".cs":
                case ".cpp":
                case ".h":
                case ".hpp":
                case ".js":
                case ".ts":
                case ".py":
                case ".java":
                    return FileType.Code;
                    
                case ".json":
                case ".xml":
                case ".yaml":
                case ".yml":
                case ".toml":
                case ".ini":
                    return FileType.Config;
                    
                case ".md":
                case ".txt":
                case ".rtf":
                    return FileType.Document;
                    
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".svg":
                    return FileType.Image;
                    
                default:
                    return FileType.Other;
            }
        }
    }
}