using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteParser.Entities
{
    public  class DataBlocks
    {
        public List<string> CommentsHeader { get; private set; }
        public List<string> Comments { get; private set; }
        public List<int> PaletteData { get; private set; }

        public DataBlocks(List<int>data, List<string> headerComments, List<string> comments)
        {
            PaletteData = data;
            CommentsHeader = headerComments;
            Comments = comments;
        }
    }
}
