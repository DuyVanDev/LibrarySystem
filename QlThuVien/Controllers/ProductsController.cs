using PagedList;
using QlThuVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QlThuVien.Controllers
{
    [HandleError]
    public class ProductsController : Controller
    {
        // GET: Products
        QLTVEntities db = new QLTVEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SachTheoDanhMuc(int? masach, int? page)
        {
            ViewBag.masach = masach;
            int iSize = 10;
            int iPageNum = (page ?? 1);
            var dc = from c in db.Categories
                     join b in db.Books on c.CategoryId equals b.CategoryId
                     where c.CategoryId == masach
                     select b;
            return View(dc.OrderBy(s => s.BookId).ToPagedList(iPageNum, iSize));
        }

        
        public ActionResult ChiTietSanPham(int? id)
        {
            var ctsp = from s in db.Books where s.BookId == id select s;
            var data = db.Books.OrderBy(c => Guid.NewGuid()).ToList().Take(6).Except(ctsp);

            ViewBag.data = data;
            return View(ctsp.FirstOrDefault());
        }

        public ActionResult SearchInput()
        {
            return PartialView();
        }

        public ActionResult TimKiem(int? page,string SearchString = "", string keyword = "")
        {
            int iSize = 10;
            int iPageNum = (page ?? 1);
            var allProduct = db.Books;
            keyword = Request["keyword"];

            if(keyword == "BookAuthor")
            {
                
                var products = db.Books.Where(x => x.BookAuthor.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.search = SearchString;
                ViewBag.keyword = keyword;
                return View(products.OrderBy(s => s.BookId).ToPagedList(iPageNum, iSize));

            }
            else if (keyword == "BookTitle")
            {

                var products = db.Books.Where(x => x.BookTitle.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.search = SearchString;
                ViewBag.keyword = keyword;
                return View(products.OrderBy(s => s.BookId).ToPagedList(iPageNum, iSize));

            }
            else if (keyword == "ISBN")
            {

                var products = db.Books.Where(x => x.ISBN.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.search = SearchString;
                return View(products.OrderBy(s => s.BookId).ToPagedList(iPageNum, iSize));

            }
            else if (keyword == "ViewTotal")
            {
                           
                if(SearchString == "")
                {
                    return View(allProduct.OrderByDescending(s => s.ViewTotal).ToPagedList(iPageNum, iSize));
                }
                var products = db.Books.Where(x => x.BookTitle.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.search = SearchString;
                return View(products.OrderByDescending(s => s.ViewTotal).ToPagedList(iPageNum, iSize));

            }
            else
                return View(allProduct.OrderBy(s => s.BookId).ToPagedList(iPageNum, iSize));
            //List<Book> products = db.Books.Where(p => p.BookTitle.Contains(search)).ToList();

        }
    }
}