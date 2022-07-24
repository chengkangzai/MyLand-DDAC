-- -------------------------------------------------------------
-- TablePlus 4.8.0(432)
--
-- https://tableplus.com/
--
-- Database: MyLand
-- Generation Time: 2022-07-24 16:11:08.0200
-- -------------------------------------------------------------


DROP TABLE IF EXISTS [dbo].[__EFMigrationsHistory];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150),
    [ProductVersion] nvarchar(32),
    PRIMARY KEY ([MigrationId])
);

DROP TABLE IF EXISTS [dbo].[AspNetRoleClaims];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id] int IDENTITY,
    [RoleId] nvarchar(450),
    [ClaimType] nvarchar(MAX),
    [ClaimValue] nvarchar(MAX),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE 1,
    PRIMARY KEY ([Id])
);

DROP TABLE IF EXISTS [dbo].[AspNetRoles];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(450),
    [Name] nvarchar(256),
    [NormalizedName] nvarchar(256),
    [ConcurrencyStamp] nvarchar(MAX),
    PRIMARY KEY ([Id])
);

DROP TABLE IF EXISTS [dbo].[AspNetUserClaims];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY,
    [UserId] nvarchar(450),
    [ClaimType] nvarchar(MAX),
    [ClaimValue] nvarchar(MAX),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE 1,
    PRIMARY KEY ([Id])
);

DROP TABLE IF EXISTS [dbo].[AspNetUserLogins];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128),
    [ProviderKey] nvarchar(128),
    [ProviderDisplayName] nvarchar(MAX),
    [UserId] nvarchar(450),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE 1,
    PRIMARY KEY ([LoginProvider],[ProviderKey])
);

DROP TABLE IF EXISTS [dbo].[AspNetUserRoles];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] nvarchar(450),
    [RoleId] nvarchar(450),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles]([Id]) ON DELETE 1,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE 1,
    PRIMARY KEY ([UserId],[RoleId])
);

DROP TABLE IF EXISTS [dbo].[AspNetUsers];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(450),
    [UserName] nvarchar(256),
    [NormalizedUserName] nvarchar(256),
    [Email] nvarchar(256),
    [NormalizedEmail] nvarchar(256),
    [EmailConfirmed] bit,
    [PasswordHash] nvarchar(MAX),
    [SecurityStamp] nvarchar(MAX),
    [ConcurrencyStamp] nvarchar(MAX),
    [PhoneNumber] nvarchar(MAX),
    [PhoneNumberConfirmed] bit,
    [TwoFactorEnabled] bit,
    [LockoutEnd] datetimeoffset,
    [LockoutEnabled] bit,
    [AccessFailedCount] int,
    [FirstName] nvarchar(MAX),
    [LastName] nvarchar(MAX),
    [Address] nvarchar(MAX),
    [Telephone] int,
    [Role] int,
    PRIMARY KEY ([Id])
);

DROP TABLE IF EXISTS [dbo].[AspNetUserTokens];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId] nvarchar(450),
    [LoginProvider] nvarchar(128),
    [Name] nvarchar(128),
    [Value] nvarchar(MAX),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE 1,
    PRIMARY KEY ([UserId],[LoginProvider],[Name])
);

