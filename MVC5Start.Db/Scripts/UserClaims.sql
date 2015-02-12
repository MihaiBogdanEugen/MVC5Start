CREATE TABLE [dbo].[UserClaims]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] NVARCHAR(MAX) NOT NULL,
	[ClaimValue] NVARCHAR(MAX) NOT NULL,
	[AddedAtUtc] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(), 
	CONSTRAINT [PK_UserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserClaims_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
) 
GO

CREATE INDEX [IX_UserClaims_UserId] ON [dbo].[UserClaims] ([UserId])
GO

CREATE UNIQUE INDEX [UX_UserClaims_UserId_ClaimType] ON [dbo].[UserClaims] ([UserId], [ClaimType])
GO