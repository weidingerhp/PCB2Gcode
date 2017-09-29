using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;

namespace at.hpw.pcb2gcode {
	class Program {
		static void Main(string[] args) {
			HpglConverter converter = new HpglConverter();
			using (var oFile = System.IO.File.CreateText(args[1])) {
				using (var iFile = System.IO.File.OpenText(args[0])) {


					oFile.Write("G91\nG21\nG1 F1000\n"); // write some init (relative, metric, travel speed)

					converter.ConvertHpgl(iFile, oFile);
				}

				String input = "PU;\nPA 0,0;\n"; // reset to initial position
				MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
				using (StreamReader rdr = new StreamReader(inputStream)) {
					converter.ConvertHpgl(rdr, oFile);
				}
				inputStream.Dispose();
					
			}
		}
	}
}
