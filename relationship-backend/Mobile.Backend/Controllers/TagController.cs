using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Mobile.Backend.DataObjects;
using Mobile.Backend.Models;

namespace Mobile.Backend.Controllers
{
    public class TagController : TableController<Tag>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Tag>(context, Request);
        }

        // GET tables/Tag
        public IQueryable<Tag> GetAllTag() => Query();

        // GET tables/Tag/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Tag> GetTag(string id) => Lookup(id);

        // PATCH tables/Tag/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Tag> PatchTag(string id, Delta<Tag> patch) => UpdateAsync(id, patch);

        // POST tables/Tag
        public async Task<IHttpActionResult> PostTag(Tag item)
        {
            Tag current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Tag/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTag(string id) => DeleteAsync(id);
    }
}
