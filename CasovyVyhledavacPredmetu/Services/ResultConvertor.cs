using Flurl;
using CasovyVyhledavacPredmetuSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Url = Flurl.Url;

namespace CasovyVyhledavacPredmetuSIS.Services
{
    /// <summary>
    /// Převedení SearchResult objektů na odkazy stažení rozvrhu předmetu jako CSV
    /// </summary>
    public class ResultConvertor
    {
        private readonly string baseSubjectLink = "https://is.cuni.cz/studium/rozvrhng/roz_predmet_macro.php";
        public Url ConvertToCSVLink(SearchResult subject, string faculty)
        {
            var queryParameters = new
            {
                predmet = subject.Code,
                fak = faculty,
                csv = 1
            };

            return new Url(baseSubjectLink).SetQueryParams(queryParameters);
        }
        public List<Url> ConvertToCSVLink(List<SearchResult> subjects, string faculty)
        {
            List<Url> urls = [];

            foreach (SearchResult subject in subjects)
            {
                urls.Add(ConvertToCSVLink(subject, faculty));
            }

            return urls;
        }
    }
}
