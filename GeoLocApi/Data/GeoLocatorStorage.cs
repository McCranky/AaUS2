using System;
using System.Collections.Generic;
using System.Linq;
using GeoLocApi.Data.Components;
using Structures.Trees.KDTree;

namespace GeoLocApi.Data
{
    public class GeoLocatorStorage
    {
        private KDTree<double, Plot> _plotTree { get; set; }
        private KDTree<double, Property> _propertyTree { get; set; }
        
        public GeoLocatorStorage()
        {
            _plotTree = new KDTree<double, Plot>(2);
            _propertyTree = new KDTree<double, Property>(2);
            SeedData();
        }

        private void SeedData()
        {
            var rnd = new Random();
            var positions = new List<double[]>();
            for (var i = 0; i < 500; i++)
            {
                var pos = new double[]{rnd.Next() % 50, rnd.Next() % 50};
                positions.Add(pos);
                
                var gps = new GPS((CardinalDirections)rnd.Next(0,1), pos[0], (CardinalDirections)rnd.Next(2,3), pos[1]);
                var plot = new Plot(i, $"Desc of plot {i}", gps);
                _plotTree.Add(pos, plot);
            }

            for (int i = 0; i < 250; i++)
            {
                var pos = positions[rnd.Next(positions.Count - 1)];

                if (_plotTree.TryFindKdtNodes(pos, out var plotNodes))
                {
                    // vytvorenie pozemku
                    var plots = plotNodes.Select(node => node.Data).ToList();
                    var gps = plots[0].Gps;
                    var property = new Property(i, $"Desc of property {i}", gps, plots);
                    // priradenie pozemku všetkym parcelám na ktorých stojí
                    foreach (var plot in plots)
                    {
                        plot.AddProperty(property);
                    }
                    _propertyTree.Add(new []{gps.Latitude, gps.Longtitude}, property);
                }
            }
        }

        public List<Plot> GetPlots()
        {
            return _plotTree.Select(plotNode => plotNode.Data).ToList();
        }

        public List<Property> GetProperties()
        {
            return _propertyTree.Select(propertyNode => propertyNode.Data).ToList();
        }

        public List<Plot> GetPlotAt(int latitude, int longtitude)
        {
            var pos = new double[]{latitude, longtitude};
            var nodes = _plotTree.FindInRange(pos, pos);
            return nodes.Select(node => node.Data).ToList();
        }
    }
}