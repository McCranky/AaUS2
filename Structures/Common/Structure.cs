using System;

namespace Structures.Common
{
    /// <summary>
    /// Prvok uchvávajúci dáta
    /// </summary>
    /// <typeparam name="T">Typ dát uchovávaných v prvku</typeparam>
    /// <remarks>Používa sa ako predok pre vnútorné prvky štruktúr (napr. zoznam, strom, ...)</remarks>
    public class ValueItem<T>
    {
        /// <summary>
        /// Dáta
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Data, ktoré uchováva</param>
        public ValueItem(T data)
        {
            Data = data;
        }
        /// <summary>
        /// Kopírovací konštruktor
        /// </summary>
        /// <param name="other">Prvok uchovávajúci dáta, z ktorého sa prevezmú vlastnosti</param>
        public ValueItem(ValueItem<T> other)
        {
            Data = other.Data;
        }
    }

    public abstract class KeyItem<T> where T : IComparable
    {

    }

    public class Structure
    {
        public virtual bool IsEmpty() => false;
        public virtual int Size() => 0;
    }
}