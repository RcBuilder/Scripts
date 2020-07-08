using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Controllers
{    
    public class ItemsController : ApiController
    {                
        [HttpGet]
        [Route("store/items")]
        public async Task<IHttpActionResult> Get() {
            var bll = new BLL.Items();
            var result = await bll.GetAsync();
            return Ok<IEnumerable<Entities.Item>>(result);
        }

        [HttpPost]
        [Route("store/item")]
        public async Task<IHttpActionResult> Add(Entities.Item Item)
        {
            Item.Code = 0; // enforce NEW item

            var bll = new BLL.Items();            
            return Ok<int>(await bll.SaveAsync(Item));            
        }

        [HttpPut]
        [Route("store/item")]
        public async Task<IHttpActionResult> Update(Entities.Item Item)
        {
            var bll = new BLL.Items();
            return Ok<int>(await bll.SaveAsync(Item));
        }

        [HttpDelete]
        [Route("store/item/{code}")]
        public async Task<IHttpActionResult> Update(int Code)
        {
            var bll = new BLL.Items();                        
            return Ok<bool>(await bll.DeleteAsync(Code));
        }
    }
}
