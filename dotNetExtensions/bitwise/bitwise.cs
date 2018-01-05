using System;


namespace dotNetExtensions
{
    /// <summary>
    /// Contains extension and helper methods that really should have been built into BitConverter, but aren't.
    /// </summary>
    public static partial class bitwise
    {
        /// <summary>
        /// Sets the specified bit into the specified unsigned 8-bit value.
        /// </summary>
        /// <param name="inValue">
        /// An unsigned 8-bit value to set a bit into.
        /// </param>
        /// <param name="bitIndex">
        /// The bit index to set.
        /// </param>
        /// <param name="bitValue">
        /// The bit value to set.
        /// </param>
        /// <returns>
        /// The new byte value, or what inValue becomes when the specified bit index is changed to the specified bitValue.
        /// </returns>
        public static byte setBit(byte inValue, int bitIndex, bool bitValue)
        {
            byte r = inValue;

            if (bitIndex > 7)
            {
                throw new IndexOutOfRangeException(@"setBit was given an invalid bitIndex " + bitIndex.ToString() + @" (bytes only have 8 bits, 0-base indexed, maximum index value is 7)");
            }
            else
            {
                // This leftward bitshift produces a value where the only bit set into the byte value is the one at
                // the specified bitIndex
                byte b = (byte)(1 << bitIndex);
                // No matter what, we're going to OR the new bit value into the return value - this is a must-do in all events
                r |= b;
                // if bitValue is false, then the new bit value HAD to be toggled on in order to absolutely ensure that it was turned off
                // otherwise there was no way to know whether the next operation turned it on or off
                if (!bitValue)
                {
                    r ^= b;
                }
            }
            
            return r;
        }

        /// <summary>
        /// Toggles the specified bit into the specified unsigned 8-bit value.
        /// </summary>
        /// <param name="inValue">
        /// An unsigned 8-bit value to toggle a bit into.
        /// </param>
        /// <param name="bitIndex">
        /// The bit index to toggle.
        /// </param>
        /// <returns>
        /// The new byte value, or what inValue becomes when the specified bit index is toggled from whatever it is to whatever 
        /// its opposite would be.
        /// </returns>
        public static byte toggleBit(byte inValue, int bitIndex)
        {
            byte r = 0;

            if (bitIndex > 7)
            {
                throw new IndexOutOfRangeException(@"toggleBit was given an invalid bitIndex " + bitIndex.ToString() + @" (bytes only have 8 bits, 0-base indexed, maximum index value is 7)");
            }
            else
            {
                // This leftward bitshift produces a value where the only bit set into the byte value is the one at
                // the specified bitIndex
                byte b = (byte)(1 << bitIndex);
                // Since this is a simple toggle operation, we do nothing but XOR, which turns the specified bit off if it's on or turns it on if it's off
                r ^= b;
            }
            
            return r;
        }

