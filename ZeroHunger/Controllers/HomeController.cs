using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.Models;
using ZeroHunger.DTOs;


namespace ZeroHunger.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult AddRestaurant()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRestaurant(Restaurant restaurant)
        {
            using (var db = new ZeroHungerEntities())
            {
                db.Restaurants.Add(restaurant);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult RestaurantList()
        {
            using (var db = new ZeroHungerEntities())
            {
                var list = db.Restaurants.ToList();
                return View(list);
            }
        }

        
        public ActionResult Request(int id)
        {
            using (var db = new ZeroHungerEntities())
            {
                List<Restaurant> reslist = null;
                var restitem = db.Restaurants.Find(id);
                if (Session["request"] == null)
                {
                    reslist = new List<Restaurant>();
                }
                else
                {
                    reslist = (List<Restaurant>)Session["request"];
                }
                reslist.Add(restitem);
                Session["request"] = reslist;
                return RedirectToAction("RestaurantList");
            }
        }

        public ActionResult Requests()
        {
            var reslist = (List<Restaurant>)Session["request"];
            return View(reslist);
        }

      
        public ActionResult AddtoReqTable()
        {
            using (var db = new ZeroHungerEntities())
            {
                var cart = (List<Restaurant>)Session["request"];
                foreach (var p in cart)
                {
                    var od = new Request
                    {
                        rid = p.Id,
                        Location = p.Location,
                        MaxTime = p.MaxTime,
                        Foodname = p.Foodname,
                        Quantity = p.Quantity,
                        OrderStatus = "Available"
                    };
                    db.Requests.Add(od);
                }
                db.SaveChanges();
                Session["request"] = null;
                TempData["Msg"] = "Order Placed Successfully";
            }
            return RedirectToAction("ShowRequestList");
        }

        public ActionResult EmployeeList()
        {
            using (var db = new ZeroHungerEntities())
            {
                var list = db.Employees.ToList();
                return View(list);
            }
        }

        
        public ActionResult ShowEmployeeList(int id)
        {
            Session["Reqid"] = id;
            using (var db = new ZeroHungerEntities())
            {
                var list = db.Employees.ToList();
                return View(list);
            }
        }

        [HttpGet]
        public ActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee employee)
        {
            using (var db = new ZeroHungerEntities())
            {
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            return RedirectToAction("EmployeeList");
        }

    
        public ActionResult ShowRequestList()
        {
            using (var db = new ZeroHungerEntities())
            {
                var list = db.Requests.ToList();
                return View(list);
            }
        }

        public ActionResult ProcessEmployee()
        {
            using (var db = new ZeroHungerEntities())
            {
                var emp = (Employee)Session["emp"];
                var c = (from item in db.Processes
                         where item.Eid == emp.Id
                         select item).ToList();
                return View(c);
            }
        }

        public ActionResult Delivered(int id)
        {
            using (var db = new ZeroHungerEntities())
            {
                var process = db.Processes.Find(id);
                process.EmpStatus = "";
                process.OrderStatus = "Delivered";
                TempData["Message"] = "Successfully Delivered";
                db.SaveChanges();
            }
            return RedirectToAction("ProcessEmployee");
        }

        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdmin(AdminLogin adminlogin)
        {
            using (var db = new ZeroHungerEntities())
            {
                var c = (from item in db.Admins
                         where item.username == adminlogin.username
                         && item.password == adminlogin.password
                         select item).SingleOrDefault();

                if (c == null)
                {
                    TempData["Message"] = "Wrong username and password";
                    return View();
                }
                TempData["Message"] = "Welcome Admin";
                return RedirectToAction("ShowRequestList");
            }
        }

        [HttpGet]
        public ActionResult EmployeeLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeLogin(EmployeeLogin employeeLogin)
        {
            using (var db = new ZeroHungerEntities())
            {
                var c = (from item in db.Employees
                         where item.Username == employeeLogin.Username
                         && item.Password == employeeLogin.Password
                         select item).SingleOrDefault();

                if (c == null)
                {
                    TempData["Message"] = "Wrong username and password";
                    return View();
                }
                TempData["Message"] = "Welcome Employee!";
                Session["emp"] = c;
                return RedirectToAction("ProcessEmployee");
            }
        }

        public ActionResult GetEmployee(int id)
        {
            using (var db = new ZeroHungerEntities())
            {
                var eid = (from item in db.Employees
                           where item.Id == id
                           select item).SingleOrDefault();
                eid.EmpStatus = "Assigned";

                var process = new Process();
                process.Rid = (int)Session["ReqId"];
                var req = (from item in db.Requests
                           where item.Id == process.Rid
                           select item).SingleOrDefault();
                process.Eid = id;
                process.Foodname = req.Foodname;
                process.Quantity = req.Quantity;
                process.EmpStatus = eid.EmpStatus;
                process.OrderStatus = req.OrderStatus;
                db.Processes.Add(process);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult RestaurantLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RestaurantLogin(RestaurantLogin restaurantLogin)
        {
            using (var db = new ZeroHungerEntities())
            {
                var c = (from item in db.Restaurants
                         where item.Username == restaurantLogin.Username
                         && item.Password == restaurantLogin.Password
                         select item).SingleOrDefault();
                if (c == null)
                {
                    TempData["Message"] = "Wrong username and password";
                    return View();
                }
                TempData["Message"] = "Welcome to ZeroHunger!";
                Session["res"] = c;
                Session["ResId"] = (int)c.Id;
                return RedirectToAction("RestaurantDetails");
            }
        }

        public ActionResult RestaurantDetails()
        {
            using (var db = new ZeroHungerEntities())
            {
                int reid = (int)Session["ResId"];
                Restaurant restaurant = db.Restaurants.Find(reid);
                List<Restaurant> restitem = new List<Restaurant> { restaurant };
                return View(restitem);
            }
        }
    }
}