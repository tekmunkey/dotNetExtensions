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

