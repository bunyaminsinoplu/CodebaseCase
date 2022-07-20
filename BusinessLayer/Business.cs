using DataAccessLayer.Global;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Business : IBusiness
    {
        private readonly CodebaseCaseContext _context;
        public Business(CodebaseCaseContext context)
        {
            _context = context;
        }

        #region Get Products
        public async Task<List<Product>> GetProducts(HttpContext context)
        {
            try
            {
                List<Product> model = await _context.Products.Where(c => c.IsDeleted != true)
                    .Include(c => c.Sales)
                    .ToListAsync();
                return model;
            }
            catch (Exception)
            {
                List<Product> model = new();
                return model;
            }
        }

        public async Task<Product> GetProduct(HttpContext context, int productId)
        {
            try
            {
                var rest = await _context.Products.FromSqlInterpolated($"SelectProduct {productId}").ToListAsync();

                return rest.FirstOrDefault();
            }
            catch (Exception)
            {
                Product model = new();
                return model;
            }
        }
        #endregion

        #region Create - Update - Delete - Sale Product
        public async Task<ResultFromBusiness> CreateProduct(HttpContext context, Product model)
        {

            ResultFromBusiness rest = new();
            try
            {
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                await _context.Products.AddAsync(model);
                await _context.SaveChangesAsync();

                rest.message = "Kayıt Başarılı";
                rest.statusCode = 200;
                return rest;

            }
            catch (Exception e)
            {
                rest.message = e.Message;
                rest.statusCode = 400;
                return rest;
            }
        }

        public async Task<ResultFromBusiness> UpdateProduct(HttpContext context, Product model)
        {
            ResultFromBusiness rest = new();
            try
            {
                Product existProduct = await _context.Products
                    .FirstOrDefaultAsync(c => c.Id == model.Id && c.IsDeleted != true);

                if (existProduct != null)
                {
                    existProduct.ProductName = model.ProductName;
                    existProduct.InPiece = model.InPiece;
                    existProduct.UpdatedDate = DateTime.Now;

                    _context.Entry(existProduct).State = EntityState.Modified;


                    await _context.SaveChangesAsync();

                    rest.message = "Güncelleme Başarılı";
                    rest.statusCode = 200;
                    return rest;
                }
                else
                {
                    rest.message = "Kayıt bulunamadı";
                    rest.statusCode = 400;
                    return rest;
                }
            }
            catch (Exception e)
            {
                rest.message = e.Message;
                rest.statusCode = 400;
                return rest;
            }
        }

        public async Task<ResultFromBusiness> DeleteProduct(HttpContext context, int productId)
        {
            ResultFromBusiness rest = new();
            try
            {
                Product existProduct = await _context.Products
                    .FirstOrDefaultAsync(c => c.Id == productId && c.IsDeleted != true);

                if (existProduct != null)
                {
                    existProduct.UpdatedDate = DateTime.Now;
                    existProduct.IsDeleted = true;

                    _context.Entry(existProduct).State = EntityState.Modified;


                    await _context.SaveChangesAsync();

                    rest.message = "Silindi";
                    rest.statusCode = 200;
                    return rest;
                }
                else
                {
                    rest.message = "Kayıt bulunamadı";
                    rest.statusCode = 400;
                    return rest;
                }
            }
            catch (Exception e)
            {
                rest.message = e.Message;
                rest.statusCode = 400;
                return rest;
            }
        }

        public async Task<ResultFromBusiness> SaleProduct(HttpContext context, Sale model)
        {
            ResultFromBusiness rest = new();
            try
            {
                int onHand = (int)_context.Products.FirstOrDefault(c => c.Id == model.ProductId).InPiece;

                int totalSale = (int)await _context.Sales.Where(c => c.ProductId == model.ProductId).Select(c => c.Piece).SumAsync();

                if (totalSale + model.Piece > onHand)
                {
                    rest.message = "Alınan üründen fazlası satılamaz";
                    rest.statusCode = 400;
                    return rest;
                }
                else
                {

                    model.CreatedDate = DateTime.Now;

                    await _context.Sales.AddAsync(model);
                    await _context.SaveChangesAsync();

                    rest.message = "Kayıt Başarılı";
                    rest.statusCode = 200;
                    return rest;
                }


            }
            catch (Exception e)
            {
                rest.message = e.Message;
                rest.statusCode = 400;
                return rest;
            }
        }


        #endregion

        #region Get Report 
        public async Task<Report> GetReport(HttpContext context, Report model)
        {
            try
            {

                model.SaleList = await _context.Sales.Where(c => c.CreatedDate >= model.StartDate && c.CreatedDate <= model.EndDate)
                    .Include(c => c.Product)
                    .ToListAsync();

                if (model.SaleList.Count == 0)
                {
                    model.result = new()
                    {
                        message = "Satış bulunamadı.",
                        statusCode = 400,
                    };
                }
                else
                {
                    model.result = new()
                    {
                        message = "Başarılı",
                        statusCode = 200,
                    };
                }

                return model;
            }
            catch (Exception e)
            {
                model.result.message = e.Message;
                model.result.statusCode = 400;
                return model;
            }
        }
        #endregion
    }
}
