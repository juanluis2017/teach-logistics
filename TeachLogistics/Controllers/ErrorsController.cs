﻿using System.Web.Mvc;

namespace TeachLogistics.Controllers
{
    public class ErrorsController : BaseController
    {
        public ActionResult Error400()
        {
            return View("404");
        }
        public ActionResult Error401()
        {
            return PartialView("404");
        }
        public ActionResult Error403()
        {
            return View("404");
        }
        public ActionResult Error404()
        {
            return View("404");
        }
        public ActionResult Error500()
        {
            return View("500");
        }
        public ActionResult Error503()
        {
            return View("500");
        }
    }
}