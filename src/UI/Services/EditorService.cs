using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public class EditorService : IEditorService
    {
        private readonly ILogger<EditorService> _logger;
        private List<Document> _openDocuments = new List<Document>();
        private Document _activeDocument;

        public EditorService(ILogger<EditorService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing EditorService");
        }

        public Document OpenDocument(string path)
        {
            _logger.LogInformation($"Opening document: {path}");
            
            // Check if document is already open
            var existingDocument = _openDocuments.FirstOrDefault(d => d.Path == path);
            if (existingDocument != null)
            {
                SetActiveDocument(existingDocument);
                return existingDocument;
            }
            
            try
            {
                // Create new document
                var document = new Document
                {
                    Path = path,
                    Name = Path.GetFileName(path),
                    Content = File.ReadAllText(path),
                    Language = GetLanguageFromExtension(Path.GetExtension(path)),
                    IsModified = false
                };
                
                _openDocuments.Add(document);
                SetActiveDocument(document);
                
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error opening document: {path}");
                return null;
            }
        }

        public void SaveDocument(Document document)
        {
            if (document == null)
            {
                _logger.LogWarning("Cannot save null document");
                return;
            }
            
            _logger.LogInformation($"Saving document: {document.Path}");
            
            try
            {
                File.WriteAllText(document.Path, document.Content);
                document.IsModified = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving document: {document.Path}");
            }
        }

        public void SaveDocumentAs(Document document, string path)
        {
            if (document == null)
            {
                _logger.LogWarning("Cannot save null document");
                return;
            }
            
            _logger.LogInformation($"Saving document as: {path}");
            
            try
            {
                File.WriteAllText(path, document.Content);
                document.Path = path;
                document.Name = Path.GetFileName(path);
                document.Language = GetLanguageFromExtension(Path.GetExtension(path));
                document.IsModified = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving document as: {path}");
            }
        }

        public void CloseDocument(Document document)
        {
            if (document == null)
            {
                _logger.LogWarning("Cannot close null document");
                return;
            }
            
            _logger.LogInformation($"Closing document: {document.Path}");
            
            _openDocuments.Remove(document);
            
            if (_activeDocument == document)
            {
                _activeDocument = _openDocuments.FirstOrDefault();
            }
        }

        public IEnumerable<Document> GetOpenDocuments()
        {
            return _openDocuments;
        }

        public Document GetActiveDocument()
        {
            return _activeDocument;
        }

        public void SetActiveDocument(Document document)
        {
            if (document == null || !_openDocuments.Contains(document))
            {
                _logger.LogWarning("Cannot set active document to null or non-open document");
                return;
            }
            
            _logger.LogInformation($"Setting active document: {document.Path}");
            
            _activeDocument = document;
        }
        
        private string GetLanguageFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return "plaintext";
            }
            
            extension = extension.ToLowerInvariant();
            
            switch (extension)
            {
                case ".cs":
                    return "csharp";
                case ".cpp":
                case ".h":
                case ".hpp":
                    return "cpp";
                case ".js":
                    return "javascript";
                case ".ts":
                    return "typescript";
                case ".html":
                    return "html";
                case ".css":
                    return "css";
                case ".json":
                    return "json";
                case ".xml":
                    return "xml";
                case ".md":
                    return "markdown";
                case ".py":
                    return "python";
                case ".java":
                    return "java";
                default:
                    return "plaintext";
            }
        }
    }
}