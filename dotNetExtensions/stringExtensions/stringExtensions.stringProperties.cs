using System;
using System.IO;
using System.Text;

namespace dotNetExtensions
{
    public static partial class stringExtensions
    {
        /// <summary>
        /// Retrieves text encoding from the specified byte array.
        /// </summary>
        /// <param name="fromTextData">
        /// The byte array to retrieve text encoding from.
        /// </param>
        /// <returns>
        /// The text encoding discovered in the specified byte array.
        /// </returns>
        public static Encoding getEncoding(ref byte[] fromTextData)
        {
            Encoding r = Encoding.Default;

            MemoryStream ms = new MemoryStream(fromTextData);
            StreamReader sr = new StreamReader(ms);
            // sr.Peek() is required before CurrentEncoding can be known
            sr.Peek();
            
            r = sr.CurrentEncoding;

            sr.Close();
            sr.Dispose();
            sr = null;

            ms.Close();
            ms.Dispose();
            ms = null;
            GC.Collect();

            return r;
        }
        
        /// <summary>
        /// Retrieves text encoding from the specified text file.
        /// </summary>
        /// <param name="fromTextFile">
        /// The file path to retrieve text encoding from.
        /// </param>
        /// <returns>
        /// The text encoding discovered in the specified file.
        /// </returns>
        public static Encoding getEncoding(string fromTextFile)
        {
            Encoding r = Encoding.Default;

            FileStream fs = new FileStream(fromTextFile, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
            StreamReader sr = new StreamReader(fs);
            // sr.Peek() is required before CurrentEncoding can be known
            sr.Peek();
            
            r = sr.CurrentEncoding;

            sr.Close();
            sr.Dispose();
            sr = null;

            fs.Close();
            fs.Dispose();
            fs = null;
            GC.Collect();

            return r;
        }

        /// <summary>
        /// Scans a specified array of bytes for evidence of existing line terminations, and returns what it finds.
        /// </summary>
        /// <param name="fromTextData">
        /// A byte array.  The data to scan for line terminations.
        /// </param>
        /// <returns>
        /// A string value representing the line termination found.  Returns the default system lineterm (System.Environment.NewLine) if 
        /// none is found.
        /// </returns>
        public static string getLineTerm(ref byte[] fromTextData)
        {
            string r = System.Environment.NewLine;

            MemoryStream ms = new MemoryStream(fromTextData);
            StreamReader sr = new StreamReader(ms);

            do
            {
                char c = (char)sr.Read();
                if (c.Equals((char)0x0d) && !sr.EndOfStream)
                {
                    // if current character is CR and not end of stream
                    c = (char)sr.Read();
                    if (c.Equals((char)0x0a))
                    {
                        // and the next character is LF
                        r = new string(new char[] { (char)0x0d, (char)0x0a });
                        // we're done here
                        break;
                    }
                    else
                    {
                        // the next character is not LF
                        r = new string(new char[] { (char)0x0d });
                        // we're done here
                        break;
                    }
                }
                else if (c.Equals((char)0x0a))
                {
                    // if current character is LF
                    r = new string(new char[] { (char)0x0a });
                    // we're done here
                    break;
                }
                else
                {
                    // if current character IS CR and also end of stream
                    r = new string(new char[] { (char)0x0d });
                    // we're done here but no need to break - it'll just make Visual Studio error/warn that the ending while 
                    // line is 'unreachable code' for no good reason
                }
            } while (!sr.EndOfStream);

            sr.Close();
            sr.Dispose();
            sr = null;

            ms.Close();
            ms.Dispose();
            ms = null;

            GC.Collect();

            return r;
        }

        /// <summary>
        /// Scans a specified text file for evidence of existing line terminations, and returns what it finds.
        /// </summary>
        /// <param name="fromTextFile">
        /// The file path to scan for line terminations.
        /// </param>
        /// <returns>
        /// A string value representing the line termination found.  Returns the default system lineterm if none is found.
        /// </returns>
        public static string getLineTerm(string fromTextFile)
        {
            string r = Environment.NewLine;

            FileStream fs = new FileStream(fromTextFile, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
            StreamReader sr = new StreamReader(fs);

            do
            {
                char c = (char)sr.Read();
                if (c.Equals((char)0x0d) && !sr.EndOfStream)
                {
                    // if current character is CR and not end of stream
                    c = (char)sr.Read();
                    if (c.Equals((char)0x0a))
                    {
                        // and the next character is LF
                        r = new string(new char[] { (char)0x0d, (char)0x0a });
                        // we're done here
                        break;
                    }
                    else
                    {
                        // the next character is not LF
                        r = new string(new char[] { (char)0x0d });
                        // we're done here
                        break;
                    }
                }
                else if (c.Equals((char)0x0a))
                {
                    // if current character is LF
                    r = new string(new char[] { (char)0x0a });
                    // we're done here
                    break;
                }
                else
                {
                    // if current character IS CR and also end of stream
                    r = new string(new char[] { (char)0x0d });
                    // we're done here but no need to break - it'll just make Visual Studio error/warn that the ending while 
                    // line is 'unreachable code' for no good reason
                }
            } while (!sr.EndOfStream);

            sr.Close();
            sr.Dispose();
            sr = null;

            fs.Close();
            fs.Dispose();
            fs = null;

            GC.Collect();

            return r;
        }
    }
}
