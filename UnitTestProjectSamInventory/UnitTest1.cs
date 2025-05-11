using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOPIP;
using System;
using System.Windows.Forms;

namespace UnitTestProjectSamInventory
{
    [TestClass]
    public class UnitTest1
    {
        

        [TestMethod]
        public void checkSearchTest()
        {
            BD bd = new BD();
            AdminForm adminForm = new AdminForm();
            Controller controller = new Controller(bd);

            ComboBox comboBox = new ComboBox();
            comboBox.Text = "2";

            bool expected = true;
         
            bool actual = controller.checkSearch(comboBox);

            Assert.AreEqual(expected, actual, "Test is not correct");
        }

        [TestMethod]
        public void getSqlCommand()
        {
            BD bd = new BD();
            Controller controller = new Controller(bd);

            string expected = "SELECT id_user, name, login, role FROM user";

            string actual = controller.getSqlCommand("user");

            Assert.AreEqual(expected, actual, "Test is not correct");
        }

        [TestMethod]
        public void checkNullgetSqlCommand()
        {
            
            BD bd = new BD();
            Controller controller = new Controller(bd);

            string expected = "SELECT id_user, name, login, role FROM user";
            String data = "user";
            string actual = null;
            if (checkSearch(data))
            {
                 actual = controller.getSqlCommand(data);
            }
               
            Assert.AreEqual(expected, actual, "Test is not correct");
        }

        public bool checkSearch(String data)
        {
            if (data.ToString() == "")
            {
                return false;
            }
            return true;
        }

    }
}
