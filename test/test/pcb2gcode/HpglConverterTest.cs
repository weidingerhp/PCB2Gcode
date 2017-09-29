using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;
using Xunit;

namespace test.pcb2gcode {
	public class HpglConverterTest {
		[Fact]
		void testReadSimpleString() {
			var fs = new FileStream("../../../../examples/pwmexample.hpgl", FileMode.Open);
			var fo = new FileStream("../../../../examples/pwmexample.gcode", FileMode.CreateNew);
			HpglConverter conv = new HpglConverter();

			MemoryStream outputStream = new MemoryStream();
			using (StreamReader rdr = new StreamReader(fs)) {
				using (StreamWriter writer = new StreamWriter(fo)) {
					conv.ConvertHpgl(rdr, writer);
				}
			}

			String output = Encoding.UTF8.GetString(outputStream.GetBuffer());
			Console.Out.Write(output);
		}
	}

}
