using Hiwkao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleLogin.Areas.Identity.Data;
using SampleLogin.Data;
using SampleLogin.Models;
using System.Security.Claims;

namespace Hiwkao.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public CustomerController(AuthDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            var viewModel = new OrderStoreViewModel();
            {
                // Filter the collection of Order objects based on your criteria
                IEnumerable<Order> allOrders = _db.Orders;
                IEnumerable<Order> filteredOrders = allOrders.Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && o.Status == "Pending");

                // Assign the filtered collection to the Orders property of the viewModel
                viewModel.Orders = filteredOrders;

                // Retrieve the collection of Store objects from the database
                IEnumerable<Store> allStores = _db.Stores;

                // Assign the collection to the Stores property of the viewModel
                viewModel.Stores = allStores;
            }

            return View(viewModel);
        }

        public IActionResult Order()
        {
            IEnumerable<Store> allOrders = _db.Stores;
            return View(allOrders);
        }

        public IActionResult CreateOrder(string storeName)
        {
            ViewData["StoreName"] = storeName;
            var obj = _db.Stores.FirstOrDefault(s => s.name == storeName);
            ViewBag.StoreImage = obj.StoreImage;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder(Order obj)
        {

            for (int i = 1; i <= 5; i++)
            {
                var menuPropertyName = $"Menu{i}";
                var menuPropertyValue = (string)obj.GetType().GetProperty(menuPropertyName).GetValue(obj);
                if (menuPropertyValue == null)
                {
                    obj.GetType().GetProperty(menuPropertyName).SetValue(obj, "No");
                }
            }

            obj.UserId = (User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "").ToString();
            obj.RiderId = "Waiting";
            obj.Status = "Pending";
            _db.Orders.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Status()

        {
            var viewModel = new OrderStoreViewModel();
            {
                IEnumerable<Order> allOrders = _db.Orders;
                IEnumerable<Order> filteredOrders = allOrders.Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (o.Status == "Ongoing" || o.Status == "Arrive" || o.Status == "Canceling"));
                viewModel.Orders = filteredOrders;
                IEnumerable<Store> allStores = _db.Stores;
                viewModel.Stores = allStores;
            }


            return View(viewModel);
        }

        public IActionResult Cancel(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Orders.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                _db.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        public IActionResult Finish(int? id)

        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Orders.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                if (obj.Status != "Arrive")
                {
                    obj.Status = "Arrive";
                    obj.Confirmation = 1;
                    _db.SaveChanges();
                    _db.Orders.Update(obj);
                    return RedirectToAction("Status");
                }
                else
                {
                    obj.Status = "Success";
                    _db.SaveChanges();
                    _db.Orders.Update(obj);
                    return RedirectToAction("Status");
                }


            }
        }

        public ActionResult GoBack()
        {

           return RedirectToAction("Order");

        }
    }
}
