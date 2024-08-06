using ManagementAPIEmployee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksAPI;

namespace ManagementAPI.Contract.Dtos
{
    public class AddTaskReviewDto
    {
        public int TasksId { get; set; }
        public int ReviewById { get; set; }
        public string Comments { get; set; }

    }
}
