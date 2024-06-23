using KycBizWebApi.Dto;
using DataAccessLayer.Models;

namespace KycBizWebApi.Repository.KycDoc
{
    public interface IKycDocRepository
    {
        void CreateKycBiz(DtoBizKycDoc cdata);
        IEnumerable<BusinessInfo> GetBusinessInfo(long userID);
    }
}
