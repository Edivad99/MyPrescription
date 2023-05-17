using System.Data;
using Dapper;
using MyPrescription.Data.Entity;

namespace MyPrescription.Data.Repository;

public class PrescriptionRepository : Repository
{
    public PrescriptionRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task AddNewPrescriptionAsync(Prescription prescription)
    {
        var sql = @"INSERT INTO `Prescriptions` (`Id`, `IdDoctor`, `IdUser`, `SingleUseCode`, `CreationDate`, `IsFree`, `DrugName`) VALUES
                  (@ID, @IDDOCTOR, @IDUSER, @SINGLEUSECODE, @CREATIONDATE, @ISFREE, @DRUGNAME);";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", prescription.Id, DbType.String, ParameterDirection.Input);
        dynParam.Add("@IDDOCTOR", prescription.IdDoctor, DbType.String, ParameterDirection.Input);
        dynParam.Add("@IDUSER", prescription.IdUser, DbType.String, ParameterDirection.Input);
        dynParam.Add("@SINGLEUSECODE", prescription.SingleUseCode, DbType.String, ParameterDirection.Input);
        dynParam.Add("@CREATIONDATE", prescription.CreationDate, DbType.Date, ParameterDirection.Input);
        dynParam.Add("@ISFREE", prescription.IsFree, DbType.Boolean, ParameterDirection.Input);
        dynParam.Add("@DRUGNAME", prescription.DrugName, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }
}
