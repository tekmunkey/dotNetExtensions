using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetExtensions
{
    public static partial class stringExtensions
    {
        /// <summary>
        /// The default collection of what are considered 'good line breaking chars' for word wrapping operations.  These are space, 
        /// dash, and underscore.
        /// </summary>
        public static char[] defaultGoodBreakChars = new char[]
        {
            (char)0x20, // space char
            (char)0x2d, // - (dash/minus) char
            (char)0x5f, // _ (underscore) char
        };

        /// <summary>
        /// Gets a value indicating whether the indicated char is one of a list of characters that represent good places to break a line for 
        /// natural wrapping operations.  
        /// 
        /// When no list is provided by a user (the goodBreakChars parameter is null) a default collection is used from stringExtensions.defaultGoodBreakChars.  
        /// </summary>
        /// <param name="c">
        /// The character to test.
        /// </param>
        /// <param name="goodBreakChars">
        /// An array of characters representing good characters to perform a linebreak upon.  
        /// 
        /// When no list is provided by a user (the goodBreakChars parameter is null) a default collection is used from stringExtensions.defaultGoodBreakChars.  
        /// </param>
        /// <returns>
        /// A boolean value.  True if the tested character is a good breaking point for word wrap, otherwise False.
        /// </returns>
        public static bool isGoodLineWrapChar(char c, char[] goodBreakChars)
        {
            bool r = false;
            //
            // If goodBreakChars param is null, default it
            //
            if (object.ReferenceEquals(goodBreakChars, null)) { goodBreakChars = stringExtensions.defaultGoodBreakChars; }

            foreach (char gbc in goodBreakChars)
            {
                r = (gbc == c);
                if (gbc == c)
                {
                    r = true;
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// Gets an array of indexes pointing to chars in the specified array that represent the possible valid wrap positions 
        /// (the characters considered good breaking points in the line, for wrapping).  
        /// </summary>
        /// <param name="inCharArray">
        /// A char array to scan for wrap positions.
        /// </param>
        /// <param name="goodBreakChars">
        /// An array of characters representing good characters to perform a linebreak upon.  
        /// 
        /// When no list is provided by a user (the goodBreakChars parameter is null) a default collection is used from stringExtensions.defaultGoodBreakChars.  
        /// </param>
        /// <returns>
        /// An array of integers representing index positions where wrap positions can be found in the input array.
        /// </returns>
        public static int[] getAllWrapIndices(char[] inCharArray, char[] goodBreakChars)
        {
            int[] r = new int[0];

            if (inCharArray != null)
            {
                for (int i = 0; i < inCharArray.Length; i++)
                {
                    if (isGoodLineWrapChar(inCharArray[i], goodBreakChars))
                    {
                        arrayExtensions.push<int>(ref r, i);
                    }
                }
                // add the array length itself as a wrap index too - this is the End of Stream or End of File marker
                arrayExtensions.push<int>(ref r, inCharArray.Length);
            }
            return r;
        }

        /// <summary>
        /// Applies padding to the specified string, if and only if forceWidth is true.  Provided for use with getWrappedLines().
        /// </summary>
        /// <param name="inputText">
        /// A single line of text to apply padding to, if necessary.
        /// </param>
        /// <param name="lineWidth">
        /// The line width to pad to.
        /// </param>
        /// <param name="forceWidth">
        /// A boolean value.  If True, then each line will be forcibly padded to fit lineWidth.  If False, no padding is performed.
        /// </param>
        /// <param name="padString">
        /// A string value.  The text that will be used to pad each line to fit lineWidth.  If null or empty, defaults to a single space character.
        /// </param>
        /// <param name="padSide">
        /// An integer value.  If 0 pads the text on both left and right (text is centered).  If -1 or less, pads the text on the left side.  If 1 or greater, pads the text on the right side.
        /// </param>
        /// <returns></returns>
        public static string getPaddedLine(string inputText, int lineWidth, bool forceWidth, string padString, int padSide)
        {
            string r = inputText;
            //
            // testing if padding is needed
            //
            if (forceWidth && (r.Length < lineWidth))
            {
                // Set up a fillString to contain the fill characters
                string fillString = string.Empty;
                // determine the number of chars needed to fill the line
                int fillNeeded = (lineWidth - r.Length);
                //
                // Now we need to iterate through the blank spaces in the text line, to fill them up.  Each one needs to be filled with a consecutive character from the padString collection.
                //
                // The best way to do this, since we don't know the actual length of either one but we do know that our only hard limit is fillNeeded, is to iterate up to that.
                // SO:
                //
                // Get a max count on the number of characters
                int maxCharCount = padString.Length;
                // Get a tracking cursor for the current padString character to use
                int curCharCursor = 0;
                // And finally begin iterating
                for (int i = 0; i < fillNeeded; i++)
                {
                    fillString += padString[curCharCursor];
                    curCharCursor++;
                    if (curCharCursor >= padString.Length) { curCharCursor = 0; }
                }
                //
                // The above rapidly, perfectly, and accurately fills up a fill string's empty space without a bunch of confusing mathematical mumbo jumbo, which ultimately results in 
                // blank spaces and even more confusing math mumbo jumbo, and performance dragging mid() or string.SubString() callouts.
                //
                // Below, we append, prepend, or chop that string up and slap it in around the output value as needed
                //
                if (padSide > 0)
                {
                    // padSide > 0 means right-side pad
                    r = r + fillString;
                }
                else if (padSide < 0)
                {
                    // padSide < 0 means left-side pad
                    r = fillString + r;
                }
                else if (padSide == 0)
                {
                    // padside == 0 means both-sides pad
                    //
                    // The next value will usually result in an uneven division so we don't want to rely on it to produce a full string output
                    //
                    int leftPad = (int)Math.Floor((decimal)fillString.Length / (decimal)2);
                    // Account for any leftPad discrepancies by subtraction as below
                    int rightPad = fillString.Length - leftPad;
                    r = fillString.Substring(0, leftPad) + r + fillString.Substring(leftPad, rightPad);
                }
            }
            return r;
        }

        /// <summary>
        /// Converts the specified string of inputText into an array of individual lines split at no more than the 
        /// specified lineWidth.  
        /// </summary>
        /// <param name="inputText">
        /// The text to wrap.
        /// </param>
        /// <param name="lineWidth">
        /// The maximum width of each line, specified as a column or character count.
        /// </param>
        /// <param name="goodBreakChars">
        /// An array of characters representing good characters to perform a linebreak upon.  
        /// 
        /// When no list is provided by a user (the goodBreakChars parameter is null) a default collection is used from stringExtensions.defaultGoodBreakChars.  
        /// </param>
        /// <param name="forceWidth">
        /// A boolean value.  If True, then each line will be forcibly padded to fit lineWidth.  If False, no padding is performed.
        /// </param>
        /// <param name="padString">
        /// A string value.  The text that will be used to pad each line to fit lineWidth.  If null or empty, defaults to a single space character.
        /// </param>
        /// <param name="padSide">
        /// An integer value.  If 0 pads the text on both left and right (text is centered).  If -1 or less, pads the text on the left side.  If 1 or greater, pads the text on the right side.
        /// </param>
        /// <returns>
        /// An array of strings no more than lineWidth characters long.  If lineWidth is ridiculously short compared to 
        /// a ridiculously long word with no possible linebreaks, you'll have unpredictable results.
        /// </returns>
        public static string[] getWrappedLines(string inputText, int lineWidth, char[] goodBreakChars, bool forceWidth, string padString, int padSide)
        {
            string[] r = new string[0];

            if (!string.IsNullOrEmpty(inputText))
            {
                //
                // Default padString to a single space if it's not provided.
                //
                if (string.IsNullOrEmpty(padString)) { padString = @" "; }
                //
                // we're going to do some funkery with inputText to avoid modifying the original string reference
                //
                char[] workChars = new char[inputText.Length];
                inputText.CopyTo(0, workChars, 0, workChars.Length);
                //
                // And BOOM goes the funkery.
                //
                // Converting workChars to a string, replacing horizontal tabs in that string with 4 spaces, converting that string back to 
                // a char array, and reassigning that value to workChars.
                //
                // Horizontal tabstops are a real bitch when it comes to word wrapping.  It works fine in a text editor because the text editor
                // knows itself.  The tabstop is a --single-- Unicode character (1 byte) in length.  A text editor knows that it will display 
                // that tabstop at a width equal to 4 spaces, or 6 spaces, or 10 spaces, or whatever.
                //
                // The problem comes in where we're performing this procedure for transmission across servers and clients.  We have no idea what 
                // different/various clients are going to do with a horizontal tabstop.  We count it as 1 character, and one client may display it 
                // as 4 spaces while another may display it as 6 while another may display it as 10, so no matter what we do with word wrapping 
                // will be wrong.  Any line with a tabstop will either be too long or too short compared to every other line that was wrapped.
                //
                // EXCEPT THIS.  We convert horizontal tabstops straight across to 4 individual whitespace characters.  Now when we measure for 
                // wordwrapping, we know exactly how many characters to measure for and we know exactly what every single client is going to do 
                // with what we transmit.  Neat, huh?  Handy, even.
                //
                workChars = (new String(workChars)).Replace(new string((char)0x09, 1), @"    ").ToCharArray();
                //
                // always respect any pre-existing linebreaks in the original text, ie: the user's original formatting
                // !!! DO NOT REMOVE EMPTY ENTRIES - this respects user-formatted blank lines !!!
                //
                string[] workLines = (new String(workChars)).Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
                //
                // Testing that the working array is actually a thing first.  It should be since inputText was not null - string.Split should 
                // always return at least 1 element - but redundancy in error checking is good.
                //
                if (workLines != null)
                {
                    //
                    // iterating from 0 to array length means that we do nothing if the array is non-null but empty, but we do something
                    // if the array is non-empty
                    //
                    // this is passing line by line
                    //
                    for (int lineIdx = 0; lineIdx < workLines.Length; lineIdx++)
                    {
                        //
                        // lstBreakCursor tracks the last linebreak position, or the last char position in the line where a linebreak was actually performed.  This way 
                        // we can backtrack from a previous break position to the next candidate position.
                        //
                        int lstBreakCursor = 0;
                        //
                        // curBreakCandidate tracks the last linebreak candidate, or the last char position in the line where the cursor encountered a goodBreakChar
                        // This way we can backtrack to a previous candidate position if/when we pass the column limit without finding a goodBreakChar exactly 
                        // AT the column limit
                        //
                        // curBreakCandidate tracks into The Big Picture, or the total line, not the line that's currently wrapped.
                        //
                        int curBreakCandidate = 0;
                        //
                        // Capture thisLine as a char array for ease of access, and as earlier perform some char[]->string->char funkery to add the user options to each line
                        //
                        char[] thisLine = workLines[lineIdx].ToCharArray();
                        //
                        // again paying respect to user-formatting and blank/empty lines
                        //
                        if (thisLine.Length != 0)
                        {
                            //
                            // Capture all potential wrapping points from thisLine
                            //
                            int[] thisLineWrapIndices = getAllWrapIndices(thisLine, goodBreakChars);
                            //
                            // Iterate through the potential wrap points in the currently inspected line
                            //
                            for (int i = 0; i < thisLineWrapIndices.Length; i++)
                            {
                                int wrapSpot = thisLineWrapIndices[i];
                                if ((wrapSpot - lstBreakCursor) < lineWidth)
                                {
                                    // as long as wrapSpot continues to fit inside a line width, continue updating the break candidate
                                    curBreakCandidate = wrapSpot;
                                }
                                else
                                {
                                    //
                                    // as soon as an overflow is hit, we have to stop to perform back-wrapping
                                    //
                                    // to the return value add the line
                                    // * In the final argument, subtract lstBreakCursor from curBreakCandidate to achieve a LENGTH from lstBreakCursor to copy chars
                                    //
                                    string newLine = new string(thisLine, lstBreakCursor, ((curBreakCandidate + 1) - lstBreakCursor));
                                    // apply padding if necessary (the padding function checks for us)
                                    newLine = getPaddedLine(newLine, lineWidth, forceWidth, padString, padSide);
                                    // Add newLine to the output
                                    arrayExtensions.push<string>(ref r, newLine);
                                    // reset lstBreakCursor to curBreakCandidate
                                    lstBreakCursor = curBreakCandidate + 1; // add 1 so we skip over the char we just used as a breaking position
                                    // now that we've performed the wrapping, curBreakCandidate becomes the current/overflow position
                                    curBreakCandidate = wrapSpot;
                                }
                            }
                            //
                            // We must ensure that the entire line is copied whether it overflowed a wrapping boundary/column width or not
                            //
                            if (lstBreakCursor != thisLine.Length)
                            {
                                //
                                // * In the final argument, subtract lstBreakCursor from curBreakCandidate to achieve a LENGTH from lstBreakCursor to copy chars
                                //
                                string newLine = new string(thisLine, lstBreakCursor, (thisLine.Length - lstBreakCursor));
                                // apply padding if necessary (the padding function checks for us)
                                newLine = getPaddedLine(newLine, lineWidth, forceWidth, padString, padSide);
                                // Add newLine to the output
                                arrayExtensions.push<string>(ref r, newLine);
                                //
                                // and ensure that all the cursors know we're at the very end of the stream - we should be about to re-loop in any/all events
                                // which resets everything to 0 anyway, but redundancy and error avoidance is good
                                //
                                lstBreakCursor = thisLine.Length;
                                curBreakCandidate = lstBreakCursor;
                            }
                            thisLineWrapIndices = null;
                        }
                        else
                        {
                            //
                            // if line length was equal to 0, then the line was blank/empty
                            //
                            // to the return value add padding only
                            arrayExtensions.push<string>(ref r, getPaddedLine(String.Empty, lineWidth, forceWidth, padString, padSide));
                        }
                        thisLine = null;
                    }
                }
                workLines = null;
                workChars = null;
                GC.Collect();
            }

            return r;
        }

        /// <summary>
        /// Converts the specified string of inputText into an array of individual lines split at no more than the 
        /// specified lineWidth.  A specified list of characters are used to identify good positions to wrap on.
        /// </summary>
        /// <param name="inputText">
        /// The text to wrap.
        /// </param>
        /// <param name="lineWidth">
        /// The maximum width of each line, specified as a column or character count.
        /// </param>
        /// <param name="lineDecoLeft">
        /// A line decoration string that will be applied to the left hand side.
        /// </param>
        /// <param name="lineDecoRight">
        /// A line decoration string that will be applied to the right hand side.
        /// </param>
        /// <param name="goodBreakChars">
        /// An array of characters representing good characters to perform a linebreak upon.  
        /// 
        /// When no list is provided by a user (the goodBreakChars parameter is null) a default collection is used from stringExtensions.defaultGoodBreakChars.  
        /// </param>
        /// <param name="forceWidth">
        /// A boolean value.  If True, then each line will be forcibly padded to fit lineWidth.  If False, no padding is performed.
        /// </param>
        /// <param name="padString">
        /// A string value.  The text that will be used to pad each line to fit lineWidth.  If null or empty, defaults to a single space character.
        /// </param>
        /// <param name="padSide">
        /// An integer value.  If 0 pads the text on both left and right (text is centered).  If -1 or less, pads the text on the left side.  If 1 or greater, pads the text on the right side.  
        /// 
        /// Padding is applied --inside-- lineDeco, so if you apply lineDecoLeft then the line deco appears before the padding, then the text.
        /// </param>
        /// <returns>
        /// An array of strings no more than lineWidth characters long.  If lineWidth is ridiculously short compared to 
        /// a ridiculously long word with no possible linebreaks, you'll have unpredictable results.
        /// </returns>
        public static string[] getWrappedLines(string inputText, int lineWidth, string lineDecoLeft, string lineDecoRight, char[] goodBreakChars, bool forceWidth, string padString, int padSide)
        {
            string[] r = new string[0];

            // conform lineWidth to fit lineDecos before calling out to other wrap functions
            int newLineWidth = lineWidth - ((!string.IsNullOrEmpty(lineDecoLeft) ? lineDecoLeft.Length : 0) + (!string.IsNullOrEmpty(lineDecoRight) ? lineDecoRight.Length : 0));
            r = stringExtensions.getWrappedLines(inputText, newLineWidth, goodBreakChars, forceWidth, padString, padSide);
            // apply line decos per line
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = (!string.IsNullOrEmpty(lineDecoLeft) ? lineDecoLeft : string.Empty) + r[i] + (!string.IsNullOrEmpty(lineDecoRight) ? lineDecoRight : string.Empty);
            }

            return r;
        }
    }
}
