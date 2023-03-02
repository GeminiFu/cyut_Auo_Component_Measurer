using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyut_Auo_Component_Measurer
{
    public class ObjectInfo
    {
        public float CenterX { get; set; }

        public float CenterY { get; set; }

        public float width { get; set; }
        public float widthError { get; set; }
        public float widthStd { get; set; }

        public float height { get; set; }
        public float heightError { get; set; }
        public float heightStd { get; set; }


        public string ShapeName { get; set; }

        public int CheckResult { get; set; }

        public uint ElementIndex { get; set; }
        public uint Area { get; set; }
    }
}
