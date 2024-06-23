using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

using DataAccessLayer.Dto;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
namespace KycBizWebApi.Repository.Users
{
    public class UserRespository : IUserRespository
    {
        #region Declaration
        private readonly KycbizContext _context;
        public IConfiguration _config;
        private readonly IWebHostEnvironment _webHE;
        private readonly ILogger<UserRespository> _logger;
        public UserRespository(KycbizContext context, IConfiguration config, IWebHostEnvironment webHE, ILogger<UserRespository> logger)
        {
            _context = context;
            _config = config;
            _webHE = webHE;
            _logger = logger;
        }

        #endregion

        #region Password EncryptDecrypt
        public string DecodePasswordFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes("kycbiz" + password + "23");
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        #endregion

        #region User
        public void InsertUser(DtoUserRegister cdata)
        {
            UserInfo userInfo = new()
            {
                FirstName = cdata.FirstName.ToString(),
                LastName = cdata.LastName.ToString(),
                Password = cdata.Password.ToString(),
                Email = cdata.EmailID.ToString(),
                ContactNumber = cdata.MobileNumber.ToString(),
                IsActive = false
            };
            _context.UserInfos.Add(userInfo);
            _context.SaveChanges();
            _logger.LogInformation("New User Added with given information " +
                                "({userInfo})", userInfo);
        }

        public void UpdateUser(DtoUpdateUserInfo cdata)
        {

            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.UserId == cdata.UserId);
            getUser.FirstName = cdata.FirstName.ToString();
            getUser.LastName = cdata.LastName.ToString();
            getUser.ProfileUrl = cdata.profileURL.ToString();
            getUser.ProfileContent = cdata.ProfileData.ToString();
            getUser.UpdatedDate = DateTime.UtcNow;
            getUser.ProfileFileType = cdata.ProfileFileType;
            _context.Entry(getUser).State = EntityState.Modified;
            _context.SaveChanges();
            //UserInfo userInfo = new()
            //{
            //    FirstName = cdata.FirstName.ToString(),
            //    LastName = cdata.LastName.ToString(),
            //    ProfileUrl = cdata.profileURL.ToString(),
            //    ProfileContent = cdata.ProfileData.ToString(),
            //    UpdatedDate = DateTime.UtcNow

            //};

            //_context.Entry(userInfo).State = EntityState.Modified;
            //_context.SaveChanges();
        }

