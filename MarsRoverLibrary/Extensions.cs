using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRover
{
    public static class Extensions
    {

        public static T DeepCopy<T>(this T input)
        {
            string result = JsonConvert.SerializeObject(input);
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
