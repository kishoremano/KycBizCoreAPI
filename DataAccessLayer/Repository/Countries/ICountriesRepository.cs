using KycBizWebApi.Dto;
using DataAccessLayer.Models;

namespace KycBizWebApi.Repository.Countries
{
    public interface ICountriesRepository
    {
        IEnumerable<Country> GetCountries();
        Country GetCountryByID(long empID);
        void InsertCountry(DtoCountry country);
        void UpdateCountry(DtoCountry country);
        void DeleteCountry(long empID);
        Country GetCountryByName(string countryName);
        IEnumerable<State> GetStateInfo(long countryID);
        IEnumerable<City> GetCityInfo(long stateID);
    }
}
