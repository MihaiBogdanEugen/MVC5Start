using System;
using Hangfire;
using SimpleInjector;

namespace MVC5Start.Infrastructure.SimpleInjector
{
    public class SimpleInjectorJobActivator : JobActivator
    {
        private readonly Container _container;

        public SimpleInjectorJobActivator(Container container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this._container = container;
        }

        public override object ActivateJob(Type jobType)
        {
            return _container.GetInstance(jobType);
        }
    }
}