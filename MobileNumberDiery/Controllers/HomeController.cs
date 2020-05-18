using MobileNumberDiery.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileNumberDiery.Controllers
{
    public class HomeController : Controller
    {
        MobileNumberDieryEntities database = new MobileNumberDieryEntities();

        [HttpGet]
        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(TBL_ADMIN admin)
        {
            TBL_ADMIN tblAdmin = database.TBL_ADMIN.Where(x => x.AD_NAME == admin.AD_NAME && x.AD_PASSWORD == admin.AD_PASSWORD).SingleOrDefault();
            if (tblAdmin != null)
            {
                Session["AD_ID"] = tblAdmin.AD_ID;
                return RedirectToAction("DiaryDashboard");
            }
            else
            {
                ViewBag.msg = "Invalid Username or Password";
            }
            return View();
        }
        [HttpGet]
        public ActionResult DiaryDashboard()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("DiaryDashboard");
            }
            int adid = Convert.ToInt16(Session["AD_ID"]);
            List<TBL_CATEGORY> category = database.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == adid).OrderBy(x => x.CAT_ID).ToList();
            ViewData["list"] = category;
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(TBL_CATEGORY cat)
        {
            Random r = new Random();
            if (cat.CAT_NAME != null)
            {
                List<TBL_CATEGORY> category = database.TBL_CATEGORY.OrderBy(x => x.CAT_ID).ToList();
                ViewData["list"] = category;

                TBL_CATEGORY tbl = new TBL_CATEGORY();
                tbl.CAT_NAME = cat.CAT_NAME;
                tbl.CAT_PK_ADID = cat.CAT_PK_ADID;
                tbl.CAT_PK_ADID = Convert.ToInt32(Session["AD_ID"].ToString());
                database.TBL_CATEGORY.Add(tbl);
                database.SaveChanges();
                return RedirectToAction("AddCategory");
            }
            else
            {
                ViewBag.Message = "Please enter catagery";
                RedirectToAction("AddCategory");
            }
            return View();
        }
        [HttpGet]
        public ActionResult RegisterContact()
        {
            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> cat = database.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == sid).ToList();
            ViewBag.list = new SelectList(cat, "CAT_ID", "CAT_NAME");
            return View();
        }
        [HttpPost]
        public ActionResult RegisterContact(DieryRecord record)
        {
            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> cat = database.TBL_CATEGORY.Where(x => x.CAT_PK_ADID == sid).ToList();
            ViewBag.list = new SelectList(cat, "CAT_ID", "CAT_NAME");

            DieryRecord reco = new DieryRecord();
            reco.Person_FirstName = record.Person_FirstName;
            reco.Person_LastName = record.Person_LastName;
            reco.Person_MobileNumber = record.Person_MobileNumber;
            reco.Person_HomeNumber = record.Person_HomeNumber;
            reco.Person_City = record.Person_City;
            reco.Person_Tehsil = record.Person_Tehsil;
            reco.Person_District = record.Person_District;
            reco.Person_State = record.Person_State;
            reco.R_FK_CATID = record.R_FK_CATID;
            database.DieryRecords.Add(reco);
            database.SaveChanges();
            TempData["msg"] = "Question added Successfully.....!";
            TempData.Keep();
            return RedirectToAction("RegisterContact");
        }
        [HttpGet]
        public ActionResult ViewAllContact(int? id)
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("AdminLogin");
            }

            if (id == 0)
            {
                return RedirectToAction("DiaryDashboard");
            }
            return View(database.DieryRecords.Where(x => x.R_FK_CATID == id).ToList());
        }
        public ActionResult GetNumberList()
        {
            int adid = Convert.ToInt16(Session["AD_ID"]);
            List<DieryRecord> category = database.DieryRecords.OrderBy(x => x.Person_ID).ToList();
            return View(category);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            
            DieryRecord record = new DieryRecord();
           
            var Studentdata = database.DieryRecords.Where(x => x.Person_ID == id).SingleOrDefault();
            
            ViewBag.R_FK_CATID = new SelectList(database.DieryRecords, "Person_ID", "R_FK_CATID", record.Person_ID);
            return View(Studentdata);

        }
        [HttpPost]
        public ActionResult Edit(DieryRecord record)
        {

            
                database.Entry(record).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("GetNumberList");
            
            
        }
        // GET: /Student/Delete/5 
        public ActionResult Delete(int id )
        {
            DieryRecord student = database.DieryRecords.Where(x => x.Person_ID==id).SingleOrDefault();
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }
        // POST: /Student/Delete/5   
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DieryRecord student = database.DieryRecords.Find(id);
            database.DieryRecords.Remove(student);
            database.SaveChanges();
            return RedirectToAction("GetNumberList");
        }
    }
}