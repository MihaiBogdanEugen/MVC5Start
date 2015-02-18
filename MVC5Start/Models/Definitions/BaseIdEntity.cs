using MVC5Start.Models.Validation;

namespace MVC5Start.Models.Definitions
{
    public abstract class BaseIdEntity : IValidate
    {
        /// <summary>
        /// The database Id of the entity.
        /// </summary>
        public int Id { get; set; }

        #region IValidate Members

        public abstract ValidationResult IsValid();

        #endregion IValidate Members
    }
}