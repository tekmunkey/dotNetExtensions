using System;

namespace dotNetExtensions
{
    /// <summary>
    /// Provides text encoding services in UTF-8.
    /// </summary>
    /// <remarks>
    /// DotNET actually provides a wide array of Encoding options, but saw fit to provide Big Endian Encoding only for UTF-16 and they did 
    /// a really piss-poor job of naming things for folks.  UTF-16 is just "Unicode" and the BE version of UTF-16 is just "BigEndianUnicode" 
    /// and then UTF-32 only provides Little Endian format and nothing else.  
    /// 
    /// Thanks a pantload for, uh, well kind of nothing, Microsoft!  What you've done is shallowly and passive-aggressively marry DotNET 
    /// developers to nothing worth using while steering them subtly away from anything that actually is handy, like the global standard 
    /// UTF-8.  
    /// 
    /// UTF-8 actually works fine in System.Encoding but since I was doing rewrites I figured, hey, might as well redo the whole damn thing.
    /// </remarks>
	public partial class utf8
	{
        /// <summary>
        /// The handler signature for invalidUTFCodePointEvent.
        /// </summary>
        /// <param name="data">
        /// A byte array, passed by reference.  The data that was being decoded when an invalid codepoint was encountered in data.
        /// </param>
        /// <param name="index">
        /// An integer value.  The index into the data where the invalid codepoint begins.  This is an offset, from 0, into the data array.
        /// </param>
        /// <param name="dataWidth">
        /// An integer value.  The data width, or length from index (in bytes), of the invalid codepoint data.
        /// </param>
        /// <param name="message">
        /// A string instance.  A message from the method or function that raises the event, that may contain more information or instructions on handling it.
        /// </param>
        /// <param name="cancel">
        /// A boolean value, passed by reference.  If event handlers/consumers set this value to True, this allows any looping or continuing processes to be aborted or canceled in response to the event.
        /// </param>
        public delegate void invalidUTF8CodePointHandler(ref byte[] data, int index, int dataWidth, string message, ref bool cancel);
        /// <summary>
        /// Event raised when an invalid UTF-8 Codepoint is encountered during UTF-8 decode (including character count and similar) operations.
        /// </summary>
        public static event invalidUTF8CodePointHandler invalidUTF8CodePointEvent = null;
        /// <summary>
        /// Raises the invalidUTF8CodePointEvent in a supremely safe manner.
        /// </summary>
        /// <param name="data">
        /// A byte array, passed by reference.  The data that was being decoded when an invalid codepoint was encountered in data.
        /// </param>
        /// <param name="index">
        /// An integer value.  The index into the data where the invalid codepoint begins.  This is an offset, from 0, into the data array.
        /// </param>
        /// <param name="dataWidth">
        /// An integer value.  The data width, or length from index (in bytes), of the invalid codepoint data.
        /// </param>
        /// <param name="message">
        /// A string instance.  A message from the method or function that raises the event, that may contain more information or instructions on handling it.
        /// </param>
        /// <param name="cancel">
        /// A boolean value, passed by reference.  If event handlers/consumers set this value to True, this allows any looping or continuing processes to be aborted or canceled in response to the event.
        /// </param>
        private static void raiseInvalidUTF8CodePointEvent(ref byte[] data, int index, int dataWidth, string message, ref bool cancel)
        {
            invalidUTF8CodePointHandler ev = invalidUTF8CodePointEvent;
            if (ev != null)
            {
                ev(ref data, index, dataWidth, message, ref cancel);
            }
        }

        /// <summary>
        /// The UTF-8 Byte Order Mark, even though it isn't really relevant to the byte order.
        /// </summary>
        public static byte[] BOM = new byte[] { 0xef, 0xbb, 0xbf };

