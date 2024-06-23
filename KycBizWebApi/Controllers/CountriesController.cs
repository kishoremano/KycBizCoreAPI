using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using KycBizWebApi.Repository.Countries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

namespace KycBizWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    
    public class CountriesController: ControllerBase
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountriesRepository countriesRepository , ILogger<CountriesController> logger)
        {
            _countriesRepository = countriesRepository;
            _logger = logger;
        }

    [HttpGet]
    [Route("GetCountry")]

    public ActionResult<DtoApiResponse> GetCountries()
    {
        DtoApiResponse obj = new DtoApiResponse();
        try
        {
            var country = _countriesRepository.GetCountries().ToList();

            if (country.Count == 0)
            {
                obj.StateCode = "204";
                obj.Message = "No data";
                obj.Data = new object();
                return obj;
            }

            obj.StateCode = "200";
            obj.Message = "";
            obj.Data = country;
            return obj;
        }
        catch (Exception ex)
        {

           _logger.LogError(ex, "An error occurred in MyAction method.");

            obj.StateCode = "400";
            obj.Message = ex.Message;
            obj.Data = new object();
            return obj;
        }

    }

    [HttpGet]
    [Route("GetCountryByID")]

    public ActionResult<DtoApiResponse> GetCountryByID(long empID)
    {
        DtoApiResponse obj = new DtoApiResponse();
        try
        {
            var country = _countriesRepository.GetCountryByID(empID);

            if (country == null)
            {
                obj.StateCode = "204";
                obj.Message = "No data";
                obj.Data = new object();
                return obj;
            }

            obj.StateCode = "200";
            obj.Message = "";
            obj.Data = country;
            return obj;
        }
        catch (Exception ex)
        {
            obj.StateCode = "400";
            obj.Message = ex.Message;
            obj.Data = new object();
            return obj;
        }

    }

    [HttpPost]
    [Route("AddUpdateCountry")]

    public ActionResult<DtoApiResponse> AddCountry([FromBody] DtoCountry country)
    {
        DtoApiResponse obj = new DtoApiResponse();
        try
        {
            if (country.CountryID == 0)
            {
                var chkAlreadyExist = _countriesRepository.GetCountryByName(country.CountryName);

                if (chkAlreadyExist == null)
                {
                    _countriesRepository.InsertCountry(country);

                    var countryList = _countriesRepository.GetCountries();

                    obj.StateCode = "200";
                    obj.Message = "Data Added Successfully";
                    obj.Data = countryList;
                    return obj;
                }
                else
                {
                    obj.StateCode = "400";
                    obj.Message = "Data Already Exists";
                    obj.Data = new object();
                    return obj;
                }

            }
            else
            {
                _countriesRepository.UpdateCountry(country);

                var countryUpdateList = _countriesRepository.GetCountries();

                obj.StateCode = "200";
                obj.Message = "Data Updated Successfully";
                obj.Data = countryUpdateList;
                return obj;
            }


        }
        catch (Exception ex)
        {
            obj.StateCode = "400";
            obj.Message = ex.Message;
            obj.Data = new object();
            return obj;
        }
    }

    [HttpDelete]
    [Route("DeleteCountryByID")]

    public ActionResult<DtoApiResponse> DeleteCountryByID(long cunryID)
    {
        DtoApiResponse obj = new DtoApiResponse();
        try
        {
            _countriesRepository.DeleteCountry(cunryID);

            var country = _countriesRepository.GetCountries();
            if (country == null)
            {
                obj.StateCode = "204";
                obj.Message = "No data";
                obj.Data = new object();
                return obj;
            }

            obj.StateCode = "200";
            obj.Message = "";
            obj.Data = country;
            return obj;
        }
        catch (Exception ex)
        {
            obj.StateCode = "400";
            obj.Message = ex.Message;
            obj.Data = new object();
            return obj;
        }

    }

    [HttpGet]
    [Route("GetState")]
    public ActionResult<DtoApiResponse> GetStateByCountryID(long countryID)
    {
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                var stateInfo = _countriesRepository.GetStateInfo(countryID);

                if (stateInfo == null)
                {
                    obj.StateCode = "204";
                    obj.Message = "No data";
                    obj.Data = new object();
                    return obj;
                }

                obj.StateCode = "200";
                obj.Message = "";
                obj.Data = stateInfo;
                return obj;
            }
            catch (Exception ex)
            {
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }

        }

        [HttpGet]
        [Route("GetCities")]
        public ActionResult<DtoApiResponse> GetCitiesByStateID(long stateID)
        {
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                var cityInfo = _countriesRepository.GetCityInfo(stateID);

                if (cityInfo == null)
                {
                    obj.StateCode = "204";
                    obj.Message = "No data";
                    obj.Data = new object();
                    return obj;
                }

                obj.StateCode = "200";
                obj.Message = "";
                obj.Data = cityInfo;
                return obj;
            }
            catch (Exception ex)
            {
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }

        }
    }
}
