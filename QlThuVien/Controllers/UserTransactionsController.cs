using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QlThuVien.Controllers
{
    public class UserTransactionsController : Controller
    {
        private QLTVEntities db = new QLTVEntities();
        UserController userController = new UserController();
        // GET: UserTransactions




        public ActionResult Requested()
        {
            int userId = userController.getUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "User"); ;
            }
            User user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "User"); ;
            }
            //UserTransactionController.userId = (int)userId;
            
            var requestList = db.Transactions.Where(s => s.TranStatus == "Requested" && s.UserId == userId);
            var data = from t in db.Transactions
                       join b in db.Books on t.BookId equals b.BookId
                       join c in db.Categories on b.CategoryId equals c.CategoryId
                       where t.TranStatus == "Requested"
                       where t.UserId == userId
                       select new InfoBook
                       {
                           BookTitle = b.BookTitle,
                           BookAuthor = b.BookAuthor,
                           CategoryName = c.CategoryName,
                           BookId = b.BookId,
                           BookImage = b.BookImage,
                           TranId = t.TranId,
                           TranStatus = t.TranStatus,
                           Description = b.Description,

                       };
            
           
            //String result1 = result.AddDays(3).ToString();
            
            if (requestList.Count() == 0)
            {
                Session["requestMessage"] = "Không có yêu cầu mượn sách.";
            }
            else
            {
                Session.Remove("requestMessage");
            }
            return View(data.ToList());
        }

        // Cancel book request, redirected to requested
        public ActionResult DeleteRequest(int? tranId)
        {
            /*try
            {*/
            if (tranId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            Book book = db.Books.FirstOrDefault(b => b.BookId == transaction.BookId);
            book.Quantity = book.Quantity + 1;
            transaction.TranDate_DeleteRequest = DateTime.Now.ToString();
            transaction.TranStatus = "DeleteRequested";
            db.SaveChanges();
            //db.Transactions.Remove(transaction);
            //db.SaveChanges();
            return RedirectToAction("Requested", "UserTransactions", new { userId = 18 });
            /* }
             catch (Exception)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }*/
        }

        public ActionResult Received()
        {

            int usid = userController.getUserId();
            if (usid == null)
            {
                return RedirectToAction("Login", "User");
            }
            User user = db.Users.Find(usid);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }


            var data = from t in db.Transactions
                       join b in db.Books on t.BookId equals b.BookId
                       join c in db.Categories on b.CategoryId equals c.CategoryId
                       where t.TranStatus == "Accepted_Request" || t.TranStatus == "Returned"
                       where t.UserId == usid
                       select new InfoBook
                       {
                           BookTitle = b.BookTitle,
                           BookAuthor = b.BookAuthor,
                           TranDate = t.TranDate,
                           TranDate_Get = t.TranDate_Get,
                           TranDate_GiveBack = t.TranDate_GiveBack,
                           TranStatus = t.TranStatus,
                           CategoryName = c.CategoryName,
                           BookId = b.BookId,
                           BookImage = b.BookImage,
                           TranId = t.TranId


                       };
         
            if (data.Count() == 0)
            {
                Session["receiveMessage"] = "Chưa có sách được mượn.";
            }
            else
            {
                Session.Remove("receiveMessage");
            }

            return View(data.ToList());

            //return JsonResult();
        }

        //public ActionResult ReturnReceived(int? tranId)
        //{
        //    int usid = userController.getUserId();
        //    /*try
        //    {*/
        //    if (tranId == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranId);
        //    if (transaction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //transaction.TranDate = DateTime.Now.ToShortDateString();
        //    transaction.TranStatus = "Returned";
        //    transaction.TranDate_Returned = DateTime.Now.ToString();
        //    db.SaveChanges();
        //    return RedirectToAction("Received", "UserTransactions");
        //    /* }
        //     catch (Exception)
        //     {
        //         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     }*/
        //}

        public JsonResult ReturnReceived(int? tranId)
        {
            int usid = userController.getUserId();
            /*try
            {*/
            //if (tranId == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            try
            {
                Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranId);
                //if (transaction == null)
                //{
                //    return HttpNotFound();
                //}
                transaction.TranDate_Returned = DateTime.Now.ToString();
                transaction.TranStatus = "Returned";
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            /* }
             catch (Exception)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }*/
        }

    }
}