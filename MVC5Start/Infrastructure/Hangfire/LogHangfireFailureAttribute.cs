using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace MVC5Start.Infrastructure.Hangfire
{
    public sealed class LogHangfireFailureAttribute : JobFilterAttribute, IApplyStateFilter
    {
        #region IApplyStateFilter Members

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var failedState = context.NewState as FailedState;
            if (failedState == null)
                return;

            var jobId = context.JobId;
            var exception = failedState.Exception;

            //LOG
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }

        #endregion
    }
}