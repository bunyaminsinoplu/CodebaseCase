using BusinessLayer;
using CodebaseCase.Models;
using DataAccessLayer.Global;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodebaseCase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusiness _business;

        public HomeController(ILogger<HomeController> logger, IBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<Product> model = await _business.GetProducts(HttpContext);
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Giris");
            }
        }

        public async Task<IActionResult> ProductAddEdit(int? productId)
        {
            try
            {
                Product model;

                if (productId == null || productId == 0)
                {
                    model = new();
                }
                else
                {
                    model = await _business.GetProduct(HttpContext, (int)productId);

                    if (model == null)
                    {
                        ResultFromBusiness rest = new()
                        {
                            message = "Ürün bulunamadı",
                            statusCode = 400
                        };

                        return Json(rest);
                    }
                }

                return PartialView("~/Views/Home/_productCRUD.cshtml", model);
            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Home");
            }
        }

        public async Task<IActionResult> SaveProduct(Product model)
        {
            try
            {
                ResultFromBusiness rest;
                if (model.Id == 0)
                {
                    rest = await _business.CreateProduct(HttpContext, model);

                }
                else
                {
                    rest = await _business.UpdateProduct(HttpContext, model);
                }

                return Json(rest);

            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Giris");
            }
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {

                ResultFromBusiness rest = await _business.DeleteProduct(HttpContext, productId);

                return Json(rest);
            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Home");
            }
        }

        public IActionResult ProductSale(int productId)
        {
            try
            {
                Sale model = new()
                {
                    ProductId = productId
                };

                return PartialView("~/Views/Home/_sale.cshtml", model);
            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Home");
            }
        }

        public async Task<IActionResult> SaveSale(Sale model)
        {
            try
            {
                ResultFromBusiness rest;
                rest = await _business.SaleProduct(HttpContext, model);
                return Json(rest);

            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Giris");
            }
        }

        public IActionResult Report()
        {
            Report model = new();
            return View(model);
        }


        public async Task<IActionResult> GetReport(Report model)
        {
            try
            {
                model = await _business.GetReport(HttpContext, model);

                if (model.result.statusCode != 200)
                {
                    return Json(model.result);
                }

                return PartialView("~/Views/Home/_reportResult.cshtml", model);
            }
            catch (Exception)
            {
                return RedirectToAction("Hata", "Home");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}