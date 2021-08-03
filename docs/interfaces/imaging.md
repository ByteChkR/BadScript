# Imaging Interface

Provides Functionality to open/edit and save bitmaps

## Objects

### Color
A Table of Color Components

#### Properties

##### A
The Alpha Component of the Color

##### R
The Red Component of the Color

##### G
The Green Component of the Color

##### B
The Blue Component of the Color

### Bitmap

#### Functions

##### getWidth()
Returns the Width of the Image

##### getHeight()
Returns the Height of the Image

##### setPixel(x, y, color)
Sets the Specified pixel to the specified color

##### getPixel(x, y)
Returns the Color of the specified pixel

##### rotateFlip(flipType)
Flips the Bitmap bases on the specified flip type

##### serialize()
Returns an array of bytes that the bitmap was serialized into

##### dispose()
Unload the Image from memory

## Functions

### loadImage(data)
Deserializes an array of bytes into a bitmap structure

### createImage(width, height)
Create new Empty Image with the specified dimensions

### color(r, g, b)/color(a, r, g, b)
Create a Color Object with the specified components
If Alpha is not specified the color will be fully opaque

### rotateFlipType
Enum that defines the possible flips a bitmap can do.