## Overview
This repository contains a couple of .NET code analyzers, built with NDepend:
1. Design Smells Detection Strategies - For more information about the detection strategies, check [this article](https://www.simpleorientedarchitecture.com/how-to-identify-common-code-smells-using-ndepend/)
2. Structural Relations

## Prerequisites
1. .NET Framework installed
2. [NDepend](https://www.ndepend.com/download) installed and NDepend license activated 
3. All solutions that you want to analyze need to be built

## Usage
1. Clone this repository. The repository needs to be in the same folder as the NDepend installation (next to the Lib folder). For more information about why, check the [NDepend Power Tools documentation](https://www.ndepend.com/API/NDepend.API_gettingstarted.html)
2. Build this solution.
3. Go to the output folder and run the executable file (CodeAnalyzer.App.exe). 
E.g.: 
```bash
CodeAnalyzer.App.exe -TopLevelInputFolder c:\path\to\input\folder\ -OutputFolder .\output\
```

### Notes
1. Because building the NDepend project can take some time, the CodeAnalyzer will reuse an already built project (if it's present in the *NDependProjects* folder in the output directory). If you want to rebuild the NDepend project, delete the contents of this directory.