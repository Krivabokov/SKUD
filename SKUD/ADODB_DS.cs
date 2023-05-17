using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SKUD
{
    public class ADODB_DS : IDataRep
    {
        SqlConnection conn;

        public ADODB_DS()
        {          
            conn = new SqlConnection(
            "Server = (LocalDB)\\MSSQLLocalDB; " +
            "AttachDbFilename = \"C:\\Users\\criva\\OneDrive\\Рабочий стол\\SKUD\\SKUD.mdf\";" +
            "Integrated Security = True; Connect Timeout = 30;");
        }

        public bool AddPerson(string FIO, string Photo, string RFID)
        {
            string sql = "INSERT INTO Person (FIO, Photo, RFID) " +
                         "VALUES (@FIO, @Photo, @RFID)";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FIO", FIO);
                cmd.Parameters.AddWithValue("@Photo", Photo);
                cmd.Parameters.AddWithValue("@RFID", RFID);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool RemovePerson(string FIO, string RFID)
        {
            string sql = "DELETE FROM Person WHERE FIO = @FIO AND RFID = @RFID";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FIO", FIO);
                cmd.Parameters.AddWithValue("@RFID", RFID);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool AddDepartment(string Caption, string HeadName, string Telephone, string Email)
        {
            string sql = "INSERT INTO Department (Caption, HeadName, Telephone, Email) " +
                         "VALUES (@Caption, @HeadName, @Telephone, @Email)";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Caption", Caption);
                cmd.Parameters.AddWithValue("@HeadName", HeadName);
                cmd.Parameters.AddWithValue("@Telephone", Telephone);
                cmd.Parameters.AddWithValue("@Email", Email);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool RemoveDepartment(string Caption)
        {
            string sql = "DELETE FROM Department WHERE Caption = @Caption";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Caption", Caption);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool AddPosition(string Caption)
        {
            string sql = "INSERT INTO Position (Caption) " +
                         "VALUES (@Caption)";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Caption", Caption);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool RemovePosition(string Caption)
        {
            string sql = "DELETE FROM Position WHERE Caption = @Caption";

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Caption", Caption);

                int res = cmd.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return res > 0;
            }
        }

        public bool AddHeadOfDepartment(string Department, string Person)
        {
            // Находим идентификатор записи о новом начальнике в таблице Person
            string findPersonIdSql = "SELECT Id FROM Person WHERE FIO = @FIO";
            SqlCommand findPersonIdCmd = new SqlCommand(findPersonIdSql, conn);
            findPersonIdCmd.Parameters.AddWithValue("@FIO", Person);
            conn.Open();
            int personId = (int)findPersonIdCmd.ExecuteScalar();
            conn.Close();

            // Обновляем запись о начальнике в таблице Department
            string updateSql = "UPDATE Department SET HeadName = @HeadName, idPersonHead = @PersonId WHERE Caption = @Caption";
            SqlCommand updateCmd = new SqlCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("@HeadName", Person);
            updateCmd.Parameters.AddWithValue("@PersonId", personId);
            updateCmd.Parameters.AddWithValue("@Caption", Department);
            conn.Open();
            int res = updateCmd.ExecuteNonQuery();
            conn.Close();

            return res > 0;
        }

        public bool RemoveHeadOfDepartment(string Department)
        {
            // Обнуляем запись о начальнике в таблице Department
            string updateSql = "UPDATE Department SET HeadName = NULL, idPersonHead = NULL WHERE Caption = @Caption";
            SqlCommand updateCmd = new SqlCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("@Caption", Department);
            conn.Open();
            int res = updateCmd.ExecuteNonQuery();
            conn.Close();

            return res > 0;
        }

        public bool SetPositionsOfDepartment(string Department, List<string> ListOfPositions)
        {
            // Находим идентификатор записи о отделе в таблице Department
            string findDepartmentIdSql = "SELECT Id FROM Department WHERE Caption = @Caption";
            SqlCommand findDepartmentIdCmd = new SqlCommand(findDepartmentIdSql, conn);
            findDepartmentIdCmd.Parameters.AddWithValue("@Caption", Department);
            conn.Open();
            int departmentId = (int)findDepartmentIdCmd.ExecuteScalar();
            conn.Close();

            // Удаляем все записи в таблице DepartmentPosition для указанного отдела
            string deleteSql = "DELETE FROM DepartmentPosition WHERE idDepartment = @DepartmentId";
            SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
            deleteCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            conn.Open();
            deleteCmd.ExecuteNonQuery();
            conn.Close();

            // Добавляем новые записи в таблицу DepartmentPosition для указанных должностей
            foreach (string position in ListOfPositions)
            {
                // Находим идентификатор записи о должности в таблице Position
                string findPositionIdSql = "SELECT Id FROM Position WHERE Caption = @Caption";
                SqlCommand findPositionIdCmd = new SqlCommand(findPositionIdSql, conn);
                findPositionIdCmd.Parameters.AddWithValue("@Caption", position);
                conn.Open();
                int positionId = (int)findPositionIdCmd.ExecuteScalar();
                conn.Close();

                // Добавляем запись в таблицу DepartmentPosition
                string insertSql = "INSERT INTO DepartmentPosition (idDepartment, idPosition) " +
                                   "VALUES (@DepartmentId, @PositionId)";
                SqlCommand insertCmd = new SqlCommand(insertSql, conn);
                insertCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                insertCmd.Parameters.AddWithValue("@PositionId", positionId);
                conn.Open();
                insertCmd.ExecuteNonQuery();
                conn.Close();
            }

            return true;
        }

        public bool AddEmployee(string Person, string Department, string Position)
        {
            // Находим идентификатор персоны в таблице Person по ее ФИО
            string findPersonIdSql = "SELECT Id FROM Person WHERE FIO = @FIO";
            SqlCommand findPersonIdCmd = new SqlCommand(findPersonIdSql, conn);
            findPersonIdCmd.Parameters.AddWithValue("@FIO", Person);
            conn.Open();
            int personId = (int)findPersonIdCmd.ExecuteScalar();
            conn.Close();

            // Находим идентификатор отдела в таблице Department по его названию
            string findDepartmentIdSql = "SELECT Id FROM Department WHERE Caption = @Caption";
            SqlCommand findDepartmentIdCmd = new SqlCommand(findDepartmentIdSql, conn);
            findDepartmentIdCmd.Parameters.AddWithValue("@Caption", Department);
            conn.Open();
            int departmentId = (int)findDepartmentIdCmd.ExecuteScalar();
            conn.Close();

            // Находим идентификатор должности в таблице Position по ее названию
            string findPositionIdSql = "SELECT Id FROM Position WHERE Caption = @Caption";
            SqlCommand findPositionIdCmd = new SqlCommand(findPositionIdSql, conn);
            findPositionIdCmd.Parameters.AddWithValue("@Caption", Position);
            conn.Open();
            int positionId = (int)findPositionIdCmd.ExecuteScalar();
            conn.Close();

            // Добавляем новую запись о сотруднике в таблицу Employee
            string insertSql = "INSERT INTO Employee (idPerson, idPosition, idDepartment) " +
                               "VALUES (@PersonId, @PositionId, @DepartmentId)";
            SqlCommand insertCmd = new SqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@PersonId", personId);
            insertCmd.Parameters.AddWithValue("@PositionId", positionId);
            insertCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            conn.Open();
            insertCmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }
        public bool RemoveEmployee(string Person)
        {
            // Находим идентификатор персоны в таблице Person по ее ФИО
            string findPersonIdSql = "SELECT Id FROM Person WHERE FIO = @FIO";
            SqlCommand findPersonIdCmd = new SqlCommand(findPersonIdSql, conn);
            findPersonIdCmd.Parameters.AddWithValue("@FIO", Person);
            conn.Open();
            int personId = (int)findPersonIdCmd.ExecuteScalar();
            conn.Close();

            // Удаляем запись о сотруднике из таблицы Employee
            string deleteSql = "DELETE FROM Employee WHERE idPerson = @PersonId";
            SqlCommand deleteCmd = new SqlCommand(deleteSql, conn);
            deleteCmd.Parameters.AddWithValue("@PersonId", personId);
            conn.Open();
            deleteCmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        public bool AddLog(string RFID, int In_Out, int check, int FaceControl)
        { return false; }
        public bool RemoveLog(string StartDate, string EndDate)
        { return false; }

        public List<string> GetDepartments()
        {
            List<string> departments = new List<string>();

            string sql = "SELECT Caption FROM Department";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string caption = reader.GetString(reader.GetOrdinal("Caption"));
                        departments.Add(caption);
                    }
                }

                conn.Close();
            }

            return departments;
        }

        public List<string> GetPositions()
        {
            List<string> positions = new List<string>();

            string sql = "SELECT Caption FROM Position";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string caption = reader.GetString(reader.GetOrdinal("Caption"));
                        positions.Add(caption);
                    }
                }

                conn.Close();
            }

            return positions;
        }
        public List<string> GetDataByRFID()
        {
            List<string> lst = new List<string>();
            return lst;
        }
        public List<string> GetLog(string StartDate, string EndDate)
        {
            List<string> lst = new List<string>();
            return lst;
        }

        
    }
}
