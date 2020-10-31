using System;
using System.Collections.Generic;
using System.Linq;
using GeoLocApi.Data;
using GeoLocApi.Models;
using GeoLocApi.Models.Requests;
using GeoLocApi.Models.Responses;
using GeoLocApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocApi.Controllers
{
    public class PlotController : Controller
    {
        private readonly GeoLocatorStorage _dataContext;

        public PlotController(GeoLocatorStorage dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("plots")]
        public IActionResult GetAll([FromQuery] PaginationFilter filter)
        {
            // var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var plots = _dataContext.GetPlots();
            var pagedData = plots
                .OrderBy(plot => plot.Number)
                .Skip((filter.PageNumber - 1)* filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(
                new PagedResponse<List<PlotModel>>(pagedData, filter.PageNumber, filter.PageSize)
                {
                    TotalRecords = plots.Count,
                    TotalPages = (int)Math.Ceiling((double)plots.Count / filter.PageSize)
                });
        }
        
        [HttpGet("plots/{fromLat}/{fromLon}/{toLat}/{toLon}")]
        public IActionResult GetRange([FromRoute]double fromLat, [FromRoute]double fromLon, [FromRoute]double toLat, [FromRoute]double toLon)
        {
            return Ok(
                _dataContext.GetPlotsInRange(fromLat, fromLon, toLat, toLon)
                    .OrderBy(plot => plot.Number));
        }

        [HttpGet("plots/{lat}/{lon}")]
        public IActionResult Get([FromRoute]double lat, [FromRoute]double lon, [FromQuery] PaginationFilter filter)
        {
            var plots = _dataContext.GetPlotAt(lat, lon);
            var pagedData = plots
                .OrderBy(plot => plot.Number)
                .Skip((filter.PageNumber - 1)* filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(
                new PagedResponse<List<PlotModel>>(pagedData, filter.PageNumber, filter.PageSize)
                {
                    TotalRecords = plots.Count,
                    TotalPages = (int)Math.Ceiling((double)plots.Count / filter.PageSize)
                });
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
            if (!_dataContext.AddPlot(plot)) return BadRequest("Something went wrong, contact developers.");
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/properties/" + plot.Id.ToString();

            var response = new PlotResponse() { Id = plot.Id };
            return Created(locationUri, response);
        }
        
        [HttpDelete("plots/{id}/{lat}/{lon}")]
        public IActionResult Delete([FromRoute] Guid id, [FromRoute] double lat, [FromRoute] double lon)
        {
            if (_dataContext.RemovePlot(id, lat, lon))
            {
                return NoContent();
            }

            return NotFound();
        }
        
        [HttpPut("plots")]
        public IActionResult Update([FromBody] UpdatePlotRequest plotRequest)
        {
            var newPlot = new PlotModel()
            {
                Description = plotRequest.Plot.Description,
                Gps = plotRequest.Plot.Gps,
                Number = plotRequest.Plot.Number
            };
            
            if (_dataContext.ModifyPlot(plotRequest.Id, plotRequest.Latitude, plotRequest.Longitude, newPlot))
            {
                return Ok(newPlot);
            }

            return NotFound();
        }
    }
}