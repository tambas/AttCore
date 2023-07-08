using Giny.Core;
using Giny.IO.D2P;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.D2P
{
    class Program
    {
        public const string OutputDirectory = "Output/";

        public const string InputDirectory = "Input/";

        static readonly PercentageLogger logger = new PercentageLogger();

        static void Main(string[] args)
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            if (!Directory.Exists(InputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            foreach (var file in Directory.GetFiles(InputDirectory))
            {
                D2PFile d2pFile = new D2PFile(file);
                d2pFile.ExtractPercentProgress += Progress;
                d2pFile.ExtractAllFiles(OutputDirectory, true, true);
            }


            Console.WriteLine("Ended.");

            Console.ReadLine();
        }

        private static void Progress(D2PFile arg1, int arg2)
        {
            logger.Update(Path.GetFileName(arg1.FilePath) + " " + arg2 + "%");
        }
    }
}
