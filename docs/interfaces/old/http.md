# Http Interface

Interface Key: `http`

Provides HTTP Functions and Features

## Objects

### URI Object
Represents an Uniform Resource Identifier

#### Functions
##### getHost()
Returns the Hostname part of the URI string

##### getLocalPath()
Returns the LocalPath part of the URI string

##### getAuthority()
Returns the Hostname and Port of the URI string

##### getScheme()
Returns the URI Scheme

### Web Response Object

#### Properties

##### status
Contains the Status Code of the Response

##### body
Contains the Body of the response

## Functions

### get(url, headers)
Performs a GET Request to the specified url with the optional headers
Returns a response object containing a `status` and `body` property.

### post(url, body)
Performs a POST Request to the specified url with the optional body
Returns a response object containing a `status` and `body` property.

### downloadFile(url, localFile)
Directly downloads the specified resource to a local file

### downloadString(url)
Directly downloads the specified resource and returns the content interpreted as string.

### createUri(url)
Creates an `URI Object` from the specified string


[List of All Interfaces](./../Interfaces.md)