using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTokenAuth.Utility
{
    public class ApplicationEnums
    {
    }

    public enum AppResultStatus
    {
        ERROR=1,
        SUCCESS,
        DUPLICATE,
        UNAUTHORIZED,

    }
}