using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Windows;


namespace GraphicalStructure
{
    class UseAccessDB
    {
        private static string strConnect; //数据库连接对象
        private static OleDbConnection oleDbConn;//Access数据库连接对象

        public bool OpenDb()
        {
            try
            {
                //捕获数据库打开异常
                oleDbConn = new OleDbConnection(strConnect);
                oleDbConn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UseAccessDB()
        {
            strConnect = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=material.accdb";
            try
            {
                if (oleDbConn == null)
                {
                    OpenDb();
                }
                if (oleDbConn.State != ConnectionState.Open)
                {
                    oleDbConn.Open();
                }
            }
            catch
            {
                MessageBox.Show("打开数据库出现错误。","警告");
                return;
            }
            
        }

        public OleDbConnection getConnection()
        {
            if (oleDbConn.State != ConnectionState.Open)
            {
                oleDbConn.Open();
            }
            return oleDbConn;
        }

        public void closeDB()
        {
            if (oleDbConn.State == ConnectionState.Open)
            {
                oleDbConn.Close();
            }
        }

        // get the table names from the Access Database
        public ArrayList getTableName()
        {
            System.Data.DataTable dt = null;
            // Get the data table containing the schema
            dt = oleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            ArrayList al = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                string strSheetTableName = row["TABLE_NAME"].ToString();
                if (row["TABLE_TYPE"].ToString() == "TABLE" && strSheetTableName.Substring(0,1) != "~")
                {
                    Console.WriteLine(strSheetTableName);
                    al.Add(strSheetTableName);
                }
            }

            return al;
        }


        public ArrayList queryALLMaterialFromTable(string sql)
        {
            ArrayList result = new ArrayList();
            OleDbCommand cmd = new OleDbCommand(sql,oleDbConn);
            cmd.CommandType = CommandType.Text;
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ArrayList data = new ArrayList();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    data.Add(dr.GetValue(i));
                }
                result.Add(data);
            }
            return result;
        }

        public List<string> GetTableFieldNameList(string TableName)
        {
            List<string> list = new List<string>();
            try
            {
                if (oleDbConn.State == ConnectionState.Closed)
                    oleDbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 * FROM [" + TableName + "]";
                    cmd.Connection = oleDbConn;
                    OleDbDataReader dr = cmd.ExecuteReader();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        list.Add(dr.GetName(i));
                    }
                }
                return list;
            }
            catch (Exception e)
            { throw e; }
        }

        public DataSet SelectToDataSet(string SQL, string subtableName)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            OleDbCommand command = new OleDbCommand(SQL, oleDbConn);
            adapter.SelectCommand = command;
            DataSet Ds = new DataSet();
            Ds.Tables.Add(subtableName);
            adapter.Fill(Ds, subtableName);
            return Ds;
        }

        public DataView queryAllTable(string sql)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(sql, oleDbConn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds.Tables[0].DefaultView;
        }

        public ArrayList queryByCondition(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql,oleDbConn);
            OleDbDataReader reader = cmd.ExecuteReader();
            reader.Read();
            ArrayList array = new ArrayList();
            if (reader.HasRows)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine(reader[i]);
                    array.Add(reader[i]);
                }
            }
            
            return array;
        }

        public bool insertTableData(string sql)
        {
            OleDbCommand oleCom = new OleDbCommand();
            oleCom.Connection = oleDbConn;
            oleCom.CommandText = sql;
            bool flag = true;

            try
            {
                oleCom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                flag = false;
            }
            return flag;
        }

        public bool deleteFromTableData(string sql,int id)
        {
            OleDbCommand oleCom = new OleDbCommand();
            oleCom.Connection = oleDbConn;
            oleCom.CommandText = sql;
            bool flag = true;
            int resultNum = 10;
            try
            {
                resultNum = Convert.ToInt32(oleCom.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                flag = false;
            }

            //if (resultNum == 1)
            //{
            //    OleDbCommand cmd = new OleDbCommand();
            //    cmd.Connection = oleDbConn;
            //    cmd.CommandText = "update Material set ID=ID-1 where ID>"+id;
            //    cmd.ExecuteNonQuery();
            //}

            return flag;
        }

        public bool updateTableData(string sql)
        {
            OleDbCommand cmd = new OleDbCommand(sql, oleDbConn);
            bool flag = true;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                flag = false;
            }

            return flag;
        }
    }
}
