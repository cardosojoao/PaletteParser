using PaletteParser.Core.Entities;
using System.Text;

namespace PaletteParser.Core.Parsers
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
            IPaletteGeneric palette = CreatePalette(9);     // gpl always create 9b palette
            DataBlocks data = ConvertData(input);
            // include all comments header
            foreach (string comment in data.CommentsHeader)
            {
                palette.CommentsHeader.Add(comment);
            }

            if (data.PaletteData.Count > 0)
            {
                //IPaletteGeneric palette = CreatePalette(9);     // gpl always create 9b palette
                int index = 0;
                for (int color = 0; color < data.PaletteData.Count; color++)
                {
                    palette[(byte)index] = data.PaletteData[index];
                    palette.Comments[(byte)index] = data.Comments[(byte)index];
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
            if (pal.CommentsHeader.Count > 0)
            {
                foreach (string comment in pal.CommentsHeader)
                {
                    text.AppendLine(comment);
                }
            }
            else
            {
                text.AppendLine("GIMP Palette");
                text.AppendLine("Channels: RGBA");
                text.AppendLine("# Created using Palette parser utility.");
                text.Append("# ").AppendLine(_args.InputFile);
            }

            for (int index = 0; index < pal.Count; index++)
            {
                var rgb = Color2RGB(pal[(byte)index]);
                text.Append(string.Format("{0,3} ", rgb.R));
                text.Append(string.Format("{0,3} ", rgb.G));
                text.Append(string.Format("{0,3} ", rgb.B));
                text.Append("255"); // A
                if (pal.Comments[index] != null)
                {
                    text.Append('\t');
                    text.AppendLine(pal.Comments[index]);
                }
                else
                {
                    int color = pal[(byte)index];
                    int r = (color & 0b111000000) >> 6;
                    int g = (color & 0b000111000) >> 3;
                    int b = (color & 0b000000111);

                    text.Append("\t");
                    text.AppendLine(NextColorNaming.GetName(r, g, b).Name);
                }
            }
            File.WriteAllText(_args.OutputFile, text.ToString());
        }


        /// <summary>
        /// convert asm file to collection of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private DataBlocks ConvertData(string[] input)
        {
            List<string> header = [];
            List<string> comments = new(input.Length);
            List<int> data = new(input.Length);
            int index = 0;
            bool firstdataLine = false;
            foreach (string line in input)
            {
                if (LineValid(line))
                {
                    firstdataLine = true;
                    int[] rgb = SplitLine(line);
                    rgb[0] = ((rgb[0] >> 5) << 6);    // keep first 3 bits of R
                    rgb[1] = ((rgb[1] >> 5) << 3);    // keep first 3 bits of G
                    rgb[2] = (rgb[2] >> 5);           // keep first 3 bits of B
                    int color = rgb[0] + rgb[1] + rgb[2];
                    data.Add(color);
                    // check inline comment
                    comments.Add(GetComment(line));
                    index++;
                }
                else if (firstdataLine)      // already process the first data line we are going to assume that is a comment colour
                {
                    comments[index] = line;
                }
                else
                {
                    header.Add(line);
                }
            }
            return new DataBlocks(data, header, comments);
        }

        private (int R, int G, int B) Color2RGB(int color)
        {
            int temp = color & 448;
            int r = (temp >> 1) + (temp >> 4) + (temp >> 7);
            temp = color & 56;
            int g = (temp << 2) + (temp >> 1) + (temp >> 4);
            temp = color & 7;
            int b = (temp << 5) + (temp << 2) + (temp >> 1);
            return (r, g, b);
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
            int[] rgb = new int[3];

            for (int i = 0; i < rgb.Length; i++)
            {
                rgb[i] = Convert.ToInt32(cols[i]);
            }
            return rgb;
        }


        private static string GetComment(string line)
        {
            string comment = new string(line.Select(c => !char.IsDigit(c) ? c : '\0').Where(c => c != '\0').ToArray()).Trim();
            return comment;
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
