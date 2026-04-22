using PaletteParser.Core.Entities;

namespace PaletteParser.Core.Services
{
    public interface IExtractor
    {
        void Extract(int count);    
        IPaletteGeneric Load();
        void Save(IPaletteGeneric palette);
    }
}