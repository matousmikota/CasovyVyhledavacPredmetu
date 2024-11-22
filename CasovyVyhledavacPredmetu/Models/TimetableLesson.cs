using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//use csv serilization library
//pouzit sep, je to pry hodne quick


namespace CasovyVyhledavacPredmetuSIS.Models
{
    public class TimetableLesson
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string DayNumber { get; set; }
        public string StartTime { get; set; }
        public string Room { get; set; }
        public string Length { get; set; }
        public string FirstLectureWeek { get; set; }
        public string NumberOfLectureWeeks { get; set; }
        public string BiWeeklyLectures { get; set; }
        public string Teachers { get; set; }
    }
}
