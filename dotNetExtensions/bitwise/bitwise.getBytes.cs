using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetExtensions
{
    public static partial class bitwise
    {
        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// An Int16 (signed 16-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(Int16 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }

            return r;
        }

        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// A UInt16 (unsigned 16-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(UInt16 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }
            
            return r;
        }

        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// An Int32 (signed 32-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(Int32 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }

            return r;
        }

        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// A UInt32 (unsigned 32-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(UInt32 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }

            return r;
        }

        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// An Int64 (signed 64-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(Int64 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }

            return r;
        }

        /// <summary>
        /// Returns the specified value as an array of bytes.  
        /// 
        /// ALWAYS RETURNS THE VALUE IN NETWORK BYTE ORDER (BIG-ENDIAN).  YOU MAY USE THIS FUNCTION ANY TIME YOU LIKE, BUT ONCE YOU PASS 
        /// DATA THROUGH THIS FUNCTION YOU MUST --ALWAYS-- PASS IT BACK THROUGH bitwise.toInt() TO GET IT BACK INTO ANOTHER 
        /// FORMAT, OR ELSE IT WON'T LOOK LIKE WHAT YOU EXPECT AT ALL.
        /// </summary>
        /// <param name="value">
        /// A UInt64 (unsigned 64-bit Integer).
        /// </param>
        /// <returns>
        /// A byte array.
        /// </returns>
        public static byte[] getBytes(UInt64 value)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (System.BitConverter.IsLittleEndian) { Array.Reverse(r); }

            return r;
        }
    }
}
