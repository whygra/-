using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WF230225.Models
{
    internal class JsonSerializer<T> : ILoader<T>, ISaver<T>
    {
        // чтение
        public T Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не найден", path);
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(json);
        }


        // запись
        public void Save(T data, string path)
        {
            if (!File.Exists(path))
                File.Create(path).Close();
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }
    }
}
