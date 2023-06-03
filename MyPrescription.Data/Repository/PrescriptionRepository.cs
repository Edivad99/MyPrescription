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
        dynParam.Add("@CREATIONDATE", prescription.CreationDate, DbType.DateTime, ParameterDirection.Input);
        dynParam.Add("@ISFREE", prescription.IsFree, DbType.Boolean, ParameterDirection.Input);
        dynParam.Add("@DRUGNAME", prescription.DrugName, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        await conn.ExecuteAsync(sql, dynParam);
    }

    public async Task<IEnumerable<PrescriptionExpanded>> GetAllPrescriptionsByPatientIdAsync(string patientId)
    {
        var sql = @"SELECT Prescriptions.*, 
                           CONCAT(doctor.FirstName, "" "", doctor.LastName) AS DoctorName,
                           CONCAT(pharmacist.FirstName, "" "", pharmacist.LastName) AS PharmacistName
                    FROM Prescriptions
                    INNER JOIN Users AS patient ON patient.Id = Prescriptions.IdUser
                    INNER JOIN Users AS doctor ON doctor.Id = Prescriptions.IdDoctor
                    LEFT JOIN Users AS pharmacist ON pharmacist.Id = Prescriptions.IdPharmacist
                    WHERE IdUser = @ID AND patient.Role = 'patient'
                    ORDER BY CreationDate DESC;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", patientId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QueryAsync<PrescriptionExpanded>(sql, dynParam);
    }

    public async Task<Prescription> GetPrescriptionByIdAsync(string prescriptionId)
    {
        var sql = @"SELECT *
                    FROM Prescriptions
                    WHERE Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", prescriptionId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QuerySingleOrDefaultAsync<Prescription>(sql, dynParam);
    }

    public async Task<Prescription> GetPrescriptionByCode(string code)
    {
        var sql = @"SELECT *
                    FROM Prescriptions
                    WHERE SingleUseCode = @CODE;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@CODE", code, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.QuerySingleOrDefaultAsync<Prescription>(sql, dynParam);
    }

    public async Task<bool> DeletePrescriptionByIdAsync(string prescriptionId)
    {
        var sql = @"DELETE FROM Prescriptions WHERE Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", prescriptionId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.ExecuteAsync(sql, dynParam) == 1;
    }

    public async Task<bool> MarkPrescriptionAsDeliveredsync(string prescriptionId, string pharmacistId)
    {
        var sql = @"UPDATE Prescriptions
                    SET IdPharmacist = @IDPHARMACIST
                    WHERE Id = @ID;";

        var dynParam = new DynamicParameters();
        dynParam.Add("@ID", prescriptionId, DbType.String, ParameterDirection.Input);
        dynParam.Add("@IDPHARMACIST", pharmacistId, DbType.String, ParameterDirection.Input);

        await using var conn = GetDbConnection();
        return await conn.ExecuteAsync(sql, dynParam) == 1;
    }
}
