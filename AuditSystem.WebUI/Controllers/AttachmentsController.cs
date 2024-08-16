using AuditSystem.Core.Models;
using AuditSystem.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AuditSystem.WebUI.Controllers
{
    [Authorize]
    public class AttachmentsController : Controller
    {
        private DataContext context = new DataContext();
        // GET: Attachments
        //public ActionResult Index(string Block_Name)
        //{

        //    if (Block_Name == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AuditSystem_Blocks block = context.AuditSystem_Blocks.FirstOrDefault(b => b.Block_Name == Block_Name);

        //    if (block == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    ////ViewBag.Block_Name = new SelectList(context.AuditSystem_Blocks, "Block_Name", "Block_Name");
        //    //Audit_Exception_Details exception = new Audit_Exception_Details { FurtherQuery = new List<Audit_FurtherQuery_Details>() };
        //    //exception.Block = block;
        //    //exception.Block_Name = block.Block_Name;
        //    //return View(exception);

        //    return View();
        //}

        public ActionResult Upload_Attachment(int? Id)
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

            Audit_Attachments attach = new Audit_Attachments();
            attach.Block_Id = block.Block_Id;
            attach.Block = block;

            return View(attach);
        }

        //[HttpPost]
        //public ActionResult Upload_Attachment(Audit_Attachments attachment, HttpPostedFile file) 
        //{
        //    if(ModelState.IsValid)
        //    {
        //        if(file != null)
        //        {
        //            var fileExt = Path.GetExtension(file.FileName);
        //            var filename = $"{attachment.Year}_{attachment.Block_Name}_{attachment.Title}{fileExt}";
        //            attachment.Doc_Address = "//Attachments//" + filename;
        //            context.Audit_Attachments.Add(attachment);
        //            context.SaveChanges();
        //            return RedirectToAction("Details", "BlockManager", new { Block_Name = attachment.Block_Name });
        //        }
        //    }

        //    return View(attachment);
        //}
        [HttpPost]
        public ActionResult Upload_Attachment(Audit_Attachments attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    
                    attachment.Id = (context.Audit_Attachments.Any() ? context.Audit_Attachments.Max(e => e.Id) : 0) + 1;
                    var fileExt = Path.GetExtension(file.FileName);
                    var filename = $"{attachment.Id}_{attachment.Year}_{attachment.Block_Id}{fileExt}";
                    attachment.Doc_Address = filename;
                    file.SaveAs(Server.MapPath("~/Attachments/") + filename);
                    context.Audit_Attachments.Add(attachment);
                    context.SaveChanges();
                    return RedirectToAction("Details", "BlockManager", new { Id = attachment.Block_Id });
                }
                else
                {
                    ModelState.AddModelError("", "Upload file");
                }

            }

            return View(attachment);
        }
        [Authorize(Roles = "superuser,admin,management")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Audit_Attachments attachment = context.Audit_Attachments.FirstOrDefault(e => e.Id == id);
            //Audit_Exception_Details exception = context.Audit_Exception_Details.Include(f => f.FurtherQuery).Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == id);
            if (attachment == null)
            {
                return HttpNotFound();
            }

            return View(attachment);
        }
        [HttpPost]
        [Authorize(Roles = "superuser,admin,management")]
        // [ValidateAntiForgeryToken]
        public ActionResult Edit(Audit_Attachments attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // attachment.Id = context.Audit_Attachments.Max(e => e.Id) + 1;
                    var fileExt = Path.GetExtension(file.FileName);
                    var filename = $"{attachment.Id}_{attachment.Year}_{attachment.Block_Id}{fileExt}";
                    attachment.Doc_Address = filename;
                    file.SaveAs(Server.MapPath("~/Attachments/") + filename);
                    context.Entry(attachment).State = EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Details", "BlockManager", new { Id = attachment.Block_Id });
                }
                else
                {

                    //  var fileExt = Path.GetExtension(file.FileName);
                    //  var filename = $"{attachment.Id}_{attachment.Year}_{attachment.Block_Name}{DateTime.Now.ToString()}{fileExt}";
                    //  attachment.Doc_Address = filename;
                    //  file.SaveAs(Server.MapPath("~/Attachments/") + filename);
                    context.Entry(attachment).State = EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Details", "BlockManager", new { Id = attachment.Block_Id });
                }

            }

            return View(attachment);

        }

    }
}