using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;
using Xunit;

namespace test.pcb2gcode {
	public class HpglConverterTest {
		[Fact]
		void testReadSimpleString() {
			// just a small test to check if the converter does not crash
			var fs = new FileStream("../../../../examples/pwmexample.hpgl", FileMode.Open);
			HpglConverter conv = new HpglConverter();

			MemoryStream outputStream = new MemoryStream();
			using (StreamReader rdr = new StreamReader(fs)) {
				using (StreamWriter writer = new StreamWriter(outputStream)) {
					conv.ConvertHpgl(rdr, writer);
				}
			}

			String output = Encoding.UTF8.GetString(outputStream.GetBuffer());
		}

				[Fact]
		void testSimpleArc() {
			// just a small test to check if the converter does not crash
			var fs = new FileStream("../../../../examples/simplecircle.hpgl", FileMode.Open);
			HpglConverter conv = new HpglConverter();
			long length = 0;

			MemoryStream outputStream = new MemoryStream();
			using (StreamReader rdr = new StreamReader(fs)) {
				using (StreamWriter writer = new StreamWriter(outputStream)) {
					conv.ConvertHpgl(rdr, writer);
					writer.Flush();
					length = outputStream.Position;
				}
			}

			String output = Encoding.UTF8.GetString(outputStream.GetBuffer(), 0, (int) length);
			Assert.Equal("G0 X0.00000 Y-0.76200\nG3 X0.00000 Y0.00000 I-0.76200 J0.00000\n" +
						"G0 X0.00000 Y-0.76200\nG3 X0.00000 Y0.00000 I-0.76200 " +
						"J0.00000\nM5\nG0 X0.00000 Y1.52400\n", output);
		}

	}

}
