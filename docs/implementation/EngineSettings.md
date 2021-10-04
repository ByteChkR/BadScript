# Bad Script Engine Settings

The BSEngineSettings object exist, to make creating engine instances easier.

## The Default Settings
To Create an Engine Settings object with the default values use `BSEngineSettings.MakeDefault`.
Optionally, BSParserSettings can be supplied to the MakeDefault method. If no parser settings are specified, `BSParserSettings.Default` will be used.
```cs
BSParserSettings parserSettings = BSParserSettings.Default;
BSEngineSettings settings = BSEngineSettings.MakeDefault(parserSettings);
```

By Default there are 3 interfaces that are loaded on start:
- [Convert](../interfaces/new/Convert.md)
- [Console](../interfaces/new/Console.md)
- [Collection](../interfaces/new/Collection.md)

When Building the Engine Instance with `BSEngineSettings.Build`, there is an optional parameter that changes wether the [Environment Interface](../interfaces/new/Environment.md) will be added to that instance. Disabling access to the `Environment` Interface effectively locks down the runtime so that no other interfaces can be loaded and no new scripts can be executed.

## Include Directories
The `BSEngineSettings.IncludeDirectories` property is a list of directories that contain scripts that get loaded during creation of the BSEngine Instance.

By Default this list is empty.

## Interfaces
A list of all `ABSScriptInterface` Instances that will be available to be loaded inside the BSEngine Instance

By Default this list is empty and needs to be filled with the desired ABSScriptInterface implementations before building.

## Active Interfaces
A list of all Interface names that will be loaded by default.

By Default there are 3 Interfaces that will be loaded on creation.

Prefixing a name with `#` will result in the interface beeing loaded in global scope.

### Console Interface loaded as `Console`
Gets Loaded into seperate table with name `Console`
```js
Console.WriteLine("Hello World")
Console.Clear()
```
### Console Interface loaded as `#Console`
Gets loaded directly into global scope.
```js
WriteLine("Hello World")
Clear()
```


