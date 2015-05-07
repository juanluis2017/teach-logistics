﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tesis.ViewModels;
using MvcFlash.Core.Extensions;
using Tesis.Models;

namespace Tesis.Controllers
{
    public class SimulationsController : BaseController
    {
        // GET: Simulations
        public async Task<ActionResult> Index()
        {
            return View(await Db.Sections.Where(x => x.CaseStudyId != null).ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> EnableSimulation([Bind(Prefix = "EnableId")] Guid? Id)
        {
            try
            {
                Section section = await Db.Sections.Where(c => c.Id == Id).FirstOrDefaultAsync();
                if (section == null)
                {
                    throw new Exception();
                }
                section.IsActivedSimulation = true;
                if (section.Periods.Count() == 0)
                {
                    Period period = new Period
                    {
                        Created = DateTime.Now,
                        Id = Guid.NewGuid(),
                        IsLastPeriod = false,
                        PeriodNumber = 0,
                    };
                    section.Periods.Add(period);
                    await Db.SaveChangesAsync();
                }
                Flash.Success("Ok", "La Simulación ha sido activada con exito");
                return RedirectToAction("Index");
            }
            catch
            {
                Flash.Error("Error", "Ha Ocurrido un error habilitando la sección");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DisableSimulation([Bind(Prefix = "DisableId")] Guid? Id)
        {
            Section section = await Db.Sections.Where(x => x.Id == Id).FirstOrDefaultAsync<Section>();
            try 
            {
                if (section == null)
                {
                    throw new Exception();
                }
                section.IsActivedSimulation = false;
                await Db.SaveChangesAsync();
                Flash.Success("Ok", "La Simulación ha sido finalizada");
                return RedirectToAction("Index");
            }
            catch
            {
                Flash.Error("Error", "Ha ocurrido un error finalizando la simulación");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> RegisterSells(Guid? Id)
        {
            Section section = await Db.Sections.Where(x => x.Id == Id).FirstOrDefaultAsync<Section>();
            if (section == null)
            {
                throw new Exception();
            }

            if (section.Periods.Count() == 0 || section.IsActivedSimulation == false)
            {
                Flash.Error("Error", "No ha sido activada la simulación");
                return RedirectToAction("Index");
            }

            var caseStudyQuery = Db.CaseStudies.Where(x => x.Id == section.CaseStudyId);
            CaseStudy caseStudy = await caseStudyQuery.FirstOrDefaultAsync();
            SellViewModel sellViewModel = new SellViewModel
            {
                CaseStudyId = caseStudy.Id,
                Products = caseStudy.InitialCharges.Select(y => y.Product).ToList<Product>(),
                Section = section,
            };
            return View(sellViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterSells(SellViewModel model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult> Groups(Guid? Id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult> Rankings(Guid? Id)
        {
            throw new NotImplementedException();
        }
    }
}