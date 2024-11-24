using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;


public class SQLHelper
{
    private readonly string _connectionString;

    public SQLHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecuteStoredProcedure(string storedProcedureName, params SqlParameter[] parameters)
    {
        DataTable result = new DataTable();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                
                if (parameters != null && parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }
        }

        return result;
    }

    public bool StoredProcedureExists(string storedProcedureName)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = @"SELECT COUNT(*) 
                             FROM sys.objects 
                             WHERE type = 'P' AND name = @ProcName";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProcName", storedProcedureName);
                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
