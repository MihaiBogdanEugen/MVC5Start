using System;
using MVC5Start.Infrastructure.Services;

namespace MVC5Start.Models.Definitions
{
    public abstract class BaseIdAddedAtEntity : BaseIdEntity
    {
        protected BaseIdAddedAtEntity()
        {
            this.AddedAtUtc = DateTimeService.Now;
        }

        /// <summary>
        /// The date when this entity was added.
        /// Initialized by default with the current timestamp.
        /// </summary>
        public DateTime AddedAtUtc { get; set; }
    }
}