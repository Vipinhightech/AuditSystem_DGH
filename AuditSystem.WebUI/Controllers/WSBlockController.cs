using AuditSystem.Core.Models;
using AuditSystem.DataAccess.SQL;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuditSystem.WebUI.Controllers
{
    [Authorize]
    public class WSBlockController : Controller
    {
        private DataContext db = new DataContext();

        // GET: WSBlock
        public ActionResult Index()
        {
            return View(db.WS_BLOCK_MASTER.ToList());
        }
        [Authorize(Roles = "superuser")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "superuser")]
        public ActionResult Create(WS_Block_Master wsBlock)
        {
            if (!ModelState.IsValid)
            {
                return View(wsBlock);
            }
            //db.Database.ExecuteSqlCommand("INSERT INTO WS_BLOCK_MASTER (BLOCK_ID, BLOCK_NAME, PSC_START_DATE) VALUES (:Block_Id, :Block_Name, :Psc_Start_Date)",
            //new OracleParameter("Block_Id", wsBlock.Block_Id),
            //new OracleParameter("Block_Name", wsBlock.Block_Name),
            //new OracleParameter("Psc_Start_Date", wsBlock.Psc_Start_Date));
            db.WS_BLOCK_MASTER.Add(wsBlock);
            //db.Entry(wsBlock).State = EntityState.Added;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}