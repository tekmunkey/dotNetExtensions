using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetExtensions
{
    public static partial class bitwise
    {
        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 16-bit signed integer.
        /// </returns>
        public static Int16 ToInt16(byte[] value, int startIndex)
        {
            Int16 r = 0;

            byte[] newValue = new byte[2];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToInt16(newValue, 0);
            
            return r;
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 16-bit unsigned integer.
        /// </returns>
        public static UInt16 ToUInt16(byte[] value, int startIndex)
        {
            UInt16 r = 0;

            byte[] newValue = new byte[2];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToUInt16(newValue, 0);

            return r;
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer.
        /// </returns>
        public static Int32 ToInt32(byte[] value, int startIndex)
        {
            Int32 r = 0;

            byte[] newValue = new byte[4];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToInt32(newValue, 0);

            return r;
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 32-bit unsigned integer.
        /// </returns>
        public static UInt32 ToUInt32(byte[] value, int startIndex)
        {
            UInt32 r = 0;

            byte[] newValue = new byte[4];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToUInt32(newValue, 0);

            return r;
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 64-bit signed integer.
        /// </returns>
        public static Int64 ToInt64(byte[] value, int startIndex)
        {
            Int64 r = 0;

            byte[] newValue = new byte[8];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToInt64(newValue, 0);

            return r;
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.  
        /// 
        /// ALWAYS CONVERTS THE VALUE FROM NETWORK BYTE ORDER (BIG-ENDIAN).  DO NOT PASS DATA THROUGH THIS FUNCTION UNLESS YOU --KNOW-- 
        /// IT WAS CONSTRUCTED USING bitwise.getBytes() OR UNLESS YOU --KNOW-- IT IS IN BIG-ENDIAN FORMAT FOR SOME OTHER 
        /// REASON.
        /// </summary>
        /// <param name="value">
        /// An array of bytes.
        /// </param>
        /// <param name="startIndex">
        /// The starting position within value.
        /// </param>
        /// <returns>
        /// A 64-bit unsigned integer.
        /// </returns>
        public static UInt64 ToUInt64(byte[] value, int startIndex)
        {
            UInt64 r = 0;

            byte[] newValue = new byte[8];
            value.CopyTo(newValue, startIndex);
            if (BitConverter.IsLittleEndian) { Array.Reverse(newValue); }

            BitConverter.ToUInt64(newValue, 0);

            return r;
        }
    }
}
