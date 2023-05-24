using Dapper;
using MyPrescription.Data.Entity;

namespace MyPrescription.Data.Repository;

public class DoctorRepository : Repository
{
    public DoctorRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<User>> GetDoctorsAsync()
    {
        var sql = @"SELECT *
                    FROM Users
                    WHERE Role = 'doctor';";

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<User>(sql);
    }
}
