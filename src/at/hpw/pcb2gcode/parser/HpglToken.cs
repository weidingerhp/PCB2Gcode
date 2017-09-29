using System;

namespace at {
namespace hpw {
namespace pcb2gcode {
namespace parser {
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
}
}
}

