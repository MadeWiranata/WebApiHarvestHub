using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class Site : BaseModel
    {
        public int FarmSiteId { get; set; }
        public string FarmSiteName { get; set; }
        public int DefaultPrimaryCropId { get; set; }
        public string CropCode { get; set; }

    }
    public class SiteListFilterBy
    {
        public string FarmSiteName { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class SiteSave : BaseModel
    {
        public int FarmSiteId { get; set; }
        public string FarmSiteName { get; set; }
        public int DefaultPrimaryCropId { get; set; }
    }

    public class DeleteSite
    {
        public int UserId { get; set; }
        public int FarmSiteId { get; set; }
    }
}
