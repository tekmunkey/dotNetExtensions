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

        /// <summary>
        /// Scans a specified text file and returns the number of detected whole lines in that text file.  Line terminations are 
        /// determined by an initializing call to getLineTerm().
        /// </summary>
        /// <param name="fromTextFile">
        /// The file path to scan for a line count.
        /// </param>
        /// <returns>
        /// An integer value.  The number of whole text lines detected in the file.  Typically 1 more than the number of line terminations (if 
        /// there's any text following the last line termination detected).  Otherwise will be the same as the number of line terminations.
        /// </returns>
        public static long countLines(string fromTextFile)
        {
            long r = 0;

            Encoding encoding = getEncoding(fromTextFile);
            string lineTerm = getLineTerm(fromTextFile);
            //
            // Depending on character encoding, not every character is guaranteed to be the same data width.  ie:  UTF-8 characters may be 
            // more than 8-bits (1 byte), UTF-16 characters may be more than 16-bits (2 bytes), etc.  The only guarantee we have is that 
            // line terminators will appear if they exist.
            //
            byte[] ltData = encoding.GetBytes(lineTerm);
            int readLen = ltData.Length;

            FileStream fs = new FileStream(fromTextFile, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write);
            fs.Position = 0;

            //
            // If fs.Length is not greater than 0, then there are 0 lines in the empty file so skip to cleanup and return
            //
            if (fs.Length > 0)
            {
                //
                // File is greater length than 0 so go ahead and increment lines to 1 automatically (there is at least 1 line in the file 
                // whether there are any line terminators or not)
                //
                r = 1;
                //
                // Test additionally if the file is long enough to contain line terminators
                //
                if (fs.Length > readLen)
                {
                    do
                    {
                        if ((fs.Position + readLen) > fs.Length)
                        {
                            // If at any point fs.Position + readLen would overrun fs.Length, any additional read operation would error and fail 
                            // (an application-fatal exception would occur)
                            readLen = (int)(fs.Length - fs.Position);
                        }

                        byte[] readBuf = new byte[readLen];

                        

                        fs.Read(readBuf, 0, readLen);
                        if (readBuf.Length == ltData.Length)
                        {

                        }

                        readBuf = null;
                    } while (fs.Position < fs.Length);
                }

                
            }

            fs.Close();
            fs.Dispose();
            fs = null;

            ltData = null;
            lineTerm = null;
            encoding = null;

            GC.Collect();

            return r;
        }
    }
}
