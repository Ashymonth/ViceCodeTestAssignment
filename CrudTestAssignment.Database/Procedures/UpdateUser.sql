Create procedure [dbo].[UpdateUser]
	@id int,
	@name nvarchar(64)
as
	update [dbo].[Users]
    set [name] = @name
    where [id] = @id