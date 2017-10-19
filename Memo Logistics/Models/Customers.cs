using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Memo_Logistics.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Cargo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public int? DriverID { get; set; }

       
        public virtual Drivers Driver { get; set; }
        public virtual ICollection<Trucks> Trucks { get; set; }

    }
}