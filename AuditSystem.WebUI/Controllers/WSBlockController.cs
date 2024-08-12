using AuditSystem.Core.Models;
using AuditSystem.DataAccess.SQL;
using System;
using System.Collections.Generic;
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
            return View(db.WS_Block_Masters.ToList());
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
            db.WS_Block_Masters.Add(wsBlock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}