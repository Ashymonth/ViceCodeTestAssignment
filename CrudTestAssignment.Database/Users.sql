CREATE TABLE [dbo].[Users]
(
	[id] int not null identity primary key,
	[name] nvarchar(64) not null ,
	[createdDate] datetime not null default(GetDate())
)

GO

CREATE UNIQUE INDEX [IX_Users_Name] ON [dbo].[Users] ([name]);
