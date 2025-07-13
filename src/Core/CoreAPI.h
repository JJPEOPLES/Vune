#pragma once

#include "pch.h"

#ifdef CORE_EXPORTS
#define CORE_API __declspec(dllexport)
#else
#define CORE_API __declspec(dllimport)
#endif

namespace Vune {
    namespace Core {

        // Version information
        struct CORE_API Version {
            int major;
            int minor;
            int patch;
            std::string toString() const;
        };

        // Core API class - main interface for the C# UI to interact with the C++ core
        class CORE_API CoreAPI {
        public:
            static CoreAPI& getInstance();
            
            // Initialize the core with configuration
            bool initialize(const std::string& configPath);
            
            // Get version information
            Version getVersion() const;
            
            // Extension related methods
            bool installExtension(const std::string& extensionId, const std::string& version);
            bool uninstallExtension(const std::string& extensionId);
            std::vector<std::string> getInstalledExtensions() const;
            
            // VS Code data import
            bool importVSCodeData(const std::string& vscodePath, bool importSettings, bool importExtensions, bool importThemes);
            
            // Cleanup and shutdown
            void shutdown();
            
        private:
            CoreAPI();
            ~CoreAPI();
            
            // Prevent copying
            CoreAPI(const CoreAPI&) = delete;
            CoreAPI& operator=(const CoreAPI&) = delete;
            
            // Implementation details
            class Impl;
            std::unique_ptr<Impl> pImpl;
        };

    } // namespace Core
} // namespace Vune