namespace PaletteParser
{
    public interface IArguments
    {
        string InputFile { get; set; }
        string InputType { get; set; }
        string OutputFile { get; set; }
        string OutputType { get; set; }
    }
}