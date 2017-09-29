using System;
using System.IO;
using System.Threading.Tasks;

namespace at {
namespace hpw {
namespace pcb2gcode {
    class HpglState {
        public enum PenState {
            PenUp,
            PenDown
        }
        
        public double XPos { get; set; }
        public double YPos { get; set; }
        public int PenNumber {get; set; }
        public PenState PenPosition { get; set; }
        public HpglConverter converter { get; private set; }
        public bool LastMovementAbsolute { get; internal set; }

        internal HpglState(float x, float y, PenState pen, HpglConverter converter) {
            this.converter = converter;
            this.XPos = x;
            this.YPos = y;
            this.PenPosition = pen;
            this.PenNumber = 0;
            this.LastMovementAbsolute = true; // default according to HPGL-Spec
        }
    }

}
}
}