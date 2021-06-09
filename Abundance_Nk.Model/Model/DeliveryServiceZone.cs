using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class DeliveryServiceZone
    {
        public int Id { get; set; }
        public DeliveryService DeliveryService { get; set; }
        public GeoZone GeoZone { get; set; }
        public FeeType FeeType { get; set; }
        public Country Country { get; set; }
        public decimal? DeliveryServiceAmount { get; set; }
        public decimal? LLoydantAmount { get; set; }
        public decimal? SchoolAmount { get; set; }
        public bool Activated { get; set; }
        public string Name { get; set; }
    }
}
