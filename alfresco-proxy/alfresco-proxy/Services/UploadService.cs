using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AlfrescoProxy.Models;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Globalization;
using System.IO;

namespace AlfrescoProxy.Services
{
    public class UploadService: IUploadService
    {
        private string CleanFileName(string fileName)
        {
            var result = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
            return result.Replace("/", string.Empty).Replace("\\", string.Empty).Replace(":", string.Empty).Replace("?", string.Empty).Trim();
        }
        public async Task<object> Upload(AlfrescoFile file)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Alfresco-Remote-User", file.User);
            var multiContent = new MultipartFormDataContent();
            var fileData = Convert.FromBase64String(file.FileContent);
            var bytes = new ByteArrayContent(fileData);
            //validate filename 
            var prefix = "un_";
            switch (file.Type)
            {
                case "Сертификат качества":
                    prefix = "cq_";
                    break;
                case "Сертификат анализа":
                    prefix = "ca_";
                    break;
                case "Декларация":
                    prefix = "d_";
                    break;
                case "Сертификат производителя (русский язык)":
                    prefix = "mc_";
                    break;
                case "Регистрационное удостоверение":
                    prefix = "rc_";
                    break;
                case "Сертификат соответствия":
                    prefix = "cc_";
                    break;
                case "Протокол анализа":
                    prefix = "ap_";
                    break;
                case "Сертификат соответствия РОСТЕСТ":
                    prefix = "ссr_";
                    break;
                case "Информационное письмо":
                    prefix = "im_";
                    break;
                case "Гигиенический сертификат":
                    prefix = "hc_";
                    break;
                case "Паспорт":
                    prefix = "p_";
                    break;
                case "Договор":
                    prefix = "a_";
                    break;
                case "Протокол разногласий":
                    prefix = "drp_";
                    break;
                case "Доверенность":
                    prefix = "pa_";
                    break;
                case "Доп. соглашения к договору":
                    prefix = "ea_";
                    break;
                case "Аналитический Лист":
                    prefix = "as_";
                    break;
                case "Акт о забраковке":
                    prefix = "ra_";
                    break;

            }
            var fileName = prefix + DateTime.Now.ToString("yyyyMMdd_Hmmss");
            multiContent.Add(bytes, "filedata", fileName);
            multiContent.Add(new StringContent("name"), fileName);
            var result = await client.PostAsync(file.UploadUrl, multiContent);
            if (result.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return await result.Content.ReadAsStringAsync();
            }
            var response = await result.Content.ReadAsStringAsync();
            var item = JObject.Parse(response);
            var props = new JObject
            {
                ["sc:type"] = !string.IsNullOrEmpty(file.Type) ? file.Type : "Неизвестный",
                ["cm:title"] = file.FileName
            };
           
            if (!string.IsNullOrEmpty(file.Date))
            {
                var date = DateTime.ParseExact(file.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                props["sc:date"] = date.ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(file.Expired))
            {
                var expired = DateTime.ParseExact(file.Expired, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                props["sc:expired"] = expired.ToString("yyyy-MM-dd");
            }
            var o = new JObject
            {
                ["nodeType"] = "sc:series",
                ["properties"] = props,
            };
            var json = o.ToString();
            var url = file.ShareUrl + item["entry"]["id"];
            result = await client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return await result.Content.ReadAsStringAsync();

        }
    }
}
