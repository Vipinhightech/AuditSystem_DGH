using AuditSystem.Core.ViewModels;
using ExcelDataReader;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AuditSystem.WebUI.Controllers
{
    public class UploadExcelController : Controller
    {
        // GET: UploadExcel
        public ActionResult UploadExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
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
                            var dataList = new List<AuditExceptionsExcel>();
                            for (int i = 1; i < dt.Rows.Count; i++)
                            {
                                var data = new AuditExceptionsExcel
                                {
                                    X_Id = Convert.ToInt32(dt.Rows[i][0]),
                                    Quantum = Convert.ToDouble(dt.Rows[i][1]),
                                    Remark = Convert.ToString(dt.Rows[i][2])
                                };
                                dataList.Add(data);
                            }
                            ViewBag.Message = "Data Imported Successfully";
                            return View(dataList);
                        }
                    }
                }
            }
          
                ViewBag.Message = "Please Upload file";
   
            return View();
        }
    }
}