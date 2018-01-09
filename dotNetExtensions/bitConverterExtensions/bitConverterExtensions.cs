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
        /// Returns the specified 16-bit unsigned integer value as an array of bytes in the specified Endian format.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 2.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
        /// Returns the specified 16-bit signed integer value as an array of bytes.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 2.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 4.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
        /// Returns the specified 32-bit signed integer value as an array of bytes.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 4.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 8.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
        /// Returns the specified 64-bit signed integer value as an array of bytes.  
        /// 
        /// Extends BitConverter.GetBytes with the bigEndian option.
        /// </summary>
        /// <param name="value">
        /// The number to convert.
        /// </param>
        /// <param name="bigEndian">
        /// A boolean value.  If True, the data will be returned in Big Endian format regardless of system architecture.  If False, the data will be returned in Little Endian format.
        /// </param>
        /// <returns>
        /// An array of bytes with length 8.  
        /// 
        /// The order of bytes in the array returned by the BitConverter.GetBytes method depends on whether the computer architecture is little-endian or big-endian.  
        /// 
        /// The order of bytes in the array returned by bitConverterExtensions.GetBytes depends on whether bigEndian is True or False.
        /// </returns>
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
