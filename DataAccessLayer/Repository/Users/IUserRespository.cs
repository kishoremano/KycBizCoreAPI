using KycBizWebApi.Dto;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Dto;

namespace KycBizWebApi.Repository.Users
{
    public interface IUserRespository
    {
        void InsertUser(DtoUserRegister userData);
        UserInfo ValidUserInfo(string emailID, string mobileNumber);
        UserInfo ValidUserInfoWithActivation(string emailID, string mobileNumber);
        string EncodePasswordToBase64(string password);
        string DecodePasswordFrom64(string encodedData);
        UserInfo FindByEmailUserInfo(string emailID);
        UserInfo FindByEmailPasswordUserInfo(string emailID, string password);
        UserInfo GetUserforMailVerification(string emailID, string password);
        DtoLoginRepToken GenerateToken(UserInfo userInfo);

        string UploadFile(IFormFile formFile, long? userID, string docType);


        void UpdateUser(DtoUpdateUserInfo cdata);
        void UpdateUserActivation(DtoUpdateUserInfo cdata);
        UserInfo GetUserInfo(long? userID);
        UserInfo GetUserInfoCheckActivation(long? userID);

        string SendEmail(string strto, string strfrom, string strCC, string strSubject, string strBody,out int flg, bool isImportExpressShipment = false);
      
        void InsertOtpLog(DtoOtpLog otpLog);

        OtpLog VerifyOtpLog(DtoOtpVerifyRequest rdata, out string msg);
         Task<TimeSpan> GetTimeUntilNextOtpAsync(int UsedId);


         Task<IdentityResult> ResetPasswordAsync(long User, string Token, string Password, LocationDeviceDet objLocationDeviceSave);

         void InsertForgotPassToken(string Token, long UserId, int count);

          Task<ForgotPasswordLog> GetForgetTokenInfo(long? userID);

         void UpdateForgotPassToken(long Id, string Token, int count);

    }
}
