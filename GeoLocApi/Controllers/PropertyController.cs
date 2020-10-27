using System.Linq;
using GeoLocApi.Data;
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
        public IActionResult GetProperties()
        {
            return Ok(_dataContext.GetProperties().OrderBy(prop => prop.RegisterNumber));
        }
    }
}