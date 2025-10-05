using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuniText.SuniClass
{
    public class TextFile
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string BackupPath { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public List<string> Paths { get; set; } = new List<string>();
        public int Index { get; set; } = 0;
    }
}
