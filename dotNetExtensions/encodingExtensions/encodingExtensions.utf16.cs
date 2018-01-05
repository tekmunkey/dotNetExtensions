using System;

namespace dotNetExtensions
{
    /// <summary>
    /// Provides text encoding services in UTF-16.
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
    /// UTF-16 (aka Unicode) actually works fine in System.Encoding but since I was doing rewrites I figured, hey, might as well redo the whole damn thing.  
    /// 
    /// While I was at it, I went ahead and improved UTF-16 (I think) about a thousandfold by providing an automagical facility for detecting and identifying 
    /// big and little endian UTF-16 data all in one tight, compact little package.  Now you don't have to dick around with guessing at your character encoding, 
    /// you can just open it up and it goes.
    /// </remarks>
    public partial class utf16
    {
        /// <summary>
        /// The UTF-16 Byte Order Mark, which (unlike UTF-8) is very relevant to the byte order.  
        /// 
        /// This is what the BOM would look like if it specified Big Endian format.
        /// </summary>
        public static byte[] BOM = new byte[] { 0xfe, 0xff };

        /// <summary>
        /// Gets a value indicating whether the specified byte array contains a Byte Order Marker (BOM) at the front.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to test for a UTF-16 BOM.
        /// </param>
        /// <returns>
        /// A boolean value.  True if the data contains a BOM at the front.  Otherwise False.  
        /// 
        /// Both Big Endian and Little Endian BOMs are searched.
        /// </returns>
        public static bool hasBOM(ref byte[] data)
        {
            bool r = false;

            if ((data[0] == BOM[0]) && (data[1] == BOM[1]))
            {
                // Big Endian BOM
                r = true;
            }
            else if ((data[1] == BOM[0]) && (data[0] == BOM[1]))
            {
                // Little Endian BOM
                r = true;
            }

            return r;
        }

        /// <summary>
        /// Given the 'w1' portion of a UTF-16 decoder operation, returns an integer instruction representing the next step in the process.
        /// </summary>
        /// <param name="w1">
        /// An unsigned 16-bit integer value.  
        /// 
        /// This should be the first word from a sequence, never the second.
        /// </param>
        /// <returns>
        /// An 8-bit unsigned integer value representing a procedure instruction.  
        ///
        /// 0x00 means Terminate.  Do nothing more.  Add character to output.  
        /// 
        /// 0x01 means read additional word.  
        /// 
        /// 0xff means error
        /// </returns>
        public static byte decodeNextOp(ushort w1)
        {
            byte r = 0xff;

            if ((w1 < 0xd800) || (w1 > 0xdfff))
            {
                //
                // If W1 < 0xD800 or W1 > 0xDFFF, the character value U is the value of W1.  Terminate
                //
                r = 0x00;
            }
            else if ((w1 > 0xd7ff) && (w1 < 0xdc00))
            {
                //
                // Determine if W1 is between 0xD800 and 0xDBFF.  This begins a 2-word (4-byte) sequence
                //   * If not, the sequence is in error and no valid character can be obtained using W1. Terminate.
                //
                r = 0x01;
            }
            else
            {
                // Determine if W1 is between 0xD800 and 0xDBFF. If not, the sequence is in error and no valid character can be obtained using W1. Terminate.
                r = 0xff;
            }

            return r;
        }

