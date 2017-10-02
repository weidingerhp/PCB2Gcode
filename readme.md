# HPGL (from Eagle) to GCode Converter

This project is aimed to make a simple converter to Covert HPGL output from Eaglecad to GCode Input for a Drawbot. I want to use this for drawing my Print layout to a Copper-Coated PCB using a Drawbot and a simple Permanent Marker.

## Info and building
The code is written in C# using [.NETcore 2.0](https://www.microsoft.com/net/download/core) - so it runs on Windows/linux/mac (even on raspberry :) )  
I've used [Visual Studio Code](https://code.visualstudio.com/) for editing but also packaged a VS-Solution file.

## How to use

*Note:* Please replace '/' with '\\' (backslash) on windows systems.

cd to the console dir  
`cd <root>/console`

and then run the console app with input and output:  
`dotnet run ../input.hpgl ../output.gcode`


