using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models
{
    public partial class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CustomerName { get; set; }
        public int? Piece { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Product Product { get; set; }
    }
}
