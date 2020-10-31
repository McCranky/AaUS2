using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoLocApi.Data.Components;
using GeoLocApi.Models;
using Structures.Trees.KDTree;

namespace GeoLocApi.Data
{
    public class GeoLocatorStorage
    {
        public const string PlotsFilePath = "plots.csv";
        public const string PropertiesFilePath = "properties.csv";
        private KDTree<double, Plot> PlotTree { get; set; }
        private KDTree<double, Property> PropertyTree { get; set; }
        public int PlotsCount => PlotTree.Count;
        public int PropertiesCount => PropertyTree.Count;
        
        public GeoLocatorStorage()
        {
            PlotTree = new KDTree<double, Plot>(2);
            PropertyTree = new KDTree<double, Property>(2);
        }

        private void SeedData()
        {
            var rnd = new Random();
            var positions = new List<double[]>();
            for (var i = 0; i < 500; i++)
            {
                var pos = new double[]{rnd.Next() % 50, rnd.Next() % 50};
                positions.Add(pos);
                
                var gps = new GPS('N', pos[0], 'W', pos[1]);
                var plot = new Plot(i, $"Desc of plot {i}", gps);
                PlotTree.Add(pos, plot);
            }

            for (var i = 0; i < 250; i++)
            {
                var pos = positions[rnd.Next(positions.Count - 1)];

                if (!PlotTree.TryFindKdtNodes(pos, out var plotNodes)) continue;
                // vytvorenie pozemku
                var plots = plotNodes.Select(node => node.Data).ToList();
                var gps = plots[0].Gps;
                var property = new Property(i, $"Desc of property {i}", gps, plots);
                // priradenie pozemku všetkym parcelám na ktorých stojí
                foreach (var plot in plots)
                {
                    plot.AddProperty(property);
                }
                PropertyTree.Add(new []{gps.Latitude, gps.Longitude}, property);
            }
        }

