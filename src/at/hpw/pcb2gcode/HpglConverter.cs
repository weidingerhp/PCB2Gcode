using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using at.hpw.pcb2gcode.parser;

namespace at.hpw.pcb2gcode {
	public class HpglConverter {
		internal NumberFormatInfo NumericFormat { get; private set; }
		public double XFactor { get; private set; }
		public double YFactor { get; private set; }
		public double ZFactor { get; private set; }

		public HpglConverter() {
			NumericFormat = new CultureInfo("en-US").NumberFormat;
			XFactor = 0.00762; // initial Values
			YFactor = 0.00762; // initial Values
		}

		public void ConvertHpgl(StreamReader iStream, StreamWriter oStream) {
			var tokenizer = new HpglTokenizer(iStream);
			HpglState state = new HpglState(0, 0, HpglState.PenState.PenUp, this);
			object token = tokenizer.readNextToken();
			while ((HpglToken)(token) != HpglToken.EOF) {
				switch ((HpglToken)token) {
					case HpglToken.SEMICOLON: // just ignore this 
						token = tokenizer.readNextToken();
						continue;
					case HpglToken.PD:
						convertPD(state, oStream);
						token = MakeMovements(tokenizer, state, oStream);
						break;
					case HpglToken.PU:
						convertPU(state, oStream);
						token = MakeMovements(tokenizer, state, oStream);
						break;
					case HpglToken.PA:
						state.LastMovementAbsolute = true;
						token = MakeMovements(tokenizer, state, oStream);
						break;
					case HpglToken.PR:
						state.LastMovementAbsolute = false;
						token = MakeMovements(tokenizer, state, oStream);
						break;
					case HpglToken.SP:
						if (!(tokenizer.readNextToken() is double)) {
							tokenizer.throwParserException("SP (select pen) must be followed by pen number");
						}
						if (!((HpglToken)tokenizer.readNextToken() == HpglToken.SEMICOLON)) {
							tokenizer.throwParserException("SP (select pen) not correctly terminated");
						}
						token = tokenizer.readNextToken();
						break;
					default:
						Console.Out.Write("Unknown token " + token);
						token = tokenizer.readNextToken();
						while (!(token is HpglToken) || (((HpglToken)token) != HpglToken.SEMICOLON && ((HpglToken)token) != HpglToken.EOF)) {
							Console.Out.Write(" " + token);
							token = tokenizer.readNextToken();
						}
						Console.Out.WriteLine();
						break;
				}
			}

		}

		private object MakeMovements(HpglTokenizer tokenizer, HpglState state, StreamWriter oStream) {
			object currentToken = tokenizer.readNextToken();
			HpglToken nextHpgl;
			while (currentToken is double) {
				double xval = (double)currentToken;
				if (((HpglToken)tokenizer.readNextToken()) != HpglToken.COMMA) {
					tokenizer.throwParserException("Expected ','");
				}
				currentToken = tokenizer.readNextToken();
				if (!(currentToken is double)) {
					tokenizer.throwParserException("Expected X and Y value");
				}
				double yval = (double)currentToken;

				if (state.LastMovementAbsolute) {
					convertAbsoluteMovement(state, xval, yval, oStream);
				} else {
					convertRelativeMovement(state, xval, yval, oStream);
				}

				nextHpgl = (HpglToken)tokenizer.readNextToken();
				if (nextHpgl == HpglToken.SEMICOLON) return tokenizer.readNextToken();
				if (nextHpgl == HpglToken.EOF) return nextHpgl;
				if (nextHpgl == HpglToken.COMMA) {
					currentToken = tokenizer.readNextToken();
					continue;
				}

				tokenizer.throwParserException("Expected ',' OR ';' ");
			}

			return currentToken;
		}

		void convertXCoord(HpglState state, double x, StreamWriter oStream) {
			x = x * XFactor;
			oStream.Write(string.Format(NumericFormat, "X{0}", x));
		}

		void convertYCoord(HpglState state, double y, StreamWriter oStream) {
			y = y * YFactor;
			oStream.Write(string.Format(NumericFormat, "Y{0}", y));
		}

		void convertZCoord(HpglState state, double y, StreamWriter oStream) {
			y = y * ZFactor;
			oStream.Write(string.Format(NumericFormat, "Z{0}", y));
		}

		internal void convertPD(HpglState state, StreamWriter oStream) {
			StringBuilder builder = new StringBuilder();

			oStream.Write("M3 S90\n");
			state.PenPosition = HpglState.PenState.PenDown;

			// TODO PD can also have some movements after doing the PD
			// if (state.LastMovementAbsolute)
			// {
			//     convertPA(state, p, oStream);
			// }
			// else
			// {
			//     convertPR(state, p, oStream);
			// }
		}

		internal void convertPU(HpglState state, StreamWriter oStream) {
			StringBuilder builder = new StringBuilder();

			oStream.Write("M5\n");
			state.PenPosition = HpglState.PenState.PenUp;
		}


		private void convertAbsoluteMovement(HpglState state, double x, double y, StreamWriter oStream) {
			convertMovementToGcode(state, x - state.XPos, y - state.YPos, oStream);
			state.XPos = x;
			state.YPos = y;
		}

		private void convertRelativeMovement(HpglState state, double x, double y, StreamWriter oStream) {
			convertMovementToGcode(state, x, y, oStream);
			state.XPos += x;
			state.YPos += y;
		}

		private void convertMovementToGcode(HpglState state, double x, double y, StreamWriter oStream) {
			if (state.PenPosition == HpglState.PenState.PenUp) {
				oStream.Write("G1 ");
			} else {
				oStream.Write("G1 ");
			}
			convertXCoord(state, x, oStream);
			oStream.Write(" ");
			convertYCoord(state, y, oStream);
			oStream.Write("\n");
		}
	}

}
