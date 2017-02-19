using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using IsiiSports.Backend.Helpers;
using IsiiSports.Backend.Models;
using IsiiSports.DataObjects;
using Microsoft.Azure.Mobile.Server;

namespace IsiiSports.Backend.Controllers
{
    public class TeamController : TableController<Team>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Team>(context, Request, true);
        }

        [QueryableExpand("Players,Games")]
        [EnableQuery(MaxTop = 100)]
        public IQueryable<Team> GetAllTeam()
        {
            return Query();
        }

        [QueryableExpand("Players,Games")]
        public SingleResult<Team> GetTeam(string id)
        {
            return Lookup(id);
        }

        public Task<Team> PatchTeam(string id, Delta<Team> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostTeam(Team item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
      
        public Task DeleteTeam(string id)
        {
            return DeleteAsync(id);
        }
    }
}