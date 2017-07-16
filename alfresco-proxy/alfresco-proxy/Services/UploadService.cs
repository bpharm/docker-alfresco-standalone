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
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        public async Task<object> Upload(AlfrescoFile file)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Alfresco-Remote-User", file.User);
            var multiContent = new MultipartFormDataContent();
            var fileData = Convert.FromBase64String(file.FileContent);
            var bytes = new ByteArrayContent(fileData);
            file.FileName = Path.ChangeExtension(file.FileName, Path.GetExtension(file.FileName).ToLower());
            //validate filename 
            file.FileName = CleanFileName(file.FileName);
            multiContent.Add(bytes, "filedata", file.FileName);
            multiContent.Add(new StringContent("name"), file.FileName);
            var result = await client.PostAsync(file.UploadUrl, multiContent);
            if (result.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return await result.Content.ReadAsStringAsync();
            }
            var response = await result.Content.ReadAsStringAsync();
            var item = JObject.Parse(response);
            var props = new JObject
            {
                ["sc:type"] = file.Type
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
                ["properties"] = props
            };
            var json = o.ToString();
            var url = file.ShareUrl + item["entry"]["id"];
            result = await client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return await result.Content.ReadAsStringAsync();

        }
    }
}
