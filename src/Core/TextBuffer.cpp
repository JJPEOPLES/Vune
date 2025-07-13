#include "pch.h"
#include "TextBuffer.h"
#include <algorithm>
#include <sstream>

namespace Vune {
    namespace Core {

        class TextBuffer::Impl {
        public:
            explicit Impl(const std::string& text) {
                setText(text);
            }
            
            void setText(const std::string& text) {
                lines.clear();
                std::istringstream stream(text);
                std::string line;
                
                while (std::getline(stream, line)) {
                    // Check if the line ends with \r (Windows line ending in a \r\n sequence)
                    if (!line.empty() && line.back() == '\r') {
                        line.pop_back();
                    }
                    lines.push_back(line);
                }
                
                // Handle the case where the last line doesn't end with a newline
                if (!text.empty() && (text.back() == '\n' || text.back() == '\r')) {
                    lines.push_back("");
                }
            }
            
            std::string getText() const {
                std::ostringstream result;
                for (size_t i = 0; i < lines.size(); ++i) {
                    result << lines[i];
                    if (i < lines.size() - 1) {
                        result << "\n";
                    }
                }
                return result.str();
            }
            
            std::vector<std::string> lines;
        };

        TextBuffer::TextBuffer() : pImpl(std::make_unique<Impl>("")) {
        }

        TextBuffer::TextBuffer(const std::string& text) : pImpl(std::make_unique<Impl>(text)) {
        }

        TextBuffer::~TextBuffer() {
        }

        std::string TextBuffer::getText() const {
            return pImpl->getText();
        }

        std::string TextBuffer::getLine(int line) const {
            if (line < 0 || line >= getLineCount()) {
                return "";
            }
            
            return pImpl->lines[line];
        }

        std::string TextBuffer::getTextInRange(const Range& range) const {
            if (!isValidRange(range)) {
                return "";
            }
            
            if (range.start.line == range.end.line) {
                // Single line
                const std::string& line = pImpl->lines[range.start.line];
                return line.substr(range.start.character, range.end.character - range.start.character);
            }
            
            // Multiple lines
            std::ostringstream result;
            
            // First line
            const std::string& firstLine = pImpl->lines[range.start.line];
            result << firstLine.substr(range.start.character);
            
            // Middle lines
            for (int i = range.start.line + 1; i < range.end.line; ++i) {
                result << "\n" << pImpl->lines[i];
            }
            
            // Last line
            const std::string& lastLine = pImpl->lines[range.end.line];
            result << "\n" << lastLine.substr(0, range.end.character);
            
            return result.str();
        }

        int TextBuffer::getLineCount() const {
            return static_cast<int>(pImpl->lines.size());
        }

        void TextBuffer::applyEdit(const TextEdit& edit) {
            if (!isValidRange(edit.range)) {
                return;
            }
            
            replace(edit.range, edit.newText);
        }

        void TextBuffer::applyEdits(const std::vector<TextEdit>& edits) {
            // Sort edits in reverse order to avoid position changes
            std::vector<TextEdit> sortedEdits = edits;
            std::sort(sortedEdits.begin(), sortedEdits.end(), [](const TextEdit& a, const TextEdit& b) {
                if (a.range.start.line != b.range.start.line) {
                    return a.range.start.line > b.range.start.line;
                }
                if (a.range.start.character != b.range.start.character) {
                    return a.range.start.character > b.range.start.character;
                }
                if (a.range.end.line != b.range.end.line) {
                    return a.range.end.line < b.range.end.line;
                }
                return a.range.end.character < b.range.end.character;
            });
            
            for (const auto& edit : sortedEdits) {
                applyEdit(edit);
            }
        }

