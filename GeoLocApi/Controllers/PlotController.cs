using System.Linq;
using System.Threading.Tasks;
using GeoLocApi.Data;
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
                    .Where(plot => plot.HasProperties)
                    .OrderBy(plot => plot.Number));
        }

        [HttpGet("plots/{lat}/{lon}")]
        public IActionResult Get([FromRoute]int lat, [FromRoute]int lon)
        {
            return Ok(_dataContext.GetPlotAt(lat, lon));
        }
    }
}