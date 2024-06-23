using System.ComponentModel.DataAnnotations;

namespace KycBizWebApi.Dto
{
    public class DtoLoginRequest
    {
        [Required]
        public string EmailID { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
