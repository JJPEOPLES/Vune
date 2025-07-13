#include "pch.h"
#include "CoreAPI.h"
#include "ExtensionHost.h"
#include "FileSystem.h"

namespace Vune {
    namespace Core {

        std::string Version::toString() const {
            return std::to_string(major) + "." + std::to_string(minor) + "." + std::to_string(patch);
        }

        class CoreAPI::Impl {
        public:
            Impl() : initialized(false), version{0, 1, 0} {}
            
            bool initialized;
            Version version;
            std::unique_ptr<ExtensionHost> extensionHost;
            std::unique_ptr<FileSystem> fileSystem;
        };

        CoreAPI& CoreAPI::getInstance() {
            static CoreAPI instance;
            return instance;
        }

        CoreAPI::CoreAPI() : pImpl(std::make_unique<Impl>()) {
        }

        CoreAPI::~CoreAPI() {
            shutdown();
        }

        bool CoreAPI::initialize(const std::string& configPath) {
            if (pImpl->initialized) {
                return true; // Already initialized
            }
            
            // Initialize subsystems
            pImpl->fileSystem = std::make_unique<FileSystem>();
            pImpl->extensionHost = std::make_unique<ExtensionHost>();
            
            // Load configuration
            // TODO: Implement configuration loading
            
            pImpl->initialized = true;
            return true;
        }

        Version CoreAPI::getVersion() const {
            return pImpl->version;
        }

        bool CoreAPI::installExtension(const std::string& extensionId, const std::string& version) {
            if (!pImpl->initialized || !pImpl->extensionHost) {
                return false;
            }
            
            return pImpl->extensionHost->installExtension(extensionId, version);
        }

        bool CoreAPI::uninstallExtension(const std::string& extensionId) {
            if (!pImpl->initialized || !pImpl->extensionHost) {
                return false;
            }
            
            return pImpl->extensionHost->uninstallExtension(extensionId);
        }

        std::vector<std::string> CoreAPI::getInstalledExtensions() const {
            if (!pImpl->initialized || !pImpl->extensionHost) {
                return {};
            }
            
            return pImpl->extensionHost->getInstalledExtensions();
        }

        bool CoreAPI::importVSCodeData(const std::string& vscodePath, bool importSettings, bool importExtensions, bool importThemes) {
            if (!pImpl->initialized) {
                return false;
            }
            
            // TODO: Implement VS Code data import
            return false;
        }

        void CoreAPI::shutdown() {
            if (!pImpl->initialized) {
                return;
            }
            
            // Shutdown subsystems in reverse order
            pImpl->extensionHost.reset();
            pImpl->fileSystem.reset();
            
            pImpl->initialized = false;
        }

    } // namespace Core
} // namespace Vune