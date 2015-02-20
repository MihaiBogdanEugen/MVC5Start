namespace MVC5Start.Infrastructure.Identity
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