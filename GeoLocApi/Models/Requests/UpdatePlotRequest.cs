using System;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Requests
{
    public class ModifyPlotInfo
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public GPS Gps { get; set; }
    }
    /// <summary>
    /// Stores information from incoming body request form modifying Plot
    /// </summary>
    public class UpdatePlotRequest
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// Modified information
        /// </summary>
        public ModifyPlotInfo Plot { get; set; }
    }
}