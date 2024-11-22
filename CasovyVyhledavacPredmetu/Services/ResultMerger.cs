using CasovyVyhledavacPredmetuSIS.Models;
using Ganss.Excel;

namespace CasovyVyhledavacPredmetuSIS.Services
{
    public class ResultMerger(DirectoryInfo directory)
    {
        private DirectoryInfo Directory { get; set; } = directory;
        private List<SearchResult> ReadXLS(FileInfo file)
        {
            return new ExcelMapper(file.FullName).Fetch<SearchResult>().ToList();
        }
        public List<SearchResult> MergeFilesInFolder()
        {
            List<SearchResult> results = [];

            foreach (FileInfo file in Directory.EnumerateFiles())
            {
                if (file.Extension == ".xls")
                {
                    results.AddRange(ReadXLS(file));
                }
            }

            return results;
        }
    }
}