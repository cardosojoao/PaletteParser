using PaletteParser.Core.Entities;
using System.Xml.Linq;

namespace PaletteParser.Core.Services
{
    public interface IConverter
    {
        bool Convert();
        IPaletteGeneric Load();
        void Save(IPaletteGeneric palette);
    }
}