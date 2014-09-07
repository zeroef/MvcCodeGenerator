using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MvcCodeGenerator.ViewModels;
using Ttg.MvcCodeGenerator.Domain;
using Ttg.MvcCodeGenerator.Domain.Enumeration;
using Ttg.MvcCodeGenerator.Service;

namespace MvcCodeGenerator.Controllers
{
    public class HomeController : Controller
    {
        public DatabaseSchemeService databaseSchemeService = new DatabaseSchemeService();
        public CodeService codeService = new CodeService();
        public FileService fileService = new FileService();

        [HttpGet]
        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            model.ConnectionStrings = databaseSchemeService.GetConnectionStrings(model.WebConfig);
            model.DatabaseTables = databaseSchemeService.GetDatabaseTables(model.ConnectionString);

            model.ProjectFiles = new List<ProjectFile>();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel model)
        {
            model.ConnectionStrings = databaseSchemeService.GetConnectionStrings(model.WebConfig);
            model.DatabaseTables = databaseSchemeService.GetDatabaseTables(model.ConnectionString);
            
            model.ProjectFiles = fileService.GetProjectFiles(model.WebConfig, GetSingularObject(model.DatabaseTable), model.DatabaseTable);
            
            model.RootNamespace = GetRootNamespace(model.WebConfig);
            model.DomainLocation = model.ProjectFiles.FirstOrDefault(w => w.ProjectFileType == ProjectFileType.Domain).Directory;
            model.ServiceLocation = model.ProjectFiles.FirstOrDefault(w => w.ProjectFileType == ProjectFileType.Service).Directory;
            model.WebAdminViewModelLocation = model.ProjectFiles.FirstOrDefault(w => w.ProjectFileType == ProjectFileType.AdminViewModel).Directory;
            model.WebAdminControllerLocation = model.ProjectFiles.FirstOrDefault(w => w.ProjectFileType == ProjectFileType.AdminController).Directory;
            model.WebAdminViewsLocation = model.ProjectFiles.FirstOrDefault(w => w.ProjectFileType == ProjectFileType.AdminViewAdd).Directory;

            model.SingularObject = GetSingularObject(model.DatabaseTable);
            model.PluralObject = model.DatabaseTable;

            model.UnitOfWorkCode = codeService.GetUnitOfWorkCode(model.SingularObject);
                
            if (model.CreateDomain){ codeService.GenerateDomain(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteDomain, model.WebConfig); }
            if (model.CreateService){ codeService.GenerateService(model.ConnectionString, model.SingularObject, model.PluralObject, model.RootNamespace, model.EnableRedis, model.OverwriteService, model.WebConfig); }
            if (model.CreateAdminViewModel){codeService.GenerateAdminViewModel(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminViewModel, model.WebConfig); }
            if (model.CreateAdminController){codeService.GenerateAdminController(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminController, model.WebConfig); }
            if (model.CreateAdminViewPlural ){codeService.GenerateAdminViewModelPlural(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminViewPlural, model.WebConfig);}
            if (model.CreateAdminViewIndex) { codeService.GenerateAdminViewIndex(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminViewIndex, model.WebConfig); }
            if (model.CreateAdminViewAdd) { codeService.GenerateAdminViewAdd(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminViewAdd, model.WebConfig); }
            if (model.CreateAdminViewEdit) { codeService.GenerateAdminViewEdit(model.ConnectionString, model.SingularObject, model.RootNamespace, model.PluralObject, model.OverwriteAdminViewEdit, model.WebConfig); }
            
            return View(model);
        }

        public string GetSingularObject(string databaseTable)
        {
            if (String.IsNullOrEmpty(databaseTable) == true)
            {
                return null;
            }

            PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

            return pluralizationService.Singularize(databaseTable);
        }

        private string GetRootNamespace(string webConfig)
        {
            webConfig = webConfig.Substring(0, webConfig.LastIndexOf("\\"));

            return webConfig.Substring(webConfig.LastIndexOf("\\") + 1).Replace(".Web", "");
        }
    }
}