using Dapper;
using MyPrescription.Data.Entity;
using System.Data;

namespace MyPrescription.Data.Repository;

public class AuthRepository : Repository
{
    public AuthRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddUserAsync(User user)
    {
        var sql = @"INSERT INTO `Users` (`Id`, `FirstName`, `LastName`, `Email`, `Password`, `Gender`, `Birthdate`, `Key2FA`, `Role`) VALUES
                  (@ID, @FIRSTNAME, @LASTNAME, @EMAIL, @PASSWORD, @GENDER, @BIRTHDATE, @KEY2FA, @ROLE);";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", user.Id, DbType.String, ParameterDirection.Input);
        dynParam.Add("@FIRSTNAME", user.FirstName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@LASTNAME", user.LastName, DbType.String, ParameterDirection.Input);
        dynParam.Add("@EMAIL", user.Email, DbType.String, ParameterDirection.Input);
        dynParam.Add("@PASSWORD", user.Password, DbType.String, ParameterDirection.Input);
        dynParam.Add("@GENDER", user.Gender, DbType.String, ParameterDirection.Input);
        dynParam.Add("@BIRTHDATE", user.Birthdate, DbType.Date, ParameterDirection.Input);
        dynParam.Add("@KEY2FA", user.Key2FA, DbType.String, ParameterDirection.Input);
        dynParam.Add("@ROLE", user.Role, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Email = @EMAIL;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@EMAIL", email, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, dynParam);
    }
}
