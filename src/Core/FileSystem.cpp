#include "pch.h"
#include "FileSystem.h"
#include <fstream>
#include <filesystem>

namespace fs = std::filesystem;

namespace Vune {
    namespace Core {

        class FileSystem::Impl {
        public:
            Impl() {}
        };

        FileSystem::FileSystem() : pImpl(std::make_unique<Impl>()) {
        }

        FileSystem::~FileSystem() {
        }

        bool FileSystem::fileExists(const std::string& path) const {
            return fs::exists(path) && fs::is_regular_file(path);
        }

        std::string FileSystem::readTextFile(const std::string& path) const {
            if (!fileExists(path)) {
                return "";
            }
            
            std::ifstream file(path);
            if (!file.is_open()) {
                return "";
            }
            
            std::string content((std::istreambuf_iterator<char>(file)), std::istreambuf_iterator<char>());
            return content;
        }

        bool FileSystem::writeTextFile(const std::string& path, const std::string& content) {
            try {
                std::ofstream file(path);
                if (!file.is_open()) {
                    return false;
                }
                
                file << content;
                return true;
            }
            catch (const std::exception&) {
                return false;
            }
        }

        bool FileSystem::deleteFile(const std::string& path) {
            try {
                return fs::remove(path);
            }
            catch (const std::exception&) {
                return false;
            }
        }

        bool FileSystem::directoryExists(const std::string& path) const {
            return fs::exists(path) && fs::is_directory(path);
        }

        bool FileSystem::createDirectory(const std::string& path) {
            try {
                return fs::create_directories(path);
            }
            catch (const std::exception&) {
                return false;
            }
        }

        bool FileSystem::deleteDirectory(const std::string& path, bool recursive) {
            try {
                if (recursive) {
                    return fs::remove_all(path) > 0;
                }
                else {
                    return fs::remove(path);
                }
            }
            catch (const std::exception&) {
                return false;
            }
        }

        std::vector<std::string> FileSystem::listFiles(const std::string& directory, const std::string& pattern) const {
            std::vector<std::string> result;
            
            if (!directoryExists(directory)) {
                return result;
            }
            
            try {
                for (const auto& entry : fs::directory_iterator(directory)) {
                    if (entry.is_regular_file()) {
                        // TODO: Implement pattern matching
                        result.push_back(entry.path().string());
                    }
                }
            }
            catch (const std::exception&) {
                // Ignore errors
            }
            
            return result;
        }

        std::vector<std::string> FileSystem::listDirectories(const std::string& directory) const {
            std::vector<std::string> result;
            
            if (!directoryExists(directory)) {
                return result;
            }
            
            try {
                for (const auto& entry : fs::directory_iterator(directory)) {
                    if (entry.is_directory()) {
                        result.push_back(entry.path().string());
                    }
                }
            }
            catch (const std::exception&) {
                // Ignore errors
            }
            
            return result;
        }

        std::string FileSystem::getAbsolutePath(const std::string& path) const {
            try {
                return fs::absolute(path).string();
            }
            catch (const std::exception&) {
                return path;
            }
        }

        std::string FileSystem::combinePaths(const std::string& path1, const std::string& path2) const {
            try {
                fs::path result = path1;
                result /= path2;
                return result.string();
            }
            catch (const std::exception&) {
                return path1 + "/" + path2;
            }
        }

        std::string FileSystem::getFileName(const std::string& path) const {
            try {
                return fs::path(path).filename().string();
            }
            catch (const std::exception&) {
                return path;
            }
        }

        std::string FileSystem::getDirectoryName(const std::string& path) const {
            try {
                return fs::path(path).parent_path().string();
            }
            catch (const std::exception&) {
                return path;
            }
        }

        std::string FileSystem::getExtension(const std::string& path) const {
            try {
                return fs::path(path).extension().string();
            }
            catch (const std::exception&) {
                return "";
            }
        }

    } // namespace Core
} // namespace Vune