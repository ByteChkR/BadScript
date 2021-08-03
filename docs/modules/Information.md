# Information

`bs interactive` starts the Interactive Shell Session

## Installing the Module System
Installing the module system is a requirement for the interactive shell to function properly(the shell itself is a module)

```js
environment.loadString(environment.loadInterface("http", {}).downloadString("http://static.byt3.dev/apps/BadScript/modules-core/modules_install.bs")).load()
```

### Notes
The Module System will not automatically uninstall itself.
To uninstall the module system, delete the `modules` directory and the `modules.bs` script.

## Installing Interactive Shell Plugins

The Interactive Shell can Load plugins that export global helper functions.
Running this command will add(and update) all utility plugins that are available by default.

```js
environment.loadString(environment.loadInterface("http", {}).downloadString("https://byt3.dev/apps/BadScript/modules/interactive-shell/plugins/update.bs"))
```

### Notes
The Interactive Shell will not clean files on disk automatically. The plugins of the interactive shell can be manually deleted from the plugin directory.