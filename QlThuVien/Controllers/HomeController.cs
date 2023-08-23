using Newtonsoft.Json.Schema;
using PagedList;
using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebMatrix.WebData;


namespace QlThuVien.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private QLTVEntities db = new QLTVEntities();
        public UserController us = new UserController();
        public ActionResult Index()
        {
            return View();
        }

        

        //public ActionResult Contact()
        //{

        //    return PartialView("ContactPartial");
        //}

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult Product(int? page)
        {
            int iSize = 10;
            int iPageNum = (page ?? 1);
            var a = db.Books;
            return View(a.OrderBy(s => s.BookId).ToPagedList(iPageNum,iSize));
        }

        public ActionResult Details()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult SideBar()
        {
            var category = from c in db.Categories select c;
            return PartialView(category);
        }

        public ActionResult Borrow(int? bookId)
        {
            
            int usid = us.getUserId();
            if (usid == 0)
            {
                return RedirectToAction("DangNhap", "User");
            }
            /*try
            {*/

            if (bookId != null)
                {
                    
                    Book book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                    if (book == null)
                    {
                        return HttpNotFound();
                    }
                    if (book.Quantity > 0)
                    {
                        book.Quantity = book.Quantity - 1;
                        Transaction trans = new Transaction()
                        {
                            BookId = book.BookId,
                            TranDate = DateTime.Now.ToString(),
                            TranDate_Get = DateTime.Now.AddDays(3).ToString(),
                            TranDate_GiveBack = DateTime.Now.AddDays(7).ToString(),
                            TranStatus = "Requested",
                            UserId = usid,
                        };
                        db.SaveChanges();
                        db.Transactions.Add(trans);
                        db.SaveChanges();
                        Session["requestMsg"] = "Requested successfully";
                    }
                    else
                    {
                        Session["requestMsg"] = "Sorry you cant take, Book copy is zero";
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
           
            
            return RedirectToAction("Product", "Home");
            /*}
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
        }


        public ActionResult MenuRequested(int userId)
        {
            return RedirectToAction("Requested", "UserTransactions", new { userId = userId });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Review()
        {
            return View();
        }

        public JsonResult GetViewBook(int? bookId)
        {
            try
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                book.ViewTotal += 1;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}