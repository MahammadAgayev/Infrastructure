using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiTemplate.Constants
{
    public class ResultCodes
    {
        public const int Success = (int)HttpStatusCode.OK;

        public const int SuccessCreated = (int)HttpStatusCode.Created;

    }
}
