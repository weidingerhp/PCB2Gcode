using System;

namespace at.hpw.pcb2gcode.parser {
	public enum HpglToken { // make this public for tests
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

