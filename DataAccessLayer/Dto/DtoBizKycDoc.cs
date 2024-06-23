using DataAccessLayer.Models;

namespace KycBizWebApi.Dto
{
    public class DtoBizKycDoc
    {
        public DtoBizInfo bizInfo { get; set; }

        public DtoKycInfo bizKycDoc { get; set; }

        public DtoKycInfoDoc bizKycInfoDoc { get; set; }

    }

    public class DtoBizInfo
    {
        public long BusinessInfoId { get; set; }

        public string? BusinessName { get; set; }

        public string? BusinessContactNumber { get; set; }

        public string? BusinessType { get; set; }

        public long? UserId { get; set; }

        public long? CountryId { get; set; }

        public long? StateId { get; set; }

        public long? CityId { get; set; }

        public string? ZipCode { get; set; }

        public string? LocationName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }

    public class DtoKycInfo 
    {
        public long KycInfoId { get; set; }

        public long? KycDocTypeId { get; set; }

        public string? KycNumber { get; set; }

        public long? UserId { get; set; }

        public DateTime? KycExpireDate { get; set; }

        public string? KycDocUrl { get; set; }

        public bool? IsActive { get; set; }

        public long? BusinessInfoId { get; set; }
    }

    public class DtoKycInfoDoc 
    {
        public long KycInfoDocId { get; set; }

        public long? UserId { get; set; }

        public string? DocNameDesc { get; set; }

        public string? DocFileExn { get; set; }

        public long? DocSize { get; set; }

        public string DocContent { get; set; }

        public DateTime? CreatedDate { get; set; }

        public long? KycInfoId { get; set; }
    }

}