DROP TABLE IF EXISTS [dbo].[Property];
-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Property] (
    [Id] int IDENTITY,
    [Type] int,
    [Title] nvarchar(MAX),
    [Description] nvarchar(MAX),
    [Price] int,
    [Size] int,
    [Photo] nvarchar(MAX),
    [Date] datetime2(7),
    [UserId] nvarchar(450),
    [IsActive] bit,
    [MyLandUserId] nvarchar(450),
    [topicArn] nvarchar(MAX),
    CONSTRAINT [FK_Property_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
    CONSTRAINT [FK_Property_AspNetUsers_MyLandUserId] FOREIGN KEY ([MyLandUserId]) REFERENCES [dbo].[AspNetUsers]([Id]),
    PRIMARY KEY ([Id])
);

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
(N'20220709145434_initdb', N'3.1.25'),
(N'20220722131508_add_arn_to_property_table', N'3.1.25');

INSERT INTO [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [LastName], [Address], [Telephone], [Role]) VALUES
(N'3af273f5-9fee-4151-81bc-d5f0e6b37c88', N'johndoe@myland.com', N'JOHNDOE@MYLAND.COM', N'johndoe@myland.com', N'JOHNDOE@MYLAND.COM', '0', N'AQAAAAEAACcQAAAAEEz7g+hxhAGbyDUkymD4o/fZUCKOflbD2FDfNoDV1omqMWzUfeNNddSwfPda3r18ig==', N'PKOEXHKKVTSYJWZWBI2ONMWJIVSKKZBJ', N'04bd0730-5591-4031-a244-9d6eb996cc7e', NULL, '0', '0', NULL, '1', '0', N'john', N'doe', N'Jalan 1, Kuala Lumpur, Malaysia', '12345678', '2'),
(N'6b365af5-20df-45ec-a3e0-367ddde30836', N'janesmith@myland.com', N'JANESMITH@MYLAND.COM', N'janesmith@myland.com', N'JANESMITH@MYLAND.COM', '0', N'AQAAAAEAACcQAAAAEFANcV2pXwhbCpYdFiaDW+aILe65vxDcJXUToz+YQUPcuUG7oFPBovkYr2tG9xuWiQ==', N'Q4LHQ3ZB66LN7XH64BPXN2P47YHYHZZM', N'4c4a8097-4b91-4945-9c65-7ffd87f77ae5', NULL, '0', '0', NULL, '1', '0', N'jane', N'smith', N'Jalan 2, Kuala Lumpur, Malaysia', '12345678', '2'),
(N'798f3a00-8146-49a6-aeb0-4f16f118d7b0', N'admin@myland.com', N'ADMIN@MYLAND.COM', N'admin@myland.com', N'ADMIN@MYLAND.COM', '0', N'AQAAAAEAACcQAAAAEJL2OlmhG0udvxEKcAiSSWxHjjXpSRSMPV0kRhuPt+1tufZgsnwEHvL/WW4FrgDEOw==', N'OK6OPU4Y4WBVVZQUOZ2EEEUH4NLIVTRT', N'07e10b9a-d3fc-4650-abc7-811e64cf2ea5', NULL, '0', '0', NULL, '1', '0', N'Dacey', N'Campbell', N'Minim doloremque consectetur dolor aut sit sint voluptatem Quibusdam dolorem a autem laborum Culp', '65', '1'),
(N'8b752bea-5538-4290-909f-438b56074d3f', N'eixuan1217@gmail.com', N'EIXUAN1217@GMAIL.COM', N'eixuan1217@gmail.com', N'EIXUAN1217@GMAIL.COM', '0', N'AQAAAAEAACcQAAAAEJvYTCPtg2+PFc2i/TszrfxRN2jPqlmySEgNU5Bm88rgKjY8pYie/SR4JYOkVU5Qog==', N'LNBOLO42IIY7XU6OWVWTDE7DKS6HRF4F', N'64a0d89b-3b76-4102-9fc9-400a00f6b3fe', NULL, '0', '0', NULL, '1', '0', N'Xuan', N'Teoh', N'123, Jalan Putih, Taman Bunga, 93250, Kuching, Sarawak', '123232330', '2'),
(N'c66896c4-d82b-4a15-8077-93cb3d195a51', N'bobwilliams@email.com', N'BOBWILLIAMS@EMAIL.COM', N'bobwilliams@email.com', N'BOBWILLIAMS@EMAIL.COM', '0', N'AQAAAAEAACcQAAAAEIT3o7UXcigG5QZZaWCDiRdllzZ5ZoGSncBIroUu7Zj/ZkTZc3M9Xd/rXgnsn9U6lA==', N'ZXGL3Q4HGF2HNX253QYQZH2KD4CVCHHI', N'e9f640f7-4ee8-4ff1-ad11-d4cf9e122d0a', NULL, '0', '0', NULL, '1', '0', N'bob', N'williams', N'Jalan 3, Kuala Lumpur, Malaysia', '12345678', '0'),
(N'cfdf6d7e-a78c-4785-9f88-6ad26891dc64', N'agent@chengkangzai.com', N'AGENT@CHENGKANGZAI.COM', N'agent@chengkangzai.com', N'AGENT@CHENGKANGZAI.COM', '0', N'AQAAAAEAACcQAAAAEM0ghGL/1QcRvQKFM7dmyzeNxFGzTwIYeQX17kh4mtKU04FYe+EKR8X9Wqe0I3SwRA==', N'CX25ODJPWHDJGOKIP2I35F2HY6DQBHFE', N'782995d0-9257-4210-9993-7e284b82e931', NULL, '0', '0', NULL, '1', '0', N'Charissa', N'Wilkerson', N'Ea dolor dolore tempora nemo nisi qui labore ipsam vel veniam molestias velit eu optio', '43', '2'),
(N'f4a11090-9b49-4bdb-a7be-c9cf0bab11aa', N'admin@chengkangzai.com', N'ADMIN@CHENGKANGZAI.COM', N'admin@chengkangzai.com', N'ADMIN@CHENGKANGZAI.COM', '0', N'AQAAAAEAACcQAAAAEPblBpvJYUMhWTGjMX574DMq0f1gc7MtwIhzvn79W8/avgL4rYV0DJf/pQ6OuVggyQ==', N'DRY37UM3XN6KKMJIQGES6MFL2GZJREKS', N'c30a367a-3221-4ef6-be30-5da181d46bc4', NULL, '0', '0', NULL, '1', '0', N'CCK', N'CCK', N'CCK', '1231232131', '2'),
(N'fbf3cba9-8e5a-4a4e-a976-92f5d6481a1c', N'pycck@hotmail.com', N'PYCCK@HOTMAIL.COM', N'pycck@hotmail.com', N'PYCCK@HOTMAIL.COM', '0', N'AQAAAAEAACcQAAAAEF00sXht5q9Z8mcZa8DvaGmgxQ7Py/5PCQMCvitCOj23ir9JCSvzBXA2J4VkLlHwkA==', N'YPYBXIOD7QYBSD5ZON4OOIEL42QGMCLD', N'80cc2977-23ff-4274-b30a-a18fe32e99fd', NULL, '0', '0', NULL, '1', '0', N'd', N'Hansen', N'Ut quia voluptatibus', '44', '2');

INSERT INTO [dbo].[Property] ([Id], [Type], [Title], [Description], [Price], [Size], [Photo], [Date], [UserId], [IsActive], [MyLandUserId], [topicArn]) VALUES
('16', '1', N'Parkhill Residence', N'Near APU', '2300', '3', N'ph.jpeg', '2022-07-23 09:35:26.4005986', N'8b752bea-5538-4290-909f-438b56074d3f', '0', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor4'),
('17', '1', N'Vista Komanwel A', N'Very good for student!', '2200', '200', N'vista.PNG', '2022-07-23 13:04:29.9530217', N'3af273f5-9fee-4151-81bc-d5f0e6b37c88', '1', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor4'),
('18', '2', N'Land Bukit Jalil', N'Near commercial area', '100000', '200', N'land.PNG', '2022-07-23 13:23:01.5847309', N'6b365af5-20df-45ec-a3e0-367ddde30836', '1', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor4'),
('19', '0', N'Ut est nesciunt sun', N'Ea pariatur Mollit', '284', '72', N'IMG_9125_large.jpg', '2022-07-24 12:27:57.6614823', N'cfdf6d7e-a78c-4785-9f88-6ad26891dc64', '1', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor4'),
('20', '2', N'Autem autem aspernat', N'Do voluptatibus cons', '217', '71', N'land.png', '2022-07-24 04:52:13.8317411', N'3af273f5-9fee-4151-81bc-d5f0e6b37c88', '1', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor5'),
('21', '1', N'Id ab illo laborum o', N'Aliquam qui consequa', '969', '4', N'land.png', '2022-07-24 05:07:22.9221592', N'f4a11090-9b49-4bdb-a7be-c9cf0bab11aa', '1', NULL, N'arn:aws:sns:us-east-1:065974820827:PropertyNotificationFor6');

