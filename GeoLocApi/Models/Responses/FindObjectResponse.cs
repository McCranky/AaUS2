using System.Collections.Generic;
using GeoLocApi.Data.Components;

namespace GeoLocApi.Models.Responses
{
    /// <summary>
    /// Combines to search result (Properties and Plots) into one model
    /// </summary>
    public class FindObjectResponse
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public List<string> RelationToObject { get; set; }
        public int Number { get; set; }
        public GPS Gps { get; set; }
    }
}