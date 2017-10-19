using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Memo_Logistics.Models
{
    public class Logistics
    {
        public int ID { get; set; }

        public int? CustomerID { get; set; }

        public int? DriverID { get; set; }


        [Display(Name = "DeparturePoint")]
        [Required]
        public string DeparturePoint { get; set; }

        [Display(Name = "ArrivalPoint")]
        [Required]
        public string ArrivalPoint { get; set; }

        [Display(Name = "Distance")]
        public int Distance { get; set; }

        [Display(Name = "DepartureDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "ArrivalDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalDate { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Drivers Driver { get; set; }
    }
}