namespace MVC5Start.Models.Definitions
{
    public abstract class BaseIdEntity
    {
        /// <summary>
        /// The database Id of the entity.
        /// </summary>
        public int Id { get; set; }
    }
}