using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Ttg.MvcCodeGenerator.Domain;
using Ttg.MvcCodeGenerator.Domain.Enumeration;

namespace Ttg.MvcCodeGenerator.Service
{
    public class FileService
    {

        public void CreateFile(ProjectFileType projectFileType, string webConfig, string singularObject, string pluralObject, string contents, bool overwriteExisting)
        {
            if (CreateFile(
                GetLocation(projectFileType, webConfig, singularObject, pluralObject),
                GetFileName(projectFileType, singularObject, pluralObject),
                contents,
                overwriteExisting))
            {
                webConfig = webConfig.Substring(0, webConfig.LastIndexOf("\\"));
                switch (projectFileType)
                {
                    case ProjectFileType.AdminController:
                    case ProjectFileType.AdminViewModel:
                    case ProjectFileType.AdminViewModelPlural:
                        AddCodeFileToXML(webConfig, GetFileName(projectFileType, singularObject, pluralObject), GetLocation(projectFileType, "\\", singularObject, pluralObject), NodeType.Compile);
                        break;
                    case ProjectFileType.Domain:
                        AddCodeFileToXML(webConfig.Replace(".Web", "") + ".Domain", GetFileName(projectFileType, singularObject, pluralObject), "", NodeType.Compile);
                        break;
                    case ProjectFileType.Service:
                        AddCodeFileToXML(webConfig.Replace(".Web", "") + ".Service", GetFileName(projectFileType, singularObject, pluralObject), "", NodeType.Compile);
                        break;
                    case ProjectFileType.AdminViewAdd:
                    case ProjectFileType.AdminViewEdit:
                    case ProjectFileType.AdminViewIndex:
                        AddCodeFileToXML(webConfig, GetFileName(projectFileType, singularObject, pluralObject), GetLocation(projectFileType, "\\", singularObject, pluralObject), NodeType.Content);
                        break;
                }                    
            };
        }
        
        private bool CreateFile(string location, string filename, string contents, bool overwriteExisting)
        {
            if (FileExists(location, filename) && !overwriteExisting){ return false; }

            CreateDirectory(location);

            System.IO.File.WriteAllText(location + filename, contents);

            return true;
        }

        public bool FileExists(string location, string filename)
        {
            return System.IO.File.Exists(location + filename);
        }

        private void CreateDirectory(string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
        }

        public IEnumerable<ProjectFile> GetProjectFiles(string webConfig, string singularObject, string pluralObject)
        {
            List<ProjectFile> projectFiles = new List<ProjectFile>();

            foreach (ProjectFileType projectFileType in Enum.GetValues(typeof (ProjectFileType)).Cast<ProjectFileType>())
            {
                ProjectFile projectFile = new ProjectFile();
                projectFile.Filename = GetFileName(projectFileType, singularObject, pluralObject);
                projectFile.Directory = GetLocation(projectFileType, webConfig, singularObject, pluralObject);
                projectFile.Exists = FileExists(projectFile.Directory, projectFile.Filename);
                projectFile.Contents = "";               
                projectFile.Overwriteable = false;
                projectFile.ProjectFileType = projectFileType;

                projectFiles.Add(projectFile);
            }

            return projectFiles;
        }

        private string GetLocation(ProjectFileType projectFileType, string webConfig, string singularObject, string pluralObject)
        {
            webConfig = webConfig.Substring(0, webConfig.LastIndexOf("\\"));

            switch (projectFileType)
            {
                case ProjectFileType.AdminController:
                    return webConfig + "\\Areas\\Admin\\Controllers\\";
                case ProjectFileType.AdminViewAdd:
                case ProjectFileType.AdminViewEdit:
                case ProjectFileType.AdminViewIndex:
                    return webConfig + "\\Areas\\Admin\\Views\\" + pluralObject + "\\";
                case ProjectFileType.AdminViewModel:
                case ProjectFileType.AdminViewModelPlural:
                    return webConfig + "\\Areas\\Admin\\ViewModels\\";
                case ProjectFileType.Domain:
                    return webConfig.Replace(".Web", "") + ".Domain\\";
                case ProjectFileType.Service:
                    return webConfig.Replace(".Web", "") + ".Service\\";
                default:
                    throw new Exception("Unknown Project File Type location.");
            }
        }

        private string GetFileName(ProjectFileType projectFileType, string singularObject, string pluralObject)
        {
            switch (projectFileType)
            {
                case ProjectFileType.AdminController:
                    return string.Format("{0}Controller.cs", pluralObject);
                
                case ProjectFileType.AdminViewAdd:
                    return "Add.cshtml";
                
                case ProjectFileType.AdminViewIndex:
                    return "Index.cshtml";
                
                case ProjectFileType.AdminViewModel:
                    return string.Format("{0}ViewModel.cs", singularObject);
                
                case ProjectFileType.AdminViewModelPlural:
                    return string.Format("{0}ViewModel.cs", pluralObject);
                
                case ProjectFileType.Domain:
                    return string.Format("{0}.cs", singularObject);
                
                case ProjectFileType.Service:
                    return string.Format("{0}Service.cs", singularObject);

                case ProjectFileType.AdminViewEdit:
                    return "Edit.cshtml";
                
                default:
                    throw new Exception(string.Format("No file name defined for this ProjectFileType ({0})", projectFileType));
            }


        }

