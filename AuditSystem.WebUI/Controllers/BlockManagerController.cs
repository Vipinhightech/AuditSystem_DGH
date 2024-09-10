using AuditSystem.Core.Models;
using AuditSystem.Core.ViewModels;
using AuditSystem.DataAccess.SQL;
using Microsoft.Ajax.Utilities;
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
            viewModel.Block = new AuditSystem_Blocks { Revenue_Expenditures = new List<Audit_Year_Revenue_Expenditure>()};
            viewModel.wS_Blocks = context.WS_BLOCK_MASTER.AsQueryable().OrderBy(b => b.Block_Name);
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
            WS_Block_Master wS_Block = context.WS_BLOCK_MASTER.FirstOrDefault(i => i.Block_Id == block.Block_Id);
            if (wS_Block == null)
            {
                return HttpNotFound();
            }
            if(wS_Block.EFFECTIVE_DATE != null)
            {
                block.Psc_Start_Date = wS_Block.EFFECTIVE_DATE.Value;
            }
            else
            {
                block.Psc_Start_Date = wS_Block.DATE_OF_SIGNING.Value;
            }
            
            block.Block_Name = wS_Block.Block_Name;
            block.UpdatedDate = DateTime.Now.Date.ToString();
            block.UpdatedBy = Session["UserId"].ToString();

            if(block.Revenue_Expenditures != null)
            {
                int RevExpId = (context.Audit_Year_Revenue_Expenditure.Any() ? context.Audit_Year_Revenue_Expenditure.Max(e => e.Id) : 0) + 1;
                foreach(var RevExp in block.Revenue_Expenditures)
                {
                    RevExp.Id = RevExpId;
                    RevExp.Block_Id = block.Block_Id;
                    RevExpId++;
                }
            }
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
            AuditSystem_Blocks block = context.AuditSystem_Blocks.Include(e => e.Revenue_Expenditures).FirstOrDefault(b => b.Block_Id == Id);
            block.Revenue_Expenditures = block.Revenue_Expenditures.OrderByDescending(e => e.Year).ToList();
            if (block == null)
            {
                return HttpNotFound();
            }
            return View(block);
        }
        [HttpPost]
        [Authorize(Roles = "superuser,admin,management")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuditSystem_Blocks block, string removedItems)
        {
            if (!ModelState.IsValid)
            {
                return View(block);
            }
            block.UpdatedDate = DateTime.Now.Date.ToString();
            block.UpdatedBy = Session["UserId"].ToString();

            if (block.Revenue_Expenditures != null)
            {
                int RevExpId = (context.Audit_Year_Revenue_Expenditure.Any() ? context.Audit_Year_Revenue_Expenditure.Max(e => e.Id) : 0) + 1;
                foreach (var RevExp in block.Revenue_Expenditures)
                {
                    RevExp.Block_Id = block.Block_Id;
                    if(RevExp.Id == 0)
                    {
                        RevExp.Id = RevExpId;
                        context.Audit_Year_Revenue_Expenditure.Add(RevExp);
                        RevExpId++;
                    }
                    else
                    {
                        context.Entry(RevExp).State = EntityState.Modified;
                    }
                }

            }
            context.Entry(block).State = EntityState.Modified;
            if (!string.IsNullOrEmpty(removedItems))
            {
                var idsToRemove = removedItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                foreach (var id in idsToRemove)
                {
                    var itemsToRemove = context.Audit_Year_Revenue_Expenditure.Find(id);
                    if (itemsToRemove != null)
                    {
                        context.Audit_Year_Revenue_Expenditure.Remove(itemsToRemove);
                    }
                }
            }
            context.SaveChanges();
            return RedirectToAction("Details", new { Id = block.Block_Id });
        }

        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSystem_Blocks block = context.AuditSystem_Blocks.Include(e => e.Exceptions).Include(a => a.Attachments).Include(a=>a.Revenue_Expenditures).FirstOrDefault(b => b.Block_Id == Id);

            if (block == null)
            {
                return HttpNotFound();
            }

            block.Exceptions = block.Exceptions.OrderByDescending(e => e.Year).ThenBy(e => e.ExceptionNo).ThenBy(e=>e.ExceptionSubNo).ToList();
            block.Attachments = block.Attachments.OrderByDescending(a => a.Year).ThenByDescending(a => a.DateofAttachment).ToList();
            block.Revenue_Expenditures = block.Revenue_Expenditures.OrderByDescending(e => e.Year).ToList();
            return View(block);

        }
    }
}