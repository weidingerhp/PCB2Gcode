using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;
using Xunit;

namespace test..pcb2gcode {
	public class HpglTokenizerTest {
		[Fact]
		void testReadSimpleString() {
			String input = "PU;\nPD;\n";
			MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			MemoryStream outputStream = new MemoryStream();
			StreamReader rdr = new StreamReader(inputStream);
			StreamWriter writer = new StreamWriter(outputStream);

			HpglTokenizer tokenizer = new HpglTokenizer(rdr);

			tokenizer.readNextToken();
		}
	}
}