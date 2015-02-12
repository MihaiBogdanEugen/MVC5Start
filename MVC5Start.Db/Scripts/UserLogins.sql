CREATE TABLE [dbo].[UserLogins]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[LoginProvider] NVARCHAR(MAX) NOT NULL,
	[ProviderKey] NVARCHAR(MAX) NOT NULL,
	[AddedAtUtc] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(), 
	CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserLogins_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
) 
GO

CREATE INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins] ([UserId])
GO

CREATE UNIQUE INDEX [UX_UserLogins_UserId_LoginProvider] ON [dbo].[UserLogins] ([UserId], [LoginProvider])
GO