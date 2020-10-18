using System;
using Structures;
using Structures.Common;

namespace StructureTestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var value = 5;
            var testinItem1 = new DataItem<int>(value);
            ++value;
            var testinItem2 = new DataItem<int>(value);
            var testinItem3 = new DataItem<int>(testinItem1);
            
            Console.WriteLine($"TI1: {testinItem1.Data}\tTI2: {testinItem2.Data}\tTI3: {testinItem3.Data}\tValue: {value}");
        }
    }
}