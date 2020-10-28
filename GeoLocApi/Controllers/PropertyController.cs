using System.Linq;
using GeoLocApi.Data;
using GeoLocApi.Data.Components;
using GeoLocApi.Models;
using GeoLocApi.Models.Requests;
using GeoLocApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocApi.Controllers
{
    public class PropertyController : Controller
    {
        private GeoLocatorStorage _dataContext;

        public PropertyController(GeoLocatorStorage dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("properties")]
        public IActionResult Get()
        {
            return Ok(
                _dataContext.GetProperties()
                    .OrderBy(prop => prop.RegisterNumber));
        }
        
        [HttpGet("properties/{fromLat}/{fromLon}/{toLat}/{toLon}")]
        public IActionResult GetRange([FromRoute]double fromLat, [FromRoute]double fromLon, [FromRoute]double toLat, [FromRoute]double toLon)
        {
            return Ok(
                _dataContext.GetPropertiesInRange(fromLat, fromLon, toLat, toLon)
                    .OrderBy(plot => plot.RegisterNumber));
        }
        
        [HttpGet("properties/{lat}/{lon}")]
        public IActionResult GetAt([FromRoute]double lat, [FromRoute]double lon)
        {
            return Ok(_dataContext.GetPropertyAt(lat, lon));
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
            
            if (_dataContext.AddProperty(property))
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                var locationUri = baseUrl + "/properties/" + property.Id.ToString();

                var response = new PropertyResponse() { Id = property.Id };
                return Created(locationUri, response);
            }

            return BadRequest("There are no plot to assign to.");
        }
    }
}