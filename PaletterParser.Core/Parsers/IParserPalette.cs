using PaletteParser.Core.Entities;

namespace PaletteParser.Core.Parsers
{
    public interface IParserPalette
    {
        void Export(IPaletteGeneric pal);
        IPaletteGeneric Import();
    }
}