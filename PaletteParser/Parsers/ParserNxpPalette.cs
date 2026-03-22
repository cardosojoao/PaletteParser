using PaletteParser.Entities;
using System.ComponentModel;
using System.Drawing;
using System.Net.Mail;
using System.Text;

namespace PaletteParser.Parsers
{
    public interface IParsetNxpPalette : IParserPalette { }

    public class ParserNxpPalette : IParsetGplPalette
    {
        private readonly IArguments _args;
        public ParserNxpPalette(IArguments args)
        {
            _args = args;
        }
        public IPaletteGeneric Import()
        {
            byte[] input = File.ReadAllBytes(_args.InputFile);
            IPaletteGeneric palette = CreatePalette(9);     // nxp always create 9b palette
            DataBlocks data = ConvertData(input);
            // include all comments header
            foreach (string comment in data.CommentsHeader)
            {
                palette.CommentsHeader.Add(comment);
            }

            if (data.PaletteData.Count > 0)
            {
                int index = 0;
                for (int color = 0; color < data.PaletteData.Count; color++)
                {
                    palette[(byte)index] = data.PaletteData[index];
                    //palette.Comments[(byte)index] = data.Comments[(byte)index];
                    index++;
                }
                palette.Count = index;
                return palette;
            }
            else
            {
                throw new FormatException("incorrect NXP palette format.");
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
                    text.AppendLine("\tUntitled");
                }
            }
            File.WriteAllText(_args.OutputFile, text.ToString());
        }



        /// <summary>
        /// convert asm file to collection of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private DataBlocks ConvertData(byte[] input)
        {
            List<string> header = [];
            List<string> comments = new();
            List<int> data = new(input.Length/2);
            int index = 0;
            bool firstdataLine = false;
            for (int i = 0; i < input.Length; i += 2)
            {
                int color9b = (input[i] + input[i + 1] << 8);
                data.Add(color9b);
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
