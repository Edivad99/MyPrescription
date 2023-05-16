using System.Data;
using Dapper;
using MyPrescription.Data.Entity;

namespace MyPrescription.Data.Repository;

public class PatientRepository : Repository
{
    public PatientRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<User>> GetPatientsAsync()
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Role = 'patient';";

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql);
    }

    public async Task<User> GetPatientByIdAsync(string patientId)
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Role = 'patient' AND Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", patientId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(sql, dynParam);
    }

    public async Task<IEnumerable<User>> GetPatientPrescriptionsAsync(string patientId)
    {
        var sql = @"SELECT *
                    FROM Prescriptions
                    INNER JOIN Users ON Users.Id = Prescriptions.IdUser
                    WHERE Role = 'patient' AND IdUser = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", patientId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql);
    }
}
