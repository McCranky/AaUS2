namespace Structures.Common
{
    /// <summary>
    /// Object that stores data
    /// </summary>
    /// <typeparam name="T">Type of data to store</typeparam>
    public class ValueItem<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data to store</param>
        public ValueItem(T data)
        {
            Data = data;
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other">Object to copy from</param>
        public ValueItem(ValueItem<T> other)
        {
            Data = other.Data;
        }
    }
}