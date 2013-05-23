using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public class Apartment
    {
        public string Address { get; set; }
        public int Bedrooms { get; set; }

        public string StreetViewImageUri
        {
            get
            {
                return String.Format(
                    "http://maps.googleapis.com/maps/api/streetview?size=400x400&location={0}&sensor=false",
                    Address);
            }
        }
    }
}
