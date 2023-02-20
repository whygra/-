using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF230225.Models
{
    public class TourRoute
    {
        public TourRoute(int id, string start, string finish, string code, int length)
        {
            Id = id;
            Start = start;
            Finish = finish;
            Code = code;
            Length = length;
        }

        public TourRoute() : this(0, "", "", "", 0) { }

        // числовой идентификатор маршрута
        public int Id { get; set; }

        // название начального пункта маршрута
        public string Start { get; set; }

        // название конечного пункта маршрута
        public string Finish { get; set; }

        // буквенно-цифровой код маршрута(например, АВ23, РВ892)
        public string Code { get; set; }

        // протяженность маршрута в целом количестве километрах
        int _length;
        public int Length {
            get => _length;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _length = value;
            }
        }
    }
}
