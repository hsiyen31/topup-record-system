using Microsoft.AspNetCore.Mvc;
using _1131651.Data;
using _1131651.Models;
using _1131651.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using X.PagedList.Extensions;
namespace _1131651.Controllers
{
    public class RechargeController : Controller
    {
        private readonly CmsContext _context;

        public RechargeController(CmsContext context)
        {
            _context = context;
        }
        private string GetUserThemeCode(int userId)
        {
            var themeSetting = _context.TableUserThemeSettings1131651
                .FirstOrDefault(t => t.UserId == userId);

            return themeSetting == null ? "blue" : themeSetting.ThemeCode;
        }

        public IActionResult Index(string keyword, string sortType, int? page)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.IsPreview = Request.Query["preview"] == "true";

            var themeSetting = _context.TableUserThemeSettings1131651
                .FirstOrDefault(t => t.UserId == userId);

            string previewTheme = Request.Query["theme"];

            if (!string.IsNullOrEmpty(previewTheme))
            {
                ViewBag.ThemeCode = previewTheme;
            }
            else
            {
                ViewBag.ThemeCode = themeSetting == null ? "blue" : themeSetting.ThemeCode;
            }

            var records = _context.TableRechargeRecords1131651
                .Where(r => r.UserId == userId);

            if (!string.IsNullOrEmpty(keyword))
            {
                records = records.Where(r => r.PoolName.Contains(keyword));
            }

            switch (sortType)
            {
                case "oldest":
                    records = records.OrderBy(r => r.CreatedAt);
                    break;

                case "high":
                    records = records.OrderByDescending(r => r.TotalAmount);
                    break;

                case "low":
                    records = records.OrderBy(r => r.TotalAmount);
                    break;

                default:
                    records = records.OrderByDescending(r => r.CreatedAt);
                    break;
            }

            ViewBag.SortType = sortType;
            ViewBag.Keyword = keyword;
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            int totalAmount = _context.TableRechargeRecords1131651
                .Where(r => r.UserId == userId)
                .Sum(r => (int?)r.TotalAmount) ?? 0;

            ViewBag.TotalAmount = totalAmount;

            var recordList = records.ToList();

            var viewModelList = recordList.Select(r => new RechargeRecordViewModel
            {
                RecordId = r.RecordId,
                PoolName = r.PoolName,
                PoolTypeId = r.PoolTypeId,
                TotalAmount = r.TotalAmount,
                CreatedAt = r.CreatedAt,

                PoolTypeName = _context.TablePoolTypes1131651
                    .FirstOrDefault(p => p.PoolTypeId == r.PoolTypeId)
                    .PoolTypeName,

                Details = _context.TableRechargeDetails1131651
                    .Where(d => d.RecordId == r.RecordId)
                    .Join(
                        _context.TablePackages1131651,
                        d => d.PackageId,
                        p => p.PackageId,
                        (d, p) => new RechargeDetailViewModel
                        {
                            PackageName = p.PackageName,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            SubTotal = d.SubTotal
                        }
                    )
                    .ToList()

            }).ToList();

