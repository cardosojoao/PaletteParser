using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser.Core.Entities
{
    public class PaletteGeneric : IPaletteGeneric
    {
        /// <summary>
        /// 8 or 9 bits palette
        /// </summary>
        public int Bits { get; private set; }

        public int Count { get; set; }
        public List<string> CommentsHeader { get; set; }
        public string[] Comments { get; set; }

        private readonly int[] paletteData;

        public PaletteGeneric(int bits, int length = 256)
        {
            Bits = bits;
            paletteData = new int[length];
            CommentsHeader = new List<string>();
            Comments = new string[length];
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
    }
}
