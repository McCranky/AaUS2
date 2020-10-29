using System;
using System.Collections.Generic;
using System.Linq;
using GeoLocApi.Data.Components;
using GeoLocApi.Models;
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

        public List<PlotModel> GetPlots()
        {
            return _plotTree
                .Select(plotNode => new PlotModel()
                {
                    Id = plotNode.PrimaryKey,
                    Description = plotNode.Data.Description,
                    Number = plotNode.Data.Number,
                    Gps = plotNode.Data.Gps,
                    Properties = plotNode.Data.Properties
                })
                .ToList();
        }

        public List<PlotModel> GetPlotsInRange(double fromLat, double fromLon, double toLat, double toLon)
        {
            var pointFrom = new []{fromLat, fromLon};
            var pointTo = new []{toLat, toLon};
            return _plotTree.FindInRange(pointFrom, pointTo)
                .Select(plotNode => new PlotModel()
                {
                    Id = plotNode.PrimaryKey,
                    Description = plotNode.Data.Description,
                    Number = plotNode.Data.Number,
                    Gps = plotNode.Data.Gps,
                    Properties = plotNode.Data.Properties
                })
                .ToList();
        }

        public List<PlotModel> GetPlotAt(double latitude, double longtitude)
        {
            var pos = new []{latitude, longtitude};
            var nodes = _plotTree.FindInRange(pos, pos);
            return nodes
                .Select(plotNode => new PlotModel()
                {
                    Id = plotNode.PrimaryKey,
                    Description = plotNode.Data.Description,
                    Number = plotNode.Data.Number,
                    Gps = plotNode.Data.Gps,
                    Properties = plotNode.Data.Properties
                })
                .ToList();
        }
        
        public List<PropertyModel> GetProperties()
        {
            return _propertyTree
                .Select(propertyNode => new PropertyModel()
                {
                    Id = propertyNode.PrimaryKey,
                    Description = propertyNode.Data.Description,
                    RegisterNumber = propertyNode.Data.RegisterNumber,
                    Gps = propertyNode.Data.Gps,
                    Plots = propertyNode.Data.Plots
                })
                .ToList();
        }
        
        public List<PropertyModel> GetPropertiesInRange(double fromLat, double fromLon, double toLat, double toLon)
        {
            var pointFrom = new []{fromLat, fromLon};
            var pointTo = new []{toLat, toLon};
            return _propertyTree.FindInRange(pointFrom, pointTo)
                .Select(propertyNode => new PropertyModel()
                {
                    Id = propertyNode.PrimaryKey,
                    Description = propertyNode.Data.Description,
                    RegisterNumber = propertyNode.Data.RegisterNumber,
                    Gps = propertyNode.Data.Gps,
                    Plots = propertyNode.Data.Plots
                })
                .ToList();
        }
        
        public List<PropertyModel> GetPropertyAt(double latitude, double longtitude)
        {
            var pos = new []{latitude, longtitude};
            var nodes = _propertyTree.FindInRange(pos, pos);
            return nodes
                .Select(propertyNode => new PropertyModel()
                {
                    Id = propertyNode.PrimaryKey,
                    Description = propertyNode.Data.Description,
                    RegisterNumber = propertyNode.Data.RegisterNumber,
                    Gps = propertyNode.Data.Gps,
                    Plots = propertyNode.Data.Plots
                })
                .ToList();
        }

        public bool AddProperty(PropertyModel propertyModel)
        {
            var keys = new[] {propertyModel.Gps.Latitude, propertyModel.Gps.Longtitude};
            if (_plotTree.TryFindKdtNodes(keys, out var plotNodes))
            {
                var plots = plotNodes.Select(node => node.Data).ToList();
                var property = new Property(
                    propertyModel.RegisterNumber, 
                    propertyModel.Description, 
                    propertyModel.Gps,
                    plots);
                propertyModel.Id = _propertyTree.Add(keys, property);
                foreach (var plot in plots)
                {
                    plot.AddProperty(property);
                }
                return true;
            }
            return false;
        }

        public bool AddPlot(PlotModel plotModel)
        {
            var keys = new[] {plotModel.Gps.Latitude, plotModel.Gps.Longtitude};
            var properties = _propertyTree.FindInRange(keys, keys).Select(node => node.Data).ToList();
            var plot = new Plot(plotModel.Number, plotModel.Description, plotModel.Gps, properties);
            plotModel.Id = _plotTree.Add(keys, plot);
            foreach (var prop in properties)
            {
                prop.AddPlot(plot);
            }
            return true;
        }

        public bool ModifyPlot(PlotModel oldPlot, PlotModel newPlot)
        {
            var keys = new[] {oldPlot.Gps.Latitude, oldPlot.Gps.Longtitude};
            if (_plotTree.TryFindKdtNode(keys, oldPlot.Id, out var plotNode))
            {
                var plot = plotNode.Data;
                if (plot.Gps != newPlot.Gps) // zmenila sa gps
                {
                    // musime odobrať pozemok všetkym nehnutelnostiam ku ktorym je viazany
                    if (_propertyTree.TryFindKdtNodes(keys, out var propNodes))
                    {
                        // var props = propNodes.Select(prop => prop.Data);
                        var propsToRemove = new List<KDTNode<double, Property>>();
                        foreach (var propNode in propNodes)
                        {
                            propNode.Data.RemovePlot(plot);
                            // ak nehnutelnosti neostal pozemok, na ktorom by stala, tak ju musime zmazať
                            if (propNode.Data.Plots.Count <= 0)
                            {
                                propsToRemove.Add(propNode);
                            }

                            foreach (var propToRemove in propsToRemove)
                            {
                                _propertyTree.Remove(propToRemove.Keys, propToRemove.PrimaryKey);
                            }
                        }
                    }
                    // teraz zmažeme samotny plot zo stromu
                    _plotTree.Remove(keys, oldPlot.Id);
                    // vytvoríme ho na novej pozicii
                    keys = new[] {newPlot.Gps.Latitude, newPlot.Gps.Longtitude};
                    newPlot.Id = _plotTree.Add(keys, plot);
                    plot.Gps = newPlot.Gps;
                    // priradime všetkym nehnutelnostiam, ak take su
                    if (_propertyTree.TryFindKdtNodes(keys, out propNodes))
                    {
                        var props = propNodes.Select(prop => prop.Data).ToList();
                        foreach (var prop in props)
                        {
                            prop.AddPlot(plot);
                            plot.AddProperty(prop);
                        }

                        newPlot.Properties = props.Select(prop => prop.Description).ToList();
                    }
                }
                plot.Description = newPlot.Description;
                plot.Number = newPlot.Number;
                return true;
            }
            return false;
        }
        
        public bool ModifyProperty(PropertyModel oldProp, PropertyModel newProp)  
        {
            var oldKeys = new[] {oldProp.Gps.Latitude, oldProp.Gps.Longtitude};
            if (_propertyTree.TryFindKdtNode(oldKeys, oldProp.Id, out var propNode))
            {
                var prop = propNode.Data;
                if (prop.Gps != newProp.Gps) // zmenila sa gps
                {
                    var newKeys = new[] {newProp.Gps.Latitude, newProp.Gps.Longtitude};
                    if (_plotTree.TryFindKdtNodes(newKeys, out var newPlotNodes)) // kontrola či máme nehnutelnosť kam umiestniť
                    {
                        // musime odobrať nehnutelnosť všetkym pozemkom ku ktorym je viazany
                        if (_plotTree.TryFindKdtNodes(oldKeys, out var plotNodes))
                        {
                            var plots = plotNodes.Select(plot => plot.Data);
                            foreach (var plot in plots)
                            {
                                plot.RemoveProperty(prop);
                            }
                        }
                        // teraz zmažeme samotnu nehnutelnost zo stromu
                        _propertyTree.Remove(oldKeys, oldProp.Id);
                        // vytvoríme ju na novej pozicii
                        newProp.Id = _propertyTree.Add(newKeys, prop);
                        prop.Gps = newProp.Gps;
                        // priradime všetkym pozemkom
                        var plotsToAssign = newPlotNodes.Select(plot => plot.Data).ToList();
                        foreach (var plot in plotsToAssign)
                        {
                            plot.AddProperty(prop);
                            prop.AddPlot(plot);
                        }

                        newProp.Plots = plotsToAssign.Select(plot => plot.Description).ToList();
                    }
                    else
                    {
                        // pre novu poziciu nehnutelnosťi neexistuje pozemok
                        return false;
                    }
                }
                prop.Description = newProp.Description;
                prop.RegisterNumber = newProp.RegisterNumber;
                return true;
            }
            return false;
        }

        public bool RemovePlot(Guid id, double latitude, double longtitude)
        {
            var keys = new[] {latitude, longtitude};
            if (_plotTree.TryFindKdtNode(keys, id, out var plot))
            {
                if (_propertyTree.TryFindKdtNodes(keys, out var propNodes))
                {
                    var propsToRemove = new List<KDTNode<double, Property>>();
                    foreach (var propNode in propNodes)
                    {
                        propNode.Data.RemovePlot(plot.Data);
                        if (propNode.Data.Plots.Count <= 0)
                        {
                            propsToRemove.Add(propNode);
                        }
                    }

                    foreach (var prop in propsToRemove)
                    {
                        _propertyTree.Remove(prop.Keys, prop.PrimaryKey);
                    }
                }
                _plotTree.Remove(keys, plot.PrimaryKey);
                return true;
            }

            return false;
        }
        
        public bool RemoveProperty(Guid id, double latitude, double longtitude)
        {
            var keys = new[] {latitude, longtitude};
            if (_propertyTree.TryFindKdtNode(keys, id, out var property))
            {
                if (_plotTree.TryFindKdtNodes(keys, out var plotNodes))
                {
                    var plots = plotNodes.Select(plot => plot.Data);
                    foreach (var plot in plots)
                    {
                        plot.RemoveProperty(property.Data);
                    }
                }
                _propertyTree.Remove(keys, property.PrimaryKey);
                return true;
            }

            return false;
        }
    }
}