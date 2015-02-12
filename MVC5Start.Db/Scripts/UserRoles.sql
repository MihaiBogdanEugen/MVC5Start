CREATE TABLE [dbo].[UserRoles]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[RoleId] INT NOT NULL,
	[AddedAtUtc] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(), 
	CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]), 
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) 
)
GO

CREATE INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId])
GO

CREATE INDEX [IX_Roles_RoleId] ON [dbo].[UserRoles] ([RoleId])
GO

CREATE UNIQUE INDEX [UX_UserRoles_UserId_RoleId] ON [dbo].[UserRoles] ([RoleId], [UserId])
GO