        public void UpdateUserActivation(DtoUpdateUserInfo cdata)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.UserId == cdata.UserId);
            getUser.IsActive = true;
            getUser.UpdatedDate = DateTime.UtcNow;

            _context.Entry(getUser).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public UserInfo ValidUserInfo(string emailID, string mobileNumber)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.Email == emailID || uInfo.ContactNumber == mobileNumber);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo ValidUserInfoWithActivation(string emailID, string mobileNumber)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.Email == emailID && uInfo.ContactNumber == mobileNumber && uInfo.IsActive == true);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo FindByEmailUserInfo(string emailID)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.Email == emailID && uInfo.IsActive == true);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo GetUserInfo(long? userID)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.UserId == userID && uInfo.IsActive == true);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo GetUserInfoCheckActivation(long? userID)
        {
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.UserId == userID);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo FindByEmailPasswordUserInfo(string emailID, string password)
        {
            string pwd = EncodePasswordToBase64(password);
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.Email == emailID && uInfo.Password == pwd && uInfo.IsActive == true);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public UserInfo GetUserforMailVerification(string emailID, string password)
        {
            string pwd = EncodePasswordToBase64(password);
            var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.Email == emailID && uInfo.Password == pwd);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }


        #endregion

        #region UploadFile
        public string UploadFile(IFormFile formFile, long? userID, string docType)
        {
            try
            {
                if (formFile != null && userID != 0 && docType != null)
                {
                    string profileFilePath;

                    if (docType == "Profile")
                    {
                        profileFilePath = _config["UploadPath:Profile"];
                    }
                    else
                    {
                        profileFilePath = _config["UploadPath:KycDoc"];
                    }


                    string uploadsFolder = Path.Combine(_webHE.WebRootPath, profileFilePath);

                    if (!System.IO.Directory.Exists(uploadsFolder))
                    {
                        System.IO.Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }

                    return filePath;
                }
                else
                {
                    return "Invaild or Missing Data";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion

        #region JWTToken
        public DtoLoginRepToken GenerateToken(UserInfo userInfo)
        {
            DtoLoginRepToken tokenRes = new DtoLoginRepToken();

            if (userInfo != null)
            {
                //create claims details based on the user information
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                            new Claim("Id", userInfo.UserId.ToString()),
                            new Claim("FirstName", userInfo.FirstName),
                            new Claim("LastName", userInfo.LastName),
                            new Claim("Email", userInfo.Email)
                    }),
                    Issuer = _config["Jwt:Issuer"], // Issuer
                    Audience = _config["Jwt:Audience"], // Audience
                    Expires = DateTime.Now.AddMinutes(Convert.ToUInt32(_config["Jwt:ExpireTime"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                tokenRes.LoginToken = tokenString;
                tokenRes.UserId = userInfo.UserId;
                _logger.LogInformation("Token Generated");
                return tokenRes;
            }
            else
            {
                return null;
            }
        }
      
        #endregion
     
        #region SendEmail
        public string SendEmail(string strto, string strfrom, string strCC, string strSubject, string strBody, out int flg, bool isImportExpressShipment = false)
        {
            System.Net.Mail.MailMessage objEmail = new System.Net.Mail.MailMessage();
            try
            {
                objEmail.To.Add(strto);
                objEmail.From = new System.Net.Mail.MailAddress(strfrom);
                if (!string.IsNullOrEmpty(strCC))
                    objEmail.CC.Add(new System.Net.Mail.MailAddress(strCC));


                objEmail.Subject = strSubject;
                objEmail.Body = strBody;
                objEmail.Priority = MailPriority.High;
                objEmail.IsBodyHtml = true; // '.BodyFormat = MailFormat.Html

                SmtpClient mSmtpClient = new SmtpClient();
                mSmtpClient.Host = "mail.skynetwwe.info";
                mSmtpClient.Port = 587;
                mSmtpClient.Credentials = new System.Net.NetworkCredential("info@skynetwwe.info", "kjGfUHEmJmkf");
                mSmtpClient.Send(objEmail);
                flg = 1;
                _logger.LogInformation("E-Mail send with given Information " +
                                "({objEmail})", objEmail);
                return "Your E-mail has been sent sucessfully - Thank You";
            }
            catch (Exception exc)
            {
                flg = 0;
                _logger.LogError("E-Mail Send failure:" + exc.ToString());
                return "Send failure: " + exc.ToString();
            }
        }

        #endregion
     
        #region OTP
        public void InsertOtpLog(DtoOtpLog cdata)
        {
            OtpLog otpLog = new()
            {
                OtpCode = cdata.OtpCode,
                UserId = cdata.UserId,
                OtpIssued = DateTime.UtcNow,
                OtpExpired = DateTime.UtcNow.AddSeconds(180)
            };
            _context.OtpLogs.Add(otpLog);
            _context.SaveChanges();
            _logger.LogInformation("OTP saved with given Information({@otpLog})", otpLog);
            //"({objEmail})", otpLog);
        }

        public OtpLog VerifyOtpLog(DtoOtpVerifyRequest rdata, out string msg)
        {
            msg = "";
            try
            {
                var getOtpLog = _context.OtpLogs.FirstOrDefault(uInfo => uInfo.UserId == rdata.UserId && uInfo.OtpCode == rdata.OtpCode);
                if (getOtpLog != null)
                {
                    if (getOtpLog.OtpExpired > DateTime.UtcNow)
                    {
                        _logger.LogInformation("OTP Verfication completed.");
                        msg = "OTP Verfication completed.";
                        return getOtpLog;
                    }
                    else
                    {
                        _logger.LogInformation("OTP Expired.");
                        msg = "OTP Expired.";
                        return null;
                    }
                }
                else
                {
                    _logger.LogInformation("OTP Not Verfied.");
                    msg = "OTP Not Verified.";
                    return null;
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task<TimeSpan> GetTimeUntilNextOtpAsync(int UsedId)
        {
            var otp = await _context.OtpLogs
        .Where(uInfo => uInfo.UserId == UsedId)
        .OrderByDescending(uInfo => uInfo.OtpIssued)
        .FirstOrDefaultAsync();
            if (otp != null)
            {
                var timeSinceLastOtp = DateTime.UtcNow - otp.OtpIssued;
                return TimeSpan.FromMinutes(1) - timeSinceLastOtp.GetValueOrDefault();
            }
            return TimeSpan.Zero;
        }

        #endregion

        #region Forgot Password
        public async Task<IdentityResult> ResetPasswordAsync(long User, string Token, string Password, LocationDeviceDet objLocationDeviceSave)
        {
            PasswordHistory objPasswordHistory = new PasswordHistory();
            var getOtpLog = await _context.ForgotPasswordLogs
        .Where(uInfo => uInfo.UserId == User && uInfo.Token == Token && uInfo.IsTokenVerified != true)
        .OrderByDescending(uInfo => uInfo.LastPasswordResetRequest)
        .FirstOrDefaultAsync();
            //var getOtpLog = _context.ForgotPasswordLogs.FirstOrDefault(uInfo => uInfo.UserId == User && uInfo.Token == Token && uInfo.IsTokenVerified != true);

            if (getOtpLog != null)
            {
                if (getOtpLog.TokenExpired > DateTime.UtcNow)
                {
                    var recentPasswords = await _context.PasswordHistories
                        .Where(ph => ph.UserId == User)
                        .OrderByDescending(ph => ph.CreatedDate)
                        .Take(5) // Adjust the number based on your policy
                        .ToListAsync();

                    if (recentPasswords.Any(ph => ph.Password ==  Password))
                    {
                        _logger.LogInformation("Password has been used recently. Please choose a different password");
                        return IdentityResult.Failed(new IdentityError { Description = "Password has been used recently. Please choose a different password." });
                    }

                    // UserInfo Update
                    var getUser = _context.UserInfos.FirstOrDefault(uInfo => uInfo.UserId == User);
                    getUser.Password = Password;
                    getUser.UpdatedDate = DateTime.UtcNow;
                    _context.Entry(getUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    //// Passowrd History Save
                    //objPasswordHistory.Password = Password;
                    //objPasswordHistory.UserId = User;
                    //objPasswordHistory.CreatedDate = DateTime.UtcNow;
                    //_context.PasswordHistories.Add(objPasswordHistory);
                    //_context.SaveChanges();
                    //_logger.LogInformation("Password History Saved");

                    await SavePasswordHistoryAsync(User, Password);


                    //Forgot Password Log Update
                    getOtpLog.IsTokenVerified = true;
                    getOtpLog.Location = objLocationDeviceSave.Location;
                    getOtpLog.Device = objLocationDeviceSave.Device;
                    getOtpLog.IsDeviceFound = objLocationDeviceSave.IsDeviceAvail;
                    getOtpLog.IsLocationFound = objLocationDeviceSave.IsLocationAvail;

                    _context.Entry(getOtpLog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Forgot Token Verification completed and Password Changed");
                    return IdentityResult.Success;
                }
                else
                {
                    _logger.LogInformation("Forgot Token Expired.");
                    return IdentityResult.Failed(new IdentityError { Description = "Token Expired." });
                }
            }
            else
            {
                var getOtp = _context.ForgotPasswordLogs.FirstOrDefault(uInfo => uInfo.UserId == User);
                if (getOtp != null)
                {
                    _logger.LogInformation("Token Mismatch.");
                    return IdentityResult.Failed(new IdentityError { Description = "Token Mismatch." });
                }
            }
            _logger.LogInformation("This User has not requested for a Password Reset.");
            return IdentityResult.Failed(new IdentityError { Description = "This User has not requested for a Password Reset." });
        }

        public void InsertForgotPassToken(string Token, long UserId, int count)
        {
            ForgotPasswordLog ForgotPasswordLog = new()
            {
                Token = Token,
                UserId = UserId,
                TokenIssued = DateTime.UtcNow,
                TokenExpired = DateTime.UtcNow.AddMinutes(30),
                IsTokenVerified = false,
                LastPasswordResetRequest = DateTime.UtcNow,
                RequestCount = count,

            };
            _context.ForgotPasswordLogs.Add(ForgotPasswordLog);
            _context.SaveChanges();
            _logger.LogInformation("Forgot Pass Token saved with given Information({ForgotPasswordLog})", ForgotPasswordLog);
            //"({objEmail})", otpLog);
        }

        public async Task<ForgotPasswordLog> GetForgetTokenInfo(long? userID)
        {
            var getUser = await _context.ForgotPasswordLogs
        .Where(uInfo => uInfo.UserId == userID  && uInfo.IsTokenVerified != true)
        .OrderByDescending(uInfo => uInfo.LastPasswordResetRequest)
        .FirstOrDefaultAsync();
            //var getUser = _context.ForgotPasswordLogs.FirstOrDefault(uInfo => uInfo.UserId == userID);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }

        public void UpdateForgotPassToken(long Id, string Token, int count)
        {

            var getToken = _context.ForgotPasswordLogs.FirstOrDefault(uInfo => uInfo.ForgotPasswordid == Id);
            getToken.Token = Token;
            getToken.RequestCount = count;
            getToken.LastPasswordResetRequest = DateTime.UtcNow;
            getToken.TokenIssued = DateTime.UtcNow;
            getToken.TokenExpired = DateTime.UtcNow.AddMinutes(30);
            getToken.IsTokenVerified = false;

            _context.Entry(getToken).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public ForgotPasswordLog VerifyForgotPassToken(string Token, long UserId, out string msg)
        {
            msg = "";
            try
            {
                var getOtpLog = _context.ForgotPasswordLogs.FirstOrDefault(uInfo => uInfo.UserId == UserId && uInfo.Token == Token);
                if (getOtpLog != null)
                {
                    if (getOtpLog.TokenExpired > DateTime.UtcNow)
                    {
                        _logger.LogInformation("Forgot Token Verfication completed.");
                        msg = "Forgot Token Verfication completed.";
                        return getOtpLog;
                    }
                    else
                    {
                        _logger.LogInformation("Forgot Token Expired.");
                        msg = "Forgot Token Expired.";
                        return null;
                    }
                }
                else
                {
                    _logger.LogInformation("Forgot Token Not Verfied.");
                    msg = "Forgot Token Not Verified.";
                    return null;
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public async Task SavePasswordHistoryAsync(long userId, string password)
        {
            var passwordHistories = await _context.PasswordHistories
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.CreatedDate)
                .ToListAsync();

            if (passwordHistories.Count >= 5)
            {
                var oldPasswords = passwordHistories.Skip(4).ToList();
                _context.PasswordHistories.RemoveRange(oldPasswords);
            }

            var objPasswordHistory = new PasswordHistory
            {
                Password = password,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.PasswordHistories.Add(objPasswordHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password History Saved");
        }

        #endregion
    }
}