        /// <summary>
        /// Given the 'w1' and 'w2' portions of the UTF-16 decoder operation, returns an integer value representing the UTF-16 character point decoded from those values.
        /// </summary>
        /// <param name="w1">
        /// An unsigned 16-bit integer value.  
        /// 
        /// This should be the first word from a sequence.
        /// </param>
        /// <param name="w2">
        /// This should be the second word from a sequence.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer value representing a char value.  
        /// 
        /// A return value of -1 represents an error condition.
        /// </returns>
        public static int decodeTwoWord(ushort w1, ushort w2)
        {
            int r = -1;

            if (!((w1 > 0xd7ff) && (w1 < 0xdc00)) || !((w2 > 0xdbff) && (w2 < 0xe000)))
            {
                // Determine if W1 is between 0xD800 and 0xDBFF. If not, the sequence is in error and no valid character can be obtained using W1. Terminate.
                // If W2 is not between 0xDC00 and 0xDFFF, the sequence is in error. Terminate.
                r = -1;
            }
            else
            {
                // need to blank all but the lower 10 bits of w1 and w2
                //   * The stupid-looking casts of ushort or/xor back to ushort?  That's because DotNET very helpfully casts any or/xor operations to int for no apparent reason
                // or 1111 1100 0000 0000 to ensure all those bits are set, then xor the same to ensure all those bits are unset
                // this blanks any metadata bits while leaving the character data bits intact
                w1 = (ushort)((w1 | 0xfc00) ^ 0xfc00);
                w2 = (ushort)((w2 | 0xfc00) ^ 0xfc00);
                //
                // Create a new int and or w1 into it, then bitshift it 10 places to the left to place its value into the 'most significant bits' position
                // then bitshift 10 places left to make way for the low-order bits from w2
                //   * It may make more sense to use a uint here, but that would result in unnecessary casting/performance dragging.  We're only using 20 bits here so the signing 
                //     bit would never come into play anyway.
                //
                r = (w1 << 10);
                // Now or in w2
                r = r | w2;
                //
                // Construct a 20-bit unsigned integer U', taking the 10 low-order bits of W1 as its 10 high-order bits and the 10 low-order bits of W2 as its 10 low-order bits.
                // 
                // Add 0x10000 to U' to obtain the character value U. Terminate.
                //
                r += 0x00010000;
            }

            return r;
        }

