using PaletteParser.Services;
using Microsoft.Extensions.DependencyInjection;
using CommandLine;
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
                IArguments arguments = new Arguments(options.Value);

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
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            argsError = true;
        }
    }
}
