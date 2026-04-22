using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser.Core
{
    public class Arguments : IArguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public string InputType { get; set; }
        public string OutputType { get; set; }
        public int OffSet { get; set; }  


        public Arguments(string inputFile, string outputFile, string? inputType, string? outputType, int offSet = 0)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
            OffSet = offSet;
            if (string.IsNullOrEmpty(inputType))
            {
                InputType = Path.GetExtension(InputFile).ToLower().Replace(".",string.Empty);
            }
            else
            {
                InputType = inputType.ToLower();
            }

            if (string.IsNullOrEmpty(outputType))
            {
                OutputType = Path.GetExtension(OutputFile).ToLower().Replace(".", string.Empty);
            }
            else
            {
                OutputType = outputType.ToLower();
            }
        }
    }
}
