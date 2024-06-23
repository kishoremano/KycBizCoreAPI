using Azure.Core;
using DataAccessLayer.Dto;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Common;
using KycBizWebApi.Dto;
using KycBizWebApi.Repository.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using UAParser;

namespace KycBizWebApi.Helper
{
    public class HelperClass
    {

        #region Declaration
        private readonly IUserRespository _userRep;
        private readonly ICommonRepository _CommonRep;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HelperClass> _logger;
        private readonly HttpClient _httpClient;
        private string msg = "";
        MessageContentMaster objMessageContentMaster = new MessageContentMaster();
        DtoMailLog objDtoMailLog = new DtoMailLog();
        DtoApiResponse obj = new DtoApiResponse();

        public HelperClass(IUserRespository userRep, IConfiguration config, IWebHostEnvironment webHostEnvironment,
            ICommonRepository CommonRepository, ILogger<HelperClass> logger)
        {
            _userRep = userRep;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _CommonRep = CommonRepository;
            _logger = logger;
            _httpClient = new HttpClient(); 
        }

        #endregion

        #region MailSend
        public DtoApiResponse MailSend(DtoCommon objCommon, LocationDeviceDet objLocationDeviceSave,out int flg)
        {
            try
            {
                string Device="", Location = "";
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZone);
                string formattedTime = Time.ToString("dddd, MMMM dd, yyyy 'at' hh:mm tt '(UTC'K')'");
                if (objLocationDeviceSave != null)
                {
                    if (objLocationDeviceSave.IsDeviceAvail == true)
                        Device = objLocationDeviceSave.Device;

                    if (objLocationDeviceSave.IsLocationAvail == true)
                    {
                        string[] LocationArr = objLocationDeviceSave.Location.Split('|');
                        Location = LocationArr[1] + ',' + LocationArr[2];
                    }
                }

                flg = 0;
                // string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "ForgetPasswordDesign.html");
                string htmlContent = System.IO.File.ReadAllText(objCommon.MailFilePath);
                objMessageContentMaster = _CommonRep.GetMessageContent(objCommon.MailContentType);
                if (objMessageContentMaster.EmailContent != null)
                {
                    htmlContent = htmlContent.Replace("[Message]", objMessageContentMaster.EmailContent.ToString());
                    htmlContent = htmlContent.Replace("[Recipient Name]", objCommon.ReceptionName);
                    htmlContent = htmlContent.Replace("[OTP]", objCommon.OTP.ToString());
                    htmlContent = htmlContent.Replace("{callbackUrl}", objCommon.callbackUrl);
                    htmlContent = htmlContent.Replace("[Support Email]", _config["KYCBYZ:SupportEmail"]);
                    htmlContent = htmlContent.Replace("[Support Phone Number]", _config["KYCBYZ:SupportPhoneNumber"]);
                    htmlContent = htmlContent.Replace("[Company Name]", _config["KYCBYZ:CompanyName"]);
                    htmlContent = htmlContent.Replace("[imgscr]", _config["KYCBYZ:Imgscr"]);
                    htmlContent = htmlContent.Replace("[Time]", formattedTime);
                    htmlContent = htmlContent.Replace("[Location]", Location);
                    htmlContent = htmlContent.Replace("[Device]", Device);


                    var emailStatus = _userRep.SendEmail(objCommon.FromEmail, "noreply@skynetwwe.info", "", objCommon.Subject, htmlContent, out flg, false);
                    if (flg == 1)
                    {
                        //string strto, string strfrom, string strCC, string strSubject, string strBody, bool isImportExpressShipment = false
                        objDtoMailLog.From = "noreply@skynetwwe.info";
                        objDtoMailLog.To = objCommon.FromEmail;
                        objDtoMailLog.ContentType = objCommon.ContentType;
                        objDtoMailLog.Body = htmlContent;
                        objDtoMailLog.Subject = objCommon.Subject;
                        objDtoMailLog.Cc = "";
                        objDtoMailLog.IsSend = true;
                        objDtoMailLog.SendOn = DateTime.UtcNow;
                        objDtoMailLog.UserSk = objCommon.UserId;
                        _CommonRep.InsertMailLog(objDtoMailLog);
                        _logger.LogInformation("Mail Log Details Saved.");
                        obj.StateCode = "200";
                        obj.Message = "Generated OTP Sent";
                        obj.Data = objCommon.UserId;
                        return obj;
                    }
                    else
                    {
                        _logger.LogWarning("There is an Issue in Mail send process.");
                        obj.StateCode = "101";
                        obj.Message = "There is an Issue in Mail send process";
                        obj.Data = new object();
                        return obj;
                    }
                }
                else
                {
                    _logger.LogWarning("No Mail Content Found");
                    obj.StateCode = "101";
                    obj.Message = "No Mail Content Found";
                    obj.Data = new object();
                    flg = 0;
                    return obj;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                obj.StateCode = "101";
                obj.Message = ex.Message;
                obj.Data = new object();
                flg = 0;
                return obj;
            }
        }

        #endregion

        #region Location and Device
        public async Task<IPinfoResponse> GetLocationDetailsAsync(string ipAddress  )
        {
            try
            {
                _logger.LogInformation("GetLocationDetails");
                var url = $"https://ipinfo.io/{ipAddress}?token={_config["IPinfo:Token"]}";
                var response = await _httpClient.GetStringAsync(url);
                var locationDetails = JsonConvert.DeserializeObject<IPinfoResponse>(response);
                _logger.LogInformation("Location Details found");
                return locationDetails;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public DeviceDetails GetDeviceDetails(HttpRequest request)
        {
            try
            {
                _logger.LogInformation("GetDeviceDetails");
                var userAgent = request.Headers["User-Agent"].ToString();
                var parser = Parser.GetDefault();
                var clientInfo = parser.Parse(userAgent);
                _logger.LogInformation("Device Details Found");

                return new DeviceDetails
                {
                    UserAgent = userAgent,
                    Browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}",
                    OperatingSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        #endregion

        #region AESEncryptDecrypt
        public string AESEncrypt(string content)
        {
            _logger.LogInformation("AESEncrypt");
            string key = _config["EncriptionKey:Key"];
            string IV = _config["EncriptionKey:Iv"];
            AesEncryptionService objAesEncryptionService = new AesEncryptionService(key, IV);
            string Encryption = objAesEncryptionService.Encrypt(content);
            return Encryption;
        }

        public string AESDecrypt(string content)
        {
            _logger.LogInformation("AESDecrypt");
            string key = _config["EncriptionKey:Key"];
            string IV = _config["EncriptionKey:Iv"];
            AesEncryptionService objAesEncryptionService = new AesEncryptionService(key, IV);
            string Decryption = objAesEncryptionService.Decrypt(content);
            return Decryption;
        }

        #endregion

        #region ValidatePassword
        public string ValidatePassword(string password)
        {
            _logger.LogInformation("Validating Password");
            string Error = "";
            if (password.Length < 8)
            {
                Error = "Password must be at least 8 characters long.";
                return Error;
            }

            if (!password.Any(char.IsUpper))
            {
                Error = "Password must contain at least one uppercase letter.";
                return Error;
            }

            if (!password.Any(char.IsLower))
            {
                Error = "Password must contain at least one lowercase letter.";
                return Error;
            }

            if (!password.Any(char.IsDigit))
            {
                Error = "Password must contain at least one digit.";
                return Error;
            }
            Error="";
            _logger.LogInformation("Password Valid");
            return Error;
        }

        #endregion

       

    }
}
