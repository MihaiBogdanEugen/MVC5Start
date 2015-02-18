
using MVC5Start.Models.Definitions;

namespace MVC5Start.Models.Validation
{
    public interface IValidate
    {
        ValidationResult IsValid();
    }
}