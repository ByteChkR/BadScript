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

3 . Move Build to `~/BadScriptEngine`

5 . `chmod +x bs`

Optional Steps:

6 . Make Alias to access `bs` anywhere by adding `alias bs="<home>/BadScriptEngine/bs"` to `~/.bashrc`

7 . Reload Bash Environment Variables with `source ~/.bashrc`

8 . Test Alias with `type -a bs`. It should print `bs is aliased to '/home/tim/BadScriptEngine/bs'`

________

## Install on Windows
1 . Build the Project with `dotnet publish -c Release -r "win-x64"`

2 . Move Build to desired install directory

Optional Steps:

3 . Add BS Shell Opener by writing a small powershell script with content. Usage: `PS> . <path/to/script>`

```powershell
# Path of the Install Directory(location of bs.exe)
$bs = "<Path of Install Directory>"

# Make Sure that the Path is fully qualified
$bs = (Get-Item $bs).fullname

# Add to Environment Path Variable, Only for this powershell session
$Env:Path += ";$bs" 
```

4 . Add Second Powershell Script that Opens a new Powershell Window with the First Script as start command.

```powershell
# Create New Powershell Window and run the BS Shell Opener.
# Shell can now be opened by doubleclicking this file
start PowerShell -ArgumentList "-noexit -command . <path/to/first/script>"
```

5 . Test Setup with `echo $Env:Path`(should print the Install Directory as one of the last entries).


____
[Console Documentation](./Console.md)