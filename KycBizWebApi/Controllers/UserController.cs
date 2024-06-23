using KycBizWebApi.Dto;
using KycBizWebApi.Helper;
using KycBizWebApi.Repository.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Repository.Common;
using DataAccessLayer.Models;
using DataAccessLayer.Dto;
using Serilog;
using DataAccessLayer.Helper;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace KycBizWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        #region Declaraion
        private readonly IUserRespository _userRep;
        private readonly ICommonRepository _CommonRep;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UserController> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        private readonly HelperClass _HelperClass;

        DtoCommon objDtoCommon = new DtoCommon();
        LocationDeviceDet objLocationDeviceDet = new LocationDeviceDet();


        private string msg = "";
        public UserController(IUserRespository userRep, IConfiguration config, IWebHostEnvironment webHostEnvironment,
            ICommonRepository CommonRepository, ILogger<UserController> logger, IDiagnosticContext diagnosticContext, HelperClass HelperClass)
        {
            _userRep = userRep;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _CommonRep = CommonRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
            _diagnosticContext.Set("UserId", "Anonymous");
            _HelperClass = HelperClass;
        }

        
        #endregion

        #region UserRegister
        [AllowAnonymous]
        [HttpPost]
        [Route("UserRegister")]
        public ActionResult<DtoApiResponse> UserRegister([FromBody] DtoUserRegister userData)
        {
            _logger.LogInformation("New User Registration.");
            DtoApiResponse obj = new DtoApiResponse();

            int flg = 0;

            try
            {
                if (userData.UserId == 0)
                {
                    var chkAlreadyExist = _userRep.ValidUserInfo(userData.EmailID, userData.MobileNumber);



                    if (chkAlreadyExist == null)
                    {
                        string msg = _HelperClass.ValidatePassword(userData.Password);
                        if(msg != "")
                        {
                            obj.StateCode = "400";
                            obj.Message = msg;
                            obj.Data = new object();
                            _logger.LogInformation(msg);
                            return obj;
                        }
                        userData.Password = _userRep.EncodePasswordToBase64(userData.Password);


                        _userRep.InsertUser(userData);

                        var getUserInfo = _userRep.ValidUserInfo(userData.EmailID, userData.MobileNumber);
                        Random random = new Random();
                        int otpGenerated = random.Next(10001, 99999);

                        DtoOtpLog otpLog = new DtoOtpLog();
                        otpLog.Otpid = 0;
                        otpLog.OtpCode = otpGenerated;
                        otpLog.UserId = getUserInfo.UserId;
                        userData.UserId = getUserInfo.UserId;
                        _userRep.InsertOtpLog(otpLog);

                        string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "OTPDesign.html");
                        objDtoCommon.OTP = otpGenerated;
                        objDtoCommon.ContentType = "OTP";
                        objDtoCommon.MailContentType = "OTP"; // content Master Name
                        objDtoCommon.Subject = "" + otpGenerated + " is your Email Verification Code"; ;
                        objDtoCommon.MailFilePath = filePath;
                        objDtoCommon.FromEmail = userData.EmailID;
                        objDtoCommon.ReceptionName = userData.FirstName;
                        objDtoCommon.callbackUrl = "";
                        objDtoCommon.UserId = getUserInfo.UserId;
                        obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out flg);
                        if (flg == 0)
                            return BadRequest(obj);
                        else
                        {
                            _logger.LogInformation("Data Added Successfully");
                            obj.StateCode = "200";
                            obj.Message = "Data Added Successfully";
                            obj.Data = userData.UserId;
                            return obj;
                        }

                    }
                    else
                    {
                        var chkAlreadyExistisActive = _userRep.ValidUserInfoWithActivation(userData.EmailID, userData.MobileNumber);

                        if (chkAlreadyExistisActive == null)
                        {
                            _logger.LogWarning("Data Already Exists,User Email Verification Pending");
                            obj.StateCode = "400";
                            obj.Message = "Data Already Exists,User Email Verification Pending";
                            obj.Data = new object();
                            return obj;
                        }
                        else
                        {
                            var IsMailExist = _userRep.FindByEmailUserInfo(userData.EmailID);
                            if(IsMailExist == null)
                            {
                                _logger.LogWarning("Mobile Number Already Exists");
                                obj.StateCode = "400";
                                obj.Message = "Mobile Number Already Exists";
                                obj.Data = new object();
                                return obj;
                            }
                            else
                            {
                                _logger.LogWarning("Email Already Exists");
                                obj.StateCode = "400";
                                obj.Message = "Email Already Exists";
                                obj.Data = new object();
                                return obj;
                            }
                           
                        }
                    }

                }
                else
                {
                    _logger.LogWarning("Invaild Data");
                    obj.StateCode = "400";
                    obj.Message = "Invaild Data";
                    obj.Data = new object();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                error("UserRegister", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }
        }

        #endregion

        #region LoginVerification
        [AllowAnonymous]
        [HttpPost]
        [Route("LoginVerification")]
        public ActionResult<DtoApiResponse> LoginVerification([FromBody] DtoLoginRequest userData)
        {
            _logger.LogInformation("Login Verification");
            DtoApiResponse obj = new DtoApiResponse();
            int flg = 0;
            try
            {
                //error("LoginVerification", "sample", 1);
                if (userData.EmailID == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Email ID";
                    obj.Data = new object();
                    return obj;

                }
                else if (userData.Password == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Password";
                    obj.Data = new object();
                    return obj;
                }

                var chkEmail = _userRep.FindByEmailUserInfo(userData.EmailID);

                if (chkEmail == null)
                {
                    obj.StateCode = "400";
                    obj.Message = "Invaild Email ID, Data Not Found";
                    obj.Data = new object();
                    return obj;
                }

                var chkValidEmailPwd = _userRep.FindByEmailPasswordUserInfo(userData.EmailID, userData.Password);

                if (chkValidEmailPwd == null)
                {
                    obj.StateCode = "400";
                    obj.Message = "Invaild Email ID and Password";
                    obj.Data = new object();
                    return obj;
                }
                else
                {

                    Random random = new Random();
                    int otpGenerated = random.Next(10001, 99999);
                    DtoMailLog objDtoMailLog = new DtoMailLog();
                    DtoOtpLog otpLog = new DtoOtpLog();
                    otpLog.Otpid = 0;
                    otpLog.OtpCode = otpGenerated;
                    otpLog.UserId = chkValidEmailPwd.UserId;
                    _userRep.InsertOtpLog(otpLog);
                    if (_config["KYCBYZ:IsLive"] == "1")
                    {
                        string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "OTPDesign.html");
                        objDtoCommon.OTP = otpGenerated;
                        objDtoCommon.ContentType = "OTP";
                        objDtoCommon.MailContentType = "OTP"; // content Master Name
                        objDtoCommon.Subject = "" + otpGenerated + " is your Login OTP code"; ;
                        objDtoCommon.MailFilePath = filePath;
                        objDtoCommon.FromEmail = userData.EmailID;
                        objDtoCommon.ReceptionName = chkValidEmailPwd.FirstName;
                        objDtoCommon.callbackUrl = "";
                        objDtoCommon.UserId = chkValidEmailPwd.UserId;
                        obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out flg);
                    }
                    else
                    {
                        obj.StateCode = "200";
                        obj.Message = "OTP Generated";
                        obj.Data = otpGenerated;
                    }
                    if (flg == 0)
                        return BadRequest(obj);
                    else
                        return Ok(obj);

                }
            }
            catch (Exception ex)
            {
                error("LoginVerification", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }

        }
        #endregion

        #region ResendOTP
        [AllowAnonymous]
        [HttpPost]
        [Route("ResendOTP")]
        public async Task<IActionResult> ResendOTP(int UserId)
        {
            _logger.LogInformation("Resend OTP");
            DtoApiResponse obj = new DtoApiResponse();
            int flg = 0;
            try
            {
                if (UserId == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please Enter User ID";
                    obj.Data = new object();
                    return BadRequest(obj);

                }
                var timeUntilNextOtp = await _userRep.GetTimeUntilNextOtpAsync(UserId);

                if (timeUntilNextOtp > TimeSpan.Zero)
                {
                    _logger.LogWarning("Please wait " + timeUntilNextOtp.Seconds + " seconds before requesting a new OTP.");
                    obj.StateCode = "400";
                    obj.Message = "Please wait " + timeUntilNextOtp.Seconds + " seconds before requesting a new OTP.";
                    obj.Data = new object();
                    //return BadRequest(obj);
                    return BadRequest(obj);
                }
                var Data = _userRep.GetUserInfoCheckActivation(UserId);  // to get all users

                if (Data == null)
                {

                    obj.StateCode = "400";
                    obj.Message = "User Details Not Found";
                    obj.Data = new object();
                    return NotFound(obj);
                }
                else
                {

                    Random random = new Random();
                    int otpGenerated = random.Next(10001, 99999);
                    DtoMailLog objDtoMailLog = new DtoMailLog();
                    DtoOtpLog otpLog = new DtoOtpLog();
                    otpLog.Otpid = 0;
                    otpLog.OtpCode = otpGenerated;
                    otpLog.UserId = UserId;
                    _userRep.InsertOtpLog(otpLog);

                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "OTPDesign.html");
                    objDtoCommon.OTP = otpGenerated;
                    objDtoCommon.ContentType = "OTP";
                    objDtoCommon.MailContentType = "OTP"; // content Master Name
                    objDtoCommon.Subject = ""+ otpGenerated + " is your Requested OTP code";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = Data.Email;
                    objDtoCommon.ReceptionName = Data.FirstName;
                    objDtoCommon.callbackUrl = "";
                    objDtoCommon.UserId = UserId;

                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet,out flg);
                    if(flg == 0)
                        return BadRequest(obj);
                    else
                        return Ok(obj);

                }
            }
            catch (Exception ex)
            {
                error("ResendOTP", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return StatusCode(400, obj);
            }

        }
        #endregion

        #region LoginOtpVerify
        [AllowAnonymous]
        [HttpPost]
        [Route("LoginOtpVerify")]
        public ActionResult<DtoApiResponse> LoginOtpVerify([FromBody] DtoOtpVerifyRequest userData)
        {
            _logger.LogInformation("Login Otp Verify");
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (userData.OtpCode == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Otp Code";
                    obj.Data = new object();
                    return obj;

                }
                else if (userData.UserId == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please provide user id";
                    obj.Data = new object();
                    return obj;
                }

                var chkEmail = _userRep.GetUserInfo(userData.UserId);

                if (chkEmail == null)
                {
                    obj.StateCode = "400";
                    obj.Message = "Invaild User ID, Data Not Found";
                    obj.Data = new object();
                    return obj;
                }

                var chkOtpLog = _userRep.VerifyOtpLog(userData, out msg);

                if (chkOtpLog == null)
                {
                    _logger.LogWarning(msg);
                    obj.StateCode = "400";
                    obj.Message = msg;
                    obj.Data = new object();
                    return obj;
                }
                else
                {
                    DtoLoginRepToken tokenRes = new DtoLoginRepToken();
                    tokenRes = _userRep.GenerateToken(chkEmail);
                    if(tokenRes != null)
                    {
                        _logger.LogInformation("Token Generated");
                        obj.StateCode = "200";
                        obj.Message = "Generated Token";
                        obj.Data = tokenRes;
                    }
                    else
                    {
                        _logger.LogWarning("Token Generation Failed");
                        obj.StateCode = "400";
                        obj.Message = "Token Generation Failed";
                        obj.Data = null;
                    }
                    
                    
                    return obj;
                }
            }
            catch (Exception ex)
            {
                error("LoginOtpVerify", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }

        }
        #endregion

        #region VerifyEmailOtp
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyEmailOtp")]
        public ActionResult<DtoApiResponse> VerifyEmailOtp([FromBody] DtoOtpVerifyRequest userData)
        {
            _logger.LogInformation("Verify Email Otp");
            int okflg = 0;
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                // Update IsActive Status in UserInfo
                if (userData.OtpCode == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Otp Code";
                    obj.Data = new object();
                    return obj;

                }
                else if (userData.UserId == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please provide user id";
                    obj.Data = new object();
                    return obj;
                }

                var chkUserID = _userRep.GetUserInfoCheckActivation(userData.UserId);

                if (chkUserID == null)
                {
                    obj.StateCode = "404";
                    obj.Message = "Invaild User ID , Data Not Found";
                    obj.Data = new object();
                    return obj;
                }

                var chkEmail = _userRep.GetUserInfo(userData.UserId);

                if (chkEmail != null)
                {
                    obj.StateCode = "400";
                    obj.Message = "Email ID Already Verified";
                    obj.Data = new object();
                    return obj;
                }

                var chkOtpLog = _userRep.VerifyOtpLog(userData, out msg);

                if (chkOtpLog == null)
                {
                    _logger.LogInformation(msg);
                    obj.StateCode = "400";
                    obj.Message = msg;
                    obj.Data = new object();
                    return obj;
                }
                else
                {

                    DtoUpdateUserInfo dtoUserActive = new DtoUpdateUserInfo();
                    dtoUserActive.isActive = true;
                    dtoUserActive.UserId = userData.UserId;

                    _userRep.UpdateUserActivation(dtoUserActive);

                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "WelcomeDesign.html");
                    objDtoCommon.OTP = 0;
                    objDtoCommon.ContentType = "Welcome Message";
                    objDtoCommon.MailContentType = "Welcome"; // content Master Name
                    objDtoCommon.Subject = chkUserID.FirstName+ ", we’re glad you’re here!";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = chkUserID.Email;
                    objDtoCommon.ReceptionName = chkUserID.FirstName;
                    objDtoCommon.callbackUrl = "";
                    objDtoCommon.UserId = chkUserID.UserId;
                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out okflg);

                    _logger.LogInformation("E-Mail Verified.");
                    obj.StateCode = "200";
                    obj.Message = "Verified Email";
                    obj.Data = new object();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                error("VerifyEmailOtp", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }
        }

        #endregion

        #region UpdateProfileInfo

        [HttpPost]
        [Route("UpdateProfileInfo")]
        public ActionResult<DtoApiResponse> UpdateProfileInfo([FromBody] DtoUpdateUserInfo userData)
        {
            _logger.LogInformation("Update Profile Info");
            Utilities.SetUserID(Request, _diagnosticContext, _logger);
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                string encryptedFileContent = "";

                if (userData == null)
                {
                    obj.StateCode = "400";
                    obj.Message = "User data empty";
                    obj.Data = new object();
                    return obj;
                }
                //if (profilefile == null || profilefile.Length == 0)
                //{
                //    obj.StateCode = "400";
                //    obj.Message = "Profile file not found";
                //    obj.Data = new object();
                //    return obj;
                //}

                // Using IForm File
                //using (var memoryStream = new MemoryStream())
                //{
                //    profilefile.CopyToAsync(memoryStream);
                //    encryptedFileContent = _encryptionService.Encrypt(Convert.ToBase64String(memoryStream.ToArray()));
                //}
                //

                // Using Base64
                //string key = AesEncryptionService.GenerateKey(256);
                //string IV = AesEncryptionService.GenerateIV(128);
                encryptedFileContent = _HelperClass.AESEncrypt(userData.ProfileData); 
                //


                //  var uploadfilename = _userRep.UploadFile(profilefile, userData.UserId, "Profile");
                userData.profileURL = "";
                userData.ProfileData = encryptedFileContent;
                _userRep.UpdateUser(userData);

                var profileData = _userRep.GetUserInfo(userData.UserId);
                _logger.LogInformation("Profile Updated Success");

                obj.StateCode = "200";
                obj.Message = "Updated Profile Successfully";
                obj.Data = profileData;
                return obj;
            }
            catch (Exception ex)
            {
                error("UpdateProfileInfo", ex.Message, 1);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }
        }

        #endregion

        #region GetUserDetails
        [HttpPost]
        [Route("GetUserDetails")]
        public async Task<ActionResult<DtoApiResponse>> GetUserDetails(int UserId)
        {
            _logger.LogInformation("Get User Details");
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                // Update IsActive Status in UserInfo
                if (UserId == 0)
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter User Id";
                    obj.Data = new object();
                    return obj;

                }

                var chkUserID = _userRep.GetUserInfoCheckActivation(UserId);

                if (chkUserID == null)
                {
                    obj.StateCode = "404";
                    obj.Message = "Invaild User ID , Data Not Found";
                    obj.Data = new object();
                    return obj;
                }

                var Data = _userRep.GetUserInfo(UserId);

                if (Data != null)
                {
                    string DecryptedFileContent = "";
                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedDoc", "ProfileImages", Data.FirstName + "." + Data.ProfileFileType);
                    if (Data.ProfileContent != null)
                    {
                        DecryptedFileContent = _HelperClass.AESDecrypt(Data.ProfileContent);
                        byte[] fileBytes = Convert.FromBase64String(DecryptedFileContent);
                        if (!System.IO.File.Exists(filePath))
                        {
                            await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
                        }
                    }

                    DtoUpdateUserInfo dtoUserActive = new DtoUpdateUserInfo();
                    dtoUserActive.FirstName = Data.FirstName;
                    dtoUserActive.LastName = Data.LastName;
                    dtoUserActive.MobileNumber = Data.ContactNumber;
                    dtoUserActive.profileURL = Data.ProfileUrl;
                    dtoUserActive.ProfileData = DecryptedFileContent;
                    dtoUserActive.UserId = UserId;

                    //_userRep.UpdateUserActivation(dtoUserActive);

                    obj.StateCode = "200";
                    obj.Message = "Success";
                    obj.Data = dtoUserActive;
                    return obj;
                }
                else
                {
                    obj.StateCode = "400";
                    obj.Message = "User Details Not Found";
                    obj.Data = new object();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                error("GetUserDetails", ex.Message, 1);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return obj;
            }
        }

        #endregion

        #region ForgotPassword
        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string EMail)
        {
           _logger.LogInformation("Forgot Password");
           int okflg = 0;
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (EMail == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please Enter Mail";
                    obj.Data = new object();
                    return BadRequest(obj);
                }
                var userData = _userRep.FindByEmailUserInfo(EMail);
                if (userData != null)
                {
                    var user = await _userRep.GetForgetTokenInfo(userData.UserId);
                    string token = AesEncryptionService.GenerateKey(256);
                    if (user != null)
                    {
                        var currentTime = DateTime.UtcNow;
                        if (user.LastPasswordResetRequest != null && user.RequestCount >= 5 &&
                            (currentTime - user.LastPasswordResetRequest.Value).TotalHours < 1)
                        {
                            obj.StateCode = "400";
                            obj.Message = "Too many requests. Please try again later.";
                            obj.Data = new object();
                            return BadRequest(obj);
                        }
                        if (user.LastPasswordResetRequest != null && (currentTime - user.LastPasswordResetRequest.Value).TotalHours >= 1)
                        {
                            user.RequestCount = 0;
                        }

                        user.RequestCount += 1;
                        _userRep.UpdateForgotPassToken(user.ForgotPasswordid, token, user.RequestCount ?? 0);
                    }
                    else
                        _userRep.InsertForgotPassToken(token, userData.UserId, 1);

                    string encryption = _HelperClass.AESEncrypt(token + '|' + EMail);

                    var callbackUrl = Url.Action("ResetPassword", "User", new { token= encryption }, protocol: HttpContext.Request.Scheme);

                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "ForgetPasswordDesign.html");
                    objDtoCommon.OTP = 0;
                    objDtoCommon.ContentType = "ForgetPassword";
                    objDtoCommon.MailContentType = "ForgetPassword"; // content Master Name
                    objDtoCommon.Subject = "Forget Passwork Link";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = EMail;
                    objDtoCommon.ReceptionName = userData.FirstName;
                    objDtoCommon.callbackUrl = callbackUrl;
                    objDtoCommon.UserId= userData.UserId;
                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet,out okflg);
                }
                else
                    _logger.LogInformation("User Details not found in ForgotPassword");

                obj.StateCode = "200";
                obj.Message = "If your email is registered, you will receive a password reset link.";
                obj.Data = new object();
                return Ok(obj);

            }
            catch (Exception ex)
            {
                error("ForgotPassword", ex.Message, 1);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return BadRequest(obj);
            }


        }
        #endregion

        #region ResetPassword
        [AllowAnonymous]
        [HttpGet]
        [Route("ResetPassword")]
        public ActionResult<ResetPasswordDto> ResetPassword(string token = null)
        {
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (token == null)
                {
                    throw new ApplicationException("A code must be supplied for password reset.");
                }
                string Decryption = _HelperClass.AESDecrypt(token);
                string[] Decryptioncontent = Decryption.Split('|');
                var model = new ResetPasswordDto { Token = Decryptioncontent[0], Email = Decryptioncontent[1] };
                return model;
            }
            catch(Exception ex)
            {
                error("ResetPassword", ex.Message, 1);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return BadRequest(obj);
            }
        }

        #endregion

        #region ResetPasswordVerify
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            _logger.LogInformation("ResetPassword");
            int okflg = 0;
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _userRep.FindByEmailUserInfo(model.Email);
                if (user == null)
                {
                    _logger.LogInformation("User not found in ResetPassword");

                    obj.StateCode = "400";
                    obj.Message = "Invalid request.";
                    obj.Data = new object();
                    return BadRequest(obj);
                    //return BadRequest(new { Message = "Invalid request." });
                }

                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(); // To get IP address

                // Location Details Get
                string Location = "";
                bool LocationFound = false;

                if (ipAddress != "::1")
                {
                    var locationDetails = await _HelperClass.GetLocationDetailsAsync(ipAddress); // To get Location details
                    if (locationDetails != null)
                    {
                        LocationFound = true;
                        Location = locationDetails.Ip + '|' + locationDetails.City + '|' + locationDetails.Country + '|' + locationDetails.Timezone;
                    }
                    else
                    {
                        LocationFound = false;
                        Location = ipAddress+"|Development";
                    }
                }
                else
                {
                    LocationFound = false;
                    Location = ipAddress+"|Development";
                }
                //

                //Device Details Get

                var DeviceDetails =  _HelperClass.GetDeviceDetails(Request); // To Get Device Details
                string Device = "";
                bool DeviceFound = false;

                if (DeviceDetails != null)
                {
                    Device = DeviceDetails.Browser + '|' + DeviceDetails.OperatingSystem;
                    DeviceFound = true;
                }
                else
                {
                    DeviceFound = true;
                    Device = "";
                }
                objLocationDeviceDet.Location = Location;
                objLocationDeviceDet.Device = Device;
                objLocationDeviceDet.IsLocationAvail = LocationFound;
                objLocationDeviceDet.IsDeviceAvail = DeviceFound;

                //
                string msg = _HelperClass.ValidatePassword(model.Password);
                if (msg != "")
                {
                    obj.StateCode = "400";
                    obj.Message = msg;
                    obj.Data = new object();
                    _logger.LogInformation(msg);
                    return BadRequest(obj);
                }

                var result = await _userRep.ResetPasswordAsync(user.UserId, model.Token, _userRep.EncodePasswordToBase64(model.Password), objLocationDeviceDet);
                if (result.Succeeded)
                {
                    string token = AesEncryptionService.GenerateKey(256);
                    _userRep.InsertForgotPassToken(token, user.UserId, 1);
                    string encryption = _HelperClass.AESEncrypt(token + '|' + model.Email);
                    var callbackUrl = Url.Action("ResetPassword", "User", new { token = encryption }, protocol: HttpContext.Request.Scheme);
                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "PasswordResetSuccessDesign.html");
                    objDtoCommon.OTP = 0;
                    objDtoCommon.ContentType = "Password change success";
                    objDtoCommon.MailContentType = "Password change success"; // content Master Name
                    objDtoCommon.Subject = "Did you just reset your password?";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = user.Email;
                    objDtoCommon.ReceptionName = user.FirstName;
                    objDtoCommon.callbackUrl = callbackUrl;
                    objDtoCommon.UserId = user.UserId;
                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out okflg);
                    _logger.LogInformation("Password reset successfully done.");
                    obj.StateCode = "400";
                    obj.Message = "Password has been reset successfully.";
                    obj.Data = new object();
                    return Ok(obj);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogInformation(error.Description);
                        obj.StateCode = "400";
                        obj.Message = error.Description;
                    }
                    obj.Data = new object();
                    return BadRequest(obj);
                }
            }
            catch (Exception ex)
            {
                error("ResetPassword", ex.Message, 1);
                _logger.LogInformation(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return BadRequest(obj);
            }
        }

        #endregion

        #region ChangePassword
        [AllowAnonymous]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string EMail)
        {
            _logger.LogInformation("Change Password");
            int okflg = 0;
            DtoApiResponse obj = new DtoApiResponse();
            try
            {
                if (EMail == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please Enter Mail";
                    obj.Data = new object();
                    return BadRequest(obj);
                }
                var userData = _userRep.FindByEmailUserInfo(EMail);
                if (userData != null)
                {
                    var user = await _userRep.GetForgetTokenInfo(userData.UserId);
                    string token = AesEncryptionService.GenerateKey(256);
                    if (user != null)
                    {
                        var currentTime = DateTime.UtcNow;
                        if (user.LastPasswordResetRequest != null && user.RequestCount >= 5 &&
                            (currentTime - user.LastPasswordResetRequest.Value).TotalHours < 1)
                        {
                            obj.StateCode = "400";
                            obj.Message = "Too many requests. Please try again later.";
                            obj.Data = new object();
                            return BadRequest(obj);
                        }
                        if (user.LastPasswordResetRequest != null && (currentTime - user.LastPasswordResetRequest.Value).TotalHours >= 1)
                        {
                            user.RequestCount = 0;
                        }

                        user.RequestCount += 1;
                        _userRep.UpdateForgotPassToken(user.ForgotPasswordid, token, user.RequestCount ?? 0);
                    }
                    else
                        _userRep.InsertForgotPassToken(token, userData.UserId, 1);

                    string encryption = _HelperClass.AESEncrypt(token + '|' + EMail);

                    var callbackUrl = Url.Action("ResetPassword", "User", new { token = encryption }, protocol: HttpContext.Request.Scheme);

                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "ForgetPasswordDesign.html");
                    objDtoCommon.OTP = 0;
                    objDtoCommon.ContentType = "Change Password";
                    objDtoCommon.MailContentType = "Change Password"; // content Master Name
                    objDtoCommon.Subject = "Change Passwork Link";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = EMail;
                    objDtoCommon.ReceptionName = userData.FirstName;
                    objDtoCommon.callbackUrl = callbackUrl;
                    objDtoCommon.UserId = userData.UserId;
                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out okflg);
                    
                }
                else
                {
                    _logger.LogInformation("User Details not found in ChangePassword");
                    obj.StateCode = "400";
                    obj.Message = "User Details not found in ChangePassword";
                    obj.Data = new object();
                    return Ok(obj);
                }
                obj.StateCode = "200";
                obj.Message = "You will receive a password reset link in your E-Mail.";
                obj.Data = new object();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                error("ChangePassword", ex.Message, 1);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return BadRequest(obj);
            }


        }
        #endregion

        #region EmailVerificationResend
        [AllowAnonymous]
        [HttpPost]
        [Route("EmailVerificationResend")]
        public async Task<IActionResult> EmailVerificationResend([FromBody] DtoLoginRequest userData)
        {
            _logger.LogInformation("Email Verification Resend OTP");
            DtoApiResponse obj = new DtoApiResponse();
            int flg = 0;
            try
            {
                if (userData.EmailID == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Email ID";
                    obj.Data = new object();
                    return BadRequest(obj);

                }
                else if (userData.Password == "")
                {
                    obj.StateCode = "400";
                    obj.Message = "Please enter Password";
                    obj.Data = new object();
                    return BadRequest(obj);
                }
                var chkValidEmailPwd = _userRep.GetUserforMailVerification(userData.EmailID, userData.Password);

                if (chkValidEmailPwd == null)
                {
                    _logger.LogWarning("Invaild Email ID and Password");
                    obj.StateCode = "400";
                    obj.Message = "Invaild Email ID and Password";
                    obj.Data = new object();
                    return BadRequest(obj);
                }
                if(chkValidEmailPwd.IsActive == true)
                {
                    _logger.LogWarning("Email Already Verified");
                   obj.StateCode = "400";
                    obj.Message = "Email Already Verified";
                    obj.Data = new object();
                    return NotFound(obj);
                }

                var timeUntilNextOtp = await _userRep.GetTimeUntilNextOtpAsync(Convert.ToInt32(chkValidEmailPwd.UserId));

                if (timeUntilNextOtp > TimeSpan.Zero)
                {
                    _logger.LogWarning("Please wait " + timeUntilNextOtp.Seconds + " seconds before requesting a new OTP.");
                    obj.StateCode = "400";
                    obj.Message = "Please wait " + timeUntilNextOtp.Seconds + " seconds before requesting a new OTP.";
                    obj.Data = new object();
                    //return BadRequest(obj);
                    return BadRequest(obj);
                }
                    Random random = new Random();
                    int otpGenerated = random.Next(10001, 99999);
                    DtoMailLog objDtoMailLog = new DtoMailLog();
                    DtoOtpLog otpLog = new DtoOtpLog();
                    otpLog.Otpid = 0;
                    otpLog.OtpCode = otpGenerated;
                    otpLog.UserId = chkValidEmailPwd.UserId;
                    _userRep.InsertOtpLog(otpLog);

                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Mail Content", "OTPDesign.html");
                    objDtoCommon.OTP = otpGenerated;
                    objDtoCommon.ContentType = "OTP";
                    objDtoCommon.MailContentType = "OTP"; // content Master Name
                    objDtoCommon.Subject = "" + otpGenerated + " is your Requested OTP code";
                    objDtoCommon.MailFilePath = filePath;
                    objDtoCommon.FromEmail = chkValidEmailPwd.Email;
                    objDtoCommon.ReceptionName = chkValidEmailPwd.FirstName;
                    objDtoCommon.callbackUrl = "";
                    objDtoCommon.UserId = chkValidEmailPwd.UserId;

                    obj = _HelperClass.MailSend(objDtoCommon, objLocationDeviceDet, out flg);
                    if (flg == 0)
                        return BadRequest(obj);
                    else
                        return Ok(obj);
            }
            catch (Exception ex)
            {
                error("ResendOTP", ex.Message, 1);
                _logger.LogError(ex.Message);
                obj.StateCode = "400";
                obj.Message = ex.Message;
                obj.Data = new object();
                return StatusCode(400, obj);
            }

        }
        #endregion

        #region Error
        private void error(string fnname, string error, int errortype)
        {
           string _ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
             ErrorLog Error = new ErrorLog();
            Error.ErrorType = errortype;
            Error.ErrorMsg = error;
            Error.FunctionName = fnname;
            Error.Ipaddress = _ipAddress;
            Error.PageName = "UserController";
            Error.UserId = 0;
            Error.UserType = 0;
            _CommonRep.SaveError(Error);
        }

        #endregion


        #region SSO-Google
        [AllowAnonymous]
        [HttpGet("signin")]
        public IActionResult SignIn()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("signin-google") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleSignIn()
        {
            // Redirect to Google for sign-in
            var redirectUrl = Url.Action("GoogleSignInCallback");

            // Build authentication properties
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                // Optionally add other properties like prompt, display, etc.
            };

            // Initiate the challenge with Google authentication scheme
            return Challenge(properties, "Google");
        }

        [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        [HttpGet("google-signin-callback")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                // Handle failed authentication
                return BadRequest("Google authentication failed.");
            }

            // Example: Retrieve user details from claims
            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            // Use email (or another identifier) to find or create the user in your database
            var existingUser = _userRep.FindByEmailUserInfo(email);
            if (existingUser == null)
            {
                // Create a new user if not exists
                //var newUser = new MyCustomUser { Email = email, UserName = email }; // Adjust based on your user model
                //var result = await _userManager.CreateAsync(newUser);
                //if (!result.Succeeded)
                //{
                //    // Handle user creation failure
                //    return BadRequest("Failed to create user.");
                //}
                //existingUser = newUser;
            }

            // Sign in the user (if needed)
            //await HttpContext.SignInAsync(existingUser.Id.ToString(), existingUser.UserName, authenticateResult.Properties);

            // Optionally issue JWT token or return success response
            return Ok(new { Email = email, Name = name });
        }



        [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest();

            var claims = result.Principal.Identities
                            .FirstOrDefault()?.Claims.Select(claim => new
                            {
                                claim.Type,
                                claim.Value
                            });

            return Ok(claims);
        }

        [AllowAnonymous]
        [HttpGet("signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    };
    #endregion
}

