﻿using ManagementAPI.Contract.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Taask_status;

namespace ManagementAPI.Contract.Dtos
{
    public class UpdateTasksDto
    {
        /*Status
        Description
        Comments
        Hours utilized.
        Change task type.
        Change parent task.
        add children tasks.*/
        [Required(ErrorMessage = "Project Id is a required field")]
        [Range (1,int.MaxValue,ErrorMessage ="ProjectId should be greater than 0")]
        public int ProjectId { get; set; }
        public string? Description { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "ParentId should be greater than or equal to 0")]
        public int? ParentId { get; set; } = null;

        [Range(0, 4, ErrorMessage = "Task Type is invalid should be between 0-4")]
        public TaskTypes ? type { get; set; } = null;

        [Range(0, 2, ErrorMessage = "Status is invalid should be between 0-2")]
        public TasksStatus ? Status { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "Estimate Hours hours cannot be less than 0")]
        public int? EstimateHours { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "Remaining hours cannot be less than 0")]
        public int ? RemainingHours { get; set; } = null;
    }
}