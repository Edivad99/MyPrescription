using Dapper;
using MyPrescription.Data.Entity;
using System.Data;

namespace MyPrescription.Data.Repository;

public class NotificationRepository : Repository
{
    public NotificationRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddNotificationSubscriptionAsync(Notification notification)
    {
        var sql = @"INSERT INTO `Notifications` (`IDUser`, `Url`, `P256dh`, `Auth`, `CreationDate`) VALUES
                  (@IDUSER, @URL, @P256DH, @AUTH, @CREATIONDATE);";

        var dynParam = new DynamicParameters();
        dynParam.Add("@IDUSER", notification.IDUser, DbType.String, ParameterDirection.Input);
        dynParam.Add("@URL", notification.Url, DbType.String, ParameterDirection.Input);
        dynParam.Add("@P256DH", notification.P256dh, DbType.String, ParameterDirection.Input);
        dynParam.Add("@AUTH", notification.Auth, DbType.String, ParameterDirection.Input);
        dynParam.Add("@CREATIONDATE", notification.CreationDate, DbType.DateTime, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }

    public async Task<IEnumerable<Notification>> GetNotificationSubscriptionAsync(string patientId)
    {
        var sql = @"SELECT *
                    FROM Notifications 
                    WHERE IDUser = @IDUSER;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@IDUSER", patientId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<Notification>(sql, dynParam);
    }

    public async Task RemoveNotificationSubscriptionByUrlAsync(string url)
    {
        var sql = @"DELETE FROM Notifications WHERE Url = @URL;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@URL", url, DbType.String, ParameterDirection.Input);

        using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }
}
