using System;

namespace dotNetExtensions
{
    /// <summary>
    /// Contains extension and helper methods for strings.
    /// </summary>
    public static partial class stringExtensions
    {
        /// <summary>
        /// Performs a full copy on the specified inputString and returns it as a char array.
        /// </summary>
        /// <param name="inputString">
        /// The string to copy.
        /// </param>
        /// <returns>
        /// A char array copied from the inputString.
        /// </returns>
        public static char[] copyStringToCharArray(string inputString)
        {
            char[] r = new char[inputString.Length];
            inputString.CopyTo(0, r, 0, r.Length);

            return r;
        }

        /// <summary>
        /// Performs a full copy on the specified inputString and returns it as a string instance.
        /// </summary>
        /// <param name="inputString">
        /// The string to copy.
        /// </param>
        /// <returns>
        /// A string copied from the inputString.
        /// </returns>
        public static string copyStringToString(string inputString)
        {
            char[] c = new char[inputString.Length];
            inputString.CopyTo(0, c, 0, c.Length);
            string r = new string(c);

            return r;
        }
    }
}
