using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF230225.Models
{
    public class TourOperator
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<TourRoute> Routes { get; set; }
    }
}
