using System;
using System.IO;
using System.Text;
using at.hpw.pcb2gcode;
using Microsoft.Extensions.CommandLineUtils;

namespace at.hpw.pcb2gcode {
	class Program {
		static void Main(string[] args) {

            // parse input
            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);
            var inputOption = commandLineApplication.Argument(
				"input <input.hpgl>", 
				"Input HPGL File to be processed.");
            var outputOption = commandLineApplication.Argument(
				"output <output.gcode>", 
				"Output GCODE File to be written.");

			commandLineApplication.Execute(args);

			if (inputOption.Value == null || outputOption.Value == null) {
				commandLineApplication.ShowHelp();
				return;
			}

            var converter = new HpglConverter();
			DoConvert(converter, inputOption.Value, outputOption.Value);
		}

        private static void DoConvert(HpglConverter converter, string inputFile, string outputFile)
        {
			converter.InitPosition0 = true;

			Console.Out.WriteLine("Converting HPGL '{0}' to GCODE '{1}'", inputFile, outputFile);

            using (var oFile = System.IO.File.CreateText(outputFile))
            {
                using (var iFile = System.IO.File.OpenText(inputFile))
                {
                    oFile.Write("G91\nG21\nG1 F1000\n"); // write some init (relative, metric, travel speed)

					converter.ConvertHpgl(iFile, oFile);
				}					
			}
			Console.Out.WriteLine("Finished Converting...");
        }
    }
}
