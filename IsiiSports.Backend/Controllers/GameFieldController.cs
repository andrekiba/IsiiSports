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
    public class GameFieldController : TableController<GameField>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<GameField>(context, Request);
        }

        // GET tables/GameField
        public IQueryable<GameField> GetAllGameField()
        {
            return Query(); 
        }

        // GET tables/GameField/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<GameField> GetGameField(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/GameField/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<GameField> PatchGameField(string id, Delta<GameField> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/GameField
        public async Task<IHttpActionResult> PostGameField(GameField item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/GameField/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGameField(string id)
        {
             return DeleteAsync(id);
        }
    }
}