        public IEnumerable<string> GetServices(string location)
        {
            IEnumerable<string> files = Directory.GetFiles(location, "*Service.cs").OrderBy(o => o);

            List<string> fileNames = new List<string>();
            foreach (string file in files)
            {
                fileNames.Add(Path.GetFileName(file).Replace(".cs", ""));
            }
            return fileNames;
        }

        private string GetVisualStudioProjectFile(string webConfig)
        {
            return webConfig.Substring(webConfig.LastIndexOf("\\")) + ".csproj";
        }

        private void AddCodeFileToXML(string webConfig, string addedFile, string location, NodeType nodeType)
        {
            string visualStudioProjectFile = GetVisualStudioProjectFile(webConfig);
            string newNodeValue = location + addedFile;
            if (newNodeValue.Substring(0, 1) == "\\")
            {
                newNodeValue = newNodeValue.Substring(1);
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(webConfig + visualStudioProjectFile);

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode firstCompileNode = doc.SelectSingleNode("/x:Project/x:ItemGroup/x:" + nodeType, mgr);
            XmlNode itemGroupNode = firstCompileNode.ParentNode;
            bool nodeExists = false;

            foreach (XmlNode childNode in itemGroupNode.ChildNodes)
            {
                XmlAttribute includeAttribute = childNode.Attributes["Include"];

                if (includeAttribute != null)
                {
                    if (includeAttribute.Value == newNodeValue)
                    {
                        nodeExists = true;
                        break;
                    }
                }
            }
            if (!nodeExists)
            {
                XmlNode newCompileNode = doc.CreateNode(XmlNodeType.Element, nodeType.ToString(), "http://schemas.microsoft.com/developer/msbuild/2003");
                XmlAttribute newCompileNodeAttribute = doc.CreateAttribute("Include");
                newCompileNodeAttribute.Value = newNodeValue;
                newCompileNode.Attributes.Append(newCompileNodeAttribute);

                itemGroupNode.AppendChild(newCompileNode);

                doc.Save(webConfig + visualStudioProjectFile);
            }
        }

        public void ModifyMapperProfile(string webConfig, string singularObject)
        {
            webConfig = webConfig.Substring(0, webConfig.LastIndexOf("\\"));
            string createMap = @"Mapper.CreateMap<{objectName}ViewModel, {objectName}>().ReverseMap();";
            createMap = createMap.Replace("{objectName}", singularObject);

            string[] mapperProfile = File.ReadAllLines(webConfig + "\\Code\\AutoMapper\\MapperProfile.cs");

            if (mapperProfile.Any(s => s.Contains(createMap))){ return; }

            StringBuilder sb = new StringBuilder();
            bool foundConfigure = false;
            foreach (string line in mapperProfile)
            {
                sb.AppendLine(line);
                if (line.Contains("protected override void Configure()"))
                {
                    foundConfigure = true;
                }
                if (foundConfigure && line.Contains("{"))
                {
                    sb.AppendLine("\t\t\t" + createMap);
                    foundConfigure = false;
                }
            }

            System.IO.File.WriteAllText(webConfig + "\\Code\\AutoMapper\\MapperProfile.cs", sb.ToString());
        }

        public void ModifyNinjectWebCommon(string webConfig, string singularObject)
        {
            webConfig = webConfig.Substring(0, webConfig.LastIndexOf("\\"));
            webConfig = webConfig.Replace(".Service", ".Web");
            
            string kernalBind = @"kernel.Bind<I{objectName}Service>().To<{objectName}Service>();";
            kernalBind = kernalBind.Replace("{objectName}", singularObject);

            string[] ninjectWebCommon = File.ReadAllLines(webConfig + "\\App_Start\\NinjectWebCommon.cs");

            if (ninjectWebCommon.Any(s => s.Contains(kernalBind))) { return; }

            StringBuilder sb = new StringBuilder();
            bool foundRegisterServices = false;
            foreach (string line in ninjectWebCommon)
            {
                sb.AppendLine(line);
                if (line.Contains("private static void RegisterServices(IKernel kernel)"))
                {
                    foundRegisterServices = true;
                }
                if (foundRegisterServices && line.Contains("{"))
                {
                    sb.AppendLine("\t\t\t" + kernalBind);
                    foundRegisterServices = false;
                }
            }

            System.IO.File.WriteAllText(webConfig + "\\App_Start\\NinjectWebCommon.cs", sb.ToString());
        }
    }
}
