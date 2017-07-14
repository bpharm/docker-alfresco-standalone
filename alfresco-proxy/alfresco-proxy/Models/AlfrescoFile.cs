using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlfrescoProxy.Models
{
    public class AlfrescoFile
    {
        public string Date { get; set; }
        public string Expired { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string PageCount { get; set; }
        public string Type { get; set; }
        public string UploadUrl { get; set; }
        public string ShareUrl { get; set; }
        public string User { get; set; }
    }
}
