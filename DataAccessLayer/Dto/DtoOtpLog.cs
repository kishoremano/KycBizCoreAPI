namespace KycBizWebApi.Dto
{
    public class DtoOtpLog
    {
        public long Otpid { get; set; }

        public long? OtpCode { get; set; }

        public long? UserId { get; set; }

        public DateTime? OtpIssued { get; set; }

        public DateTime? OtpExpired { get; set; }
    }
}
