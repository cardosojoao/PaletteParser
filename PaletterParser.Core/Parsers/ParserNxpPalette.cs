using PaletteParser.Core.Entities;

namespace PaletteParser.Core.Parsers
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
            byte[] output = new byte[pal.Count * 2];
            int outputIndex = 0;
            for (int index = 0; index < pal.Count; index++)
            {
                int lb = pal[(byte)index] >> 1;
                int hb = pal[(byte)index] & 0x001;
                output[outputIndex] = (byte)lb;
                output[outputIndex + 1] = (byte)hb;
                outputIndex += 2;
            }
            File.WriteAllBytes(_args.OutputFile, output);

        }



        /// <summary>
        /// convert asm file to collection of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static DataBlocks ConvertData(byte[] input)
        {
            List<string> header = [];
            List<string> comments = [];
            List<int> data = new(input.Length / 2);
            for (int i = 0; i < input.Length; i += 2)
            {
                int bytelow = input[i];
                int bytehigh = input[i + 1];
                int color9b = (bytelow << 1) + (bytehigh);
                data.Add(color9b);
            }
            return new DataBlocks(data, header, comments);
        }

        /// <summary>
        /// create a generic palette based on the first line of assembler palette
        /// </summary>
        /// <param name="cols">array with colors</param>
        /// <returns>Generic palette object</returns>
        /// <exception cref="FormatException">if can't detect if is 8 or 9 bits palette will throw bad format exception</exception>
        private static IPaletteGeneric CreatePalette(byte paletteType)
        {
            IPaletteGeneric palette = new PaletteGeneric(paletteType);
            return palette;
        }
    }
}
