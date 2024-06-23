using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace KycBizWebApi.Repository.Cities
{
    public class CitiesRespository : ICitiesRespository
    {
        private readonly KycbizContext _context;

        public CitiesRespository(KycbizContext context)
        {
            _context = context;
        }
        public void DeleteCity(long empID)
        {
            var stateList = _context.Cities.Find(empID);
            if (stateList != null)
            {
                _context.Cities.Remove(stateList);
            }
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.ToList();
        }

        public City GetCityByID(long empID)
        {
            var getCities = _context.Cities.Find(empID);
            if (getCities != null)
            {
                return getCities;
            }
            else
            {
                return null;
            }
        }

        public City GetCityByName(string cityName)
        {
            var getData = _context.Cities.FirstOrDefault(City => City.CityName == cityName);
            if (getData != null)
            { return getData; }
            else { return null; }
        }

        public void InsertCity(DtoCities cityData)
        {
            City cntry = new()
            {   CityName = cityData.CityName.ToString().Trim(),
                StateId = cityData.StateId
            };
            _context.Cities.Add(cntry);
            _context.SaveChanges();
        }

        public void UpdateCity(DtoCities cityData)
        {
            _context.Entry(cityData).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
