using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAreMyBusDriver.Models
{
    public class Route
    {
        [JsonProperty(PropertyName = "ruta")]
        public string Ruta { get; set; }
    }

}
