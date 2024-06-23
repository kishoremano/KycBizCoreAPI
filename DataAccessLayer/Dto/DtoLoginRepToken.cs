namespace KycBizWebApi.Dto
{
    public class DtoLoginRepToken
    {
        public string LoginToken { get; set; } = string.Empty;

        public long? UserId { get; set; }
    }
}
