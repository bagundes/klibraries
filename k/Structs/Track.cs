using System;
using System.Collections.Generic;
using System.Text;

namespace k.Structs
{
    public struct Track
    {
        private string id;
        public string Id { get => id; set => id = value + new string ('0', value.Length <= 6 ? 6 - value.Length : 0); }
        public string path;

        public Track(string path, string id = null) : this()
        {
            this.path = path;
            Id = id ?? k.Security.Id(DateTime.Now.ToString());
        }

        public string File => $"{path}\\{Id}.track";

        /// <summary>
        /// Create Id
        /// </summary>
        /// <param name="values"></param>
        public void CreateId(params object[] values) => Id = k.Security.Id(values);

        public override string ToString()
        {
            return $"Track:{Id}";
        }

        public static string Null => "1BB000";
        public static Track TrackNull()
        {
            return new Track(null, Null);
        }
    }
}
