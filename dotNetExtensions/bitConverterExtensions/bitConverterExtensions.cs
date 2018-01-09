using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetExtensions
{
    /// <summary>
    /// Provides functions that really should have been provided by Microsoft from the beginning, but never were.
    /// </summary>
    public static partial class bitConverterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(UInt16 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
                        
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(Int16 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(UInt32 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(Int32 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(UInt64 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        /// 
        /// </param>
        /// <param name="bigEndian">
        /// 
        /// </param>
        /// <returns></returns>
        public static byte[] getBytes(Int64 value, bool bigEndian)
        {
            byte[] r = BitConverter.GetBytes(value);

            if (!bigEndian && !System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Little Endian (by signalling bigEndian = false)
                // and also
                // The platform is not Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }
            else if (bigEndian && System.BitConverter.IsLittleEndian)
            {
                //
                // If the caller specified Big Endian (by signalling bigEndian = true)
                // and also
                // The platform is Little Endian
                // Then the data output by BitConverter must be reversed
                //
                Array.Reverse(r);
            }

            return r;
        }
    }
}
