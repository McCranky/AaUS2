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
    /// <summary>
    /// Represents database which is storing data about all properties and plots in 2-dimensional trees
    /// and is performing all required functionality almost like CRUD on database
    /// </summary>
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
        /// <summary>
        /// Seeds storage witch required amount of data
        /// </summary>
        /// <param name="plotsCount">Count of plots to be inserted</param>
        /// <param name="propertiesCount">count of properties to be inserted</param>
        public void SeedData(int plotsCount, int propertiesCount)
        {
            var rnd = new Random();
            for (var i = 0; i < plotsCount; i++)
            {
                var pos = GenerateRandomKeys();                
                var gps = new GPS('N', pos[0], 'W', pos[1]);
                var plot = new Plot(i, $"Desc of plot {i}", gps);
                PlotTree.Add(pos, plot);
            }

            for (var i = 0; i < propertiesCount; i++)
            {
                var pos = GenerateRandomKeys();
                var gps = new GPS('N', pos[0], 'W', pos[1]);
                var property = new Property(i, $"Desc of property {i}", gps);

                if (PlotTree.TryFindKdtNodes(pos, out var plotNodes)) 
                {
                    foreach (var plotNode in plotNodes) {
                        plotNode.Data.AddProperty(property);
                        property.AddPlot(plotNode.Data);
                    }
                }
                
                PropertyTree.Add(pos, property);
            }
        }
        /// <summary>
        /// Saves all stored data into csv file format.
        /// The path is stored in PropertiesFilePath a PlotsFilePath attributes.
        /// </summary>
        /// <returns>True if process of saving is successful</returns>
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
        /// <summary>
        /// Loads all data from csv file format into memory.
        /// The path is to files is stored in PropertiesFilePath a PlotsFilePath attributes.
        /// </summary>
        /// <returns>True if process of loading is successful</returns>
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
                            Latitude = double.Parse(gpsAttributes[1]),
                            LongitudeSymbol = gpsAttributes[2][0],
                            Longitude = double.Parse(gpsAttributes[3]),
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
                            Latitude = double.Parse(gpsAttributes[1]),
                            LongitudeSymbol = gpsAttributes[2][0],
                            Longitude = double.Parse(gpsAttributes[3]),
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
        /// <summary>
        /// Return all plots stored
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Return all plots stored which has gps location between specified boundaries.
        /// </summary>
        /// <param name="fromLat"></param>
        /// <param name="fromLon"></param>
        /// <param name="toLat"></param>
        /// <param name="toLon"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Return all plots at given position
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public List<PlotModel> GetPlotAt(double latitude, double longitude)
        {
            var pos = new []{latitude, longitude};
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
        /// <summary>
        /// Return all properties stored
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Return all properties stored which has gps location between specified boundaries.
        /// </summary>
        /// <param name="fromLat"></param>
        /// <param name="fromLon"></param>
        /// <param name="toLat"></param>
        /// <param name="toLon"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Return all properties at given position
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public List<PropertyModel> GetPropertyAt(double latitude, double longitude)
        {
            var pos = new []{latitude, longitude};
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
        /// <summary>
        /// Adds Property from given PropertyModel into storage
        /// </summary>
        /// <param name="propertyModel"></param>
        /// <returns>Success of adding</returns>
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
        /// <summary>
        /// Adds Plot from given PlotModel into storage
        /// </summary>
        /// <param name="plotModel"></param>
        /// <returns>Success of adding process</returns>
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
        /// <summary>
        /// Modifies stored data about Plot
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="newPlot"></param>
        /// <returns></returns>
        public bool ModifyPlot(Guid id, double latitude, double longitude, PlotModel newPlot)
        {
            var keys = new[] {latitude, longitude};
            if (!PlotTree.TryFindKdtNode(keys, id, out var plotNode)) return false;
            var plot = plotNode.Data;
            var tmp = Math.Abs(plot.Gps.Latitude - newPlot.Gps.Latitude);
            var tmp2 = Math.Abs(plot.Gps.Longitude - newPlot.Gps.Longitude);
            if (Math.Abs(plot.Gps.Latitude - newPlot.Gps.Latitude) > 0.0001f
            || Math.Abs(plot.Gps.Longitude - newPlot.Gps.Longitude) > 0.0001f) // zmenila sa gps
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
        /// <summary>
        /// Modifies stored data about Property
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="newProp"></param>
        /// <returns></returns>
        public bool ModifyProperty(Guid id, double latitude, double longitude, PropertyModel newProp)  
        {
            var oldKeys = new[] {latitude, longitude};
            if (!PropertyTree.TryFindKdtNode(oldKeys,id, out var propNode)) return false;
            var prop = propNode.Data;
            if (Math.Abs(prop.Gps.Latitude - newProp.Gps.Latitude) > 0.0001f
                || Math.Abs(prop.Gps.Longitude - newProp.Gps.Longitude) > 0.0001f) // zmenila sa gps
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
        /// <summary>
        /// Removes specific Plot from storage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool RemovePlot(Guid id, double latitude, double longitude)
        {
            var keys = new[] {latitude, longitude};
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
        /// <summary>
        /// Removes specific Property from storage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool RemoveProperty(Guid id, double latitude, double longitude)
        {
            var keys = new[] {latitude, longitude};
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

        /// <summary>
        /// Generates random key array of size 2
        /// </summary>
        /// <returns></returns>
        private double[] GenerateRandomKeys()
        {
            var rnd = new Random();
            // return new double[] { Math.Round(rnd.NextDouble() * 100, 3), Math.Round(rnd.NextDouble() * 100, 3) };
            return new double[] { rnd.Next(1, 15), rnd.Next(1, 15) };
        }
    }
}