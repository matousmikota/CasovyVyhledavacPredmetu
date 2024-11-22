using nietras.SeparatedValues;
using CasovyVyhledavacPredmetuSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasovyVyhledavacPredmetuSIS.Services
{
    public class TimetableMerger
    {
        public List<TimetableLesson> Merge(string filePath)
        {
            using var reader = Sep.Reader().FromFile(filePath);

            List<TimetableLesson> lessons = [];

            foreach (SepReader.Row readRow in reader)
            {
                lessons.Add(
                    new TimetableLesson()
                    {
                        Code = readRow["kod predmetu"].ToString(),
                        Name = readRow["nazev"].ToString(),
                        DayNumber = readRow["den(1=po)"].ToString(),
                        StartTime = readRow["cas(min od 0:00)"].ToString(),
                        Room = readRow["mistnost"].ToString(),
                        Length = readRow["delka(min)"].ToString(),
                        NumberOfLectureWeeks = readRow["pocet tydnu vyuky"].ToString(),
                        BiWeeklyLectures = readRow["ctrnactideni vyuka"].ToString(),
                        Teachers = readRow["ucitele"].ToString()
                    });
            }

            return lessons;
        }
    }
}
