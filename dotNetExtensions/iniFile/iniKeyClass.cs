using System;

namespace dotNetExtensions
{
    /// <summary>
    /// Represents an individual INI Key or Property value.
    /// </summary>
    public partial class iniKeyClass
    {
        /// <summary>
        /// Contains the INI Key Name, or its identifier which resides on the left side of the = in the INI file.
        /// </summary>
        public string keyName = null;
        /// <summary>
        /// Contains the INI Key Value, or its content which resides on the right side of the = in the INI file.
        /// </summary>
        public string keyValue = null;
        /// <summary>
        /// Contains the separator value that was found in the original content.  Some INI implementations may allow or recognize keys 
        /// that contain spaces around the = char while others may not.  We would like to ensure that we don't break anything if users 
        /// open and resave their INI files with our engine.
        /// </summary>
        public string sepValue = null;

        /// <summary>
        /// Converts the iniKeyClass instance's keyName and keyValue properties to a writable value in the form of 'keyName = keyValue'
        /// </summary>
        /// <returns>
        /// A string.  The iniKeyClass instance's keyName and keyValue properties to a writable value in the form of 'keyName = keyValue'
        /// </returns>
        public string getWritableKey()
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(this.keyName))
            {
                r = this.keyName + this.sepValue + this.keyValue;
            }
            return r;
        }

        /// <summary>
        /// Constructor.  Initializes the iniKeyClass with empty strings as its keyName and keyValue field values.
        /// </summary>
        public iniKeyClass()
        {
            this.keyName = string.Empty;
            this.sepValue = string.Empty;
            this.keyValue = string.Empty;
        }

        /// <summary>
        /// Constructor.  Initializes the iniKeyClass with the specified keyName and keyValue field values.
        /// </summary>
        /// <param name="newname">
        /// A string instance.  The keyName to set into the new iniKeyClass instance.
        /// </param>
        /// <param name="newvalue">
        /// A string instance.  The keyValue to set into the new iniKeyClass instance.
        /// </param>
        public iniKeyClass(string newname, string newvalue)
        {
            this.keyName = newname;
            this.sepValue = string.Empty;
            this.keyValue = newvalue;
        }

        /// <summary>
        /// Destructor.  Cleans up memory objects as the iniKeyClass instance is de-initialized.
        /// </summary>
        ~iniKeyClass()
        {
            this.keyValue = null;
            this.sepValue = null;
            this.keyName = null;
        }

        /// <summary>
        /// Determines whether the specified string is a valid INI Key or Property identifier.  The processing engine is format agnostic.  It will accept INI values in the form of keyName=keyValue, keyName = keyValue, and keyName (without 
        /// keyValue), but it will not support non-standard formats such as multi-line INI outputs or 'interpolation' styles which are not INI files at all.
        /// </summary>
        /// <param name="testString">
        /// The string instance to test.
        /// </param>
        /// <returns>
        /// A boolean value.  True if testString represents a valid INI Key value.  Otherwise False.
        /// </returns>
        public static bool isINIKey(string testString)
        {
            bool r = false;

            //
            // if testString isn't really a thing or else it's actually an INI Section definition then we're already done
            //   * Section names may appear to be keys/properties by containing = chars, but keys/properties will never appear to be sections 
            //     because keys/properties will never be surrounded by square brackets
            //
            if (!string.IsNullOrEmpty(testString) || !iniSectionClass.isINISection(testString))
            {
                //
                // since every language/platform does things differently, this is a platform/syntax agnostic way of separating strings at a 
                // particular character
                //
                string keyName = string.Empty;
                string keyValue = string.Empty;

                char keyNameSep = (char)0x3d; // the = character

                char[] testChars = testString.ToCharArray();
                // the valueStarted boolean will toggle the first time we encounter the = sign in the line
                bool valueStarted = false;
                for (int i = 0; i < testChars.Length; i++)
                {
                    if (!valueStarted)
                    {
                        //
                        // Value has not yet started
                        //
                        if (arrayExtensions.contains<char>(ref iniFileClass.linetermChars, testChars[i], false) || arrayExtensions.contains<char>(ref iniFileClass.commentChars, testChars[i], false))
                        {
                            // Do not accept line terms or semicolons during key/property name retrieval
                            r = false;
                            break;
                        }
                    }
                    else if (testChars[i] == keyNameSep)
                    {
                        //
                        // if the currently inspected character is an = sign then the test as to whether this is a valid INI key or not 
                        // becomes a question of whether the left side of the equals sign contains non-space text at all (empty values 
                        // are perfectly valid, including keys/properties with no = char at all, but empty names are not)
                        //
                        r = (stringExtensions.trimBoth(keyName, new char[] { (char)0x20 }).Length > 0);
                    }
                    else
                    {
                        // In all other cases, simply add the currently inspected character to the keyName
                        keyName += testChars[i];
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Constructs an iniKeyClass instance from the specified string value.  The processing engine is format agnostic.  It will accept INI values in the form of keyName=keyValue, keyName = keyValue, and keyName (without 
        /// keyValue), but it will not support non-standard formats such as multi-line INI outputs or 'interpolation' styles which are not INI files at all.
        /// </summary>
        /// <param name="fromString">
        /// 
        /// </param>
        /// <returns></returns>
        public static iniKeyClass getINIKey(string fromString)
        {
            iniKeyClass r = new iniKeyClass();

            if (isINIKey(fromString))
            {
                //
                // since every language/platform does things differently, this is a platform/syntax agnostic way of separating strings at a 
                // particular character
                //
                char keyNameSep = (char)0x3d; // the = character

                char[] testChars = fromString.ToCharArray();
                // the valueStarted boolean will toggle the first time we encounter the = sign in the line
                bool valueStarted = false;
                for (int i = 0; i < testChars.Length; i++)
                {
                    if (!valueStarted)
                    {
                        // if the value has not yet started, we will test whether to start it
                        if (testChars[i].Equals(keyNameSep))
                        {
                            // if this is an = char then toggle valueStarted and we're done with the key name by the next pass
                            valueStarted = true;
                            // set up the sepValue into the new instance
                            if ((i > 0) && arrayExtensions.contains<char>(ref iniFileClass.whitespaceChars, testChars[i - 1], false))
                            {
                                // This if case tests first if the index is > 0 to ensure we don't underrun the array bounds
                                // The andalso case tests if there is whitespace around the = sign - if so, then copy it along with the = sign
                                r.sepValue = testChars[i - i] + @"=";
                                // Additionally we must test if the string is long enough to contain additional chars, and if so whether there 
                                // is more whitespace ahead of the = sign, and if so copy that in as well
                                if (((i + 1) < testChars.Length) && arrayExtensions.contains<char>(ref iniFileClass.whitespaceChars, testChars[i + 1], false))
                                {
                                    r.sepValue += testChars[i + i];
                                }
                            }
                            else
                            {
                                r.sepValue = @"=";
                            }
                        }
                        else
                        {
                            // if this isn't an = char then simply append the char to the key name
                            r.keyName += testChars[i];
                        }
                    }
                    else
                    {
                        if (arrayExtensions.contains<char>(ref iniFileClass.linetermChars, testChars[i], false))
                        {
                            // if we've encountered any type of line terminater, we're done with the value
                            break;
                        }
                        else
                        {
                            // otherwise if the value has started, simply append to the key value
                            r.keyValue += testChars[i];
                        }
                    }
                }
                r.keyName = stringExtensions.trimBoth(r.keyName, new char[] { (char)0x20 });
                // Do not set r.keyName.ToLower() in here - only do that during value equality testing and only in-memory - let case insensitivity be a back-end thing, never let it muck up user aesthetics
                r.keyValue = stringExtensions.trimBoth(r.keyValue, new char[] { (char)0x20 });
            }

            return r;
        }
    }
}
