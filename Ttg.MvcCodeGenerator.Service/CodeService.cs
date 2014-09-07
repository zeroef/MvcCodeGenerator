using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Ttg.MvcCodeGenerator.Domain;
using Ttg.MvcCodeGenerator.Domain.Enumeration;

namespace Ttg.MvcCodeGenerator.Service
{
    public class CodeService
    {
        public DatabaseSchemeService databaseSchemeService = new DatabaseSchemeService();
        public FileService fileService = new FileService();

        public string GetUnitOfWorkCode(string singularObject)
        {
            string file = Domain.Constant.FileTemplate.UnitOfWork;

            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));

            return file;
        }

        public void GenerateService(string connectionString, string singularObject, string pluralObject, string rootNamespace, bool enableRedis, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            string file = Domain.Constant.FileTemplate.Service;

            string columnList = "";

            foreach (TableColumn column in columns)
            {
                columnList += "\t\tpublic " + GetDataType(column.Type.Name) + " " + column.Name + " { get; set; }\n";
            }

            if (enableRedis == true)
            {
                file = file.Replace("{redisDefinition}", Domain.Constant.FileTemplate.RedisDefinition);
                file = file.Replace("{addRedis}", "redisService.SetEntry({objectNameFirstCharLower});");
                file = file.Replace("{saveRedis}", "redisService.SetEntry({objectNameFirstCharLower});");
                file = file.Replace("{getByIdRedis}", Domain.Constant.FileTemplate.GetByIdRedis);
                file = file.Replace("{getById}", "");
            }
            else
            {
                file = file.Replace("{getById}", Domain.Constant.FileTemplate.GetById);
                file = file.Replace("{getByIdRedis}", "");
                file = file.Replace("{redisDefinition}", "");
                file = file.Replace("{addRedis}", "");
                file = file.Replace("{saveRedis}", "");
            }

            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));
            file = file.Replace("{columns}", columnList);

            if (columns.Where(w => w.Name == "Created").Any() == true)
            {
                file = file.Replace("{createdDate}", FirstLetterToLower(singularObject) + ".Created = DateTime.Now;\n");
            }
            else
            {
                file = file.Replace("{createdDate}", "");
            }

            if (columns.Where(w => w.Name == "StatusId").Any() == true)
            {
                file = file.Replace("{statusId}", FirstLetterToLower(singularObject) + ".StatusId = (int)StatusType.Active;\n");
            }
            else
            {
                file = file.Replace("{statusId}", "");
                file = file.Replace(string.Format("using {0}.Domain.Enumeration;", rootNamespace), "");
            }

            fileService.CreateFile(ProjectFileType.Service, webconfig, singularObject, pluralObject, file, overwriteExisting);

            fileService.ModifyNinjectWebCommon(webconfig, singularObject);
        }

        public void GenerateDomain(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            string columnList = "";

            foreach (TableColumn column in columns)
            {
                columnList += "\t\tpublic " + GetDataType(column.Type.Name) + " " + column.Name + " { get; set; }\n";
            }

            string namespaces = "";

            if (columns.Where(w => w.Type.Name.Contains("DateTime")).Any())
            {
                namespaces += "using System;\n";
            }

            string file = Domain.Constant.FileTemplate.Domain;

            file = file.Replace("{namespaces}", namespaces);
            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{columns}", columnList);

            fileService.CreateFile(ProjectFileType.Domain, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        public void GenerateAdminViewModel(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            string columnList = "";

            foreach (TableColumn column in columns)
            {
                columnList += "\t\tpublic " + GetDataType(column.Type.Name) + " " + column.Name + " { get; set; }\n";
            }

            string namespaces = "";

            if (columns.Where(w => w.Type.Name.Contains("DateTime")).Any())
            {
                namespaces += "using System;\n";
            }

            string file = Domain.Constant.FileTemplate.AdminViewModel;

            file = file.Replace("{namespaces}", namespaces);
            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{columns}", columnList);

            fileService.CreateFile(ProjectFileType.AdminViewModel, webconfig, singularObject, pluralObject, file, overwriteExisting);

            fileService.ModifyMapperProfile(webconfig, singularObject);
        }

        public void GenerateAdminViewIndex(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }
            
            string file = Domain.Constant.FileTemplate.AdminViewIndex;

            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{pluralObjectName}", pluralObject);
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));
            file = file.Replace("{pluralObjectNameFirstCharLower}", FirstLetterToLower(pluralObject));

            fileService.CreateFile(ProjectFileType.AdminViewIndex, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        public void GenerateAdminViewAdd(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }
            
            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            List<string> excludedColmums = new List<string>();
            excludedColmums.Add(singularObject + "Id");
            excludedColmums.Add("Created");
            excludedColmums.Add("StatusId");

            string templateFields = "";

            foreach (TableColumn column in columns)
            {
                if (!excludedColmums.Contains(column.Name))
                {
                    templateFields += string.Format("\t@Html.{0}(m => m.{1})\n", GetEditorType(column), column.Name);    
                }
            }

            string file = Domain.Constant.FileTemplate.AdminViewAdd;

            
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{templateFields}", templateFields);                        
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));
            file = file.Replace("{pluralObjectNameFirstCharLower}", FirstLetterToLower(pluralObject));

            fileService.CreateFile(ProjectFileType.AdminViewAdd, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        public void GenerateAdminViewEdit(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            List<string> excludedColmums = new List<string>();
            excludedColmums.Add("Created");

            string templateFields = "";

            foreach (TableColumn column in columns)
            {
                if (!excludedColmums.Contains(column.Name))
                {
                    if (column.Name == singularObject + "Id")
                    {
                        templateFields += string.Format("\t@Html.HiddenFor(m => m.{0})\n", column.Name);
                    }
                    else
                    {
                        templateFields += string.Format("\t@Html.{0}(m => m.{1})\n", GetEditorType(column), column.Name);
                    }
                }
            }

            string file = Domain.Constant.FileTemplate.AdminViewAdd;


            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{templateFields}", templateFields);
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));
            file = file.Replace("{pluralObjectNameFirstCharLower}", FirstLetterToLower(pluralObject));

            fileService.CreateFile(ProjectFileType.AdminViewEdit, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        public void GenerateAdminController(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            string columnList = "";

            foreach (TableColumn column in columns)
            {
                columnList += "\t\tpublic " + GetDataType(column.Type.Name) + " " + column.Name + " { get; set; }\n";
            }

            string file = Domain.Constant.FileTemplate.AdminController;

            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{pluralObjectName}", pluralObject);
            file = file.Replace("{columns}", columnList);
            file = file.Replace("{objectNameFirstCharLower}", FirstLetterToLower(singularObject));

            fileService.CreateFile(ProjectFileType.AdminController, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        public void GenerateAdminViewModelPlural(string connectionString, string singularObject, string rootNamespace, string pluralObject, bool overwriteExisting, string webconfig)
        {
            if (String.IsNullOrEmpty(connectionString) == true || String.IsNullOrEmpty(singularObject) || String.IsNullOrEmpty(rootNamespace))
            {
                return;
            }

            List<TableColumn> columns = databaseSchemeService.GetTableColumns(connectionString, pluralObject);

            string columnList = "";

            foreach (TableColumn column in columns)
            {
                columnList += "\t\tpublic " + GetDataType(column.Type.Name) + " " + column.Name + " { get; set; }\n";
            }

            string file = Domain.Constant.FileTemplate.AdminViewModelPlural;

            file = file.Replace("{rootNamespace}", rootNamespace);
            file = file.Replace("{objectName}", singularObject);
            file = file.Replace("{pluralObjectName}", pluralObject);

            fileService.CreateFile(ProjectFileType.AdminViewModelPlural, webconfig, singularObject, pluralObject, file, overwriteExisting);
        }

        private string GetDataType(string type)
        {
            if (type == "Int32")
            {
                return "int";
            }

            if (type == "String")
            {
                return "string";
            }

            return type;
        }

        private string GetEditorType(TableColumn column)
        {
            switch (column.Type.Name)
            {
                case "DateTime":
                    return "FormInputDate";
                case "String":
                    return "FormInputText";
                case "Int32":
                    if (column.Name == "StatusId"){ return "FormStatus"; }

                    return "FormInputInt";
                default:
                    return column.Type.Name;
            }
        }

        private string FirstLetterToLower(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);

            return str.ToLower();
        }
    }
}
