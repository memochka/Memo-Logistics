using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Memo_Logistics.ViewModels
{
    public class AssignedTrucksData
    {
        public int TruckID { get; set; }
        public string Truck { get; set; }
        public bool Assigned { get; set; }
    }
}