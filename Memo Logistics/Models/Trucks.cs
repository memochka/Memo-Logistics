using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Memo_Logistics.Models
{
    public class Trucks
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        [Key]
        public int TruckID { get; set; }

        [StringLength(50)]
        public string Truck { get; set; }
        public int Mileage { get; set; }

        public int? CustomerID { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual ICollection<Drivers> Drivers { get; set; }

    }
}