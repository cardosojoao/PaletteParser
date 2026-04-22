using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using PaletteParser.Core;
using PaletteParser.Core.Services;
using System.Reflection;

namespace PaletteParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options, ExtractOptions>(args)
                        .WithParsed<Options>(options =>
                        {
                            IArguments arguments = new Arguments(options.InputFile, options.OutputFile, options.InputType, options.OutputType);
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
                        })
                        .WithParsed<ExtractOptions>(extractOptions =>
                        {
                            IArguments arguments = new Arguments(extractOptions.InputFile, extractOptions.OutputFile, null, null, extractOptions.OffSet);

                            var services = new ServiceCollection()
                            .AddSingleton<IArguments>(arguments)
                            .AddScoped<IExtractor, Extractor>()
                            .BuildServiceProvider();
                            var Extractor = services.GetRequiredService<IExtractor>();
                            Extractor.Extract(16);
                            Console.WriteLine("Finished extract.");
                        });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
