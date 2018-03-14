using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.DevAzt.FormsX.Json
{
    public static class JsonHelper
    {


        public static T DeserializeObject<T>(this string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeObject(this object element)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(element);
        }


    }
}
