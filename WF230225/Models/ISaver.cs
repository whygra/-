using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF230225.Models
{
    public interface ISaver<T>
    {
        public void Save(T obj, string path);
    }
}
