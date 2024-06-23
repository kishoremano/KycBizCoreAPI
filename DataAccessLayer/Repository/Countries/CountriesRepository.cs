using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace KycBizWebApi.Repository.Countries
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly KycbizContext _context;

        public CountriesRepository(KycbizContext context)
        {
            _context = context;
        }

        public void DeleteCountry(long countryID)
        {
            var country = _context.Countries.Find(countryID);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }
           
        }

        public IEnumerable<Country> GetCountries()
        {
            return _context.Countries.ToList();
        }

        public Country GetCountryByID(long empID)
        {
            var getcountry = _context.Countries.Find(empID);
            if (getcountry != null)
            {
                return getcountry;
            }
            else 
            {             
                return null; 
            }

        }

        public Country GetCountryByName(string countryName)
        {
            var getCounty = _context.Countries.FirstOrDefault(Country => Country.CountryName == countryName);
            if (getCounty != null)
            {  return getCounty; }
            else { return null; }
           
        }
        public void InsertCountry(DtoCountry cdata)
        {
            Country cntry = new()
            {
                CountryName = cdata.CountryName.ToString()
            };
            _context.Countries.Add(cntry);
            _context.SaveChanges();
        }

       

        public void UpdateCountry(DtoCountry country)
        {
            _context.Entry(country).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }

        public IEnumerable<State> GetStateInfo(long countryID)
        {
            var getStateInfo = _context.States
                .Where(p => p.CountryId == countryID)
                .ToList();



            if (getStateInfo != null)
            { return getStateInfo; }
            else { return null; }
        }

        public IEnumerable<City> GetCityInfo(long stateID)
        {
            var getStateInfo = _context.Cities
                .Where(p => p.StateId == stateID)
                .ToList();

            if (getStateInfo != null)
            { return getStateInfo; }
            else { return null; }
        }
    }
}
