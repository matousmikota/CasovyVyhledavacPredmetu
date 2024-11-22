using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasovyVyhledavacPredmetuSIS.Models
{
    public class SearchForm
    {
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string ClassName { get; set; }
        public string Classification { get; set; }
        public string FourEUPlus { get; set; }
        public string VirtualMobility { get; set; }
        public string[] Competencies { get; set; }
        public string Semester { get; set; }
        public string ShowAmount { get; set; }

        public string SubjectName { get; set; }
        public string TeacherName { get; set; }

        public bool Guarantors { get; set; }
        public bool Teachers { get; set; }
        public bool GuarantorsAndTeachers { get; set; }
        public bool Exact { get; set; }
        public bool Substring { get; set; }

        //TODO: Implement these filters in BrowserInteracter.cs
        public bool SearchInName { get; set; }
        public bool SearchInAnnotation { get; set; }
        public bool SearchInSylabus { get; set; }

        public static void SetDefaultValues()
        {

        }
    }
}
