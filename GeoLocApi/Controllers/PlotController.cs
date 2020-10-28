﻿using System.Linq;
using System.Threading.Tasks;
using GeoLocApi.Data;
using GeoLocApi.Models;
using GeoLocApi.Models.Requests;
using GeoLocApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocApi.Controllers
{
    public class PlotController : Controller
    {
        private GeoLocatorStorage _dataContext;

        public PlotController(GeoLocatorStorage dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("plots")]
        public IActionResult GetAll()
        {
            return Ok(
                _dataContext.GetPlots()
                    .OrderBy(plot => plot.Number));
        }
        
        [HttpGet("plots/{fromLat}/{fromLon}/{toLat}/{toLon}")]
        public IActionResult GetRange([FromRoute]double fromLat, [FromRoute]double fromLon, [FromRoute]double toLat, [FromRoute]double toLon)
        {
            return Ok(
                _dataContext.GetPlotsInRange(fromLat, fromLon, toLat, toLon)
                    .OrderBy(plot => plot.Number));
        }

        [HttpGet("plots/{lat}/{lon}")]
        public IActionResult Get([FromRoute]double lat, [FromRoute]double lon)
        {
            return Ok(_dataContext.GetPlotAt(lat, lon));
        }

        [HttpPost("plots")]
        public IActionResult Create([FromBody] CreatePlotRequest plotRequest)
        {
            var plot = new PlotModel()
            {
                Description = plotRequest.Description,
                Gps = plotRequest.Gps,
                Number = plotRequest.Number
            };
            if (_dataContext.AddPlot(plot))
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                var locationUri = baseUrl + "/properties/" + plot.Id.ToString();

                var response = new PlotResponse() { Id = plot.Id };
                return Created(locationUri, response);
            }
            return BadRequest("Something went wrong, contact developers.");
        }
    }
}