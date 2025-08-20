using FileSharing.Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileSharing.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FileSharingController : ControllerBase
    {
        private readonly ILogger<FileSharingController> _logger;
        private IFileSharingService _fileSharingService;

        public FileSharingController(ILogger<FileSharingController> logger, IFileSharingService fileSharingService)
        {
            _logger = logger;
            _fileSharingService = fileSharingService;
        }
        [Authorize("read")]
        [HttpGet("{*filename}")]
        public ActionResult Get(string filename)
        {
            byte[] filedata;
            try
            {
                filedata = _fileSharingService.DownloadFile(filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);
                return NotFound(ex);
            }
            if (filedata != null)
                return File(filedata, "application/octet-stream");
            return NotFound();
        }
        [Authorize("readwrite")]
        [HttpPost("{*filename}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult Post(string filename)
        {
            _logger.LogTrace($"Post : {filename}");
            byte[] filedata = null;
            if (Request.Form.Files.Any())
            {
                using (var stream = new MemoryStream())
                {
                    Request.Form.Files[0].CopyTo(stream);
                    filedata = stream.GetBuffer();
                }
            }
            else
            {
                filedata = new byte[Request.ContentLength.Value];
                int len = 0;
                CancellationToken cancellationToken = new CancellationToken();
                do
                {
                    len += Request.Body.ReadAsync(filedata, len, filedata.Length - len, cancellationToken: cancellationToken).Result;
                    if (len >= filedata.Length || cancellationToken.IsCancellationRequested)
                        break;
                    if (len == 0)
                        throw new Exception("INVALID_DATA");
                } while (true);
            }
            try
            {
                _fileSharingService.UploadFile(filename, filedata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                    return BadRequest(ex.InnerException.Message);
                else
                    return BadRequest(ex.Message);
            }
            return Ok();
        }
        [Authorize("readwrite")]
        [HttpDelete("{*filename}")]
        public ActionResult DeleteFile(string filename)
        {
            try
            {
                _fileSharingService.DeleteFile(filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(ex.Message);
            }
            return Ok();
        }
        [Authorize("readwrite")]
        [HttpDelete("folder/{*filename}")]
        public ActionResult DeleteFolder(string filename)
        {
            try
            {
                _fileSharingService.DeleteFolder(filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
            return Ok();
        }
    }
}