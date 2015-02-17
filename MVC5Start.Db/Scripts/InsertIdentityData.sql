SET IDENTITY_INSERT [dbo].[Roles] ON 

GO
INSERT [dbo].[Roles] ([Id], [Name], [Description], [AddedAtUtc], [ModifiedAtUtc]) VALUES (1, N'Administrator', N'Administrator role, full privileges', CAST(N'2015-02-16 16:10:18.1470000' AS DateTime2), NULL)
GO
INSERT [dbo].[Roles] ([Id], [Name], [Description], [AddedAtUtc], [ModifiedAtUtc]) VALUES (2, N'User', N'Default user role, limited privileges', CAST(N'2015-02-16 16:13:43.4100000' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

GO
INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [UserName], [PasswordHash], [SecurityStamp], [Email], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [GoogleAuthenticatorSecretKey], [TwoFactorAuthPasswordHash], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [LastLoginAtUtc], [IsDisabled], [AddedAtUtc], [ModifiedAtUtc]) VALUES (1, N'Bogdan', N'Mihai', N'mbe1224@gmail.com', N'ANcpbhlcgyKGIEIOuPYJcy7/8ndS20jxGeZ6l7Hw6CprueeEDZLtjUw07ZZaHt4faQ==', N'36a908f9-27e2-4f18-aef1-12e0d458e3db', N'mbe1224@gmai.com', 1, N'0721601985', 1, 0, NULL, NULL, NULL, 1, 0, NULL, 0, CAST(N'2015-02-17 08:51:10.1900000' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRoles] ON 

GO
INSERT [dbo].[UserRoles] ([Id], [UserId], [RoleId], [AddedAtUtc]) VALUES (1, 1, 1, CAST(N'2015-02-17 15:09:28.8870000' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[UserRoles] OFF
GO