        /// <summary>
        /// Gets a value indicating whether the specified byte array contains a Byte Order Marker (BOM) at the front.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to test for a UTF-8 BOM.
        /// </param>
        /// <returns>
        /// A boolean value.  True if the data contains a BOM at the front.  Otherwise False.  
        /// </returns>
        public static bool hasBOM(ref byte[] data)
        {
            bool r = false;

            if ((data[0] == BOM[0]) && (data[1] == BOM[1]) && (data[2] == BOM[2]))
            {
                // Big Endian BOM
                r = true;
            }

            return r;
        }

        /// <summary>
        /// Gets the number of bytes required to decode the UTF-8 character (codepoint) that begins with the specified byte.
        /// </summary>
        /// <param name="fromByte">
        /// The byte that begins a UTF-8 sequence.
        /// </param>
        /// <returns>
        /// An integer value.  The number of bytes required to decode the UTF-8 character (codepoint) that begins with fromByte.  
        /// 
        /// This value will range from 0 to 4.  
        /// 
        /// If this function returns 0, then fromByte is not a valid UTF-8 sequence starter (invalid codepoint).  A zero return value represents a UTF-8 decode error.
        /// 
        /// When this function returns 1 through 4, this value will include the 'fromByte' itself, so if a reader receives '1' then it has already read the byte it needs because it has read 'fromByte' in order to pass it to this function or if it receives 2 then it only needs to read 1 more byte to decode the full character.
        /// </returns>
        public static int getDecodeDataWidth(byte fromByte)
        {
            //
            // This function performs a high-performance analysis of the given byte without performing any actual decode operation, providing a modularized facility for decode-support operations.
            //
            int r = 0;

            if (fromByte < 0x80)
            {
                // Any value in range of 0x00 up to 0x7f is ASCII standard single-byte char
                r = 1;
            }
            else if ((fromByte > 0xc1) && (fromByte < 0xe0))
            {
                //
                // Any value in range of 0xc2 up to 0xdf is a valid 2-byte codepoint
                //
                // If currently inspected data is 0xc0, 0xc1, or anything in the range from 0xf5 to 0xff, unicode defines these values 
                // as invalid for UTF-8 codepoints (they must never appear in a UTF-8 sequence)
                //   * c0 and c1 could only be used for a 2-byte encoding of a 7-bit ASCII char which should be encoded in 1 byte and 
                //     would therefore create an 'overlong' sequence condition
                //   * f5 up to ff (the upper limit of the byte value) indicate leading bytes of 4-byte or longer sequences which can 
                //     not be valid because they would encode code points larger than the U+10ffff limit of Unicode (a limit derived 
                //     from the maximum code point encodable in UTF-16) and fe and ff were never defined for any purpose in UTF-8
                //
                r = 2;
            }
            else if ((fromByte > 0xdf) && (fromByte < 0xf0))
            {
                //
                // Most values in range of 0xe0 up to 0xef is a valid 3-byte codepoint
                //   * 0xd0 could start overlong encodings
                //   * 0xed can start the encoding of a code point in the range U+d800-U+dfff which is invalid since they are reserved for UTF-16 surrogate halves
                //
                r = 3;
            }
            else if ((fromByte > 0xef) && (fromByte < 0xf5))
            {
                //
                // Most values in range of 0xf0 up to 0xf4 is a valid 4-byte codepoint
                //   * 0xf0 could start overlong encodings
                //   * 0xf4 can start code points greater than U+10ffff which are invalid
                //
                r = 4;
            }

            return r;
        }

