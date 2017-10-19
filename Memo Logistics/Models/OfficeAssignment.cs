using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Memo_Logistics.Models
{
    public class OfficeAssignment
    {
        [Key]
        [ForeignKey("Driver")]
        public int DriverID { get; set; }
        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public virtual Drivers Driver { get; set; }
    }
}