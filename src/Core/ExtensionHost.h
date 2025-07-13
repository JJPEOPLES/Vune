#pragma once

#include "pch.h"

namespace Vune {
    namespace Core {

        class ExtensionHost {
        public:
            ExtensionHost();
            ~ExtensionHost();

            // Extension management
            bool installExtension(const std::string& extensionId, const std::string& version);
            bool uninstallExtension(const std::string& extensionId);
            std::vector<std::string> getInstalledExtensions() const;
            
            // VS Code extension compatibility
            bool isVSCodeExtensionCompatible(const std::string& extensionId) const;
            
            // Extension execution
            bool activateExtension(const std::string& extensionId);
            void deactivateExtension(const std::string& extensionId);
            
        private:
            // Implementation details
            class Impl;
            std::unique_ptr<Impl> pImpl;
        };

    } // namespace Core
} // namespace Vune