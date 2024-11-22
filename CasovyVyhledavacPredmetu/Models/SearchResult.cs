using Ganss.Excel;

namespace CasovyVyhledavacPredmetuSIS.Models
{
    //TODO: Zatím nechávám vše ve stringu, aby nedocházelo k chybám jako u sloupce "Rozsah za akademický rok (navíc):", kde se vyskytují čísla s písmeny jako např. "16  [H]"
    //TODO: Chybí tu pár sloupců
    public class SearchResult 
    {
        [Column("Kód")]
        public string Code { get; set; }
        [Column("Název")]
        public string Name { get; set; }
        [Column("Fakulta")]
        public string Faculty { get; set; }
        [Column("Katedra")]
        public string Department { get; set; }
        [Column("Vyučován:")]
        public string IsTaught { get; set; } //TODO: Convert to enum or bool
        [Column("E-Kredity")]
        public string Credits { get; set; }
        [Column("Semestr")]
        public string Semester { get; set; } //TODO: Convert to enum
        [Column("Počet nekontaktních hodin")]
        public string NumberOfNonContactHours { get; set; } //TODO: Možná lepší překlad? Off campus hours? 
        [Column("Minimální obsazenost:")]
        public string MinimumOccupancy { get; set; }
        [Column("Max.počet studentů:")]
        public string MaximumNumberOfStudents { get; set; }
        [Column("Jazyk výuky:")]
        public string Language { get; set; }
        public List<TimetableLesson> Lessons { get; set; }
    }
}
