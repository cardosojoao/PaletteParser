using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser.Entities
{
    public class PaletteGeneric : IPaletteGeneric
    {
        /// <summary>
        /// 8 or 9 bits palette
        /// </summary>
        public int Bits { get; private set; }

        public int Count { get; set; }

        private int[] paletteData;

        public PaletteGeneric(int bits)
        {
            Bits = bits;
            paletteData = new int[256];
        }


        public int this[byte index]
        {
            get
            {
                return paletteData[index];
            }
            set
            {
                paletteData[index] = value;
            }
        }

        //public static int Byte2Int(byte byteL, byte byteH)
        //{
        //    return (int)(byteH * 256 + byteL);
        //}

        //public static (int byteL, int byteH) Int2Byte(int value)
        //{
        //    return ((byte)(value & 255), (byte)(value >> 8));
        //}

    }
}
