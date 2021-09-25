using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTemplate.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TemplateController : ApiBaseController
    {
        [HttpGet]
        public IActionResult GetTest()
        {
            return Ok(new ApiResponse
            {
                Result = "this is result api response",
                ResultCode = (int)HttpStatusCode.OK
            });
        }
    }
}