        /// <summary>
        /// Gets a value indicating the current state of the specified bit in the specified unsigned 8-bit value.
        /// </summary>
        /// <param name="inValue">
        /// An unsigned 8-bit value to test a bit into.
        /// </param>
        /// <param name="bitIndex">
        /// The bit index to test.
        /// </param>
        /// <returns>
        /// True if the specified bit index is 1.  False if the specified bit index is 0.
        /// </returns>
        public static bool getBit(byte inValue, int bitIndex)
        {
            // in this case r is not the actual return value, but it is critical to obtaining/determining the return value
            byte r = 0;

            if (bitIndex > 7)
            {
                throw new IndexOutOfRangeException(@"getBit was given an invalid bitIndex " + bitIndex.ToString() + @" (bytes only have 8 bits, 0-base indexed, maximum index value is 7)");
            }
            else
            {
                byte b = (byte)(1 << bitIndex);
                // In the read operation, all we need to know is the bit index and the original value
                r = b;
            }
            //
            // To paraphrase Monte Python:  "NOBODY expects the Punctilious Exposition!"
            //
            // Morons think AND is a "logical" operation that produces only "true" and "false" values.  The truth is that "logical" operations in computing are "mathematical" operations that 
            // produce numeric values.  This is just as true whether you're using AND against a boolean variable or a numeric variable, whether your language is strongly typed or not, because 
            // of how computers work and because you're running your code on a computer whether your compiler or interpreted runtime treats your strongly typed boolean like a numeric value for 
            // your benefit or not.  Many languages will help morons keep being stupid by hiding the need to test for eq, gtn, etc (ie:  allowing any nonzero to be considered "true"), and clowns 
            // who only know those languages (but know them pretty well) will rabidly assert the opposite of reality.  You'll find these clowns on StackOverflow, MSDN and every other forum you try 
            // to learn from, you'll find them on IRC and any other live chat system you try to learn from, many of these fools have their own websites, and you'll even find this stupidity in 
            // books you pay good money for at Barnes & Noble or Amazon.com.  
            //
            // That's plain disinformation and it does enormous harm to anyone who has a legitimate interest in really learning the craft of computer programming.  This particular piece of 
            // disinformation is as harmful to fledgeling programmers as if they were telling newbie carpenters to drive screws with a hammer.  The widely disseminated disinformation that "logical" 
            // operators in computing are somehow Boolean Algebraic rather than pure Binary Arithmetic will slow you down if you let it, and stems from being completely and utterly uninformed as to 
            // what a computer is and how it works in the first place.
            //
            // And and Or and Xor are logical, sure, but logical means mathematical in electrical so it means mathematical in electronics so it means mathematical in computing.  Computing has less than 
            // nothing to do with Boolean Algebra, in any way, shape, or form.  The only way computer programming even vaguely relates to Boolean Algebra is if you're actually working on a project to 
            // automate Boolean Algebraic equations, and then Boolean Algebra is only relevant to the functions that perform those calculations.  It still has less than nothing to do with the code you 
            // write, only the business logic the code performs.  This holds true no matter how much you find the term "boolean" tossed about.  "Boolean" is a keyword that represents an abstract concept, 
            // it isn't proof positive of your favorite conspiracy theory about the JFK Assassination or any other ridiculous thing that has nothing to do with electronics and therefore nothing to do with 
            // computer programming.
            //
            // Computer Programming represents electronic circuit diagramming and electrical states on specific pins in hardware components/chips.  If you want a better understanding of programming, go to 
            // Home Depot and grab yourself a copy of "Ugly's Electrical Reference."  What "boolean" values in computing represent, in terms of electrical states, is whether there's an electrical charge 
            // on a specific pin or pins on a microchip or not.  
            //
            // Consider an 8-bit value to represent a microchip with 4 pins down each side, numbered 0 to 7.  Consider a 16-bit value to represent a microchip with 8 pins down each side, numbered 
            // 0 to 15.   So on.  There really isn't any such thing as a "boolean" true or false in any computer system, in this context, there's only "0" or "3" or "4" or "7".   3 if pin 0 and pin 1 are 
            // 'hot' (in an 8-bit microchip:  0000 0011).  4 if pin 2 is hot (0000 0100).  7 if 0, 1, and 2 are hot at the same time (0000 0111).  A "boolean false" then only exists when the whole 
            // microchip is 'cold' or 0 (0000 0000) and in all other cases such as 255 (unsigned, or -1 if signed) (1111 1111) or 32 (0010 0000) or 8 (0000 1000) or 90 (0101 1010) this "boolean" is "true."  
            // The electrical state on each individual pin (a single bit) may pretty accurately be considered boolean, 1 or 0, true or false, but only in the abstract sense.
            // 
            // Once upon a time there was no such thing as a programmer who didn't know this because there was only Control Code, which is a collection of numerics which is the only "real" programming 
            // language there is.  Every other 
            // programming language is a lie programmers tell each other, and some of those lies are really, really cool!  The modern Control Codes are such as Intel's x86 and IA32 or AMD's 
            // amd64 Instruction Sets, which are CPU programming  languages.  You pass a numeric command followed by numeric arguments such as memory addresses.  These are also known as Assembly 
            // languages because the "compilers" for them are called Assemblers.  So programmers got tired of poring through moldy old tomes (that get bigger every year) of Control Codes and 
            // wrote compilers for "languages" such as C, which expanded to C++.  With compiled languages you can write in a more "human sensible" format with these "logical constructs" and 
            // then run it through software that boils it down to those Control Codes for you, so you can write computer programs without actually knowing jack about how computers work - 
            // you just need to know your language syntax and how your particular compiler works.  What happens is that the "compiler" takes multiple files of "language" code and compiles them 
            // all into one big file, then "assembles" that into Control Code, optionally applying an executable file formatting style as outlined below.
            //
            // But compiled languages are a bit of a bear when it comes to "porting" code from one platform to another.  Linux and Windows, for example, have radically different ways of doing 
            // things.  Windows uses the Portable Executable (PE) format and Linux uses the Executable and Linkable Format (ELF), as well as some others.  So if you want to compile a program 
            // for Windows and Linux, you have to write the program and then compile it --twice--, once for each platform.  On top of that, Linux uses interrupts and specific callouts for 
            // specific tasks while Windows uses API calls (even if you're writing a simple console product that only uses interrupts on each platform, Linux only uses 1 interrupt with a plethora 
            // of parameters while DOS/Windows use a plethora of interrupts with a plethora of parameters each).  On top of that, Linux has a whole cadre of GUIs available each of which has its 
            // own speshul little short-bus type of API (designed by college students and wannabe programmers mostly), so if you really want to make a product that supports Linux (and several of 
            // its GUI flavors) and Windows you basically have to write 6 or 8 different products in one.  It is true that a language like C or C++ actually has a bunch of "standard" or "builtin" 
            // features and functions that you can fairly expect to be implemented no matter what OS the file is run on, but regardless you would still have to provide a PE for Windows or an ELF 
            // for Linux and if you wanted to do anything really cool (like graphics, legitimate low-level networking, legitimate low-level hard disk I/O, etc), you absolutely and positively must 
            // callout to the operating system to do it.
            //
            // People who don't understand either Windows or Linux very well will argue that Linux lets you execute damn near anything, but they're listing nonsense like bash and perl scripts
            // for Linux and ignoring that you can do the same thing with batch or .vbs (or even perl if you have the runtime) in Windows.  Which brings me to "interpreted" languages.
            //
            // Interpreted languages are high level, and high level programming is mainly where this awful collection of misconceptions that people really aggressively fling around the internet 
            // comes from.  Morons who only work in those high-level interpreted languages don't know jack about how computers work because they've never needed to learn.  They honestly don't even 
            // have a clue how their own runtimes work, which is a far cry from even the compiled programmer who at least had to learn how their compiler operates.  The interpreted "programmer" 
            // knows their language syntax and that's absolutely it.  End of list.  
            //
            // Don't get me wrong about interpreted languages such as Python and Perl and VBScript:  They're great.  I'd go as far as to say "awesome to behold."  Rather than being "compiled", 
            // interpreted scripts are written in their "language" form and stored that way, and then you can install a "runtime" which is the only component that actually is compiled and that 
            // runtime executes your script for you.  This is how the Python and Perl programs you write can run on Linux and Windows systems when Linux and Windows have such radically different 
            // ways of doing things.  These interpreted languages are great for programmers who need to whip something out in a hurry and present it to a hundred users across half a dozen OS 
            // platforms that share a common runtime, but when you're dealing with a "programmer" who really only knows an interpreted language or two (but has a monastic knowledge of the 
            // syntax[es]) then there's a high likelihood that you're dealing with a truly rabid halfwit who's going to make fully disinformational statements to you as if they're God's own Gospel,
            // and back them up with wild claims about his decades of experience in some field or industry that would really only be intimidating/impressive to a 4th grader (but are clearly 
            // intended to be intimidating/impressive to their target).  It's just godawful confusing to somebody who's actually trying to learn.
            //
            // These interpreted languages tend to do a lot of things for the programmer that are helpful.  They're shortcuts.  They're intended to speed development, going from blank slate to 
            // distribution in a matter of minutes or hours instead of days or weeks.  They are not strongly typed so they don't throw errors unless they're specifically programmed to do so, 
            // typically only if you throw completely wackadoo data at them.  They'll take any nonzero and/or nonnegative value and consider it a BOOLEAN TRUE (even if you originally used that 
            // data as ASCII or Unicode string data - the interpreted runtime doesn't know or care because it isn't strongly typed), or they'll take any zero/negative and consider it BOOLEAN FALSE, 
            // or sometimes any nonzero will be TRUE while only zero is FALSE, depending on runtime or specific script options in play.  A lot of compilers will do this too, depending on options set 
            // at the compiler's commandline or options set into the codefile.
            //
            // Most compiled-language programmers will actually know what they're doing when they set these options or use these conventions.  This only becomes a problem when interpreted-language 
            // "programmers" pick up their knowledge of writing code from Google and Github (ie:  other clowns just like them who never once in their life RTFM), and learn to just shallowly think "true" 
            // and "false" are things a computer understands without the faintest clue that it's really the runtime interpreting their stupidity into numbers for the computer's benefit.
            //
            // DotNET is in fact one of these interpreted languages, in spite of its rather misleading "Build" menu options, but because it was originally intended as a learning tool and it actually IS 
            // strongly typed, it actually WILL throw errors when you try to run an if() against a numeric instead of a boolean.
            //
            // If you ever venture as low level as Assembly programming yourself, you'll find that there's actually no such thing, in real computer programming, as an if-else statement or a boolean or 
            // even an object or a type.  There's only data and data width and it's what you do with it that makes it into an object or a type.  There's only cmp cpuReg0,cpuReg1 or sub cpuReg0,cpuReg1 
            // followed by a jg, jl, jlz, jgz, jeq, or such instruction which means jump if greater, jump if less, jump if less than zero, jump if greater than zero, and so forth, followed by a memory 
            // address.
            //
            // So when you're programming in literally any or every language, compiled or interpreted, you should always bear in mind what you're really doing there.  You're not writing an if-else.  The 
            // if-else is a lie because the programming language you're working in is a lie.  You're writing an if-else that will be interpreted to a number of mathematical procedures finally resulting 
            // in a cmp (comparison) followed by a series of conditional jumps such as jez or jgz or jlz to a memory address.  That's what your Perl or Python or DotNET runtime or your C/++ compiler will 
            // boil any code you write down to.  Anyone who tells you different is either an idiot or a liar, or maybe both, and if they tack on that you should believe them because they have umpteen 
            // years/decades experience in some high-value IT field or government job then they're definitely a liar and absolutely a rank attention whore on top of it.
            // 
            // ********************************************************
            //
            // So if you've fallen victim to one of the aforementioned halfwitted clowns in the past, you may be confused by the next line of uncommented code.  The parenthesed & (AND) operation is actually 
            // returning a numeric value, not a boolean value.  It's the GTN operation that performs the conversion to bool.
            //
            // This is not a function of DotNET, it's how computers work all the way down to Control Code.  DotNET simply doesn't hide the fact from programmers the way more simplified platforms do.
            //
            // If the same bit (or collection of bits) is/are set in r and inValue, then the parenthesed value will be that value.  
            //
            // Let's say that r is 2 (0000 0010) and inValue is 3 (0000 0011).  The parenthesed value returns 2 (0000 0010).
            //
            // Or if r is still 2 and inValue is 6 (0000 0110).  The parenthesed value still returns 2 (0000 0010) because that's still the common bit collection in the 2 values.
            //
            // But if r is 2 and inValue is 5 (0000 0101), then the parenthesed value returns 0 (0000 0000) because there is no common bit between the 2 values inside.
            //
            // Or if r is 4 (0000 0100) and inValue is 140 (1000 1100), then the parenthesed value returns 4 (0000 0100) because that's the common bit in the values.
            //
            // See?  EZ-PZ.
            //
            return ((r & inValue) > 0);
        }

        public static sbyte setBit(sbyte inValue, int bitIndex)
        {
            sbyte r = 0;
            
            return r;
        }

        public static sbyte toggleBit(sbyte inValue, int bitIndex)
        {
            sbyte r = 0;

            return r;
        }

        public static bool getBit(sbyte inValue, int bitIndex)
        {
            bool r = false;

            return r;
        }
    }
}
