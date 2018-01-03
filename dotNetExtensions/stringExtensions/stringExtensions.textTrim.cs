using System;

namespace dotNetExtensions
{
    public static partial class stringExtensions
    {
        /// <summary>
        /// Trims any/all occurrances of the specified trimChars from the left side of the specified inputString, and returns the 
        /// resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimChars">
        /// A char array.  The characters to trim from inputString.
        /// </param>
        /// <returns>
        /// A string resulting from trimming trimChars from the left side of inputString.
        /// </returns>
        public static string trimLeft(string inputString, char[] trimChars)
        {
            string r = string.Empty;

            // don't do anything if the inputString isn't really a thing
            if (!string.IsNullOrEmpty(inputString))
            {
                char[] c = copyStringToCharArray(inputString);

                if ((c.Length > 0) && !object.ReferenceEquals(trimChars, null) && (trimChars.Length > 0))
                {
                    //
                    // stopTrim will toggle to false if we meet any match in trimChars
                    //   * This boolean value must be declared outside the do/while loop in order for the loop to use it
                    //
                    bool stopTrim = true;
                    do
                    {
                        // reset stopTrim to true at the beginning of each loop pass
                        stopTrim = true;
                        for (int i = 0; i < trimChars.Length; i++)
                        {
                            // Test the leftmost string character for equality with each member of trimChars
                            //   * Make sure this is a negative equality - stopTrim must be FALSE if a match is found
                            stopTrim = !c[0].Equals(trimChars[i]);
                            if (!stopTrim)
                            {
                                // if stopTrim is now false, perform left-side trimming into the char array
                                arrayExtensions.removeAt<char>(ref c, 0);
                                // and break from the for loop
                                break;
                            }
                        }
                        //
                        // if at any point the leftmost character fails to match any of the trimChars, stopTrim will be TRUE and 
                        // therefore the loop will end
                        // OR
                        // if at any point the entire string is trimmed to 0 length, the loop will end
                        //
                    } while (!stopTrim && (c.Length > 0));
                }

                // finally cut the char array back into the string return value
                r = new string(c);
            }

            return r;
        }

