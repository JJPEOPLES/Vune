#pragma once

#include "pch.h"

namespace Vune {
    namespace Core {

        class FileSystem {
        public:
            FileSystem();
            ~FileSystem();

            // File operations
            bool fileExists(const std::string& path) const;
            std::string readTextFile(const std::string& path) const;
            bool writeTextFile(const std::string& path, const std::string& content);
            bool deleteFile(const std::string& path);
            
            // Directory operations
            bool directoryExists(const std::string& path) const;
            bool createDirectory(const std::string& path);
            bool deleteDirectory(const std::string& path, bool recursive = false);
            std::vector<std::string> listFiles(const std::string& directory, const std::string& pattern = "*") const;
            std::vector<std::string> listDirectories(const std::string& directory) const;
            
            // Path operations
            std::string getAbsolutePath(const std::string& path) const;
            std::string combinePaths(const std::string& path1, const std::string& path2) const;
            std::string getFileName(const std::string& path) const;
            std::string getDirectoryName(const std::string& path) const;
            std::string getExtension(const std::string& path) const;
            
        private:
            // Implementation details
            class Impl;
            std::unique_ptr<Impl> pImpl;
        };

    } // namespace Core
} // namespace Vune