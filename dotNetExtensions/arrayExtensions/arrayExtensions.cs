using System;

namespace dotNetExtensions
{
    /// <summary>
    /// Contains extension and helper methods for arrays.
    /// </summary>
    public static class arrayExtensions
    {
        /// <summary>
        /// Allows adding an element to the end of an array, with at-runtime dynamic array resizing.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array and the object that will be inserted into it.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <param name="obj">
        /// An object variable instance to append to the end of the array.
        /// </param>
        public static void push<T>(ref T[] array, T obj)
        {
            if (array != null)
            {
                Array.Resize<T>(ref array, array.Length + 1);
            }
            else
            {
                array = new T[1];
            }
            array[array.Length - 1] = obj;
        }

        /// <summary>
        /// Allows removing and returning an element from the end of an array, with at-runtime dynamic array resizing.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array and the object that will be inserted into it.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <returns>
        /// The item retrieved from the last index position in the array, if one existed.  If there was no instance object to retrieve, 
        /// then the default value for T (the type) will be returned.  This may be null for some types, 0, false, etc.  You need to know 
        /// your Types and their default values to know what return value to expect if there's nothing to pop.
        /// </returns>
        public static T pop<T>(ref T[] array)
        {
            T r = default(T);
            if ((array != null) && (array.Length > 0))
            {
                r = array[array.Length - 1];
                //
                // The last index in the array is always array.Length - 1
                //
                // When re/declaring an array's upper boundary, ALWAYS declare its Length (how many elements it can hold) 
                // and NEVER declare its INDEX
                //
                // So as long as there's something to retrieve we should always be safe to simply resize down by 1, until 
                // reaching 0
                //
                Array.Resize<T>(ref array, array.Length - 1);
                
            }
            return r;
        }

        /// <summary>
        /// Allows inserting an element into a specified index into any array, with at-runtime dynamic array resizing.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <param name="index">
        /// An integer.  The index into the specified array where an item should be removed.
        /// </param>
        /// <param name="obj">
        /// An object variable instance to insert into the array at the specified index.
        /// </param>
        public static void insertAt<T>(ref T[] array, int index, T obj)
        {
            if (object.ReferenceEquals(array, null))
            {
                // ensuring that the array is in fact an instance
                array = new T[0];
            }
            //
            // The newSize value will help determine the size of the array, based on the index specifier.  In this 
            // fashion a programmer may insert an item far past the end of an array's upper boundary, to be filled 
            // in later.
            //
            int newSize = 0;
            if (index <= array.Length)
            {
                //
                // if the specified index is less than or equal to the specified array length, we must expand the 
                // array by just 1 item 
                // 
                newSize = array.Length + 1;
            }
            else if (index > array.Length)
            {
                //
                // if the specified index is greater than the specified array length, we must expand the array by 
                // enough items to fit the new one
                //
                newSize = index + 1;
            }
            // Although these are explicit if statements, there would never be any other/alternate case
            //
            // Finally perform the array resize operation
            //
            Array.Resize<T>(ref array, newSize);
            //
            // Testing if the specified index is less than the --old-- array size, not caring about 0-base indexing
            //
            if (index < (array.Length - 1))
            {
                //
                // Index was less than the old array length, so we need to forward-fix the old array members, meaning 
                // copy everything from the specified index value one element/index forward into the array
                //   * In a normal array iteration we would go as long as index was < array.Length, but in this case 
                //     we want to skip the last item, which we know is empty anyway, because that last item would throw 
                //     an IndexOutOfBoundsException when we attempted to copy it +1 index forward (past the upper array 
                //     boundary).
                //
                for (int i = index; i < (array.Length - 1); i++)
                {
                    array[i + 1] = array[i];
                }
            }
            //
            // There is no need for an else or else if here.
            //   * If the specified index is equal to or greater than the old array length, then the index we will be 
            //     setting into is already empty
            //
            array[index] = obj;
        }

        /// <summary>
        /// Allows removing an element from a specified index into any array, with at-runtime dynamic array resizing.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <param name="index">
        /// An integer.  The index into the specified array where an item should be removed.
        /// </param>
        public static void removeAt<T>(ref T[] array, int index)
        {
            if ((array != null) && (index < array.Length))
            {
                //
                // the specified array is not null and the index fits inside its bounds, so we're good to go with modification
                //
                if ((index + 1) < array.Length)
                {
                    // there are elements following index, so copy them backwards into place
                    for (int i = index + 1; i < array.Length; i++)
                    {
                        array[i - 1] = array[i];
                    }
                }
                // No need for an else - if there are no elements to copy in, simply dump the last element off the end of the array
                Array.Resize<T>(ref array, array.Length - 1);
            }
        }