        void TextBuffer::insert(const Position& position, const std::string& text) {
            if (!isValidPosition(position)) {
                return;
            }
            
            if (text.empty()) {
                return;
            }
            
            // Split the text into lines
            std::vector<std::string> newLines;
            std::istringstream stream(text);
            std::string line;
            
            while (std::getline(stream, line)) {
                // Check if the line ends with \r (Windows line ending in a \r\n sequence)
                if (!line.empty() && line.back() == '\r') {
                    line.pop_back();
                }
                newLines.push_back(line);
            }
            
            // Handle the case where the last line doesn't end with a newline
            if (!text.empty() && (text.back() == '\n' || text.back() == '\r')) {
                newLines.push_back("");
            }
            
            if (newLines.empty()) {
                return;
            }
            
            // Insert the text
            if (newLines.size() == 1) {
                // Single line insert
                std::string& currentLine = pImpl->lines[position.line];
                currentLine.insert(position.character, newLines[0]);
            }
            else {
                // Multi-line insert
                std::string& currentLine = pImpl->lines[position.line];
                std::string beforeInsert = currentLine.substr(0, position.character);
                std::string afterInsert = currentLine.substr(position.character);
                
                // Update the first line
                pImpl->lines[position.line] = beforeInsert + newLines[0];
                
                // Insert the middle lines
                pImpl->lines.insert(pImpl->lines.begin() + position.line + 1, newLines.begin() + 1, newLines.end() - 1);
                
                // Insert the last line with the remainder of the original line
                pImpl->lines.insert(pImpl->lines.begin() + position.line + newLines.size() - 1, newLines.back() + afterInsert);
            }
        }

        void TextBuffer::remove(const Range& range) {
            replace(range, "");
        }

        void TextBuffer::replace(const Range& range, const std::string& text) {
            if (!isValidRange(range)) {
                return;
            }
            
            // Remove the text in the range
            if (range.start.line == range.end.line) {
                // Single line
                std::string& line = pImpl->lines[range.start.line];
                line.erase(range.start.character, range.end.character - range.start.character);
            }
            else {
                // Multiple lines
                std::string& firstLine = pImpl->lines[range.start.line];
                std::string& lastLine = pImpl->lines[range.end.line];
                
                // Keep the part of the first line before the range start
                std::string newLine = firstLine.substr(0, range.start.character);
                
                // Append the part of the last line after the range end
                newLine += lastLine.substr(range.end.character);
                
                // Update the first line
                firstLine = newLine;
                
                // Remove the lines in between
                pImpl->lines.erase(pImpl->lines.begin() + range.start.line + 1, pImpl->lines.begin() + range.end.line + 1);
            }
            
            // Insert the new text
            if (!text.empty()) {
                insert(range.start, text);
            }
        }

        Position TextBuffer::positionAt(int offset) const {
            if (offset <= 0) {
                return Position(0, 0);
            }
            
            int currentOffset = 0;
            for (int i = 0; i < getLineCount(); ++i) {
                const std::string& line = pImpl->lines[i];
                int lineLength = static_cast<int>(line.length());
                
                if (currentOffset + lineLength >= offset) {
                    return Position(i, offset - currentOffset);
                }
                
                currentOffset += lineLength + 1; // +1 for the newline
            }
            
            // If we get here, the offset is beyond the end of the document
            int lastLine = getLineCount() - 1;
            return Position(lastLine, static_cast<int>(pImpl->lines[lastLine].length()));
        }

        int TextBuffer::offsetAt(const Position& position) const {
            if (!isValidPosition(position)) {
                return 0;
            }
            
            int offset = 0;
            for (int i = 0; i < position.line; ++i) {
                offset += static_cast<int>(pImpl->lines[i].length()) + 1; // +1 for the newline
            }
            
            offset += std::min(position.character, static_cast<int>(pImpl->lines[position.line].length()));
            return offset;
        }

        bool TextBuffer::isValidPosition(const Position& position) const {
            if (position.line < 0 || position.line >= getLineCount()) {
                return false;
            }
            
            if (position.character < 0) {
                return false;
            }
            
            // Allow position at the end of the line
            return position.character <= static_cast<int>(pImpl->lines[position.line].length());
        }

        bool TextBuffer::isValidRange(const Range& range) const {
            return isValidPosition(range.start) && isValidPosition(range.end) && (range.start <= range.end);
        }

    } // namespace Core
} // namespace Vune