        public bool SaveData()
        {
            try
            {
                // plots
                // format -> number;description;latitudeSymbol:latitude:longitudeSymbol:longitude
                var dataToWrite = PlotTree
                    .Select(plotNode => $"{plotNode.Data.Number};{plotNode.Data.Description};{plotNode.Data.Gps.LatitudeSymbol}:{plotNode.Data.Gps.Latitude}:{plotNode.Data.Gps.LongitudeSymbol}:{plotNode.Data.Gps.Longitude}");
                File.WriteAllLines(PlotsFilePath, dataToWrite, Encoding.UTF8);
                // properties
                // format -> registerNumber;description;latitudeSymbol:latitude:longitudeSymbol:longitude
                dataToWrite = PropertyTree
                    .Select(plotNode => $"{plotNode.Data.RegisterNumber};{plotNode.Data.Description};{plotNode.Data.Gps.LatitudeSymbol}:{plotNode.Data.Gps.Latitude}:{plotNode.Data.Gps.LongitudeSymbol}:{plotNode.Data.Gps.Longitude}");
                File.WriteAllLines(PropertiesFilePath, dataToWrite, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool LoadData()
        {
            try
            {
                // plots
                // format -> number;description;latitudeSymbol:latitude:longitudeSymbol:longitude// properties
                var records = File.ReadAllLines(PlotsFilePath);
                foreach (var record in records)
                {
                    var plotAttributes = record.Split(";");
                    var gpsAttributes = plotAttributes[2].Split(':');
                    var plot = new Plot(
                        int.Parse(plotAttributes[0]), 
                        plotAttributes[1], 
                        new GPS()
                        {
                            LatitudeSymbol = gpsAttributes[0][0],
                            Latitude = int.Parse(gpsAttributes[1]),
                            LongitudeSymbol = gpsAttributes[2][0],
                            Longitude = int.Parse(gpsAttributes[3]),
                        });
                    PlotTree.Add(new []{plot.Gps.Latitude, plot.Gps.Longitude}, plot);
                }
                // properties
                // format -> registerNumber;description;latitudeSymbol:latitude:longitudeSymbol:longitude
                records = File.ReadAllLines(PropertiesFilePath);
                foreach (var record in records)
                {
                    var propertyAttributes = record.Split(';');
                    var gpsAttributes = propertyAttributes[2].Split(':');
                    var property = new Property(
                        int.Parse(propertyAttributes[0]), 
                        propertyAttributes[1], 
                        new GPS()
                        {
                            LatitudeSymbol = gpsAttributes[0][0],
                            Latitude = int.Parse(gpsAttributes[1]),
                            LongitudeSymbol = gpsAttributes[2][0],
                            Longitude = int.Parse(gpsAttributes[3]),
                        });
                    var keys = new[] {property.Gps.Latitude, property.Gps.Longitude};
                    PropertyTree.Add(keys, property);
                    if (!PlotTree.TryFindKdtNodes(keys, out var plotNodes)) continue;
                    // naplnenie referencii
                    foreach (var plotNode in plotNodes)
                    {
                        plotNode.Data.AddProperty(property);
                        property.AddPlot(plotNode.Data);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<PlotModel> GetPlots()
        {
            return PlotTree
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

        public IEnumerable<PlotModel> GetPlotsInRange(double fromLat, double fromLon, double toLat, double toLon)
        {
            var pointFrom = new []{fromLat, fromLon};
            var pointTo = new []{toLat, toLon};
            return PlotTree.FindInRange(pointFrom, pointTo)
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
            var nodes = PlotTree.FindInRange(pos, pos);
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
            return PropertyTree
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
        
        public IEnumerable<PropertyModel> GetPropertiesInRange(double fromLat, double fromLon, double toLat, double toLon)
        {
            var pointFrom = new []{fromLat, fromLon};
            var pointTo = new []{toLat, toLon};
            return PropertyTree.FindInRange(pointFrom, pointTo)
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
            var nodes = PropertyTree.FindInRange(pos, pos);
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
            var keys = new[] {propertyModel.Gps.Latitude, propertyModel.Gps.Longitude};
            var plots = PlotTree.FindInRange(keys, keys).Select(node => node.Data).ToList();
            
            var property = new Property(
                propertyModel.RegisterNumber, 
                propertyModel.Description, 
                propertyModel.Gps,
                plots);
            propertyModel.Id = PropertyTree.Add(keys, property);

            foreach (var plot in plots)
            {
                plot.AddProperty(property);
            }

            return true;
        }

        public bool AddPlot(PlotModel plotModel)
        {
            var keys = new[] {plotModel.Gps.Latitude, plotModel.Gps.Longitude};
            var properties = PropertyTree.FindInRange(keys, keys).Select(node => node.Data).ToList();
            
            var plot = new Plot(plotModel.Number, plotModel.Description, plotModel.Gps, properties);
            plotModel.Id = PlotTree.Add(keys, plot);
            
            foreach (var prop in properties)
            {
                prop.AddPlot(plot);
            }
            
            return true;
        }

        public bool ModifyPlot(Guid id, double latitude, double longttude, PlotModel newPlot)
        {
            var keys = new[] {latitude, longttude};
            if (!PlotTree.TryFindKdtNode(keys, id, out var plotNode)) return false;
            var plot = plotNode.Data;
            if (plot.Gps != newPlot.Gps) // zmenila sa gps
            {
                // musime odobrať pozemok všetkym nehnutelnostiam ku ktorym je viazany
                if (PropertyTree.TryFindKdtNodes(keys, out var propNodes))
                {
                    // var props = propNodes.Select(prop => prop.Data);
                    var propsToRemove = new List<KDTNode<double, Property>>();
                    foreach (var propNode in propNodes)
                    {
                        propNode.Data.RemovePlot(plot);
                    }
                }
                // teraz zmažeme samotny plot zo stromu
                PlotTree.Remove(keys, id);
                // vytvoríme ho na novej pozicii
                keys = new[] {newPlot.Gps.Latitude, newPlot.Gps.Longitude};
                newPlot.Id = PlotTree.Add(keys, plot);
                plot.Gps = newPlot.Gps;
                // priradime všetkym nehnutelnostiam, ak take su
                if (PropertyTree.TryFindKdtNodes(keys, out propNodes))
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
        
        public bool ModifyProperty(Guid id, double latitude, double longtitude, PropertyModel newProp)  
        {
            var oldKeys = new[] {latitude, longtitude};
            if (!PropertyTree.TryFindKdtNode(oldKeys,id, out var propNode)) return false;
            var prop = propNode.Data;
            if (prop.Gps != newProp.Gps) // zmenila sa gps
            {
                // musime odobrať nehnutelnosť všetkym pozemkom ku ktorym je viazany
                if (PlotTree.TryFindKdtNodes(oldKeys, out var plotNodes))
                {
                    var plots = plotNodes.Select(plot => plot.Data);
                    foreach (var plot in plots)
                    {
                        plot.RemoveProperty(prop);
                    }
                }
                // teraz zmažeme samotnu nehnutelnost zo stromu
                PropertyTree.Remove(oldKeys, id);
                // vytvoríme ju na novej pozicii
                var newKeys = new[] {newProp.Gps.Latitude, newProp.Gps.Longitude};
                newProp.Id = PropertyTree.Add(newKeys, prop);
                prop.Gps = newProp.Gps;
                    
                if (PlotTree.TryFindKdtNodes(newKeys, out var newPlotNodes)) // najdenie novych pozemkov
                {
                    // priradime všetkym pozemkom
                    var plotsToAssign = newPlotNodes.Select(plot => plot.Data).ToList();
                    foreach (var plot in plotsToAssign)
                    {
                        plot.AddProperty(prop);
                        prop.AddPlot(plot);
                    }

                    newProp.Plots = plotsToAssign.Select(plot => plot.Description).ToList();
                }
            }
            prop.Description = newProp.Description;
            prop.RegisterNumber = newProp.RegisterNumber;
            return true;
        }

        public bool RemovePlot(Guid id, double latitude, double longtitude)
        {
            var keys = new[] {latitude, longtitude};
            if (!PlotTree.TryFindKdtNode(keys, id, out var plot)) return false;
            if (PropertyTree.TryFindKdtNodes(keys, out var propNodes))
            {
                foreach (var propNode in propNodes)
                {
                    propNode.Data.RemovePlot(plot.Data);
                }
            }
            PlotTree.Remove(keys, plot.PrimaryKey);
            return true;
        }
        
        public bool RemoveProperty(Guid id, double latitude, double longtitude)
        {
            var keys = new[] {latitude, longtitude};
            if (!PropertyTree.TryFindKdtNode(keys, id, out var property)) return false;
            if (PlotTree.TryFindKdtNodes(keys, out var plotNodes))
            {
                var plots = plotNodes.Select(plot => plot.Data);
                foreach (var plot in plots)
                {
                    plot.RemoveProperty(property.Data);
                }
            }
            PropertyTree.Remove(keys, property.PrimaryKey);
            return true;
        }
    }
}