using Ttg.MvcCodeGenerator.Domain.Enumeration;

namespace Ttg.MvcCodeGenerator.Domain
{
    public class ProjectFile
    {
        public string Filename { get; set; }
        public string Directory { get; set; }
        public string FullPath { get { return Directory + Filename; } }
        public bool Exists { get; set; }
        public string Contents { get; set; }
        public bool Overwriteable { get; set; }
        public ProjectFileType ProjectFileType { get; set; }
    }
}
