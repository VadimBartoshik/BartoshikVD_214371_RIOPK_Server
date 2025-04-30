using MaterialSkin.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOPIP
{
    public class BD
    {
        private MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=sam_inventory;password=dflbv181818");
        private MySqlDataAdapter adapter;
        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            { connection.Open(); }
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            { connection.Close(); }
        }



        public DataTable getDataTable(String sqlCommand)
        {
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = getNewMySqlAdapter(sqlCommand);

            MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(adapter);

            
            adapter.UpdateCommand = mySqlCommandBuilder.GetUpdateCommand();
            adapter.DeleteCommand = mySqlCommandBuilder.GetDeleteCommand();
            adapter.InsertCommand = mySqlCommandBuilder.GetInsertCommand();
            adapter.Fill(table);
            setAdapter(adapter);
            return table;
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        public MySqlDataAdapter getAdapter()
        {
            return adapter;
        }

        public void setAdapter(MySqlDataAdapter adapter)
        {
            this.adapter = adapter;
        }

        public DataSet getDataSet(String sqlCommand){
                openConnection();
                DataSet ds = new DataSet();
          
                getNewMySqlAdapter(sqlCommand).Fill(ds);
                return ds;
        }

        public MySqlDataReader getMySqlDataReader(String sqlCommand)
        {
            openConnection();
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);
            MySqlDataReader reader = command.ExecuteReader();
            return reader;
        }


        public void doMySqlCommand(String sqlCommand)
        {
            MySqlCommand command = new MySqlCommand(sqlCommand, connection);
            command.ExecuteNonQuery();
        }


        public MySqlDataAdapter getNewMySqlAdapter(String sqlCommand)
        {
            return new MySqlDataAdapter(sqlCommand, connection);
        }
    }
}
