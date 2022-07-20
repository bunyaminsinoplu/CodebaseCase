using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Global
{
    public class Report
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Sale> SaleList { get; set; }
        public ResultFromBusiness result { get; set; }
    }
}
