using Newtonsoft.Json;
using Rocket.Core.Logging;
using System;
using System.IO;

namespace Anomoly.KitsPlusv2.Utils
{
    public class JsonFileDB<T> where T: class
    {
        private string _file;

        private T _default;

        private Formatting _formatting;

        public T Instance { get; set; }

        public JsonFileDB(string file, T defaultInstance, Formatting formatting = Formatting.None)
        {
            _file = file;
            _default = defaultInstance;
            _formatting = formatting;
        }

        public void Save()
        {
            if (Instance == null)
                Instance = _default;

            string json = null;
            try
            {
                json = JsonConvert.SerializeObject(Instance, _formatting);
            }
            catch(Exception ex) { Logger.LogException(ex, $"Failed to serialize JSON object for saving: {_file}"); }

            File.WriteAllText(_file, json);
        }

        public void Load()
        {
            if (!File.Exists(_file))
            {
                Save();
                return;
            }

            var json = File.ReadAllText(_file);

            try
            {
                Instance = JsonConvert.DeserializeObject<T>(json);
            }
            catch(Exception ex) { Logger.LogException(ex, $"Failed to parse JSON from file: {_file}"); }
        }
    }
}
