using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PaletteParser.Core;
using PaletteParser.Core.Services;
using System.Reflection;

namespace PaletteParser
{
    internal class Program
    {
        private static bool argsError = false;
        static void Main(string[] args)
        {
            ParserResult<Options> options = CommandLine.Parser.Default.ParseArguments<Options>(args)
           .WithNotParsed(HandleParseError);

            if (argsError)
            {
                Environment.Exit(-1);
                return;
            }
            else
            {
                try
                {
                    IArguments arguments = new Arguments(options.Value.InputFile, options.Value.OutputFile, options.Value.inputType, options.Value.outputType);
                    Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Console.WriteLine("PalettePaser v" + appVersion.Major + "." + appVersion.Minor + "." + appVersion.Build + ".");
                    Console.WriteLine("Input file: " + arguments.InputFile);
                    Console.WriteLine("Ouput file: " + arguments.OutputFile);
                    Console.WriteLine("Input file type: " + arguments.InputType);
                    Console.WriteLine("Ouput file type: " + arguments.OutputType);

                    var services = new ServiceCollection()
                    .AddSingleton<IArguments>(arguments)
                    .AddScoped<IConverter, Converter>()
                    .BuildServiceProvider();
                    var convert = services.GetRequiredService<IConverter>();
                    convert.Convert();
                    Console.WriteLine("Finished Parserd.");
                    Environment.ExitCode = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error!");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            argsError = true;
        }
    }
}
