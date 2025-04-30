using MaterialSkin.Controls;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;


namespace OOPIP
{
    public class Controller
    {
        private BD bd;
        DataTable dataTable;
        private String searchTable;
        private String searchColoumn;
        private String searchValue;
        private String hash="#MFJUbf&7fkf!";

        public Controller (BD bd)
        {
            this.bd = bd;
        }

        public void autorization(String loginUser, String passwordUser, AutorizationForm autorizationForm)
        {
            bd.openConnection();
            string hashPasswordUser= Rijndael256.Hash.Sha512(passwordUser);
            String loginCommand = "SELECT * FROM user where `login` = '" + loginUser + "' and `password` = '" + hashPasswordUser + "';";
            DataTable table = bd.getDataTable(loginCommand);
            if (table.Rows.Count > 0)
            {
                checkAdmin(loginCommand);
                autorizationForm.Hide();
                bd.closeConnection();
            }
            else
                MaterialMessageBox.Show("wrong login or password");
        }

        public DataSet viewTable(String tableName)
        {
            
            return bd.getDataSet(getSqlCommand(tableName));

        }

        public void readyDataSearch(String choose_table, MaterialComboBox SearchColumnComboBox)
        {
            setSearchTable(choose_table);
            switch (getSerchTable())
            {
                case "user":
                    SearchColumnComboBox.Items.AddRange(new string[] { "id_user", "name", "login", "password", "role"});
                    break;
                case "equipment":
                    SearchColumnComboBox.Items.AddRange(new string[] { "id_equipment", "name_equipment", "name_category", "price", "inventory_number", "state" });
                    break;
                case "supply":
                    SearchColumnComboBox.Items.AddRange(new string[] { "id_supply", "id_supplier", "total_price", "count_equipment", "date_supply", "id_user" });
                    break;
                case "supplier":
                    SearchColumnComboBox.Items.AddRange(new string[] { "id_supplier", "name", "phone", "adress", "email" });
                    break;
                case "category":
                    SearchColumnComboBox.Items.AddRange(new string[] { "id_category", "name_category" });
                    break;
            }
        }

        public void readyColoumnSearch(String choose_coloumn)
        {
            setSearchColoumn(choose_coloumn);
        }

        public void readyValueSearch(String choose_value)
        {
            setSearchValue(choose_value);
        }

        public DataSet search()
        {
            
            bd.openConnection();
            String searchCommand = null;
            switch (getSerchTable())
            {
                case "equipment":
                    searchCommand = "SELECT e.name_equipment, c.name_category, e.inventory_number, e.price, " +
                        "em.name, e.state from equipment as e " +
                        "left join category as c on e.id_category=c.id_category " +
                        "left join employee as em on e.id_employee=em.id_employee " +
                        "where " + getSearchColoumn() + " = '" + getSearchValue() + "' ;";
                    break;
                case "order":

                    searchCommand = "SELECT oc.id, c.name_client, oc.date_out, " +
                        "SUM(op.cost), u.name_user from order_client " +
                        "as oc left join order_product as op on oc.id=op.id_order " +
                        "left join user as u on oc.id_user=u.id_user left join client " +
                        "as c on oc.id_client=c.id_client group by " +
                        "oc.id having " + getSearchColoumn() + " = '" + getSearchValue() + "' ;";
                    break;
 
                default:
                    searchCommand = "SELECT * FROM " + getSerchTable() + " where " + getSearchColoumn() + " = '" + getSearchValue() + "' ;";
                    break;
            }
            
            return bd.getDataSet(searchCommand);
        }

        

        public String getSqlCommand(String tableName)
        {

            String sqlCommand = null;
            switch (tableName)
            {
                case "user":
                    sqlCommand = "SELECT id_user, name, login, role FROM user";
                    break;
                case "category":
                    sqlCommand = "SELECT name_category FROM category";
                    break;
                case "department":
                    sqlCommand = "SELECT name_department FROM department";
                    break;
                case "device":
                    sqlCommand = "SELECT d.id_device, d.ip_address, d.os, e.name " +
                        "from device as d " +
                        "left join employee as e on d.id_employee = e.id_employee;";
                    break;
                case "last_inventory":
                    sqlCommand = "SELECT li.name_equipment, c.name_category, e.inventory_number, e.price, " +
                        "em.name, e.state, e.last_inventory from last_inventory as e " +
                        "left join category as c on e.id_category=c.id_category " +
                        "left join employee as em on e.id_employee=em.id_employee;";
                    break;

                case "installed_software":
                    sqlCommand = "SELECT e.name AS 'Сотрудник', d.ip_address AS 'ip-адрес', s.name AS 'Название ПО', " +
                                 "i.date_install AS 'Дата установки', u.name AS 'Исполнитель' " +
                                 "from installed_software as i " +
                                 "left join device as d on d.id_device = i.id_device " +
                                 "left join employee as e on e.id_employee = d.id_employee " +
                                 "left join software as s on s.id_software = i.id_software " +
                                 "left join user as u on u.id_user = i.id_user;";
                    break;
                case "employee":
                    sqlCommand = "SELECT e.name AS 'ФИО', e.room_number AS '№ кабинета', d.name_department AS 'Отдел', e.position AS 'Должность' " +
                        "FROM employee as e " +
                        "left join department as d on d.id_department = e.id_department;";
                    break;
                default:
                    MaterialMessageBox.Show("Таблица не выбрана!");

                    break;
            }
            return sqlCommand;
        }

