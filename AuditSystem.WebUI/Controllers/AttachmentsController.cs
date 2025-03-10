﻿using AuditSystem.Core.Models;
using AuditSystem.DataAccess.SQL;
using Newtonsoft.Json;
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

            Audit_Attachments attach = new Audit_Attachments
            {
                Block_Id = block.Block_Id,
                Block = block
            };

            return View(attach);
        }

     
        [HttpPost]
        public ActionResult Upload_Attachment(Audit_Attachments attachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
  
                    var fileExt = Path.GetExtension(file.FileName);

                    if (fileExt.ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("", "Upload only PDF file");
                        return View(attachment);
                    }
                    attachment.Id = (context.Audit_Attachments.Any() ? context.Audit_Attachments.Max(e => e.Id) : 0) + 1;
                    var filename = $"{attachment.Id}_{attachment.FromYear.Substring(0, 4)}-{attachment.ToYear.Substring(attachment.ToYear.Length - 4)}_{attachment.Block_Id}{fileExt}";
                    attachment.Doc_Address = filename;
                    file.SaveAs(Server.MapPath("~/Attachments/") + filename);
                    attachment.Updated_Date = DateTime.Now;
                    attachment.Updated_By = Session["UserId"].ToString();
                    context.Audit_Attachments.Add(attachment);

                    AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                    {
                        Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                        IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                        USERID = Session["UserId"].ToString(),
                        TIME = DateTime.Now,
                        ACTIVITY = "Add",
                        ACTIVITY_VALUES = JsonConvert.SerializeObject(attachment),
                        TABLE_NAME = "Audit_Attachments"
                    };
                    context.AUDIT_UPDATE_LOG.Add(logs);
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
        [Authorize(Roles = "superuser,admin,management,coord")]
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
        [Authorize(Roles = "superuser,admin,management,coord")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Audit_Attachments attachment, HttpPostedFileBase file,string Title)
        {
            if (ModelState.IsValid)
            {
                attachment.Title = Title; // Assign selected Title value from the form
                if (file != null)
                {
                    // attachment.Id = context.Audit_Attachments.Max(e => e.Id) + 1;
                    var fileExt = Path.GetExtension(file.FileName);
                    if (fileExt.ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("", "Upload only PDF file");
                        return View(attachment);
                    }
                    var filename = $"{attachment.Id}_{attachment.FromYear.Substring(0, 4)}-{attachment.ToYear.Substring(attachment.ToYear.Length - 4)}_{attachment.Block_Id}{fileExt}";
                    attachment.Doc_Address = filename;
                    file.SaveAs(Server.MapPath("~/Attachments/") + filename);
                    attachment.Updated_Date = DateTime.Now;
                    attachment.Updated_By = Session["UserId"].ToString();
                    context.Entry(attachment).State = EntityState.Modified;

                    AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                    {
                        Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                        IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                        USERID = Session["UserId"].ToString(),
                        TIME = DateTime.Now,
                        ACTIVITY = "Edit",
                        ACTIVITY_VALUES = JsonConvert.SerializeObject(attachment),
                        TABLE_NAME = "Audit_Attachments"
                    };
                    context.AUDIT_UPDATE_LOG.Add(logs);

                    context.SaveChanges();
                    return RedirectToAction("Details", "BlockManager", new { Id = attachment.Block_Id });
                }
                else
                {
                    attachment.Updated_Date = DateTime.Now;
                    attachment.Updated_By = Session["UserId"].ToString();
                    context.Entry(attachment).State = EntityState.Modified;

                    AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                    {
                        Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                        IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                        USERID = Session["UserId"].ToString(),
                        TIME = DateTime.Now,
                        ACTIVITY = "Edit",
                        ACTIVITY_VALUES = JsonConvert.SerializeObject(attachment),
                        TABLE_NAME = "Audit_Attachments"
                    };
                    context.AUDIT_UPDATE_LOG.Add(logs);

                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Data updated successfully!";
                    return RedirectToAction("Details", "BlockManager", new { Id = attachment.Block_Id });
                }
            }
            if (!ModelState.IsValid)
            {
                // Debugging ModelState Errors
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage + " " + e.Exception)
                                              .ToList();

                // Send errors to the view (or log them)
                ViewBag.Errors = errors;
                return View(attachment); // Return view with errors
            }
            return View(attachment);

        }

    }
}