using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicalStructure
{
    public class Validity
    {
        public Dictionary<string, Dictionary<string, string>> data { get; set; } 

        public bool isConfirm { get; set; }

        // 枚举值：all,materialName,mat,soe
        public string type { get; set; }
    }
}