            var typeStats = recordList
                .GroupBy(r => r.PoolTypeId)
                .Select(g => new
                {
                    PoolTypeName = _context.TablePoolTypes1131651
                        .FirstOrDefault(p => p.PoolTypeId == g.Key)
                        .PoolTypeName,

                    TotalAmount = g.Sum(r => r.TotalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            ViewBag.TypeStats = typeStats;
            ViewBag.MaxTypeAmount = typeStats.Any()
                ? typeStats.Max(x => x.TotalAmount)
                : 0;

            int pageSize = 3;
            int pageNumber = page ?? 1;

            return View(viewModelList.ToPagedList(pageNumber, pageSize));
        }


        public IActionResult Create(int? poolTypeId, string? poolName)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.ThemeCode = GetUserThemeCode(userId.Value);
            var model = new CreateRechargeViewModel();
            model.PoolName = poolName ?? "";
            model.PoolTypes = _context.TablePoolTypes1131651
                .Select(p => new SelectListItem
                {
                    Value = p.PoolTypeId.ToString(),
                    Text = p.PoolTypeName
                })
                .ToList();

            if (poolTypeId != null)
            {
                model.PoolTypeId = poolTypeId.Value;

                model.PackageItems = _context.TablePackages1131651
                    .Where(p => p.PoolTypeId == poolTypeId.Value)
                    .Select(p => new PackageItemViewModel
                    {
                        PackageId = p.PackageId,
                        PackageName = p.PackageName,
                        Price = p.Price,
                        LimitQuantity = p.LimitQuantity,
                        Quantity = 0
                    })
                    .ToList();
            }

            return View(model);
        }

      
        [HttpPost]
        public IActionResult Create(CreateRechargeViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int totalAmount = model.PackageItems
                .Where(p => p.Quantity > 0)
                .Sum(p => p.Price * p.Quantity);

            var record = new RechargeRecord
            {
                UserId = userId.Value,
                PoolTypeId = model.PoolTypeId,
                PoolName = model.PoolName,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.Now
            };

            _context.TableRechargeRecords1131651.Add(record);
            _context.SaveChanges();

            foreach (var item in model.PackageItems.Where(p => p.Quantity > 0))
            {
                var detail = new RechargeDetail
                {
                    RecordId = record.RecordId,
                    PackageId = item.PackageId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    SubTotal = item.Price * item.Quantity
                };

                _context.TableRechargeDetails1131651.Add(detail);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        
        public IActionResult Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var record = _context.TableRechargeRecords1131651
                .FirstOrDefault(r => r.RecordId == id
                                  && r.UserId == userId);

            if (record != null)
            {
               
                var details = _context.TableRechargeDetails1131651
                    .Where(d => d.RecordId == record.RecordId)
                    .ToList();

                _context.TableRechargeDetails1131651.RemoveRange(details);

               
                _context.TableRechargeRecords1131651.Remove(record);

                _context.SaveChanges();

                TempData["DeleteMessage"] = "已刪除儲值紀錄";
            }

            return RedirectToAction("Index");
        }

        
        public IActionResult Edit(int id, int? poolTypeId, string? poolName)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.ThemeCode = GetUserThemeCode(userId.Value);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var record = _context.TableRechargeRecords1131651
                .FirstOrDefault(r => r.RecordId == id && r.UserId == userId);

            if (record == null)
            {
                return RedirectToAction("Index");
            }

            var model = new CreateRechargeViewModel();

            model.RecordId = record.RecordId;

            model.PoolName =
                string.IsNullOrEmpty(poolName)
                ? record.PoolName
                : poolName;

            model.PoolTypeId =
                poolTypeId ?? record.PoolTypeId;

            model.PoolTypes = _context.TablePoolTypes1131651
                .Select(p => new SelectListItem
                {
                    Value = p.PoolTypeId.ToString(),
                    Text = p.PoolTypeName
                })
                .ToList();

            model.PackageItems = _context.TablePackages1131651
                .Where(p => p.PoolTypeId == model.PoolTypeId)
                .Select(p => new PackageItemViewModel
                {
                    PackageId = p.PackageId,
                    PackageName = p.PackageName,
                    Price = p.Price,
                    LimitQuantity = p.LimitQuantity,

                    Quantity =
                        _context.TableRechargeDetails1131651
                        .Where(d =>
                            d.RecordId == record.RecordId &&
                            d.PackageId == p.PackageId)
                        .Select(d => d.Quantity)
                        .FirstOrDefault()
                })
                .ToList();

            return View(model);
        }

        
        [HttpPost]
        public IActionResult Edit(CreateRechargeViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var record = _context.TableRechargeRecords1131651
                .FirstOrDefault(r =>
                    r.RecordId == model.RecordId &&
                    r.UserId == userId);

            if (record == null)
            {
                return RedirectToAction("Index");
            }

            int totalAmount = model.PackageItems
                .Where(p => p.Quantity > 0)
                .Sum(p => p.Price * p.Quantity);

            record.PoolName = model.PoolName;
            record.PoolTypeId = model.PoolTypeId;
            record.TotalAmount = totalAmount;

            _context.TableRechargeDetails1131651.RemoveRange(
                _context.TableRechargeDetails1131651
                .Where(d => d.RecordId == record.RecordId)
            );

            foreach (var item in model.PackageItems.Where(p => p.Quantity > 0))
            {
                var detail = new RechargeDetail
                {
                    RecordId = record.RecordId,
                    PackageId = item.PackageId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    SubTotal = item.Price * item.Quantity
                };

                _context.TableRechargeDetails1131651.Add(detail);
            }

            _context.SaveChanges();

            TempData["EditSuccess"] = "修改成功";

            return RedirectToAction("Index");
        }

        public IActionResult Theme()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var setting = _context.TableUserThemeSettings1131651
                .FirstOrDefault(t => t.UserId == userId);

            ViewBag.CurrentTheme = setting == null ? "blue" : setting.ThemeCode;
            ViewBag.ThemeCode = ViewBag.CurrentTheme;
            return View();
        }

        [HttpPost]
        public IActionResult Theme(string themeCode)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var setting = _context.TableUserThemeSettings1131651
                .FirstOrDefault(t => t.UserId == userId);

            if (setting == null)
            {
                setting = new UserThemeSetting
                {
                    UserId = userId.Value,
                    ThemeCode = themeCode
                };

                _context.TableUserThemeSettings1131651.Add(setting);
            }
            else
            {
                setting.ThemeCode = themeCode;
            }

            _context.SaveChanges();

            return RedirectToAction("Theme");
        }
    }
}