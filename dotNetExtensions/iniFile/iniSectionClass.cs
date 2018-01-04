using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetExtensions
{
    
    /// <summary>
    /// Represents an individual INI Section and its Keys or Properties content.
    /// </summary>
    public partial class iniSectionClass
    {
        /// <summary>
        /// Contains the INI Section Name.
        /// </summary>
        public string sectionName = null;

        /// <summary>
        /// An array of iniKeyClass instances representing the INI Keys subordinate to this section.
        /// </summary>
        public iniKeyClass[] iniKeys = null;

        /// <summary>
        /// Gets the index into the section's iniKeys collection of the key with the given name.  The name is case-insensitive.  
        /// 
        /// If no key is found with a matching name, the return value will be -1.
        /// </summary>
        /// <param name="keyName">
        /// The name of the key to retrieve from the section.  The key name is case-insensitive.  
        /// </param>
        /// <returns>
        /// An integer value.  The index into the iniKeys collection where an iniKeyClass instance with a matching name will be found.  
        /// 
        /// If no key is found with a matching name, the return value will be -1.
        /// </returns>
        public int getKeyIndex(string keyName)
        {
            int r = -1;

            if (!object.ReferenceEquals(this.iniKeys, null))
            {
                for (int i = 0; i < this.iniKeys.Length; i++)
                {
                    if (keyName.ToLower() == this.iniKeys[i].keyName.ToLower())
                    {
                        r = i;
                        break;
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Gets a key with the given name from the section's iniKeys collection.  The name is case-insensitive.  
        ///
        /// If no key is found with a matching name, the return value will be null.
        /// </summary>
        /// <param name="keyName">
        /// The name of the key to retrieve from the section.  The key name is case-insensitive.  
        /// </param>
        /// <returns>
        /// An iniKeyClass instance.  The INI Key with a name matching the one specified.  
        /// 
        /// If no key is found with a matching name, the return value will be null.
        /// </returns>
        public iniKeyClass getKey(string keyName)
        {
            iniKeyClass r = null;

            if (!object.ReferenceEquals(this.iniKeys, null))
            {
                for (int i = 0; i < this.iniKeys.Length; i++)
                {
                    if (keyName.ToLower() == this.iniKeys[i].keyName.ToLower())
                    {
                        r = this.iniKeys[i];
                        break;
                    }
                }
            }

            return r;
        }

        

        /// <summary>
        /// Constructor.  Initializes the iniSectionClass with an empty string as its sectionName and an empty collection of iniKeys.
        /// </summary>
        public iniSectionClass()
        {
            this.sectionName = string.Empty;
            this.iniKeys = new iniKeyClass[0];
        }

        /// <summary>
        /// Constructor.  Initializes the iniSectionClass with the specified sectionName and an empty collection of iniKeys.
        /// </summary>
        /// <param name="newname">
        /// The name for the new iniSectionClass instance.
        /// </param>
        public iniSectionClass(string newname)
        {
            this.sectionName = newname;
            this.iniKeys = new iniKeyClass[0];
        }

        /// <summary>
        /// Destructor.  Cleans up memory objects as the iniSectionClass is de-initialized.
        /// </summary>
        ~iniSectionClass()
        {
            this.iniKeys = null;
            this.sectionName = null;
        }

        /// <summary>
        /// Determines whether the specified string is a valid INI Section heading identifier.
        /// </summary>
        /// <param name="testString">
        /// The string instance to test.
        /// </param>
        /// <returns>
        /// A boolean value.  True if testString represents a valid INI Section heading identifier.  Otherwise False.
        /// </returns>
        public static bool isINISection(string testString)
        {
            bool r = false;
            //
            // if testString isn't really a thing then we're already done
            //
            if (!string.IsNullOrEmpty(testString))
            {
                //
                // since every language/platform does things differently, this is a platform/syntax agnostic way of testing strings in a 
                // regular expressions type manner
                //
                char wtSpace = (char)0x20; // whitespace char
                char ltBrack = (char)0x5b; // the [ char
                char rtBrack = (char)0x5d; // the ] char
                char[] lineTermChars = new char[] { (char)0x0d, (char)0x0a }; // CR and LF line terminators
                
                char[] testChars = testString.ToCharArray();
                // a boolean indicating whether the section head has started
                bool secHeadStarted = false;
                // a string value containing any section name we find between section brackets that may or may not exist
                string secHeadName = string.Empty;
                // a boolean indicating whether the section head has exited
                bool secHeadExited = false;
                for (int i = 0; i < testChars.Length; i++)
                {
                    if (!secHeadStarted)
                    {
                        // while the section heading has not yet started
                        if (testChars[i] == wtSpace)
                        {
                            // flatly ignore whitespace
                        }
                        else if (testChars[i] == ltBrack)
                        {
                            // toggle secHeadStarted if we encounter a left bracket
                            secHeadStarted = true;
                        }
                        else
                        {
                            // any other character at all represents an invalid section heading line
                            r = false;
                            break;
                        }
                    }
                    else if (!secHeadExited)
                    {
                        // otherwise if the section heading has started but the section heading has not yet exited
                        if (testChars[i] == rtBrack)
                        {
                            // toggle secHeadExited if we encounter a right bracket
                            secHeadExited = true;
                            //
                            // if the currently inspected character is a ] char (and secHeadStarted is true) then the test as to whether this
                            // is a valid INI section heading or not becomes a question of whether the text between the brackets contains non-
                            // space text at all (empty names are not valid)
                            //
                            r = (stringExtensions.trimBoth(secHeadName, new char[] { (char)0x20 }).Length > 0);
                        }
                        else if (arrayExtensions.contains<char>(ref lineTermChars, testChars[i], false))
                        {
                            // if we encounter any lineterms between section heading brackets this represents an invalid section heading line
                            r = false;
                            break;
                        }
                        else
                        {
                            // any other character at all represents a valid section heading identifier
                            secHeadName += testChars[i];
                        }
                    }
                    else
                    {
                        // if the section heading has both started and exited
                        if (testChars[i] == wtSpace)
                        {
                            // flatly ignore whitespace
                        }
                        else if (arrayExtensions.contains<char>(ref lineTermChars, testChars[i], false))
                        {
                            // flatly ignore line terms as long as we're dealing with nothing else in the string but lineterms and whitespace
                        }
                        else
                        {
                            // any other character at all represents an invalid section heading line
                            r = false;
                            break;
                        }
                    }
                }
            }

            return r;
        }
    }
}
