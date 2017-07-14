using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlfrescoProxy.Models;

namespace AlfrescoProxy.Services
{
    public interface IUploadService
    {
        Task<object> Upload(AlfrescoFile file);
    }
}
