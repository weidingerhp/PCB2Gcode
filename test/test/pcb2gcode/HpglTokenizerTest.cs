using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;
using at.hpw.pcb2gcode.parser;
using Xunit;

namespace test.pcb2gcode {
	public class HpglTokenizerTest {
		[Fact]
		void testReadSimplePUPD() {
			String input = "PU;\nPD;\n";
			MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			MemoryStream outputStream = new MemoryStream();
			StreamReader rdr = new StreamReader(inputStream);
			StreamWriter writer = new StreamWriter(outputStream);

			HpglTokenizer tokenizer = new HpglTokenizer(rdr);

			Assert.Equal(HpglToken.PU, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.SEMICOLON, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.PD, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.SEMICOLON, (HpglToken) tokenizer.readNextToken());
		}
	}
}