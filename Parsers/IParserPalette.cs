using PaletteParser.Entities;

namespace PaletteParser.Parsers
{
    public interface IParserPalette
    {
        void Export(IPaletteGeneric pal);
        IPaletteGeneric Import();
    }
}