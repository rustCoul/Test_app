using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ConsoleApp3
{
    internal class Person
    {
        private string FullName;
        private DateTime Birthday;
        private string sex;
        private string table= "Employee";
        SqlConnection conn_database = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=adonetdb;Integrated Security=True;Trusted_Connection=True;");

        public Person(string FullName, DateTime Age , String sex) { 
            this.FullName = FullName;
            this.Birthday = Age;
            this.sex = sex;
            ADD();
        }
        internal async void ADD()
        {
            using (conn_database)
            {
                await conn_database.OpenAsync();

                string input_names = $"'{FullName}','{Birthday.ToString("yyyy-MM-dd")}','{sex}'";
                SqlCommand cmd = new SqlCommand("INSERT INTO " + table + " VALUES (" + input_names + ")", conn_database);
                 cmd.ExecuteNonQuery();
                Console.WriteLine($"Добавлено объектов");


            }
           
        }
        internal int How_Old_is()
        {
        
            
            DateTime Today = DateTime.Today;
            TimeSpan diffResult = Today-Birthday;
            return diffResult.Days/365;
        }
    }
}
