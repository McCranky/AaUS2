using System;
using System.Collections.Generic;
using System.Linq;
using GeoLocApi.Data;
using GeoLocApi.Data.Components;
using GeoLocApi.Models;
using GeoLocApi.Models.Requests;
using GeoLocApi.Models.Responses;
using GeoLocApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocApi.Controllers
{
    public class PropertyController : Controller
    {
        private readonly GeoLocatorStorage _dataContext;

        public PropertyController(GeoLocatorStorage dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("properties")]
        public IActionResult GetAll([FromQuery] PaginationFilter filter)
        {
            var properties = _dataContext.GetProperties();
            var pagedData = properties
                //.OrderBy(prop => prop.RegisterNumber)
                .Skip((filter.PageNumber - 1)* filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(
                new PagedResponse<List<PropertyModel>>(pagedData, filter.PageNumber, filter.PageSize)
                {
                    TotalRecords = properties.Count,
                    TotalPages = (int)Math.Ceiling((double)properties.Count / filter.PageSize)
                });
        }
        
        [HttpGet("properties/{fromLat}/{fromLon}/{toLat}/{toLon}")]
        public IActionResult GetRange([FromRoute]double fromLat, [FromRoute]double fromLon, [FromRoute]double toLat, [FromRoute]double toLon)
        {
            return Ok(
                _dataContext.GetPropertiesInRange(fromLat, fromLon, toLat, toLon)
                    //.OrderBy(plot => plot.RegisterNumber)
                );
        }
        
        [HttpGet("properties/{lat}/{lon}")]
        public IActionResult GetAt([FromRoute]double lat, [FromRoute]double lon, [FromQuery] PaginationFilter filter)
        {
            var properties = _dataContext.GetPropertyAt(lat, lon);
            var pagedData = properties
                //.OrderBy(prop => prop.RegisterNumber)
                .Skip((filter.PageNumber - 1)* filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(
                new PagedResponse<List<PropertyModel>>(pagedData, filter.PageNumber, filter.PageSize)
                {
                    TotalRecords = properties.Count,
                    TotalPages = (int)Math.Ceiling((double)properties.Count / filter.PageSize)
                });
        }

        [HttpPost("properties")]
        public IActionResult Create([FromBody] CreatePropertyRequest propertyRequest)
        {
            var property = new PropertyModel()
            {
                Description = propertyRequest.Description,
                Gps = propertyRequest.Gps,
                RegisterNumber = propertyRequest.RegisterNumber
            };

            if (!_dataContext.AddProperty(property)) return BadRequest("Something went wrong, contact developers.");
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/properties/" + property.Id.ToString();

            var response = new CreatePropertyResponse() { Id = property.Id };
            return Created(locationUri, response);

        }

        [HttpDelete("properties/{id}/{lat}/{lon}")]
        public IActionResult Delete([FromRoute] Guid id, [FromRoute] double lat, [FromRoute] double lon)
        {
            if (_dataContext.RemoveProperty(id, lat, lon))
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPut("properties")]
        public IActionResult Update([FromBody] UpdatePropertyRequest propertyRequest)
        {
            var newProp = new PropertyModel()
            {
                Description = propertyRequest.Property.Description,
                Gps = propertyRequest.Property.Gps,
                RegisterNumber = propertyRequest.Property.RegisterNumber
            };
            if (_dataContext.ModifyProperty(propertyRequest.Id, propertyRequest.Latitude, propertyRequest.Longitude, newProp))
            {
                return Ok(newProp);
            }

            return NotFound();
        }
    }
}