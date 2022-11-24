# System Monitor
This GenericNET driver extension is a proof-of-concept (POC) to read system parameters and provide the values to zenon variables. In this POC the CPU temperature of available CPU Cores is read with LibreHardwareMonitor. This is a .NET library or OpenHardwareMonitor. 

Please note, that in order to read system parameters (like the CPU temperature) **administrator rights** are needed.

The following explains the configuration of:

- Generic NET driver
- zenon variables
- Starting and Admin Rights 

<br/><br/>
## Generic NET driver

The Generic NET driver needs the driver extension provided in this project. You can either clone the sources and build
the .dll for yourself or download the zip package. 
Extract the contents of the zip package to the installation folder of your zenon version in an according sub folder of the folder 'DriverExtensions'

> **_Example:_**  ```C:\Program Files (x86)\COPA-DATA\zenon Software Platform 11\DriverExtension\SystemMonitor```

If you rather want to build the sources have a look at the .cproj file. It contains an instruction to copy the build artifacts to the ```..\DriverExtension\SystemMonitor``` folder of your zenon installation. You need to correct the zenon version in the instruction eventually. 

In the Engineerings Studio create a driver of the type ```.NET Driver API``` located in the folder ```COPA-DATA```. In the driver configuration switch to the tab ```Assemby setting``` and select the extension folder and the extension filename ```CopaData.Drivers.Samples.SystemMonitor.dll```. Note that the extension folder contains multiple .dlls. These are needed. Do not delete them. Do not reference them. Do reference the correct one.

Close the configuration dialogue with OK.

<br/><br/>
## zenon Variables
Create a zenon variable of type ```REAL``` on the Generic NET driver for each physical CPU Core of your machine. Name the variables as you like. The **symbolic address** of the variables **must** follow the pattern:

> **_Pattern_**:  ```CPU Core 1```

The number ```1``` in the pattern references to the CPU core number and could be ```2```, ```3```, etc... The numbers start with 1.

<br/><br/>
## Starting and Admin Rights

The extension needs admin-rights in order to gather the information of the CPU. Therefore you can either 
- Start the service engine with admin rights

or

-  Start the Generic NET driver (only) with admin rights.

The latter can be done by starting a powershell command with admin rights. Then navigate to the installation folder of your zenon installation. Start the executable ```GenericNet.exe```. 
> **_Powershell_**: 

>```cd\```

> ```.\"Program Files (x86)\COPA-DATA\zenon Software Platform 11\GenericNet.exe"```

Adjust the command according to your zenon installation (version).

After the Generic NET driver has been started (check in the task manager) you can start the Service Engine as usual.