        /// <summary>
        /// Gets the total number of UTF-8 characters (codepoints) represented by the bytes in the specified data array.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to test for a UTF-8 character count.
        /// </param>
        /// <returns>
        /// An integer value.  The total number of UTF-8 characters (codepoints) contained in the raw byte data.
        /// </returns>
        public static int getDecodeCharCount(ref byte[] data)
        {
            //
            // This function provides a high-performance facility for indexing/counting all the decodable characters in a given UTF-8 data array, making it suitable for 
            // advance scouting missions such as initializing the char array in size before performing a real decode operation.
            //   * Rather than perform any actual decode operation, it simply:
            //     :: Obtains the number of bytes required to decode each character
            //     :: Increments the character count by 1
            //     :: Advances the data cursor past the data width
            //     :: Raises invalid codepoint events if any are encountered
            //   ** No, I'm not going to beat the built-in System.Text.Encoding when testing against a System.Diagnostics.Stopwatch.  Theirs is binary compiled, mine is MSIL interpreted.
            //      Apples and oranges, night and day.
            //
            int r = 0;

            if (data != null)
            {
                //
                // Because of the way we have to work our way through the data array, a for loop won't do.  
                // We must loop manually and track the cursor ourselves.
                //
                int i = 0;
                //
                // This toggle-able switch value can be used to halt the decode/count operation prematurely.
                //
                bool cancel = false;
                do
                {
                    // retrieving the next relevant byte from the data array
                    byte b = data[i];
                    // obtaining the width of the data required to decode the sequence that begins with this byte
                    int w = getDecodeDataWidth(b);
                    //
                    // This if clause is error checking.
                    //
                    if ((w > 0) && ((i + w) <= data.Length))
                    {
                        //
                        // Everything looks good
                        //
                        // Increment the character counter
                        //
                        r++;
                        // And advance the data cursor past the data width of this character
                        i += w;
                    }
                    else if (w == 0)
                    {
                        //
                        // (w == 0) means there is a UTF-8 invalid codepoint error to handle
                        //
                        raiseInvalidUTF8CodePointEvent(ref data, i, w, @"Invalid UTF-8 data (Invalid Codepoint) at data index " + i.ToString(), ref cancel);
                    }
                    else if ((i + w) > data.Length)
                    {
                        //
                        // ((i + w) < data.Length) may mean there is a UTF-8 invalid codepoint
                        //   * or it may mean the consumer/reader/programmer made some kind of error in reading or passing the data
                        //   * or it may mean the platform has some kind of memory problem that causes us to be unable to read what we need to
                        //   * it's impossible to know anything for sure except that we need to read more data than there is available
                        //
                        raiseInvalidUTF8CodePointEvent(ref data, i, w, @"Invalid UTF-8 data (" + w.ToString() + @"-byte codepoint detected, but no more data to read) at data index " + i.ToString(), ref cancel);
                    }
                } while (!cancel && (i < data.Length));
            }

            return r;
        }

