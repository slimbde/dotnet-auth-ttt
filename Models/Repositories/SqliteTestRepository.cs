using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using ttt.Models.DTOs;

namespace ttt.Models.Repositories
{
    public class SqliteTestRepository: IRepository
    {
        string conString;


        public SqliteTestRepository(IConfiguration config) => conString = config.GetConnectionString("sqlite");

        public void Create(Test obj)
        {
            string stmt = "INSERT INTO Test (Name, Age) VALUES (@Name, @Age)";

            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                SQLiteCommand cmd = new SQLiteCommand(stmt, conn);
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@Age", obj.Age);

                conn.Open();

                int result = cmd.ExecuteNonQuery();
            }
        }

        public List<Test> GetList()
        {
            List<Test> result = new List<Test>();

            string stmt = "SELECT * FROM Test";

            using(SQLiteConnection conn = new SQLiteConnection(conString))
            {
                SQLiteCommand cmd = new SQLiteCommand(stmt, conn);

                conn.Open();
                DbDataReader rd = cmd.ExecuteReader();

                while(rd.Read())
                {
                    result.Add(new Test
                    {
                        Age = Convert.ToInt32(rd["Age"]),
                        Id = Convert.ToInt32(rd["Id"]),
                        Name = rd["Name"].ToString(),
                    });
                }

                return result;
            }
        }
    }
}
