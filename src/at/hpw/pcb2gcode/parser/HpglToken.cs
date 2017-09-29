using System;

namespace at.hpw.pcb2gcode.parser {
	internal enum HpglToken {
		IN, // initialize
		PU,
		PD,
		PA,
		PR,
		AA,
		SP, // select pen
		IP,
		SC,

		COMMA,
		SEMICOLON,
		EOF
	}

}

