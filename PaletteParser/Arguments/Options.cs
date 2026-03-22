using CommandLine;


namespace PaletteParser
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to be processed.")]
        public string InputFile { get; set; }


        [Option('o', "output", Required = true, HelpText = "Output file to be created.")]
        public string OutputFile { get; set; }

        [Option('f', "format input", Required = false, Default = "", HelpText = "format of input file.")]
        public string inputType { get; set; }


        [Option('g', "format output", Required = false, Default = "", HelpText = "format of input file.")]
        public string outputType { get; set; }

    }
}
