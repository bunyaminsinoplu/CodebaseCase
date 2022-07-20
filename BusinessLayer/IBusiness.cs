using DataAccessLayer.Global;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer
{
    public interface IBusiness
    {
        #region Gets
        Task<List<Product>> GetProducts(HttpContext context);
        Task<Product> GetProduct(HttpContext context, int productId);
        #endregion

        #region Create & Update & Delete && Sale
        Task<ResultFromBusiness> CreateProduct(HttpContext context, Product model);
        Task<ResultFromBusiness> UpdateProduct(HttpContext context, Product model);
        Task<ResultFromBusiness> DeleteProduct(HttpContext context, int productId);
        Task<ResultFromBusiness> SaleProduct(HttpContext context, Sale model);
        #endregion

        #region Report 
        Task<Report> GetReport(HttpContext context, Report model);

        #endregion
    }
}
