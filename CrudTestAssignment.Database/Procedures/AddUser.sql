Create procedure [dbo].[AddUser]
	@name nvarchar(64),
	@createdDate datetime
as
	insert into [dbo].[Users] ([name], [createdDate])
	values(@name, @createdDate)
	select SCOPE_IDENTITY()