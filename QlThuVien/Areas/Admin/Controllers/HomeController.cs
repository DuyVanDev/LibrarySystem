using Newtonsoft.Json;
using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace QlThuVien.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private QLTVEntities db = new QLTVEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Account");
            }
            var book = (from b in db.Books select b).Count();
            ViewBag.book = book;
            var user = (from u in db.Users select u).Count();
            ViewBag.user = user;
            var borrow = (from t in db.Transactions where t.TranStatus == "Accepted" select t).Count();
            ViewBag.borrow = borrow;
            var request = (from t in db.Transactions where t.TranStatus == "Requested" select t).Count();
            ViewBag.request = request;
            return View();
        }
        public ActionResult Requests()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Account");
            }
            var transactionList = db.GetAllRequest();
            var data_user = from s in db.Users select s;           
            ViewBag.data = data_user;
            return View(transactionList);
        }
        
        public ActionResult AcceptRequest(int? tranId)
        {
            /* try
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
            transaction.TranStatus = "Accepted_Request";
            //transaction.TranDate = DateTime.Now.ToShortDateString();
            transaction.TranDate_Accepted = DateTime.Now.ToString();
            db.SaveChanges();
            return RedirectToAction("Requests");
            /*}
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/

        }
        // Reject the book request. 
        public ActionResult RejectRequest(int? tranId)
        {
            /*try
            {*/
            
            Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranId);
            
            transaction.TranStatus = "Rejected";
            transaction.TranDate_DeleteRequest = DateTime.Now.ToString();
            Book book = db.Books.FirstOrDefault(b => b.BookId == transaction.BookId);
            book.Quantity = book.Quantity + 1;
            db.SaveChanges();
            return RedirectToAction("Requests");
            /*}
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
        }

        public ActionResult Returned()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Account");
            }
            var transactionList = db.getAllReturn();
            return View(transactionList);
        }
        // Returns all return books in json format.

        public ActionResult AcceptReturn(int? tranId)
        {

            /*try
            {*/
            if (tranId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(tranId);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            Book book = db.Books.FirstOrDefault(b => b.BookId == transaction.BookId);
            book.Quantity = book.Quantity + 1;
            transaction.TranDate_AccReturned = DateTime.Now.ToString();
            transaction.TranStatus = "Accepted_Return";
            db.SaveChanges();
            //db.Transactions.Remove(transaction);
            //db.SaveChanges();
            return RedirectToAction("Returned");
            /*}
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
        }

        public ActionResult AllUserBorrow()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Account");
            }
            var data = from t in db.Transactions
                       join b in db.Books on t.BookId equals b.BookId
                       join c in db.Categories on b.CategoryId equals c.CategoryId
                       join u in db.Users on t.UserId equals u.UserId
                       

                       select new InfoBook
                       {
                           //BookTitle = b.BookTitle,
                           //BookAuthor = b.BookAuthor,
                           //TranDate = t.TranDate,
                           //CategoryName = c.CategoryName,
                           //BookId = b.BookId,
                           //BookImage = b.BookImage,
                           //UserName = u.UserName,
                           //TranId = t.TranId,
                           //Email = u.UserEmail

                           BookTitle = b.BookTitle,
                           BookAuthor = b.BookAuthor,
                           TranDate = t.TranDate,
                           CategoryName = c.CategoryName,
                           BookId = b.BookId,
                           BookImage = b.BookImage,
                           UserName = u.UserName,
                           UserCode = u.UserCode,
                           UserPhone = u.UserPhone,
                           TranId = t.TranId,
                           TranStatus = t.TranStatus,
                           TranDate_Accepted = t.TranDate_Accepted,
                           TranDate_AccReturned = t.TranDate_AccReturned,
                           TranDate_Returned = t.TranDate_Returned,                          
                           TranDate_DeleteRequest = t.TranDate_DeleteRequest,
                           Email = u.UserEmail

                       };
            ViewBag.data = from s in db.Users select s;

            return View(data.ToList());
        }

        public ActionResult Details_Returned(int? id)
        {
            var ctsp = from s in db.Books where s.BookId == id select s;
            return View(ctsp.FirstOrDefault());
        }
        public ActionResult Details_Requests(int? id)
        {
            var ctsp = from s in db.Books where s.BookId == id select s;
            return View(ctsp.FirstOrDefault());
        }
        public ActionResult Details_AllBorrow(int? id)
        {
            var ctsp = from s in db.Books where s.BookId == id select s;
            return View(ctsp.FirstOrDefault());
        }

        public ActionResult SendMail_Request(int? id,int? tranid)  
        {
            User user = db.Users.FirstOrDefault(t => t.UserId == id);
            Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranid);
            string email = user.UserEmail.ToString();
            string TranDate_Get = transaction.TranDate_Get.ToString();
            var fromEmail = new MailAddress("chuhaist123@gmail.com", "Thư viện Đại học Thủ Dầu Một");
            var toEmail = new MailAddress(email);

            string subject = "";
            string body = "";
            subject = "Thông Báo";
            body = "<p>Yêu cầu mượn sách của bạn đã được duyệt. Xin vui lòng lấy sách trước ngày <strong>" + TranDate_Get + "</strong></p>\r\n<p><em>Lưu ý: Nếu Sinh viên lấy sách sau thời gian quy định, sẽ bị xử phạt theo quy định của nhà trường</em></p>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, "gvnggrtvigbpeihd")
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return RedirectToAction("Requests", "Home");

        }

        public ActionResult SendMail_Return(int? id, int? tranid)
        {
            User user = db.Users.FirstOrDefault(t => t.UserId == id);
            Transaction transaction = db.Transactions.FirstOrDefault(t => t.TranId == tranid);
            string email = user.UserEmail.ToString();
            string TranDate_GiveBack = transaction.TranDate_GiveBack.ToString();
            var fromEmail = new MailAddress("chuhaist123@gmail.com", "Thư viện Đại học Thủ Dầu Một");
            var toEmail = new MailAddress(email);

            string subject = "";
            string body = "";
            subject = "Thông Báo";
            body = "<p>Bạn đã yêu cầu trả sách. Xin vui lòng trả sách tại thư viện trường trước ngày <strong>" + TranDate_GiveBack + "</strong></p>\r\n<p><em>Lưu ý: Nếu Sinh viên trả sách sau thời gian quy định, sẽ bị xử phạt theo quy định của nhà trường</em></p>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, "gvnggrtvigbpeihd")
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return RedirectToAction("Returned", "Home");

        }
    }
}
