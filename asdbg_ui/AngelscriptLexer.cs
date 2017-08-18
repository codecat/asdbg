using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace asdbg_ui
{
	public class AngelscriptLexer
	{
		public const int StyleDefault = 0;
		public const int StyleIdentifier = 1;
		public const int StyleConstant = 2;
		public const int StyleString = 3;
		public const int StyleStorageModifier = 4;
		public const int StyleStorageType = 5;
		public const int StyleVariable = 6;
		public const int StyleControl = 7;

		private const int STATE_UNKNOWN = 0;
		private const int STATE_IDENTIFIER = 1;
		private const int STATE_NUMBER = 2;
		private const int STATE_STRING = 3;

		private HashSet<string> keywordsStorageModifier;
		private HashSet<string> keywordsStorageType;
		private HashSet<string> keywordsConstant;
		private HashSet<string> keywordsVariable;
		private HashSet<string> keywordsControl;

		public void Style(Scintilla scintilla, int startPos, int endPos)
		{
			// Back up to the line start
			var line = scintilla.LineFromPosition(startPos);
			startPos = scintilla.Lines[line].Position;

			var length = 0;
			var state = STATE_UNKNOWN;

			// Start styling
			scintilla.StartStyling(startPos);
			while (startPos < endPos) {
				var c = (char)scintilla.GetCharAt(startPos);

				REPROCESS:
				switch (state) {
					case STATE_UNKNOWN:
						if (c == '"') {
							// Start of "string"
							scintilla.SetStyling(1, StyleString);
							state = STATE_STRING;
						} else if (Char.IsDigit(c)) {
							state = STATE_NUMBER;
							goto REPROCESS;
						} else if (Char.IsLetter(c)) {
							state = STATE_IDENTIFIER;
							goto REPROCESS;
						} else {
							// Everything else
							scintilla.SetStyling(1, StyleDefault);
						}
						break;

					case STATE_STRING:
						if (c == '"') {
							length++;
							scintilla.SetStyling(length, StyleString);
							length = 0;
							state = STATE_UNKNOWN;
						} else {
							length++;
						}
						break;

					case STATE_NUMBER:
						if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || c == 'x') {
							length++;
						} else {
							scintilla.SetStyling(length, StyleConstant);
							length = 0;
							state = STATE_UNKNOWN;
							goto REPROCESS;
						}
						break;

					case STATE_IDENTIFIER:
						if (Char.IsLetterOrDigit(c)) {
							length++;
						} else {
							var style = StyleIdentifier;
							var identifier = scintilla.GetTextRange(startPos - length, length);

							if (keywordsStorageModifier.Contains(identifier)) {
								style = StyleStorageModifier;
							} else if (keywordsStorageType.Contains(identifier)) {
								style = StyleStorageType;
							} else if (keywordsConstant.Contains(identifier)) {
								style = StyleConstant;
							} else if (keywordsVariable.Contains(identifier)) {
								style = StyleVariable;
							} else if (keywordsControl.Contains(identifier)) {
								style = StyleControl;
							}

							scintilla.SetStyling(length, style);
							length = 0;
							state = STATE_UNKNOWN;
							goto REPROCESS;
						}
						break;
				}

				startPos++;
			}
		}

		public AngelscriptLexer()
		{
			keywordsStorageModifier = new HashSet<string>(new[] {
				"get", "in", "inout", "out", "override", "set", "private",
				"public", "const", "default", "final", "shared", "mixin"
			});
			keywordsStorageType = new HashSet<string>(new[] {
				"enum", "void", "bool", "typedef", "funcdef", "int", "int8",
				"int16", "int32", "int64", "uint", "uint8", "uint16", "uint32",
				"uint64", "string", "ref", "array", "double", "float", "auto",
				"dictionary"
			});
			keywordsConstant = new HashSet<string>(new[] {
				"null", "true", "false"
			});
			keywordsVariable = new HashSet<string>(new[] {
				"this", "super"
			});
			keywordsControl = new HashSet<string>(new[] {
				"for", "in", "break", "continue", "while", "do", "return", "if",
				"else", "case", "switch", "namespace"
			});
		}
	}
}
