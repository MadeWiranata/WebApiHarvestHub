using System;
using Dapper.Contrib.Extensions;

namespace WebApiHarvestHub.Models.Master
{
    public class BaseModel
    {
        public bool IsDeleted { get; set; } 
        public int CreatedUserId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; } 
        public int ModifiedUserId { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}