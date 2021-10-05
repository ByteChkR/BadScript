# Bad Script Console `project` Subsystems

## Subsystem `new`

Creates a new Project from a selected Template

### Optional Arguments
- `-t/--template [templateName]`
	- Specifies the Template of the project
	- Default Value: `app`
- `-n/--name [projectName]`
	- Specifies the name of the project
	- Default Value: `Unset(depends on selected template)`
- `--nologo`
	- Does not display Console Header information when set.

### Default Templates
- `app`
	- Application template.
	- Contains `build`/`run`/`publish` build targets
	- Automatic Versioning
- `empty`
	- Empty Project Template
	- Contains `build` target
- `lib`
	- Library template
	- Contains `build`/`test` build target
	- Automatic Versioning

### Examples
Creates a new Project based on the `app` template in the current working directory.
```
bs project new -t app
```

Creates a new Project with name `MyApp` based on the `app` template in the current working directory.
```
bs project new -t app -n MyApp
```

## Subsystem `make`

Runs a selected build target

### Optional Arguments
- `-t/--target [targetName]`
	- Specifies the Build target to be executed.
	- Default Value: `build`
- `--nologo`
	- Does not display Console Header information when set.


### Examples

Run the default target of the project in the current working directory
```
project make
```

Run the `build` target of the project in the current working directory
```
project make -t build
```


## Project Build Settings
The Project Information gets stored in the file `build-settings.json` in the root of the project directory.

[App Template Build Settings Example](https://github.com/ByteChkR/BadScript/blob/master/src/BadScript.Console/data/project/templates/app/build-settings.json)

### App Info
Contains the Name and Version of the Project
```json
"AppInfo": {
    "Name": "App",
    "Version": "0.0.0.1"
  }
```

### Build Targets
Contains a list of all build targets that are defined for this project.

```json
"BuildTargets": {
    "Targets": [
    	...
    ]
}
```

### Build Target
A build target defines the way a project gets built.

```json
{
	"Name": "build",
	"OutputFormat": "text",
	"OutputFile": "./bin/build/App.bs",
	"Include": [
		"./src/*.bs"
	],
	"SubTarget": "",
	"References": [],
	"PreEvents": [],
	"PostEvents": [],
	"PreprocessorDirectives": ""
}
```

#### Sub Targets
It is possible to specify a sub target that will get executed before the current target is running.

### Preprocessor Directives
A List of Directives that will be defined in the preprocessor during execution of the build target.

```json
"PreprocessorDirectives":
	"TARGET=\"build\" NAME=\"App\" VERSION=\"1.0.0.0\" FULLNAME=\"App@1.0.0.0\""
```

### Reflected Properties
The Project Settings have a feature that enables writing properties that get resolved at runtime

Instead of Explicitly writing out the Output Path of the build target, the output path can be specified dynamically like so:
```json
"OutputFile": "./bin/%Target.Name%/%AppInfo.Name%%AppInfo.Version%.%Target.Output.OutputExtension%"
```
During Runtime the OutputFile property will have this value:
- Target: `build`
- App Name: `App`
- App Version: `1.0.0.0`
- OutputFormat: `text`
	- The Output Format defines the Output Extension of the build target.
```json
"OutputFile": "./bin/build/App1.0.0.0.bs"
```

It is also possible to use the properties from subtargets inside the current target.
Which enables one to pass through the output file and other information of the subtarget.
```json
"OutputFile": "%SubTarget.OutputFile%",
```

The Properties of the Settings file will resolve recursively.

___

[List of all Console Subsystems](./Subsystems.md)