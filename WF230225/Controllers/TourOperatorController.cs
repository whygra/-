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
        JsonSerializer<TourOperator> _serializer;
        public string FilePath { get; set; }
        TourOperator _tourOperator;

        public TourOperatorController(TourOperator tourOperator, string filePath)
        {
            _tourOperator = tourOperator;
            _serializer = new();
            FilePath = filePath;
        }

        public TourOperatorController() : this(new(), Utils.TempFilePath)
        { }

        // свойство для чтения коллекции
        public List<TourRoute> Items => _tourOperator.Routes;

        // свойства для чтения данных турфирмы
        public string Name => _tourOperator.Name;
        public string Address => _tourOperator.Address;


        // сброс
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

        }

        // сериализация
        public void Serialize()
        {

        }

        // десериализация
        public void Deserialize()
        {

        }

        // TODO: методы для получения отсортированной коллекции



    }
}
