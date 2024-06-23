using DataAccessLayer.Dto;
using DataAccessLayer.Models;
using KycBizWebApi.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Common
{

    public class CommonRepository: ICommonRepository
    {
        private readonly KycbizContext _context;
        public IConfiguration _config;
        private readonly IWebHostEnvironment _webHE;

        public CommonRepository(KycbizContext context, IConfiguration config, IWebHostEnvironment webHE)
        {
            _context = context;
            _config = config;
            _webHE = webHE;
        }

        public MessageContentMaster GetMessageContent(string ContentType)
        {
            var getUser = _context.MessageContentMasters.FirstOrDefault(MInfo => MInfo.ContentType == ContentType && MInfo.IsActive == true);
            if (getUser != null)
            { return getUser; }
            else { return null; }
        }
        public void InsertMailLog(DtoMailLog cdata)
        {
            EmailLog LogInfo = new()
            {
                Subject = cdata.Subject.ToString(),
                ContentType = cdata.ContentType.ToString(),
                Cc = cdata.Cc.ToString(),
                From = cdata.From.ToString(),
                To = cdata.To.ToString(),
                IsSend = cdata.IsSend,
                Body = cdata.Body,
                SendOn = DateTime.Now,
                UserSk = cdata.UserSk
            };
            _context.EmailLogs.Add(LogInfo);
            _context.SaveChanges();
        }

        public void SaveError(ErrorLog Error)
        {
            ErrorLog ErrorLog = new ErrorLog();

            ErrorLog.ErrorType = Error.ErrorType;
            ErrorLog.ModuleId = Error.ModuleId;
            ErrorLog.UserType = Error.UserType;
            ErrorLog.UserId = Error.UserId;
            ErrorLog.ErrorMsg = Error.ErrorMsg;
            ErrorLog.ErrorDate = DateTime.UtcNow;
            ErrorLog.Ipaddress = Error.Ipaddress;
            ErrorLog.PageName = Error.PageName;
            ErrorLog.FunctionName = Error.FunctionName;
            ErrorLog.ErrorStatus = 1;
            ErrorLog.Status = 1;
            ErrorLog.ModifiedOn = DateTime.UtcNow;
            _context.ErrorLogs.Add(ErrorLog);
            _context.SaveChanges();
        }
    }
}
