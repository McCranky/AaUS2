using System;
using System.Collections.Generic;
using System.Linq;
using GeoLocApi.Data;
using GeoLocApi.Models.Requests;
using GeoLocApi.Models.Responses;
using GeoLocApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocApi.Controllers 
{
    public class StorageController: Controller
    {
        
        private readonly GeoLocatorStorage _dataContext;

        public StorageController(GeoLocatorStorage dataContext)
        {
            _dataContext = dataContext;
        }
        
        [HttpGet("storageInfo")]
        public IActionResult GetInfo()
        {
            return Ok(new StorageInfoRequest()
            {
                PlotsCount = _dataContext.PlotsCount,
                PropertiesCount = _dataContext.PropertiesCount
            });
        }
        
        [HttpGet("load")]
        public IActionResult Load()
        {
            if (_dataContext.LoadData())
            {
                return Ok(new StorageInfoRequest()
                {
                    PlotsCount = _dataContext.PlotsCount,
                    PropertiesCount = _dataContext.PropertiesCount
                });
            }

            return BadRequest("Something went wrong while loading data from file.");
        }
        
        [HttpGet("save")]
        public IActionResult Save()
        {
            if (_dataContext.SaveData())
            {
                return Ok(new StorageInfoRequest()
                {
                    PlotsCount = _dataContext.PlotsCount,
                    PropertiesCount = _dataContext.PropertiesCount
                });
            }
            return BadRequest("Something went wrong while saving data to file.");
        }

        [HttpGet("find/{lat1}/{lon1}/{lat2}/{lon2}")]
        public IActionResult FindObjects([FromRoute]double lat1, [FromRoute]double lon1, [FromRoute]double lat2, [FromRoute]double lon2,[FromQuery] PaginationFilter filter)
        {
            var plotModels = _dataContext.GetPlotsInRange(lat1, lon1, lat2, lon2);
            var response = plotModels.Select(pm => new FindObjectResponse()
            {
                Type = "Plot",
                Description = pm.Description,
                Number = pm.Number,
                Gps = pm.Gps,
                RelationToObject = pm.Properties
            }).ToList();
            var propertyModels = _dataContext.GetPropertiesInRange(lat1, lon1, lat2, lon2);
            response.AddRange(propertyModels.Select(pm => new FindObjectResponse()
            {
                Type = "Property",
                Description = pm.Description,
                Number = pm.RegisterNumber,
                Gps = pm.Gps,
                RelationToObject = pm.Plots
            }));
            
            var pagedResponse = response.Skip((filter.PageNumber - 1)* filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return Ok(
                new PagedResponse<List<FindObjectResponse>>(pagedResponse, filter.PageNumber, filter.PageSize)
                {
                    TotalRecords = response.Count,
                    TotalPages = (int)Math.Ceiling((double)response.Count / filter.PageSize)
                });
        }
    }
}