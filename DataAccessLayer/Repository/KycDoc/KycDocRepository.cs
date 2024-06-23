using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace KycBizWebApi.Repository.KycDoc
{
    public class KycDocRepository : IKycDocRepository
    {
        private readonly KycbizContext _context;

        public KycDocRepository(KycbizContext context)
        {
            _context = context;
        }

        public void CreateKycBiz(DtoBizKycDoc cdata)
        {
            BusinessInfo cntry = new()
            {
                BusinessInfoId = cdata.bizInfo.BusinessInfoId,
                BusinessName = cdata.bizInfo.BusinessName,
                BusinessContactNumber = cdata.bizInfo.BusinessContactNumber,
                BusinessType = cdata.bizInfo.BusinessType,
                UserId = cdata.bizInfo.UserId,
                CityId = cdata.bizInfo.CityId,
                StateId = cdata.bizInfo.StateId,
                CountryId = cdata.bizInfo.CountryId,
                ZipCode = cdata.bizInfo.ZipCode,
                LocationName = cdata.bizInfo.LocationName,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                KycInfos = new List<KycInfo> { new KycInfo {
                KycInfoId = cdata.bizKycDoc.KycInfoId,
                KycDocTypeId = cdata.bizKycDoc.KycDocTypeId,
                KycNumber = cdata.bizKycDoc.KycNumber,
                UserId = cdata.bizInfo.UserId,
                KycExpireDate = cdata.bizKycDoc.KycExpireDate,
                KycDocUrl = cdata.bizKycDoc.KycDocUrl,
                IsActive = true,

                KycInfoDocs = new List<KycInfoDoc> { new KycInfoDoc
                  {
                    KycInfoDocId = cdata.bizKycInfoDoc.KycInfoDocId,
                    UserId = cdata.bizInfo.UserId,
                    DocNameDesc = cdata.bizKycInfoDoc.DocNameDesc,
                    DocContent = cdata.bizKycInfoDoc.DocContent,
                    DocFileExn = cdata.bizKycInfoDoc.DocFileExn,
                    DocSize = cdata.bizKycInfoDoc.DocSize,
                    CreatedDate = DateTime.UtcNow

                  }
                }

                } }


            };
            _context.BusinessInfos.Add(cntry);
            _context.SaveChanges();
        }

        public IEnumerable<BusinessInfo> GetBusinessInfo(long userID)
        {
            //var getBusinessInfo = _context.BusinessInfos
            //    .Include(x => x.KycInfos.Where(p => p.UserId == userID))
            //    .ThenInclude(y => y.KycInfoDocs.Where(x => x.UserId == userID))  
            //    .ToList();

            var getBusinessInfo = _context.BusinessInfos
             .Where(b => b.UserId == userID) // Filter BusinessInfos by UserId
             .Include(b => b.KycInfos.Where(k => k.UserId == userID)) // Include and filter KycInfos by UserId
             .ThenInclude(k => k.KycInfoDocs.Where(d => d.UserId == userID)) // Include and filter KycInfoDocs by UserId
             .ToList();

            if (getBusinessInfo != null)
            { return getBusinessInfo; }
            else { return null; }
        }
    }
}
