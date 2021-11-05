using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContactSalesOrder.Models;
using System.Data.SqlClient;

namespace ContactSalesOrder.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<Customer> customers = new List<Customer>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            // Connecting server-side to database
            con.ConnectionString = "Server = koretechinterview.database.windows.net; Initial Catalog = KORESampleDatabase; Persist Security Info = False; User ID = koreinterview; Password = a8Xp6Pz6&$TR; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";
        }

        public IActionResult Index()
        {
            FetchData();
            return View(customers);
        }

        // This function fetches the data from the database and stores it in a Model which is then passed to the View
        private void FetchData()
        {
            // Clearing old customers list to prevent duplication.
            if(customers.Count > 0)
            {
                customers.Clear();
            }
            try
            {
                // Setup to run the query on the database.
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [LastName] , [FirstName], [Title], [MiddleName], [EmailAddress], [Phone], [ModifiedDate] " +
                    "FROM [SalesLT].[Customer] " +
                    "WHERE [EmailAddress] <> '' AND [Phone] <> '' AND ([FirstName] <> '' OR [LastName] <> '') " +
                    "ORDER BY [LastName], [FirstName]";
                dr = com.ExecuteReader();
                
                // Reading tuples that returned.
                while(dr.Read())
                {
                    // Converting date time of customer that is being checked
                    string dateString = dr["ModifiedDate"].ToString();
                    DateTime md = Convert.ToDateTime(dateString);
                    // To make sure if no duplicates are found, customer is added.
                    bool foundDuplicate = false;
                    for(int i = 0; i < customers.Count; i++)
                    {
                        // Checking if user's email is already stored in Customer List
                        string email = dr["EmailAddress"].ToString();
                        if(customers[i].Email == email)
                        {
                            // If it is stored, compare their Modified Dates
                            foundDuplicate = true;
                            DateTime inList = Convert.ToDateTime(customers[i].ModifiedDate);
                            int result = DateTime.Compare(inList, md);
                            // If incoming user has a later modified date, remove user being checked and
                            // add incoming user.
                            if(result <= 0)
                            {
                                customers.RemoveAt(i);
                                string nameField = CheckNames(dr["LastName"].ToString(), dr["FirstName"].ToString());
                                customers.Add(new Customer()
                                {
                                    Name = nameField,
                                    FullName = dr["Title"].ToString() + " " + dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString(),
                                    Email = dr["EmailAddress"].ToString(),
                                    Phone = dr["Phone"].ToString(),
                                    ModifiedDate = dr["ModifiedDate"].ToString()
                                });
                            } 
                            // Otherwise, that user should not be stored.
                            else
                            {
                                i = customers.Count;
                            }
                        }
                    }
                    // If email was not found in the list, add customer to it.
                    if (foundDuplicate == false)
                    {
                        customers.Add(new Customer()
                        {
                            Name = dr["LastName"].ToString() + ", " + dr["FirstName"].ToString(),
                            FullName = dr["Title"].ToString() + " " + dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString(),
                            Email = dr["EmailAddress"].ToString(),
                            Phone = dr["Phone"].ToString(),
                            ModifiedDate = dr["ModifiedDate"].ToString()
                        });
                    }
                    // Set duplicate back to false, so check would still work.
                    foundDuplicate = false;
                }
                //Console.WriteLine(customers.Count);
                con.Close();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private string CheckNames(string firstName, string lastName)
        {
            if(firstName == "")
            {
                return lastName;
            } else if(lastName == "")
            {
                return firstName;
            } else
            {
                return lastName + ", " + firstName;
            }
        }

        // I was still working on a search bar, keeping it just in case anyone wants to take a look
        /*
        private Task<IActionResult> SearchCustomers(string input)
        {
            List<Customer> searchedList;
            for(int i = 0; i < customers.Count; i++)
            {
                if(customers[i]["LastName"].Contains(input))
                {

                }
            }
            return customers;
        }
        */
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
