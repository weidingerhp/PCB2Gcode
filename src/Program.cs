using System;
using at.hpw.pcb2gcode;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
            var iFile = System.IO.File.OpenText(args[0]);
            var oFile = System.IO.File.CreateText(args[1]);

            HpglConverter converter = new HpglConverter();
            converter.ConvertHpgl(iFile, oFile);
        }
    }
}
