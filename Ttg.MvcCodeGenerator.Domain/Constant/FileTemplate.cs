namespace Ttg.MvcCodeGenerator.Domain.Constant
{
    public class FileTemplate
    {

        public static string UnitOfWork = @"
private GenericRepository<{objectName}> {objectNameFirstCharLower}Repository;
        
public GenericRepository<{objectName}> {objectName}Repository
{
    get { if (this.{objectNameFirstCharLower}Repository == null) { this.{objectNameFirstCharLower}Repository = new GenericRepository<{objectName}>(this.context); } return this.{objectNameFirstCharLower}Repository; }
}
";

        public static string Domain = @"
{namespaces}
namespace {rootNamespace}.Domain
{
	public class {objectName}
	{
{columns}
	}
}
";

        public static string AdminViewModel = @"
{namespaces}
namespace {rootNamespace}.Web.Areas.Admin.ViewModels
{
	public class {objectName}ViewModel
	{
{columns}
	}
}
";

        public static string AdminViewAdd = @"
@model {objectName}ViewModel

<h2 class=""portlet-title"">
    <u>Add {objectName}</u>
</h2>

@Html.Partial(""ValidationSummary"")

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()

{templateFields}
        
    @Html.Partial(""ButtonSpacer"")
    
    @Html.ButtonLink(""Back"", ""/admin/{pluralObjectNameFirstCharLower}"")
    @Html.ButtonSave()
}
";

        public static string AdminViewEdit = @"
@model {objectName}ViewModel

<h2 class=""portlet-title"">
    <u>Edit {objectName}</u>
</h2>

@Html.Partial(""ValidationSummary"")

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()      

{templateFields}
        
    @Html.Partial(""ButtonSpacer"")
    
    @Html.ButtonLink(""Back"", ""/admin/{pluralObjectNameFirstCharLower}"")
    @Html.ButtonSave()
}
";



        public static string AdminViewIndex = @"
@model {pluralObjectName}ViewModel

