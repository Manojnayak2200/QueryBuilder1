using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QueryBuilder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace QueryBuilder.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
     
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult bhul()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public ViewResult QueryBuilder()
        {
            ViewBag.data = 0;
            return View();
        }
        [HttpPost]
        public IActionResult QueryBuilder(Query query)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp;

            try
            {
                SqlConnection con = new SqlConnection("Data Source=CSMBHUL562\\SQLEXPRESS; Database=Gosmile;Trusted_Connection=True;MultipleActiveResultSets=true");

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
               
                if (query.QueryText != null)
                {

                    

                    bool checkdmlop = checkDML(query.QueryText);
                    if (checkdmlop == true)
                    {
                        if (query.pass != null)
                        {
                            int passiscurrect = CheckPass(query.pass);
                            if (passiscurrect == 1)
                            {

                            }
                            else
                            {
                                ViewBag.exce = "Invaliad Password";
                                return View(query);
                            }
                        }
                        else
                        {
                            ViewBag.exce = "Enter Password";
                            return View(query);
                        }

                        SqlCommand cmd = new SqlCommand(query.QueryText, con);
                        int cout = Convert.ToInt32(cmd.ExecuteNonQuery());

                        ViewBag.data = cout;

                        //PartialView("PView", dt);

                        ViewBag.list = dt;

                        return View(query);


                    }
                    else
                    {
                        adp = new SqlDataAdapter(query.QueryText, con);
                        adp.Fill(dt);
                        //PartialView("PView", dt);
                        ViewBag.list = dt;
                        return View(query);
                    }
                }
                else
                {
                    ViewBag.exce = "Enter Query";
                }

                } catch (Exception ex)
            {

                ViewBag.exce = "Invalid Query";
            }

            return View();
        
        }

        public bool checkDML(string query)
        {

            bool IsDML;
            if (query.ToLower().StartsWith("update ") ||
                query.ToLower().StartsWith("insert ") ||
                query.ToLower().StartsWith("create ") ||
                query.ToLower().StartsWith("alter ") ||
                query.ToLower().StartsWith("delete ") ||
                query.ToLower().StartsWith("truncate ") ||
                query.ToLower().StartsWith("drop ") ||
                query.ToLower().Contains("procedure") ||
                query.ToLower().Contains("update ") ||
                query.ToLower().Contains("insert ") ||
                query.ToLower().Contains("create ") ||
                query.ToLower().Contains("alter ") ||
                query.ToLower().Contains("delete ") ||
                query.ToLower().Contains("truncate ") ||
                query.ToLower().Contains("drop ")
                )
            {
                IsDML = true;
            }
            else
            {
                IsDML = false;
            }

            return IsDML;


        }

        public int CheckPass(string Password)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=CSMBHUL562\\SQLEXPRESS; Database=Gosmile;Trusted_Connection=True;MultipleActiveResultSets=true");

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                DataTable dt = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter("Select * from QueryBuilderPassword where pass='" + Password + "'", con);
                adp.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            catch(Exception ex)
            {
                return 404;
            }
        }
    }
}
