# Http Server Interface

Interface Key: `http-server`

Provides a HTTP Server Implementation with SSL Support and concurrent requests

## Objects

### HTTPServer Listener Object
Represents the HTTP Server
#### Functions

##### start()
Starts the Listener

##### stop()
Stops the Listener

#### Properties

##### isRunning
Is true if the listener is active

### HTTPServer Context Object
Represents a full HTTP Transaction

#### Functions

##### request()
Returns the `HTTPServer Request Object` of this Context

##### response()
Returns the `HTTPServer Response Object` of this Context

### HTTPServer Request Object
Represents a Request from a Client

#### Functions

#### Properties

##### headers
A table of headers that were passed by the requesting system

##### uri
The Raw Uri String of the Request

##### acceptTypes
A list of accepted content types

##### contentLength
The Length of the Body

##### contentType
The Type of content that the body contains

##### query
A table of Query Parameters

#### Functions

##### readBody()
returns the body as string representation

### HTTPServer Response Object
Represents the Response to a Request

#### Properties

##### headers
A table of headers that will be passed when sending the response

#### Functions

##### addHeader(key, value)
Adds a header to the response

##### redirect(url)
Redirect the Request to the specified url

##### writeBody(bodyStr)
Writes the Body of the Response and closes the response(sending it in the process)

##### setStatus(statusCode)/setStatus(statusCode, message)
Sets the Specified status code and message.