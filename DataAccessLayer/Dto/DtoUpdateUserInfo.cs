using System.ComponentModel.DataAnnotations;

namespace KycBizWebApi.Dto
{
    public class DtoUpdateUserInfo
    {
        public long? UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        public string profileURL { get; set; }

        public string ProfileData { get; set; }

        //public IFormFile profilefile { get; set; }

        public bool isActive { get; set; }

        public string? ProfileFileType { get; set; }
    }
}
