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
        /// An object variable reference to append to the end of the array.
        /// </param>
        public static void push<T>(ref T[] array, T obj)
        {
            if (obj != null)
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
    }
}
