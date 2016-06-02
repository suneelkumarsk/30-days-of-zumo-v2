using Backend.DataObjects;
using Backend.Models;
using Microsoft.Azure.Mobile.Server;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;

namespace Backend.Controllers
{
    public class ImageController : TableController<Image>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Image>(context, Request);
        }

        // GET tables/Image
        public IQueryable<Image> GetAllImage()
        {
            return Query();
        }

        // GET tables/Image/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Image> GetImage(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Image/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public IHttpActionResult PatchImage(string id, Delta<Image> patch)
        {
            return BadRequest();
        }

        // POST tables/Image
        public IHttpActionResult PostImage(Image item)
        {
            return BadRequest();
        }

        // DELETE tables/Image/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public IHttpActionResult DeleteImage(string id)
        {
            return BadRequest();
        }
    }
}
