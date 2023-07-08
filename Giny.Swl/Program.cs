using Giny.Core;
using Giny.IO.D2P;
using Giny.IO.SWL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.Swl
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
                SwlFile swl = new SwlFile(File.ReadAllBytes(file));
                Console.WriteLine(Path.GetFileName(file));
                swl.ExtractSwf(OutputDirectory + "/" + Path.GetFileNameWithoutExtension(file) + ".swf");
            }


            Console.WriteLine("Ended.");

            Console.ReadLine();
        }
    }
}
