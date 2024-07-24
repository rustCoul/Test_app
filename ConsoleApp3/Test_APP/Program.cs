using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp3
{
    internal class Program
    {
        static async Task Main(string[] args) 
        {
            

            int choice_to_continue = 0;
           
           
               
            while (choice_to_continue == 0)
            {
                switch (choice_to_continue)
                {
                    case 1:
                        // Создание базы данных
                        await Create_database_Async();
                        // Создание таблицы
                        await Create_table_Async();
                        Console.WriteLine("continue?Yes:0 No:any number");
                        choice_to_continue = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 2: 
                        //создание Объекта
                        Console.WriteLine("Заполни в формате Ivanov Petr Sergeevich 2009-07-12 Male");
                        string sentence = Console.ReadLine();
                        Create_Employee(sentence);
                        Console.WriteLine("continue?Yes:0 No:any number");
                        choice_to_continue = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 3:
                       await select_Every_employee();
                        Console.WriteLine("continue?Yes:0 No:any number");
                        choice_to_continue = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 4:
                        await Fill_Million_rows(10);
                        Console.WriteLine("continue?Yes:0 No:any number");
                        choice_to_continue = Convert.ToInt32(Console.ReadLine());
                        break;
                }


                
               
                //}
                
               
            }

            //await select_Every_employee();
            Console.Read();

        }
        static async Task Create_database_Async()
         {
            SqlConnection conn_server = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trusted_Connection=True");
            using (conn_server)
            {
                await conn_server.OpenAsync();  
                SqlCommand command = new SqlCommand("CREATE DATABASE adonetdb", conn_server);
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("База данных создана");
            }
        }
        static async Task Create_table_Async()
        {
            SqlConnection conn_database = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=adonetdb;Integrated Security=True;Trusted_Connection=True;");
            using (conn_database)
            {
                await conn_database.OpenAsync();
                SqlCommand command2 = new SqlCommand("CREATE TABLE Employee (Id INT PRIMARY KEY IDENTITY,Full_Name NVARCHAR(100) NOT NULL ,Age Date NOT NULL ,sex  NVARCHAR(100)  NOT NULL)", conn_database);
                await command2.ExecuteNonQueryAsync();
                Console.WriteLine("Таблица создана");
            }
        }
        static void Create_Employee(string sentence)
        {
            string[] f = sentence.Split(' ');
            for (int i = 0; i < f.Length; i++)
            {
                Console.WriteLine(f[i]);

            }

            string FullName = "";
            for (int i = 0; i < 3; i++)
            {
                FullName += " " + f[i] + "";
            }
            // для дата Рождения
            string[] s = f[3].Split('-');
            DateTime date1 = new DateTime(Convert.ToInt32(s[0]), Convert.ToInt32(s[1]), Convert.ToInt32(s[2]));

            Person person = new Person(FullName, date1, f[4]);
            Console.WriteLine("Объект создан");
        }

        static async Task select_Every_employee()
        {
            SqlConnection conn_database = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=adonetdb;Integrated Security=True;Trusted_Connection=True;");
            using (conn_database)
            {
                await conn_database.OpenAsync();
                SqlCommand command = new SqlCommand("SELECt * FROM Employee", conn_database);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<string[]> table_dataset = new List<string[]>();


                    if (reader.HasRows) 
                    {
                    // выводим названия столбцов
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        string columnName3 = reader.GetName(2);
                        string columnName4 = reader.GetName(3);

                        Console.WriteLine($"{columnName1} \t{columnName2} \t{columnName3} \t{columnName4}");
                       
                        while (await reader.ReadAsync()) // построчно считываем данные
                        {
                            string[] row = new string[4];
                            row[0] = reader.GetValue(0).ToString();
                            row[1] = reader.GetValue(1).ToString();
                            row[2] = reader.GetValue(2).ToString();
                            row[3] = reader.GetValue(3).ToString();
                            table_dataset.Add(row);


                            Console.WriteLine($"{row[0]} \t{row[1]} \t{row[2]} \t{row[3]}");



                        }
                        string[][] array_dataset = table_dataset.ToArray();
                        Bubble_Sort_Matrix(array_dataset);
                        Console.WriteLine("отсортированный массив:");
                        for(int j=0; j< array_dataset.GetLength(0); j++)
                        {

                            Console.WriteLine($"{array_dataset[j][0]} \t{array_dataset[j][1]} \t{array_dataset[j][2]} \t{array_dataset[j][3]}");

                        }
                    }
                }
            }
        }
        static void Swap_Array(string[][] arr,int j,int s)
        {
            string[] temp= new string[4];
            for (int i = 0;i < 4; i++)
            {
                temp[i]= arr[j][i]; 
                arr[j][i] = arr[s][i];
                arr[s][i]= temp[i];
            }
        }
       static void Bubble_Sort_Matrix(string[][] arr)
        {
            for (int i = 1; i < arr.GetLength(0); i++)
            {
                for(int j = 0; j < arr.GetLength(0)-1; j++)
                {
                    if ( string.Compare(arr[j][1] , arr[j + 1][1])>0)
                    {
                        Swap_Array(arr,j,j+1);
                    }
                }
            }
        }

        static async Task Fill_Million_rows(int limit)
        {
            SqlConnection conn_database = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=adonetdb;Integrated Security=True;Trusted_Connection=True;");
            using (conn_database)
            {
                await conn_database.OpenAsync();
      
                SqlCommand command = new SqlCommand(@"
 
WITH CleanedData AS (
    SELECT 
        e.Full_Name AS Full_Name,
        CAST(e.Age AS VARCHAR(50)) AS CleanedAge,
        e.Sex AS Sex
    FROM Employee e
),
ParsedData AS (
    SELECT 
        Full_Name,
        
        SUBSTRING(CleanedAge, 1, 4) AS YearOfbirth,
        SUBSTRING(CleanedAge, 6, 2) AS MonthOfbirth,
        SUBSTRING(CleanedAge, 9, 2) AS DayOfbirth,
        Sex
    FROM CleanedData
),
SplitNames AS (
    SELECT 
        Full_Name,
        -- Extract the positions of spaces for splitting names
        CHARINDEX(' ', LTRIM(RTRIM(Full_Name))) AS Start,
        CHARINDEX(' ', LTRIM(RTRIM(Full_Name)), CHARINDEX(' ', LTRIM(RTRIM(Full_Name))) + 1) AS SecondStart
    FROM CleanedData -- Ensure we are selecting from CleanedData to avoid ambiguity
)

SELECT 
    SUBSTRING(LTRIM(RTRIM(pd.Full_Name)), 1, sn.Start - 1) AS FirstName,
    SUBSTRING(LTRIM(RTRIM(pd.Full_Name)), sn.Start + 1, sn.SecondStart - (sn.Start + 1)) AS MiddleName,
    SUBSTRING(LTRIM(RTRIM(pd.Full_Name)), sn.SecondStart + 1, LEN(pd.Full_Name) - (sn.SecondStart+1)) AS LastName,
    pd.YearOfbirth AS YearOfbirth,
    pd.MonthOfbirth AS MonthOfbirth,
    pd.DayOfbirth AS DayOfbirth,
    pd.Sex AS Sex
FROM ParsedData pd
JOIN SplitNames sn ON pd.Full_Name = sn.Full_Name;
                    ", conn_database);
                List<string[]> table_dataset = new List<string[]>();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    // выводим названия столбцов
                    string columnName1 = reader.GetName(0);
                    string columnName2 = reader.GetName(1);
                    string columnName3 = reader.GetName(2);
                    string columnName4 = reader.GetName(3);
                    string columnName5 = reader.GetName(4);
                    string columnName6 = reader.GetName(5);
                    string columnName7 = reader.GetName(6);
                    int i = 0;
                    Console.WriteLine($"{columnName1} \t{columnName2} \t{columnName3} \t{ columnName4} \t{ columnName5} \t{ columnName6} \t{ columnName7}");
                    while (await reader.ReadAsync())
                    {
                        string[] row = new string[7];
                        row[0] = reader.GetValue(0).ToString();
                        row[1] = reader.GetString(1);
                        row[2] = reader.GetValue(2).ToString();
                        row[3] = reader.GetValue(3).ToString();
                        row[4] = reader.GetString(4);
                        row[5] = reader.GetString(5);
                        row[6] = reader.GetString(6);
                        table_dataset.Add(row);
                        Console.WriteLine($"{row[0]} \t{row[1]} \t{row[2]} \t{ row[3]} \t{ row[4]} \t{ row[5]} \t{ row[6]}");
                        i++;

                    }
                    string[][] array = table_dataset.ToArray(); 
                    foreach (var n in Generate_Combinations(array, limit))
                    {
                        await Insert_Table_Async(n);
                    }
                    Console.WriteLine("База данных заполнена");
                }
            }
        }
        static string[] Generate_Combinations(string[][] arr, int limit)
        {
            string input_names="";
            string[] str_arr = new string[limit];
            int stop = 0;

            while (stop < limit)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(0); j++)
                    {
                        for (int k = 0; k < arr.GetLength(0); k++)
                        {
                            if (stop >= limit) break;
                             input_names = "'" + arr[i][0] + " " + arr[j][1] + " " + arr[k][2] + "','" + arr[i][3] + "-" + arr[j][4] + "-" + arr[k][5] + "','" + arr[k][6] + "'"; 
                            
                            
                            Console.WriteLine(input_names);
                            str_arr[stop] = input_names;
                            stop++;
                        }
                        if (stop >= limit) break;
                    }
                    if (stop >= limit) break;
                }
            }

            return str_arr;
        }
        static async Task Insert_Table_Async(string input_names)
        {
            SqlConnection conn_database = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=adonetdb;Integrated Security=True;Trusted_Connection=True;");
            using (conn_database)
            {
                await conn_database.OpenAsync();
                SqlCommand command2 = new SqlCommand("INSERT INTO  Employee  VALUES (" + input_names+ ")", conn_database);
                await command2.ExecuteNonQueryAsync();
                Console.WriteLine("Таблица создана");
            }
        }
    }
}
