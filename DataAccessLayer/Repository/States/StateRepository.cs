using KycBizWebApi.Dto;
using DataAccessLayer.Models;

namespace KycBizWebApi.Repository.States
{
    public class StateRepository : IStateRepository
    {
        private readonly KycbizContext _context;

        public StateRepository(KycbizContext context)
        {
            _context = context;
        }

        public void DeleteState(long stateID)
        {
            var stateList = _context.States.Find(stateID);
            if (stateList != null)
            {
                _context.States.Remove(stateList);
            }

        }

        public IEnumerable<State> GetState()
        {
            return _context.States.ToList();
        }

        public State GetStateByID(long empID)
        {
            var getStates = _context.States.Find(empID);
            if (getStates != null)
            {
                return getStates;
            }
            else
            {
                return null;
            }

        }

        public State GetStateByName(string stateName)
        {
            var getState = _context.States.FirstOrDefault(State => State.StateName == stateName);
            if (getState != null)
            { return getState; }
            else { return null; }

        }
        public void InsertState(DtoState cdata)
        {
            State cntry = new()
            {   StateName = cdata.StateName.ToString().Trim(),
                CountryId = cdata.CountryId
            };
            _context.States.Add(cntry);
            _context.SaveChanges();
        }

        public void UpdateState(DtoState sdata)
        {
            _context.Entry(sdata).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
