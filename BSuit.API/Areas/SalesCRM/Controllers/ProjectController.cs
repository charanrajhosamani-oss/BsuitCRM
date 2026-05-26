using BSuit.API.Areas.Admin.Models;
using BSuit.API.Areas.SalesCRM.Models;
using BSuit.API.Infrastructure.Services;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.HR.Entities;
using BSuit.Identity.Data;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Models;
using BSuit.SalesCRM.Services.Project;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models.Security;
using SQLitePCL;
using System.Globalization;
using System.Globalization;
using static BSuit.SalesCRM.Models.ProjectVM;


namespace BSuit.API.Areas.SalesCRM.Controllers
{
    [Area("SalesCRM")]
    public class ProjectController : Controller
    {

        private readonly SalesCRMContext _context;
        private readonly CoreDbContext _context1;
        private readonly IProjectService _projectService;
        //private readonly HttpUserContext _http;
        private readonly IUserContext _http;
        private readonly IdentityDbContext _identityDbContext;



        public ProjectController(
            SalesCRMContext context,
            CoreDbContext coreDbContext,
            IProjectService projectService,
            IUserContext httpUserContext, IdentityDbContext IdentityDbContext
            )
        {
            _context = context;
            _context1 = coreDbContext;
            _projectService = projectService;
            _http = httpUserContext;
            _identityDbContext = IdentityDbContext;


        }
        private void BindPriority(ProjectVM model)
        {
            model.PriorityList = _context.ProjectTaskPriorities
                .Where(x => x.IsActive == true)
                .Select(x => new SelectListItem
                {
                    Value = x.PriorityId.ToString(),
                    Text = x.PriorityName
                }).ToList();
        }


        [HttpGet]
        public IActionResult Index(
            int pageNumber = 1,
            int pageSize = 10,
            string searchText = null,
            string startDate = null,
            string endDate = null
        )
        {
            // 🔥 Get All Projects
            var data = _projectService.GetAllProjects();

            // ✅ Parse Dates
            DateTime? start = null;
            DateTime? end = null;

            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.ParseExact(
                    startDate,
                    "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture
                );
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.ParseExact(
                    endDate,
                    "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture
                );
            }

            // ✅ Start Date Filter
            // ✅ Start Date Filter
            if (start.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(start.Value);

                data = data
                    .Where(x =>
                        x.StartDate.HasValue &&
                        x.StartDate.Value >= startDateOnly
                    )
                    .ToList();
            }

            // ✅ End Date Filter
            if (end.HasValue)
            {
                var endDateOnly = DateOnly.FromDateTime(end.Value);

                data = data
                    .Where(x =>
                        x.EndDate.HasValue &&
                        x.EndDate.Value <= endDateOnly
                    )
                    .ToList();
            }

            // ✅ Search Filter
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();

