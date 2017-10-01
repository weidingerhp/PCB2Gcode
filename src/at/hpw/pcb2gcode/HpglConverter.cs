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
		public bool InitPosition0 { get; set; }
		public bool FlipXY { get; set; }

		public HpglConverter() {
			NumericFormat = new CultureInfo("en-US").NumberFormat;
			XFactor = -0.00762; // initial Values
			YFactor = -0.00762; // initial Values
			InitPosition0 = true;
			FlipXY = false;
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
					case HpglToken.AA:
						state.LastMovementAbsolute = true;
						token = MakeArc(tokenizer, state, oStream);
						break;
					case HpglToken.IN:
						token = tokenizer.readNextToken();
						continue;
					case HpglToken.IP: 
						setScalingAndOffset(tokenizer);
						token = tokenizer.readNextToken();
						continue;
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

			if (InitPosition0) {
				state.LastMovementAbsolute = true;  //assuming everything we do is absolute
				convertMovementToGcode(state, 0, 0, oStream);
			}
		}

        private void setScalingAndOffset(HpglTokenizer tokenizer)
        {
			double p1x = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double p1y = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double p2x = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double p2y = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.SEMICOLON);

			// expect scaling directly after this
			expectHpglToken(tokenizer, HpglToken.SC);
			double xmin = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double xmax = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double ymin = expectNumericToken(tokenizer);
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double ymax = expectNumericToken(tokenizer);

			// correct scaling factors
			XFactor *= (xmax-xmin) / (p2x-p1x);
			YFactor *= (ymax-ymin) / (p2y-p1y);
        }

        private HpglToken expectHpglToken(HpglTokenizer tokenizer) {
			object currentToken = tokenizer.readNextToken();
			// if (!(tokenizer.readNextToken() is HpglToken)) {
			// 	tokenizer.throwParserException("Expected HpglToken but got " + currentToken);
			// }
			return (HpglToken) currentToken;
		}

		private void expectHpglToken(HpglTokenizer tokenizer, HpglToken token) {
			HpglToken currentToken = expectHpglToken(tokenizer);
			if (currentToken != token) {
				tokenizer.throwParserException("Expected " + Enum.GetName(typeof(HpglToken), token) + " but got " + Enum.GetName(typeof(HpglToken), currentToken));
			}
		}

		private double expectNumericToken(HpglTokenizer tokenizer, string message = "Expected Numeric Value") {
			object currentToken = tokenizer.readNextToken();
			if (!(currentToken is double)) {
				tokenizer.throwParserException(message + "; got: " + currentToken);
			}
			return (double) currentToken;	
		}

        private object MakeArc(HpglTokenizer tokenizer, HpglState state, StreamWriter oStream)
        {
			// AA pivotX, pivotY, angle [,granulatrity];
			double xcenter = expectNumericToken(tokenizer, "Expected x-value");
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double ycenter = expectNumericToken(tokenizer, "Expected y-value");
			expectHpglToken(tokenizer, HpglToken.COMMA);
			double angle = expectNumericToken(tokenizer, "Expected angle");
			HpglToken nextHpgl = expectHpglToken(tokenizer);
			if (nextHpgl == HpglToken.SEMICOLON) return tokenizer.readNextToken();
			if (nextHpgl == HpglToken.COMMA) {
				expectNumericToken(tokenizer, "Expected optional granularity");
				nextHpgl = expectHpglToken(tokenizer);
			}
			expectHpglToken(tokenizer, HpglToken.SEMICOLON);

			// do the arc calculation
			// TODO:
			// use G02 (clockwise, negative angle) or G03(counterclockwise, positive angle)
			// from current point using x and y from params above
			// see http://www.devenezia.com/docs/HP/index.html?1901 (HPGL)
			// and http://s3.cnccookbook.com/CCCNCGCodeArcsG02G03.htm (GCODE)
			// for info
		
			if (nextHpgl == HpglToken.EOF) return nextHpgl;
			return tokenizer.readNextToken();
        }

        private object MakeMovements(HpglTokenizer tokenizer, HpglState state, StreamWriter oStream) {
			object currentToken = tokenizer.readNextToken();
			HpglToken nextHpgl;
			while (currentToken is double) {
				double xval = (double)currentToken;
				expectHpglToken(tokenizer, HpglToken.COMMA);
				double yval = expectNumericToken(tokenizer, "Expected X and Y value");

				if (state.LastMovementAbsolute) {
					convertAbsoluteMovement(state, xval, yval, oStream);
				} else {
					convertRelativeMovement(state, xval, yval, oStream);
				}

				nextHpgl = expectHpglToken(tokenizer);
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

		private void convertPD(HpglState state, StreamWriter oStream) {
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

		private void convertPU(HpglState state, StreamWriter oStream) {
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
				oStream.Write("G0 ");
			} else {
				oStream.Write("G1 ");
			}
			convertXCoord(state, FlipXY ? y : x, oStream);
			oStream.Write(" ");
			convertYCoord(state, FlipXY ? x : y, oStream);
			oStream.Write("\n");
		}
	}

}
