using System;
using System.IO;
using System.Threading.Tasks;

namespace at.hpw.pcb2gcode {
	class HpglState {
		public enum PenState {
			PenUp,
			PenDown
		}

		public double XPos { get; set; }
		public double YPos { get; set; }
		public int PenNumber { get; set; }
		public PenState PenPosition { get; set; }
		public HpglConverter converter { get; private set; }
		public bool LastMovementAbsolute { get; internal set; }

		public double XMin {get; set;}
		public double YMin {get; set;}
		public double XMax {get; set;}
		public double YMax {get; set;}

		internal HpglState(float x, float y, PenState pen, HpglConverter converter) {
			this.converter = converter;
			this.XPos = x;
			this.YPos = y;
			this.PenPosition = pen;
			this.PenNumber = 0;
			this.LastMovementAbsolute = true; // default according to HPGL-Spec
			XMin = 0;
			XMax = 0;
			YMin = 100;
			YMax = 100;
		}
	}

}