        public DataTable chooseTableUpdate(String tableName)
        {
            bd.openConnection();
            dataTable = bd.getDataTable("Select * from " + tableName);

            return dataTable;
        }

        public void update(String tableName)
        {
            bd.getAdapter().Update(dataTable);
        }

        public void checkAdmin(String sqlCommand)
        {
            MySqlDataReader reader = bd.getMySqlDataReader(sqlCommand);
            while (reader.Read())
            {
                if (reader[4].ToString() == "admin")
                {
                    AdminForm adminForm = new AdminForm();
                    adminForm.Show();
                   
                }
                if (reader[4].ToString() == "user")
                {
                    UserForm userForm = new UserForm();
                    userForm.Show();
                }
            }
        }

       public bool checkSearch(ComboBox SearchDataComboBox)
        {
            if (SearchDataComboBox.Text.ToString() == "")
            {
                MaterialMessageBox.Show("Select Search Data!!!");
                return false;
            }
            return true;
        }

        public void export(DataGridView dataGridView)
        {
            Excel.Application exApp = new Excel.Application();
            exApp.Workbooks.Add();
            Excel.Worksheet wsh = (Excel.Worksheet)exApp.ActiveSheet;

            int i, j;

            for (i = 0; i <= dataGridView.RowCount - 2; i++)
            {
                for (j = 0; j <= dataGridView.ColumnCount - 1; j++)
                {
                    wsh.Cells[1, j + 1] = dataGridView.Columns[j].HeaderText.ToString();
                    wsh.Cells[i + 2, j + 1] = dataGridView[j, i].Value.ToString();
                }

            }
            exApp.Visible = true;
        }

        public void exportToHead(DataGridView dataGridView)
        {
            var wordApp = new Word.Application();
            wordApp.Visible = true;
            var wordDoc = wordApp.Documents.Add();

            wordApp.Selection.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
            InsertDataWord(wordDoc, dataGridView);
            Marshal.ReleaseComObject(wordApp);
            Marshal.ReleaseComObject(wordApp);
        }

        public void InsertDataWord(Document doc, DataGridView dgv)
        { 
            var table = doc.Tables.Add(doc.Range(), dgv.Rows.Count + 1, dgv.Columns.Count);

            for (int j = 0; j < dgv.Columns.Count; j++)
            {
                table.Rows[1].Cells[j + 1].Range.Text = dgv.Columns[j].HeaderText;
            }

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    if (dgv[j, i].Value != null)
                    {
                        table.Rows[i + 2].Cells[j + 1].Range.Text = dgv[j, i].Value.ToString();
                    }
                }
            }

            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            table.Range.ParagraphFormat.SpaceAfter = 6;
            table.Borders.Enable = 1;
        }

            public void copyLastInventory()
        {
            string sqlCommand = "TRUNCATE inventory_software;";
            string sqlCommand2 = "INSERT INTO inventory_software(id_installed_software, id_device, id_software, date_install, " +
                 "id_user) " +
                 "SELECT id_installed_software, id_device, id_software, date_install, id_user " +
                 "FROM installed_software;";
            bd.doMySqlCommand(sqlCommand);
            bd.doMySqlCommand(sqlCommand2);
            MaterialMessageBox.Show("Инвентаризация сохранена!");
        }



            public DataSet viewChanges()
        {
            string sql = "SELECT 'Удалено' AS 'Действие над ПО', " +
                "e.name AS 'ФИО сотрудника', " +
                "d.ip_address AS 'ip-адрес', " +
                "s.name AS 'Название ПО', " +
                "i.date_install AS 'Дата установки' " +
                "FROM inventory_software i " +
                "LEFT JOIN installed_software ins ON " +
                "i.id_device = ins.id_device " +
                "AND i.id_software = ins.id_software " +
                "AND i.id_user = ins.id_user " +
                "LEFT JOIN device d ON i.id_device = d.id_device " +
                "LEFT JOIN employee e ON d.id_employee = e.id_employee " +
                "LEFT JOIN software s ON i.id_software = s.id_software " +
                "WHERE ins.id_installed_software IS NULL " +
                "UNION ALL SELECT 'Установлено' AS action, " +
                "e.name AS employee_name, d.ip_address, s.name AS software_name, " +
                "ins.date_install AS date_installed FROM " +
                "installed_software ins LEFT JOIN inventory_software i ON " +
                "ins.id_device = i.id_device " +
                "AND ins.id_software = i.id_software " +
                "AND ins.id_user = i.id_user " +
                "LEFT JOIN device d ON ins.id_device = d.id_device " +
                "LEFT JOIN employee e ON d.id_employee = e.id_employee " +
                "LEFT JOIN software s ON ins.id_software = s.id_software " +
                "WHERE i.id_installed_software IS NULL;";
            return bd.getDataSet(sql);
        }

        public BD getBD() { return bd; }
        public void setBD(BD bd) {  this.bd = bd; }

        public String getSerchTable() { return searchTable; }
        public void setSearchTable(String searchTable) { this.searchTable = searchTable; }

        public String getSearchColoumn() { return searchColoumn; }
        public void setSearchColoumn(String searchColoumn) { this.searchColoumn = searchColoumn; }

        public String getSearchValue() { return searchValue; }
        public void setSearchValue(String searchValue) { this.searchValue = searchValue; }


    }
}
