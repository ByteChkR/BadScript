# IO Interface

Provides Functionality that allows accessing the local disk

## Object

### FileSystem Stream Object

A Wrapper for a C# FileStream Object

#### Functions

##### close()
Closes the underlying file stream

##### write(str)
Writes the specified String to the stream

##### writeb(data)
Writes the specified binary data array to the stream

##### writeLine(str)
Writes the specified String(followed by a newline) to the stream

##### readLine()
Reads a line from the stream

##### readbAll()
Reads all contents and returns it as a binary array

##### readAll()
Reads all contents and returns it as string

##### getPosition()
Returns the current position in the stream

##### setPosition(pos)
Sets the specified position in the stream

##### getLength()
Returns the total length of the stream

##### setLength(len)
Sets the total length of the stream

## Functions

### exists(path)
returns true if the specified path exists

### setCurrentDir(path)
Sets the Current Directory

### getFiles(path, searchpattern, recurse)
Returns a list of all files that match the searchpattern

### getDirectories(path, searchpattern, recurse)
Returns a list of all directories that match the searchpattern

### getCurrentDir()
Returns the current directory

### isDir(path)
Returns true if a directory with that name exists

### isFile(path)
Returns true if a file with that name exists

### createDir(path)
Creates the specified directory

### copy(source, destination)
Copies the specified source file or directory to the specified destination

### move(source, destination)
Moves the specified source file or directory to the specified destination

### delete(path, recurse)
Deletes the specified file or directory

### crc(path)
Computes the CRC32 Checksum of the specified file

### open(path)
Opens the Specified File and returns a `FileSystem Stream Object`

### writeAll(path, data)
Writes all text to the specified path

### readAll(path)
Reads all text from the specified path