# Project System SubTargets Example

Adding a Custom Build Target named `run` that will execute the output as a post build step.

## The `run`-Target

Defines the target that will be executed when using the command `bs project make -t run`

It defines the SubTarget that will be run before the current target to be `build`.

The Build Output for this target is set to the subtargets build output.

To run the built project the target defines a PostEvent that uses %BS_EXEC%, a hidden variable that contains the full path to the Console Executable, to run the output file.

Optionally the PostEvent can define arguments passed to the script with `-a`/`--args`

```json
{
	"Name": "run",
	"OutputFormat": "none", 
	"OutputFile": "%SubTarget.OutputFile%",
	"PreprocessorDirectives": "",
	"Include": [],
	"SubTarget": "build",
	"References": [],
	"PreEvents": [],
	"PostEvents": [
		"%BS_EXEC% run --nologo -f %Target.OutputFile%"
	]
}
```

When executing `bs project make -t run` you should find the "Hello World" Script output inside the `[POST_EVENT]` logs.