        /// <summary>
        /// Gets a char decoded from the specified data array at the specified index, using the number of bytes specified by length.
        /// </summary>
        /// <param name="data">
        /// A byte array passed by reference.  The data to decode from.
        /// </param>
        /// <param name="index">
        /// An integer value.  The index into the array to begin decoding from.  An offset from 0.
        /// </param>
        /// <param name="length">
        /// An integer value.  The number of bytes to use in decoding, starting at index.  Must be a value from 1 to 4.
        /// </param>
        /// <returns>
        /// A char (a character constructed from a UTF Codepoint).
        /// </returns>
        public static char getDecodeChar(ref byte[] data, int index, int length)
        {
            //
            // Default output value to 0x2060 
            // UTF-16 Codepoint for Word Joiner, zero-width nonbreaking space provided for disambiguation from BOM
            //   * Used here because wherever DotNET blithely uses the term 'Unicode' they really mean UTF-16, and the System.Char struct 'Represents a Unicode character.'
            //   * Whether this grotesque form is misinformation in documentation is because Microsoft farmed out large portions of their DotNET development (including 
            //     documentation) the lowest bidder in the lowest-dollar-exchange-value 3rd world country they could find or whether they for some reason deliberately wanted 
            //     to sabotage anyone with a legitimate desire to learn about programming and text encoding is anybody's guess.
            // Use of this character as the default output is just a pre-initialized form of error correction.
            char r = (char)0x2060;

            if ((length < 1) | (length > 4))
            {
                throw new System.Exception(@"utf8.getDecodeChar expects a length parameter in the range of 1 to 4.  Got " + length.ToString());
            }

            //
            // Declaring these complex bools outside of the if statement makes for nice, easy readability.
            //   * For indexInRange we're also concerned about the data length, because in this case we only care about the index itself if length is only 1 byte
            //   * So if length is exactly 1 byte and index is inside data bounds
            //
            bool indexInRange = ((length == 1) & (index < data.Length));
            //
            // For lengthInRange, we're only concerned if length is greater than 1 byte
            //   * So if length is more than 1 and index + length is inside data bounds
            //
            bool lengthInRange = ((length > 1) & ((index + length) <= data.Length));
            //
            // The final test then is an or seeking whether either of those cases is true
            //   * Either one will do
            //
            if (indexInRange | lengthInRange)
            {
                if (length == 1)
                {
                    r = (char)data[index];
                }
                else if (length == 2)
                {
                    // xor 1100 0000 off the current data to obtain its bitwise character value
                    // bitshift 6 times leftward to place the bitwise character value in the "most significant bits" position
                    int newChar = ((data[index] ^ 0xc0) << 6);
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // or that character value into the "least significant bits" position for newChar
                    newChar = newChar | (data[index] ^ 0x80);
                    r = (char)newChar;
                }
                else if (length == 3)
                {
                    // xor 1110 0000 off the current data to obtain its bitwise character value
                    // bitshift 8 times leftward to place the bitwise character value in the "most significant bits" position
                    int newChar = ((data[index] ^ 0xe0) << 12);
                    // increment the byte index into data
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // bitshift 4 times leftward to place the bitwise character value in the "more significant bits" position
                    // or that character value into newChar
                    newChar = newChar | ((data[index] ^ 0x80) << 6);
                    // increment the byte index into data
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // or that character value into the "least significant bits" position into newChar
                    newChar = newChar | (data[index] ^ 0x80);
                    r = (char)newChar;
                }
                else if (length == 4)
                {
                    // xor 1111 0000 off the current data to obtain its bitwise character value
                    // bitshift 12 times leftward to place the bitwise character value in the "most significant bits" position
                    int newChar = ((data[index] ^ 0xf0) << 18);
                    // increment the byte index into data
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // bitshift 8 times leftward to place the bitwise character value in the "more significant bits" position
                    // or that character value into newChar
                    newChar = newChar | ((data[index] ^ 0x80) << 12);
                    // increment the byte index into data
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // bitshift 4 times leftward to place the bitwise character value in the "less significant bits" position
                    // or that character value into newChar
                    newChar = newChar | ((data[index] ^ 0x80) << 6);
                    // increment the byte index into data
                    index++;
                    // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                    // or that character value into the "least significant bits" position into newChar
                    newChar = newChar | (data[index] ^ 0x80);

                    r = (char)newChar;
                }
            }
            else
            {
                throw new IndexOutOfRangeException(@"getDecodeChar was passed index (" + index.ToString() + @") and length + (" + length.ToString() + @") values that are outside the bounds of the data array with " + data.Length.ToString() + " elements.");
            }

            return r;
        }

