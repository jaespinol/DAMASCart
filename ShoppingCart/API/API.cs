using ShoppingCart.API.Exceptions;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.API
{
    /*
    public class Param
    {

        public object Element { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public ParamType Type { get; set; }

        public Param(string name, object value, ParamType type = ParamType.File, string filename = "file.jpg")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("El parametro NAME es null o esta vacio");
            }

            if(value is null)
            {
                throw new Exception("El parametro VALUE es null");
            }

            Element = value;
            Name = name;
            FileName = filename;
            Type = type;
        }

        public System.Net.Http.HttpContent ToHttpContent()
        {
            System.Net.Http.HttpContent content = null;
            if (Type == ParamType.File)
            {
                if (Element is byte[])
                {
                    var bytearray = Element as byte[];
                    content = new System.Net.Http.ByteArrayContent(bytearray, 0, bytearray.Length);
                }
                else if(Element is Stream)
                {
                    var streamcontent = Element as Stream;
                    streamcontent.Position = 0;
                    content = new System.Net.Http.StreamContent(streamcontent);
                }
            }
            else if (Type == ParamType.String)
            {
                if (Element is string)
                {
                    var mystring = Element as string;
                    content = new System.Net.Http.StringContent(mystring);
                }
            }
            return content;
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    public enum ParamType
    {
        File, String
    }
    */

    /*
    public class RestClient
    {

        public static bool CheckConnectivity { get; set; }

        /// <summary>
        /// Este método realiza una petición POST
        /// </summary>
        /// <typeparam name="T">Generic, convierte la respuesta json, en un objeto de tipo T</typeparam>
        /// <param name="url">url del recurso</param>
        /// <param name="formdata">datos a enviar via post</param>
        /// <returns>T</returns>
        public async Task<T> Post<T>(string url, Dictionary<string, string> simpleparams = null, List<Param> complexparams = null)
        {
            if (complexparams != null)
            {
                try
                {
                    using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                    {
                        System.Net.Http.MultipartFormDataContent form = new System.Net.Http.MultipartFormDataContent("------WebKitFormBoundary7MA4YWxkTrZu0gW--");

                        foreach (var multipartparam in complexparams)
                        {
                            System.Net.Http.HttpContent content = multipartparam.ToHttpContent();
                            if (content != null)
                            {
                                switch (multipartparam.Type)
                                {
                                    case ParamType.File:
                                        form.Add(content, multipartparam.Name, multipartparam.FileName);
                                        break;
                                    case ParamType.String:
                                        form.Add(content, multipartparam.Name);
                                        break;
                                }
                            }
                        }

                        System.Net.Http.HttpResponseMessage response = await client.PostAsync(url, form);
                        if (response != null && response.IsSuccessStatusCode)
                        {
                            var responsestring = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine(responsestring);
                            return JsonConvert.DeserializeObject<T>(responsestring);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    Debug.WriteLine(url);
                    System.Net.Http.FormUrlEncodedContent form = new System.Net.Http.FormUrlEncodedContent(simpleparams);
                    Debug.WriteLine(simpleparams);
                    System.Net.Http.HttpResponseMessage response = await client.PostAsync(url, form);
                    Debug.WriteLine(response);
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var responsestring = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(responsestring);
                        return JsonConvert.DeserializeObject<T>(responsestring);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Este método realiza una petición POST
        /// </summary>
        /// <typeparam name="T">Generic, convierte la respuesta json, en un objeto de tipo T</typeparam>
        /// <typeparam name="K">Generic, tipo de objeto a enviar como raw content application/json</typeparam>
        /// <param name="url">url del recurso</param>
        /// <param name="formdata">datos a enviar via post</param>
        /// <returns>T</returns>
        public async Task<T> Post<T, K>(string url, K objecttosend)
        {
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                string contentjson = JsonConvert.SerializeObject(objecttosend);
                Debug.WriteLine(contentjson);
                byte[] buffer = Encoding.UTF8.GetBytes(contentjson);
                System.Net.Http.ByteArrayContent rawcontent = new System.Net.Http.ByteArrayContent(buffer);
                rawcontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                Debug.WriteLine(url);
                System.Net.Http.HttpResponseMessage response = await client.PostAsync(url, rawcontent);
                Debug.WriteLine(response.StatusCode);
                Debug.WriteLine(response.Content);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responsestring = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responsestring);
                    return JsonConvert.DeserializeObject<T>(responsestring);
                }
            }
            catch
            {
            }
            return default(T);
        }

        /// <summary>
        /// Este método realiza una petición GET
        /// </summary>
        /// <typeparam name="T">Generic, convierte la respuesta json, en un objeto de tipo T</typeparam>
        /// <param name="url">url del recurso</param>
        /// <param name="formdata">datos a enviar via GET</param>
        /// <returns>T</returns>
        public async Task<T> Get<T>(string url, Dictionary<string, string> formdata = null)
        {
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                
                if (formdata != null)
                {
                    if (!url.EndsWith("?"))
                    {
                        url += "?";
                    }

                    foreach (var item in formdata)
                    {
                        url += item.Key + "=" + item.Value + "&";
                    }

                    if (url.EndsWith("&"))
                    {
                        url = url.Remove(url.Length - 1, 1);
                    }
                }

                System.Net.Http.HttpResponseMessage response = await client.GetAsync(url);
                Debug.WriteLine($"Url: {url}");
                Debug.WriteLine($"HttpContent: {response}");
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responsestring = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response: {responsestring}");
                    return JsonConvert.DeserializeObject<T>(responsestring);
                }
            }
            catch
            {
            }
            return default(T);
        }
    }
    */
}
