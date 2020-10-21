using System;
using System.Collections;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {

            var gps1 = new GPSPoint(GPSPoint.CardinalDirections.East, 22.3, GPSPoint.CardinalDirections.South, 54.3);
            var tree = new KDTree<List<GPSPoint>, int>();
            tree.Add(new []{gps1}, 69);
            
            Console.WriteLine($"Wow");
        }
    }
}