
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleLogin.Areas.Identity.Data;
using SampleLogin.Data;
using SampleLogin.Models;
using System.ComponentModel.Design;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace SampleLogin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public OrderController(AuthDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            IEnumerable<Order> allOrders = _db.Orders;
            IEnumerable<Order> filteredOrders = allOrders.Where(o => o.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && o.Status == "Pending");

            return View(filteredOrders);
        }

        public IActionResult Create(string storeName)
        {
            ViewData["StoreName"] = storeName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order obj)
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


        public IActionResult Accept(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Index"); // or RedirectToAction("Home");
            }

            var obj = _db.Orders.Find(id);
            if (obj == null)
            {

                return RedirectToAction("RiderOrder");
            }
            else
            {
                if (obj.Status == "Ongoing")
                {
                    return RedirectToAction("Fail");
                }
                else
                {
                    obj.RiderId = (User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                    obj.Status = "Ongoing";
                    _db.Orders.Update(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
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
                if(obj.Status != "Arrive")
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
        public IActionResult RiderOrder(Order obj)
        {
            IEnumerable<Order> allOrders = _db.Orders;
            IEnumerable<Order> filteredOrders = allOrders.Where(o => o.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && o.Status == "Pending");

            return View(filteredOrders);
        }

        public IActionResult Status()

        {
            IEnumerable<Order> allOrders = _db.Orders;
            return View(allOrders);
        }

        public IActionResult Store()
        {
            IEnumerable<Store> allOrders = _db.Stores;
            return View(allOrders);
        }

        public IActionResult Fail()
        {
            return View();
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
                    return RedirectToAction("Status");
            }
        }

    }
}