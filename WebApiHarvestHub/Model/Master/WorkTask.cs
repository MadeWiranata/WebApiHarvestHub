using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class WorkTask : BaseModel
    {
        public int WorkTaskId { get; set; }
        public int FarmFieldId { get; set; }
        public int WorkTaskTypeCode { get; set; }
        public DateTimeOffset StartedDate { get; set; }
        public DateTimeOffset CanceledDate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCancelled { get; set; }
        public string FarmFieldName { get; set; }
        public string FarmFieldCode { get; set; }
        public float RowWidth { get; set; }
        public string FarmFieldRowDirection { get; set; }
        public string FarmFieldColorHexCode { get; set; }
        public int FarmSiteId { get; set; }
        public string FarmSiteName { get; set; }
        public int CropId { get; set; }
        public string CropCode { get; set; }
        public string WorkTaskSatusCode { get; set; }

    }
    public class WorkTaskListFilterBy
    {
        public string FarmFieldCode { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class WorkTaskSave : BaseModel
    {
        public int WorkTaskId { get; set; }
        public int FarmFieldId { get; set; }
        public int WorkTaskTypeCode { get; set; }
        public DateTimeOffset StartedDate { get; set; }
        public DateTimeOffset CanceledDate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCancelled { get; set; }

    }
}