        /// <summary>
        /// Allows testing the array to determine if it contains a specified object.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <param name="obj">
        /// An object variable instance to test into the array for element existence and position.
        /// </param>
        /// <param name="referenceEqualityOnly">
        /// A boolean value.  If True, then indexOf will test only for reference equality.  If False, then indexOf will test for either reference or value equality.
        /// </param>
        /// <returns>
        /// An integer value indicating the specified object's index into the specified array.  -1 if the object does not exist in the array.
        /// </returns>
        public static int indexOf<T>(ref T[] array, T obj, bool referenceEqualityOnly)
        {
            int r = -1;

            if ((array != null) && (obj != null))
            {
                if (!referenceEqualityOnly)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].Equals(obj) || object.ReferenceEquals(array[i], obj))
                        {
                            r = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (object.ReferenceEquals(array[i], obj))
                        {
                            r = i;
                            break;
                        }
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Allows testing the array to determine if it contains a specified object.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array">
        /// An array variable reference.
        /// </param>
        /// <param name="obj">
        /// An object variable instance to test into the array for element existence.
        /// </param>
        /// <param name="referenceEqualityOnly">
        /// A boolean value.  If True, then contains will test only for reference equality.  If False, then contains will test for either reference or value equality.
        /// </param>
        /// <returns>
        /// A boolean value indicating whether the specified object exists in the array.  True if the object exists in the array.  Otherwise False.
        /// </returns>
        public static bool contains<T>(ref T[] array, T obj, bool referenceEqualityOnly)
        {
            return (indexOf<T>(ref array, obj, referenceEqualityOnly) > -1);
        }

        /// <summary>
        /// Copies a range of elements from an Array starting at the specified source index and pastes them to another Array starting at the specified destination index.  
        /// 
        /// Guarantees that all changes are undone if the copy does not succeed completely.  
        /// 
        /// Ensures that the copy is performed in a Big Endian fashion, such that the copied items will be reversed only if the system is Little Endian.
        /// </summary>
        /// <param name="sourceArray">
        /// The Array that contains the data to copy
        /// </param>
        /// <param name="sourceIndex">
        /// A 32-bit integer that represents the index in the sourceArray at which copying begins.
        /// </param>
        /// <param name="destinationArray">
        /// The Array that receives the data.
        /// </param>
        /// <param name="destinationIndex">
        /// A 32-bit integer that represents the index in the destinationArray at which storing begins.
        /// </param>
        /// <param name="length">
        /// A 32-bit integer that represents the number of elements to copy.
        /// </param>
        public static void constrainedCopyBE(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        {
            Array.ConstrainedCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
            if (System.BitConverter.IsLittleEndian)
            {
                Array.Reverse(destinationArray, destinationIndex, length);
            }
        }

        /// <summary>
        /// Copies a range of elements from an Array starting at the specified source index and pastes them to another Array starting at the specified destination index.  
        /// 
        /// Guarantees that all changes are undone if the copy does not succeed completely.  
        /// 
        /// Ensures that the copy is performed in a Little Endian fashion, such that the copied items will be reversed only if the system is Big Endian.
        /// </summary>
        /// <param name="sourceArray">
        /// The Array that contains the data to copy
        /// </param>
        /// <param name="sourceIndex">
        /// A 32-bit integer that represents the index in the sourceArray at which copying begins.
        /// </param>
        /// <param name="destinationArray">
        /// The Array that receives the data.
        /// </param>
        /// <param name="destinationIndex">
        /// A 32-bit integer that represents the index in the destinationArray at which storing begins.
        /// </param>
        /// <param name="length">
        /// A 32-bit integer that represents the number of elements to copy.
        /// </param>
        public static void constrainedCopyLE(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        {
            Array.ConstrainedCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
            if (!System.BitConverter.IsLittleEndian)
            {
                Array.Reverse(destinationArray, destinationIndex, length);
            }
        }

        /// <summary>
        /// Determines whether two arrays are equal by comparing the elements using the default equality comparer for the element type.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array0">
        /// An array to compare against another.
        /// </param>
        /// <param name="array1">
        /// An array to compare against another.
        /// </param>
        /// <returns>
        /// A boolean value.  True if both arrays are of the same length and each element in both arrays is equal according to the default equality comparer for the element type.  
        /// 
        /// If both array parameters are to the same reference, including if both arrays are null, the function returns True.  
        /// 
        /// Otherwise, if either array is null, no exception is thrown but the function simply returns False.
        /// </returns>
        public static bool compare<T>(ref T[] array0, ref T[] array1)
        {
            bool r = object.ReferenceEquals(array0, array1);

            if (!r && !object.ReferenceEquals(array0, null) && !object.ReferenceEquals(array1, null))
            {
                if (array0.Length == array1.Length)
                {
                    // set return value to true by default - this will change only if inequality is caught
                    r = true;
                    for (int i = 0; i < array0.Length; i++)
                    {
                        if (!array0[i].Equals(array1[i]))
                        {
                            r = false;
                            break;
                        }
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Determines whether a specified range of elements in two arrays are equal by comparing the elements in the range using the default equality comparer for the element type.
        /// </summary>
        /// <typeparam name="T">
        /// A Type.  Requires you to specify the object type of the array.
        /// </typeparam>
        /// <param name="array0">
        /// An array to compare against another.
        /// </param>
        /// <param name="startOffset0">
        /// An integer.  The index into array0 where comparison should begin.
        /// </param>
        /// <param name="array1">
        /// An array to compare against another.
        /// </param>
        /// <param name="startOffset1">
        /// An integer.  The index into array1 where comparison should begin.
        /// </param>
        /// <param name="length">
        /// An integer.  The number of elements, in both arrays, to compare.
        /// </param>
        /// <returns>
        /// A boolean value.  True if both arrays are of a length that can contain the specified startOffsets and length, and each element in both arrays is equal according to the default equality comparer for the element type.  Otherwise False.  
        /// 
        /// If both array parameters are to the same reference, including if both arrays are null, the function returns True.  
        /// 
        /// Otherwise, if either array is null, no exception is thrown but the function simply returns False.  
        /// 
        /// No exceptions are thrown if either startOffset is outside the bounds of its respective array, or if length would overrun the bounds of either array.  The function simply returns False.
        /// </returns>
        public static bool compare<T>(ref T[] array0, int startOffset0, ref T[] array1, int startOffset1, int length)
        {
            bool r = object.ReferenceEquals(array0, array1);

            if (!r && !object.ReferenceEquals(array0, null) && !object.ReferenceEquals(array1, null))
            {
                if ((array0.Length >= (startOffset0 + length)) && (array1.Length >= (startOffset1 + length)))
                {
                    // set return value to true by default - this will change only if inequality is caught
                    r = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (!array0[startOffset0 + i].Equals(array1[startOffset1 + i]))
                        {
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