        /// <summary>
        /// Gets a value indicating whether the specified byte array contains UTF-16 data in Big Endian format.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to test for Big Endian format.
        /// </param>
        /// <returns>
        /// A boolean value.  True if the data appears to be in Big Endian format.  Otherwise false.
        /// </returns>
        /// <remarks>
        ///
        /// For the IANA registered charsets UTF-16BE and UTF-16LE, a byte order mark should not be used because the names of these character sets already determine the byte order. If encountered 
        /// anywhere in such a text stream, U+FEFF is to be interpreted as a "zero width no-break space".
        ///
        /// Clause D98 of conformance (section 3.10) of the Unicode standard states, "The UTF-16 encoding scheme may or may not begin with a BOM. However, when there is no BOM, and in the absence 
        /// of a higher-level protocol, the byte order of the UTF-16 encoding scheme is big-endian." Whether or not a higher-level protocol is in force is open to interpretation. Files local to a 
        /// computer for which the native byte ordering is little-endian, for example, might be argued to be encoded as UTF-16LE implicitly. Therefore, the presumption of big-endian is widely ignored. 
        /// When those same files are accessible on the Internet, on the other hand, no such presumption can be made. Searching for 16-bit characters in the ASCII range or just the space character 
        /// (U+0020) is a method of determining the UTF-16 byte order.
        ///
        /// The W3C/WHATWG encoding standard used in HTML5 specifies that content labelled either "utf-16" or "utf-16le" are to be interpreted as little-endian "to deal with deployed content".[12] 
        /// However, if a byte-order mark is present, then that BOM is to be treated as "more authoritative than anything else".[13]
        ///
        /// ** From RFC 2781 **
        /// 
        /// It is important to understand that the character 0xFEFF appearing at any position other than the beginning of a stream MUST be interpreted with the semantics for the zero-width non-breaking 
        /// space, and MUST NOT be interpreted as a byte-order mark. The contrapositive of that statement is not always true: the character 0xFEFF in the first position of a stream MAY be interpreted as 
        /// a zero-width non-breaking space, and is not always a byte-order mark. For example, if a process splits a UTF-16 string into many parts, a part might begin with 0xFEFF because there was a 
        /// zero-width non-breaking space at the beginning of that substring.
        /// 
        /// /// </remarks>
        public static bool isBigEndian(ref byte[] data)
        {
            //
            // ****************************************
            //    REPEATING THE REMARKS DOCUMENTATION
            // ****************************************
            //
            // For the IANA registered charsets UTF-16BE and UTF-16LE, a byte order mark should not be used because the names of these character sets already determine the byte order. If encountered 
            // anywhere in such a text stream, U+FEFF is to be interpreted as a "zero width no-break space".
            //
            // Clause D98 of conformance (section 3.10) of the Unicode standard states, "The UTF-16 encoding scheme may or may not begin with a BOM. However, when there is no BOM, and in the absence 
            // of a higher-level protocol, the byte order of the UTF-16 encoding scheme is big-endian." Whether or not a higher-level protocol is in force is open to interpretation. Files local to a 
            // computer for which the native byte ordering is little-endian, for example, might be argued to be encoded as UTF-16LE implicitly. Therefore, the presumption of big-endian is widely ignored. 
            // When those same files are accessible on the Internet, on the other hand, no such presumption can be made. Searching for 16-bit characters in the ASCII range or just the space character 
            // (U+0020) is a method of determining the UTF-16 byte order.
            //
            // The W3C/WHATWG encoding standard used in HTML5 specifies that content labelled either "utf-16" or "utf-16le" are to be interpreted as little-endian "to deal with deployed content".[12] 
            // However, if a byte-order mark is present, then that BOM is to be treated as "more authoritative than anything else".[13]
            //
            /// ** From RFC 2781 **
            /// 
            /// It is important to understand that the character 0xFEFF appearing at any position other than the beginning of a stream MUST be interpreted with the semantics for the zero-width non-breaking 
            /// space, and MUST NOT be interpreted as a byte-order mark. The contrapositive of that statement is not always true: the character 0xFEFF in the first position of a stream MAY be interpreted as 
            /// a zero-width non-breaking space, and is not always a byte-order mark. For example, if a process splits a UTF-16 string into many parts, a part might begin with 0xFEFF because there was a 
            /// zero-width non-breaking space at the beginning of that substring.
            /// 
            // ****************************************
            //    REPEATING THE REMARKS DOCUMENTATION
            // ****************************************
            //
            // hasBOM is ultimately provided for educational/informative purposes only, if anybody ever wants to inspect this code via breakpoints at runtime.
            // It won't be used to actually determine data endianness in any way
            //
            bool hasBOM = false;
            bool isDataBE = false;
            bool r = true;

            // dataCursor will just tell our iterator (when we get to it) whether to jump ahead of the BOM or not when it starts up
            int dataCursor = 0;
            if ((data[0] == BOM[0]) && (data[1] == BOM[1]))
            {
                // Big Endian BOM
                hasBOM = true;
                r = true;
                dataCursor = 2;
            }
            else if ((data[1] == BOM[0]) && (data[0] == BOM[1]))
            {
                // Little Endian BOM
                hasBOM = true;
                r = false;
                dataCursor = 2;
            }

            //
            // These two data buffers will always contain the same data, but will be used to discern data endianness by the following method:
            //   wBuf0 will have its data reversed only if the system is little endian
            //   wBuf1 will have its data reversed only if the system is big endian
            //   Both buffers will then have their values tested for UTF-16 validity up to 512 test cases
            // In this fashion we will ensure that no matter what format the file is stored in or whether the BOM is or is not accurate (or is or is not a BOM at all), we will quickly and accurately 
            // test both the raw data and the raw data's inverse and return a value determined by the system's endianness indicating which endianness best represents the data.
            //
            byte[] wBuf0 = new byte[2]; // a BE testing buffer - if the system is little endian, this array data will ALWAYS reverse
            byte[] wBuf1 = new byte[2]; // a LE testing buffer
            // testMaximum is the maximum number of characters we'll decode to consider a valid/adequate test
            int testMaximum = 512;
            // testCounter is a running total of the number of characters actively decoded in this specific test
            int testCounter = 0;
            //
            // These simple counters will suffice to tally up the results between BigEndian and LittleEndian hits as we test through character decodes in the data
            //
            int isBE = 0;
            int isLE = 0;
            for (int i = dataCursor; i < data.Length; i += 2)
            {
                // copy the next 16 bits into the w1 buffers
                //   * constrainedCopyBE ensures that if the system is Little Endian, the copy is performed in a Big Endian style (bytes reversed)
                arrayExtensions.constrainedCopyBE(data, i, wBuf0, 0, wBuf0.Length);
                //   * constrainedCopyLE ensures that if the system is Big Endian, the copy is performed in a Little Endian style (bytes reversed)
                arrayExtensions.constrainedCopyLE(data, i, wBuf1, 0, wBuf1.Length);

                //
                // w1 as represented by wbuf0
                //
                ushort w1_0 = BitConverter.ToUInt16(wBuf0, 0);
                //
                // The decodeNextOp function advises us whether we're looking at a single-word operation, a double-word operation, or an error condition
                //
                byte w1_0_proof = decodeNextOp(w1_0);
                if (w1_0_proof == 0x00)
                {
                    // if the proof is 0x00 then we met a valid 1-word UTF-16 value
                    // +1 for BigEndian!  WOOHOO!
                    isBE++;
                }
                else if (w1_0_proof == 0x01)
                {
                    // if the proof is 0x01 then we met a valid 2-word UTF-16 value
                    // increment the index counter so we can grab the next word value from the data array
                    i += 2;
                    // constrainedCopyBE gets the guaranteed BigEndian copy style for wBuf0
                    arrayExtensions.constrainedCopyBE(data, i, wBuf0, 0, wBuf0.Length);
                    ushort w2_0 = BitConverter.ToUInt16(wBuf0, 0);
                    // The decodeTwoWord function performs a UTF-16 decode operation against 2 UINT-16s provided and returns an integer value representing 
                    // the UTF-16 codepoint, or returns -1 in the event of a UTF-16 decode error.
                    if (decodeTwoWord(w1_0, w2_0) != -1)
                    {
                        // decodeTwoWord passed
                        // +1 for BigEndian!  YIPPEE!
                        isBE++;
                    }
                    else
                    {
                        // decodeTwoWord failed
                        // -1 for BigEndian!  Yikes!
                        isBE--;
                    }
                }
                else if (w1_0_proof == 0xff)
                {
                    // If the proof is 0xff then there was an error in the first word to begin with
                    // -1 for BigEndian!  Uh-oh!
                    isBE--;
                }
                
                //
                // w1 as represented by wbuf1
                //
                ushort w1_1 = BitConverter.ToUInt16(wBuf1, 0);
                //
                // The decodeNextOp function advises us whether we're looking at a single-word operation, a double-word operation, or an error condition
                //
                byte w1_1_proof = decodeNextOp(w1_1);
                if (w1_1_proof == 0x00)
                {
                    // if the proof is 0x00 then we met a valid 1-word UTF-16 value
                    // +1 for LittleEndian!  YAY!
                    isLE++;
                }
                else if (w1_1_proof == 0x01)
                {
                    // if the proof is 0x01 then we met a valid 2-word UTF-16 value
                    // don't increment the index counter again - we already did that earlier in this pass
                    // constrainedCopyLE gets the guaranteed LittleEndian copy style for wBuf1
                    arrayExtensions.constrainedCopyLE(data, i, wBuf1, 0, wBuf1.Length);
                    ushort w2_1 = BitConverter.ToUInt16(wBuf1, 0);
                    // The decodeTwoWord function performs a UTF-16 decode operation against 2 UINT-16s provided and returns an integer value representing 
                    // the UTF-16 codepoint, or returns -1 in the event of a UTF-16 decode error.
                    if (decodeTwoWord(w1_1, w2_1) != -1)
                    {
                        // decodeTwoWord passed
                        // +1 for LittleEndian!  SHIBBY!
                        isLE++;
                    }
                    else
                    {
                        // decodeTwoWord failed
                        // -1 for LittleEndian!  Awww!
                        isLE--;
                    }
                }
                else if (w1_0_proof == 0xff)
                {
                    // If the proof is 0xff then there was an error in the first word to begin with
                    // -1 for LittleEndian!  Dangit!
                    isLE--;
                }

                testCounter++;
                if (testCounter >= testMaximum) { break; }
            }

            wBuf0 = null;
            wBuf1 = null;

            //
            // The proof is in the pudding they say, so let's dig into the pudding!  Did somebody say pudding?!  Now I'm hungry for pudding but I don't have any pudding :(
            //   * We should be safe doing this as long as we ensure that both "proofs" arrays are the same size/length
            //
            if ((isBE < testMaximum) && (isLE < testMaximum))
            {
                // Something's gone wrong - neither BE nor LE had fully successful test runs.  This may not be a valid UTF-16 encoded file at all.
                throw new System.Text.DecoderFallbackException(@"Invalid UTF-16 data.  Unable to validate endianness across " + testMaximum.ToString() + @" decode runs.");
            }
            else
            {
                // If one or the other of the endianness counters is equal to testMaximum though, the question is simply a matter of which counter is bigger.
                //   * if isBE is greater than isLE, then return value of isBigEndian is TRUE!
                //     Otherwise, False
                r = (isBE > isLE);
            }

            GC.Collect();
            return r;
        }

