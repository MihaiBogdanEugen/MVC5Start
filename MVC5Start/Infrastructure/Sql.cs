
namespace MVC5Start.Infrastructure
{
    public static class Sql
    {
        public const string SelectScopeIdentity = @"SELECT CAST(SCOPE_IDENTITY() AS INT)";

        public static class Roles
        {
            public const string SelectIdByName = "SELECT TOP 1 Id FROM Roles WHERE Name = @Name";
            public const string Insert = "INSERT INTO Roles(Name, Description, AddedAtUtc) VALUES (@Name, @Description, GETUTCDATE())";
            public const string Update = "UPDATE Roles SET Name = @Name, Description = @Description, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
            public const string Delete = "DELETE FROM Roles WHERE Id = @Id";
            public const string Select = "SELECT Id, Name, Description, AddedAtUtc, ModifiedAtUtc FROM Roles ORDER BY Name ASC";
            public const string FindById = "SELECT TOP 1 Id, Name, Description, AddedAtUtc, ModifiedAtUtc FROM Roles WHERE Id = @Id;";
            public const string FindByName = "SELECT TOP 1 Id, Name, Description, AddedAtUtc, ModifiedAtUtc FROM Roles WHERE Name = @Name;";
        }

        public static class UserRoles
        {
            public const string AddToRole = @"
INSERT INTO UserRoles(UserId, RoleId, AddedAtUtc) 
VALUES (@UserId, (SELECT TOP 1 Id FROM Roles WHERE Name = @RoleName), GETUTCDATE())
";

            public const string RemoveFromRole = @"
DELETE FROM UserRoles 
WHERE UserId = @UserId 
AND RoleId = (SELECT TOP 1 Id FROM Roles WHERE Name = @RoleName)
";

            public const string GetRoles = @"
SELECT R.Name FROM Roles R 
INNER JOIN UserRoles UR ON R.Id = UR.RoleId
WHERE UR.UserId = @UserId
ORDER BY R.Name
";

            public const string IsInRole = @"
SELECT TOP 1 Id, UserId, RoleId, AddedAtUtc
FROM UserRoles
WHERE UserId = @UserId 
AND RoleId = (SELECT Id FROM Roles WHERE Name = @RoleName)
";

            public const string DeleteByRoleId = @"DELETE FROM UserRoles WHERE RoleId = @RoleId";

            public const string DeleteByUserId = @"DELETE FROM UserRoles WHERE UserId = @UserId";
        }

        public static class UserClaims
        {
            public const string Insert = @"INSERT INTO UserClaims (UserId, ClaimType, ClaimValue, AddedAtUtc) VALUES (@UserId, @ClaimType, @ClaimValue, GETUTCDATE())";

            public const string Delete = @"DELETE FROM UserClaims WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue";

            public const string DeleteByUserId = @"DELETE FROM UserClaims WHERE UserId = @UserId";

            public const string SelectByUserId = @"SELECT Id, UserId, ClaimType, ClaimValue, AddedAtUtc FROM UserClaims WHERE UserId = @UserId";
        }

        public static class UserLogins
        {
            public const string Insert = @"INSERT INTO UserLogins (UserId, LoginProvider, ProviderKey, AddedAtUtc) VALUES (@UserId, @LoginProvider, @ProviderKey, GETUTCDATE())";

            public const string Delete = @"DELETE FROM UserLogins WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey";

            public const string DeleteByUserId = @"DELETE FROM UserLogins WHERE UserId = @UserId";

            public const string SelectByUserId = @"SELECT Id, UserId, LoginProvider, ProviderKey, AddedAtUtc FROM UserLogins WHERE UserId = @UserId";
        }

        public static class Users
        {
            public const string SetPasswordHash = "UPDATE Users SET PasswordHash = @PasswordHash, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetPasswordHash = "SELECT TOP 1 PasswordHash FROM Users WHERE Id = @UserId";

            public const string SetSecurityStamp = "UPDATE Users SET SecurityStamp = @SecurityStamp, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetSecurityStamp = "SELECT TOP 1 Users FROM AspNetUsers WHERE Id = @UserId";

            public const string SetEmail = "UPDATE Users SET Email = @Email, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetEmail = "SELECT TOP 1 Email FROM Users WHERE Id = @UserId";

            public const string SetEmailConfirmed = "UPDATE Users SET EmailConfirmed = @EmailConfirmed, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetEmailConfirmed = "SELECT TOP 1 EmailConfirmed FROM Users WHERE Id = @UserId";

            public const string SetPhoneNumber = "UPDATE Users SET PhoneNumber = @PhoneNumber, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetPhoneNumber = "SELECT TOP 1 PhoneNumber FROM Users WHERE Id = @UserId";

            public const string SetPhoneNumberConfirmed = "UPDATE Users SET PhoneNumberConfirmed = @EmailConfirmed, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetPhoneNumberConfirmed = "SELECT TOP 1 PhoneNumberConfirmed FROM Users WHERE Id = @UserId";

            public const string SetTwoFactorEnabled = "UPDATE Users SET TwoFactorEnabled = @TwoFactorEnabled, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetTwoFactorEnabled = "SELECT TOP 1 TwoFactorEnabled FROM Users WHERE Id = @UserId";

