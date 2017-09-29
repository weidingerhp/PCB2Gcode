using System;

namespace src
{
    class Program
    {
        static void Main(string[] args)
        {
            var iFile = System.IO.File.OpenText(args[0]);
            Console.WriteLine("Hello World!");

        }
    }
}
