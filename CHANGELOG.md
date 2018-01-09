# 2017-12-20

* Project Started (not really started this date, but this particular Visual Studio Solution was created on this date)

# 2018-01-03

* Changed up 'bitconverterExtensions' to 'bitwise' since BitConverter really ought to provide the features and functions I've written in, but BitConverter's a really stupid name.  Nothing's actually being converted.

* Added arrayExtensions.insertAt and arrayExtensions.removeAt

* Added stringExtensions.stringProperties.cs with getEncoding and getLineTerm functions, which respectively take filepath or byte array parameters to determine text encoding or line terminators from original text.

* Added stringExtensions.copyStringToString and copyStringToCharArray

* Added stringExtensions.textTrim.cs file containing several functions (trimLeft, trimRight, trimBoth, with each function providing overloads that accept either a string-to-trim or a charArray-to-trim)

# 2018-01-04

* Added arrayExtensions.indexOf and arrayExtensions.contains functions

* Added custom iniFileClass, iniSectionClass, iniKeyClass

* Added and tested encodingExtensions.utf8 against System.Text.Encoding.UTF8 - works identically in encode and decode operations.

# 2018-01-05

* Added arrayExtensions.constrainedCopyBE and constrainedCopyLE

* Added and tested encodingExtensions.utf16 against System.Text.Encoding.Unicode and BigEndianUnicode - works one heck of a lot better.  My version automatically detects big vs little endian encodings in UTF-16 during decode operations, with or without a Byte Order Marker, whether that BOM is accurate or not.

# 2018-01-08

* Added bitwise.isLittleEndian and bitwise.isBigEndian functions to provide demonstrations of how to perform these functions in any programming language, given a platform-based facility to convert integer values to byte arrays.

* Added bitwise.getBytes functions accepting all forms of integer (unsigned/signed 16/32/64 bit integers) and accepting a boolean value specifying whether the output bytes should be Big or Little Endian.  These functions construct the output byte array using only bitshift and logical operations rather than relying on the BitConverter, in order to produce Big and Little Endian output values in a truly platform agnostic way (providing Endian-specific output without knowing or caring about the Endianness of the platform itself).  These can be ported to literally any programming language on any platform to serve the same purpose.

* Added arrayExtensions.compare<T> for unit testing and other purposes (an array compare function equivalent to Enumerable.SequenceEqual, which isn't available for DotNET 2.0)

* Revived bitConverterExtensions in order to provide actual extensions to the BitConverter itself (using BitConverter calls and then adding functionality), since the previous thing called bitconverterExtensions was actually replacement coding in legitimate bitwise functionality.

* In bitConverterExtensions, getBytes functions were added mirroring the functionality provided by bitwise.getBytes.  These are provided both to demonstrate how to do the same task in native DotNET and also to provide unit testing facilities for bitwise.getBytes (in order to test my custom bitwise.getBytes functions against the BitConverter itself, in the corrected Endiannesses).  Documentation for these functions is not yet complete.

# 2018-01-09

* Flushed out documentation for bitConverterExtensions

* Added arrayExtensions.compare overload to compare a range of elements in 2 arrays of arbitrary lengths.

* Added invalidUTF8CodePointHandler delegate, invalidUTF8CodePointEvent, and raiseinvalidUTF8CodePointEvent method providing consumers an even smoother error-correction facility when using this decoding implementation.

* Added getDecodeDataWidth, getDecodeCharCount, and getDecodeChar to encodingExtensions.utf8

* Converted utf8.decodeData to use the new modularized architecture.

* Added getEncodeDataWidth, getEncodeByteCount, and getEncodeData to encodingExtensions.utf8

* Converted utf8.encodeData to use the new modularized architecture.

* Still haven't provided any CODEC facility for file-on-disk or data streams.

