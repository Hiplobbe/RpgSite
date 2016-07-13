using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using RPGSite.Models;
using RPGSite.Models.Wiki;

namespace RPGSite.Controllers
{
    public class WikiController : Controller
    {
        // GET: Wiki
        [Route("Wiki")]
        public ActionResult Index()
        {
            using (RpgContext context = RpgContext.Create())
            {
                WikiEntry entry = context.WikiEntries.First(w => w.Title == "Index");

                return View(entry);
            }
        }

        // GET: Wiki/Index
        [Route("Wiki/{title}")]
        public ActionResult GetPage(string title)
        {
            using (RpgContext context = RpgContext.Create())
            {
                WikiEntry entry = context.WikiEntries.FirstOrDefault(w => w.Title == title);

                if (entry != null)
                {
                    return View("Index",entry);
                }

                return Index(); //If none is found, returns to start.
            }
        }

        //Pressed the save button.
        [Route("Wiki/{title}/Save")]
        public PartialViewResult Save(string title,FormCollection collection)
        {
            using (RpgContext context = RpgContext.Create())
            {
                WikiEntry entry = context.WikiEntries.FirstOrDefault(w => w.Title == title);
                string newText = collection["Text"];

                if (entry == null)
                {
                    return PartialView("Index");
                }

                if (entry.Text != newText)
                {
                    FixLinks(newText,entry.Text);

                    entry.Text = newText;

                    context.SaveChangesAsync();
                }
            }

            return null;
        }

        private void FixLinks(string NewText,string OldText)
        {
            Regex regex = new Regex(@"\[!(\w+)\]");

            List<string> NewTitles = regex.Matches(NewText).Cast<Match>().Select(m => m.Groups[1].ToString()).Distinct().ToList();
            List<string> OldTitles = regex.Matches(OldText).Cast<Match>().Select(m => m.Groups[1].ToString()).Distinct().ToList();

            using (RpgContext context = RpgContext.Create())
            {
                foreach (string newTitle in NewTitles)
                {
                    if (!OldTitles.Any(old => old.ToLower() == newTitle.ToLower())) //If a new link has been added.
                    {
                        if (!context.WikiEntries.Any(entry => entry.Title == newTitle.ToLower()))//If there isn't any title in db yet.
                        {
                            AddTitle(context, newTitle.ToLower()); //Add the new title to db.
                        }
                        else
                        {
                            AddLink(context, newTitle.ToLower());
                        }
                    }
                    else
                    {
                        OldTitles.Remove(newTitle);
                    }
                }

                foreach (string oldTitle in OldTitles)//Everyone of the old links that is not in the new text, gets -1 to their referance variable.
                {
                    RemoveLink(context, oldTitle.ToLower());
                }
            }
        }

        private void AddLink(RpgContext Context, string Title)
        {
            WikiEntry entry = Context.WikiEntries.FirstOrDefault(e => e.Title == Title);

            if (entry != null)
            {
                entry.Referanced++;

                Context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Modifies the wiki entrys referanced variable, to symbolize it being less referanced in other entries.
        /// 
        /// And removes the entry completely if not referanced at all.
        /// </summary>
        /// <param name="Context"> The db context</param>
        /// <param name="Title">The title of the wiki entry</param>
        private void RemoveLink(RpgContext Context, string Title)
        {
            WikiEntry entry = Context.WikiEntries.FirstOrDefault(e => e.Title == Title);

            if (entry != null)
            {
                entry.Referanced--;

                if (entry.Referanced <= 0)
                {
                    Context.WikiEntries.Remove(entry);
                }

                Context.SaveChangesAsync();
            }
        }

        private void AddTitle(RpgContext Context,string Title)
        {
            Context.WikiEntries.Add(new WikiEntry(Title));
            Context.SaveChangesAsync();
        }
    }
}