            public const string SetLockoutEnabled = "UPDATE Users SET LockoutEnabled = @LockoutEnabled, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetLockoutEnabled = "SELECT TOP 1 LockoutEnabled FROM Users WHERE Id = @UserId";

            public const string SetLockoutEndDateUtc = "UPDATE Users SET LockoutEndDateUtc = @LockoutEndDateUtc, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string GetLockoutEndDateUtc = "SELECT TOP 1 LockoutEndDateUtc FROM Users WHERE Id = @UserId";

            public const string GetAccessFailedCount = "SELECT TOP 1 AccessFailedCount FROM Users WHERE Id = @UserId";
            public const string ResetAccessFailedCount = "UPDATE Users SET AccessFailedCount = 0, ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";
            public const string IncrementAccessFailedCount = "UPDATE Users SET AccessFailedCount = (AccessFailedCount + 1), ModifiedAtUtc = GETUTCDATE() WHERE Id = @UserId";

            public const string FindById = @"
SELECT TOP 1
    Id, 
    FirstName,
    LastName,
    UserName,
    PasswordHash,
    SecurityStamp,
    Email,
    EmailConfirmed,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEndDateUtc,
    LockoutEnabled,
    AccessFailedCount,
    LastLogInAtUtc,
    IsDisabled,
    AddedAtUtc,
    ModifiedAtUtc
FROM Users
WHERE Id = @Id
";

            public const string Select = @"
SELECT
    Id, 
    FirstName,
    LastName,
    UserName,
    PasswordHash,
    SecurityStamp,
    Email,
    EmailConfirmed,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEndDateUtc,
    LockoutEnabled,
    AccessFailedCount,
    LastLogInAtUtc,
    IsDisabled,
    AddedAtUtc,
    ModifiedAtUtc
FROM Users
ORDER BY UserName ASC
";

            public const string FindByName = @"
SELECT TOP 1
    Id, 
    FirstName,
    LastName,
    UserName,
    PasswordHash,
    SecurityStamp,
    Email,
    EmailConfirmed,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEndDateUtc,
    LockoutEnabled,
    AccessFailedCount,
    LastLogInAtUtc,
    IsDisabled,
    AddedAtUtc,
    ModifiedAtUtc
FROM Users
WHERE UserName = @UserName
";

            public const string FindByEmail = @"
SELECT TOP 1
    Id, 
    FirstName,
    LastName,
    UserName,
    PasswordHash,
    SecurityStamp,
    Email,
    EmailConfirmed,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEndDateUtc,
    LockoutEnabled,
    AccessFailedCount,
    LastLogInAtUtc,
    IsDisabled,
    AddedAtUtc,
    ModifiedAtUtc
FROM Users
WHERE Email = @Email
";

            public const string Insert = @"
INSERT INTO Users (
    FirstName,
    LastName,
    UserName,
    PasswordHash,
    SecurityStamp,
    Email,
    EmailConfirmed,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEndDateUtc,
    LockoutEnabled,
    AccessFailedCount,
    LastLogInAtUtc,
    IsDisabled,
    AddedAtUtc) 
VALUES (
    @FirstName,
    @LastName,
    @UserName,
    @PasswordHash,
    @SecurityStamp,
    @Email,
    @EmailConfirmed,
    @PhoneNumber,
    @PhoneNumberConfirmed,
    @TwoFactorEnabled,
    @LockoutEndDateUtc,
    @LockoutEnabled,
    @AccessFailedCount,
    @LastLogInAtUtc,
    @IsDisabled,
    GETUTCDATE())
";

            public const string Update = @"
UPDATE Users SET
FirstName = @FirstName,
LastName = @LastName, 
UserName = @UserName, 
PasswordHash = @PasswordHash, 
SecurityStamp = @SecurityStamp, 
Email = @Email, 
EmailConfirmed = @EmailConfirmed, 
PhoneNumber = @PhoneNumber, 
PhoneNumberConfirmed = @PhoneNumberConfirmed, 
TwoFactorEnabled = @TwoFactorEnabled, 
LockoutEndDateUtc = @LockoutEndDateUtc, 
LockoutEnabled = @LockoutEnabled, 
AccessFailedCount = @AccessFailedCount,
LastLogInAtUtc = @LastLogInAtUtc,
IsDisabled = @IsDisabled,
ModifiedAtUtc = GETUTCDATE()
WHERE Id = @Id
";

            public const string Delete = @"
DELETE FROM Users WHERE Id = @Id
";

            public const string SelectByLogin = @"
SELECT TOP 1
    U.Id, 
    U.FirstName,
    U.LastName,
    U.UserName,
    U.PasswordHash,
    U.SecurityStamp,
    U.Email,
    U.EmailConfirmed,
    U.PhoneNumber,
    U.PhoneNumberConfirmed,
    U.TwoFactorEnabled,
    U.LockoutEndDateUtc,
    U.LockoutEnabled,
    U.AccessFailedCount,
    U.LastLogInAtUtc,
    U.IsDisabled,
    U.AddedAtUtc,
    U.ModifiedAtUtc
FROM Users U
INNER JOIN UserLogins UL ON UL.UserId = U.Id
WHERE UL.LoginProvider = @LoginProvider
AND UL.ProviderKey = @ProviderKey
";
        }
    }
}