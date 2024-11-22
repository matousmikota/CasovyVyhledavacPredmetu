using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.ComponentModel;
using CasovyVyhledavacPredmetuSIS.Models;
using Flurl;

namespace CasovyVyhledavacPredmetuSIS.Services
{
    /// <summary>
    /// Třída pro interakci s webovým rozhraním. Pro sestavení použij BuildBrowserInteracterAsync(). A pro spuštení výškrabu použij Scrape().
    /// </summary>
    public class BrowserInteracter
    {
        private IPage Page { get; set; }
        private DirectoryInfo TemporaryDownloads { get; set; }
        private SearchForm SearchForm { get; set; }
        private readonly string mainPageUrl = "https://is.cuni.cz/studium/index.php";
        private readonly string subjectsPageUrl = "https://is.cuni.cz/studium/predmety/";

        // "nahrazení" konstruktoru kvůli async
        public static async Task<BrowserInteracter> BuildBrowserInteracterAsync(SearchForm searchForm)
        {
            string directoryName = Path.GetRandomFileName();
            DirectoryInfo temporaryDownloads = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), directoryName));

            using IPlaywright playwright = await Playwright.CreateAsync();
            await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                DownloadsPath = temporaryDownloads.FullName,
                Headless = false,
            });
            IBrowserContext context = await browser.NewContextAsync();
            IPage page = await context.NewPageAsync();

            return new BrowserInteracter(page, temporaryDownloads, searchForm);
        }
        private BrowserInteracter(IPage page, DirectoryInfo temporaryDownloads, SearchForm searchForm)
        {
            Page = page;
            TemporaryDownloads = temporaryDownloads;
            SearchForm = searchForm;
        }
        public void Scrape()
        {
            List<Task> tasks = ShowResults();
            tasks.Add(GetCSVfromResults());
            tasks.Add(ConvertDownloadedResults());

            Task.WaitAll([.. tasks]);
        }
        public async Task ConvertDownloadedResults()
        {
            ResultMerger merger = new(TemporaryDownloads);
            List<SearchResult> results = merger.MergeFilesInFolder();

            ResultConvertor convertor = new();
            List<Url> urls = convertor.ConvertToCSVLink(results, SearchForm.Faculty);

            foreach (Url url in urls)
            {
                await NavPage(url); //Maybe just yolo access the links?
            }
        }
        /// <summary>
        /// Přesměrování na stránku
        /// </summary>
        private async Task NavPage(string url)
        {
            try
            {
                IResponse? response = await Page.GotoAsync(url);

                if (response == null)
                {
                    ErrorHandler.LogAndThrow($"Žádná odpověď od {url}", new HttpRequestException());
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogAndThrow($"Neúspěšné přesměrování na {url}: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Kliknutí na tlačítko předměty na hlavní stránce SIS, pokud tam prohlížeč není, tak se tam přesměruje
        /// </summary>
        private async Task ClickSubjects()
        {
            if (Page.Url != mainPageUrl)
            {
                await NavPage(mainPageUrl);
            }

            await Page.GetByRole(AriaRole.Link, new() { Name = "Předměty" }).ClickAsync();
        }
        private List<Task> ShowResults()
        {
            List<Task> tasks =
            [
                NavPage(subjectsPageUrl)
            ];
            tasks.AddRange(SearchFillSelects());
            tasks.AddRange(SearchClicks());

            return tasks;
        }
        /// <summary>
        /// Vyplnění vyhledávacího formuláře
        /// </summary>
        private List<Task> SearchFillSelects()
        {
            List<Task> tasks =
            [
                SearchFillSelect("Fakulta:", SearchForm.Faculty),
                SearchFillSelect("Katedra:", SearchForm.Department),
                SearchFillSelect("Sekce:", SearchForm.Section),
                SearchFillSelect("Třída:", SearchForm.ClassName),
                SearchFillSelect("Klasifikace:", SearchForm.Classification),
                SearchFillSelect("4EU+:", SearchForm.FourEUPlus),
                SearchFillSelect("Virtuální mobilita:", SearchForm.VirtualMobility),
                SearchFillSelect("Kompetence:", SearchForm.Competencies),
                SearchFillSelect("Semestr:", SearchForm.Semester),
                SearchFillSelect("Zobrazit:", SearchForm.ShowAmount)
            ];

            return tasks;
        }
        private List<Task> SearchFillFields()
        {
            List<Task> tasks =
            [
                SearchFillFieldByRole("Název: Kód: Shoda: podřetězec   přesná", true, "#nazev", SearchForm.SubjectName),
                SearchFillFieldByLabel("Vyučující:", SearchForm.TeacherName)
            ];

            return tasks;
        }
        private List<Task> SearchFillRadioBoxes()
        {
            //TODO: Check if only one in group is selected
            List<Task> tasks =
            [
                SearchFillRadioBox("Garanty", SearchForm.Guarantors),
                SearchFillRadioBox("Vyučujícími", SearchForm.Teachers),
                SearchFillRadioBox("Garanty a Vyučujícími", SearchForm.GuarantorsAndTeachers),
                SearchFillRadioBox("přesná", SearchForm.Exact),
                SearchFillRadioBox("podřetězec", SearchForm.Substring)
            ];

            return tasks;
            
            //Nelze bez přihlášení
            //await SearchFillRadioBox("vyučován", true);
            //await SearchFillRadioBox("nevyučován", true);
        }
        private List<Task> SearchClicks()
        {
            List<Task> tasks =
            [
                SearchClickButton("Hledej"),
                SearchClickDownload("exportovat do excelu"),
            ];

            return tasks;
            
        }
        private async Task GetCSVfromResults()
        {
            bool nextPageButtonVisible = true;

            while (nextPageButtonVisible)
            {
                await SearchClickDownload("exportovat do excelu");
                nextPageButtonVisible = await SearchClickNextPage();
            }
        }
        private async Task<bool> SearchClickNextPage()
        {
            if (await IsFirstLinkVisible("další"))
            {
                await SearchClickLink("další");
                return true;
            }
            return false;
        }
        private List<Task> SearchFillCheckBoxes()
        {
            List<Task> tasks =
            [
                SearchFillCheckBox("Název", true, true),
                SearchFillCheckBox("Anotace", false, true),
                SearchFillCheckBox("Sylabus", false, true),
            ];

            return tasks;
        }
        private async Task SearchFillCheckBox(string label, bool exactMatch, bool check)
        {
            if (check)
            { 
                await SearchCheckCheckBox(label, exactMatch);
            }
            else
            {
                await SearchUncheckCheckBox(label, exactMatch);
            }
        }
        private async Task SearchCheckCheckBox(string label, bool exactMatch)
        {
            if (exactMatch)
            {
                await Page.GetByLabel(label, new() { Exact = true }).CheckAsync();
            }
            else
            {
                await Page.GetByLabel(label).CheckAsync();
            }
        }
        private async Task SearchUncheckCheckBox(string label, bool exactMatch)
        {
            if (exactMatch)
            {
                await Page.GetByLabel(label, new() { Exact = true }).UncheckAsync();
            }
            else
            {
                await Page.GetByLabel(label).UncheckAsync();
            }
        }
        private async Task SearchFillRadioBox(string label, bool exactMatch)
        {
            if (exactMatch)
            {
                await Page.GetByLabel(label, new() { Exact = true }).CheckAsync();
            }
            else
            {
                await Page.GetByLabel(label).CheckAsync();
            }
        }
        private async Task SearchFillSelect(string label, params string[] options)
        {
            await Page.GetByLabel(label).SelectOptionAsync(options);
        }
        private async Task SearchFillFieldByLabel(string label, string value)
        {
            await Page.GetByLabel(label).FillAsync(value);
        }
        private async Task SearchFillFieldByRole(string role, bool exactMatch, string locator, string value)
        {
            await Page.GetByRole(AriaRole.Row, new() { Name = role, Exact = exactMatch }).Locator(locator).FillAsync(value);
        }
        private async Task SearchClickButton(string role)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = role }).ClickAsync();
        }
        private async Task SearchClickLink(string role)
        {
            await Page.GetByRole(AriaRole.Link, new() { Name = role }).First.ClickAsync();
        }
        private async Task SearchClickDownload(string role)
        {
            IDownload download = await Page.RunAndWaitForDownloadAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Link, new() { Name = role }).ClickAsync();
            });
        }
        // pokud by přístup byl neveřejný
        private async Task Login(string username, string password)
        {
            if (await IsButtonVisible("Přihlásit se"))
            {
                await Page.GetByLabel("Login :").ClickAsync();
                await Page.GetByLabel("Login :").FillAsync(username);
                await Page.GetByLabel("Heslo :").ClickAsync();
                await Page.GetByLabel("Heslo :").FillAsync(password);
                await Page.GetByRole(AriaRole.Button, new() { Name = "Přihlásit se" }).ClickAsync();
            }
        }
        private async Task<bool> IsButtonVisible(string role)
        {
            return await Page.GetByRole(AriaRole.Button, new() { Name = role }).IsVisibleAsync();
        }
        private async Task<bool> IsFirstLinkVisible(string role)
        {
            return await Page.GetByRole(AriaRole.Link, new() { Name = role }).First.IsVisibleAsync();
        }
    }
}
