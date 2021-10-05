# Bad Script Console `appbuilder` Subsystem

Builds an app package from a script

## Required Arguments
- `-n/--name <AppName>`
	- Specifies the Name of the App
- `-v/--version <AppVersion>`
	- Specifies the Name of the App
- `-f/--file <InputFile>`
	- Specifies the input script


## Optional Arguments
- `-o/--output [outputFile]`
	- Specifies the Path of the Output File.
	- Default Value: `./app.bsapp`
- `-r/--resources [resourceDirectory]` Specifies the optional resource directory that will get packaged with the app.
	- Default Value: `Unset(No Resources will be included)`
- `-i/--interfaces [interfaceName0] [interfaceName1] ...`
	- The Required interfaces that the app will need to function properly.
	- Default Value: `Unset(All interfaces need to be loaded at runtime + no checks if interfaces are available prior to starting the app)`
- `--minVer [Version]`
	- Specifies the Minimum version of the runtime required to run this app
	- Default Value: `Unset(All Runtime Versions)`
- `--maxVer [Version]`
	- Specifies the Maximum version of the runtime required to run this app
	- Default Value: `Unset(All Runtime Versions)`
- `--nologo`
	- Does not display Console Header information when set.

## Examples

Build the App `app.bsapp` with all default values from the Script `MyApp.bs`.
```
bs appbuilder -n MyApp -v 1.0.0.0 -f MyApp.bs
```

Build the App `MyApp.bsapp` with the resource folder `./resources` from the Script `MyApp.bs`.
```
bs appbuilder -n MyApp -v 1.0.0.0 -f MyApp.bs -o MyApp.bsapp -r ./resources
```

___

[List of all Console Subsystems](./Subsystems.md)