<div class=""portlet"">

    @Html.Partial(""ValidationSummary"")

    <h4 class=""portlet-title"">
        <u>{pluralObjectName}</u>
    </h4>

    <div class=""pull-right"">
        @Html.ButtonLink(""Add {objectName}"", ""/admin/{pluralObjectNameFirstCharLower}/add"")
    </div>
    <div class=""clearfix""></div>

    @Html.Partial(""ButtonSpacer"")

    <div class=""portlet-body"">

        @if (Model.{pluralObjectName}.Any())
        {
            <table class=""table"">
                <thead>
                    <tr>
                        <th>{objectName}</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    @foreach ({objectName} {objectNameFirstCharLower} in Model.{pluralObjectName})
                    {
                        <tr>
                            <td>@{objectNameFirstCharLower}.{objectName}Id</td>
                            <td>
                                <div class=""pull-right"">
                                    @Html.ButtonLink(""Edit"", ""/admin/{pluralObjectNameFirstCharLower}/edit/"" + {objectNameFirstCharLower}.{objectName}Id)
                                </div>

                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        }
        else
        {
            <text>No results found.</text>
        }
    </div>
</div>
";

        public static string AdminViewModelPlural = @"
using System.Collections.Generic;
using {rootNamespace}.Domain;

namespace {rootNamespace}.Web.Areas.Admin.ViewModels
{
	public class {pluralObjectName}ViewModel
	{
        public IEnumerable<{objectName}> {pluralObjectName} { get; set; }
	}
}
";


        public static string AdminController = @"
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using {rootNamespace}.Domain;
using {rootNamespace}.Service;
using {rootNamespace}.Web.Areas.Admin.ViewModels;

namespace {rootNamespace}.Web.Areas.Admin.Controllers
{
    public class {pluralObjectName}Controller : BaseController
    {
        private readonly I{objectName}Service {objectNameFirstCharLower}Service;

        public {pluralObjectName}Controller(I{objectName}Service {objectNameFirstCharLower}Service)
        {
            this.{objectNameFirstCharLower}Service = {objectNameFirstCharLower}Service;
        }

        [HttpGet]
        public ActionResult Index()
        {
            {pluralObjectName}ViewModel model = new {pluralObjectName}ViewModel();

            model.{pluralObjectName} = {objectNameFirstCharLower}Service.GetAll();

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            {objectName}ViewModel model = new {objectName}ViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Add({objectName}ViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            {objectName} {objectNameFirstCharLower} = new {objectName}();

            Mapper.Map(model, {objectNameFirstCharLower});                  

            {objectNameFirstCharLower}Service.Add({objectNameFirstCharLower});

            TempData[Domain.Constant.TempData.Confirmation] = Domain.Constant.ValidationMessage.SuccessfullyAdded;

            return RedirectToAction(""Index"");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {            
            {objectName}ViewModel model = new {objectName}ViewModel();

            {objectName} {objectNameFirstCharLower} = {objectNameFirstCharLower}Service.GetById(id);

            Mapper.Map({objectNameFirstCharLower}, model);          

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit({objectName}ViewModel model)
        {
            if (ModelState.IsValid == false)
            {

                return View(model);
            }

            {objectName} {objectNameFirstCharLower} = {objectNameFirstCharLower}Service.GetById(model.{objectName}Id);

            Mapper.Map(model, {objectNameFirstCharLower});

            {objectNameFirstCharLower}Service.Save({objectNameFirstCharLower});

            TempData[Domain.Constant.TempData.Confirmation] = Domain.Constant.ValidationMessage.SuccessfullySaved;

            return RedirectToAction(""Index"");
        }
	}
}";



        public static string Service = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using {rootNamespace}.Data;
using {rootNamespace}.Domain;
using {rootNamespace}.Domain.Enumeration;

namespace {rootNamespace}.Service
{
    public interface I{objectName}Service
    {
        {objectName} GetById(int {objectNameFirstCharLower}Id);
        void Add({objectName} {objectNameFirstCharLower});
        void Delete({objectName} {objectNameFirstCharLower});
        void Save({objectName} {objectNameFirstCharLower});
        IEnumerable<{objectName}> GetAll();
    }

    public class {objectName}Service : I{objectName}Service
    {
        {redisDefinition}

        public IEnumerable<{objectName}> GetAll()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                return uow.{objectName}Repository.Get.ToList();
            }
        }

        {getById}
        {getByIdRedis}

        public void Add({objectName} {objectNameFirstCharLower})
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                {createdDate}
                {statusId}
                uow.{objectName}Repository.Add({objectNameFirstCharLower});

                uow.SaveChanges();

                {addRedis}
            }
        }

        public void Delete({objectName} {objectNameFirstCharLower})
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.{objectName}Repository.Delete({objectNameFirstCharLower});

                uow.SaveChanges();
            }
        }

        public void Save({objectName} {objectNameFirstCharLower})
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.{objectName}Repository.Update({objectNameFirstCharLower});              

                uow.SaveChanges();

                {saveRedis}
            }
        }
    }
}
";

        public static string RedisDefinition = @"
        private readonly IRedisService redisService;

        public {objectName}Service(IRedisService redisService)
        {
            this.redisService = redisService;
        }
";
        public static string GetById = @"
        public {objectName} GetById(int {objectName}Id)
        {
            return uow.{objectName}Repository.Find({objectName}Id);
        }
";

        public static string GetByIdRedis = @"
        public {objectName} GetById(int {objectNameFirstCharLower}Id)
        {
            {objectName} {objectNameFirstCharLower} = new {objectName}();

            if (redisService.GetValue<{objectName}>({objectNameFirstCharLower}Id.ToString(), out {objectNameFirstCharLower}) == false)
            {
                using (UnitOfWork uow = new UnitOfWork())
                {                    
                    {objectNameFirstCharLower} = uow.{objectName}Repository.Find({objectNameFirstCharLower}Id);
                    redisService.SetEntry({objectNameFirstCharLower});
                }
            }

            return {objectNameFirstCharLower};
        }
";
    }
}
