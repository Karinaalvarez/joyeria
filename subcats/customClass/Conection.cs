using Microsoft.Data.SqlClient;
using System;


namespace subcats.customClass
{
    public class Conection
    {
        public SqlConnection connection;
        public SqlCommand sqlCommand;
        public SqlDataReader sqlDataReader;
        private const string connectionString = @"Server=LAPTOP-UVA1MQ5B\SQLEXPRESS;Database=casaOro;Trusted_Connection=True;TrustServerCertificate=true;";


        public Conection()
        {
            connection = new SqlConnection(connectionString);
        }
    }
}