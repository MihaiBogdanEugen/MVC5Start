using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC5Start.Infrastructure.Identity.Managers
{
    public enum EnhancedSignInStatus
    {
        Success,
        LockedOut,
        RequiresVerification,
        Failure,
        Disabled,
        Unknown
    }
}