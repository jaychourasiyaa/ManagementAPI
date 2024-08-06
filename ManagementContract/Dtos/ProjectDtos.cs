using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Contract.Dtos
{
    public class ProjectDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Team_Member { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        
        [ForeignKey(nameof(AssignedById))]
        public int AssignedById { get; set; } = 0;
        public Employee ProjectCreator { get; set; }
    }
}
