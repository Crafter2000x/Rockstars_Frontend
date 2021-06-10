﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rockstars_Frontend.Models;
using System.Text.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rockstars_Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        ApiController api = new ApiController();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string SearchTerm, string type = "ALL", string tribe = "ALL")
        {
            ViewData["Type"] = type;
            ViewData["Tribe"] = tribe;
            ViewData["KeyWord"] = SearchTerm;
            await api.TribePaginaAPI();
            ViewData["Tribes"] = api.AllTribes;
            return View();
        }

        public IActionResult ArtikelPagina()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> RequestArtikelPartial(int Page)
        {
            ArtikelenViewModel artikelenViewModel = new ArtikelenViewModel();

            ApiController api = new ApiController();
            await api.PostOverzichtAPI(0,1,6,Page);
            artikelenViewModel.artikelen = api.artikelen;

            return PartialView("ArtikelPartialView", artikelenViewModel);
        }

        public IActionResult Podcasts()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> RequestPodcastPartial(int Page)
        {
            PodcastsViewModel podcastsViewModel = new PodcastsViewModel();

            ApiController api = new ApiController();
            await api.PostOverzichtAPI(2,1,4, Page);
            podcastsViewModel.podcasts = api.podcasts;

            return PartialView("PodcastPartialView", podcastsViewModel);
        }

        public async Task<IActionResult> Artikel(string? title)
        {
            if (title == null)
            {
                return RedirectToAction("TribeOverzicht");
            }
            ViewData["ID"] = title;
            List<ArtikelModel> artikelen = new List<ArtikelModel>();
            ApiController api = new ApiController();
            await api.ArtikelPaginaAPI();
            artikelen = api.artikelen;
            ArtikelModel artikel = new ArtikelModel();
            Console.WriteLine(ViewData["ID"].ToString());
            foreach (ArtikelModel ar in artikelen)
            {
                if (ar.Title.Equals(ViewData["ID"].ToString()))
                {
                    artikel = ar;
                }
            }
            string contentValue;
            artikel.Attributes.TryGetValue("content", out contentValue);
            if (contentValue != null)
            {
                ViewData["Content"] = Base64Decode(contentValue);
            }
            else
            {
                ViewData["Content"] = "";
            }
            string PdfUrlvalue;
            artikel.Attributes.TryGetValue("pdf", out PdfUrlvalue);
            if (PdfUrlvalue != null)
            {
                ViewData["PdfUrl"] = "https://localhost:6001/api/File/" + artikel.Attributes["pdf"] + "/retrieve";
            }
            else
            {
                ViewData["PdfUrl"] = "";
            }

            return View(artikel);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public IActionResult OnDemand(string Tribe)
        {
            if (Tribe == null)
            {
                ViewData["TRIBE"] = "allen";
            }
            else
            {
                ViewData["TRIBE"] = Tribe;
            }

            return View();
        }


        public IActionResult Podcast(string? title)
        {
            if (title == null)
            {
                return RedirectToAction("Podcasts");
            }
            ViewData["ID"] = title;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnDemand(FormulierEnAanvraagModel model)
        {
            if (model.form.preferredDateTime < DateTime.Now)
            {
                ModelState.AddModelError("model","Vul een geldige datum in.");
                return View();
            }
                if (ModelState.IsValid)
                {

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Bevestiging";
                    mail.Body = "<h1>Hierbij krijgt u de bevestiging van uw talkaanvraag.</h1><br/>" +
                    "voorkeursdag:  " + model.form.preferredDateTime.ToString("dd-MM-yyyy") +
                    "<br/>bedrijfsnaam:  " + model.form.companyName +
                    "<br/>aantal mensen aanwezig:  " + model.form.amountOfPeople +
                    "<br/>tribe:  " + model.form.tribe.Title +
                    "<br/>opmerking:  " + model.form.comment +
                    "<br/>nummer:  " + model.form.phoneNumber;

                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return View();
                }
            
            //await api.AddToAPI(model.form);
            return View("OnDemand");
        }

        public IActionResult TalkAanvraagPartial() 
        {
            var e = new FormulierEnAanvraagModel();
            return PartialView(e);
        }

        [HttpPost]
        public async Task<ActionResult> TalkAanvraag(FormulierEnAanvraagModel model)
        {
                    model.form.tribeId = 1;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Bevestiging";
                    mail.Body = "<h1>Hierbij krijgt u de bevestiging van uw talkaanvraag.</h1><br/>" +
                    "voorkeursdag:  " + model.form.preferredDateTime.ToString("dd-MM-yyyy") +
                    "<br/>bedrijfsnaam:  " + model.form.companyName +
                    "<br/>aantal mensen aanwezig:  " + model.form.amountOfPeople +
/*                    "<br/>tribe:  " + f.form.tribe.Title +
*/                    "<br/>opmerking:  " + model.form.comment +
                    "<br/>nummer:  " + model.form.phoneNumber;

                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                await api.AddToAPI(model.form);
                return RedirectToAction("OnDemand");
        }


        [HttpPost]
        public IActionResult TalkAanvraagPartial(FormulierEnAanvraagModel model)
        {
            Console.Write("Baanad");
            if (ModelState.IsValid)
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Bevestiging";
                    mail.Body = "<h1>Hierbij krijgt u de bevestiging van uw talkaanvraag.</h1><br/>" +
                    "voorkeursdag:  " + model.form.preferredDateTime.ToString("dd-MM-yyyy") +
                    "<br/>bedrijfsnaam:  " + model.form.companyName +
                    "<br/>aantal mensen aanwezig:  " + model.form.amountOfPeople +
/*                    "<br/>tribe:  " + f.form.tribe.Title +
*/                    "<br/>opmerking:  " + model.form.comment +
                    "<br/>nummer:  " + model.form.phoneNumber;

                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return View("OnDemand");
            }

            //await api.AddToAPI(model.form);
            return View("OnDemand");
        }

        [HttpPost]
        public async Task<IActionResult> Aanvragen(FormulierEnAanvraagModel model)
        {
            if (ModelState.IsValid)
            {

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Aangevraagde Talk";
                    //deze link moet uiteindelijk uit de api gehaald worden op basis van welke webinar je aanklikt.
                    mail.Body = "Link naar youtube:  https://www.youtube.com/watch?v=dQw4w9WgXcQ";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                //await api.AddToAPI(model.talk);
                return RedirectToAction("OnDemand");
            }
                return View();
           
            
        }
        [HttpPost]
        public async Task<IActionResult> vragen(FormulierEnAanvraagModel model)
        {
            if (ModelState.IsValid)
            {

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Aangevraagde Talk";
                    //deze link moet uiteindelijk uit de api gehaald worden op basis van welke webinar je aanklikt.
                    mail.Body = "Link naar youtube:  https://www.youtube.com/watch?v=dQw4w9WgXcQ";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                //await api.AddToAPI(model.talk);
                return View(OnDemand("Tribe"));
            }
            return View();


        }
        [HttpPost]
        public async Task<IActionResult> HulpAanvragen(FormulierEnAanvraagModel model)
        {
            if (ModelState.IsValid)
            {

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("RockstarsITFrontEnd@gmail.com");
                    mail.To.Add(model.form.contactEmail);
                    mail.Subject = "Aangevraagde hulp";
                    mail.Body = model.form.comment;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("RockstarsITFrontEnd@gmail.com", "DitIsOnzeMail1234");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return RedirectToAction("SpecialAgent");
            }
            return RedirectToAction("SpecialAgent");

        }

        public IActionResult TribeOverzicht()
        {
            return View();
        }
        public async Task<IActionResult> SpecialAgent(FormulierEnAanvraagModel f)
        {
            await api.TribeleadsAPI();
            f.userlist = api.Tribeleads;
            return View(f);
        }
        public IActionResult TribePagina(string? title)
        {
            if (title == null)
            {
                return RedirectToAction("ArtikelPagina");
            }
            ViewData["ID"] = title;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
