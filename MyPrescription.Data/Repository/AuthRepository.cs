using Dapper;
using Dapper.Transaction;
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
        conn.Open();
        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            await transaction.ExecuteAsync(sql, dynParam);

            if (user.DoctorId is not null)
            {
                var sql2 = @"INSERT INTO `DoctorUser` (`IdDoctor`, `IdUser`) VALUES
                           (@IDDOCTOR, @IDUSER);";
                var dynParam2 = new DynamicParameters();
                dynParam2.Add("@IDDOCTOR", user.DoctorId, DbType.String, ParameterDirection.Input);
                dynParam2.Add("@IDUSER", user.Id, DbType.String, ParameterDirection.Input);

                await transaction.ExecuteAsync(sql2, dynParam2);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message, ex2.InnerException);
            }
            throw new Exception(ex.Message, ex.InnerException);
        }
        finally
        {
            await conn.CloseAsync();
        }
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