        /// <summary>
        /// Decodes the specified data array from bytes into characters, using the UTF-8 specification.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to decode according to UTF-8 specification.
        /// </param>
        /// <returns>
        /// An array of char instances.
        /// </returns>
        public static char[] decodeData(ref byte[] data)
        {
            // Initialize an output value as null
            char[] r = null; 
            // Then proceed to further operations only if the input data is not null
            if (data != null)
            {
                //
                // Initialize an output array per the decode character counter
                //
                r = new char[getDecodeCharCount(ref data)];
                //
                // Just as I like to use i as an index cursor through most input arrays, I'm going to use o as an index cursor through this output array.
                // This value will increment as chars are added into r[]
                //
                int o = 0;

                //
                // Because of the way we have to work our way through the data array, a for loop won't do.  
                // We must loop manually and track the input data cursor ourselves.
                //
                int i = 0;
                //
                // This toggle-able switch value can be used to halt the decode operation prematurely.
                //
                bool cancel = false;
                do
                {
                    // retrieving the next relevant byte from the data array
                    byte b = data[i];
                    // obtaining the width of the data required to decode the sequence that begins with this byte
                    int w = getDecodeDataWidth(b);

                    //
                    // This if clause is error checking.
                    //
                    if ((w > 0) && ((i + w) <= data.Length))
                    {
                        //
                        // Everything looks good
                        //

                        // Add the decoded character to r[]
                        r[o] = getDecodeChar(ref data, i, w);
                        // Increment the output cursor
                        o++;
                        
                        // And advance the input data cursor past the data width of this character
                        i += w;
                    }
                    else if (w == 0)
                    {
                        //
                        // (w == 0) means there is a UTF-8 invalid codepoint error to handle
                        //
                        raiseInvalidUTF8CodePointEvent(ref data, i, w, @"Invalid UTF-8 data (Invalid Codepoint) at data index " + i.ToString(), ref cancel);
                    }
                    else if ((i + w) > data.Length)
                    {
                        //
                        // ((i + w) < data.Length) may mean there is a UTF-8 invalid codepoint
                        //   * or it may mean the consumer/reader/programmer made some kind of error in reading or passing the data
                        //   * or it may mean the platform has some kind of memory problem that causes us to be unable to read what we need to
                        //   * it's impossible to know anything for sure except that we need to read more data than there is available
                        //
                        raiseInvalidUTF8CodePointEvent(ref data, i, w, @"Invalid UTF-8 data (" + w.ToString() + @"-byte codepoint detected, but no more data to read) at data index " + i.ToString(), ref cancel);
                    }
                } while (i < data.Length);
            }

            return r;
        }

        /// <summary>
        /// Gets the number of bytes required to encode the specified char (Unicode Codepoint).
        /// </summary>
        /// <param name="fromChar">
        /// A char value.  The codepoint to be encoded.
        /// </param>
        /// <returns>
        /// An integer value.  The number of bytes necessary to encode the specified char in UTF-8 specification..
        /// </returns>
        public static int getEncodeDataWidth(char fromChar)
        {
            // Return value will always be at least 1.
            int r = 1;
            //
            // Convert input char to int value for value testing
            //
            int c = (int)fromChar;

            //
            // Any value in range of 0x00 up to 0x7f is ASCII standard single-byte char.  No if required due to pre-initialization.
            //

            if ((c > 0x0000007f) && (c < 0x00000800))
            {
                //
                // Any value in range of 0x80 up to 0x07ff requires a 2-byte codepoint
                //
                r = 2;
            }
            else if ((c > 0x000007ff) && (c < 0x00010000))
            {
                //
                // Any value in the range of 0x0800 up to 0xffff  requires a 3-byte codepoint
                //
                //
                r = 3;
            }
            else if ((c > 0x0000ffff) && (c < 0x00110000))
            {
                //
                // Any value in the range of 0x00010000 up to 0x0010ffff requires a 4-byte codepoint
                //
                //
                r = 4;
            }
            //
            // 0x00110000 and up would be invalid UTF Codepoints, but would (hopefully) never appear in DotNET char instances.
            // Handling for this eventuality can be added if necessary
            //
            

            return r;
        }

        /// <summary>
        /// Gets the total number of bytes required to encode the specified char array into UTF-8 data.
        /// </summary>
        /// <param name="fromChars">
        /// An array of chars, passed by reference.  The characters to test for an encoding data count.
        /// </param>
        /// <returns>
        /// An integer value.  The total number of bytes required to encode the specified characters as UTF-8 data.
        /// </returns>
        public static int getEncodeByteCount(ref char[] fromChars)
        {
            int r = 0;

            if (fromChars != null)
            {
                for (int i = 0; i < fromChars.Length; i++)
                {
                    r += getEncodeDataWidth(fromChars[i]);
                }
            }

            return r;
        }

