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
			Assert.Equal(HpglToken.EOF, (HpglToken) tokenizer.readNextToken());
		}

		[Fact]
		void testReadSimpleINIPSC() {
			String input = "IN; IP 0,0,100,10.03; SC 0,100,0,-100;";
			MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			MemoryStream outputStream = new MemoryStream();
			StreamReader rdr = new StreamReader(inputStream);
			StreamWriter writer = new StreamWriter(outputStream);

			HpglTokenizer tokenizer = new HpglTokenizer(rdr);

			Assert.Equal(HpglToken.IN, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.SEMICOLON, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.IP, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 0, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 0, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 100, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 10.03, tokenizer.readNextToken());
			Assert.Equal(HpglToken.SEMICOLON, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.SC, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 0, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 100, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) 0, tokenizer.readNextToken());
			Assert.Equal(HpglToken.COMMA, (HpglToken) tokenizer.readNextToken());
			Assert.Equal((double) -100, tokenizer.readNextToken());
			Assert.Equal(HpglToken.SEMICOLON, (HpglToken) tokenizer.readNextToken());
			Assert.Equal(HpglToken.EOF, (HpglToken) tokenizer.readNextToken());
		}
	}
}