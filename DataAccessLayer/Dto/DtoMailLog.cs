using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto
{
    public class DtoMailLog
    {
        public string? ContentType { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }

        public string? Cc { get; set; }

        public string? Body { get; set; }

        public string? Subject { get; set; }

        public DateTime? SendOn { get; set; }

        public decimal? UserSk { get; set; }

        public bool? IsSend { get; set; }
    }
}
