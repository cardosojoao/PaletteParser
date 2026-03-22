namespace PaletteParser.Entities
{
    public interface IPaletteGeneric
    {
        int this[byte index] { get; set; }

        int Bits { get; }
        int Count { get; set; }
        string []  Comments{ get; set; }
        List<string> CommentsHeader { get; set; }
    }
}