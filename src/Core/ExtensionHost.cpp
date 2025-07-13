#include "pch.h"
#include "ExtensionHost.h"

namespace Vune {
    namespace Core {

        class ExtensionHost::Impl {
        public:
            Impl() {}
            
            // Store installed extensions
            std::unordered_map<std::string, std::string> installedExtensions; // extensionId -> version
            
            // Store active extensions
            std::unordered_set<std::string> activeExtensions;
        };

        ExtensionHost::ExtensionHost() : pImpl(std::make_unique<Impl>()) {
        }

        ExtensionHost::~ExtensionHost() {
            // Deactivate all extensions
            for (const auto& extensionId : pImpl->activeExtensions) {
                deactivateExtension(extensionId);
            }
        }

        bool ExtensionHost::installExtension(const std::string& extensionId, const std::string& version) {
            // TODO: Implement actual extension installation
            pImpl->installedExtensions[extensionId] = version;
            return true;
        }

        bool ExtensionHost::uninstallExtension(const std::string& extensionId) {
            // Deactivate if active
            if (pImpl->activeExtensions.find(extensionId) != pImpl->activeExtensions.end()) {
                deactivateExtension(extensionId);
            }
            
            // Remove from installed extensions
            auto it = pImpl->installedExtensions.find(extensionId);
            if (it != pImpl->installedExtensions.end()) {
                pImpl->installedExtensions.erase(it);
                return true;
            }
            
            return false;
        }

        std::vector<std::string> ExtensionHost::getInstalledExtensions() const {
            std::vector<std::string> result;
            result.reserve(pImpl->installedExtensions.size());
            
            for (const auto& pair : pImpl->installedExtensions) {
                result.push_back(pair.first);
            }
            
            return result;
        }

        bool ExtensionHost::isVSCodeExtensionCompatible(const std::string& extensionId) const {
            // TODO: Implement VS Code extension compatibility check
            return true;
        }

        bool ExtensionHost::activateExtension(const std::string& extensionId) {
            // Check if extension is installed
            if (pImpl->installedExtensions.find(extensionId) == pImpl->installedExtensions.end()) {
                return false;
            }
            
            // TODO: Implement actual extension activation
            pImpl->activeExtensions.insert(extensionId);
            return true;
        }

        void ExtensionHost::deactivateExtension(const std::string& extensionId) {
            // TODO: Implement actual extension deactivation
            pImpl->activeExtensions.erase(extensionId);
        }

    } // namespace Core
} // namespace Vune