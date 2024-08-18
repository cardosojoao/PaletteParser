using PaletteParser.Entities;
using PaletteParser.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser.Services
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
