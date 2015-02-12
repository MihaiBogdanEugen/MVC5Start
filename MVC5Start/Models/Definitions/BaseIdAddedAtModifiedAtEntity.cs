using System;

namespace MVC5Start.Models.Definitions
{
    public abstract class BaseIdAddedAtModifiedAtEntity : BaseIdAddedAtEntity
    {
        /// <summary>
        /// The date when this entity was last modified.
        /// A null value means it was never modified.
        /// </summary>
        public DateTime? ModifiedAtUtc { get; set; }
    }
}