using System;

namespace at.hpw.pcb2gcode.parser {
	public enum HpglToken { // make this public for tests
		IN, // initialize
		VS,
		PU,
		PD,
		PA,
		PR,
		AA,
		SP, // select pen
		IP,
		SC,
		CI,

		COMMA,
		SEMICOLON,
		EOF
	}

}

