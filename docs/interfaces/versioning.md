# Versioning Interface

Provides functionality to work with versions.

## Objects

### Version Object


#### Properties

##### major
Major Version Number

##### minor
Minor Version Number

##### majorRevision
Major Revision Number

##### minorRevision
Minor Revision Number

##### revision
Revision Number

##### build
Build Number


#### Functions

##### change(changeStr)
Modifies the version based on the changeString

## Functions

### parse(str)
Parses a version object from the specified string

### create(major, minor, revision, build)
Creates a new version object

### calVer(build)
Creates a new version object with the format: `YYYY.MM.DD.build`