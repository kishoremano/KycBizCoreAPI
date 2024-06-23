using KycBizWebApi.Dto;
using DataAccessLayer.Models;

namespace KycBizWebApi.Repository.States
{
    public interface IStateRepository
    {
        IEnumerable<State> GetState();
        State GetStateByID(long empID);
        void InsertState(DtoState state);
        void UpdateState(DtoState state);
        void DeleteState(long empID);
        State GetStateByName(string countryName);
    }
}
