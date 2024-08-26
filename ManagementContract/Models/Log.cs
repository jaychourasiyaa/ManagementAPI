using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TasksAPI;

namespace ManagementAPI.Contract.Models
{
    public class Log
    {
        public int Id { get; set; }
        [ForeignKey(nameof(TaskId))]
        public int TaskId { get; set; }
        public Tasks Taask { get; set; }
        public string Message { get; set; }
        public DateTime dateTime { get; set; }=DateTime.Now;

    }
}
