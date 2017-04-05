using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using IsiiSports.Backend.Models;
using IsiiSports.DataObjects;

namespace IsiiSports.Backend.Controllers
{
    [Authorize]
    public class PlayerController : TableController<Player>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Player>(context, Request);
        }

        // GET tables/Player
        public IQueryable<Player> GetAllPlayer()
        {
            return Query(); 
        }

        // GET tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Player> GetPlayer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Player> PatchPlayer(string id, Delta<Player> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Player
        public async Task<IHttpActionResult> PostPlayer(Player item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePlayer(string id)
        {
             return DeleteAsync(id);
        }
    }
}
