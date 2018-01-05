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
        /// The UTF-8 Byte Order Mark, even though it isn't really relevant to the byte order.
        /// </summary>
        public static byte[] BOM = new byte[] { 0xef, 0xbb, 0xbf };
        /// <summary>
        /// The UTF-8 Byte Order Mark in reverse order, even though it isn't really relevant to the byte order.  
        /// 
        /// Provided for error correction if dealing with poorly implemented text editor products.
        /// </summary>
        public static byte[] MOB = new byte[] { 0xbf, 0xbb, 0xef };

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
            char[] r = new char[0];

            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] < 0x80)
                    {
                        // Any value in range of 0x00 up to 0x7f is ASCII standard single-byte char
                        arrayExtensions.push<char>(ref r, (char)data[i]);
                    }
                    else if ((data[i] > 0xc1) && (data[i] < 0xe0))
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

                        if ((i + 1) < data.Length)
                        {
                            // xor 1100 0000 off the current data to obtain its bitwise character value
                            // bitshift 6 times leftward to place the bitwise character value in the "most significant bits" position
                            int newChar = ((data[i] ^ 0xc0) << 6); 
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // or that character value into the "least significant bits" position for newChar
                            newChar = newChar | (data[i] ^ 0x80);
                            arrayExtensions.push<char>(ref r, (char)newChar);
                        }
                        else
                        {
                            // throw exception or call callback or raise event alerting that invalid data is detected - not enough data to continue UTF-8 decode operation
                            throw new System.Text.DecoderFallbackException(@"Invalid UTF-8 data (2-byte codepoint detected, but no more data to read).", data, i);
                        }
                        
                    }
                    else if ((data[i] > 0xdf) && (data[i] < 0xf0))
                    {
                        //
                        // Most values in range of 0xe0 up to 0xef is a valid 3-byte codepoint
                        //   * 0xd0 could start overlong encodings
                        //   * 0xed can start the encoding of a code point in the range U+d800-U+dfff which is invalid since they are reserved for UTF-16 surrogate halves
                        //
                        if ((i + 2) < data.Length)
                        {
                            // xor 1110 0000 off the current data to obtain its bitwise character value
                            // bitshift 8 times leftward to place the bitwise character value in the "most significant bits" position
                            int newChar = ((data[i] ^ 0xe0) << 12);
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // bitshift 4 times leftward to place the bitwise character value in the "more significant bits" position
                            // or that character value into newChar
                            newChar = newChar | ((data[i] ^ 0x80) << 6);
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // or that character value into the "least significant bits" position into newChar
                            newChar = newChar | (data[i] ^ 0x80);
                            arrayExtensions.push<char>(ref r, (char)newChar);
                        }
                        else
                        {
                            // throw exception or call callback or raise event alerting that invalid data is detected - not enough data to continue UTF-8 decode operation
                            throw new System.Text.DecoderFallbackException(@"Invalid UTF-8 data (3-byte codepoint detected, but no more data to read).", data, i);
                        }
                    }
                    else if ((data[i] > 0xef) && (data[i] < 0xf5))
                    {
                        //
                        // Most values in range of 0xf0 up to 0xf4 is a valid 4-byte codepoint
                        //   * 0xf0 could start overlong encodings
                        //   * 0xf4 can start code points greater than U+10ffff which are invalid
                        //
                        if ((i + 3) < data.Length)
                        {
                            // xor 1111 0000 off the current data to obtain its bitwise character value
                            // bitshift 12 times leftward to place the bitwise character value in the "most significant bits" position
                            int newChar = ((data[i] ^ 0xf0) << 18);
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // bitshift 8 times leftward to place the bitwise character value in the "more significant bits" position
                            // or that character value into newChar
                            newChar = newChar | ((data[i] ^ 0x80) << 12);
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // bitshift 4 times leftward to place the bitwise character value in the "less significant bits" position
                            // or that character value into newChar
                            newChar = newChar | ((data[i] ^ 0x80) << 6);
                            // increment the byte index into data
                            i++;
                            // xor 1000 0000 off the next byte of data to obtain its bitwise character value
                            // or that character value into the "least significant bits" position into newChar
                            newChar = newChar | (data[i] ^ 0x80);
                            arrayExtensions.push<char>(ref r, (char)newChar);
                        }
                        else
                        {
                            // throw exception or call callback or raise event alerting that invalid data is detected - not enough data to continue UTF-8 decode operation
                            throw new System.Text.DecoderFallbackException(@"Invalid UTF-8 data (4-byte codepoint detected, but no more data to read).", data, i);
                        }
                    }
                }
            }

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
            byte[] r = new byte[0];

            for (int i = 0; i < fromChars.Length; i++)
            {
                uint thisChar = (uint)fromChars[i];
                uint newChar = 0;
                if (thisChar < 0x00000080)
                {
                    // Any value in range of 0x00 up to 0x7f is ASCII standard single-byte char
                    arrayExtensions.push<byte>(ref r, (byte)thisChar);
                }
                else if ((thisChar > 0x0000007f) && (thisChar < 0x00000800))
                {
                    //
                    // Any value in range of 0x80 up to 0x07ff requires a 2-byte codepoint
                    //
                    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the most significant bits are stored here
                    // or on 1111 1111 1110 0000 so the bits are definitely set, then xor 1111 1111 1110 0000 so the bits are definitely unset
                    // then or on 0000 0000 1100 0000 indicating that this is the first in a 2-byte sequence
                    newChar = ((((thisChar >> 6) | 0xffffffe0) ^ 0xffffffe0) | 0x000000c0);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    //
                    // Well, DotNET does some real stupidity with bitshifting, namely that if you bitshift left until you shift unwanted bits all the way off a signed value and then bring them back to the right, 
                    // it drags back 1s instead of 0s.  Because that makes total sense.  Nobody ever shifted 1s off a value.  Nope! XD
                    //
                    // or on 1111 1111 1100 0000 so the bits are definitely set, then xor 1111 1111 1100 0000 so the bits are definitely unset
                    // then or on 0000 0000 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                    //
                    newChar = ((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080;
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                }
                else if ((thisChar > 0x000007ff) && (thisChar < 0x00010000))
                {
                    //
                    // Any value in the range of 0x0800 up to 0xffff  requires a 3-byte codepoint
                    //
                    // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the most significant bits are stored here
                    // or on 1111 0000 so the bits are definitely set, then xor 1111 0000 so the bits are definitely unset
                    // then or on 1110 0000 indicating that this is the first in a 3-byte sequence
                    newChar = ((((thisChar >> 12) | 0xfffffff0) ^ 0xfffffff0) | 0x000000e0);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the less and most significant bits are stored here
                    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
                    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
                    newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    // no bitshift this time
                    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
                    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                    newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                }
                else if ((thisChar > 0x0000ffff) && (thisChar < 0x00110000))
                {
                    //
                    // Any value in the range of 0x00010000 up to 0x0010ffff requires a 4-byte codepoint
                    //
                    // bitshift right 18 times to drop the least and less and more significant bits from thisChar, ensuring that only the most significant bits are stored here
                    // or on 1111 1000 so the bits are definitely set, then xor 1111 1000 so the bits are definitely unset
                    // then or on 1111 0000 indicating that this is the first in a 4-byte sequence
                    newChar = ((((thisChar >> 18) | 0xfffffff8) ^ 0xfffffff8) | 0x000000f0);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    // bitshift right 12 times to drop the least and less significant bits from thisChar, ensuring that only the more and most significant bits are stored here
                    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the more significant bits are stored here
                    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point more significant bits
                    newChar = ((((thisChar >> 12) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    // bitshift right 6 times to drop the least significant bits from thisChar, ensuring that only the more and most and less significant bits are stored here
                    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the less significant bits are stored here
                    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point less significant bits
                    newChar = ((((thisChar >> 6) | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
                    // no bitshift this time
                    // or on 1110 0000 so the bits are definitely set, then xor 1110 0000 so the bits are definitely unset - now only the least significant bits are stored here
                    // then or on 1000 0000 so what's properly set is 0000 0000 10xx xxxx vis a vis the UTF-8 code point least significant bits
                    newChar = (((thisChar | 0xffffffc0) ^ 0xffffffc0) | 0x00000080);
                    arrayExtensions.push<byte>(ref r, (byte)newChar);
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
