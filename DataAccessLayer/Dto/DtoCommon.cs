using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dto
{
    public class DtoCommon
    {
        public string MailFilePath { get; set; }
        public string MailContentType { get; set; }
        public long OTP { get; set; }
        public string FromEmail { get; set; }
        public string ContentType { get; set; }
        public string ReceptionName { get; set; }

        public string Subject { get; set; }

        public long UserId { get; set; }
        public string callbackUrl { get; set; }


    }
    public class IPinfoResponse
    {
        public string Ip { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Postal { get; set; }
        public string Timezone { get; set; }
    }
    public class DeviceDetails
    {
        public string UserAgent { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
    }

    public class LocationDeviceDet
    {
        public string Location { get; set; }
        public string Device { get; set; }
        public bool IsLocationAvail { get; set; }
        public bool IsDeviceAvail { get; set; }
    }
}
