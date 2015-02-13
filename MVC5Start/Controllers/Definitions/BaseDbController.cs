using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC5Start.Infrastructure;

namespace MVC5Start.Controllers.Definitions
{
    public abstract class BaseDbController : BaseController
    {
        protected BaseDbController(DbConnectionInfo dbConnectionInfo)
        {
            if (dbConnectionInfo == null) 
                throw new ArgumentNullException("dbConnectionInfo");

            this.DbConnectionInfo = dbConnectionInfo;
        }

        protected DbConnectionInfo DbConnectionInfo { get; private set; }
    }
}