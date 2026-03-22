using PaletteParser.Entities;
using System;
using System.Text;
using System.Xml.Linq;

namespace PaletteParser.Parsers
{
    public interface IParsetAsmPalette : IParserPalette { }

    public class ParserAsmPalette : IParsetAsmPalette
    {
        private readonly IArguments _args;
        public ParserAsmPalette(IArguments args)
        {
            _args = args;
        }
        public IPaletteGeneric Import()
        {
            string[] input = File.ReadAllLines(_args.InputFile, Encoding.ASCII);

            DataBlocks data = ConvertData(input);

            int paletteType = DetectPalette(data.PaletteData);
            if (data.PaletteData.Count > 0)
            {
                IPaletteGeneric palette = new PaletteGeneric(paletteType);
                bool bit8 = palette.Bits == 8;
                int index = 0;
                foreach (int color in data.PaletteData)
                {
                    palette[(byte)index] = color;
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

            foreach (string comment in pal.CommentsHeader)
            {
                text.Append(';').AppendLine(comment);
            }
            for (int index = 0; index < pal.Count; index++)
            {
                text.Append('\t').Append("db ");
                if (pal.Bits == 8)
                {
                    text.Append('$').Append(pal[(byte)index].ToString("X2"));
                }
                else
                {
                    int color = pal[(byte)index];
                    text.Append('$').Append((color >> 1).ToString("X2"));
                    text.Append(", ");
                    text.Append('$').Append((color & 1).ToString("X2"));
                }
                text.Append('\t').Append("; ").AppendLine(pal.Comments[index]);
            }
            File.WriteAllText(_args.OutputFile, text.ToString().ToLower());
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
                    data.Add(SplitLine(line));
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
            return new DataBlocks(data, header, comments); ;
        }

        /// <summary>
        /// decide if is a valida line to be consumed or ignored e.g. emppty lines or comment lines should be ignored
        /// </summary>
        /// <param name="line">line to be validated</param>
        /// <returns>true if is valid to be parsed</returns>
        private bool LineValid(string line)
        {
            line = line.Trim().Replace("\t", string.Empty);
            return !(line.Length == 0 || line.StartsWith(';'));
        }

        private int SplitLine(string line)
        {
            string[] cols = line.Trim().Replace("db ", string.Empty).Replace("db\t", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty).Replace("$", string.Empty).Split(new char[] { ',', });
            int colors;
            bool bits8 = cols.Length == 1;
            if (bits8)
            {
                colors = Convert.ToByte(cols[0], 16);
            }
            else
            {
                colors = (Convert.ToInt16(cols[0], 16)) + (Convert.ToInt16(cols[1], 16) * 256);
            }
            return colors;
        }


        /// <summary>
        /// create a generic palette based on the first line of assembler palette
        /// </summary>
        /// <param name="cols">array with colors</param>
        /// <returns>Generic palette object</returns>
        /// <exception cref="FormatException">if can't detect if is 8 or 9 bits palette will throw bad format exception</exception>
        private IPaletteGeneric CreatePalette(byte[] cols)
        {
            IPaletteGeneric palette;
            if (cols.Length == 16)
            {
                palette = new PaletteGeneric(8);
            }
            else if (cols.Length == 32)
            {
                palette = new PaletteGeneric(9);
            }
            else
            {
                throw new FormatException("incorrect assembler palette format, must have 16 or 32 bytes per line.");
            }
            return palette;
        }

        private static string GetComment(string line)
        {
            int sep = line.IndexOf(';');
            if (sep == -1)
            {
                return string.Empty;
            }
            else
            {
                return line.Substring(sep + 1);
            }

        }

        private int DetectPalette(List<int> data)
        {
            int bits = 8;
            foreach (int color in data)
            {
                if (color > 255)
                {
                    bits = 9;
                    break;
                }
            }
            return bits;
        }
    }
}
