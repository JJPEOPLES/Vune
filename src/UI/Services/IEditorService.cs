using System.Collections.Generic;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public interface IEditorService
    {
        void Initialize();
        Document OpenDocument(string path);
        void SaveDocument(Document document);
        void SaveDocumentAs(Document document, string path);
        void CloseDocument(Document document);
        IEnumerable<Document> GetOpenDocuments();
        Document GetActiveDocument();
        void SetActiveDocument(Document document);
    }
}