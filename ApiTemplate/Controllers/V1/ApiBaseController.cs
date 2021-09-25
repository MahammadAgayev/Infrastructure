using Microsoft.AspNetCore.Mvc;

namespace ApiTemplate.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
    }
}
