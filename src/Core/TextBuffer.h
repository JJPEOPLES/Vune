#pragma once

#include "pch.h"

namespace Vune {
    namespace Core {

        // Position in a text document
        struct Position {
            int line;
            int character;
            
            Position() : line(0), character(0) {}
            Position(int line, int character) : line(line), character(character) {}
            
            bool operator==(const Position& other) const {
                return line == other.line && character == other.character;
            }
            
            bool operator!=(const Position& other) const {
                return !(*this == other);
            }
            
            bool operator<(const Position& other) const {
                if (line < other.line) return true;
                if (line > other.line) return false;
                return character < other.character;
            }
            
            bool operator<=(const Position& other) const {
                return *this < other || *this == other;
            }
            
            bool operator>(const Position& other) const {
                return !(*this <= other);
            }
            
            bool operator>=(const Position& other) const {
                return !(*this < other);
            }
        };

        // Range in a text document
        struct Range {
            Position start;
            Position end;
            
            Range() : start(), end() {}
            Range(const Position& start, const Position& end) : start(start), end(end) {}
            Range(int startLine, int startCharacter, int endLine, int endCharacter)
                : start(startLine, startCharacter), end(endLine, endCharacter) {}
            
            bool operator==(const Range& other) const {
                return start == other.start && end == other.end;
            }
            
            bool operator!=(const Range& other) const {
                return !(*this == other);
            }
        };

        // Text edit operation
        struct TextEdit {
            Range range;
            std::string newText;
            
            TextEdit() : range(), newText() {}
            TextEdit(const Range& range, const std::string& newText) : range(range), newText(newText) {}
        };

        // Text document change event
        struct TextDocumentChangeEvent {
            std::vector<TextEdit> changes;
        };

        // Text buffer class for efficient text editing
        class TextBuffer {
        public:
            TextBuffer();
            explicit TextBuffer(const std::string& text);
            ~TextBuffer();
            
            // Get the entire text
            std::string getText() const;
            
            // Get a specific line
            std::string getLine(int line) const;
            
            // Get a range of text
            std::string getTextInRange(const Range& range) const;
            
            // Get the number of lines
            int getLineCount() const;
            
            // Apply edits
            void applyEdit(const TextEdit& edit);
            void applyEdits(const std::vector<TextEdit>& edits);
            
            // Insert text at position
            void insert(const Position& position, const std::string& text);
            
            // Delete text in range
            void remove(const Range& range);
            
            // Replace text in range
            void replace(const Range& range, const std::string& text);
            
            // Get position at offset
            Position positionAt(int offset) const;
            
            // Get offset at position
            int offsetAt(const Position& position) const;
            
            // Check if position is valid
            bool isValidPosition(const Position& position) const;
            
            // Check if range is valid
            bool isValidRange(const Range& range) const;
            
        private:
            // Implementation details
            class Impl;
            std::unique_ptr<Impl> pImpl;
        };

    } // namespace Core
} // namespace Vune