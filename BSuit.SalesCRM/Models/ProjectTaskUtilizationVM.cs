using BSuit.SalesCRM.Entities;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Models
{
    public class ProjectTaskUtilizationVM
    {
        public Guid TimeLogId { get; set; }

        public Guid ProjectTaskId { get; set; }

        public string EmployeeName { get; set; }

        public DateOnly WorkDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string HoursSpent { get; set; }

        public string Description { get; set; }

        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public string ProjectCode { get; set; }

        public string ProjectStartDate { get; set; }

        public string ProJectEndDate { get; set; }

        public string ProjectHours { get; set; }

        public string BalanceHours { get; set; }

        public string TotalHours { get; set; }

        public List<SelectListItem> ProjectTasks { get; set; }

    }

    public class TaskUtilizationListVM
    {
        public Guid TimeLogId { get; set; }

        public Guid ProjectTaskId { get; set; }

        public string EmployeeName { get; set; }

        // Display format
        public string WorkDate { get; set; }

        // Display format
        public string StartTime { get; set; }

        // Display format
        public string EndTime { get; set; }

        public string HoursSpent { get; set; }

        public string Description { get; set; }

        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public string ProjectCode { get; set; }

        public string ProjectStartDate { get; set; }

        public string ProJectEndDate { get; set; }

        public string ProjectHours { get; set; }

        public string BalanceHours { get; set; }

        public string TotalHours { get; set; }
    }
}
