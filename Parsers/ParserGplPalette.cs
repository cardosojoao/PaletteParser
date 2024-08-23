using PaletteParser.Entities;
using System.ComponentModel;
using System.Net.Mail;
using System.Text;

namespace PaletteParser.Parsers
{
    public interface IParsetGplPalette : IParserPalette { }

    public class ParserGplPalette : IParsetGplPalette
    {
        private readonly IArguments _args;
        public ParserGplPalette(IArguments args)
        {
            _args = args;
        }
        public IPaletteGeneric Import()
        {
            string[] input = File.ReadAllLines(_args.InputFile, Encoding.ASCII);

            List<int> data = ConvertData(input);

            if (data.Count > 0)
            {
                IPaletteGeneric palette = CreatePalette(9);     // gpl always create 9b palette
                byte index = 0;
                for (int color = 0; color < data.Count; color++)
                {
                    palette[index] = data[index];
                    index++;
                }
                palette.Count = index;
                return palette;
            }
            else
            {
                throw new FormatException("incorrect assembler palette format.");
            }
        }
        public void Export(IPaletteGeneric pal)
        {
            StringBuilder text = new(3096);
            text.AppendLine("GIMP Palette");
            text.AppendLine("Channels: RGB");
            text.AppendLine("# Created using Palette parser utility.");
            text.Append("# ").AppendLine(_args.InputFile);
            for (int index = 0; index < pal.Count; index++)
            {
                var rgb = Color2RGB(pal[(byte)index]);
                text.Append(string.Format("{0,3} ", rgb.R));
                text.Append(string.Format("{0,3} ", rgb.G));
                text.Append(string.Format("{0,3} ", rgb.B));
                text.AppendLine();
            }
            File.WriteAllText(_args.OutputFile, text.ToString());
        }



        /// <summary>
        /// convert asm file to collection of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<int> ConvertData(string[] input)
        {
            List<int> data = new(input.Length);
            int index = 0;
            foreach (string line in input)
            {
                if (LineValid(line))
                {
                    int[] rgb = SplitLine(line);
                    rgb[0] = ((rgb[0] >> 5) << 6);    // keep first 3 bits of R
                    rgb[1] = ((rgb[1] >> 5) << 3);    // keep first 3 bits of G
                    rgb[2] = (rgb[2] >> 5);           // keep first 3 bits of B
                    int color = rgb[0] + rgb[1] + rgb[2];
                    data.Add(color);
                    index++;
                }
            }
            return data;
        }

        private (int R, int G, int B) Color2RGB(int color)
        {
            int temp = color & 448;
            int r = (temp>>1) + (temp >> 4) + (temp  >> 7);
            temp = color & 56;
            int g = (temp << 2) + (temp >> 1) + (temp >> 4);
            temp = color & 7;
            int b = (temp<<5) + (temp << 2) + (temp>>1);
            return ( r, g, b);
        }

        /// <summary>
        /// decide if is a valida line to be consumed or ignored e.g. emppty lines or comment lines should be ignored
        /// </summary>
        /// <param name="line">line to be validated</param>
        /// <returns>true if is valid to be parsed</returns>
        private bool LineValid(string line)
        {
            line = line.Trim().Replace("\t", string.Empty);
            return !(line.Length == 0 || line.StartsWith("Name:") || line.StartsWith("Channels:") || line.StartsWith("Columns:") || line.StartsWith('#') || line.Equals("GIMP Palette", StringComparison.InvariantCultureIgnoreCase));
        }

        private int[] SplitLine(string line)
        {

            line = string.Join(" ", line.Replace('\t', ' ').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            string[] cols = line.Split(new char[] { ' ' });

            // we just need 3 values
            int[] bytes = new int[3];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToInt32(cols[i]);
            }
            return bytes;
        }


        /// <summary>
        /// create a generic palette based on the first line of assembler palette
        /// </summary>
        /// <param name="cols">array with colors</param>
        /// <returns>Generic palette object</returns>
        /// <exception cref="FormatException">if can't detect if is 8 or 9 bits palette will throw bad format exception</exception>
        private IPaletteGeneric CreatePalette(byte paletteType)
        {
            IPaletteGeneric palette = new PaletteGeneric(paletteType);
            return palette;
        }
    }
}
