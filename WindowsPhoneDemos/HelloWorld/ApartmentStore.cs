using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public class ApartmentStore
    {
        private List<Apartment> _apartments = new List<Apartment>();

        public ApartmentStore()
        {
            _apartments.Add(new Apartment
            {
                Address = "1 Ruppin Street, Jerusalem, Israel",
                Bedrooms = 2
            });
            _apartments.Add(new Apartment
            {
                Address = "Times Square, New York",
                Bedrooms = 3
            });
        }

        public void AddApartment(Apartment apartment)
        {
            _apartments.Add(apartment);
        }

        public IList<Apartment> Apartments
        {
            get { return _apartments; }
        }
    }
}
