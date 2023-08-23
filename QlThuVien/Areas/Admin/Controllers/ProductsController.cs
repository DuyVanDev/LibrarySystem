using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QlThuVien.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Admin/Products
        QLTVEntities db = new QLTVEntities();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Account");
            }
            var book = from b in db.Books join c in db.Categories on b.CategoryId equals c.CategoryId select b ;

            return View(book);
        }

        public ActionResult Create()
        {
            ViewBag.DanhMuc = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");

            return View();
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Book book, FormCollection f, HttpPostedFileBase UploadImage, HttpPostedFileBase UploadFile)
        {
            try
            {
                if (UploadImage.ContentLength > 0 && UploadFile.ContentLength > 0)
                {
                    string _FileImage = Path.GetFileName(UploadImage.FileName);
                    string _pathImg = Path.Combine(Server.MapPath("~/UploadImage"), _FileImage);
                    string _FileName = Path.GetFileName(UploadFile.FileName);
                    string _pathName = Path.Combine(Server.MapPath("~/UploadFile"), _FileName);
                    UploadImage.SaveAs(_pathImg);
                    UploadFile.SaveAs(_pathName);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                book.BookTitle = f["sTensach"];
                book.Quantity = int.Parse(f["mSoluong"]);
                book.Description = f["sMoTa"].Replace("<p>", "").Replace("</p>", "");
                book.BookAuthor = f["Tacgia"];
                book.ISBN = f["isbn"];
                book.Publisher = f["nhaxuatban"];
                book.Source = f["source"];
                book.Date = f["date"];
                book.BookPosition = f["vitri"];
                book.CopyRights = f["copyright"];
                book.BookImage = UploadImage.FileName.Replace(" ", "%20");
                book.FileUpLoad = UploadFile.FileName.Replace(" ", "%20");

                book.CategoryId = int.Parse(f["DanhMuc"]);
                book.Language = f["Language"];

                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }

                return View();

        }
        //Edit
        public ActionResult Edit(int id)
        {
            ViewBag.DanhMuc = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");

            var book = db.Books.SingleOrDefault(n => n.BookId == id);
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {


                return View(book);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase UploadImage, HttpPostedFileBase UploadFile)
        {
            var book = db.Books.AsEnumerable().SingleOrDefault(n => n.BookId == int.Parse(f["bookid"]));
            if (ModelState.IsValid)
            {
                try
                {
                    if (UploadImage != null && UploadImage.ContentLength > 0) //kiểm tra để xác nhận đổi ảnh
                    {
                        

                        var newImage = Guid.NewGuid();
                        var renameImage = Path.GetExtension(UploadImage.FileName);
                        string newImageName = newImage + renameImage;
                        //lấy tên file, khai báo thư viện(System IO)

                        var sImage = Path.GetFileName(newImageName);

                        var path = Path.Combine(Server.MapPath("~/UploadImage"), sImage);
                        

                        UploadImage.SaveAs(path);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);

                        }
                        else
                        {
                            UploadImage.SaveAs(path);
                            book.BookImage = sImage;

                        }



                    }

                    if(UploadFile != null && UploadFile.ContentLength > 0)
                    {
                        var newFile = Guid.NewGuid();
                        var renamefile = Path.GetExtension(UploadFile.FileName);
                        string newName = newFile + renamefile;
                        string sFileName2 = Path.GetFileName(newName);
                        string path2 = Path.Combine(Server.MapPath("~/UploadFile"), sFileName2);
                        UploadFile.SaveAs(path2);
                        if ( !System.IO.File.Exists(path2))
                        {
                            System.IO.File.Delete(path2);

                        }
                        else
                        {
                            UploadFile.SaveAs(path2);
                            book.FileUpLoad = sFileName2;


                        }

                    }
                    


                    //}
                    book.BookTitle = f["sTensach"];
                    book.Quantity = int.Parse(f["mSoluong"]);
                    book.Description = f["sMoTa"].Replace("<p>", "").Replace("</p>", "");
                    //book.BookImage = fileImage;
                    //book.FileUpLoad = fileBook;
                    book.BookAuthor = f["Tacgia"];
                    book.ISBN = f["isbn"];
                    book.Publisher = f["nhaxuatban"];
                    book.Source = f["source"];
                    book.Date = f["date"];
                    book.BookPosition = f["vitri"];
                    book.CopyRights = f["copyright"];


                    book.CategoryId = int.Parse(f["DanhMuc"]);
                    book.Language = f["Language"];

                    db.SaveChanges();



                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ViewBag.Thongbao = "Chưa chọn file";
                }
                

                //Lưu sách vào cơ sở dử liệu

            }
            return View(book);
        }

        public ActionResult Delete(int id)
        {
            Book p = db.Books.SingleOrDefault(x=>x.BookId == id);

            db.Books.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            var ctsp = from s in db.Books where s.BookId == id select s;
            var data = db.Books.OrderBy(c => Guid.NewGuid()).ToList().Take(6).Except(ctsp);

            ViewBag.data = data;
            return View(ctsp.FirstOrDefault());
        }
    }
}