        /// <summary>
        /// Decodes the specified data array from bytes into characters, using the UTF-16 specification.
        /// </summary>
        /// <param name="data">
        /// An array of bytes.  The data to decode according to UTF-16 specification.
        /// </param>
        /// <param name="endianness">
        /// An integer value indicating what UTF-16 endianness to use.  
        /// 
        /// If 0, this implementation attempts to automatically identify the UTF-16 Big or Little Endianness to use.  
        /// 
        /// If -1, this implementation will use Little Endian.  
        /// 
        /// If 1, this implementation will use Big Endian.
        /// </param>
        /// <returns>
        /// An array of char instances.
        /// </returns>
        public static char[] decodeData(ref byte[] data)
        {
            char[] r = new char[0];

            if (data != null)
            {
                //
                // For the IANA registered charsets UTF-16BE and UTF-16LE, a byte order mark should not be used because the names of these character sets already determine the byte order. If encountered 
                // anywhere in such a text stream, U+FEFF is to be interpreted as a "zero width no-break space".
                //
                // Clause D98 of conformance (section 3.10) of the Unicode standard states, "The UTF-16 encoding scheme may or may not begin with a BOM. However, when there is no BOM, and in the absence 
                // of a higher-level protocol, the byte order of the UTF-16 encoding scheme is big-endian." Whether or not a higher-level protocol is in force is open to interpretation. Files local to a 
                // computer for which the native byte ordering is little-endian, for example, might be argued to be encoded as UTF-16LE implicitly. Therefore, the presumption of big-endian is widely ignored. 
                // When those same files are accessible on the Internet, on the other hand, no such presumption can be made. Searching for 16-bit characters in the ASCII range or just the space character 
                // (U+0020) is a method of determining the UTF-16 byte order.
                //
                // The W3C/WHATWG encoding standard used in HTML5 specifies that content labelled either "utf-16" or "utf-16le" are to be interpreted as little-endian "to deal with deployed content".[12] 
                // However, if a byte-order mark is present, then that BOM is to be treated as "more authoritative than anything else".[13]
                //
                bool bigEndian = isBigEndian(ref data);

                //
                // Throughout the UTF16 decode operation, we'll be using a 16-bit data buffer.  May as well set that up right now.
                // 
                byte[] wBuf = new byte[2];
                for (int i = 0; i < data.Length; i += 2)
                {
                    // copy the next 16 bits into the w1 buffer
                    Array.ConstrainedCopy(data, i, wBuf, 0, wBuf.Length);
                    if ((bigEndian && System.BitConverter.IsLittleEndian) || (!bigEndian && !System.BitConverter.IsLittleEndian))
                    {
                        // If the UTF-16 filespec is BigEndian and the system is LittleEndian 
                        // orelse 
                        // if the UTF-16 filespec is LittleEndian and the system is BigEndian
                        // turn the bytes around
                        System.Array.Reverse(wBuf);
                    }
                    // convert the data to a ushort w1
                    ushort w1 = BitConverter.ToUInt16(wBuf, 0);
                    if ((w1 < 0x0000d800) || (w1 > 0x0000dfff))
                    {
                        //
                        // If W1 < 0xD800 or W1 > 0xDFFF, the character value U is the value of W1.  Terminate
                        //
                        arrayExtensions.push<char>(ref r, (char)w1);
                    }
                    else if ((w1 > 0x0000d7ff) && (w1 < 0x0000dc00))
                    {
                        //
                        // Determine if W1 is between 0xD800 and 0xDBFF.  This begins a 2-word (4-byte) sequence
                        //   * If not, the sequence is in error and no valid character can be obtained using W1. Terminate.
                        //
                        // Testing if the data contains another 2 bytes of data
                        if ((i + 2) < data.Length)
                        {
                            // increment the index counter
                            i += 2;
                            // copy the next 16 bits into the w1 buffer
                            Array.ConstrainedCopy(data, i, wBuf, 0, wBuf.Length);
                            if ((bigEndian && System.BitConverter.IsLittleEndian) || (!bigEndian && !System.BitConverter.IsLittleEndian))
                            {
                                // If the UTF-16 filespec is BigEndian and the system is LittleEndian 
                                // orelse 
                                // if the UTF-16 filespec is LittleEndian and the system is BigEndian
                                // turn the bytes around
                                System.Array.Reverse(wBuf);
                            }
                            // convert the data to a ushort
                            ushort w2 = BitConverter.ToUInt16(wBuf, 0);
                            if ((w2 > 0xdbff) && (w2 < 0xe000))
                            {
                                // If W2 is not between 0xDC00 and 0xDFFF, the sequence is in error. Terminate.
                                throw new System.Text.DecoderFallbackException(@"Invalid UTF-16 data (invalid data in second word of sequence).", data, i);
                            }

                            // need to blank all but the lower 10 bits of w1 and w2
                            //   * The stupid-looking casts of ushort or/xor back to ushort?  That's because DotNET very helpfully casts any or/xor operations to int for no apparent reason
                            // or 1111 1100 0000 to ensure all those bits are set, then xor the same to ensure all those bits are unset
                            // this blanks any metadata bits while leaving the character data bits intact
                            w1 = (ushort)((w1 | 0xfc00) ^ 0xfc00);
                            w2 = (ushort)((w2 | 0xfc00) ^ 0xfc00);
                            //
                            // Create a new int and or w1 into it, then bitshift it 10 places to the left to place its value into the 'most significant bits' position
                            // then bitshift 10 places left to make way for the low-order bits from w2
                            //   * It may make more sense to use a uint here, but that would result in unnecessary casting/performance dragging.  We're only using 20 bits here so the signing 
                            //     bit would never come into play anyway.
                            //
                            int newChar = (w1 << 10);
                            // Now or in w2
                            newChar = newChar | w2;
                            //
                            // Construct a 20-bit unsigned integer U', taking the 10 low-order bits of W1 as its 10 high-order bits and the 10 low-order bits of W2 as its 10 low-order bits.
                            // 
                            // Add 0x10000 to U' to obtain the character value U. Terminate.
                            //
                            newChar += 0x00010000;
                            arrayExtensions.push<char>(ref r, (char)newChar);
                        }
                        else
                        {
                            // If there is no W2 (that is, the sequence ends with W1), or if W2 is not between 0xDC00 and 0xDFFF, the sequence is in error. Terminate.
                            throw new System.Text.DecoderFallbackException(@"Invalid UTF-16 data (2-word [4-byte] codepoint detected, but no more data to read).", data, i);
                        }

                    }
                    else
                    {
                        // Determine if W1 is between 0xD800 and 0xDBFF. If not, the sequence is in error and no valid character can be obtained using W1. Terminate.
                        throw new System.Text.DecoderFallbackException(@"Invalid UTF-16 data (invalid data in first word of sequence).", data, i);
                    }
                }

                wBuf = null;
            }

            GC.Collect();
            return r;
        }

