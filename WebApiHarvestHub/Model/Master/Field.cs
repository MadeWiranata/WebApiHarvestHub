using Dapper.Contrib.Extensions;
using WebApiHarvestHub.Models.Master;

namespace WebApiHarvestHub.Model.Master
{
    public class Field : BaseModel
    {
        public int FarmSiteId { get; set; }
        public int FarmFieldId { get; set; }
        public string FarmFieldName { get; set; }
        public string FarmFieldCode { get; set; }
        public float RowWidth { get; set; }
        public string FarmFieldRowDirection { get; set; }
        public string FarmFieldColorHexCode { get; set; }
        public string FarmSiteName { get; set; }

    }
    public class FieldListFilterBy
    {
        public string FarmFieldName { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class FieldSave : BaseModel
    {
        public int FarmSiteId { get; set; }
        public int FarmFieldId { get; set; }
        public string FarmFieldName { get; set; }
        public string? FarmFieldCode { get; set; }
        public float? RowWidth { get; set; }
        public string? FarmFieldRowDirection { get; set; }
        public string? FarmFieldColorHexCode { get; set; }    

    }
    public class DeleteField
    {
        public int UserId { get; set; }
        public int FarmFieldId { get; set; }
    }
}
