using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class TaskTypes : BaseModel
    {
        public int WorkTaskTypeCode { get; set; }
        public string WorkTaskSatusCode { get; set; }

    }
    public class TaskTypesListFilterBy
    {
        public string WorkTaskSatusCode { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class DeleteTaskTypes
    {
        public int UserId { get; set; }
        public int WorkTaskTypeCode { get; set; }
    }
}
