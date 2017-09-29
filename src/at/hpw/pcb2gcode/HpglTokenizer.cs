using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using at.hpw.pcb2gcode.parser;

namespace at {
namespace hpw {
namespace pcb2gcode {

class HpglTokenizer {
    private StreamReader input;
    private int line;
    public HpglTokenizer(StreamReader input) {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        this.input = input;
        line = 1;
    }

    private HpglToken readCommand() {
        char [] ch = new char[2];
        int i=0;
        while ( !input.EndOfStream  && (i = i + input.Read(ch,i,2-i)) < 2 );

        string retVal = new string(ch, 0, 2);

        try {
            return (HpglToken) Enum.Parse(typeof(HpglToken), retVal);
        } catch (Exception e) {
            throw new ParserException(retVal + " is not a valid command on line " + line, e);
        }
    }

    // TODO implement a readParamlist for the format of <double>,<double>, <double> .... ,<double>;
    // do this in a whitespace insensitive manner. Commas are US-Style
    public object readNextToken() {
        ignoreWhitspace();
        if (input.EndOfStream) return HpglToken.EOF;

        char ch = (char) input.Peek();
        if (((ch >= '0') && (ch <= '9')) || (ch == '.') || (ch == '-') || (ch == '+')) {
            return readDouble();
        }
        if (ch == ',') {
            input.Read();
            return HpglToken.COMMA;
        }
        if (ch == ';') {
            input.Read();
            return HpglToken.SEMICOLON;
        }
        return readCommand();
    }

    private double readDouble() {
        double afterComma=0;
        double result = 0;
        bool isNegative = false;
        bool foundOne = false;
        while (!input.EndOfStream) {
            char ch = (char) input.Peek();
            if (ch == '+') {
                input.Read();
                if (foundOne) {
                    throw new ParserException("+ sign has to be before any number " + line);
                }
                continue;
            }
            if (ch == '-') {
                input.Read();
                if (foundOne) {
                    throw new ParserException("- sign has to be before any number " + line);
                }
                isNegative = true;
                continue;
            }
            if (ch == '.') {
                if (afterComma == 0) {
                    input.Read();
                    afterComma = 1;
                    continue;
                } else { // second comma is not allowed
                    throw new ParserException("Failed to Parse second comma on line " + line);
                }
            }
            if (ch>= '0' && ch <='9') {
                input.Read();
                foundOne = true;
                if (afterComma == 0) {
                    result = result * 10 + (ch - '0');
                } else {
                    afterComma = afterComma / 10;
                    result = result + (afterComma * ((double) (ch - '0')));
                }
                continue;
            }
            break;
        }

        if (!foundOne) throw new ParserException("Expected Numeric Value on Line " + line);

        if (isNegative) result = -result;
        return result;
    }

    internal void throwParserException(string message) {
        throw new ParserException(message + " at line " + line);
    }

    private void ignoreWhitspace() {
        while (!input.EndOfStream) {
            char ch = (char) input.Peek();
            switch(ch) {
                case '\r':
                case '\n':
                    line = line + 1;
                    break;
                case ' ':
                case '\t':
                    break;
                default:
                    return;
            }
            input.Read(); // take char from stream;
        }

    }
}

}
}
}