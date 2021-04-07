﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rockstars_Frontend.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

        public IActionResult ArtikelPagina()
        {
            return View();
        }
        public IActionResult OnDemand()
        {
            return View();
        }

        public IActionResult Artikel(string? title)
        {
            if (title == null)
            {
                return RedirectToAction("ArtikelPagina");
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
                return View("OnDemand");
            }
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
