using CommandLine;


namespace PaletteParser
{
    [Verb("convert", HelpText = "convert a palette from one format to another")]
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to be processed.")]
        public string InputFile { get; set; }


        [Option('o', "output", Required = true, HelpText = "Output file to be created.")]
        public string OutputFile { get; set; }

        [Option('f', "format input", Required = false, Default = "", HelpText = "format of input file.")]
        public string InputType { get; set; }


        [Option('g', "format output", Required = false, Default = "", HelpText = "format of input file.")]
        public string OutputType { get; set; }

    }

    [Verb("extract", HelpText = "extract a portion of a palette")]
    public class ExtractOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input file to be processed.")]
        public string InputFile { get; set; }


        [Option('o', "output", Required = true, HelpText = "Output file to be created.")]
        public string OutputFile { get; set; }

        [Option('f', "offset", Required = false, Default = 0, HelpText = "offset of 4bits palette")]
        public int  OffSet{ get; set; }

    }


}
