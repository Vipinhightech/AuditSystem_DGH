using AuditSystem.Core.Models;
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
    public class ExceptionController : Controller
    {
        private DataContext context = new DataContext();
        // GET: Exception
        //public ActionResult Index()
        //{
        //    var exception = context.Audit_Exception_Details.Include(e => e.FurtherQuery).Include(i => i.Block).ToList();
        //    return View(exception);
        //}

        public ActionResult Create(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSystem_Blocks block = context.AuditSystem_Blocks.FirstOrDefault(b => b.Block_Id == Id);

            if (block == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //ViewBag.Block_Name = new SelectList(context.AuditSystem_Blocks, "Block_Name", "Block_Name");
            Audit_Exception_Details exception = new Audit_Exception_Details { FurtherQuery = new List<Audit_FurtherQuery_Details>() };
            exception.Block = block;
            exception.Block_Id = block.Block_Id;
            return View(exception);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Audit_Exception_Details exception)
        {
            if (ModelState.IsValid)
            {
                exception.ExceptionId = (context.Audit_Exception_Details.Any() ? context.Audit_Exception_Details.Max(e => e.ExceptionId) : 0) + 1;
               // exception.ExceptionId = context.Audit_Exception_Details.Max(e => e.ExceptionId) + 1;
                exception.Updated_Date = DateTime.Now.Date.ToString();
                exception.Updated_By = "Test";
                if (exception.FurtherQuery != null)
                {
                    foreach (var fqr in exception.FurtherQuery)
                    {
                        fqr.ExceptionId = exception.ExceptionId;
                    }
                }

                context.Audit_Exception_Details.Add(exception);
                context.SaveChanges();
                //if (furtherQuery != null) 
                //{
                //    foreach (var fqr in furtherQuery) 
                //    {
                //        fqr.ExceptionId = exception.ExceptionId;
                //        context.Audit_FurtherQuery_Details.Add(fqr);
                //    }

                //    context.SaveChanges();
                //}

                return RedirectToAction("Details", "BlockManager", new { Id = exception.Block_Id });
            }

            //ViewBag.Block_Name = new SelectList(context.AuditSystem_Blocks, "Block_Name", "Block_Name");
            return View(exception);
        }
        [Authorize(Roles = "superuser,admin,management")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Audit_Exception_Details exception = context.Audit_Exception_Details.Include(f => f.FurtherQuery).Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == id);
            if (exception == null)
            {
                return HttpNotFound();
            }

            return View(exception);
        }

        [HttpPost]
        [Authorize(Roles = "superuser,admin,management")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Audit_Exception_Details exception, string removedItems)
        {
            if (ModelState.IsValid)
            {
                exception.Updated_Date = DateTime.Now.Date.ToString();
                exception.Updated_By = "Test_Edit";
                if (exception.FurtherQuery != null)
                {
                    foreach (var fqr in exception.FurtherQuery)
                    {
                        fqr.ExceptionId = exception.ExceptionId;
                        if (fqr.Id == 0)
                        {
                            context.Audit_FurtherQuery_Details.Add(fqr);
                        }
                        else

                        {
                            context.Entry(fqr).State = EntityState.Modified;
                        }
                    }
                }

                context.Entry(exception).State = EntityState.Modified;

                if (!string.IsNullOrEmpty(removedItems))
                {
                    var idsToRemove = removedItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    foreach (var id in idsToRemove)
                    {
                        var itemsToRemove = context.Audit_FurtherQuery_Details.Find(id);
                        if (itemsToRemove != null)
                        {
                            context.Audit_FurtherQuery_Details.Remove(itemsToRemove);
                        }
                    }
                }

                context.SaveChanges();
                return RedirectToAction("Details", new { id = exception.ExceptionId });
            }
            return View(exception);
        }

        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Audit_Exception_Details exception = context.Audit_Exception_Details.Include(f => f.FurtherQuery).Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == id);
            if (exception == null)
            {
                return HttpNotFound();
            }

            return View(exception);
        }

        

    }
}