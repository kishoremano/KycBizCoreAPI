using System.ComponentModel.DataAnnotations;

namespace KycBizWebApi.Dto
{
    public class DtoUserRegister
    {
        public long UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }      

        [Required]
        public string Password { get; set; }

        [Required]
        public string EmailID { get; set; }

        [Required]
        public string MobileNumber { get; set; }
    }
}