        /// <summary>
        /// Gets the number of bytes required to encode the specified char in UTF-16.
        /// </summary>
        /// <param name="fromChar">
        /// The char to encode.
        /// </param>
        /// <returns>
        /// A 32-bit integer value.  The number of bytes required to encode the specified char in UTF-16.  
        /// 
        /// Will return either 2 or 4 for UTF-16 valid characters.  Will also return 2 for invalid UTF-16 characters.
        /// </returns>
        public static int getEncodeSize(char fromChar)
        {
            int r = 2;

            uint c = (uint)fromChar;
            if (c < 0x00010000)
            {
                // If U < 0x10000, encode U as a 16-bit unsigned integer and terminate.
                r = 2;
            }
            else
            {
                r = 4;
            }

            return r;
        }

        /// <summary>
        /// Gets the total number of bytes required to encode the entire array of specified chars in UTF-16.
        /// </summary>
        /// <param name="fromChars">
        /// The array of chars to encode.
        /// </param>
        /// <returns>
        /// A 32-bit integer value.  The number of bytes required to encode the specified char array in UTF-16.  
        /// 
        /// Will return either 2 or 4 bytes per character.  Will return 2 even for invalid UTF-16 characters.
        /// </returns>
        public static int getEncodeSizeFull(ref char[] fromChars)
        {
            int r = 0;

            if (fromChars != null)
            {
                for (int i = 0; i < fromChars.Length; i++)
                {
                    r += getEncodeSize(fromChars[i]);
                }
            }

            return r;
        }

