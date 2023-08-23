using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;



namespace QlThuVien.Controllers
{
    public class UserController : Controller
    {
        private QLTVEntities db = new QLTVEntities();

        static int userId = 0;
        public int getUserId()
        {
            return userId;
        }
        public static string Encrypt(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            encrypt = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());



            }
            return encryptdata.ToString();
        }

        public ActionResult DangKy()
        {
            return View();
        }



        [HttpPost]
        public ActionResult DangKy(FormCollection collection, User kh)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["Matkhau"];
            var sMSSV = collection["MSSV"];
            var sNhapLaiMatKhau = collection["MatkhauNL"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DT"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "*Tên không được để trống";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "*Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(sNhapLaiMatKhau))
            {
                ViewData["err4"] = "*Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err6"] = "*Email không được để trống";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err7"] = "*Điện thoại không được để trống";
            }
            else if (db.Users.SingleOrDefault(n => n.UserName == sTenDN) != null)
            {
                ViewData["err8"] = "*Tên đăng nhâp đã tồn tại";
            }
            else if (db.Users.SingleOrDefault(n => n.UserEmail == sEmail) != null)
            {
                ViewData["err9"] = "*Email đã được sử dụng";
            }
            else if (db.Users.SingleOrDefault(n => n.UserPhone == sDienThoai) != null)
            {
                ViewData["err10"] = "*Số điện thoại đã được sử dụng";
            }
            else
            {
                try
                {
                    kh.UserName = sTenDN;
                    kh.UserPassword = Encrypt(sMatKhau);
                    kh.UserCode = sMSSV;
                    kh.UserEmail = sEmail;
                    kh.UserPhone = sDienThoai;
                    db.Users.Add(kh);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Console.WriteLine(e);
                }
                return RedirectToAction("DangNhap", "User");
            }
            return this.DangKy();
        }



        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }



        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {

            var sEmail = collection["Email"].ToString();
            var sMatKhau = Encrypt(collection["Matkhau"].ToString());
            if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err1"] = "*Email không được để trống";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err2"] = "*Mật khẩu không được để trống";
            }
            else
            {
                User kh = db.Users.SingleOrDefault(n => n.UserEmail == sEmail && n.UserPassword == sMatKhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "*Chúc mừng bạn đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    userId = kh.UserId;
                    Session["TaikhoanKH"] = kh.UserId;
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ViewBag.ThongBao = "*Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }



        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult LoginLogout()
        {
            return PartialView();
        }



        //[NonAction]
        //public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        //{
        //    var verifyUrl = "/User/" + emailFor + "/" + activationCode;
        //    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

        //    var fromEmail = new MailAddress("chuhaist123@gmail.com", "Cửa hàng TDT FoodStore");
        //    var toEmail = new MailAddress(email);

        //    string subject = "";
        //    string body = "";

        //    if (emailFor == "VerifyAccount")
        //    {
        //        subject = "Your account is successfully created";
        //        body = "<br /><br /> We are  <a href='" + link + "'>" + link + "</a>";
        //    }
        //    else if (emailFor == "ResetPassword")
        //    {
        //        subject = "Reset Password";
        //        body = "Chúng tôi xác nhận tài khoản này thuộc về bạn, vui lòng ấn vào <a href='" + link + "'> Khôi phục mật khẩu </a>";
        //    }

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = true,
        //        Credentials = new NetworkCredential(fromEmail.Address, "gvnggrtvigbpeihd")
        //    };
        //    using (var message = new MailMessage(fromEmail, toEmail)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true
        //    })
        //        smtp.Send(message);



        //}

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("chuhaist123@gmail.com", "Thư viện Đại học Thủ Dầu Một");
            var toEmail = new MailAddress(email);

            string subject = "";
            string body = "";

            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created";
                body = "<br /><br /> We are  <a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Chúng tôi xác nhận tài khoản này thuộc về bạn, vui lòng ấn vào <a href='" + link + "'> Khôi phục mật khẩu </a>";
            }

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



        }
        public ActionResult ForgotPassword()
        {
            return View();
        }



        public ActionResult HttpNotFound()
        {
            return View();
        }




        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            bool status = false;
            var account = db.Users.Where(a => a.UserEmail == Email).FirstOrDefault();
            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(account.UserEmail, resetCode, "ResetPassword");
                account.UserResetPassword = resetCode;
                db.SaveChanges();
                message = "Liên kết khôi phục mật khẩu đã được gửi đến email của bạn <3";
            }
            else
            {
                message = "Không tìm thấy email";
            }
            ViewBag.Message = message;
            return View();
        }


        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            var user = db.Users.Where(a => a.UserResetPassword == id).FirstOrDefault();
            if (user != null)
            {
                ResetPassword model = new ResetPassword();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]

        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.UserResetPassword == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.UserPassword = Encrypt(model.NewPassword);
                    user.UserResetPassword = " ";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Thay đổi mật khẩu thành công";

                }
                return RedirectToAction("DangNhap", "User");



            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);

        }

        public ActionResult DangNhap_DangKy()
        {
            return View();
        }



        public ActionResult ChangeInfo()
        {
            //
            User user = db.Users.Find(userId);
           

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInfo([Bind(Include = "UserId,UserCode,UserName,UserEmail,UserPassword,UserPhone")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Session.Remove("Taikhoan");
                Session["Taikhoan"] = user;
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

    }
}