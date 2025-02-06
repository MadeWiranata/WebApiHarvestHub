using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class Crops : BaseModel
    {
        public int CropId { get; set; }
        public string CropCode { get; set; }

    }
    public class CropsListFilterBy
    {
        public string CropCode { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
