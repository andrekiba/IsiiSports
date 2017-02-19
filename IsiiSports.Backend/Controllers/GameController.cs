using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using IsiiSports.Backend.Helpers;
using Microsoft.Azure.Mobile.Server;
using IsiiSports.Backend.Models;
using IsiiSports.DataObjects;

namespace IsiiSports.Backend.Controllers
{
    public class GameController : TableController<Game>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Game>(context, Request);
        }

        [QueryableExpand("Team1,Team2,GameField,GameResult")]
        [EnableQuery(MaxTop = 100)]// GET tables/Game
        public IQueryable<Game> GetAllGame()
        {
            return Query(); 
        }

        [QueryableExpand("Team1,Team2,GameField,GameResult")]
        public SingleResult<Game> GetGame(string id)
        {
            return Lookup(id);
        }

        public Task<Game> PatchGame(string id, Delta<Game> patch)
        {
             return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostGame(Game item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteGame(string id)
        {
             return DeleteAsync(id);
        }
    }
}
