using Microsoft.AspNetCore.Mvc;
using _1131651.Data;
using _1131651.Models;
using System.Linq;

namespace _1131651.Controllers
{
    public class AccountController : Controller
    {
        private readonly CmsContext _context;

        public AccountController(CmsContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Register(User user)
        {
           
            bool accountExists = _context.TableUsers1131651
                .Any(u => u.Account == user.Account);

            if (accountExists)
            {
                ViewBag.Message = "此帳號已被註冊";

                return View(user);
            }

           
            _context.TableUsers1131651.Add(user);

            _context.SaveChanges();

          
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var loginUser = _context.TableUsers1131651
                .FirstOrDefault(m => m.Account == user.Account && m.Password == user.Password);

            if (loginUser != null)
            {
                HttpContext.Session.SetInt32("UserId", loginUser.UserId);

                HttpContext.Session.SetString("UserName", loginUser.UserName);

                TempData["LoginMessage"] = $"{loginUser.UserName}，你好！登入成功";

                return RedirectToAction("Index", "Recharge");
            }

            ViewBag.Message = "帳號或密碼錯誤";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult MemberCenter()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.TableUsers1131651
                .FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var themeSetting = _context.TableUserThemeSettings1131651
                .FirstOrDefault(t => t.UserId == userId.Value);

            ViewBag.ThemeCode = themeSetting == null ? "blue" : themeSetting.ThemeCode;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View(user);
        }


        [HttpPost]
        public IActionResult MemberCenter(User model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.TableUsers1131651
                .FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            bool accountExists = _context.TableUsers1131651
                .Any(u => u.Account == model.Account && u.UserId != userId.Value);

            if (accountExists)
            {
                var themeSetting = _context.TableUserThemeSettings1131651
                    .FirstOrDefault(t => t.UserId == userId.Value);

                ViewBag.ThemeCode = themeSetting == null ? "blue" : themeSetting.ThemeCode;
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.Message = "此帳號已被使用，請更換其他帳號";

                return View(model);
            }

            user.Account = model.Account;
            user.Password = model.Password;
            user.UserName = model.UserName;

            _context.SaveChanges();

            HttpContext.Session.SetString("UserName", user.UserName);

            TempData["MemberMessage"] = "會員資料已更新";

            return RedirectToAction("MemberCenter");
        }


        [HttpPost]
        public IActionResult DeleteAccount()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.TableUsers1131651
                .FirstOrDefault(u => u.UserId == userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

           
            var themeSettings = _context.TableUserThemeSettings1131651
                .Where(t => t.UserId == userId.Value)
                .ToList();

            _context.TableUserThemeSettings1131651.RemoveRange(themeSettings);

            var records = _context.TableRechargeRecords1131651
                .Where(r => r.UserId == userId.Value)
                .ToList();

            var recordIds = records.Select(r => r.RecordId).ToList();

            foreach (var record in records)
            {
                var details = _context.TableRechargeDetails1131651
                    .Where(d => d.RecordId == record.RecordId)
                    .ToList();

                _context.TableRechargeDetails1131651.RemoveRange(details);
            }

           
            _context.TableRechargeRecords1131651.RemoveRange(records);

            _context.TableUsers1131651.Remove(user);

            _context.SaveChanges();

            HttpContext.Session.Clear();

            TempData["LogoutMessage"] = "會員資料已刪除";

            return RedirectToAction("Login", "Account");
        }
    }
}