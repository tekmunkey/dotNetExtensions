using System;
using System.IO;
using System.Text;

namespace dotNetExtensions
{
    /// <summary>
    /// The iniFileClass encapsulates what is needed to work with an INI-formatted configuration file.
    /// </summary>
    /// <remarks>
    /// The INI File Format is an "unofficial" standard originally designed by Microsoft to provide human readable/editable software configuration 
    /// files.  
    /// 
    /// The INI file itself is a flatfile database (CSV) delimited by linebreaks where each entry may be a [Section Header] (where the section 
    /// header must be enclosed in square brackets) or each entry may be a keyName=keyValue entry where the key or property entry must contain 
    /// an equals sign, and then each line-item value following that equals sign may in fact be a full-fledged multi-tiered CSV in and of itself.  
    /// 
    /// A lot of implementations get this really godawfully wrong, including Python's ConfigParser implementation which allows for multi-line 
    /// INI keys/properties and even more moronically, commentation on the same line along with referencial backlinks from one INI property to 
    /// another in the same file.  The reason why the INI file takes one entry per line and doesn't allow commentation on the same line as a key 
    /// or section entry is that each line could potentially be a CSV itself, or a multiple-tiered CSV using literally any collection of character 
    /// delimiters.  So Python's implementation isn't even an INI file anymore, destroys human readability and editation, literally nerfs the standard 
    /// by eliminating 90% of the purpose for it, and goes as far as to make their output files immediately incompatible with literally every 
    /// implementation that actually is built to meet the standard (including the one built into the WinAPI which is literally the original which spawned 
    /// the standard in the first place).  It was in fact reading up on Python's retard abortion of an INI implementation that inspired me to write my 
    /// own.  
    /// 
    /// Why DotNET doesn't provide a native INI implementation I don't know.  Maybe they intended for developers to fall back on the WinAPI 
    /// implementation?  If so, then Microsoft is just as stupid as the Python devs, because calling out to the WinAPI from a DotNET app immediately 
    /// breaks the portability of the DotNET app.
    /// </remarks>
    public partial class iniFileClass
    {
        /// <summary>
        /// fileName contains the name of the last filepath successfully loaded by the loadFile function.
        /// </summary>
        public string fileName = null;

        /// <summary>
        /// fileLineTerm contains the line terminator that was determined to have been in use in the last filepath successfully loaded by the loadFile function, or is defaulted to System.Environment.NewLine.
        /// </summary>
        public string fileLineTerm = null;

        /// <summary>
        /// fileEncoding contains a file encoding that will be used when the file is written out to disk.  This is determined from the last file successfully loaded by the loadFile function, or is defaulted 
        /// to System.Text.Encoding.ASCII.
        /// </summary>
        public Encoding fileEncoding = null;

        /// <summary>
        /// The iniLines array contains all the lines of text read from the INI File on disk, including commentation.  By doing this, we can read what the user wrote 
        /// as notes to themselves and work around it rather than overwriting it any time data is saved back to disk.
        /// </summary>
        public string[] iniLines = null;

        internal static char[] commentChars = new char[] 
        { 
            (char)0x3b, // the ; (semicolon) character is recognized as a comment character when it is the first character on a line
            (char)0x23, // the # (hash) character is recognized as a comment character when it is the first character on a line
        };

        internal static char[] linetermChars = new char[]
        {
            (char)0x0a, // the LF (line feed) character
            (char)0x0d, // the CR (carriage return) character
        };

        internal static char[] whitespaceChars = new char[]
        {
            (char)0x09, // the HT (horizontal tab) character - the standard tabstop
            (char)0x20, // the WS (whitespace) character - the literal single non-breaking space
        };

        /// <summary>
        /// Loads an INI file from the specified filepath.
        /// </summary>
        /// <param name="filepath">
        /// A path to the INI file to load into memory.
        /// </param>
        public void loadFile(string filepath)
        {
            // clean up any fileName or iniData that was previously in place
            this.fileName = null;
            this.iniLines = null;
            GC.Collect();

            // reading the raw INI file data into the iniFileClass data buffer, in whatever native file encoding is in place
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
            fs.Position = 0;
            // re-initialize iniData
            this.iniLines = new string[0];
            // a data cursor into iniData as we read through the file
            int datOffset = 0;

            // since Windows XP or so 4096 is Windows' sector size
            int datlen = 4096;
            do
            {
                if ((fs.Position + datlen) > fs.Length)
                {
                    // if the current FileStream cursor plus the read length is going to overflow the file length,
                    // the read length must be adjusted to the perfect fit
                    datlen = (int)(fs.Length - fs.Position);
                }
                // finally read the data into the buffer
                
                // and increment the data buffer cursor
                datOffset += datlen;
            } while (fs.Position < fs.Length);
            
            fs.Close();
            fs.Dispose();
            fs = null;
            GC.Collect();

            // obtain the data text encoding through stringExtensions
            this.fileEncoding = stringExtensions.getEncoding(filepath);
            // obtain the data line termination through stringExtensions
            this.fileLineTerm = stringExtensions.getLineTerm(filepath);
            // and store the filepath since there have been no errors in the operation
            this.fileName = filepath;
        }

        /// <summary>
        /// Constructor.  Initializes the iniFileClass with its default values.
        /// </summary>
        public iniFileClass()
        {
            this.fileLineTerm = Environment.NewLine;
            this.fileEncoding = Encoding.ASCII;
        }

        /// <summary>
        /// Destructor.  Cleans up memory objects as the iniFileClass instance is de-initialized.
        /// </summary>
        ~iniFileClass()
        {
            this.iniLines = null;
            this.fileEncoding = null;
            this.fileLineTerm = null;
            this.fileName = null;
            GC.Collect();
        }

    }
}
