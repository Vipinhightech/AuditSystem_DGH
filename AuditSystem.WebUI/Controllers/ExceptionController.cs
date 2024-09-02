using AuditSystem.Core.Models;
using AuditSystem.Core.ViewModels;
using AuditSystem.DataAccess.SQL;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
                exception.Updated_Date = DateTime.Now.Date.ToString("ddMMyyyy");
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

            //ViewBag.Block_Name = new SelectList(context.AuditSystem_Blocks, "Block_Name", "Block_Name");
            //Audit_Exception_Details exception = new Audit_Exception_Details { FurtherQuery = new List<Audit_FurtherQuery_Details>() };
            //exception.Block = block;
            //exception.Block_Id = block.Block_Id;
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
                                var data = new AuditExceptionsExcel
                                {
                                    X_Id = Convert.ToInt32(dt.Rows[i][0]),
                                    Year = Convert.ToString(dt.Rows[i][1]),
                                    Name_Of_Auditor = Convert.ToString(dt.Rows[i][2]),
                                    ExceptionNo = Convert.ToInt32(dt.Rows[i][3]),
                                    ExceptionSubNo = Convert.ToString(dt.Rows[i][4]),
                                    NatureOfException = Convert.ToString(dt.Rows[i][5]),
                                    ExceptionTitle = Convert.ToString(dt.Rows[i][6]),
                                    ZistOfException = Convert.ToString(dt.Rows[i][7]),
                                    ExceptionType = Convert.ToString(dt.Rows[i][8]),
                                    Quantum = dt.Rows[i][9] != DBNull.Value ? Convert.ToDouble(dt.Rows[i][9]) : 0,
                                    OperatorsReply = Convert.ToString(dt.Rows[i][10]),
                                    CFComments = Convert.ToString(dt.Rows[i][11]),
                                    BlockCoordinatorsComments = Convert.ToString(dt.Rows[i][11]),
                                    FinalRecommendations = Convert.ToString(dt.Rows[i][12]),
                                    CurrentStatus = Convert.ToString(dt.Rows[i][13]),
                                    Remark = Convert.ToString(dt.Rows[i][14]),
                                    FurtherQuery = new List<Audit_FurtherQuery_Details>()
                                };
                                for(int j = 1; j < dt1.Rows.Count; j++)
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
                                }
                                dataList.Add(data);
                            }

                            if (dataList != null)
                            {
                               
                                int ExcpID = (context.Audit_Exception_Details.Any() ? context.Audit_Exception_Details.Max(e => e.ExceptionId) : 0) + 1;
                                int FqrID = (context.Audit_FurtherQuery_Details.Any() ? context.Audit_FurtherQuery_Details.Max(e => e.Id) : 0) + 1;
                                foreach (var item in dataList)
                                {
                                    Audit_Exception_Details exception = new Audit_Exception_Details();
                                    exception.ExceptionId = ExcpID;
                                    exception.Block_Id = block.Block_Id;
                                    // exception.ExceptionId = context.Audit_Exception_Details.Max(e => e.ExceptionId) + 1;
                                    exception.Updated_Date = DateTime.Now.Date.ToString("ddMMyyyy");
                                    exception.Updated_By = Session["UserId"].ToString();
                                    exception.S_Status = 0;
                                    exception.ActionTaken = "Unsettled";
                                    
                                    exception.Year = item.Year;
                                    exception.Name_Of_Auditor = item.Name_Of_Auditor;
                                    exception.ExceptionNo = item.ExceptionNo;
                                    exception.ExceptionSubNo = item.ExceptionSubNo;
                                    exception.NatureOfException = item.NatureOfException;
                                    exception.ExceptionTitle = item.ExceptionTitle;
                                    exception.ZistOfException = item.ZistOfException;
                                    exception.ExceptionType = item.ExceptionType;
                                    exception.Quantum = item.Quantum;
                                    exception.OperatorsReply = item.OperatorsReply;
                                    exception.CFComments = item.CFComments;
                                    exception.BlockCoordinatorsComments = item.BlockCoordinatorsComments;
                                    exception.FinalRecommendations = item.FinalRecommendations;
                                    exception.CurrentStatus = item.CurrentStatus;
                                    exception.Remark = item.Remark;

                                    if(item.FurtherQuery != null)
                                    {
                                        foreach(var fqr in item.FurtherQuery)
                                        {
                                            fqr.Id = FqrID;
                                            fqr.ExceptionId = ExcpID;
                                            FqrID++;
                                        }
                                        exception.FurtherQuery = item.FurtherQuery;
                                    }

                                    //if (exception.FurtherQuery != null)
                                    //{
                                    //    int FqrID = (context.Audit_FurtherQuery_Details.Any() ? context.Audit_FurtherQuery_Details.Max(e => e.Id) : 0) + 1;
                                    //    foreach (var fqr in exception.FurtherQuery)
                                    //    {
                                    //        fqr.Id = FqrID;
                                    //        fqr.ExceptionId = exception.ExceptionId;
                                    //        FqrID++;
                                    //    }
                                    //}

                                    context.Audit_Exception_Details.Add(exception);
                                    ExcpID++;
                                 
                                    //if (furtherQuery != null) 
                                    //{
                                    //    foreach (var fqr in furtherQuery) 
                                    //    {
                                    //        fqr.ExceptionId = exception.ExceptionId;
                                    //        context.Audit_FurtherQuery_Details.Add(fqr);
                                    //    }

                                    //    context.SaveChanges();
                                    //}

                                    //return RedirectToAction("Details", "BlockManager", new { Id = exception.Block_Id });
                                }
                                context.SaveChanges();


                            }








                            ViewBag.Message = "Data Imported Successfully";
                            return View(block);
                        }
                    }
                }
            }

            ViewBag.Message = "Please Upload file";

            return View();

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
                exception.Updated_Date = DateTime.Now.Date.ToString("ddMMyyyy");
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
            exception.S_Status = 1;
            exception.S_InitiatedBy = Session["UserId"].ToString();


           // attachment.Id = (context.Audit_Attachments.Any() ? context.Audit_Attachments.Max(e => e.Id) : 0) + 1;
            var fileExt = Path.GetExtension(file.FileName);
            var filename = $"SLetter_{exceptionId}_{DateTime.Now.ToString("ddMMyyyy")}{fileExt}";
            exception.S_Doc_Address = filename;
            file.SaveAs(Server.MapPath("~/Attachments/") + filename);
            context.Entry(exception).State = EntityState.Modified;
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
            context.SaveChanges();
            return RedirectToAction("Details", new { id = exception.ExceptionId });
        }

    }
}