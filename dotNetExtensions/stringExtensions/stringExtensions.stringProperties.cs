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
        public static Encoding getEncoding(byte[] fromTextData)
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
        /// A string value representing the line termination found.  Returns the default system lineterm if none is found.
        /// </returns>
        public static string getLineTerm(byte[] fromTextData)
        {
            string r = null;

            MemoryStream ms = new MemoryStream(fromTextData);
            StreamReader sr = new StreamReader(ms);
            // sr.Peek() is required before CurrentEncoding can be known
            sr.Peek();

            string oneLine = sr.ReadLine();

            if ((oneLine.Length >= 2) && (oneLine[oneLine.Length - 1] == 0x0a) && (oneLine[oneLine.Length - 2] == 0x0d))
            {
                // if the last character on the line is LF and the char before that is CR, then this is a network/windows lineterm
                r = new string(new char[] { (char)0x0d, (char)0x0a });
            }
            else if ((oneLine.Length >= 1) && (oneLine[oneLine.Length - 1] == 0x0a))
            {
                // if the last character on the line is LF and the char before that is not CR, then this is a Linux lineterm
                r = new string((char)0x0a, 1);
            }
            else if ((oneLine.Length >= 1) && (oneLine[oneLine.Length - 1] == (char)0x0d))
            {
                // If the last character on the line is CR, then this is a Mac lineterm
                r = new string((char)0x0d, 1);
            }
            else
            {
                // If no in-file newline is defined, default to system newline
                r = System.Environment.NewLine;
            }

            oneLine = null;

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
            // sr.Peek() is required before CurrentEncoding can be known
            sr.Peek();

            string oneLine = sr.ReadLine();

            if ((oneLine.Length >= 2) && (oneLine[oneLine.Length - 1] == 0x0a) && (oneLine[oneLine.Length - 2] == 0x0d))
            {
                // if the last character on the line is LF and the char before that is CR, then this is a network/windows lineterm
                r = new string(new char[] { (char)0x0d, (char)0x0a });
            }
            else if ((oneLine.Length >= 1) && (oneLine[oneLine.Length - 1] == 0x0a))
            {
                // if the last character on the line is LF and the char before that is not CR, then this is a Linux lineterm
                r = new string((char)0x0a, 1);
            }
            else if ((oneLine.Length >= 1) && (oneLine[oneLine.Length - 1] == (char)0x0d))
            {
                // If the last character on the line is CR, then this is a Mac lineterm
                r = new string((char)0x0d, 1);
            }
            else
            {
                // If no in-file newline is defined, default to system newline
                r = System.Environment.NewLine;
            }

            oneLine = null;

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
