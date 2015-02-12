CREATE TABLE [dbo].[Users]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
	[UserName] NVARCHAR(100) NOT NULL,
	[PasswordHash] NVARCHAR(MAX) NOT NULL,
	[SecurityStamp] NVARCHAR(MAX) NOT NULL,
	[Email] NVARCHAR(100) NOT NULL,
	[EmailConfirmed] BIT NOT NULL DEFAULT 0,
	[PhoneNumber] NVARCHAR(100) NULL,
	[PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
	[TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
	[LockoutEndDateUtc] DATETIME2(7) NULL,
	[LockoutEnabled] BIT NOT NULL DEFAULT 0,
	[AccessFailedCount] INT NOT NULL DEFAULT 0,
	[LastLoginAtUtc] DATETIME2 NULL, 
    [IsDisabled] BIT NOT NULL DEFAULT 0, 
    [AddedAtUtc] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(), 
    [ModifiedAtUtc] DATETIME2(7) NULL, 
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE UNIQUE INDEX [UX_Users_UserName] ON [dbo].[Users] ([UserName])
GO

CREATE UNIQUE INDEX [UX_Users_Email] ON [dbo].[Users] ([Email])
GO