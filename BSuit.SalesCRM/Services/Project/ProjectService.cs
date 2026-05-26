using BSuit.Identity.Data;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Models;
using BSuit.SalesCRM.Services.Project;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSuit.SalesCRM.Models.ProjectVM;

namespace BSuit.SalesCRM.Services.Project
{
    public  class ProjectService : IProjectService
    {
        private readonly SalesCRMContext _context;
        private readonly IdentityDbContext _identityDbContext;

        public ProjectService(SalesCRMContext context, IdentityDbContext identityDbContext)
        {
            _context = context;
            _identityDbContext = identityDbContext;
        }

        public List<ProjectVM> GetAllProjects()
        {
            return (from p in _context.Projects
                    join ps in _context.Priorities
                        on p.PriorityId equals ps.PriorityId into psGroup
                    from ps in psGroup.DefaultIfEmpty() // LEFT JOIN

                    select new ProjectVM
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        ProjectCode = p.ProjectCode,
                        CustomerID = p.CustomerID,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,

                        // ✅ Get from ProjectStatus table
                        Priority = ps != null ? ps.PriorityName : null,

                        ProjectHours = (decimal)p.ProjectHours,
                        BalanceHours = (decimal)p.BalanceHours
                    })
                    .ToList();
        }

        public ProjectVM GetById(Guid id)
        {
            return (from p in _context.Projects
                    join ps in _context.Priorities
                        on p.PriorityId equals ps.PriorityId into psGroup
                    from ps in psGroup.DefaultIfEmpty()

                    where p.ProjectId == id

                    select new ProjectVM
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        ProjectCode = p.ProjectCode,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        ProjectHours = (decimal)p.ProjectHours,
                        BalanceHours = (decimal)p.BalanceHours,
                        PriorityId = p.PriorityId,
                        PriorityName = ps != null ? ps.PriorityName : null
                    }).FirstOrDefault();
        }

        public void UpdateProject(ProjectVM dto)
        {
            var data = _context.Projects.FirstOrDefault(x => x.ProjectId == dto.ProjectId);

            if (data != null)
            {
                data.ProjectName = dto.ProjectName;
                data.ProjectCode = dto.ProjectCode;
                data.StartDate = dto.StartDate;
                data.EndDate = dto.EndDate;
                data.ProjectHours = dto.ProjectHours;
                data.BalanceHours = dto.BalanceHours;

                // ✅ FIXED
                data.PriorityId = dto.PriorityId;

                _context.SaveChanges();
            }
        }

        public void DeleteProject(Guid id)
        {
            var data = _context.Projects.FirstOrDefault(x => x.ProjectId == id);

            if (data != null)
            {
                _context.Projects.Remove(data);
                _context.SaveChanges();
            }
        }
        public ProjectAccount GetProjectDetails(Guid projectId)
        {
            // ✅ STEP 1: Get Project + Account (ONLY from _context)
            var data = (from p in _context.Projects
                        join a in _context.Accounts
                        on p.AccountId equals a.AccountId
                        where p.ProjectId == projectId
                        select new ProjectAccount
                        {
                            ProjectId = p.ProjectId,
                            ProjectName = p.ProjectName,
                            ProjectCode = p.ProjectCode,
                            Description = p.Description,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            ProjectHours = (decimal)p.ProjectHours,
                            BalanceHours = (decimal)p.BalanceHours,

                            AccountId = a.AccountId,
                            AccountName = a.AccountName,
                            CompanyName = a.CompanyName,
                            Phone = a.PersonalPhone,
                            Website = a.Website,
                            City = a.City,
                            State = a.State,
                            Address = a.Address,

                            // ✅ Keep IDs to fetch later
                            ProjectManagerId = p.ProjectManagerId,
                            ProjectSupervisorId = p.ProjectSupervisorId,
                            PrimaryResourceId = p.PrimaryResourceId,
                            OwnerId = a.OwnerId
                        }).FirstOrDefault();

            if (data == null)
                return null;

            // ✅ STEP 2: Get Users separately (from Identity DB)
            var userIds = new List<Guid?>
            {
                data.ProjectManagerId,
                data.ProjectSupervisorId,
                data.PrimaryResourceId,
                data.OwnerId
            };

            var users = _identityDbContext.Users
                         .Where(u => userIds.Contains(u.UserId))
                         .ToList();

            // ✅ STEP 3: Map Users
            var pm = users.FirstOrDefault(x => x.UserId == data.ProjectManagerId);
            var ps = users.FirstOrDefault(x => x.UserId == data.ProjectSupervisorId);
            var pr = users.FirstOrDefault(x => x.UserId == data.PrimaryResourceId);
            var oi = users.FirstOrDefault(x => x.UserId == data.OwnerId);

            data.ProjectManagerName = pm?.FullName;
            data.ProjectManagerEmail = pm?.Email;

            data.ProjectSupervisorName = ps?.FullName;
            data.ProjectSupervisorEmail = ps?.Email;

            data.PrimaryResourceName = pr?.FullName;
            data.PrimaryResourceEmail = pr?.Email;


            data.OwnerName = oi?.FullName;
            data.OwnerEmail = oi?.Email;

            return data;
        }

        public async Task AddAsync(ProjectTaskVM model)
        {
            var data = await (
                from p in _context.Projects
                join pm in _context.ProjectModules
                    on p.ProjectId equals pm.ProjectId
                where p.ProjectId == model.ProjectId
                select new
                {
                    p.TenantId,pm.ProjectModuleId
                   
                }
            ).FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Required data not found");

            var entity = new ProjectTask
            {
                ProjectTaskId = Guid.NewGuid(),
                ProjectId = model.ProjectId,

                // ✅ All auto-bound
                ProjectModuleId = (Guid)model.ProjectModuleId,
                TenantId = data.TenantId,
                PriorityId = model.PriorityId,
                TaskStatusId = model.TaskStatusId,

                TaskName = model.TaskName,
                Description = model.Description,
                AssignedTo = model.AssignedTo,

                StartDate = model.StartDate,
                DueDate = model.DueDate,
                EstimatedHours = model.EstimatedHours,

                CreatedBy = model.CreatedBy,
                CreatedOn = DateTime.Now
            };

            _context.ProjectTasks.Add(entity);
            await _context.SaveChangesAsync();
        }

        //public List<ProjectTaskVM> GetTasksByProjectId(Guid projectId)
        //{
        //    return _context.ProjectTasks
        //        .Where(x => x.ProjectId == projectId)
        //        .Select(x => new ProjectTaskVM
        //        {
        //            ProjectTaskId = x.ProjectTaskId,
        //            ProjectId = x.ProjectId,
        //            TaskName = x.TaskName,
        //            AssignedTo = x.AssignedTo,
        //            Description = x.Description,
        //            TaskStatusId = x.TaskStatusId,
        //            PriorityId = x.PriorityId,
        //            StartDate = x.StartDate,
        //            DueDate = x.DueDate,
        //            EstimatedHours = x.EstimatedHours
        //        }).ToList();
        //}
        public List<ProjectTaskVM> GetTasksByProjectId(Guid projectId)
        {
            // ✅ Get Tasks + Priority
            var tasks = (
                from t in _context.ProjectTasks

                join p in _context.ProjectTaskPriorities
                    on t.PriorityId equals p.PriorityId into priorityJoin
                from priority in priorityJoin.DefaultIfEmpty()

                join m in _context.ProjectModules
                   on t.ProjectModuleId equals m.ProjectModuleId into moduleJoin
                from module in moduleJoin.DefaultIfEmpty()

                where t.ProjectId == projectId

                select new ProjectTaskVM
                {
                    ProjectTaskId = t.ProjectTaskId,
                    ProjectId = t.ProjectId,

                    TaskName = t.TaskName,
                    Description = t.Description,

                    AssignedTo = t.AssignedTo,
                    ProjectModuleId = t.ProjectModuleId,
                    ModuleName = module != null
                                ? module.ModuleName
                                : "",

                    PriorityId = t.PriorityId,
                    PriorityName = priority.PriorityName,

                    TaskStatusId = t.TaskStatusId,

                    StartDate = t.StartDate,
                    DueDate = t.DueDate,

                    EstimatedHours = t.EstimatedHours
                }
            ).ToList();

            // ✅ Get Users
            var users = _identityDbContext.Users.ToList();

            // ✅ Map User Name
            foreach (var task in tasks)
            {
                task.AssignedToName = users
                    .FirstOrDefault(x =>
                        task.AssignedTo.HasValue &&
                        x.Id == task.AssignedTo.Value.ToString()
                    )
                    ?.FullName;
            }

            return tasks;
        }
        public async Task AddAsync(ProjectModuleVM model)
        {
            var project = await _context.Projects
                .Where(x => x.ProjectId == model.ProjectId)
                .Select(x => new { x.OpportunityId, x.TenantId })
                .FirstOrDefaultAsync();

            var entity = new ProjectModule
            {
                ProjectModuleId = (Guid)model.ProjectModuleId,
                ProjectId = model.ProjectId,
                OpportunityId = project?.OpportunityId,

                // ✅ FIXED HERE
                TenantId = project.TenantId,

                ModuleName = model.ModuleName,
                Description = model.Description,
                EstimatedHours = model.EstimatedHours,
                CreatedOn = model.CreatedOn
            };

            _context.ProjectModules.Add(entity);
            await _context.SaveChangesAsync();
        }
        public List<ProjectModuleVM> GetByProjectId(Guid projectId)
        {
            return _context.ProjectModules
                .Where(x => x.ProjectId == projectId)
                .Select(x => new ProjectModuleVM
                {
                    ProjectModuleId = x.ProjectModuleId,
                    ProjectId = (Guid)x.ProjectId,
                    ModuleName = x.ModuleName,
                    Description = x.Description,
                    EstimatedHours = x.EstimatedHours,
                    CreatedOn = (DateTime)x.CreatedOn
                })
                .ToList();
        }

        //Edit -upadte Pro module 
        public ProjectVM.ProjectModuleVM GetProjectModuleById(Guid id)
        {
            return _context.ProjectModules
                .Where(x => x.ProjectModuleId == id)
                .Select(x => new ProjectVM.ProjectModuleVM
                {
                    ProjectModuleId = x.ProjectModuleId,
                    ProjectId = (Guid)x.ProjectId,
                    ModuleName = x.ModuleName,
                    Description = x.Description,
                    EstimatedHours = x.EstimatedHours
                })
                .FirstOrDefault();
        }

        public async Task UpdateProjectModuleAsync(ProjectVM.ProjectModuleVM model)
        {
            var entity = await _context.ProjectModules
                .FirstOrDefaultAsync(x =>
                    x.ProjectModuleId == model.ProjectModuleId);

            if (entity == null)
                throw new Exception("Module not found");

            entity.ModuleName = model.ModuleName;
            entity.Description = model.Description;
            entity.EstimatedHours = model.EstimatedHours;

            await _context.SaveChangesAsync();
        }

        //Edit - update Pro Task 

        public ProjectVM.ProjectTaskVM GetProjectTaskById(Guid id)
        {
            return _context.ProjectTasks
                .Where(x => x.ProjectTaskId == id)
                .Select(x => new ProjectVM.ProjectTaskVM
                {
                    ProjectTaskId = x.ProjectTaskId,
                    ProjectId = x.ProjectId,
                    ProjectModuleId = x.ProjectModuleId,
                    TaskName = x.TaskName,
                    AssignedTo = x.AssignedTo,
                    PriorityId = x.PriorityId,
                    TaskStatusId = x.TaskStatusId,
                    StartDate = x.StartDate,
                    DueDate = x.DueDate,
                    EstimatedHours = x.EstimatedHours,
                    Description = x.Description
                })
                .FirstOrDefault();
        }

        public async Task UpdateProjectTaskAsync(ProjectVM.ProjectTaskVM model)
        {
            if (model.ProjectTaskId == Guid.Empty)
                throw new Exception("Invalid Task Id");

            var entity = await _context.ProjectTasks
                .FirstOrDefaultAsync(x => x.ProjectTaskId == model.ProjectTaskId);

            if (entity == null)
                throw new Exception("Task not found");

            entity.TaskName = model.TaskName;
            entity.ProjectModuleId = (Guid)model.ProjectModuleId;
            entity.AssignedTo = model.AssignedTo;
            entity.PriorityId = model.PriorityId;
            entity.TaskStatusId = model.TaskStatusId;
            entity.StartDate = model.StartDate;
            entity.DueDate = model.DueDate;
            entity.EstimatedHours = model.EstimatedHours;
            entity.Description = model.Description;

            _context.ProjectTasks.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
//public ProjectAccount GetProjectDetails(Guid projectId)
//{
//    var data = (from p in _context.Projects
//                join a in _context.Accounts
//                on p.AccountId equals a.AccountId   // FIX TYPE FIRST
//                where p.ProjectId == projectId
//                select new ProjectAccount
//                {
//                    // Project
//                    ProjectId = p.ProjectId,
//                    ProjectName = p.ProjectName,
//                    ProjectCode = p.ProjectCode,
//                    Description = p.Description,
//                    StartDate = p.StartDate,
//                    EndDate = p.EndDate,
//                    ProjectHours = (decimal)p.ProjectHours,
//                    BalanceHours = (decimal)p.BalanceHours,

//                    // Account
//                    AccountId = a.AccountId,
//                    AccountName = a.AccountName,
//                    CompanyName = a.CompanyName,
//                    Phone = a.Phone,
//                    Website = a.Website,
//                    City = a.City,
//                    State = a.State,
//                    Address = a.Address
//                }).FirstOrDefault();

//    return data;
//}
// 🔹 ADD TASK