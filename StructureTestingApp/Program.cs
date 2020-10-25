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
            if (other.Latitude > Latitude) return -1;
            return other.Latitude == Latitude ? 0 : 1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tree = new KDTree<IComparable, string>(2);
            // var testNodes = new List<KDTNode<IComparable, string>>();
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{23,35}.ToList(), "Nitra"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{20,33}.ToList(), "Sereď"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{25,36}.ToList(), "Topoľčianky"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{16,31}.ToList(), "Galanta"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{14,39}.ToList(), "Senica"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{28,34}.ToList(), "Tlmače"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{24,40}.ToList(), "Bošany"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{13,32}.ToList(), "Bratislava"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{14,41}.ToList(), "Hodonín"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{17,42}.ToList(), "Trnava"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{29,46}.ToList(), "Bojnice"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{27,43}.ToList(), "Nováky"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{26,35}.ToList(), "Moravce"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{30,33}.ToList(), "Levice"));
            // testNodes.Add(new KDTNode<IComparable, string>(new IComparable[]{17,42}.ToList(), "Hohoo"));
            
            Random rnd = new Random();
            var insertedKeys = new List<IComparable[]>();
            // insertedKeys.Add(new IComparable[] {2, 65});
            // insertedKeys.Add(new IComparable[] {3, 12});
            // insertedKeys.Add(new IComparable[] {23, 85});
            // insertedKeys.Add(new IComparable[] {24, 5});
            // insertedKeys.Add(new IComparable[] {62, 7});
            // insertedKeys.Add(new IComparable[] {12, 51});
            // insertedKeys.Add(new IComparable[] {0, 6});
            // insertedKeys.Add(new IComparable[] {74, 4});
            // insertedKeys.Add(new IComparable[] {12, 21});
            // insertedKeys.Add(new IComparable[] {51, 11});
            // for (var index = 0; index < insertedKeys.Count; index++)
            // {
            //     var k = insertedKeys[index];
            //     Console.WriteLine($"[{k[0]}, {k[1]}]");
            //     tree.Add(k, $"{index}");
            // }
            //
            // Console.WriteLine(tree.Count);
            // for (var index = 0; index < insertedKeys.Count; index++)
            // {
            //     Console.WriteLine("---Tree---");
            //     foreach (var n in tree)
            //     {
            //         Console.Write($"[{n.Keys[0]}, {n.Keys[1]}] - ");
            //     }
            //
            //     Console.WriteLine("----");
            //     var k = insertedKeys[index];
            //     Console.WriteLine($"[{k[0]}, {k[1]}]");
            //     tree.DeleteNode(k);
            //     // Console.WriteLine(tree.TryFindKDTNodes(k, out var ll));
            // }
            //
            // Console.WriteLine(tree.Count);
            for (int i = 0; i < 10; i++)
            {
                var keys = new IComparable[] {rnd.Next() % 10, rnd.Next() % 10};
                insertedKeys.Add(keys);
                tree.Add(keys, $"{i}");
            }

            foreach (var keys in insertedKeys)
            {
                tree.DeleteNode(keys);
            }
            Console.WriteLine(tree.Count);
        }
    }
}