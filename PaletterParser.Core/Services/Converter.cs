using PaletteParser.Core.Entities;
using PaletteParser.Core.Parsers;

namespace PaletteParser.Core.Services
{
    public class Converter : IConverter
    {
        private readonly IArguments _arguments;

        public Converter(IArguments arguments)
        {
            _arguments = arguments;
        }

        public bool Convert()
        {
            var parser = GetParsers();
            IPaletteGeneric inputPalette = parser.input.Import();
            parser.output.Export(inputPalette);
            bool result = true  ;
            return result;
        }

        public IPaletteGeneric Load()
        {
            var parser = getParser(_arguments.InputType);

            IPaletteGeneric inputPalette = parser.Import();

            return inputPalette;
        }

        public void Save(IPaletteGeneric palette)
        {
            var parser = getParser(_arguments.OutputType);
            parser.Export(palette);
        }


        private (IParserPalette input, IParserPalette output) GetParsers()
        {
            return (getParser(_arguments.InputType), getParser(_arguments.OutputType));
        }

        private IParserPalette getParser(string type)
        {
            IParserPalette parser;
            switch (type)
            {
                case "gpl":
                    {
                        parser = new ParserGplPalette(_arguments);
                        break;
                    }
                case "asm":
                    {
                        parser = new ParserAsmPalette(_arguments);
                        break;
                    }
                case "nxp":
                    {
                        parser = new ParserNxpPalette(_arguments);
                        break;
                    }
                default:
                    {
                        parser = new ParserGplPalette(_arguments);
                        break;
                    }

            }
            return parser;
        }

    }
}
