using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/images")]
    public class ImagesController : ApiController
    {
        private UnitOfWork _uow;
        private int _userId;
        public ImagesController(IUnitOfWork uow)
        {
            _uow = uow as UnitOfWork;
            var username = User.Identity.Name;
            _userId = _uow.UserProfilesRepository.Get(u => u.Username == username).First().Id;
        }

        [HttpGet]
        [Route("getPhoto")]
        public IHttpActionResult GetPhoto(string imageName)
        {
            if (string.IsNullOrEmpty(imageName) || imageName.Equals("null")) return Ok();
            
            String filePath = HostingEnvironment.MapPath("~/App_Data/ProfilePictures/" + imageName);
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);

            memoryStream.Close();
            fileStream.Close();
            return Ok(Convert.ToBase64String(memoryStream.ToArray()));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadPhoto()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            string picturesFolder = HttpContext.Current.Server.MapPath("~/App_Data/ProfilePictures");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.LocalFileName.Substring(file.LocalFileName.LastIndexOf("BodyPart")) + ".jpg";
                    var path = Path.Combine(picturesFolder, filename);
                    File.Move(file.LocalFileName, path);

                    var me = _uow.UserProfilesRepository.Get(_userId);
                    me.ProfilePhotoUrl = filename;
                    _uow.UserProfilesRepository.Update(me);
                    _uow.Save();
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
