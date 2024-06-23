using KycBizWebApi.Dto;
using DataAccessLayer.Models;

namespace KycBizWebApi.Repository.Cities
{
    public interface ICitiesRespository
    {
        IEnumerable<City> GetCities();
        City GetCityByID(long empID);
        void InsertCity(DtoCities city);
        void UpdateCity(DtoCities city);
        void DeleteCity(long empID);
        City GetCityByName(string cityName);
    }
}
