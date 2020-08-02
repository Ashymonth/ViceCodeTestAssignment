namespace CrudTestAssignment.DAL
{
    internal static class StoredProcedures
    {
        public static class Users
        {
            public const string AddUserProcedureName = "AddUser";
            public const string GetByIdProcedureName = "GetUserById";
            public const string GetAllProcedureName = "GetAllUsers";
            public const string GetByNameProcedureName = "GetUserByName";
            public const string UpdateProcedureName = "UpdateUser";
            public const string DeleteProcedureName = @"DeleteUser";
        }
    }
}