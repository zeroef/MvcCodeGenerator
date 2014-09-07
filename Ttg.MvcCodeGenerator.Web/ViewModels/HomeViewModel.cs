using System.Collections.Generic;
using System.Collections.Specialized;
using Ttg.MvcCodeGenerator.Domain;

namespace MvcCodeGenerator.ViewModels
{
    public class HomeViewModel
    {
        public string WebConfig { get; set; }
        public NameValueCollection ConnectionStrings { get; set; }
        public string ConnectionString { get; set; }
        public IEnumerable<string> DatabaseTables { get; set; }
        public string SingularObject { get; set; }
        public string PluralObject { get; set; }
        public string DatabaseTable { get; set; }
        public string RootNamespace { get; set; }
        public string ServiceLocation { get; set; }
        public string DomainLocation { get; set; }
        public string WebAdminViewModelLocation { get; set; }
        public string WebAdminViewsLocation { get; set; }
        public string WebAdminControllerLocation { get; set; }
        public bool EnableRedis { get; set; }
        public bool ActuallyGenerateCode { get; set; }
        public IEnumerable<ProjectFile> ProjectFiles { get; set; }
        public string UnitOfWorkCode { get; set; }

        public bool OverwriteDomain { get; set; }
        public bool OverwriteService { get; set; }
        public bool OverwriteAdminController { get; set; }
        public bool OverwriteAdminViewAdd { get; set; }
        public bool OverwriteAdminViewIndex { get; set; }
        public bool OverwriteAdminViewModel { get; set; }
        public bool OverwriteAdminViewPlural { get; set; }
        public bool OverwriteAdminViewEdit { get; set; }

        public bool CreateDomain { get; set; }
        public bool CreateService { get; set; }
        public bool CreateAdminController { get; set; }
        public bool CreateAdminViewAdd { get; set; }
        public bool CreateAdminViewIndex { get; set; }
        public bool CreateAdminViewModel { get; set; }
        public bool CreateAdminViewPlural { get; set; }
        public bool CreateAdminViewEdit { get; set; }

    }
}