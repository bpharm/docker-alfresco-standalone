using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlfrescoProxy.Models;
using AlfrescoProxy.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace AlfrescoProxy.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]AlfrescoFile file)
        {
            
            try
            {
                var task = _uploadService.Upload(file);
                return new ObjectResult(task.Result);
            }
            catch (Exception e)
            {
                return new ObjectResult(new ErrorResponse() { Error = "invalid metadata", ErrorMessage = e.Message });
            }

        }

    }
}
