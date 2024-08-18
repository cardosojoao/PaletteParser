using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser
{
    public class Arguments : IArguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public string InputType { get; set; }
        public string OutputType { get; set; }


        public Arguments(Options args)
        {
            InputFile = args.InputFile;
            OutputFile = args.OutputFile;
            if (args.inputType.Length == 0)
            {
                InputType = Path.GetExtension(args.InputFile).ToLower().Replace(".",string.Empty);
            }
            else
            {
                args.inputType = args.inputType.ToLower();
            }

            if (args.outputType.Length == 0)
            {
                OutputType = Path.GetExtension(args.OutputFile).ToLower().Replace(".", string.Empty);
            }
            else
            {
                args.outputType = args.outputType.ToLower();
            }
        }
    }
}
