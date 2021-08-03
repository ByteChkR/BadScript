# First Steps

## Build

### Windows
1. Clone Repository `git clone https://github.com/ByteChkR/BadScript.git`
2. Run `dotnet publish -c Release -r "win-x64"`

### Linux
1. Clone Repository `git clone https://github.com/ByteChkR/BadScript.git`
2. Run `dotnet publish -c Release -r "linux-x64"`

________

## Install on Linux

1 . `mkdir ~/BadScriptEngine`

2 . `cd ~/BadScriptEngine`

3 . `wget https://bytechkr.github.io/BadScript/build-linux.zip`

4 . `unzip build-linux.zip`

5 . `chmod +x bs`

Optional Steps:

6 . Make Alias to access `bs` anywhere by adding `alias bs="<home>/BadScriptEngine/bs"` to `~/.bashrc`

7 . Reload Bash Environment Variables with `source ~/.bashrc`

8 . Test Alias with `type -a bs`. It should print `bs is aliased to '/home/tim/BadScriptEngine/bs'`

________

## Install on Windows
1 . Download Build and Extract

Optional Steps:

2 . Add BS Shell Opener by writing a small powershell script with content. Usage: `PS> . <path/to/script>`

```powershell
# Path of the Install Directory(location of bs.exe)
$bs = "<Path of Install Directory>"

# Make Sure that the Path is fully qualified
$bs = (Get-Item $bs).fullname

# Add to Environment Path Variable, Only for this powershell session
$Env:Path += ";$bs" 
```

3 . Add Second Powershell Script that Opens a new Powershell Window with the First Script as start command.

```powershell
# Create New Powershell Window and run the BS Shell Opener.
# Shell can now be opened by doubleclicking this file
start PowerShell -ArgumentList "-noexit -command . <path/to/first/script>"
```

4 . Test Setup with `echo $Env:Path`(should print the Install Directory as one of the last entries).

## Usage
- Running Scripts: `bs <path/to/script>`
- Running Apps: `bs <appname/path>`

________

Notes:

The file extension is optional if the path points to a `*.bs` file(e.g: `bs main` is the same as `bs main.bs`).

It is possible to run multiple scripts and/or apps at once by splitting the executions with `,` or `;`. Example: `bs apps-install interactive,interactive` updates and starts the script or app `interactive.bs`

Apps are stored `./bs-data/apps/` and can be called by passing the relative path of the app based on `./bs-data/apps/`.

Scripts are able to hide apps, `bs myApp` will execute `myApp.bs` in the current directory if it exists, otherwise it will try to execute app `./bs-data/apps/myApp.bs`

________


## Setup
The runtime comes with a default app called `apps-install` that can install other apps from repositories.

A good first step is to update the `apps-install` script with the command `bs apps-install apps-install`

After the script has been updated, other apps can be installed([Default App List](https://byt3.dev:3785/list)).

A small tool to manage the BS Runtime and its files is called `cliconf` and is recommended to be installed. For a list of commands run `bs cliconf list-all` or `bs cliconf help`


________

Notes:

The app `interactive` is an interactive shell session that is very useful when prototyping or testing.(Install with `bs apps-install interactive`)

________