        /// <summary>
        /// Trims any/all occurrances of the specified trimString from the left side of the specified inputString, and returns the 
        /// resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimString">
        /// The string to trim from inputString.
        /// </param>
        /// <returns>
        /// The string resulting from trimming trimString from the left side of inputString.
        /// </returns>
        public static string trimLeft(string inputString, string trimString)
        {
            string r = copyStringToString(inputString);

            if (!string.IsNullOrEmpty(inputString) && !string.IsNullOrEmpty(trimString))
            {
                //
                // I have seen it happen where, occasionally, DotNET will decide to treat reference types as if they were passed by the ref 
                // keyword whether they were or not.  To avoid this, take a copy of the inputString rather than modifying it inplace during 
                // function processing.
                //
                r = copyStringToString(inputString);

                // Ensuring that the trimString would actually fit into the string it would be trimmed from
                if (trimString.Length <= r.Length)
                {
                    // Testing if there is any initial match from the right side of the test value with the trimString
                    //
                    // The startIndex value tells us where to start our string evaluation test
                    //   * This is really more relevant to trimRight than trimLeft - in trimRight it requires a bit more of a complex math 
                    //     statement that's better placed into a seprate int declaration than horked into an if() statement, but it was 
                    //     included in trimLeft code also for uniformity
                    int startIndex = 0;
                    if (r.Substring(startIndex, trimString.Length).Equals(trimString))
                    {
                        // The match exists, so begin trimming - we'll continue trimming trimString from inputString until/unless there is no 
                        // further matchup
                        do
                        {
                            //
                            // For trimLeft, the substring grabber below begins at (index = trimString.length) and then the length of the grab 
                            // must be the length of the remaining inputString minus the start index
                            //
                            // For trimRight, the substring grabber begins at (index = 0) and then the length of the grab must be the length of 
                            // the inputString up to the start index
                            //
                            r = r.Substring(trimString.Length, r.Length - trimString.Length);
                            //
                            // If at any point the output value is trimmed to 0 length, the loop will end 
                            // OR
                            // If at any point the remaining output length is no longer enough to fit the trimString into, the loop will end
                            // OR
                            // If at any point the next substring no longer matches the trimstring, the loop will end
                            //
                        } while ((r.Length > 0) && (trimString.Length <= r.Length) && r.Substring(startIndex, trimString.Length).Equals(trimString));
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Trims any/all occurrances of the specified trimChars from the right side of the specified inputString, and returns the 
        /// resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimChars">
        /// A char array.  The characters to trim from inputString.
        /// </param>
        /// <returns>
        /// A string resulting from trimming trimChars from the right side of inputString.
        /// </returns>
        public static string trimRight(string inputString, char[] trimChars)
        {
            string r = string.Empty;

            // don't do anything if the inputString isn't really a thing
            if (!string.IsNullOrEmpty(inputString))
            {
                char[] c = copyStringToCharArray(inputString);

                if ((c.Length > 0) && !object.ReferenceEquals(trimChars, null) && (trimChars.Length > 0))
                {
                    //
                    // stopTrim will toggle to false if we meet any match in trimChars
                    //   * This boolean value must be declared outside the do/while loop in order for the loop to use it
                    //
                    bool stopTrim = true;
                    do
                    {
                        // reset stopTrim to true at the beginning of each loop pass
                        stopTrim = true;
                        for (int i = 0; i < trimChars.Length; i++)
                        {
                            // Test the rightmost string character for equality with each member of trimChars
                            //   * Make sure this is a negative equality - stopTrim must be FALSE if a match is found
                            stopTrim = !c[c.Length - 1].Equals(trimChars[i]);
                            if (!stopTrim)
                            {
                                // if stopTrim is now false, perform right-side trimming into the char array
                                arrayExtensions.removeAt<char>(ref c, c.Length - 1);
                                // and break from the for loop
                                break;
                            }
                        }
                        //
                        // if at any point the rightmost character fails to match any of the trimChars, stopTrim will be TRUE and 
                        // therefore the loop will end
                        // OR
                        // if at any point the entire string is trimmed to 0 length, the loop will end
                        //
                    } while (!stopTrim && (c.Length > 0));
                }

                // finally cut the char array back into the string return value
                r = new string(c);
            }

            return r;
        }

        /// <summary>
        /// Trims any/all occurrances of the specified trimString from the right side of the specified inputString, and returns the 
        /// resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimString">
        /// The string to trim from inputString.
        /// </param>
        /// <returns>
        /// The string resulting from trimming trimString from the right side of inputString.
        /// </returns>
        public static string trimRight(string inputString, string trimString)
        {
            string r = copyStringToString(inputString);

            if (!string.IsNullOrEmpty(inputString) && !string.IsNullOrEmpty(trimString))
            {
                //
                // I have seen it happen where, occasionally, DotNET will decide to treat reference types as if they were passed by the ref 
                // keyword whether they were or not.  To avoid this, take a copy of the inputString rather than modifying it inplace during 
                // function processing.
                //
                r = copyStringToString(inputString);

                // Ensuring that the trimString would actually fit into the string it would be trimmed from
                if (trimString.Length <= r.Length)
                {
                    // Testing if there is any initial match from the right side of the test value with the trimString
                    //
                    // The startIndex value tells us where to start our string evaluation test
                    //   * This is really more relevant to trimRight than trimLeft - in trimRight it requires a bit more of a complex math 
                    //     statement that's better placed into a seprate int declaration than horked into an if() statement, but it was 
                    //     included in trimLeft code also for uniformity
                    int startIndex = r.Length - trimString.Length;
                    if (r.Substring(startIndex, trimString.Length).Equals(trimString))
                    {
                        // The match exists, so begin trimming - we'll continue trimming trimString from inputString until/unless there is no 
                        // further matchup
                        do
                        {
                            //
                            // For trimLeft, the substring grabber below begins at (index = trimString.length) and then the length of the grab 
                            // must be the length of the remaining inputString minus the start index
                            //
                            // For trimRight, the substring grabber begins at (index = 0) and then the length of the grab must be the length of 
                            // the inputString up to the start index
                            //
                            r = r.Substring(0, startIndex);
                            //
                            // Since we just cut the end off the string, trimRight must reset the startIndex
                            //
                            startIndex = r.Length - trimString.Length;
                            //
                            // If at any point the output value is trimmed to 0 length, the loop will end 
                            // OR
                            // If at any point the remaining output length is no longer enough to fit the trimString into, the loop will end
                            // OR
                            // If at any point the next substring no longer matches the trimstring, the loop will end
                            //
                        } while ((r.Length > 0) && (trimString.Length <= r.Length) && r.Substring(startIndex, trimString.Length).Equals(trimString));
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Trims any/all occurrances of the specified trimChars from both the left and right sides of the specified inputString, and 
        /// returns the resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimChars">
        /// A char array.  The characters to trim from inputString.
        /// </param>
        /// <returns>
        /// A string resulting from trimming trimChars from the left and right sides of inputString.
        /// </returns>
        public static string trimBoth(string inputString, char[] trimChars)
        {
            string r = inputString;
            //
            // Calling trimRight first will make trimLeft's job easier - trimLeft calls out to arrayExtensions.removeAt which has to do the 
            // job of copying array data from all character indices forward of 0 backstepping once for each item, so the fewer items there 
            // are (even though it's only really a potential fewer with the trim operation here) the better performance we get per callout 
            // to trimBoth
            //
            r = trimRight(r, trimChars);
            r = trimLeft(r, trimChars);
            return r;
        }

        /// <summary>
        /// Trims any/all occurrances of the specified trimString from both the left and right sides of the specified inputString, and 
        /// returns the resulting string.
        /// </summary>
        /// <param name="inputString">
        /// The string to trim characters from.
        /// </param>
        /// <param name="trimString">
        /// The string to trim from inputString.
        /// </param>
        /// <returns>
        /// The string resulting from trimming trimString from the left and right sides of inputString.
        /// </returns>
        public static string trimBoth(string inputString, string trimString)
        {
            string r = inputString;
            r = trimRight(r, trimString);
            r = trimLeft(r, trimString);
            return r;
        }
    }
}
