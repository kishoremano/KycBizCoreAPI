using DataAccessLayer.Dto;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Common
{
    public interface ICommonRepository
    {
        public MessageContentMaster GetMessageContent(string ContentType);

        public void InsertMailLog(DtoMailLog cdata);

        public void SaveError(ErrorLog Error);


    }
}
