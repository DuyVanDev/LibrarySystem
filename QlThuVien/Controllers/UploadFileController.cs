using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QlThuVien.Controllers
{
    
    public class UploadFileController : Controller
    {
        private QLTVEntities db = new QLTVEntities();
        // GET: UploadFile
        public ActionResult Index()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");
            return View();
        }

        public FileResult PDFFlyer(int id, string pathname)
        {
            //include the .pdf extention at the end
            var str = from c in db.Books where c.BookId == id select c.FileUpLoad;



            string path = Server.MapPath(String.Format("~/UploadFile/" + pathname));



            string mime = MimeMapping.GetMimeMapping(path);




            return File(path, mime);
        }



        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }



        [HttpPost]
        [ValidateInput(false)]

        public ActionResult Upload(Book book)
        {
            string finalPath = "\\UploadFile\\" + book.UploadFile.FileName;
            string finalPath1 = "\\UploadFile\\" + book.UploadImage.FileName;
            book.UploadFile.SaveAs(Server.MapPath("~") + finalPath);
            book.UploadImage.SaveAs(Server.MapPath("~") + finalPath1);



            var b = new Book()
            {
                BookTitle = book.BookTitle,
                CategoryId = book.CategoryId,
                BookAuthor = book.BookAuthor,
                BookPosition = book.BookPosition,
                BookStatus = book.BookStatus,
                Quantity = book.Quantity,
                FileUpLoad = book.UploadFile.FileName,
                BookImage = book.UploadImage.FileName
            };









            db.Books.Add(b);
            db.SaveChanges();
            return View();
        }




        public ActionResult DownloadFile(String file)
        {
            string path = Server.MapPath("~/UploadFile/");
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + @"\" + file);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }

    }
}