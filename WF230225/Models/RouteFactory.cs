using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WF230225.Infrastructure;

namespace WF230225.Models
{
    public static class RouteFactory
    {
        // опорные пункты маршрутов
        public static string[] Points = new[]
        {
            "поселок Слюдянка",
            "мыс Заворотный",
            "поселок Имандра",
            "бухта Капсель",
            "поселок Кировский",
            "водопад Учан-Су",
            "дворец Гаспра",
            "урочище Аязьма",
            "маяк Меганом",
        };

        // генерация случайного кода маршрута
        public static string GetCode()
        {
            string letters = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЭЮЯ";
            string numbers = "0123456789";

            StringBuilder sb = new();

            sb.Append(Utils.SelectRand(letters));
            sb.Append(Utils.SelectRand(letters));
            sb.Append(Utils.SelectRand(numbers));
            sb.Append(Utils.SelectRand(numbers));

            return sb.ToString();
        }

        // генерация случайной протяженности
        public static int GetLength() => Utils.GetRand(10, 200);
    }
}
