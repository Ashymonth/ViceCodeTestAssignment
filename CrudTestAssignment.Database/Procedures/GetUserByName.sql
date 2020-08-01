Create procedure [dbo].[GetUserByName]
	@userName nvarchar(64)
as
	select [id], [name], [createdDate] 
    from [dbo].[Users] 
    where [name] = @userName

