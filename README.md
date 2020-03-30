## Overview
This repository contains a couple of .NET code analyzers, built with NDepend:
1. Design Smells Detection Strategies - For more information about the detection strategies, check [this article](https://www.simpleorientedarchitecture.com/how-to-identify-common-code-smells-using-ndepend/)
2. Structural Relations

## Prerequisites
1. .NET Framework installed
2. NDepend installed and NDepend license activated 

## Usage
1. Clone this repository. The repository needs to be in the same folder as the NDepend installation (next to the Lib folder). For more information about why, check the [NDepend Power Tools documentation](https://www.ndepend.com/API/NDepend.API_gettingstarted.html)
2. Build the solution.
3. Go to the output folder and run the executable file (CodeAnalyzer.App.exe). 
E.g.: 
```bash
CodeAnalyzer.App.exe -TopLevelInputFolder c:\path\to\input\folder\ -OutputFolder .\output\
```