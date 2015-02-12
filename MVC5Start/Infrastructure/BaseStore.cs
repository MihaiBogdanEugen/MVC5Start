using System;
using System.Data;
using System.Data.SqlClient;

namespace MVC5Start.Infrastructure
{
    public abstract class BaseStore : IDisposable
    {
        #region Fields

        private SqlConnection _sqlConnection;

        #endregion Fields

        #region Constructors

        protected BaseStore(DbConnectionInfo dbConnectionInfo)
        {
            this._sqlConnection = new SqlConnection(dbConnectionInfo.ConnectionString);
        }

        #endregion Constructors

        #region Properties

        protected SqlConnection Connection
        {
            get
            {
                if (this._sqlConnection.State != ConnectionState.Open)
                    this._sqlConnection.Open();

                return this._sqlConnection;
            }
        }

        #endregion Properties

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        ~BaseStore() 
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing == false || this._sqlConnection == null) 
                return;

            this._sqlConnection.Dispose();
            this._sqlConnection = null;
        }

        #endregion IDisposable Members
    }
}