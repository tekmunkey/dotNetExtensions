using System;

namespace dotNetExtensions
{
    public static partial class bitwise
    {
        /// <summary>
        /// Converts the specified unsigned 16-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.  
        /// </summary>
        /// <param name="value">
        /// An unsigned 16-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 16-bit unsigned value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(UInt16 value, bool bigEndian)
        {
            //
            // Since I'm posting things to github under the auspices of programming tutorials, I thought it would be important to depart from letting the System.BitConverter do things for us 
            // (and since BitConverter doesn't really do a great job of anything anyway) and demonstrate to folks how things really work in computing, so that these kinds of tasks can be 
            // performed in literally any programming language, in a truly platform agnostic manner, using numeric data types and logical operators.
            //
            byte[] r = new byte[2];
            //
            // bi is the 'byte index' or the index into our return value.  Just a data cursor.
            //   * For Big Endian operations, we default bi to 1
            //   * For Little Endian operations, we would default bi to 0
            //
            int bi = 1;
            //
            // Negative qualities always process faster than positive assertions in if statements
            //   * When you process a positive assertion such as == the system must validate each and every bit in one or both operands to ensure a full match
            //   * This isn't particularly true with a strongly typed language such as C# (or anything with a strong boolean type) since DotNET would use 1111 1111 1111 1111 for FALSE or 
            //     0000 0000 0000 0000 for TRUE, and therefore would really only test 1 bit of the boolean quantity for any TRUE or FALSE statement
            //   * Nonetheless, using negative assertions in if statements is a best practice because when comparing multiple operands it is always best performance
            //     If that weren't the case, bi could default to 0 and then this test would be if (bigEndian) { bi = 0; } instead
            //
            if (!bigEndian) { bi = 0; }
            //
            // ba is the 'byte adjust' or an adjustment that will have to be performed if/when we transform a bit index into the *platform's integer* to meet a bit index into the
            // *return value's* integer
            //   * The platform's integer is the input value, a UInt16 (16 bits) while the return value's integer is the output value, one of the members of an array of 8-bit integers
            //     :: More on this later
            //
            int ba = 0;
            //
            // The integer value we're working from contains 16 bits so that's all we're worried about.
            //   :: 16 bits, 16 iterations.
            //
            for (int i = 0; i < 16; i++)
            {
                //
                // The next if statement is a MODULUS test.  Modulus returns the remainder of dividing the first operand by the second.
                // So if the current iterative cursor divided by 8 (the size of a byte) is equal to 0...
                //   * This if statement triggers on each byte boundary, allowing us to perform tasks each time we flop over to the next byte 
                //     in the return value, without having to explicitly define an if/else clause for each individual boundary, such as with 
                //     16-bit integers (where there would be 1 test for the 8-bit boundary), 32-bit integers (3 tests, 1 for 8-bit, 1 for 16-bit, 
                //     another at 24-bit), and 64-bit integers (7 tests).
                //
                if ((i > 0) && (i % 8) == 0)
                {
                    //
                    // Remember that bi tells us which byte into r[] we'll be setting our 'read' bit value into
                    //   * At each byte boundary (8 bits) 
                    //     :: In Big Endian operations, decrement bi by 1
                    //     :: In Little Endian operations, increment bi by 1
                    //
                    if (!bigEndian)
                    {
                        // if not bigEndian, this is a Little Endian operation
                        bi = bi + 1;
                    }
                    else
                    {
                        // otherwise this is a Big Endian operation
                        bi = bi - 1;
                    }
                    //
                    // Remember that ba adjusts the bit index into the *platform's integer* to match an index into the *return value's* integer
                    //   * at each byte boundary increment bv by the size of a byte, exactly 8
                    //     :: This is done regardless of output Endianness
                    //
                    ba = ba + 8;
                }

                //
                // The next uncommented line is a bitshift.
                //   * The left side (1) simply says "bit on" or literally "1" or 0000 0000 0000 0001
                //   * The right side (i) says "shift leftward i times"
                // So we're shifting the bit value "0000 0001" leftward "i" times.
                //   * If i is 4, we're shifting 0000 0001 leftward (4) times which results in 0000 0000 0001 0000
                //   * If i is 7, we're shifting 1 leftward (7) times which results in 0000 0000 1000 0000
                //   * If i is 12, we're shifting 1 leftward (12) times 0001 0000 0000 0000
                //   * If i is 15, we're shifting 1 leftward (15) times 1000 0000 0000 0000
                // This is how we test whether or not the current bit (0 through 15) is set into the UInt16 'value' that was passed in.
                //
                // This must be explicitly cast to (UInt16) for several reasons in DotNET.  The value 'i' is int, yes, but also any number (such as '1') 
                // that you blithely compile with Visual Studio's compiler would automagically be treated as an int, and also literally any bitshift, even 
                // a bitshift that was performed using two explicitly declared UInt16s as the operands, would return an integer value that had to be 
                // cast to the desired type in order to be considered a UInt16 by DotNET.
                //
                //   ** Extra special handling required for Int64, because DotNET insists on typecasting everything as an Int32 (signed 32-bit integer) regardless of all other 
                //      factors around it.  If the '1' in the next line of code isn't explicitly cast as the type we desperately want and need it to be, the function will break 
                //      and the output data will come out all wrong.  This typecast has been updated across *all* the functions even though the 64-bit functions (U/Int64) are 
                //      the only ones really affected.
                //
                UInt16 bitVal = (UInt16)((UInt16)1 << i);
                //
                // If the next line of code confuses you, it may be because you've been grossly disinformed by morons who want you to believe that "logical operators" such as 
                // AND, OR, and XOR in programming perform the same function as in "Boolean Algebra."  They do not!  Logical Operators in computer programming are mathematical, 
                // they are arithmetic, and return numeric values - not logical values.  The 'bool' type itself is in fact numeric, under the hood.  Logic is a human thing, an 
                // abstract, and computer programming is neither logical nor abstract depsite the willy-nilly tossing about of both terms.  Programming, at its core, is 100% 
                // numeric.  There's really only 1 programming language and that's Assembly.  All other languages are lies.  They're really cool lies, really helpful and productive 
                // lies, and they're lies that are actually told in the Assembly language itself, but they aren't true.
                // 
                // The abstraction layer, the thing that provides a sense of logic to your feeble, spongy human brain, is in your compiler/assembler/runtime or whatever.  That's a 
                // piece of software that somebody programmed for you to make programming easier.  That converts your weaksauce thoughts (C# or Perl or Python or C/++ or Javascript) 
                // into Assembly, the only real programming language there is, and that language is nothing but numbers.  In fact even that language is a lie, which through another 
                // abstraction layer represents electrical states in hardware.  If you want the truth, go stick your tongue to the terminals on 9v battery.  That's the only truth!
                //
                //   * In this case, the AND returns a numeric value which will either be 0 or not 0.  If it's not 0 then the bit at this index (0-15) was set.
                //
                bool isBitSet = ((value & bitVal) != 0);
                if (isBitSet)
                {
                    //
                    // If the bitVal we're seeking is in fact set into the value that was passed in, we must then convert it into a bitval appropriate to the byte addresses we're 
                    // working with
                    //   * Some programmers will tell you not to re-use memory addresses or labels or variable names in this way, but that is sheer stupidity.  Any Assembly 
                    //     programmer or anybody with half a clue how computers work will re-use the same memory address as often as possible to keep system resource bloat to an 
                    //     absolute minimum.  If there actually is a runtime out there where it's a good idea NOT to re-use memory addresses this way, you should never, EVER use 
                    //     it because it's garbage engineered by complete idiots.
                    //   * As before, DotNET will automatically cast the output of any OR operation (even when both operands are already declared/typecast as bytes) as Int32, 
                    //     so after the OR operation we have to cast the output back to (byte) before we can store it back into the return value index it originated from.
                    //   * When performing this new bitshift, a transformation must occur:
                    //     :: Because we're x-Bits deep into a 16-bit integer and y-Bytes deep into an array of 8-bit values, the bit index into the *platform's integer* must be 
                    //     :: adjusted to meet the bit index into the *return value's* integer
                    //
                    bitVal = (byte)(1 << (i - ba));
                    //
                    // Now we can set it into the appropriate byte in the output value by simply ORing the bitVal (which will be all 0s except for the particular bit we're currently working with) 
                    // into the relevant byte in the return value.
                    //
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            //
            // And that's it!  17 lines of code (brackets not included) to build either a Big or Little Endian byte array out of a UInt16, regardless of 
            // platform endianness.  EZ-PZ!
            //
            return r;
        }

        /// <summary>
        /// Converts the specified signed 16-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.
        /// </summary>
        /// <param name="value">
        /// A signed 16-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 16-bit signed value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(Int16 value, bool bigEndian)
        {
            byte[] r = new byte[2];

            int bi = 1;
            if (!bigEndian) { bi = 0; }

            int ba = 0;
            for (int i = 0; i < 16; i++)
            {
                if ((i > 0) && (i % 8) == 0)
                {
                    if (!bigEndian)
                    {
                        bi = bi + 1;
                    }
                    else
                    {
                        bi = bi - 1;
                    }
                    ba = ba + 8;
                }

                //
                // There is no meaningful changeover when transforming from signed to unsigned conversion to bytes.  The only difference, in data, between signed and unsigned 
                // integer values is that the most significant bit is used to indicate positive or negative (0 if positive, 1 if negative), which is why the minimum/maximum 
                // values of any signable integer (byte, short, int, long, etc) are always about half of the maximum value of the same bit container if it were unsigned, but 
                // the same container can still hold the same number of values.
                //
                // ie:  An unsigned byte can hold 256 unique values (minimum 0, maximum 255), and so can an unsigned byte (minimum -128, maximum 127).
                //      Unsigned Minimum:  0000 0000 (0)
                //      Unsigned Maximum:  1111 1111 (255)
                //        * The above is also -1 if the byte were signed
                //      Signed Minimum:    1000 0000 (-128)
                //        * The above is also 128 if the byte were unsigned
                //      Signed Maximum:    0111 1111 (127)
                //        * The above is 127 whether the byte is signed or not
                // 
                UInt16 bitVal = (UInt16)((UInt16)1 << i);
                //
                // For the purposes of our bitwise AND test, it is best to ensure an explicit typecast back to the signed type of the original input value
                //
                bool isBitSet = ((value & (Int16)bitVal) != 0);
                if (isBitSet)
                {
                    bitVal = (byte)(1 << (i - ba));
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            return r;
        }

        /// <summary>
        /// Converts the specified unsigned 32-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.
        /// </summary>
        /// <param name="value">
        /// An unsigned 32-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 32-bit unsigned value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(UInt32 value, bool bigEndian)
        {
            byte[] r = new byte[4];

            int bi = 3;
            if (!bigEndian) { bi = 0; }

            int ba = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((i > 0) && (i % 8) == 0)
                {
                    if (!bigEndian)
                    {
                        bi = bi + 1;
                    }
                    else
                    {
                        bi = bi - 1;
                    }
                    ba = ba + 8;
                }

                UInt32 bitVal = (UInt32)((UInt32)1 << i);

                bool isBitSet = ((value & bitVal) != 0);
                if (isBitSet)
                {
                    bitVal = (byte)(1 << (i - ba));
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            return r;
        }

        /// <summary>
        /// Converts the specified signed 32-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.
        /// </summary>
        /// <param name="value">
        /// A signed 32-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 32-bit signed value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(Int32 value, bool bigEndian)
        {
            byte[] r = new byte[4];

            int bi = 3;
            if (!bigEndian) { bi = 0; }

            int ba = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((i > 0) && (i % 8) == 0)
                {
                    if (!bigEndian)
                    {
                        bi = bi + 1;
                    }
                    else
                    {
                        bi = bi - 1;
                    }
                    ba = ba + 8;
                }

                UInt32 bitVal = (UInt32)((UInt32)1 << i);
                bool isBitSet = ((value & (Int32)bitVal) != 0);
                if (isBitSet)
                {
                    bitVal = (byte)(1 << (i - ba));
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            return r;
        }

        /// <summary>
        /// Converts the specified unsigned 64-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.
        /// </summary>
        /// <param name="value">
        /// An unsigned 64-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 64-bit unsigned value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(UInt64 value, bool bigEndian)
        {
            byte[] r = new byte[8];

            int bi = 7;
            if (!bigEndian) { bi = 0; }

            int ba = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((i > 0) && (i % 8) == 0)
                {
                    if (!bigEndian)
                    {
                        bi = bi + 1;
                    }
                    else
                    {
                        bi = bi - 1;
                    }
                    ba = ba + 8;
                }

                UInt64 bitVal = (UInt64)((UInt64)1 << i);

                bool isBitSet = ((value & bitVal) != 0);
                if (isBitSet)
                {
                    bitVal = (byte)(1 << (i - ba));
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            return r;
        }

        /// <summary>
        /// Converts the specified signed 64-bit integer value to a byte array in the specified Endian format, regardless of the current system architecture.
        /// </summary>
        /// <param name="value">
        /// A signed 64-bit value.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  
        /// 
        /// If True, the return value will be given in Big Endian format.  
        /// 
        /// If False, the return value will be given in Little Endian format.
        /// </param>
        /// <returns>
        /// A byte array representing the specified 64-bit signed value in the specified Endian format.
        /// </returns>
        public static byte[] getBytes(Int64 value, bool bigEndian)
        {
            byte[] r = new byte[8];

            int bi = 7;
            if (!bigEndian) { bi = 0; }

            int ba = 0;
            for (int i = 0; i < 64; i++)
            {
                if ((i > 0) && (i % 8) == 0)
                {
                    if (!bigEndian)
                    {
                        bi = bi + 1;
                    }
                    else
                    {
                        bi = bi - 1;
                    }
                    ba = ba + 8;
                }

                //
                // There is no meaningful changeover when transforming from signed to unsigned conversion to bytes.  The only difference, in data, between signed and unsigned 
                // integer values is that the most significant bit is used to indicate positive or negative (0 if positive, 1 if negative), which is why the minimum/maximum 
                // values of any signable integer (byte, short, int, long, etc) are always about half of the maximum value of the same bit container if it were unsigned, but 
                // the same container can still hold the same number of values.
                //
                // ie:  An unsigned byte can hold 256 unique values (minimum 0, maximum 255), and so can an unsigned byte (minimum -128, maximum 127).
                //      Unsigned Minimum:  0000 0000 (0)
                //      Unsigned Maximum:  1111 1111 (255)
                //        * The above is also -1 if the byte were signed
                //      Signed Minimum:    1000 0000 (-128)
                //        * The above is also 128 if the byte were unsigned
                //      Signed Maximum:    0111 1111 (127)
                //        * The above is 127 whether the byte is signed or not
                // 
                UInt64 bitVal = (UInt64)((UInt64)1 << i);
                bool isBitSet = ((value & (Int64)bitVal) != 0);
                if (isBitSet)
                {
                    bitVal = (byte)(1 << (i - ba));
                    r[bi] = (byte)(r[bi] | bitVal);
                }
            }

            return r;
        }
    }
}