        /// <summary>
        /// Gets a byte array representing the specified character encoded in UTF-8.
        /// </summary>
        /// <param name="fromChar">
        /// The character to encode in UTF-8.
        /// </param>
        /// <returns>
        /// An array of bytes.  The specified character encoded in UTF-8.
        /// </returns>
        public static byte[] getEncodeData(char fromChar)
        {
            byte[] r = new byte[getEncodeDataWidth(fromChar)];

            uint thisChar = (uint)fromChar;
            uint newChar = 0;

            if (r.Length > 3)
            {
                // this must be a 4-byte value
                //
                // bitshift right 18 times to drop the least and less and more significant bits from thisChar, ensuring that only the most significant bits are stored here
                // or on 1111 1000 so the bits are definitely set, then xor 1111 1000 so the bits are definitely unset
                // then or on 1111 0000 indicating that this is the first in a 4-byte sequence
                //
                newChar = ((((thisChar >> 18) | 0xfffffff8) ^ 0xfffffff8) | 0x000000f0);
                r[0] = (byte)newChar;
                //
                // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the more and most significant bits are stored here
                // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the more significant bits are stored here
                // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point more significant bits
                //
                newChar = ((((thisChar >> 12) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                r[1] = (byte)newChar;
                //
                // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the more and most and less significant bits are stored here
                // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
                // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
                //
                newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                r[2] = (byte)newChar;
                //
                // no bitshift this time
                // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
                // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                //
                newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                r[3] = (byte)newChar;
            }
            else if (r.Length > 2)
            {
                // this must be a 3-byte value
                //
                // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the most significant bits are stored here
                // or on 1111 0000 so the bits are definitely set, then xor 1111 0000 so the bits are definitely unset
                // then or on 1110 0000 indicating that this is the first in a 3-byte sequence
                //
                newChar = ((((thisChar >> 12) | 0xfffffff0) ^ 0xfffffff0) | 0x000000e0);
                r[0] = (byte)newChar;
                //
                // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the less and most significant bits are stored here
                // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
                // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
                //
                newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                r[1] = (byte)newChar;
                //
                // no bitshift this time
                // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
                // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                //
                newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                r[2] = (byte)newChar;
            }
            else if (r.Length > 1)
            {
                // this must be a 2-byte value
                //
                // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the most significant bits are stored here
                // or on 1111 1111 1110 0000 so the bits are definitely set, then xor 1111 1111 1110 0000 so the bits are definitely unset
                // then or on 0000 0000 1100 0000 indicating that this is the first in a 2-byte sequence
                //
                newChar = ((((thisChar >> 6) | 0xffffffe0) ^ 0xffffffe0) | 0x000000c0);
                r[0] = (byte)newChar;
                //
                // Well, DotNET does some real stupidity with bitshifting, namely that if you bitshift left until you shift unwanted bits all the way off a signed value and then bring them back to the right, 
                // it drags back 1s instead of 0s.  Because that makes total sense.  Nobody ever shifted 1s off a value.  Nope! XD
                //
                // or on 1111 1111 1100 0000 so the bits are definitely set, then xor 1111 1111 1100 0000 so the bits are definitely unset
                // then or on 0000 0000 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                //
                newChar = ((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080;
                r[1] = (byte)newChar;
            }
            else
            {
                // this must be a 1-byte value
                r[0] = (byte)thisChar;
            }

            //
            // The following is best possible precision - but not best possible performance
            //   * Keeping it here, commented, in case the above doesn't work out for any reason we can always fall back on precision
            //   * Precision was actually already performed during the callout to getEncodeDataWidth() in the first place
            //
            //if (thisChar < 0x00000080)
            //{
            //    // Any value in range of 0x00 up to 0x7f is ASCII standard single-byte char
            //    r[0] = (byte)thisChar;
            //}
            //else if ((thisChar > 0x0000007f) && (thisChar < 0x00000800))
            //{
            //    //
            //    // Any value in range of 0x80 up to 0x07ff requires a 2-byte codepoint
            //    //
            //    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the most significant bits are stored here
            //    // or on 1111 1111 1110 0000 so the bits are definitely set, then xor 1111 1111 1110 0000 so the bits are definitely unset
            //    // then or on 0000 0000 1100 0000 indicating that this is the first in a 2-byte sequence
            //    newChar = ((((thisChar >> 6) | 0xffffffe0) ^ 0xffffffe0) | 0x000000c0);
            //    r[0] = (byte)newChar;
            //    //
            //    // Well, DotNET does some real stupidity with bitshifting, namely that if you bitshift left until you shift unwanted bits all the way off a signed value and then bring them back to the right, 
            //    // it drags back 1s instead of 0s.  Because that makes total sense.  Nobody ever shifted 1s off a value.  Nope! XD
            //    //
            //    // or on 1111 1111 1100 0000 so the bits are definitely set, then xor 1111 1111 1100 0000 so the bits are definitely unset
            //    // then or on 0000 0000 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
            //    //
            //    newChar = ((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080;
            //    r[1] = (byte)newChar;
            //}
            //else if ((thisChar > 0x000007ff) && (thisChar < 0x00010000))
            //{
            //    //
            //    // Any value in the range of 0x0800 up to 0xffff  requires a 3-byte codepoint
            //    //
            //    // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the most significant bits are stored here
            //    // or on 1111 0000 so the bits are definitely set, then xor 1111 0000 so the bits are definitely unset
            //    // then or on 1110 0000 indicating that this is the first in a 3-byte sequence
            //    newChar = ((((thisChar >> 12) | 0xfffffff0) ^ 0xfffffff0) | 0x000000e0);
            //    r[0] = (byte)newChar;
            //    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the less and most significant bits are stored here
            //    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
            //    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
            //    newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
            //    r[1] = (byte)newChar;
            //    // no bitshift this time
            //    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
            //    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
            //    newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
            //    r[2] = (byte)newChar;
            //}
            //else if ((thisChar > 0x0000ffff) && (thisChar < 0x00110000))
            //{
            //    //
            //    // Any value in the range of 0x00010000 up to 0x0010ffff requires a 4-byte codepoint
            //    //
            //    // bitshift right 18 times to drop the least and less and more significant bits from thisChar, ensuring that only the most significant bits are stored here
            //    // or on 1111 1000 so the bits are definitely set, then xor 1111 1000 so the bits are definitely unset
            //    // then or on 1111 0000 indicating that this is the first in a 4-byte sequence
            //    newChar = ((((thisChar >> 18) | 0xfffffff8) ^ 0xfffffff8) | 0x000000f0);
            //    r[0] = (byte)newChar;
            //    // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the more and most significant bits are stored here
            //    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the more significant bits are stored here
            //    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point more significant bits
            //    newChar = ((((thisChar >> 12) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
            //    r[1] = (byte)newChar;
            //    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the more and most and less significant bits are stored here
            //    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
            //    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
            //    newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
            //    r[2] = (byte)newChar;
            //    // no bitshift this time
            //    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
            //    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
            //    newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
            //    r[3] = (byte)newChar;
            //}

            return r;
        }

        /// <summary>
        /// Encodes the specified char array into byte data, according to the UTF-8 specification.
        /// </summary>
        /// <param name="fromChars">
        /// The char array to encode to data according to UTF-8.
        /// </param>
        /// <returns>
        /// A byte array.  The UTF-8 encoded data.
        /// </returns>
        public static byte[] encodeData(ref char[] fromChars)
        {
            byte[] r = null;

            if (fromChars != null)
            {
                r = new byte[getEncodeByteCount(ref fromChars)];
                int o = 0;

                for (int i = 0; i < fromChars.Length; i++)
                {
                    //
                    // Retrieve the encoded character data from the current input index
                    //
                    byte[] ec = getEncodeData(fromChars[i]);
                    // Copy the encoded character data into the output data
                    Array.ConstrainedCopy(ec, 0, r, o, ec.Length);
                    // Advance the output cursor by encode-data length
                    o += ec.Length;
                    ec = null;
                }
            }
            return r;
        }

        public static char[] decodeFile(string filePath)
        {
            char[] r = null;

            return r;
        }
	}
}
