using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Structures;
using Structures.Common;
using Structures.Trees.KDTree;

namespace StructureTestingApp
{
    public class GPSPoint : IComparable<GPSPoint>
    {
        public enum CardinalDirections
        {
            Nort = 'N',
            South = 'S',
            East = 'E',
            West = 'W'
        }

        public CardinalDirections Latitude { get; set; }
        public CardinalDirections Longtitude { get; set; } 
        public double LatitudePos { get; set; }
        public double LongtitudePos { get; set; }
        
        public GPSPoint(CardinalDirections latitude, double latPos, CardinalDirections longtitude, double longPos)
        {
            Latitude = latitude;
            Longtitude = longtitude;
            LatitudePos = latPos;
            LongtitudePos = longPos;
        }

        public int CompareTo(GPSPoint other)
        {
            if (other.Latitude < Latitude) return -1;
            return other.Latitude == Latitude ? 0 : 1;
        }
    }

    class Program
    {
         class MyClass         {
            public int Type { get; set; }
            MyClass()
            {
                Type = 2;
            }
        }
        static void Main(string[] args)
        {
            var gps1 = new GPSPoint(GPSPoint.CardinalDirections.East, 22.3, GPSPoint.CardinalDirections.South, 54.3);
            var tree = new KDTree<IComparable, string>(2);
            tree.Add(new IComparable[]{23,35}, "Nitra");
            tree.Add(new IComparable[]{20,33}, "Sereď");
            tree.Add(new IComparable[]{25,36}, "Topoľčianky");
            tree.Add(new IComparable[]{16,31}, "Galanta");
            tree.Add(new IComparable[]{14,39}, "Senica");
            tree.Add(new IComparable[]{28,34}, "Tlmače");
            tree.Add(new IComparable[]{24,40}, "Bošany");
            tree.Add(new IComparable[]{13,32}, "Bratislava");
            tree.Add(new IComparable[]{14,41}, "Hodonín");
            tree.Add(new IComparable[]{17,42}, "Trnava");
            tree.Add(new IComparable[]{29,46}, "Bojnice");
            tree.Add(new IComparable[]{27,43}, "Nováky");
            tree.Add(new IComparable[]{26,35}, "Moravce");
            tree.Add(new IComparable[]{30,33}, "Levice");
            tree.Add(new IComparable[]{17,42}, "Hohoo");


            var col1 = new IComparable[] {17, 42};
            var col2 = new IComparable[] {17, 42};
            Console.WriteLine(col1.SequenceEqual(col2));
            
            if (tree.TryFindKDTNodes(new IComparable[]{17,42}, out var nody))
            {
                foreach (var noda in nody)
                {
                    Console.WriteLine(noda.Data);
                }
            }
            
            // foreach (var node in tree)
            // {
            //     Console.WriteLine(node.Data);
            // }
        }
    }
}