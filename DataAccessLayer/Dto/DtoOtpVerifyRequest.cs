namespace KycBizWebApi.Dto
{
    public class DtoOtpVerifyRequest
    {
        public long? OtpCode { get; set; }

        public long? UserId { get; set; }
    }
}
