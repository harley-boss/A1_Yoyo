/// File:        DatabaseManager.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: Class that handles all interactions with a database. That involves connecting to and writing/reading
///              data to/from the database.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A1_Yoyo {
    
    class DatabaseManager {

        private static SqlConnection sqlConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="machineName">String name of machine where database is hosted</param>
        /// <param name="userName">String username if applicable</param>
        /// <param name="password">String password if applicable</param>
        public static void Init(String machineName, String userName, String password) {
            String connectionString = "Data Source=" + machineName + ";Initial Catalog=YoYo;";
            if (userName == "" || password == "") {
                connectionString += "Integrated Security=True";
            } else {
                connectionString += "User Id=" + userName + ";password=" + password + ";Trusted_Connection=False;";
            }
            sqlConnection = new SqlConnection(connectionString);
        }

        
        /// <summary>
        /// Tests a connection to the database
        /// </summary>
        /// <returns>True if connected</returns>
        public static Boolean IsConnected() {
            Boolean isConnected = true;
            try {
                sqlConnection.Open();
            } catch (SqlException sql) {
                Console.WriteLine("Error: " + sql.Message);
                isConnected = false;
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
                isConnected = false;
            } finally {
                sqlConnection.Close();
            }
            return isConnected;
        }


        /// <summary>
        /// Inserts a yoyo into the database
        /// </summary>
        /// <param name="yoyo">Yoyo object with which to insert into the database</param>
        /// <returns>An integer as an asynchronous task: row id</returns>
        public static Task<int> Insert(Yoyo yoyo) {
            int returnCode = 0;
            using (SqlCommand command = new SqlCommand()) {
                command.Connection = sqlConnection;
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT into Yoyo_details (work_area, serial_number, line_number, state, reason, time_stamp, product_id)" + 
                                      "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7);";
                command.Parameters.AddWithValue("@param1", yoyo.WorkArea);
                command.Parameters.AddWithValue("@param2", yoyo.SerialNumber);
                command.Parameters.AddWithValue("@param3", yoyo.LineNumber);
                command.Parameters.AddWithValue("@param4", yoyo.State);
                command.Parameters.AddWithValue("@param5", yoyo.Reason);
                command.Parameters.AddWithValue("@param6", yoyo.TimeStamp);
                command.Parameters.AddWithValue("@param7", yoyo.ProductId);

                try {
                    sqlConnection.Open();
                    returnCode = command.ExecuteNonQuery();
                } catch (SqlException sql) {
                    Console.WriteLine("Caught exception on INSERT : " + sql.Message);
                } catch (Exception e) {
                    Console.WriteLine("Caught generic exception on INSERT: " + e.Message);
                } finally {
                    sqlConnection.Close();
                }
            }
            return Task.FromResult<int>(returnCode);
        }


        /// <summary>
        /// Selets all rows from the database based on unique id's
        /// </summary>
        /// <returns>List of yoyos selected as an asynchronous task</returns>
        public static async Task<List<Yoyo>> SelectAll() {
            String query = "SELECT * from(" +
                "SELECT " +
                "work_area, " +
                "serial_number, " +
                "line_number, " +
                "state, " +
                "reason, " +
                "time_stamp, " +
                "product_id, " +
                "row_number() over(partition by serial_number order by time_stamp desc) as rn " +
                "FROM " +
                    "Yoyo_details) t " +
                "WHERE t.rn = 1;";
            return await Select(query);
        }


        /// <summary>
        /// Selects all rows from the database where a rejection reason was provided
        /// </summary>
        /// <returns>List of yoyos selected as an asynchronous task</returns>
        public static async Task<List<Yoyo>> SelectAllRejections() {
            String query = "SELECT * from Yoyo_details " +
                "WHERE reason != '';";
            return await Select(query);
        }


        /// <summary>
        /// Select all yoyo product types where a rejection reason was provided
        /// </summary>
        /// <param name="type">Yoyo type to return</param>
        /// <returns>List of yoyos selected as an asynchronous task</returns>
        public static async Task<List<Yoyo>> SelectTypeRejections(YoyoType type) {
            String query = "SELECT * from Yoyo_details " +
                "WHERE reason != '' " +
                "AND product_id = " + (int)type + "; ";
            return await Select(query);
        }


        /// <summary>
        /// Select all unique yoyo id's based on type
        /// </summary>
        /// <param name="yoyoType">Yoyo type to return</param>
        /// <returns>List of yoyos selected as an asynchronous task</returns>
        public static async Task<List<Yoyo>> Select(YoyoType yoyoType) {
            String query = "SELECT * from(" +
                "SELECT " +
                "work_area, " +
                "serial_number, " +
                "line_number, " +
                "state, " +
                "reason, " +
                "time_stamp, " +
                "product_id, " +
                "row_number() over(partition by serial_number order by time_stamp desc) as rn " +
                "FROM " +
                    "Yoyo_details) t " +
                "WHERE t.rn = 1 AND product_id = " + (int)yoyoType + ";";
            return await Select(query);
        }


        /// <summary>
        /// Select method that performs actual query on the database
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>List of yoyos selected as an asynchronous task</returns>
        private static Task<List<Yoyo>> Select(String query) {
            List<Yoyo> yoyos = new List<Yoyo>();
            using (SqlCommand command = new SqlCommand()) {
                command.Connection = sqlConnection;
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                try {
                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            String yoyoString = reader.GetValue(0) + "," +
                                reader.GetValue(1) + "," +
                                reader.GetValue(2) + "," +
                                reader.GetValue(3) + "," +
                                reader.GetValue(4) + "," +
                                reader.GetValue(5) + "," +
                                reader.GetValue(6);
                            Yoyo yoyo = new Yoyo();
                            yoyos.Add(yoyo.Build(yoyoString));

                        }
                    }
                } catch (SqlException sql) {
                    Console.WriteLine("Caught exception on SELECT : " + sql.Message);
                } catch (Exception e) {
                    Console.WriteLine("Caught exception on INSERT: " + e.Message);
                } finally {
                    sqlConnection.Close();
                }
            }
            return Task.FromResult<List<Yoyo>>(yoyos); ;
        }
    }
}
