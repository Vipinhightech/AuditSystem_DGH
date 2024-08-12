using AuditSystem.Core.Models;
using AuditSystem.Core.ViewModels;
using AuditSystem.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AuditSystem.WebUI.Controllers
{
    [Authorize]
    public class BlockManagerController : Controller
    {
        private DataContext context = new DataContext();
        // GET: BlockManager
        public ActionResult Index()
        {
            List<AuditSystem_Blocks> block = context.AuditSystem_Blocks.OrderBy(b => b.Block_Name).ToList();
            return View(block);
        }
        public ActionResult Create()
        {
            AuditBlocksViewModel viewModel = new AuditBlocksViewModel();
            viewModel.Block = new AuditSystem_Blocks();
            viewModel.wS_Blocks = context.WS_Block_Masters.AsQueryable();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuditSystem_Blocks block)
        {
            if (!ModelState.IsValid)
            {
                return View(block);
            }
            WS_Block_Master wS_Block = context.WS_Block_Masters.FirstOrDefault(i => i.Block_Id == block.Block_Id);
            block.Psc_Start_Date = wS_Block.Psc_Start_Date.Date;
            block.Block_Name = wS_Block.Block_Name;
            block.UpdatedDate = DateTime.Now.Date.ToString();
            block.UpdatedBy = "Test";
            context.AuditSystem_Blocks.Add(block);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "superuser,admin,management")]
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSystem_Blocks block = context.AuditSystem_Blocks/*.Include(e => e.Exceptions)*/.FirstOrDefault(b => b.Block_Id == Id);
            if (block == null)
            {
                return HttpNotFound();
            }
            return View(block);
        }
        [HttpPost]
        [Authorize(Roles = "superuser,admin,management")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuditSystem_Blocks block)
        {
            if (!ModelState.IsValid)
            {
                return View(block);
            }
            block.UpdatedDate = DateTime.Now.Date.ToString();
            block.UpdatedBy = "Test_edit";
            context.Entry(block).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Details", new { Id = block.Block_Id });

        }

        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSystem_Blocks block = context.AuditSystem_Blocks.Include(e => e.Exceptions).Include(a => a.Attachments).FirstOrDefault(b => b.Block_Id == Id);

            if (block == null)
            {
                return HttpNotFound();
            }

            block.Exceptions = block.Exceptions.OrderByDescending(e => e.Year).ThenBy(e => e.ExceptionNo).ToList();
            block.Attachments = block.Attachments.OrderByDescending(a => a.Year).ThenByDescending(a => a.DateofAttachment).ToList();

            return View(block);

        }
    }
}