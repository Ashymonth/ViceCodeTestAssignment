Create procedure [dbo].[GetUserById]
	@userId int
as
	select [id], [name], [createdDate]
	from [dbo].[Users]
	where [id] = @userId