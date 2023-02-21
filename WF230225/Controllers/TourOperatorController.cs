using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF230225.Models;
using WF230225.Infrastructure;

namespace WF230225.Controllers
{
    public class TourOperatorController
    {
        // сериализатор
        JsonSerializer<TourOperator> _serializer;
        // обрабатываемый объект
        TourOperator _tourOperator;
        // путь к файлу
        public string FilePath { get; set; }

        // компаратор сортировки,
        // для добавления нового критерия сортировки достаточно создать новый компаратор
        public Comparer<TourRoute> SortComp { get; set; }

        public TourOperatorController(TourOperator tourOperator, string filePath)
        {
            SortComp = null!;
            _tourOperator = tourOperator;
            _serializer = new();
            FilePath = filePath;
        }

        public TourOperatorController() : this(new(), Utils.TempFilePath)
        { }

        #region геттеры и сеттеры для представлений (Views)
        // свойство для чтения коллекции
        public List<TourRoute> Items => _tourOperator.Routes;

        // свойства для чтения данных турфирмы
        public string Name => _tourOperator.Name;
        public string Address => _tourOperator.Address;

        // изменение названия
        public void SetName(string name) => _tourOperator.Name = name;
        
        // изменение адреса
        public void SetAddress(string address) => _tourOperator.Address = address;

        #endregion

        #region работа с файлом
        // инициализация - открытие/создание(+заполнение) файла по-умолчанию
        public void Init(bool fill = false)
        {
            // если файл отсутствует - создаем и заполняем
            if (!File.Exists(Utils.TempFilePath))
            {
                if (!Directory.Exists(Utils.DataPath))
                    Directory.CreateDirectory(Utils.DataPath);
                File.Create(Utils.TempFilePath).Close();

                // заполнение и запись коллекции в созданный файл
                if (fill)
                {
                    Fill(15);
                    Serialize();
                }
            }
            // иначе загружаем данные из файла
            else
                Deserialize();
        }

        // сериализация
        public void Serialize() => _serializer.Save(_tourOperator, FilePath);


        // десериализация
        public void Deserialize()
        {
            if (!File.Exists(FilePath))
                throw new Exception($"Файл {FilePath} не найден");

            // загрузка данных
            var loaded = _serializer.Load(FilePath);
            if (loaded == null)
                throw new Exception("Ошибка десериализации");
            _tourOperator = loaded;
        }

        #endregion


        #region операции с коллекцией
        // установить идентификатор вновь добавленному элементу
        public void SetLastId()
        {
            if (Items.Count <= 1)
                return;
            int maxId = 0;
            foreach (var item in Items)
                if (item.Id > maxId)
                    maxId = item.Id;

            Items[Items.Count() - 1].Id = maxId + 1;
        }

        // заполнение коллекции маршрутов
        public void Fill(int n)
        {
            _tourOperator.Routes.Clear();

            _tourOperator.Routes.AddRange(RouteFactory.GetRange(n));
        }
        #endregion

        #region получение отсортированной коллекции
        // компараторы сортировки:
        // код маршрута
        public static readonly Comparer<TourRoute> CodeComparer =
            Comparer<TourRoute>.Create((r1, r2) => r1.Code.CompareTo(r2.Code));

        // начальный пункт
        public static readonly Comparer<TourRoute> StartComparer =
            Comparer<TourRoute>.Create((r1, r2) => r1.Start.CompareTo(r2.Start));

        // протяженность
        public static readonly Comparer<TourRoute> LengthComparer =
            Comparer<TourRoute>.Create((r1, r2) => r1.Length.CompareTo(r2.Length));


        // вернуть отсортированный список
        public List<TourRoute> GetSorted(bool isAscend)
        {
            List<TourRoute> sorted = Items;

            if (SortComp != null)
            {
                Comparer<TourRoute> comp =
                isAscend
                ?
                SortComp
                :
                // для порядка "по убыванию" просто меняем аргументы местами
                Comparer<TourRoute>.Create((tv1, tv2) => SortComp.Compare(tv2, tv1));

                sorted.Sort(comp);
            }
            return sorted;
        }
        #endregion

        #region выборка
        // минимальная протяженность (для выборки по протяженности)
        int GetMinLen()
        {
            int minLen = Items[0].Length;
            foreach (TourRoute route in Items)
                if (route.Length < minLen) minLen = route.Length;
            return minLen;
        }

        // максимальная протяженность (для выборки по протяженности)
        int GetMaxLen()
        {
            int maxLen = 0;
            foreach (TourRoute route in Items)
                if (route.Length > maxLen) maxLen = route.Length;
            return maxLen;
        }

        // массив строковых представлений чисел
        // в диапазоне от минимальной до максимальной протяженности и кратных 10
        public string[] GetRangeSteps()
        {
            int minLen = GetMinLen();
            int maxLen = GetMaxLen();

            List<string> result = new();

            // минимальное значение
            result.Add($"{minLen}");
            // промежуточные значение, кратные десяти
            for (int i = minLen+1; i < maxLen; i++)
                if(i%10 == 0)
                    result.Add($"{i}");
            // максимальное значение
            result.Add($"{maxLen}");

            return result.ToArray();
        }

        // массив уникальных пунктов маршрутов
        public string[] GetPoints()
        {
            Dictionary<string, int> points = new();

            Items.ForEach(i => points[i.Start] = points[i.Finish] = 0);

            return points.Keys.ToArray();
        }

        // по диапазону протяженности
        public List<TourRoute> SelectByLengthRange(int lo, int hi) =>
            Items.FindAll(i => i.Length >= lo && i.Length <= hi);

        // по пункту
        public List<TourRoute> SelectByPoint(string point) =>
            Items.FindAll(i => i.Start == point || i.Finish == point);

        #endregion
    }
}
