using AuditSystem.Core.Models;
using AuditSystem.Core.ViewModels;
using AuditSystem.DataAccess.SQL;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            IEnumerable<AuditSystem_Blocks> b1 = context.AuditSystem_Blocks.AsQueryable();
            viewModel.Block = new AuditSystem_Blocks { Revenue_Expenditures = new List<Audit_Year_Revenue_Expenditure>()};
            viewModel.v_Blocks = context.V_BLOCK_MASTER.AsQueryable().OrderBy(b => b.Block_Name).Where(v => !b1.Any(b => b.Block_Id == v.Block_Id));
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
            V_Block_Master v_Block = context.V_BLOCK_MASTER.FirstOrDefault(i => i.Block_Id == block.Block_Id);
            if (v_Block == null)
            {
                return HttpNotFound();
            }
            if(v_Block.EFFECTIVE_DATE != null)
            {
                block.Psc_Start_Date = v_Block.EFFECTIVE_DATE.Value;
            }
            else
            {
                block.Psc_Start_Date = v_Block.DATE_OF_SIGNING.Value;
            }
            
            block.Block_Name = v_Block.Block_Name;
            block.UpdatedDate = DateTime.Now.Date;
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

            AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
            {
                Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                USERID = Session["UserId"].ToString(),
                TIME = DateTime.Now,
                ACTIVITY = "Add",
                ACTIVITY_VALUES = "Block Id:" + block.Block_Id + ",Block Name:" + block.Block_Name + ",Category:" + block.Block_Category + ",Psc Start Date:" + block.Psc_Start_Date,
                TABLE_NAME = "AuditSystem_Blocks"
            };
            context.AUDIT_UPDATE_LOG.Add(logs);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "superuser,admin,management,coord")]
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
        [Authorize(Roles = "superuser,admin,management,coord")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuditSystem_Blocks block, string removedItems)
        {
            if (!ModelState.IsValid)
            {
                return View(block);
            }
            block.UpdatedDate = DateTime.Now.Date;
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

            AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
            {
                Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                USERID = Session["UserId"].ToString(),
                TIME = DateTime.Now,
                ACTIVITY = "Edit",
                ACTIVITY_VALUES = "Block Id:"+ block.Block_Id + ",Block Name:" + block.Block_Name + ",Category:"+ block.Block_Category+ "",
                TABLE_NAME = "AuditSystem_Blocks"
            };
            context.AUDIT_UPDATE_LOG.Add(logs);


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
            block.Attachments = block.Attachments.OrderByDescending(a => a.FromYear).ThenByDescending(a => a.DateofAttachment).ToList();
            block.Revenue_Expenditures = block.Revenue_Expenditures.OrderByDescending(e => e.Year).ToList();

            // Fetch unique NatureOfException values from the database
            ViewBag.NatureOfException = context.Audit_Exception_Details
                .Select(e => e.NatureOfException)
                .Distinct()
                .OrderBy(e => e) // Sort alphabetically
                .Select(e => new SelectListItem
                {
                    Value = e,
                    Text = e
                })
                .ToList();

            return View(block);

        }
    }
}