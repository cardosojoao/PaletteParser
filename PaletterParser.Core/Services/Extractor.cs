using PaletteParser.Core.Entities;
using PaletteParser.Core.Parsers;

namespace PaletteParser.Core.Services
{
    public class Extractor : IExtractor
    {
        IArguments _arguments;


        public Extractor(IArguments arguments)
        {
            _arguments = arguments;
        }

        public void Extract( int length)
        {
            var parser = GetParsers();
            IPaletteGeneric inputPalette = parser.input.Import();
            IPaletteGeneric outputPalette = new PaletteGeneric(inputPalette.Bits, length);
            byte indexIni = (byte)(_arguments.OffSet * 16);
            byte indexOutput= 0;
            for (byte index = indexIni ; index < indexIni+length; index++)
            {
                if (index < inputPalette.Count)
                {
                    outputPalette[indexOutput] = inputPalette[index];
                    indexOutput++;
                }
            }
            outputPalette.Count = indexOutput;
            parser.output.Export(outputPalette);
        }

        public IPaletteGeneric Load()
        {
            var parser = GetParser(_arguments.InputType);

            IPaletteGeneric inputPalette = parser.Import();

            return inputPalette;
        }

        public void Save(IPaletteGeneric palette)
        {
            var parser = GetParser(_arguments.OutputType);
            parser.Export(palette);
        }


        private (IParserPalette input, IParserPalette output) GetParsers()
        {
            return (GetParser(_arguments.InputType), GetParser(_arguments.OutputType));
        }

        private IParserPalette GetParser(string type)
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
