using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace QUANLYBANHANG_FOOD.Controllers
{

    public class TrangchuController : Controller
    {
        QUANLYBANHANG_FOODEntities1 DB = new QUANLYBANHANG_FOODEntities1();

        private const string CartSession = "CartSession";
        // GET: Trangchu
        public ActionResult Index(int? page)
        {
            int pagesize = 6;
            int pagenumber=page==null||page<0?1:page.Value;
            var listsp = DB.SANPHAM.ToList();
            PagedList<SANPHAM> list = new PagedList<SANPHAM>(listsp, pagenumber, pagesize);

            ViewBag.CartQuantity = GetCartQuantity(); // Thêm dòng này
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("Index", "Login");
            }
            return View(list);
        }
        public ActionResult ProducsCate(int? page, int? categoryId)
        {
            if (categoryId == null)
            {
                // Nếu categoryId không được cung cấp, chuyển hướng hoặc xử lý theo ý của bạn.
                return RedirectToAction("Index");
            }
            int pagesize = 6;
            int pagenumber = page == null || page < 0 ? 1 : page.Value;

            var listSPDM = DB.SANPHAM.Where(p => p.MaDM == categoryId).ToList();
            PagedList<SANPHAM> list = new PagedList<SANPHAM>(listSPDM, pagenumber, pagesize);
            
            ViewBag.Page = pagenumber;
            ViewBag.CategoryId = categoryId;

            ViewBag.CartQuantity = GetCartQuantity(); // Thêm dòng này

            // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
               
                return RedirectToAction("Index", "Login");
            }

            // Trả về PartialView chứa danh sách sản phẩm
            return View(list);
        }
        public ActionResult Detail(int? productId)
        {
            if (productId == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            //var product = DB.SANPHAM.Find(productId);
            var product = DB.SANPHAM.Where(p => p.MaSP == productId).ToList();
            if (product == null)
            {
                return HttpNotFound(); // Xử lý khi không tìm thấy sản phẩm
            }

            ViewBag.CartQuantity = GetCartQuantity(); // Thêm dòng này
            
            // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {

                return RedirectToAction("Index", "Login");
            }
            return View(product);
        }
        public ActionResult cart()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }

            ViewBag.CartQuantity = GetCartQuantity(); // Thêm dòng này
            
            // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {

                return RedirectToAction("Index", "Login");
            }
            return View(list);
        }
        public ActionResult AddItem(int productId, int quantity)
        {

            var product = DB.SANPHAM.FirstOrDefault(c => c.MaSP == productId);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.shoppingsp.MaSP == productId))
                {

                    foreach (var item in list)
                    {
                        if (item.shoppingsp.MaSP == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new CartItem();
                    item.shoppingsp = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item
                var item = new CartItem();
                item.shoppingsp = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //Gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("cart");
        }
       
        [HttpPost]
        public ActionResult UpdateCartItem(int[] productIds, int[] quantities)
        {
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;

                // Cập nhật số lượng cho từng sản phẩm
                for (int i = 0; i < productIds.Length && i < quantities.Length; i++)
                {
                    var productId = productIds[i];
                    var quantity = quantities[i];

                    var cartItem = list.FirstOrDefault(x => x.shoppingsp.MaSP == productId);

                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity;
                    }
                }

                // Cập nhật lại session
                Session[CartSession] = list;
                ViewBag.CartQuantity = GetCartQuantity();
            }

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("cart");
        }

        private int GetCartQuantity()
        {
            var cart = Session[CartSession];
            var list = cart as List<CartItem>;

            //Nếu list khác null thì trả về tổng số lượng sản phẩm có trong giỏ hàng, nếu null thì trả về 0
            return list?.Sum(item => item.Quantity) ?? 0;
        }
        [HttpPost]
        public ActionResult removeCartItem(int[] selectedProductIds)
        {


            if (selectedProductIds != null)
            {
                var cart = Session[CartSession];
                if (cart != null)
                {
                    var list = (List<CartItem>)cart;

                    if (list != null && list.Any())
                    {
                        // Loại bỏ các sản phẩm có MaSP nằm trong danh sách selectedProductIds
                        list = list.Where(item => !selectedProductIds.Contains(item.shoppingsp.MaSP)).ToList();

                        // Lưu lại giỏ hàng sau khi xóa vào session
                        Session[CartSession] = list;
                    }
                }
            }

            // Sau khi xóa, có thể làm điều gì đó như redirect hoặc trả về một view
            return RedirectToAction("cart");
        }
        public ActionResult quanlysanpham(int? page)
        {
            int pagesize = 5;
            int pagenumber = page == null || page < 0 ? 1 : page.Value;
            var ds = DB.SANPHAM.ToList();
            PagedList<SANPHAM> dssp = new PagedList<SANPHAM>(ds, pagenumber, pagesize);

            ViewBag.CartQuantity = GetCartQuantity();
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("Index", "Login");
            }
            return View(dssp);
        }

    }
}