                data = data
                    .Where(x =>

                        (!string.IsNullOrEmpty(x.ProjectName) &&
                         x.ProjectName.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.ProjectCode) &&
                         x.ProjectCode.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.CustomerID) &&
                         x.CustomerID.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.Priority) &&
                         x.Priority.ToLower().Contains(searchText))

                    )
                    .ToList();
            }

            // ✅ Total Count
            int totalCount = data.Count();

            // ✅ Pagination
            var projects = data
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ✅ Preserve Filters
            ViewBag.SearchText = searchText;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // ✅ Pagination Values
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(projects);
        }



        [HttpGet]
        public IActionResult GetEditPartial(Guid id)
        {
            var data = _projectService.GetById(id);
            if (data != null)
            {
                data.PriorityList = _context.ProjectTaskPriorities
                    .Where(x => x.IsActive == true)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PriorityId.ToString(),   // ✅ GUID
                        Text = x.PriorityName              // ✅ Name
                    }).ToList();
            }
            return PartialView("_EditProject", data);
        }


        [HttpPost]
        public IActionResult Update(ProjectVM dto)
        {
            // ✅ Ignore dropdown list validation
            ModelState.Remove("PriorityList");

            if (!ModelState.IsValid)
            {
                BindPriority(dto);
                return PartialView("_EditProject", dto);
            }

            _projectService.UpdateProject(dto);
            //return Json(new { success = true, message = "Project updated successfully!" });
            TempData["SuccessMessage"] = "Project updated successfully!";

            return RedirectToAction("Index", "Project", new { area = "SalesCRM" });
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            _projectService.DeleteProject(id);
            return Ok();
        }

        [HttpGet]
        public IActionResult ProjectDetails(Guid id, int modulePageNumber = 1, int modulePageSize = 5, int taskPageNumber = 1, int taskPageSize = 5)
        {
            var model = _projectService.GetProjectDetails(id);

            if (model == null)
                return NotFound();

            // =========================================================
            // PROJECT MODULE PAGINATION
            // =========================================================

            var allModules = _projectService
                .GetByProjectId(id)
                .ToList();

            int totalModuleCount = allModules.Count();

            model.ProjectModules = allModules
                .Skip((modulePageNumber - 1) * modulePageSize)
                .Take(modulePageSize)
                .ToList();

            // MODULE PAGINATION VIEWBAG
            ViewBag.ModulePageNumber = modulePageNumber;
            ViewBag.ModulePageSize = modulePageSize;
            ViewBag.ModuleTotalCount = totalModuleCount;
            ViewBag.ModuleTotalPages =
                (int)Math.Ceiling((double)totalModuleCount / modulePageSize);

            // =========================================================
            // MODULE FORM
            // =========================================================

            model.NewProjectModule = new ProjectVM.ProjectModuleVM
            {
                ProjectId = id
            };

            // =========================================================
            // PROJECT TASK PAGINATION
            // =========================================================

            var allTasks = _projectService.GetTasksByProjectId(id).ToList();

            int totalTaskCount = allTasks.Count();

            model.ProjectTasks = allTasks
                .Skip((taskPageNumber - 1) * taskPageSize)
                .Take(taskPageSize)
                .ToList();

            // TASK PAGINATION VIEWBAG
            ViewBag.TaskPageNumber = taskPageNumber;
            ViewBag.TaskPageSize = taskPageSize;
            ViewBag.TaskTotalCount = totalTaskCount;
            ViewBag.TaskTotalPages =
                (int)Math.Ceiling((double)totalTaskCount / taskPageSize);

            // =========================================================
            // TASK FORM
            // =========================================================

            var users = _identityDbContext.Users.ToList();

            model.NewProjectTask = new ProjectVM.ProjectTaskVM
            {
                ProjectId = id,

                ProjectTaskPriorities = _context.ProjectTaskPriorities
                    .Select(x => new SelectListItem
                    {
                        Value = x.PriorityId.ToString(),
                        Text = x.PriorityName
                    }).ToList(),

                ProjectTaskStatus = _context.ProjectTaskStatuses
                    .Select(x => new SelectListItem
                    {
                        Value = x.StatusId.ToString(),
                        Text = x.StatusName
                    }).ToList(),

                UserList = users
                    .Where(x => x.IsActive == true)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.FullName
                    }).ToList(),
                ProjectModule = _context.ProjectModules
                    .Select(x => new SelectListItem
                    {
                        Value = x.ProjectModuleId.ToString(),
                        Text = x.ModuleName
                    }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ProjectTask(Guid projectId)
        {
            var tasks = _projectService.GetTasksByProjectId(projectId);

            ViewBag.ProjectId = projectId;

            return View(tasks); // pass LIST
        }
        [HttpGet]

        public IActionResult CreateProject(Guid projectId)
        {
            var users = _identityDbContext.Users.ToList();

            var model = new ProjectTaskVM
            {
                ProjectId = projectId,

                ProjectTaskPriorities = _context.ProjectTaskPriorities
                .Select(x => new SelectListItem
                {
                    Value = x.PriorityId.ToString(),
                    Text = x.PriorityName   // Low, Medium, High, Urgent
                }).ToList(),
                UserList = users
                        .Where(x => x.IsActive == true)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.FullName
                        }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectAccount model)
        {
            // =========================
            // REMOVE UNWANTED VALIDATION
            // =========================

            ModelState.Remove("ProjectModules");
            ModelState.Remove("ProjectTasks");
            ModelState.Remove("ProjectTimeLogs");

            // REMOVE MODULE FORM VALIDATION
            ModelState.Remove("NewProjectModule.ModuleName");
            ModelState.Remove("NewProjectModule.Description");
            ModelState.Remove("NewProjectModule.EstimatedHours");

            // REMOVE DROPDOWN VALIDATION
            ModelState.Remove("NewProjectTask.ProjectTaskPriorities");
            ModelState.Remove("NewProjectTask.UserList");
            ModelState.Remove("NewProjectTask.ProjectTaskStatus");
            ModelState.Remove("NewProjectTask.ProjectModule");

            if (ModelState.IsValid)
            {
                try
                {
                    var task = model.NewProjectTask;

                    task.CreatedBy = Guid.NewGuid();
                    task.CreatedOn = DateTime.Now;
                    task.TenantId = Guid.NewGuid();

                    await _projectService.AddAsync(task);

                    TempData["Success"] =
                        "Project Task Created Successfully";
                    TempData["ActiveTab"] = "tab-project-task";
                    return RedirectToAction(
                        "ProjectDetails",
                        "Project",
                        new
                        {
                            area = "SalesCRM",
                            id = task.ProjectId
                        });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("",
                        "Error while saving task");
                }
            }

            // =========================
            // KEEP TASK TAB OPEN
            // =========================

            TempData["ActiveTab"] = "tab-project-task";
            TempData["OpenTaskPanel"] = "true";

            // =========================
            // RELOAD PAGE DATA
            // =========================

            var projectModel =
                _projectService.GetProjectDetails(model.ProjectId);

            // KEEP OLD VALUES
            projectModel.NewProjectTask =
                model.NewProjectTask;

            // =========================
            // REBIND DROPDOWNS
            // =========================

            projectModel.NewProjectTask.ProjectModule =
                _context.ProjectModules
                .Where(x => x.ProjectId == model.ProjectId)
                .Select(x => new SelectListItem
                {
                    Value = x.ProjectModuleId.ToString(),
                    Text = x.ModuleName
                }).ToList();

            projectModel.NewProjectTask.UserList =
                _identityDbContext.Users
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                }).ToList();

            projectModel.NewProjectTask.ProjectTaskPriorities =
                _context.ProjectTaskPriorities
                .Select(x => new SelectListItem
                {
                    Value = x.PriorityId.ToString(),
                    Text = x.PriorityName
                }).ToList();

            projectModel.NewProjectTask.ProjectTaskStatus =
                _context.ProjectTaskStatuses
                .Where(x => x.IsActive == true)
                .Select(x => new SelectListItem
                {
                    Value = x.StatusId.ToString(),
                    Text = x.StatusName
                }).ToList();

            return View("ProjectDetails", projectModel);
        }

        [HttpGet]
        public IActionResult ProjectModules(Guid projectId)
        {
            var data = _projectService.GetByProjectId(projectId);

            ViewBag.ProjectId = projectId;

            return View(data);
        }
        [HttpGet]
        public IActionResult CreateProjectModule(Guid projectId)
        {
            var model = new ProjectModuleVM
            {
                ProjectId = projectId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProjectModule(ProjectAccount model)
        {
            // ==========================================
            // GET MODULE OBJECT
            // ==========================================

            var module = model.NewProjectModule;

            // ==========================================
            // REMOVE UNNECESSARY VALIDATION
            // ==========================================

            ModelState.Remove("NewProjectTask");

            if (ModelState.IsValid)
            {
                try
                {
                    module.ProjectModuleId = Guid.NewGuid();
                    module.CreatedOn = DateTime.Now;

                    await _projectService.AddAsync(module);

                    TempData["Success"] = "Project Module Created Successfully";
                    TempData["ActiveTab"] = "tab-project-module";
                    return RedirectToAction(
                        "ProjectDetails",
                        "Project",
                        new
                        {
                            area = "SalesCRM",
                            id = module.ProjectId
                        });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("",
                        "Error while saving module");
                }
            }

            // ==========================================
            // KEEP MODULE TAB ACTIVE
            // ==========================================

            TempData["ActiveTab"] = "tab-project-module";
            TempData["OpenModulePanel"] = "true";

            // ==========================================
            // RELOAD PAGE DATA
            // ==========================================

            var projectModel =
                _projectService.GetProjectDetails(module.ProjectId);

            projectModel.NewProjectModule = module;

            return View("ProjectDetails", projectModel);
        }


        //Edit and Update pro Module

        [HttpGet]
        public IActionResult EditProjectModule(Guid id)
        {


            var module = _projectService.GetProjectModuleById(id);

            if (module == null)
                return NotFound();

            var model = _projectService
                .GetProjectDetails(module.ProjectId);

            if (model == null)
                return NotFound();

            // =========================================
            // LOAD ALL MODULES
            // =========================================

            model.ProjectModules = _projectService
                .GetByProjectId(module.ProjectId)
                .ToList();

            // =========================================
            // LOAD ALL TASKS
            // =========================================

            model.ProjectTasks = _projectService
                .GetTasksByProjectId(module.ProjectId)
                .ToList();

            // =========================================
            // LOAD EDIT MODULE DATA
            // =========================================

            model.NewProjectModule = module;

            // =========================================
            // TASK DROPDOWNS
            // =========================================

            var users = _identityDbContext.Users.ToList();

            model.NewProjectTask = new ProjectVM.ProjectTaskVM
            {
                ProjectId = module.ProjectId,

                ProjectTaskPriorities = _context.ProjectTaskPriorities
                    .Select(x => new SelectListItem
                    {
                        Value = x.PriorityId.ToString(),
                        Text = x.PriorityName
                    }).ToList(),

                ProjectTaskStatus = _context.ProjectTaskStatuses
                    .Select(x => new SelectListItem
                    {
                        Value = x.StatusId.ToString(),
                        Text = x.StatusName
                    }).ToList(),

                UserList = users
                    .Where(x => x.IsActive)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.FullName
                    }).ToList(),

                ProjectModule = _context.ProjectModules
                    .Where(x => x.ProjectId == module.ProjectId)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ProjectModuleId.ToString(),
                        Text = x.ModuleName
                    }).ToList()
            };

            // =========================================
            // OPEN MODULE TAB + PANEL
            // =========================================

            TempData["ActiveTab"] = "tab-project-module";
            TempData["OpenModulePanel"] = "true";
            TempData["EditModule"] = "true";

            return View("ProjectDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectModule(ProjectAccount model)
        {
            ModelState.Remove("ProjectTasks");
            ModelState.Remove("ProjectTimeLogs");

            if (ModelState.IsValid)
            {
                await _projectService
                    .UpdateProjectModuleAsync(model.NewProjectModule);

                TempData["Success"] =
                    "Module Updated Successfully";

                TempData["ActiveTab"] =
                   "tab-project-module";


                return RedirectToAction(
                    "ProjectDetails",
                    new
                    {
                        area = "SalesCRM",
                        id = model.NewProjectModule.ProjectId
                    });


            }
            TempData["ActiveTab"] =
                "tab-project-module";

            TempData["OpenModulePanel"] =
                "true";

            TempData["EditModule"] =
                "true";

            return View("ProjectDetails", model);
        }

        //Edit - Update Pro Task 

        [HttpGet]
        public IActionResult EditProjectTask(Guid id)
        {
            // =========================================
            // GET TASK
            // =========================================

            var task = _projectService.GetProjectTaskById(id);

            if (task == null)
                return NotFound();

            // =========================================
            // GET PROJECT DETAILS
            // =========================================

            var model = _projectService
                .GetProjectDetails(task.ProjectId);

            if (model == null)
                return NotFound();

            // =========================================
            // LOAD TASKS
            // =========================================

            model.ProjectTasks = _projectService
                .GetTasksByProjectId(task.ProjectId)
                .ToList();

            // =========================================
            // LOAD MODULES
            // =========================================

            model.ProjectModules = _projectService
                .GetByProjectId(task.ProjectId)
                .ToList();

            // =========================================
            // LOAD EDIT TASK
            // =========================================

            var users = _identityDbContext.Users.ToList();

            task.ProjectTaskPriorities = _context.ProjectTaskPriorities
                .Select(x => new SelectListItem
                {
                    Value = x.PriorityId.ToString(),
                    Text = x.PriorityName
                }).ToList();

            task.ProjectTaskStatus = _context.ProjectTaskStatuses
                .Select(x => new SelectListItem
                {
                    Value = x.StatusId.ToString(),
                    Text = x.StatusName
                }).ToList();

            task.UserList = users
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                }).ToList();

            task.ProjectModule = _context.ProjectModules
                .Where(x => x.ProjectId == task.ProjectId)
                .Select(x => new SelectListItem
                {
                    Value = x.ProjectModuleId.ToString(),
                    Text = x.ModuleName
                }).ToList();

            model.NewProjectTask = task;

            // =========================================
            // OPEN TASK TAB
            // =========================================

            TempData["ActiveTab"] = "tab-project-task";
            TempData["OpenTaskPanel"] = "true";
            TempData["EditTask"] = "true";

            return View("ProjectDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectTask(ProjectAccount model)
        {
            ModelState.Remove("ProjectModules");
            ModelState.Remove("ProjectTasks");
            ModelState.Remove("ProjectTimeLogs");
            ModelState.Remove("NewProjectModule.ModuleName");
            ModelState.Remove("NewProjectModule.Description");
            ModelState.Remove("NewProjectModule.EstimatedHours");

            if (ModelState.IsValid)
            {
                await _projectService
                    .UpdateProjectTaskAsync(model.NewProjectTask);

                TempData["Success"] =
                    "Project Task Updated Successfully";

                TempData["ActiveTab"] = "tab-project-task";

                return RedirectToAction(
                    "ProjectDetails",
                    new
                    {
                        area = "SalesCRM",
                        id = model.NewProjectTask.ProjectId
                    });
            }

            TempData["ActiveTab"] = "tab-project-task";
            TempData["OpenTaskPanel"] = "true";
            TempData["EditTask"] = "true";

            return View("ProjectDetails", model);
        }


        #region Dhirendra  Code for Task Log

        [HttpGet]
        public async Task<IActionResult> ProjectTaskUtilization()
        {
            BSuit.SalesCRM.Models.ProjectTaskUtilizationVM model =
                new BSuit.SalesCRM.Models.ProjectTaskUtilizationVM();

            // Load dropdown list
            model.ProjectTasks = await _context.ProjectTasks
                .Select(x => new SelectListItem
                {
                    Value = x.ProjectTaskId.ToString(),
                    Text = x.TaskName
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectTimeLogs()
        {
            var users = await _identityDbContext.Users.ToListAsync();

            var result = await (
                from L in _context.ProjectTimeLogs

                //join U in users on L.UserId equals U.UserId into uJoin
                //from U in uJoin.DefaultIfEmpty()

                //join T in _context.ProjectTasks on L.ProjectTaskId equals T.ProjectTaskId

                //join P in _context.Projects on T.ProjectId equals P.ProjectId into pGroup
                //from P in pGroup.DefaultIfEmpty()

                orderby L.CreatedOn descending

                select new BSuit.SalesCRM.Models.TaskUtilizationListVM
                {
                    TimeLogId = L.TimeLogId,

                    WorkDate = L.WorkDate.Value.ToString("dd-MMM-yyyy"),
                    //EmployeeName = U != null  ? U.FullName : "",
                    Description = L.Description,
                    //TaskName = T.TaskName,
                    StartTime = L.StartTime.Value.ToString("hh:mm tt"),
                    EndTime = L.EndTime.Value.ToString("hh:mm tt"),
                    HoursSpent = L.HoursSpent.Value.ToString()
                }

            ).ToListAsync();

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateTimeLogs(IFormCollection frm)
        {
            try
            {
                // =========================
                // Parse TaskLogId
                // =========================
                Guid taskLogId = Guid.Empty;
                Guid.TryParse(frm["TaskLogId"], out taskLogId);

                // =========================
                // Parse ProjectTaskId
                // =========================
                Guid projectTaskId = Guid.Empty;
                Guid.TryParse(frm["ProjectTaskId"], out projectTaskId);

                // =========================
                // Parse WorkDate
                // =========================
                DateOnly workDate;

                if (!DateOnly.TryParse(frm["WorkDate"], out workDate))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Work Date."
                    });
                }

                // =========================
                // Convert HoursSpent to Seconds
                // Format: HH:mm
                // =========================
                string hoursSpentText = frm["HoursSpent"];

                int totalSeconds = 0;

                string[] parts = hoursSpentText.Split(':');

                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes))
                {
                    totalSeconds = (hours * 3600) + (minutes * 60);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid HoursSpent format."
                    });
                }

                // =========================
                // Parse Start & End Time
                // =========================
                DateTime startTime;
                DateTime endTime;

                if (!DateTime.TryParse(frm["StartTime"], out startTime))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Start Time."
                    });
                }

                if (!DateTime.TryParse(frm["EndTime"], out endTime))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid End Time."
                    });
                }

                // =========================
                // Insert / Update
                // =========================
                ProjectTimeLog opp;

                if (taskLogId != Guid.Empty)
                {
                    // UPDATE
                    opp = await _context.ProjectTimeLogs
                        .FirstOrDefaultAsync(x => x.TimeLogId == taskLogId);

                    if (opp == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Record not found."
                        });
                    }

                    opp.Description = frm["Description"];
                    opp.WorkDate = workDate;
                    opp.ProjectTaskId = projectTaskId;
                    opp.StartTime = startTime;
                    opp.EndTime = endTime;
                    opp.HoursSpent = totalSeconds;

                    _context.ProjectTimeLogs.Update(opp);

                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Record updated successfully."
                    });
                }
                else
                {
                    // INSERT
                    opp = new ProjectTimeLog
                    {
                        TimeLogId = Guid.NewGuid(),
                        Description = frm["Description"],
                        WorkDate = workDate,
                        ProjectTaskId = projectTaskId,
                        StartTime = startTime,
                        EndTime = endTime,
                        HoursSpent = totalSeconds
                    };

                    await _context.ProjectTimeLogs.AddAsync(opp);

                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Record added successfully."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        #endregion
    }
}


