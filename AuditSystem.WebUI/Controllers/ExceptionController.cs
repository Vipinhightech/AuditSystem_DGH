﻿using AuditSystem.Core.Models;
using AuditSystem.Core.ViewModels;
using AuditSystem.DataAccess.SQL;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

            // Fetch data from ExceptionNature table for dropdown
            var exceptionNatureList = context.Set<ExceptionNature>().OrderBy(e => e.NatureOfException).Select(e => new SelectListItem { Value = e.NatureOfException, Text = e.NatureOfException }).ToList();
            ViewBag.NatureOfException = exceptionNatureList;

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
                bool isDuplicate = context.Audit_Exception_Details.Any(e => e.Block_Id == exception.Block_Id && e.Year == exception.Year && e.ExceptionNo == exception.ExceptionNo && e.ExceptionSubNo.Trim().ToLower() == exception.ExceptionSubNo.Trim().ToLower());
                if (isDuplicate)
                {
                    ModelState.AddModelError(string.Empty, "Entry already exist for the Block for Year:"+ exception.Year + ", Exception No: " + exception.ExceptionNo +" & Exception subno: " + exception.ExceptionSubNo);
                    if (exception.FurtherQuery == null) 
                    {
                        exception.FurtherQuery = new List<Audit_FurtherQuery_Details>();
                    }
                    return View(exception);
                }
                exception.ExceptionId = (context.Audit_Exception_Details.Any() ? context.Audit_Exception_Details.Max(e => e.ExceptionId) : 0) + 1;
               // exception.ExceptionId = context.Audit_Exception_Details.Max(e => e.ExceptionId) + 1;
                exception.Updated_Date = DateTime.Now.Date;
                exception.Updated_By = Session["UserId"].ToString();
                exception.S_Status = 0;
                if (exception.FurtherQuery != null)
                {
                    int FqrID = (context.Audit_FurtherQuery_Details.Any() ? context.Audit_FurtherQuery_Details.Max(e => e.Id) : 0) + 1;
                    foreach (var fqr in exception.FurtherQuery)
                    {
                        fqr.Id = FqrID;
                        fqr.ExceptionId = exception.ExceptionId;
                        FqrID++;
                    }
                }

                context.Audit_Exception_Details.Add(exception);

                AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                {
                    Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                    IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                    USERID = Session["UserId"].ToString(),
                    TIME = DateTime.Now,
                    ACTIVITY = "Add",
                    ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo +" " + exception.ExceptionSubNo + "Title"+ exception.ExceptionTitle,
                    TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
                };
                context.AUDIT_UPDATE_LOG.Add(logs);

                context.SaveChanges();
              

                return RedirectToAction("Details", "BlockManager", new { Id = exception.Block_Id });
            }
            return View(exception);
        }


        public ActionResult AddExceptionExcel(int? Id)
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

            return View(block);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExceptionExcel(int block_Id, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditSystem_Blocks block = context.AuditSystem_Blocks.FirstOrDefault(b => b.Block_Id == block_Id);
            if (block == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith(".xlsx"))
                {
                    int lineno = 1;
                    int flineno = 0;
                    int err = 0;
                    try
                    {
                        using (var stream = file.InputStream)
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet();
                                DataTable dt = result.Tables[0];
                                DataTable dt1 = result.Tables[1];
                                var dataList = new List<AuditExceptionsExcel>();
                             
                                for (int i = 1; i < dt.Rows.Count; i++)
                                {
                                    flineno = 0;
                                    var data = new AuditExceptionsExcel
                                    {
                                        X_Id = Convert.ToInt32(dt.Rows[i][0]),
                                        Year = Convert.ToString(dt.Rows[i][1]),
                                        ToYear = Convert.ToString(dt.Rows[i][2]),
                                        Name_Of_Auditor = Convert.ToString(dt.Rows[i][3]),
                                        ExceptionNo = Convert.ToInt32(dt.Rows[i][4]),
                                        ExceptionSubNo = Convert.ToString(dt.Rows[i][5]),
                                        NatureOfException = Convert.ToString(dt.Rows[i][6]),
                                        ExceptionTitle = Convert.ToString(dt.Rows[i][7]),
                                        ZistOfException = Convert.ToString(dt.Rows[i][8]),
                                        ExceptionType = Convert.ToString(dt.Rows[i][9]),
                                        Currency = Convert.ToString(dt.Rows[i][10]),
                                        Quantum = dt.Rows[i][11] != DBNull.Value ? Convert.ToDouble(dt.Rows[i][11]) : 0,
                                        OperatorsReply = Convert.ToString(dt.Rows[i][12]),
                                        CFComments = Convert.ToString(dt.Rows[i][13]),
                                        BlockCoordinatorsComments = Convert.ToString(dt.Rows[i][14]),
                                        FinalRecommendations = Convert.ToString(dt.Rows[i][15]),
                                        CurrentStatus = Convert.ToString(dt.Rows[i][16]),
                                        Remark = Convert.ToString(dt.Rows[i][17]),

                                        FurtherQuery = new List<Audit_FurtherQuery_Details>()
                                    };
                                  
                                  //  string[] dateFormats = { "dd.MM.yyyy", "dd-MM-yyyy", "dd/MM/yyyy" };  // Array of accepted date formats
                                    
                                    if (dt.Rows[i][18] != DBNull.Value)
                                    {
                                        //string dateString = dt.Rows[i][18].ToString();
                                        //DateTime expdate;
                                        //bool success = DateTime.TryParseExact(dateString, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out expdate);

                                        //if (success)
                                        //{
                                        //    data.EXCEPTION_ISSUE_DATE = expdate;
                                        //}
                                        //else
                                        //{
                                        //    ModelState.AddModelError(string.Empty, "Unable to read data. Exception Issue Date in Exceptions Line no. "+ lineno + " must be in DD.MM.YYYY, DD-MM-YYYY or DD/MM/YYYY format.");
                                        //    return View(block);
                                        //}
                                         data.EXCEPTION_ISSUE_DATE = Convert.ToDateTime(dt.Rows[i][18]);
                                    }
                                    
                                    if (dt.Rows[i][19] != DBNull.Value)
                                    {
                                        data.OPERATOR_REPLY_DATE = Convert.ToDateTime(dt.Rows[i][19]);
                                    }
                                   
                                    flineno = 1;
                                    for (int j = 1; j < dt1.Rows.Count; j++)
                                    {
                                        //int F_X_Id = Convert.ToInt32(dt1.Rows[j][0]);
                                        if (data.X_Id == Convert.ToInt32(dt1.Rows[j][0]))
                                        {
                                            var fqr = new Audit_FurtherQuery_Details
                                            {
                                                ExceptionId = Convert.ToInt32(dt1.Rows[j][0]),
                                                FurtherQuery = Convert.ToString(dt1.Rows[j][1]),
                                                FurtherOperatorsReply = Convert.ToString(dt1.Rows[j][2]),
                                                FurtherCFComments = Convert.ToString(dt1.Rows[j][3]),
                                                FurtherBlockCoordinatorsComments = Convert.ToString(dt1.Rows[j][4]),
                                                FurtherFinalRecommendations = Convert.ToString(dt1.Rows[j][5])
                                            };
                                            data.FurtherQuery.Add(fqr);
                                            
                                        }
                                        flineno++;
                                    }
                                    dataList.Add(data);
                                    lineno++;
                                }

                                if (dataList != null)
                                {
                                    lineno = 1;
                                    int ExcpID = (context.Audit_Exception_Details.Any() ? context.Audit_Exception_Details.Max(e => e.ExceptionId) : 0) + 1;
                                    int FqrID = (context.Audit_FurtherQuery_Details.Any() ? context.Audit_FurtherQuery_Details.Max(e => e.Id) : 0) + 1;
                                    int logId = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1;
                                    foreach (var item in dataList)
                                    {
                                        bool isDuplicate = context.Audit_Exception_Details.Any(e => e.Block_Id == block.Block_Id && e.Year == item.Year && e.ExceptionNo == item.ExceptionNo && e.ExceptionSubNo.Trim().ToLower() == item.ExceptionSubNo.Trim().ToLower());
                                        if (isDuplicate)
                                        {
                                            ModelState.AddModelError(string.Empty, "Entry already exist for the Block for Year:" + item.Year + ", Exception No: " + item.ExceptionNo + " & Exception subno: " + item.ExceptionSubNo);                                           
                                            return View(block);
                                        }

                                        flineno = 0;
                                        Audit_Exception_Details exception = new Audit_Exception_Details();
                                        exception.ExceptionId = ExcpID;
                                        exception.Block_Id = block.Block_Id;
                                        // exception.ExceptionId = context.Audit_Exception_Details.Max(e => e.ExceptionId) + 1;
                                        exception.Updated_Date = DateTime.Now.Date;
                                        exception.Updated_By = Session["UserId"].ToString();
                                        exception.S_Status = 0;
                                        exception.ActionTaken = "Unsettled";

                                        exception.Year = item.Year;
                                        exception.ToYear = item.ToYear;
                                        exception.Name_Of_Auditor = item.Name_Of_Auditor;
                                        exception.ExceptionNo = item.ExceptionNo;
                                        exception.ExceptionSubNo = item.ExceptionSubNo;
                                        exception.NatureOfException = item.NatureOfException;
                                        exception.ExceptionTitle = item.ExceptionTitle;
                                        exception.ZistOfException = item.ZistOfException;
                                        exception.ExceptionType = item.ExceptionType;
                                        exception.Currency = item.Currency;
                                        exception.Quantum = item.Quantum;
                                        exception.OperatorsReply = item.OperatorsReply;
                                        exception.CFComments = item.CFComments;
                                        exception.BlockCoordinatorsComments = item.BlockCoordinatorsComments;
                                        exception.FinalRecommendations = item.FinalRecommendations;
                                        exception.CurrentStatus = item.CurrentStatus;
                                        exception.Remark = item.Remark;
                                        exception.EXCEPTION_ISSUE_DATE = item.EXCEPTION_ISSUE_DATE;
                                        exception.OPERATOR_REPLY_DATE = item.OPERATOR_REPLY_DATE;
                                        flineno = 1;
                                        if (item.FurtherQuery != null)
                                        {
                                            foreach (var fqr in item.FurtherQuery)
                                            {
                                                fqr.Id = FqrID;
                                                fqr.ExceptionId = ExcpID;
                                                FqrID++;
                                            }
                                            exception.FurtherQuery = item.FurtherQuery;
                                            flineno++;
                                        }

                                        context.Audit_Exception_Details.Add(exception);

                                        AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                                        {
                                            Id = logId,
                                            IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                                            USERID = Session["UserId"].ToString(),
                                            TIME = DateTime.Now,
                                            ACTIVITY = "Add",
                                            ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo + " " + exception.ExceptionSubNo + "Title" + exception.ExceptionTitle,
                                            TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
                                        };
                                        context.AUDIT_UPDATE_LOG.Add(logs);

                                        
                                        ExcpID++;
                                        logId++;
                                        lineno++;

                                    }
                                    err = 1;
                                    context.SaveChanges();

                                }

                                ViewBag.Message = "Data Imported Successfully";
                                return View(block);
                            }
                        }
                    }
                    catch 
                    {
                        if(err != 1)
                        {
                            ModelState.AddModelError(string.Empty, "Unable to read data. Error in Exceptions Line no. " + lineno + " or in Further_Query Line no." + flineno);
                           // ViewBag.Message = "Unable to read data. Error in Exceptions Line no. " + lineno + " or in Further_Query Line no." + flineno;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Unable to read data.");
                        }
                       
                       
                        return View(block);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Please Upload Excel file (.xlsx)");
                    //ViewBag.Message = "Please Upload Excel file (.xlsx)";
                    return View(block);
                }
                
            }
            ModelState.AddModelError("", "Please Upload file");
            //ViewBag.Message = "Please Upload file";
            
            return View(block);

        }


        [Authorize(Roles = "superuser,admin,management,coord")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Audit_Exception_Details exception = context.Audit_Exception_Details.Include(f => f.FurtherQuery).Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == id);

            ViewBag.NatureOfExceptionList = context.Set<ExceptionNature>().OrderBy(e => e.NatureOfException).Select(e => new SelectListItem{ Value = e.NatureOfException, Text = e.NatureOfException }).ToList();

            if (exception == null)
            {
                return HttpNotFound();
            }
            return View(exception);
        }

        [HttpPost]
        [Authorize(Roles = "superuser,admin,management,coord")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Audit_Exception_Details exception, string removedItems)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = context.Audit_Exception_Details.Any(e => e.Block_Id == exception.Block_Id && e.Year == exception.Year && e.ExceptionNo == exception.ExceptionNo && e.ExceptionSubNo.Trim().ToLower() == exception.ExceptionSubNo.Trim().ToLower() && e.ExceptionId != exception.ExceptionId);
                if (isDuplicate)
                {
                    ModelState.AddModelError(string.Empty, "Entry already exist for the Block for Year:" + exception.Year + ", Exception No: " + exception.ExceptionNo + " & Exception subno: " + exception.ExceptionSubNo);
                    if (exception.FurtherQuery == null)
                    {
                        exception.FurtherQuery = new List<Audit_FurtherQuery_Details>();
                    }
                    return View(exception);
                }
                exception.Updated_Date = DateTime.Now.Date;
                exception.Updated_By = Session["UserId"].ToString();
                if (exception.FurtherQuery != null)
                {
                    int FqrID = (context.Audit_FurtherQuery_Details.Any() ? context.Audit_FurtherQuery_Details.Max(e => e.Id) : 0) + 1;
                    foreach (var fqr in exception.FurtherQuery)
                    {
                        fqr.ExceptionId = exception.ExceptionId;
                        if (fqr.Id == 0)
                        {
                            fqr.Id = FqrID;
                            context.Audit_FurtherQuery_Details.Add(fqr);
                            FqrID++;
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

                AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
                {
                    Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                    IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                    USERID = Session["UserId"].ToString(),
                    TIME = DateTime.Now,
                    ACTIVITY = "Edit",
                    ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo + " " + exception.ExceptionSubNo + "Title" + exception.ExceptionTitle,
                    TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
                };
                context.AUDIT_UPDATE_LOG.Add(logs);

                context.SaveChanges();
                return RedirectToAction("Details", new { id = exception.ExceptionId });
            }
            // Reload dropdown if model state is invalid
            ViewBag.NatureOfExceptionList = context.Set<ExceptionNature>().OrderBy(e => e.NatureOfException).Select(e => new SelectListItem{ Value = e.NatureOfException, Text = e.NatureOfException }).ToList();
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
        [Authorize(Roles = "superuser,admin,management")]
        public ActionResult InitiateSettlement(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit_Exception_Details exception = context.Audit_Exception_Details/*.Include(f => f.FurtherQuery)*/.Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == id);
            if (exception == null)
            {
                return HttpNotFound();
            }
            return View(exception);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "superuser,admin,management")]
        public ActionResult InitiateSettlement(int exceptionId, string S_Remark, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            Audit_Exception_Details exception = context.Audit_Exception_Details.Include(b => b.Block).FirstOrDefault(e => e.ExceptionId == exceptionId);
            if (exception == null) 
            {
                return HttpNotFound();
            }
            if (file == null || S_Remark == "") 
            {
                if (S_Remark != "")
                {
                    exception.S_Remark = S_Remark;                  
                }
                else
                {
                    ModelState.AddModelError("", "Settlement Authorization Request No & Date is required");
                }
                if(file == null)
                {
                    ModelState.AddModelError("", "Upload file");
                }
                return View(exception);
            }
            exception.S_Remark = S_Remark;
           // attachment.Id = (context.Audit_Attachments.Any() ? context.Audit_Attachments.Max(e => e.Id) : 0) + 1;
            var fileExt = Path.GetExtension(file.FileName);
            if (fileExt.ToLower() != ".pdf")
            {
                ModelState.AddModelError("", "Upload only PDF file");
                return View(exception);
            }
            var filename = $"SLetter_{exceptionId}_{DateTime.Now.ToString("ddMMyyyy")}{fileExt}";
            file.SaveAs(Server.MapPath("~/Attachments/") + filename);

            exception.S_Doc_Address = filename;
            exception.S_Status = 1;
            exception.S_InitiatedBy = Session["UserId"].ToString();
            
            context.Entry(exception).State = EntityState.Modified;

            AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
            {
                Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                USERID = Session["UserId"].ToString(),
                TIME = DateTime.Now,
                ACTIVITY = "Settlement Initiate",
                ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo + " " + exception.ExceptionSubNo + "Title" + exception.ExceptionTitle
                                    + ",Settlement_Remark:" + exception.S_Remark + ",Settlement_Doc_Name:" + exception.S_Doc_Address + "Settlement InitiatedBy: " + exception.S_InitiatedBy,
                TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
            };
            context.AUDIT_UPDATE_LOG.Add(logs);

            context.SaveChanges();
            return RedirectToAction("Details", new { id = exception.ExceptionId });

        }
        [Authorize(Roles = "superuser,admin")]
        public ActionResult ApproveSettlement(int? Id) 
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit_Exception_Details exception = context.Audit_Exception_Details.FirstOrDefault(e => e.ExceptionId == Id);
            if (exception == null) 
            {
                return HttpNotFound();
            }
            if (exception.S_Status != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session["UserId"].ToString().ToLower() == exception.S_InitiatedBy.ToLower())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            exception.S_Status = 2;
            exception.ActionTaken = "Settled";
            exception.S_ApprovedBy = Session["UserId"].ToString();
            exception.S_RejectedBy = null;
            context.Entry(exception).State = EntityState.Modified;
            //Log Table Update
            AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
            {
                Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                USERID = Session["UserId"].ToString(),
                TIME = DateTime.Now,
                ACTIVITY = "Settlement Approve",
                ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo + " " + exception.ExceptionSubNo + "Title" + exception.ExceptionTitle
                                    + ",Settlement_Remark:" + exception.S_Remark + ",Settlement_Doc_Name:" + exception.S_Doc_Address + ",Settlement InitiatedBy: " + exception.S_InitiatedBy + "Settlement ApprovedBy:" + exception.S_ApprovedBy,
                TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
            };
            context.AUDIT_UPDATE_LOG.Add(logs);

            context.SaveChanges();
            return RedirectToAction("Details", new { id = exception.ExceptionId });
        }

        [Authorize(Roles = "superuser,admin")]
        public ActionResult RejectSettlement(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Audit_Exception_Details exception = context.Audit_Exception_Details.FirstOrDefault(e => e.ExceptionId == Id);
            if (exception == null)
            {
                return HttpNotFound();
            }
            if (exception.S_Status != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            exception.S_Status = 0;      
            exception.S_RejectedBy = Session["UserId"].ToString();
            context.Entry(exception).State = EntityState.Modified;
            //Log Table Update
            AUDIT_UPDATE_LOG logs = new AUDIT_UPDATE_LOG
            {
                Id = (context.AUDIT_UPDATE_LOG.Any() ? context.AUDIT_UPDATE_LOG.Max(e => e.Id) : 0) + 1,
                IP = Request.ServerVariables["REMOTE_ADDR"].ToString(),
                USERID = Session["UserId"].ToString(),
                TIME = DateTime.Now,
                ACTIVITY = "Settlement Reject",
                ACTIVITY_VALUES = "Exception ID:" + exception.ExceptionId + ",BLOCK_ID:" + exception.Block_Id + ",FROM_YEAR:" + exception.Year + ",To_YEAR::" + exception.ToYear + "Exception No:" + exception.ExceptionNo + " " + exception.ExceptionSubNo + "Title" + exception.ExceptionTitle
                                    + ",Settlement_Remark:" + exception.S_Remark + ",Settlement_Doc_Name:" + exception.S_Doc_Address + ",Settlement InitiatedBy: " + exception.S_InitiatedBy + "Settlement RejectedBy:" + exception.S_RejectedBy,
                TABLE_NAME = "AUDIT_EXCEPTION_DETAILS"
            };
            context.AUDIT_UPDATE_LOG.Add(logs);


            context.SaveChanges();
            return RedirectToAction("Details", new { id = exception.ExceptionId });
        }

    }
}