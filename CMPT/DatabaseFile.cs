﻿using CMPT;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CMPT
{
    public class DatabaseFile
    {
        public SqlConnection myConnection;
        public SqlCommand myCommand;
        public SqlDataReader myReader;

        public DatabaseFile(string userName, string password, string database)
        {
            string connectionString = "user id = " + userName + ";" + // Username  
                "password = " + password + ";" +// password 
                "server = localhost" + ";" +// IP for the server
                "Trusted_Connection = yes" + ";" +
                "database = " + database + ";" +// Database to connect to: CMPT291_MovieRentalSystem
                "connection timeout = " + "30";

            myConnection = new SqlConnection(connectionString); // timeout in seconds
            try
            {
                myConnection.Open(); // Open connection
                myCommand = new SqlCommand();
                myCommand.Connection = myConnection;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string[] getLoginInfo(string userID)
        {
            string salt = "";
            string passHash = "";
            
            myCommand.CommandText = "select passHash, salt from Login where userID = " + "\'" + userID + "\'";

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Read();

                salt = myReader["salt"].ToString().Trim();
                passHash = myReader["passHash"].ToString().Trim();

                myReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            var returnList = new List<string>();
            returnList.Add(salt);
            returnList.Add(passHash);

            return returnList.ToArray();
        }

        public Movie[] getAllMovies()
        {
            var movieList = new List<Movie>();
            myCommand.CommandText = "select * from Movies";
            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    MovieStruct movieStruct = new(myReader[(int)MovieEnum.movieID].ToString());

                    movieStruct.price = myReader[(int)MovieEnum.price].ToString();
                    movieStruct.genre = myReader[(int)MovieEnum.genre].ToString();
                    movieStruct.rating = myReader[(int)MovieEnum.rating].ToString();
                    movieStruct.copies = myReader[(int)MovieEnum.copies].ToString();
                    movieStruct.name = myReader[(int)MovieEnum.movieName].ToString();

                    movieList.Add(new Movie(movieStruct));
                }

                myReader.Close();
            }
            catch (Exception e3)
            {
                throw new Exception(e3.Message);
            }

            return movieList.ToArray();
        }

        public string[] getMovieTitles()
        {
            var movieList = new List<string>();

            myCommand.CommandText = "select * from Movies";
            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {      
                    string title = myReader.GetString(myReader.GetOrdinal("movieName")).ToString();
                    movieList.Add(title);
                }

                myReader.Close();
            }
            catch (Exception e3)
            {
                throw new Exception(e3.Message);
            }

            return movieList.ToArray();
        }

        public Movie[] GetMovies(string movieName)
        {
            var movieList = new List<Movie>();
            myCommand.CommandText = "select * from Movies where movieName LIKE '%" + movieName + "%';";

            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    MovieStruct movieStruct = new(myReader[(int)MovieEnum.movieID].ToString());

                    movieStruct.price = myReader[(int)MovieEnum.price].ToString();
                    movieStruct.genre = myReader[(int)MovieEnum.genre].ToString();
                    movieStruct.rating = myReader[(int)MovieEnum.rating].ToString();
                    movieStruct.copies = myReader[(int)MovieEnum.copies].ToString();
                    movieStruct.name = myReader[(int)MovieEnum.movieName].ToString();

                    movieList.Add(new Movie(movieStruct));
                }

                myReader.Close();
            }
            catch (Exception e3)
            {
                throw new Exception(e3.Message);
            }

            return movieList.ToArray();
        }

        public void SaveMovies(Movie[] movieArray)
        {
            foreach (Movie movie in movieArray)
            {
                myCommand.CommandText = "Update Movies SET movieName='" + movie.getName() + "',genre='" + movie.getGenre() + "',price=" + movie.getPrice() + ",copies=" + movie.getCopies()+ ",rating=" + movie.getRating() + " where movieID=" + movie.getId() + ";";

                try
                {
                    myReader = myCommand.ExecuteReader();

                    myReader.Close();

                }
                catch (Exception e3)
                {
                    throw new Exception(e3.Message);
                }
            }
        }

        public void AddMovie(Movie movie)
        { 
            {
                myCommand.CommandText = string.Format("insert into Movies values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');",
                    movie.getId(), movie.getName(), movie.getGenre(), movie.getPrice(), movie.getCopies(), movie.getRating());
                try
                {
                    myReader = myCommand.ExecuteReader();

                    myReader.Close();

                }
                catch (Exception e3)
                {
                    throw new Exception(e3.Message);
                }
            }
        }

        public void DeleteMovie(string movieId)
        {
            myCommand.CommandText = "DELETE FROM Movies where movieID=" + movieId;
            try
            {
                myReader = myCommand.ExecuteReader();
                myReader.Close();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AssignActor(string actorID, string movieID)
        {
            myCommand.CommandText = "insert into cast values(" + actorID + ", " + movieID + ");";
            try
            {
                myReader = myCommand.ExecuteReader();
                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void MakeCopy(string copyID, string movieID)
        {
            myCommand.CommandText = "insert into copies values(" + copyID + ", " + movieID + ");";
            try
            {
                myReader = myCommand.ExecuteReader();
                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Customer[] GetAllCustomers()
        {
            myCommand.CommandText = "select * from Customer";

            var customerList = new List<Customer>();
            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    CustomerStruct customerStruct = new CustomerStruct(Int32.Parse(myReader[(int)CustomerEnum.accountNo].ToString()));

                    customerStruct.lastName = myReader[(int)CustomerEnum.lastName].ToString();
                    customerStruct.firstName = myReader[(int)CustomerEnum.firstName].ToString();
                    customerStruct.phoneNumber = myReader[(int)CustomerEnum.phoneNumber].ToString();
                    customerStruct.email = myReader[(int)CustomerEnum.email].ToString();
                    customerStruct.streetNo = myReader[(int)CustomerEnum.streetNo].ToString();
                    customerStruct.streetName = myReader[(int)CustomerEnum.streetName].ToString();
                    customerStruct.aptNo = myReader[(int)CustomerEnum.aptNo].ToString();
                    customerStruct.city = myReader[(int)CustomerEnum.city].ToString();
                    customerStruct.postalCode = myReader[(int)CustomerEnum.postalCode].ToString();
                    customerStruct.phoneNumber = myReader[(int)CustomerEnum.phoneNumber].ToString();
                    customerStruct.creditCard = myReader[(int)CustomerEnum.creditCard].ToString();
                    customerStruct.rating = myReader[(int)CustomerEnum.rating].ToString();

                    Customer customer = new(customerStruct);
                    customerList.Add(customer);
                }

                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return customerList.ToArray();
        }


        public Customer[] GetCustomers(string customerName)
        {
            myCommand.CommandText = string.Format("select * from Customer where firstName LIKE %'{0}'%", customerName);

            var customerList = new List<Customer>();
            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    CustomerStruct customerStruct = new CustomerStruct(Int32.Parse(myReader[(int)CustomerEnum.accountNo].ToString()));

                    customerStruct.lastName = myReader[(int)CustomerEnum.lastName].ToString();
                    customerStruct.firstName = myReader[(int)CustomerEnum.firstName].ToString();
                    customerStruct.phoneNumber = myReader[(int)CustomerEnum.phoneNumber].ToString();
                    customerStruct.email = myReader[(int)CustomerEnum.email].ToString();
                    customerStruct.streetNo = myReader[(int)CustomerEnum.streetNo].ToString();
                    customerStruct.streetName = myReader[(int)CustomerEnum.streetName].ToString();
                    customerStruct.aptNo = myReader[(int)CustomerEnum.aptNo].ToString();
                    customerStruct.city = myReader[(int)CustomerEnum.city].ToString();
                    customerStruct.postalCode = myReader[(int)CustomerEnum.postalCode].ToString();
                    customerStruct.phoneNumber = myReader[(int)CustomerEnum.phoneNumber].ToString();
                    customerStruct.creditCard = myReader[(int)CustomerEnum.creditCard].ToString();
                    customerStruct.rating = myReader[(int)CustomerEnum.rating].ToString();

                    Customer customer = new(customerStruct);
                    customerList.Add(customer);
                }

                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return customerList.ToArray();
        }

        public void RemoveCustomer(int accountNo)
        {
            myCommand.CommandText = "DELETE FROM Customer where accountNo=" + accountNo;
            //MessageBox.Show(myCommand.CommandText);
            myReader = myCommand.ExecuteReader();
            myReader.Close();
        }

        public void SaveCustomers(Customer[] customers)
        {
            foreach (Customer customer in customers)
            {
                myCommand.CommandText = string.Format("Update Customer SET firstName='{0}',lastname='{1}',streetNo='{2}',streetName='{3}',aptNo='{4}',city='{5}',postalCode='{6}',phoneNumber='{7}',email='{8}',rating='{9}'",
                    customer.FirstName, customer.LastName, customer.StreetNo, customer.StreetName, customer.AptNo, customer.City, customer.PostalCode, customer.PhoneNumber, customer.Email, customer.Rating);

                try
                {
                    myReader = myCommand.ExecuteReader();

                    myReader.Close();
                } 
                catch (Exception ex) 
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void SaveCustomer(Customer customer)
        {
            myCommand.CommandText = string.Format("Update Customer SET firstName='{0}',lastname='{1}',streetNo='{2}',streetName='{3}',aptNo='{4}',city='{5}',postalCode='{6}',phoneNumber='{7}',email='{8}',creditCard='{9}',rating='{10}' where accountNo='{11}'",
                    customer.FirstName, customer.LastName, customer.StreetNo, customer.StreetName, customer.AptNo, customer.City, customer.PostalCode, customer.PhoneNumber, customer.Email, customer.CreditCard, customer.Rating, customer.AccountNo);

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Customer GetCustomerByAccountNumber(int accountNo)
        {
            Customer customer = null;

            myCommand.CommandText = "select * from Customer where accountNo = " + accountNo;

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Read();

                CustomerStruct customerStruct = new(accountNo);

                customerStruct.lastName = myReader[(int)CustomerEnum.lastName].ToString();
                customerStruct.firstName = myReader[(int)CustomerEnum.lastName].ToString();
                customerStruct.phoneNumber = myReader[(int)CustomerEnum.phoneNumber].ToString();
                customerStruct.email = myReader[(int)CustomerEnum.email].ToString();
                customerStruct.streetNo = myReader[(int)CustomerEnum.streetNo].ToString();
                customerStruct.streetName = myReader[(int)CustomerEnum.streetName].ToString();
                customerStruct.aptNo = myReader[(int)CustomerEnum.aptNo].ToString();
                customerStruct.city = myReader[(int)CustomerEnum.city].ToString();
                customerStruct.creditCard = myReader[(int)CustomerEnum.creditCard].ToString();
                customerStruct.rating = myReader[(int)CustomerEnum.rating].ToString();

                customer = new Customer(customerStruct);

                myReader.Close();
            } 
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }

            return customer;
        }


        public void RunCustomQuery(string query)
        {
            myCommand.CommandText = query;

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GetLowestAvailableCopyID(string movieID)
        {
            myCommand.CommandText =
            "select\r\n" +
            "case \r\n" +
            "when exists (select copyID from copies where movieID = " + movieID + ") then\r\n" +
            "(select MIN(copyID) + 1 as lowestAvailableCopyID \r\n" +
            "from copies C1\r\n" +
            "where C1.copyID + 1 not in (select C2.copyID from copies C2))\r\n" +
            "else\r\n" +
            "1\r\n" +
            "end as lowestAvailableCopyID\r\n" +
            "from Movies";

            try
            {
                myReader = myCommand.ExecuteReader();
                if (myReader.Read())
                {
                    int copyID = 1;

                    int.TryParse(myReader["lowestAvailableCopyID"].ToString(), out copyID);

                    myReader.Close();

                    return copyID;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return 1;
        }

        public int GetLowestAvailableMovieID()
        {
            myCommand.CommandText =
            "select\r\n" +
            "case \r\n" +
            "when exists (select movieID from Movies where movieID = 1) then\r\n" +
            "(select MIN(movieID) + 1 as lowestAvailableID \r\n" +
            "from Movies M1\r\n" +
            "where M1.movieID + 1 not in (select M2.movieID from Movies M2))\r\n" +
            "else\r\n" +
            "1\r\n" +
            "end as lowestAvailableID\r\n" +
            "from Movies";

            try
            {
                myReader = myCommand.ExecuteReader();
                if (myReader.Read())
                {
                    int movieID = 1;

                    int.TryParse(myReader["lowestAvailableID"].ToString(), out movieID);

                    myReader.Close();

                    return movieID;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return 1;
        }

        public int GetLowestAvailableAccountNumber()
        {
            myCommand.CommandText =
            "select distinct\r\n" +
            "case\r\n" +
            "when exists (select accountNo from Customer where accountNo = 1) then\r\n" +
            "(select MIN(accountNo) + 1 as lowestAvailableAccountNo\r\n" +
            "from Customer C1\r\n" +
            "where C1.accountNo + 1 not in (select C2.accountNo from Customer C2))\r\n" +
            "else\r\n" +
            "1\r\n" +
            "end as lowestAvailableAccountNo\r\n" +
            "from Customer";

            try
            {
                myReader = myCommand.ExecuteReader();
                if(myReader.Read())
                {
                    int accountNumber = 1;

                    int.TryParse(myReader["lowestAvailableAccountNo"].ToString(), out accountNumber);

                    myReader.Close();

                    return accountNumber;
                }
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return 1;

        }

        public int GetLowestAvailableEmployeeID()
        {
            myCommand.CommandText =
            "select distinct\r\n" +
            "case\r\n" +
            "when exists (select employeeID from Employees where employeeID = 1) then\r\n" +
            "(select MIN(employeeID) + 1 as lowestAvailableEmployeeID\r\n" +
            "from Employees E1\r\n" +
            "where E1.employeeID + 1 not in (select E2.employeeID from Customer E2))\r\n" +
            "else\r\n" +
            "1\r\n" +
            "end as lowestAvailableEmployeeID\r\n" +
            "from Employees";

            try
            {
                myReader = myCommand.ExecuteReader();
                if (myReader.Read())
                {
                    int employeeID = 1;

                    int.TryParse(myReader["lowestAvailableAccountNo"].ToString(), out employeeID);

                    myReader.Close();

                    return employeeID;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddCustomer(Customer customer)
        {
            myCommand.CommandText = string.Format("insert into Customer values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');",
                customer.AccountNo, customer.LastName, customer.FirstName, customer.StreetNo, customer.StreetName, customer.AptNo, customer.City, customer.PostalCode, customer.PhoneNumber, customer.Email, customer.CreditCard, customer.Rating);

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Close();
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int[] getMovieCopies(string movieName)
        {
            
            myCommand.CommandText = "SELECT copyID FROM copy WHERE movieID IN (SELECT movieID FROM Movies WHERE movieName ='" + movieName + "')";

            var copies = new List<int>();

            try
            {
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    int cID = myReader.GetInt32(myReader.GetOrdinal("copyID"));
                    copies.Add(cID);
                }

                myReader.Close();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message); 
            }

            return copies.ToArray();

        }

        public int[] getBookedCopies(int movieID, DateTime fromDate, DateTime toDate)
        {
            var availableCopies = new List<int>();
            try
            {   
                //get all cases where toDate and fromDate lie in the interval of fromDate and toDate of existing orders.  **TODO Need Edge Cases** from < minfrom to > toMax  
                myCommand.CommandText = "SELECT copyID FROM \"Order\" WHERE movieID = '" + movieID + "' AND (('" + fromDate + "'>= fromDate AND '" + fromDate + "' <= toDate) OR ('" + toDate + "' >=  fromDate AND '" + toDate + "' <= toDate))";
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    int cID = myReader.GetInt32(myReader.GetOrdinal("copyID"));
                    availableCopies.Add(cID);
                }
                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }



            return availableCopies.ToArray();
        }


        public void insertBooking(int movieID, int copyID, DateTime fromDate, DateTime toDate)
        {
            myCommand.CommandText = "INSERT INTO orders ()";
        }

        public int convertMovieTitleToID(string movieName)
        {
            var movieID = new int();

            try
            {
                myCommand.CommandText = "SELECT movieID FROM Movies WHERE movieName = '" + movieName + "'";
                myReader = myCommand.ExecuteReader();


                while (myReader.Read())
                {
                    movieID = myReader.GetInt32(myReader.GetOrdinal("movieID"));
                }

                myReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return movieID;
        }


        public void AddOrder(Order order)
        {
            myCommand.CommandText = string.Format("insert into \"Order\" values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
                order.OrderID, order.Date, order.Status, order.FromDate, order.ToDate, order.EmployeeID, order.CopyID, order.MovieID, order.AccountNo);

            try
            {
                myReader = myCommand.ExecuteReader();

                myReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GetLatestOrderNumber()
        {
            myCommand.CommandText = "select MAX(orderID) as latestOrder from \"Order\"";
            try
            {
                myReader = myCommand.ExecuteReader();
                if (myReader.Read())
                {
                    int orderNumber = 0;

                    int.TryParse(myReader["latestOrder"].ToString(), out orderNumber);

                    myReader.Close();

                    return orderNumber;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
