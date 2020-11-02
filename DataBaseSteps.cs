using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

namespace DataBase_TEsting
{
    [Binding]
    public class DataBaseSteps
    {
        string connectionString = @"Data Source=DESKTOP-1VA8JIU\SQLEXPRESS;Initial Catalog=TEST_DB;Integrated Security=True";
        string sqlExpression;
        SqlDataAdapter adapter;
        DataSet dataSet;
        SqlConnection connection;
        SqlCommand sqlCommand;
        int num;
        
        string firstName;
        string lastName;
        int age;
        string city;

        [Given(@"person data (.*), (.*), (.*), (.*)")]
        public void GivenPersonData(string firstName, string lastName, int age, string city)
        {
            sqlExpression = string.Format("INSERT INTO Persons(firstName,lastName,age, city) VALUES('{0}','{1}','{2}','{3}')", firstName, lastName, age, city);
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.city = city;

        }

        [Given(@"new person data (.*), (.*), (.*), (.*)")]
        public void GivenNewPersonData(string firstName, string lastName, int age, string city)
        {
            sqlExpression = string.Format("UPDATE Persons SET FirstName = '{0}', LastName = '{1}', Age = '{2}', City = '{3}'  WHERE ID = (SELECT MAX(ID) FROM Persons)", firstName, lastName, age, city);
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = age;
            this.city = city;

        }



        [When(@"I send request to DB")]
        public void WhenISendRequestToDB()
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                sqlCommand = new SqlCommand(sqlExpression, connection);
                num = sqlCommand.ExecuteNonQuery();
            }
            
        }
        
        [Then(@"new user created with person data")]
        public void ThenNewUserCreatedWithPersonData()
        {
            sqlExpression = "SELECT * FROM Persons WHERE ID = (SELECT MAX(ID) FROM Persons)";
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            Assert.AreEqual(firstName, dataSet.Tables[0].Rows[0].ItemArray[1]);
            Assert.AreEqual(lastName, dataSet.Tables[0].Rows[0].ItemArray[2]);
            Assert.AreEqual(age, dataSet.Tables[0].Rows[0].ItemArray[3]);
            Assert.AreEqual(city, dataSet.Tables[0].Rows[0].ItemArray[4]);
        }
        int sum;
        [Given(@"order sum (.*)")]
        public void GivenOrderSum(int sum)
        {
            this.sum = sum;
            sqlExpression = string.Format("INSERT INTO Orders(SUM_order, ID) VALUES({0},(SELECT MAX(ID) FROM Persons))", sum);
        }

        [Then(@"new order created")]
        public void ThenNewOrderCreated()
        {
            sqlExpression = "SELECT * FROM Orders WHERE ID = (SELECT MAX(ID) FROM Persons)";
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            Assert.AreEqual(sum, dataSet.Tables[0].Rows[0].ItemArray[1]);
        }

        [Given(@"request for delete orders")]
        public void GivenRequestForDeleteOrders()
        {
            sqlExpression = string.Format("DELETE FROM Orders WHERE ID=(SELECT MAX(ID) FROM Persons)");
            
        }

        [Then(@"order is deleted")]
        public void ThenOrderIsDeleted()
        {
            sqlExpression = String.Format("SELECT COUNT(ID) AS Count_ID FROM Orders WHERE ID = (SELECT MAX(ID)+1 FROM Orders)");
            
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            Assert.AreEqual(0, dataSet.Tables[0].Rows[0].ItemArray[0]);
        }
        int maxId;
        [Given(@"request for delete person")]
        public void GivenRequestForDeletePerson()
        {
            sqlExpression = String.Format("SELECT MAX(ID) AS max_id FROM Persons");
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            maxId = int.Parse(dataSet.Tables[0].Rows[0].ItemArray[0].ToString());
            sqlExpression = String.Format("DELETE FROM Persons WHERE ID = {0}", maxId);
        }

        [Then(@"person is deleted")]
        public void ThenPersonIsDeleted()
        {
            sqlExpression = String.Format("SELECT MAX(ID) AS max_id FROM Persons");
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlExpression, connection);

                dataSet = new DataSet();
                adapter.Fill(dataSet);
            }
            int newMaxId = int.Parse(dataSet.Tables[0].Rows[0].ItemArray[0].ToString());
            Assert.AreNotEqual(maxId, newMaxId);
        }

    }
}
