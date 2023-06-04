using System.Data;
using Dapper;
using MyPrescription.Data.Entity;

namespace MyPrescription.Data.Repository;

public class PatientRepository : Repository
{
    public PatientRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<User>> GetPatientsAsync(string doctorId)
    {
        var sql = @"SELECT Users.*
                    FROM Users
                    INNER JOIN DoctorUser ON DoctorUser.IdUser = Users.Id
                    WHERE Role = 'patient' AND DoctorUser.IdDoctor = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", doctorId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql, dynParam);
    }

    public async Task<User> GetPatientByIdAsync(string patientId)
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Role = 'patient' AND Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", patientId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(sql, dynParam);
    }
}
