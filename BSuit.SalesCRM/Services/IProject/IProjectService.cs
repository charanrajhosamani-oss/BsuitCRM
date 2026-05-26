using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSuit.SalesCRM.Models.ProjectVM;

namespace BSuit.SalesCRM.Services.Project
{
   
    public interface IProjectService
    {
        List<ProjectVM> GetAllProjects();
        ProjectVM GetById(Guid id);
        void UpdateProject(ProjectVM dto);
        void DeleteProject(Guid id);
        Task AddAsync(ProjectTaskVM model);

        List<ProjectTaskVM> GetTasksByProjectId(Guid projectId);
        ProjectAccount GetProjectDetails(Guid projectId);
        //Edit - update pro Module
        ProjectVM.ProjectModuleVM GetProjectModuleById(Guid id);
        Task UpdateProjectModuleAsync(ProjectVM.ProjectModuleVM model);
        //Edit - update Pro Task

        ProjectVM.ProjectTaskVM GetProjectTaskById(Guid id);

        Task UpdateProjectTaskAsync(ProjectVM.ProjectTaskVM model);
        Task AddAsync(ProjectModuleVM model);

        List<ProjectModuleVM> GetByProjectId(Guid projectId);

    }
}
