using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Proyecto_Planilla_Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Services
{

    public class ToResponse
    {

        public ToResponse()
        {

        }


        public static async Task<TResponse> HTTPExecuteAsync<TRequest, TResponse>(
            GlobalEnum.Metodo metodo,
            string controller,
            string api,
            TRequest objeto = default,
            string token = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    HttpResponseMessage response = null;
                    string url = $"{controller}{api}";
                    HttpContent content = null;

                    // Detectar tipo de contenido
                    if (objeto is HttpContent)
                    {
                        content = objeto as HttpContent;
                    }
                    else if (objeto != null)
                    {
                        var tipo = objeto.GetType();
                        var tieneArchivo = tipo.GetProperties().Any(p =>
                            p.PropertyType == typeof(FileInfo) ||
                            (p.PropertyType == typeof(byte[]) && p.Name.ToLower().Contains("archivo")));

                        if (tieneArchivo)
                        {
                            content = ToMultipartFormData(objeto);
                        }
                        else
                        {
                            string json = JsonConvert.SerializeObject(objeto);
                            content = new StringContent(json, Encoding.UTF8, "application/json");
                        }
                    }

                    switch (metodo)
                    {
                        case GlobalEnum.Metodo.POST:
                            response = await client.PostAsync(url, content);
                            break;
                        case GlobalEnum.Metodo.GET:
                            response = await client.GetAsync($"{url}/{objeto}");
                            break;
                        case GlobalEnum.Metodo.PUT:
                            response = await client.PutAsync(url, content);
                            break;
                        case GlobalEnum.Metodo.DELETE:
                            response = await client.DeleteAsync($"{url}/{objeto}");
                            break;
                        default:
                            throw new InvalidOperationException("Método HTTP no soportado");
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy
                            {
                                ProcessDictionaryKeys = false
                            }
                        }
                    };

                    var obj = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                    int status;
                    JToken statusToken = obj.Properties()
                        .FirstOrDefault(p => p.Name.Equals("status", StringComparison.OrdinalIgnoreCase))?.Value;

                    if (statusToken == null)
                        throw new Exception("No se encontró campo status en la respuesta");

                    status = statusToken.Value<int>();

                    switch (status)
                    {
                        case 200:
                            JToken dataToken = obj.Properties()
                                .FirstOrDefault(p => p.Name.Equals("data", StringComparison.OrdinalIgnoreCase))?.Value;

                            if (dataToken == null)
                                throw new Exception("No se encontró campo data en la respuesta");

                            Type responseType = typeof(TResponse);

                            // Manejo especial para tipos primitivos cuando dataToken es un valor simple
                            if (dataToken.Type == JTokenType.String ||
                                dataToken.Type == JTokenType.Boolean ||
                                dataToken.Type == JTokenType.Date ||
                                dataToken.Type == JTokenType.Integer ||
                                dataToken.Type == JTokenType.Float)
                            {
                                string data = dataToken.ToString();

                                switch (responseType)
                                {
                                    case Type t when t == typeof(string):
                                        return (TResponse)(object)data;

                                    case Type t when t == typeof(bool):
                                        return (TResponse)(object)(bool.TryParse(data, out var b) && b);

                                    case Type t when t == typeof(DateTime):
                                        return (TResponse)(object)DateTime.Parse(data);

                                    default:
                                        // Si es un tipo primitivo en dataToken pero TResponse no es primitivo,
                                        // intentar deserializar normalmente
                                        return JsonConvert.DeserializeObject<TResponse>(data, settings);
                                }
                            }
                            else
                            {
                                // Para objetos complejos, deserializar directamente desde el JToken
                                if (responseType == typeof(object))
                                {
                                    // Para object o dynamic, convertir el JToken a objeto dinámico
                                    return (TResponse)(object)dataToken.ToObject<dynamic>();
                                }
                                else if (responseType == typeof(string))
                                {
                                    // Si solicitan string pero es un objeto complejo, devolver el JSON
                                    return (TResponse)(object)dataToken.ToString();
                                }
                                else
                                {
                                    // Deserializar directamente desde el JToken
                                    return dataToken.ToObject<TResponse>(JsonSerializer.Create(settings));
                                }
                            }
                        /*
                        JToken dataToken = obj.Properties()
                            .FirstOrDefault(p => p.Name.Equals("data", StringComparison.OrdinalIgnoreCase))?.Value;

                        if (dataToken == null)
                            throw new Exception("No se encontró campo data en la respuesta");

                        string data = dataToken.ToString();
                        Type responseType = typeof(TResponse);

                        switch (responseType)
                        {
                            case Type t when t == typeof(string):
                                return (TResponse)(object)data;

                            case Type t when t == typeof(bool):
                                return (TResponse)(object)(bool.TryParse(data, out var b) && b);

                            case Type t when t == typeof(DateTime):
                                return (TResponse)(object)DateTime.Parse(data);

                            default:
                                return JsonConvert.DeserializeObject<TResponse>(data, settings);
                        }
                        */
                        case 401:
                        case 412:
                        default:
                            JToken messageToken = obj.Properties()
                                .FirstOrDefault(p => p.Name.Equals("message", StringComparison.OrdinalIgnoreCase))?.Value;
                            string message = messageToken?.ToString() ?? "Sin mensaje";
                            throw new Exception($"Error {status}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la ejecución del método HTTP: " + ex.Message, ex);
            }
        }

        public static async Task<TResponse> HTTPExecuteAsync<TRequest, TResponse>(GlobalEnum.Metodo metodo, string controller, string api, TRequest objeto = default)
        {
            return await HTTPExecuteAsync<TRequest, TResponse>(metodo, controller, api, objeto, null);
        }
        public static MultipartFormDataContent ToMultipartFormData(object obj)
        {
            var form = new MultipartFormDataContent();
            var type = obj.GetType();
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(obj);

                if (value == null)
                    continue;

                if (value is FileInfo fileInfo)
                {
                    var fileBytes = File.ReadAllBytes(fileInfo.FullName);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    form.Add(fileContent, prop.Name, fileInfo.Name);
                }
                else if (value is byte[] byteArray && prop.Name.ToLower().Contains("archivo"))
                {
                    var fileContent = new ByteArrayContent(byteArray);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    form.Add(fileContent, prop.Name, "archivo.bin");
                }
                else
                {
                    form.Add(new StringContent(value.ToString()), prop.Name);
                }
            }

            return form;
        }

    }

}
