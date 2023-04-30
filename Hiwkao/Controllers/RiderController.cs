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
    public class RiderController : Controller
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public RiderController(AuthDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            var viewModel = new OrderStoreViewModel();
            {
                IEnumerable<Order> allOrders = _db.Orders;
                IEnumerable<Order> filteredOrders = allOrders.Where(o => o.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && o.Status == "Pending");
                viewModel.Orders = filteredOrders;
                IEnumerable<Store> allStores = _db.Stores;
                viewModel.Stores = allStores;
            }

            return View(viewModel);
        }

        public IActionResult Status()

        {
            var viewModel = new OrderStoreViewModel();
            {
                IEnumerable<Order> allOrders = _db.Orders;
                IEnumerable<Order> filteredOrders = allOrders.Where(o => o.RiderId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (o.Status == "Ongoing" || o.Status == "Arrive" || o.Status == "Canceling"));
                viewModel.Orders = filteredOrders;
                IEnumerable<Store> allStores = _db.Stores;
                viewModel.Stores = allStores;
            }

            return View(viewModel);
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
                    return RedirectToAction("Status");
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
                obj.Status = "Canceling";
                obj.Confirmation = 1;
                _db.SaveChanges();
                _db.Orders.Update(obj);
                return RedirectToAction("Status");
            }
        }
    }
}
