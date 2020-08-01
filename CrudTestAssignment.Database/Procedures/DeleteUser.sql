Create procedure [dbo].[DeleteUser]
	@id int
as
	delete [dbo].[Users] where [id] = @id