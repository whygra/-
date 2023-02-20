using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF230225.Models
{
    public class TourOperator
    {
        public TourOperator(string name, string address, List<TourRoute> routes)
        {
            Name = name;
            Address = address;
            Routes = routes;
        }

        public TourOperator() : this("", "", new()) { }

        // название
        public string Name { get; set; }
        // адрес
        public string Address { get; set; }
        // коллекция маршрутов
        public List<TourRoute> Routes { get; set; }
    }
}
