using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class ProjectDetailsByIdDto
    {
        
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
      


        public string CreatedBy { get; set; }
        public ProjectStatus Status { get; set; }
        public List<IdandNameDto> ProjectEmployee { get; set; }
    }
}
