using PaletteParser.Entities;
using System.Text;

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

            List<int[]> data = ConvertData(input);

            int paletteType = DetectPalette(data);
            if (data.Count > 0)
            {
                IPaletteGeneric palette = new PaletteGeneric(paletteType);
                bool bit8 = palette.Bits == 8;
                byte index = 0;
                foreach (int[] cols in data)
                {
                    for (int s = 0; s < cols.Length; s++)
                    {
                        palette[index] = cols[s];
                        index++;
                    }
                }
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
            byte index = 0;
            for (int row = 0; row < 16; row++)
            {
                text.Append('\t').Append("db ");
                for (int col = 0; col < 16; col++)
                {

                    if (col > 0)
                    {
                        text.Append(", ");
                    }
                    if (pal.Bits == 8)
                    {
                        text.Append('$').Append(pal[index].ToString("X2"));
                    }
                    else
                    {
                        int color = pal[index];
                        text.Append('$').Append((color >> 1).ToString("X2"));
                        text.Append(", ");
                        text.Append('$').Append((color & 1).ToString("X2"));
                    }
                    index++;
                }
                text.AppendLine();
            }
            File.WriteAllText(_args.OutputFile, text.ToString().ToLower());
        }



        /// <summary>
        /// convert asm file to collection of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<int[]> ConvertData(string[] input)
        {
            List<int[]> data = new(input.Length);
            foreach (string line in input)
            {
                if (LineValid(line))
                {
                    data.Add(SplitLine(line));
                }
            }
            return data;
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

        private int[] SplitLine(string line)
        {
            string[] cols = line.Trim().Replace("db ", string.Empty).Replace("db\t", string.Empty).Replace("\t", string.Empty).Replace(" ", string.Empty).Replace("$", string.Empty).Split(new char[] { ',', });

            int[] colors = new int[16];
            bool bits8 = cols.Length == 16;

            int index = 0;
            for (int i = 0; i < cols.Length; i++)
            {

                if (bits8)
                {
                    colors[index] = Convert.ToByte(cols[i], 16);
                }
                else
                {
                    colors[index] = (Convert.ToInt16(cols[i], 16) << 1) + Convert.ToInt16(cols[i + 1], 16);
                    i++;
                }
                index++;
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

        private int DetectPalette(List<int[]> data)
        {
            int bits = 8;
            foreach (int[] row in data)
            {
                foreach (int col in row)
                {
                    if (col > 255)
                    {
                        bits = 9;
                        break;
                    }
                }
                if (bits == 9)
                {
                    break;
                }
            }
            return bits;
        }
    }
}
