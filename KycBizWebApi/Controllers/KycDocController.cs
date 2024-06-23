using KycBizWebApi.Dto;
using KycBizWebApi.Helper;
using KycBizWebApi.Repository.KycDoc;
using KycBizWebApi.Repository.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace KycBizWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KycDocController : ControllerBase
    {
        private readonly IKycDocRepository _bizKycRep;
        private readonly HelperClass _HelperClass;
        private readonly ILogger<UserController> _logger;


        public KycDocController(IKycDocRepository bizKycRep, HelperClass HelperClass, ILogger<UserController> logger)
        {
            _bizKycRep = bizKycRep;
            _HelperClass = HelperClass;
            _logger = logger;
        }

        [HttpPost]
        [Route("CreateBizKycInfo")]

        public ActionResult<DtoApiResponse> CreateBizKycInfo([FromBody] DtoBizKycDoc userData)
        {
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (userData.bizInfo.BusinessInfoId == 0)
                {
                    string encryptedFileContent = "";
                    encryptedFileContent = _HelperClass.AESEncrypt(userData.bizKycInfoDoc.DocContent.ToString());
                   // byte[] byteArray = Encoding.UTF8.GetBytes(encryptedFileContent);
                    userData.bizKycInfoDoc.DocContent = encryptedFileContent;
                    _bizKycRep.CreateKycBiz(userData);
                    obj.StateCode = "200";
                    obj.Message = "Data Added Successfully";
                    obj.Data = new object();
                    return obj;

                }
                else
                {
                    obj.StateCode = "400";
                    obj.Message = "Invaild Data"; 
                    obj.Data = new object();
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

        [HttpGet]
        [Route("GetBusinessInfo")]

        public ActionResult<DtoApiResponse> GetBusinessInfo(long userID)
        {
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                var getBusinessInfo = _bizKycRep.GetBusinessInfo(userID).ToList();

                if (getBusinessInfo.Count == 0)
                {
                    obj.StateCode = "204";
                    obj.Message = "No data";
                    obj.Data = new object();
                    return obj;
                }
                List<DtoBizKycDoc> docObj = new List<DtoBizKycDoc>();
                foreach (var item in getBusinessInfo)
                {
                    DtoBizKycDoc bizItem = new DtoBizKycDoc();

                    DtoBizInfo bizInfo = new DtoBizInfo();
                    bizInfo.BusinessInfoId = item.BusinessInfoId;
                    bizInfo.UserId = userID;
                    bizInfo.CityId = item.CityId;
                    bizInfo.CountryId = item.CountryId;
                    bizInfo.StateId = item.StateId;
                    bizInfo.ZipCode = item.ZipCode;
                    bizInfo.BusinessName = item.BusinessName;
                    bizInfo.BusinessContactNumber = item.BusinessContactNumber;
                    bizInfo.BusinessType = item.BusinessType;
                    bizItem.bizInfo = bizInfo;

                    DtoKycInfo dtoKycInfo = new DtoKycInfo();
                    foreach (var itemKyc in item.KycInfos)
                    {
                        dtoKycInfo.BusinessInfoId = itemKyc.BusinessInfoId;
                        dtoKycInfo.KycInfoId = itemKyc.KycInfoId;
                        dtoKycInfo.KycDocTypeId = itemKyc.KycDocTypeId;
                        dtoKycInfo.KycNumber = itemKyc.KycNumber;
                        dtoKycInfo.KycDocUrl = itemKyc.KycDocUrl;
                        dtoKycInfo.KycExpireDate = itemKyc.KycExpireDate;
                        dtoKycInfo.IsActive = itemKyc.IsActive;
                        bizItem.bizKycDoc = dtoKycInfo;

                        DtoKycInfoDoc dtoKycInfoDoc = new DtoKycInfoDoc();

                        foreach (var itemKycDoc in itemKyc.KycInfoDocs)
                        {
                            dtoKycInfoDoc.KycInfoDocId = itemKycDoc.KycInfoDocId;
                            dtoKycInfoDoc.KycInfoId = itemKycDoc.KycInfoId;
                            dtoKycInfoDoc.CreatedDate = itemKycDoc.CreatedDate;
                            dtoKycInfoDoc.DocNameDesc = itemKycDoc.DocNameDesc;
                            ////byte[] encryptedByteArray = itemKycDoc.DocContent;
                            //// Convert the byte array to an encrypted string
                            //string encryptedString = Encoding.UTF8.GetString(itemKycDoc.DocContent);
                            //// Decrypt the encrypted string
                            string decryptedContent = _HelperClass.AESDecrypt(itemKycDoc.DocContent);
                            dtoKycInfoDoc.DocContent = decryptedContent;
                            dtoKycInfoDoc.DocFileExn = itemKycDoc.DocFileExn;
                            dtoKycInfoDoc.DocSize = itemKycDoc.DocSize;
                            bizItem.bizKycInfoDoc = dtoKycInfoDoc;
                        }
                    }




                    docObj.Add(bizItem);
                }
                

                obj.StateCode = "200";
                obj.Message = "";
                obj.Data = docObj;
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