        /// <summary>
        /// Encodes the specified char array into byte data, according to the UTF-16 specification.
        /// </summary>
        /// <param name="fromChars">
        /// The char array to encode to data according to UTF-16.
        /// </param>
        /// <param name="useLittleEndian">
        /// A boolean value.  If True, data will be written in Little Endian format.
        /// </param>
        /// <param name="writeBOM">
        /// A boolean value.  If True, a Byte Order Marker will be written at the front of the data indicating that this is UTF-16 and what byte order it is stored in.  If False, no BOM will be written.
        /// </param>
        /// <returns>
        /// A byte array.  The UTF-16 encoded data.
        /// </returns>
        public static byte[] encodeData(ref char[] fromChars, bool useLittleEndian, bool writeBOM)
        {
            // The getEncodeSizeFull function will kindly determine the required size of the output buffer before we begin
            byte[] r = new byte[getEncodeSizeFull(ref fromChars)];
            //
            // outIndex will track our way through the return value array as we iterate through the input array
            //
            int outIndex = 0;

            if (writeBOM)
            {
                // It's best to do this manually rather than try to use the arrayExtensions.constrainedCopyBE and LE functions, since we know for a fact that the static BOM data is stored in BE format
                //   * Copying from BE format to BE format would bork it/reverse it (effectively making it LE)
                Array.ConstrainedCopy(BOM, 0, r, outIndex, BOM.Length);
                // Since we know for sure that the static BOM data array is in BE format, we must reverse it explicitly if the function was called with the useLittleEndian option
                if (useLittleEndian) { Array.Reverse(r, outIndex, BOM.Length); }
                // and increment the outIndex
                outIndex += 2;
            }

            //
            // Throughout the UTF16 decode operation, we'll be using a 16-bit data buffer.  May as well set that up right now.
            // 
            byte[] wBuf = new byte[2];
            for (int i = 0; i < fromChars.Length; i++)
            {
                uint c = (uint)fromChars[i];
                if (c < 0x00010000)
                {
                    // If U < 0x10000, encode U as a 16-bit unsigned integer and terminate.
                    wBuf = BitConverter.GetBytes((ushort)c);
                    if (!useLittleEndian)
                    {
                        // We must explicitly copy in Big Endian format if the user didn't specify useLittleEndian
                        arrayExtensions.constrainedCopyBE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    else
                    {
                        // Or else we must explicitly copy in Little Endian format if the user did specify useLittleEndian
                        arrayExtensions.constrainedCopyLE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    // and increment the outIndex regardless
                    outIndex += wBuf.Length;
                }
                else
                {
                    // Let U' = U - 0x10000. Because U is less than or equal to 0x10FFFF, U' must be less than or equal to 0xFFFFF. That is, U' can be represented in 20 bits.
                    c -= 0x00010000;
                    //
                    // Initialize two 16-bit unsigned integers, W1 and W2, to 0xD800 and 0xDC00, respectively. These integers each have 10 bits free to encode the character value, for a total of 20 bits.
                    // Assign the 10 high-order bits of the 20-bit U' to the 10 low-order bits of W1 and the 10 low-order bits of U' to the 10 low-order bits of W2. Terminate.
                    //
                    // Bitshift c 10 places right to drop the 'least significant bits' off and or what's left onto w1
                    ushort w1 = (ushort)(0xd800 | (ushort)(c >> 10));
                    wBuf = BitConverter.GetBytes(w1);
                    if (!useLittleEndian)
                    {
                        // We must explicitly copy in Big Endian format if the user didn't specify useLittleEndian
                        arrayExtensions.constrainedCopyBE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    else
                    {
                        // Or else we must explicitly copy in Little Endian format if the user did specify useLittleEndian
                        arrayExtensions.constrainedCopyLE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    // and increment the outIndex regardless
                    outIndex += wBuf.Length;
                    //
                    // As I discovered in developing utf8 that DotNET likes to really fuck you if you bitshift left and then come back to the right to run 1s of and bring back 0s (it doesn't bring back 0s), 
                    // we instead must or on 1111 1111 1111 1111 1111 1100 0000 0000 and then xor it back off to completely clear those bits before we can or the least significant char bits onto w2
                    //
                    ushort w2 = (ushort)(0xdc00 | (ushort)((c | 0xfffffc00) ^ 0xfffffc00));
                    wBuf = BitConverter.GetBytes(w2);
                    if (!useLittleEndian)
                    {
                        // We must explicitly copy in Big Endian format if the user didn't specify useLittleEndian
                        arrayExtensions.constrainedCopyBE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    else
                    {
                        // Or else we must explicitly copy in Little Endian format if the user did specify useLittleEndian
                        arrayExtensions.constrainedCopyLE(wBuf, 0, r, outIndex, wBuf.Length);
                    }
                    // and increment the outIndex regardless
                    outIndex += wBuf.Length;
                }
            }

            return r;
        }
    }
}
