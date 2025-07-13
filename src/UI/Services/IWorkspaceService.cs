using System.Collections.Generic;
using Vune.UI.Models;

namespace Vune.UI.Services
{
    public interface IWorkspaceService
    {
        void Initialize();
        Workspace OpenWorkspace(string path);
        void CloseWorkspace();
        Workspace GetCurrentWorkspace();
        IEnumerable<WorkspaceFile> GetWorkspaceFiles();
        WorkspaceFile GetFile(string path);
    }
}