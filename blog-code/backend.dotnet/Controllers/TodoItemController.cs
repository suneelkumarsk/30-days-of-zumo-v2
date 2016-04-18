﻿using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using backend.dotnet.DataObjects;
using backend.dotnet.Models;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace backend.dotnet.Controllers
{
    [Authorize]
    public class TodoItemController : TableController<TodoItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MyDbContext context = new MyDbContext();
            DomainManager = new EntityDomainManager<TodoItem>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<TodoItem> GetAllTodoItem()
        {
            Debug.WriteLine("GET tables/TodoItem");
            var emailAddr = GetEmailAddress();
            Debug.WriteLine($"email address = {emailAddr}");
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TodoItem> GetTodoItem(string id)
        {
            Debug.WriteLine($"GET tables/TodoItem/{id}");
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TodoItem> PatchTodoItem(string id, Delta<TodoItem> patch)
        {
            Debug.WriteLine($"PATCH tables/TodoItem/{id}");
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTodoItem(TodoItem item)
        {
            Debug.WriteLine($"PATCH tables/TodoItem");
            TodoItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTodoItem(string id)
        {
            Debug.WriteLine($"DELETE tables/TodoItem/{id}");
            return DeleteAsync(id);
        }

        private string GetAzureSID()
        {
            var principal = this.User as ClaimsPrincipal;
            var sid = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            return sid;
        }

        private async Task<string> GetEmailAddress()
        {
            var credentials = await User.GetAppServiceIdentityAsync<AzureActiveDirectoryCredentials>(Request);
            Debug.WriteLine("Obtained Credentials");
            foreach (var claim in credentials.UserClaims)
            {
                Debug.WriteLine($"Claim: k={claim.Type} v={claim.Value}");
            }
            return GetAzureSID();
        }
    }
}
