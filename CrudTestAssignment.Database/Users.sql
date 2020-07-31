CREATE TABLE [dbo].[Users]
(
	[id] int primary key identity not null,
	[name] nvarchar(max) not null ,
	[createdDate] datetime not null
)
