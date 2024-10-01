using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos.TasksDtos
{
    public class GetTaskChildrenDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TaskTypes Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
