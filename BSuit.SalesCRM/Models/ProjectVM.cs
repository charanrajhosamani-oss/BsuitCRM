using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Models
{
    public class ProjectVM
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string? ProjectCode { get; set; }
        public string? CustomerID { get; set; }
        public string? Description { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal ProjectHours { get; set; }
        public Guid ProjectManagerId { get; set; }
        public Guid ProjectStatusId { get; set; }
        public DateOnly? Today { get; set; }
        public string? Priority { get; set; }
        public decimal BalanceHours { get; set; }
        public string? PriorityName { get; set; }
        public string? Status { get; set; }
        public Guid CreatedBy { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
        public Guid? PriorityId { get; set; }
        public List<SelectListItem> PriorityList { get; set; }

        // ✅ ADD THESE (Account Fields)
        // ✅ PROJECT ACCOUNT
        public class ProjectAccount
        {
            // =========================
            // ACCOUNT DETAILS
            // =========================

            public Guid? AccountId { get; set; }
            public string? AccountName { get; set; }
            public string? CompanyName { get; set; }
            public string? Phone { get; set; }
            public string? Website { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Address { get; set; }

            // =========================
            // PROJECT DETAILS
            // =========================

            public Guid ProjectId { get; set; }

            public string? ProjectName { get; set; }

            public string? ProjectCode { get; set; }

            public string? CustomerId { get; set; }

            public string? Description { get; set; }

            public DateOnly? StartDate { get; set; }

            public DateOnly? EndDate { get; set; }

            public decimal ProjectHours { get; set; }

            public decimal BalanceHours { get; set; }

            public string? Priority { get; set; }

            public string? Status { get; set; }

            // =========================
            // TEAM DETAILS
            // =========================

            public Guid? ProjectManagerId { get; set; }

            public Guid? ProjectSupervisorId { get; set; }

            public Guid? PrimaryResourceId { get; set; }

            public Guid OwnerId { get; set; }


            public string? ProjectManager { get; set; }
            public string? ProjectManagerName { get; set; }

            public string? ProjectSupervisorName { get; set; }

            public string? PrimaryResourceName { get; set; }
            public string? OwnerName { get; set; }

            public string? ProjectManagerEmail { get; set; }

            public string? ProjectSupervisorEmail { get; set; }

            public string? PrimaryResourceEmail { get; set; }

            public string? OwnerEmail { get; set; }


            // =========================
            // AUDIT
            // =========================

            public Guid? CreatedBy { get; set; }

            public DateTime? CreatedOn { get; set; }

            // =========================
            // CHILD COLLECTIONS
            // =========================

            public List<ProjectModuleVM> ProjectModules { get; set; }
                = new List<ProjectModuleVM>();

            public List<ProjectTaskVM> ProjectTasks { get; set; }
                = new List<ProjectTaskVM>();

            public List<ProjectTimeLogVM> ProjectTimeLogs { get; set; }
                = new List<ProjectTimeLogVM>();

           

            public ProjectModuleVM NewProjectModule { get; set; }
                = new ProjectModuleVM();


            public ProjectTaskVM? NewProjectTask { get; set; }
        }

        public class ProjectTaskVM
        {
            public Guid? ProjectTaskId { get; set; }

            public Guid ProjectId { get; set; }
            [Required(ErrorMessage = "Module is required")]
            public Guid? ProjectModuleId { get; set; }

            // =====================================
            // REQUIRED FIELDS
            // =====================================

            [Required(ErrorMessage = "Task Name is required")]
            public string TaskName { get; set; }

            public string? ModuleName { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Assigned User is required")]
            public Guid? AssignedTo { get; set; }

            [Required(ErrorMessage = "Task Status is required")]
            public Guid? TaskStatusId { get; set; }

            [Required(ErrorMessage = "Priority is required")]
            public Guid? PriorityId { get; set; }

            [Required(ErrorMessage = "Start Date is required")]
            public DateTime? StartDate { get; set; }

            [Required(ErrorMessage = "Due Date is required")]
            public DateTime? DueDate { get; set; }

            [Required(ErrorMessage = "Estimated Hours is required")]
            public decimal? EstimatedHours { get; set; }

            // =====================================
            // DISPLAY ONLY
            // =====================================

            public string? AssignedToName { get; set; }

            public string? PriorityName { get; set; }

            // =====================================
            // SYSTEM FIELDS
            // =====================================

            public Guid CreatedBy { get; set; }

            public DateTime CreatedOn { get; set; }

            public Guid TenantId { get; set; }

            // =====================================
            // DROPDOWNS
            // =====================================

            public List<SelectListItem>? ProjectTaskPriorities { get; set; }

            public List<SelectListItem>? UserList { get; set; }

            public List<SelectListItem>? ProjectTaskStatus { get; set; }
            public List<SelectListItem>? ProjectModule { get; set; }
 public List<SelectListItem>? PriorityList { get; set; }
        }

        // =========================
        // PROJECT TIME LOG
        // =========================

        public class ProjectTimeLogVM
        {
            public Guid ProjectTimeLogId { get; set; }

            public Guid ProjectTaskId { get; set; }

            public Guid ProjectId { get; set; }

            public Guid? EmployeeId { get; set; }

            public string? EmployeeName { get; set; }

            public string? TaskName { get; set; }

            public DateTime? WorkDate { get; set; }

            public TimeSpan? StartTime { get; set; }

            public TimeSpan? EndTime { get; set; }

            public decimal? TotalHours { get; set; }

            public string? Description { get; set; }
        }

        //Project Modules
        public class ProjectModuleVM
        {
            public Guid? ProjectModuleId { get; set; }

            public Guid? OpportunityId { get; set; }

            public Guid ProjectId { get; set; }

            [Required(ErrorMessage = "Module Name is required")]
            public string ModuleName { get; set; }

            [Required(ErrorMessage = "Description is required")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Estimated Hours is required")]
            public decimal? EstimatedHours { get; set; }

            public DateTime CreatedOn { get; set; }

            public Guid TenantId { get; set; }
        }


    }
}
//public class ProjectAccount
//{
//    public Guid? AccountId { get; set; }
//    public string? AccountName { get; set; }
//    public string? CompanyName { get; set; }
//    public string? Phone { get; set; }
//    public string? Website { get; set; }
//    public string? City { get; set; }
//    public string? State { get; set; }
//    public string? Address { get; set; }
//    public Guid ProjectId { get; set; }
//    public string ProjectName { get; set; }
//    public string? ProjectCode { get; set; }
//    public string? Description { get; set; }
//    public DateOnly? StartDate { get; set; }
//    public DateOnly? EndDate { get; set; }
//    public decimal ProjectHours { get; set; }
//    public decimal BalanceHours { get; set; }

//    public Guid? ProjectManagerId { get; set; }
//    public Guid? ProjectSupervisorId { get; set; }
//    public Guid? PrimaryResourceId { get; set; }

//    public string? ProjectManagerName { get; set; }
//    public string? ProjectSupervisorName { get; set; }
//    public string? PrimaryResourceName { get; set; }

//    public string? ProjectManagerEmail { get; set; }
//    public string? ProjectSupervisorEmail { get; set; }
//    public string? PrimaryResourceEmail { get; set; }

//}

//Project Task 