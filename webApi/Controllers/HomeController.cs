using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using webApi;
using webApi.classes;
using webApi.Models;

namespace webExample.Controllers
{
    public class HomeController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public SqlConnection conn = new SqlConnection();
        //public static string connectionString = @"Data Source=.;Initial Catalog=Congress;Integrated Security=True;MultipleActiveResultSets=True";
        public static string connectionString = @"Data Source=PRISMAOLAP\REZEF;Initial Catalog=Congress;Integrated Security=True;MultipleActiveResultSets=True";

        //public static string LoginUserName = "";
        //public static string Division;
        // CongressEntities3 db = new CongressEntities3();
        //[Route("api/Home/getUserNameExists")]
        //[HttpGet]
        //public Boolean getUserNameExists()
        //{
        //    if (LoginUserName == "")
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        [Route("api/Home/tranzilaSuccess")]
        [HttpPost]
        public async Task<IHttpActionResult> tranzilaSuccess()
        {
            CatchExeption("success");

            var form = await Request.Content.ReadAsFormDataAsync();
            var queryString = string.Empty;
            if (form != null)
            {
                queryString = $"?userName={form["email"]}&response={form["Response"]}";
            }
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            CatchExeption(baseUrl + "/success");

            return Redirect(baseUrl + "/success");
        }


        //[Route("api/Home/tranzilaNotify")]
        //[HttpPost]
        //public async Task<IHttpActionResult> tranzilaNotify()
        //{
        //    var form = await Request.Content.ReadAsFormDataAsync();
        //    var queryString = string.Empty;
        //    if (form != null)
        //    {
        //        queryString = $"?userName={form["email"]}&response={form["Response"]}&sum={form["sum"]}&currency={form["currency"]}";
        //    }
        //    string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
        //    return Redirect(baseUrl + "/notify");
        //}‏

        [Route("api/Home/tranzilaNotify")]
        [HttpPost]
        public async Task<IHttpActionResult> tranzilaNotify()
        {
            var form = await Request.Content.ReadAsFormDataAsync();
            var queryString = string.Empty;
            if (form != null)
            {
                queryString = $"?userName={form["email"]}&response={form["Response"]}&sum={form["sum"]}&currency={form["currency"]}";
            }
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return Redirect(baseUrl + "/notify");
        }

        [Route("api/Home/tranzilaFail")]
        [HttpPost]
        public async Task<IHttpActionResult> tranzilaFail()
        {
            var form = await Request.Content.ReadAsFormDataAsync();
            var queryString = string.Empty;
            if (form != null)
            {
                queryString = $"?userName={form["email"]}&response={form["Response"]}";
            }
            string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            return Redirect(baseUrl + "/fail" + queryString);
        }


        [Route("api/Home/CatchExeption")]
        [HttpGet]
        public void CatchExeption(string ex)
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();

            int IdLog = GetLastIdLog() + 1;
            string sqlText = "Insert into Log (Id,Exeption,Date) values(@Id,@Exeption,@Date)";
            sqlCmd = new SqlCommand(sqlText, myConnection);
            sqlCmd.Parameters.AddWithValue("@Id", IdLog);
            sqlCmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("M/d/yyyy"));
            sqlCmd.Parameters.AddWithValue("@Exeption", ex);
            myConnection.Close();
            myConnection.Open();
            int ii = sqlCmd.ExecuteNonQuery();
            myConnection.Close();
        }

        [Route("api/Home/GetLastIdLog")]
        [HttpGet]
        public int GetLastIdLog()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "select top(1) Id from Log ORDER BY Id DESC";
            sqlCmd.Connection = myConnection;
            myConnection.Close();
            try
            {
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                myConnection.Close();
                return 1;
            }
            return 1;
        }

        [Route("api/Home/getFromDB")]
        [HttpGet]
        // קבלת כל הספרים הנמצאים בדאטה 
        public List<webApi.classes.Book> getFromDB()
        {

            List<webApi.classes.Book> dict = new List<webApi.classes.Book>();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM Books";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();


                while (reader.Read())
                {
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    var c = reader.GetString(2);
                    var e = reader.GetDouble(3);
                    var d = reader.GetString(4);
                    var currencyCode = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                    var _PriceUSD = reader.IsDBNull(9) ? 0 : reader.GetDouble(9);
                    var _PriceILS = reader.IsDBNull(10) ? 0 : reader.GetDouble(10);
                    var _SalePriceUSD = reader.IsDBNull(11) ? 0 : reader.GetDouble(11);
                    var _SalePriceILS = reader.IsDBNull(12) ? 0 : reader.GetDouble(12);

                    double f = 000;
                    f = reader.IsDBNull(5) ? 0 : reader.GetDouble(5);
                    var g = reader.GetInt32(6);
                    bool h = reader.GetBoolean(7);
                    //Image d;
                    var r = new webApi.classes.Book { SalePriceUSD = _SalePriceUSD, SalePriceILS = _SalePriceILS, PriceUSD = _PriceUSD, PriceILS = _PriceILS, Id = a, Name = b, Details = c, Price = (int)e, Image = d, SallePrice = (int)f, GroupBook = g, Available = h‏, Currency = currencyCode };
                    //var d;
                    //Image d;
                    dict.Add(r);//,d);
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
            return dict;


        }


        [Route("api/Home/getFromDBHebrew")]
        [HttpGet]
        // קבלת כל הספרים הנמצאים בדאטה 
        public List<webApi.classes.Book> getFromDBHebrew()
        {
            Logger.Info("enter getFromDBHebrew");
            List<webApi.classes.Book> dict = new List<webApi.classes.Book>();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            Logger.Info("connectionString: " + myConnection.ConnectionString);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM BooksHebrew";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();


                while (reader.Read())
                {
                    var a = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    var b = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var c = reader.IsDBNull(2) ? null : reader.GetString(2);
                    var e = reader.IsDBNull(3) ? 0 : reader.GetDouble(3);
                    var d = reader.IsDBNull(4) ? null : reader.GetString(4);
                    var currencyCode = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                    var _PriceUSD = reader.IsDBNull(9) ? 0 : reader.GetDouble(9);
                    var _PriceILS = reader.IsDBNull(10) ? 0 : reader.GetDouble(10);
                    var _salePriceUSD = reader.IsDBNull(11) ? 0 : reader.GetDouble(11);
                    var _salePriceILS = reader.IsDBNull(12) ? 0 : reader.GetDouble(12);

                    double f = 000;
                    f = reader.IsDBNull(5) ? 0 : reader.GetDouble(5);
                    var g = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);

                    bool h = reader.IsDBNull(7) ? false : reader.GetBoolean(7);
                    //Image d;
                    var r = new webApi.classes.Book { SalePriceILS = _salePriceILS, SalePriceUSD = _salePriceUSD, PriceILS = _PriceILS, PriceUSD = _PriceUSD, Id = a, Name = b, Details = c, Price = (int)e, Image = d, SallePrice = (int)f, GroupBook = g, Available = h‏, Currency = currencyCode };
                    //var d;
                    //Image d;
                    dict.Add(r);//,d);
                }

                myConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("getFromDBHebrew failed dict: " + dict);
                Logger.Error("getFromDBHebrew failed ex: " + ex.ToString());
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return dict;

        }
        [Route("api/Home/getFromDBCart")]
        [HttpGet]
        // בינתיים ללא סינון על שם משתמש
        public List<ShoppingCart> getFromDBCart(string LoginUserName)
        {

            List<ShoppingCart> dict = new List<ShoppingCart>();

            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM ShoppingCart  where UserName ='" + LoginUserName + "' ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    var c = reader.GetInt32(2);
                    //var e = rdr.GetDouble(3);
                    var d = reader.GetString(3);
                    var e = reader.GetDouble(4);
                    var f = reader.GetString(5);
                    var g = reader.GetInt32(6);
                    var currencyCode = reader.GetInt32(10);
                    var _PriceUSD = reader.IsDBNull(11) ? 0 : reader.GetDouble(11);
                    var _PriceILS = reader.IsDBNull(12) ? 0 : reader.GetDouble(12);
                    var _SalePriceUSD = reader.IsDBNull(13) ? 0 : reader.GetDouble(13);
                    var _SalePriceILS = reader.IsDBNull(14) ? 0 : reader.GetDouble(14);
                    double ff = 000;
                    ff = reader.IsDBNull(7) ? 0 : reader.GetDouble(7);

                    //Image d;
                    var r = new ShoppingCart { SalePriceILS = _SalePriceILS, SalePriceUSD = _SalePriceUSD, PriceUSD = _PriceUSD, PriceILS = _PriceILS, Id = a, UserName = b, NameBook = d, IdBook = c, PriceBook = e, ImageBook = f, Quantity = g, SallePrice = (int)ff };
                    //var d;
                    //Image d;
                    dict.Add(r);//,d);

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return dict;

        }

        [Route("api/Home/getPriceShoppCart")]
        [HttpGet]
        // בינתיים ללא סינון על שם משתמש
        public CartPriceResponse getPriceShoppCart(string LoginUserName)
        {

            double count = 0;
            double sumUSD = 0;
            double sumILS = 0;

            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM ShoppingCart  where UserName ='" + LoginUserName + "' ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    var c = reader.GetInt32(2);
                    //var e = rdr.GetDouble(3);
                    var d = reader.GetString(3);
                    var e = reader.GetDouble(4);
                    var f = reader.GetString(5);
                    var g = reader.GetInt32(6);
                    var _PriceUSD = reader.IsDBNull(8) ? 0 : reader.GetDouble(8);
                    var _PriceILS = reader.IsDBNull(9) ? 0 : reader.GetDouble(9);
                    var _SalePriceUSD = reader.IsDBNull(10) ? 0 : reader.GetDouble(10);
                    var _SalePriceILS = reader.IsDBNull(11) ? 0 : reader.GetDouble(11);
                    double ff = 0;
                    ff = reader.IsDBNull(7) ? 0 : reader.GetDouble(7);

                    //Image d;
                    var r = new ShoppingCart { SalePriceILS = _SalePriceILS, SalePriceUSD = _SalePriceUSD, PriceILS = _PriceILS, PriceUSD = _PriceUSD, Id = a, UserName = b, NameBook = d, IdBook = c, PriceBook = e, ImageBook = f, Quantity = g, SallePrice = (int)ff };
                    //var d;
                    //Image d;
                    if (ff == 0)
                    {
                        count += Math.Round(r.PriceBook * r.Quantity);
                        sumUSD += Math.Round(r.PriceUSD * r.Quantity);
                        sumILS += Math.Round(r.PriceILS * r.Quantity);
                    }
                    else
                    {
                        count += Math.Round(r.SallePrice * r.Quantity, 2);
                        sumUSD += Math.Round(r.SalePriceUSD * r.Quantity, 2);
                        sumILS += Math.Round(r.SalePriceILS * r.Quantity, 2);

                    }

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return new CartPriceResponse(count, sumUSD, sumILS);

        }

        [Route("api/Home/GetLastId")]
        [HttpGet]
        //לקבל את המזהה האחרון שיש
        public int GetLastId()
        {

            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from ShoppingCart ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;

        }

        [Route("api/Home/GetLastIdSession")]
        [HttpGet]
        //לקבל את המזהה האחרון שיש
        public int GetLastIdSession()
        {

            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from Sessions ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;

        }


        [Route("api/Home/GetLastIdUser")]
        [HttpGet]
        public int GetLastIdUser()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from Users ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;

        }

        [Route("api/Home/GetLastIdUserPass")]
        [HttpGet]
        public int GetLastIdUserPass()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from UserPeass ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;

        }

        [Route("api/Home/GetLastIdDraft")]
        [HttpGet]
        public int GetLastIdDraft()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from ProposalDrafts ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }

        [Route("api/Home/GetLastIdDraftSecond")]
        [HttpGet]
        public int GetLastIdDraftSecond(string LoginUser)
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from ProposalDrafts  where UserName LIKE N'" + LoginUser + "%' ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }




        [Route("api/Home/GetLastIdJudges")]
        [HttpGet]
        public int GetLastIdJudges()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from Judges ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }
        [Route("api/Home/GetLastIdProposal")]
        [HttpGet]
        public int GetLastIdProposal()
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from Proposal ORDER BY Id DESC";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }

        [Route("api/Home/GetBookById")]
        [HttpGet]
        public webApi.classes.Book GetBookById(int Id)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select  * from Books where Id =" + Id;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {

                    var a = rdr.GetInt32(0);
                    var b = rdr.GetString(1);
                    var c = rdr.GetString(2);
                    var e = rdr.GetDouble(3);
                    var d = rdr.GetString(4);
                    var currencyCode = rdr.GetInt32(8);
                    var _PriceUSD = rdr.IsDBNull(9) ? 0 : rdr.GetDouble(9);
                    var _PriceILS = rdr.IsDBNull(10) ? 0 : rdr.GetDouble(10);
                    var _SalePriceUSD = rdr.IsDBNull(11) ? 0 : rdr.GetDouble(11);
                    var _SalePriceILS = rdr.IsDBNull(12) ? 0 : rdr.GetDouble(12);
                    double f = 0;
                    f = rdr.IsDBNull(5) ? 0 : rdr.GetDouble(5);
                    var g = rdr.GetInt32(6);
                    bool h = rdr.GetBoolean(7);

                    //Image d;
                    var r = new webApi.classes.Book {SalePriceILS=_SalePriceILS,SalePriceUSD=_SalePriceUSD, PriceILS = _PriceILS, PriceUSD = _PriceUSD, Id = a, Name = b, Details = c, Price = (int)e, Image = d, SallePrice = (int)f, GroupBook = g, Available = h‏, Currency = currencyCode };
                    myConnection.Close();
                    return r;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return new webApi.classes.Book { };

        }

        [Route("api/Home/GetBookByIdHebrew")]
        [HttpGet]
        public webApi.classes.Book GetBookByIdHebrew(int Id)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select  * from BooksHebrew where Id =" + Id;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {

                    var a = rdr.GetInt32(0);
                    var b = rdr.GetString(1);
                    var c = rdr.GetString(2);
                    var e = rdr.GetDouble(3);
                    var d = rdr.GetString(4);
                    var currencyCode = rdr.GetInt32(8);
                    var _PriceUSD = rdr.IsDBNull(9) ? 0 : rdr.GetDouble(9);
                    var _PriceILS = rdr.IsDBNull(10) ? 0 : rdr.GetDouble(10);
                    var _SalePriceUSD = rdr.IsDBNull(11) ? 0 : rdr.GetDouble(11);
                    var _SalePriceILS = rdr.IsDBNull(12) ? 0 : rdr.GetDouble(12);
                    double f = 0;
                    f = rdr.IsDBNull(5) ? 0 : rdr.GetDouble(5);
                    var g = rdr.GetInt32(6);
                    bool h = rdr.GetBoolean(7);
                    //Image d;
                    var r = new webApi.classes.Book { SalePriceILS = _SalePriceILS, SalePriceUSD = _SalePriceUSD, PriceUSD = _PriceUSD, PriceILS = _PriceILS, Id = a, Name = b, Details = c, Price = (int)e, Image = d, Currency = currencyCode, SallePrice = (int)f, GroupBook = g, Available = h‏ };
                    myConnection.Close();
                    return r;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return new webApi.classes.Book { };

        }

        [Route("api/Home/AddToCart")]
        [HttpPost]
        // הכנסת ספר לסל הקניות
        public int AddToCart(webApi.classes.CartWithAddress item)
        {

            // SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            // int ID = 0;
            SqlCommand sqlCmd = new SqlCommand();
            //  sqlCmd.CommandType = CommandType.Text;
            try
            {
                //sqlCmd.CommandText = "select Id from  ShoppingCart where IdBook=" + item.Id + " and NameBook='" + item.UserName + "'";
                //sqlCmd.Connection = myConnection;
                //myConnection.Open();
                //reader = sqlCmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    ID = Convert.ToInt32(reader.GetValue(0));

                //}
                //myConnection.Close();

                //if (ID == 0)
                //{
                //    int Id = GetLastId() + 1;
                //    conn.ConnectionString = connectionString;
                //    string sqlText = "Insert into Cart values(@Id,'" + item.UserName + "',@IdBook,@NameBook,@PriceBook,@ImageBook,@Quantity,@SallePrice)";
                //    sqlCmd = new SqlCommand(sqlText, conn);
                //    sqlCmd.Parameters.AddWithValue("@Id", Id);
                //    sqlCmd.Parameters.AddWithValue("@IdBook", item.IdBook);
                //    sqlCmd.Parameters.AddWithValue("@NameBook", item.NameBook);
                //    sqlCmd.Parameters.AddWithValue("@ImageBook", item.ImageBook);
                //    sqlCmd.Parameters.AddWithValue("@PriceBook", item.PriceBook);
                //    sqlCmd.Parameters.AddWithValue("@SallePrice", item.SallePrice);
                //    sqlCmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                //    conn.Open();
                //    int i = sqlCmd.ExecuteNonQuery();
                //    conn.Close();

                //}
                //else
                //{
                // var Quantity = 0;
                myConnection.ConnectionString = connectionString;
                sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                int Id = GetLastId() + 1;
                conn.ConnectionString = connectionString;//SallePrice                           @DetailsBook
                string sqlText = "Insert into Cart values('" + item.UserName + "'@IdBook,@NameBook,@DetailsBook,@PriceBook,@ImageBook,@Quantity,@SallePrice,@Address,@Currency,@PriceUSD,@PriceILS,@SalePriceUSD,@SalePriceILS)";
                sqlCmd = new SqlCommand(sqlText, conn);
                //sqlCmd.Parameters.AddWithValue("@Id", Id);
                sqlCmd.Parameters.AddWithValue("@IdBook", item.IdBook);
                sqlCmd.Parameters.AddWithValue("@NameBook", item.NameBook);
                sqlCmd.Parameters.AddWithValue("@DetailsBook", "DetailsBook");
                sqlCmd.Parameters.AddWithValue("@PriceBook", item.PriceBook);
                sqlCmd.Parameters.AddWithValue("@ImageBook", item.ImageBook);
                sqlCmd.Parameters.AddWithValue("@SallePrice", item.SallePrice);
                sqlCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                sqlCmd.Parameters.AddWithValue("@Address", item.Address);
                sqlCmd.Parameters.AddWithValue("@Currency", item.Currency);
                sqlCmd.Parameters.AddWithValue("@SalePriceUSD", item.SalePriceUSD);
                sqlCmd.Parameters.AddWithValue("@SalePriceILS", item.SalePriceILS);
                sqlCmd.Parameters.AddWithValue("@PriceUSD", item.PriceUSD);
                sqlCmd.Parameters.AddWithValue("@PriceILS", item.PriceILS);


                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();

                //sqlCmd.CommandText = "select Quantity from  Cart where IdBook=" + item.IdBook + " and UserName='" + item.UserName + "'";
                //sqlCmd.Connection = myConnection;
                //myConnection.Open();
                //reader = sqlCmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    Quantity = Convert.ToInt32(reader.GetValue(0));

                //}
                //myConnection.Close();
                //Quantity = Quantity + item.Quantity;

                //conn.ConnectionString = connectionString;
                //string sqlText = "update  Cart set Quantity=" + (int)Quantity + " where IdBook=" + item.IdBook + " and UserName='" + item.UserName + "'";
                //sqlCmd = new SqlCommand(sqlText, conn);

                //conn.Open();
                //int i = sqlCmd.ExecuteNonQuery();
                //conn.Close();
                // }
            }

            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }

        [Route("api/Home/PostToCart")]
        [HttpPost]
        // הכנסת ספר לסל הקניות
        public int PostToCart(webApi.classes.ShoppingCart item)
        {

            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            int ID = 0;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select Id from  ShoppingCart where IdBook=" + item.Id + " and UserName='" + item.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));

                }
                myConnection.Close();

                if (ID == 0)
                {
                    int Id = GetLastId() + 1;
                    conn.ConnectionString = connectionString;
                    string sqlText = "Insert into ShoppingCart values(@Id,'" + item.UserName + "'@SalePriceUSD,@SalePriceILS,@SalePriceUSD,@SalePriceUSD,@IdBook,@Name,@Price,@Image,@Quantity,@SallePrice,@Currency,@PriceUSD,@PriceILS,@SalePriceUSD,@SalePriceILS)";
                    sqlCmd = new SqlCommand(sqlText, conn);
                    sqlCmd.Parameters.AddWithValue("@Id", Id);
                    sqlCmd.Parameters.AddWithValue("@IdBook", item.IdBook);
                    sqlCmd.Parameters.AddWithValue("@Name", item.NameBook);
                    sqlCmd.Parameters.AddWithValue("@Image", item.ImageBook);
                    sqlCmd.Parameters.AddWithValue("@Price", item.PriceBook);
                    sqlCmd.Parameters.AddWithValue("@SallePrice", item.SallePrice);
                    sqlCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    sqlCmd.Parameters.AddWithValue("@Currency", item.Currency);
                    sqlCmd.Parameters.AddWithValue("@SalePriceUSD", item.SalePriceUSD);
                    sqlCmd.Parameters.AddWithValue("@SalePriceILS", item.SalePriceILS);
                    sqlCmd.Parameters.AddWithValue("@PriceUSD", item.PriceUSD);
                    sqlCmd.Parameters.AddWithValue("@PriceILS", item.PriceILS);

                    conn.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    conn.Close();

                }
                else
                {
                    var Quantity = 0;
                    myConnection.ConnectionString = connectionString;
                    sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "select Quantity from  ShoppingCart where IdBook=" + item.IdBook + " and UserName='" + item.UserName + "'";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Quantity = Convert.ToInt32(reader.GetValue(0));

                    }
                    myConnection.Close();
                    Quantity = Quantity + item.Quantity;

                    conn.ConnectionString = connectionString;
                    string sqlText = "update  ShoppingCart set Quantity=" + (int)Quantity + " where IdBook=" + item.IdBook + " and UserName='" + item.UserName + "'";
                    sqlCmd = new SqlCommand(sqlText, conn);

                    conn.Open();
                    int i = sqlCmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;
        }
        [Route("api/Home/postAddQuantity")]

        [HttpPost]
        public void postAddQuantity(ShoppingCart item)
        {
            conn.ConnectionString = connectionString;
            try
            {
                string sqlText = "Update ShoppingCart set Quantity = @Quantity where Id = @Id";
                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", item.Id);
                sqlCmd.Parameters.AddWithValue("@Quantity", (item.Quantity + 1));


                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                //db.SaveChanges();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }

        [Route("api/Home/postRemoveQuantity")]
        [HttpPost]
        public void postRemoveQuantity(ShoppingCart item)
        {
            conn.ConnectionString = connectionString;
            string sqlText;
            try
            {
                sqlText = "Delete From ShoppingCart  where Id = @Id";


                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", item.Id);
                sqlCmd.Parameters.AddWithValue("@Quantity", item.Quantity - 1);


                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }
        [Route("api/Home/postdeleteQuantity")]
        [HttpPost]
        public void postdeleteQuantity(ShoppingCart item)
        {
            conn.ConnectionString = connectionString;
            try
            {
                string sqlText;
                if (item.Quantity == 1)
                    sqlText = "Delete From ShoppingCart  where Id = @Id";
                else
                    sqlText = "Update ShoppingCart set Quantity = @Quantity where Id = @Id";



                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", item.Id);
                sqlCmd.Parameters.AddWithValue("@Quantity", item.Quantity - 1);


                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }


        [Route("api/Home/GetIdMember")]
        [HttpGet]
        public int GetIdMember(string Email)
        {

            try
            {
                CatchExeption("line 947 came to function");

                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                int ID = 0;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "select Id from Users where Email like N'%" + Email + "%'";

                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    return ID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 1;
        }






        [Route("api/Home/RegistrationGuest")]
        [HttpPost]
        public void RegistrationGuest(webApi.classes.User user)
        {
            try
            {
                CatchExeption("line 947 came to function");

                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                int ID = 0;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "select Id from Users where Email like N'%" + user.Email + "%'";

                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));

                }
                CatchExeption("line 966");

                myConnection.Close();
                string sqlText = "";
                myConnection.ConnectionString = connectionString;



                if (ID != 0)
                {
                    sqlText = "Update Users set FirstNameEnglish=@FirstNameEnglish , LastNameEnglish=@LastNameEnglish,";
                    sqlText = sqlText + " FirstNameHebrew=@FirstNameHebrew , LastNameHebrew=@LastNameHebrew,";
                    sqlText = sqlText + " City=@City , Street=@Street , NumberHome=@NumberHome , NumberPhone1=@NumberPhone1,";
                    sqlText = sqlText + " NumberPhone2=@NumberPhone2 , selectedCountry=@selectedCountry,";
                    sqlText = sqlText + " selectedTitle=@selectedTitle , PostCode=@PostCode,";
                    sqlText = sqlText + " Email=@Email , Bio=@Bio , Students=@Students , WithoutStudemt=@WithoutStudemt,";
                    sqlText = sqlText + " EAJS=@EAJS , AJS=@AJS , Hebrew=@Hebrew , English=@English ,  Both=@Both ,Address=@Address,MemberShip=@MemberShip, Language=@Language where Email like N'%" + user.Email + "%'";
                }
                bool sendMailAppend = false;
                int Id = GetLastIdUser() + 1;

                if (ID == 0)
                {
                    Console.WriteLine("insert to user");

                    webApi.classes.UserPass Eexists = new webApi.classes.UserPass();
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew"
                       + ",NumberPhone1,selectedCountry,selectedTitle,PostCode," +
                       "Email,Bio,Students,WithoutStudemt,EAJS,AJS,Hebrew,English,Both,Address,MemberShip,Language) " +
                       "values(@Id,@FirstNameEnglish,@LastNameEnglish,@FirstNameHebrew,@LastNameHebrew"
                       + ",@NumberPhone1,@selectedCountry,@selectedTitle,@PostCode," +
                       "@Email,@Bio,@Students,@WithoutStudemt,@EAJS,@AJS,@Hebrew,@English,@Both,@Address,@MemberShip,@Language)";
                }

                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", (object)Id ?? 0);
                sqlCmd.Parameters.AddWithValue("@FirstNameEnglish", (object)user.FirstNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameEnglish", (object)user.LastNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FirstNameHebrew", (object)user.FirstNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameHebrew", (object)user.LastNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@City", (object)user.City ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Street", (object)user.Street ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberHome", (object)user.NumberHome ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone1", (object)user.NumberPhone1 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone2", (object)user.NumberPhone2 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedCountry", (object)user.selectedCountry ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedTitle", (object)user.selectedTitle ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@PostCode", (object)user.PostCode ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Bio", (object)user.Bio ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Students", (object)user.Students ?? 0);
                sqlCmd.Parameters.AddWithValue("@WithoutStudemt", (object)user.WithoutStudemt ?? 0);
                sqlCmd.Parameters.AddWithValue("@EAJS", (object)user.EAJS ?? 0);
                sqlCmd.Parameters.AddWithValue("@AJS", (object)user.AJS ?? 0);
                sqlCmd.Parameters.AddWithValue("@Hebrew", (object)user.Hebrew ?? 0);
                sqlCmd.Parameters.AddWithValue("@English", (object)user.English ?? 0);
                sqlCmd.Parameters.AddWithValue("@Both", (object)user.Both ?? 0);
                sqlCmd.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@MemberShip", (object)user.MemberShip ?? 0);
                sqlCmd.Parameters.AddWithValue("@Language", (object)user.Language ?? DBNull.Value);

                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.ConnectionString = connectionString;

                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                CatchExeption("line 1026");

                var stringChars = new char[8];
                //if (ID == 0)
                //{
                //    int Id11;
                //    string sqlText11;
                //    Id11 = GetLastIdDraft() + 1;
                //    conn.ConnectionString = connectionString;
                //    sqlText11 = "Insert into ProposalDrafts (Id,UserName) values(@Id,@UserName)";
                //    SqlCommand sqlCmd1 = new SqlCommand(sqlText11, conn);
                //    sqlCmd1.Parameters.AddWithValue("@Id", Id11);
                //    sqlCmd1.Parameters.AddWithValue("@UserName", user.Email);
                //    conn.Close();

                //    conn.Open();
                //    int z = sqlCmd1.ExecuteNonQuery();
                //    conn.Close();
                //}
                int checkExists = checkEmailFind(user.Email);
                if (checkExists == 1)
                {

                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var random = new Random();

                    for (int j = 0; j < stringChars.Length; j++)
                    {
                        stringChars[j] = chars[random.Next(chars.Length)];
                    }
                    Id = GetLastIdUserPass() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                    SqlCommand sqlCmd1 = new SqlCommand(sqlText, conn);
                    sqlCmd1.Parameters.AddWithValue("@Id", Id);
                    // sqlCmd1.Parameters.AddWithValue("@UserName", user.UserName);
                    sqlCmd1.Parameters.AddWithValue("@Password", new String(stringChars));
                    sqlCmd1.Parameters.AddWithValue("@Email", user.Email);


                    // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                    conn.Open();
                    i = sqlCmd1.ExecuteNonQuery();
                    conn.Close();
                    try
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            if (user.Language == "Hebrew")
                            {
                                CatchExeption("line 1075");
                                mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                mail.To.Add(user.Email);
                                mail.Subject = "יוזר חדש";
                                var loginUrl = "";

                                loginUrl = "http://jewish-studies.b2story.com/NewMemberAccount/1";


                                mail.Body = "שלום, " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew;
                                mail.Body = mail.Body + "<br/> תודה שביקרת בחנות הספרים של האיגוד העולמי למדעי היהדות.  ";
                                mail.Body = mail.Body + "<br/> אנו מזמינים אותך להצטרף לחברי האיגוד ולהנות מהנחות לספרים ועותק חינם של כתב העת מדעי היהדות. ";
                                mail.Body = mail.Body + "<br/> 2020 אם טרם הסדרת את תשלום דמי החבר שלך לשנת  ";
                                mail.Body = mail.Body + string.Format("ניתן לשלם .<a href='{0}'>בקישור</a>", loginUrl);
                                mail.Body = mail.Body + "<br/> שם משתמש: " + user.Email;
                                mail.Body = mail.Body + "<br/>סיסמה: " + new String(stringChars);
                                mail.Body = mail.Body + "<br/><br/>  לכל שאלה ניתן לפנות אלינו";
                                mail.Body = mail.Body + "<br/><br/> בברכה,";
                                mail.Body = mail.Body + "<br/><br/> צוות האיגוד";
                                mail.IsBodyHtml = true;
                            }
                            else
                            {
                                CatchExeption("line 1075");
                                mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                mail.To.Add(user.Email);
                                mail.Subject = "New user";
                                var loginUrl = "";

                                loginUrl = "http://jewish-studies.b2story.com/NewMemberAccount/1";
                                mail.Body = "Dear  , " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish;
                                mail.Body = mail.Body + "<br/>Thank you for visiting the book store of the World Union of Jewish Studies.";
                                mail.Body = mail.Body + "<br/> We invite you to become a member of the World Union of Jewish Studies and enjoy discounts on our books and a free copy of our journal “Jewish Studies. ";
                                mail.Body = mail.Body + "<br/> If you have not yet paid your membership dues for 2020, ";
                                mail.Body = mail.Body + string.Format(", you may pay them <a href='{0}'>online.</a>", loginUrl);
                                mail.Body = mail.Body + "<br/>Your login details:";
                                mail.Body = mail.Body + "<br/> Email address:   " + user.Email;
                                mail.Body = mail.Body + "<br/>Password:  " + new String(stringChars);
                                mail.Body = mail.Body + "<br/><br/> Feel free to contact us if you have any questions. ";
                                mail.Body = mail.Body + "<br/><br/> Best regards,";
                                mail.Body = mail.Body + "<br/><br/>WUJS staff";
                                mail.IsBodyHtml = true;
                            }
                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                CatchExeption("line 1095");

                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                CatchExeption("line 1098");

                                sendMailAppend = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    SqlDataReader rdr = null;
                    myConnection = new SqlConnection();
                    myConnection.ConnectionString = connectionString;

                    SqlCommand sqlCmd11 = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    UserPass Eexists = new UserPass();
                    sqlCmd11.CommandText = "select  * from UserPeass where Email='" + user.Email + "'";
                    sqlCmd11.Connection = myConnection;
                    myConnection.Open();
                    rdr = sqlCmd11.ExecuteReader();
                    while (rdr.Read())
                    {

                        var a = rdr.GetInt32(0);
                        var b = rdr.GetString(1);
                        var c = rdr.GetString(2);
                        Eexists = new webApi.classes.UserPass { Email = c, Password = b };
                    }
                    try
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            if (user.Language == "Hebrew")
                            {
                                CatchExeption("line 1075");
                                mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                mail.To.Add(user.Email);
                                mail.Subject = "יוזר חדש";
                                var loginUrl = "";

                                loginUrl = "http://jewish-studies.b2story.com/NewMemberAccount/1";



                                mail.Body = "שלום, " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew;
                                mail.Body = mail.Body + "<br/> תודה שביקרת בחנות הספרים של האיגוד העולמי למדעי היהדות.  ";
                                mail.Body = mail.Body + "<br/> אנו מזמינים אותך להצטרף לחברי האיגוד ולהנות מהנחות לספרים ועותק חינם של כתב העת מדעי היהדות. ";
                                mail.Body = mail.Body + "<br/> 2020 אם טרם הסדרת את תשלום דמי החבר שלך לשנת  ";
                                mail.Body = mail.Body + string.Format("ניתן לשלם .<a href='{0}'>בקישור</a>", loginUrl);
                                mail.Body = mail.Body + "<br/> שם משתמש: " + user.Email;
                                mail.Body = mail.Body + "<br/>סיסמה: " + new String(stringChars);
                                mail.Body = mail.Body + "<br/><br/>  לכל שאלה ניתן לפנות אלינו";
                                mail.Body = mail.Body + "<br/><br/> בברכה,";
                                mail.Body = mail.Body + "<br/><br/> צוות האיגוד";
                                mail.IsBodyHtml = true;
                            }
                            else
                            {
                                CatchExeption("line 1075");
                                mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                mail.To.Add(user.Email);
                                mail.Subject = "New user";
                                var loginUrl = "";

                                loginUrl = "http://jewish-studies.b2story.com/NewMemberAccount/1";
                                mail.Body = "Dear  , " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish;
                                mail.Body = mail.Body + "<br/>Thank you for visiting the book store of the World Union of Jewish Studies.";
                                mail.Body = mail.Body + "<br/> We invite you to become a member of the World Union of Jewish Studies and enjoy discounts on our books and a free copy of our journal “Jewish Studies. ";
                                mail.Body = mail.Body + "<br/> If you have not yet paid your membership dues for 2020, ";
                                mail.Body = mail.Body + string.Format(", you may pay them <a href='{0}'>online.</a>", loginUrl);
                                mail.Body = mail.Body + "<br/>Your login details:";
                                mail.Body = mail.Body + "<br/> Email address:   " + user.Email;
                                mail.Body = mail.Body + "<br/>Password:  " + new String(stringChars);
                                mail.Body = mail.Body + "<br/><br/> Feel free to contact us if you have any questions. ";
                                mail.Body = mail.Body + "<br/><br/> Best regards,";
                                mail.Body = mail.Body + "<br/><br/>WUJS staff";
                                mail.IsBodyHtml = true;
                            }
                            using (SmtpClient smtp = new SmtpClient("10.0.0.71", 25))
                            {
                                CatchExeption("line 1152");

                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                CatchExeption("line 1159");

                                sendMailAppend = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CatchExeption(ex.Message + "line 1167 FAIL!!!");
                    }


                }
                var finalString = new String(stringChars);
                if (sendMailAppend == false)
                {



                    //LoginUserName = user.Email;

                    /* try
                     {
                         using (MailMessage mail = new MailMessage())
                         {
                             mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                             //  mail.To.Add("tamy@bytes2story.com");
                             mail.To.Add(user.Email);
                             mail.Subject = "Test send email";
                             var loginUrl = "http://localhost:4000/UserPass/5";
                             var loginUrlNew = "http://localhost:4000/UserPass/2";
                             mail.Body = "<h3>your Details:</h3><br/><span> password:" + finalString + "</pan> <br/> <span> user Name: " + user.Email + "</span><br/>";

                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a>for a 2021 Congressional Lecture Proposal", loginUrl);
                             mail.Body = mail.Body + "<br/>";
                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a> to buy books", loginUrlNew);

                             mail.IsBodyHtml = true;
                             using (SmtpClient smtp = new SmtpClient("10.0.0.71", 25))
                             {
                                 smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                 //smtp.EnableSsl = true;
                                 smtp.Send(mail);


                             }
                     }
                     catch (Exception ex)
                     {
                         // Console.log(ex.message)
                     }*/
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }



        [Route("api/Home/Registration")]
        [HttpPost]
        public void Registration(webApi.classes.User user)
        {
            Console.WriteLine("registration");

            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                int ID = 0;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "select Id from Users where Email like N'%" + user.Email + "%'";

                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));

                }
                myConnection.Close();
                string sqlText = "";
                myConnection.ConnectionString = connectionString;
                DateTime datenow = DateTime.Now;

                if (ID != 0)
                {
                    sqlText = "Update Users set FirstNameEnglish=@FirstNameEnglish , LastNameEnglish=@LastNameEnglish,";
                    sqlText = sqlText + " FirstNameHebrew=@FirstNameHebrew , LastNameHebrew=@LastNameHebrew,";
                    sqlText = sqlText + " City=@City , Street=@Street , NumberHome=@NumberHome , NumberPhone1=@NumberPhone1,";
                    sqlText = sqlText + " NumberPhone2=@NumberPhone2 , selectedCountry=@selectedCountry,";
                    sqlText = sqlText + " selectedTitle=@selectedTitle , PostCode=@PostCode,";
                    sqlText = sqlText + " Email=@Email , Bio=@Bio , Students=@Students , WithoutStudemt=@WithoutStudemt,";
                    sqlText = sqlText + " EAJS=@EAJS , AJS=@AJS , Hebrew=@Hebrew , English=@English ,  Both=@Both ,Address=@Address,MemberShip=@MemberShip, Language=@Language,DateMember= @DateMember where Email like '%" + user.Email + "%'";
                }
                bool sendMailAppend = false;
                int Id = GetLastIdUser() + 1;

                if (ID == 0)
                {
                    Console.WriteLine("insert to user");

                    webApi.classes.UserPass Eexists = new webApi.classes.UserPass();
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew"
                       + ",NumberPhone1,selectedCountry,selectedTitle,PostCode," +
                       "Email,Bio,Students,WithoutStudemt,EAJS,AJS,Hebrew,English,Both,Address,MemberShip,Language,DateMember) " +
                       "values(@Id,@FirstNameEnglish,@LastNameEnglish,@FirstNameHebrew,@LastNameHebrew"
                       + ",@NumberPhone1,@selectedCountry,@selectedTitle,@PostCode," +
                       "@Email,@Bio,@Students,@WithoutStudemt,@EAJS,@AJS,@Hebrew,@English,@Both,@Address,@MemberShip,@Language,@DateMember)";
                }

                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", (object)Id ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FirstNameEnglish", (object)user.FirstNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameEnglish", (object)user.LastNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FirstNameHebrew", (object)user.FirstNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameHebrew", (object)user.LastNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@City", (object)user.City ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Street", (object)user.Street ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberHome", (object)user.NumberHome ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone1", (object)user.NumberPhone1 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone2", (object)user.NumberPhone2 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedCountry", (object)user.selectedCountry ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedTitle", (object)user.selectedTitle ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@PostCode", (object)user.PostCode ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Bio", (object)user.Bio ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Students", (object)user.Students ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@WithoutStudemt", (object)user.WithoutStudemt ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@EAJS", (object)user.EAJS ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@AJS", (object)user.AJS ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Hebrew", (object)user.Hebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@English", (object)user.English ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Both", (object)user.Both ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@MemberShip", (object)user.MemberShip ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Language", (object)user.Language ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@DateMember", DateTime.Now.ToString("M/d/yyyy"));

                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.ConnectionString = connectionString;

                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                var stringChars = new char[8];
                if (ID == 0)
                {

                    Console.WriteLine("insert to draft");

                    int Id11;
                    string sqlText11;
                    Id11 = GetLastIdDraft() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText11 = "Insert into ProposalDrafts (Id,UserName) values(@Id,@UserName)";
                    SqlCommand sqlCmd1 = new SqlCommand(sqlText11, conn);
                    sqlCmd1.Parameters.AddWithValue("@Id", Id11);
                    sqlCmd1.Parameters.AddWithValue("@UserName", user.Email);
                    conn.Close();

                    conn.Open();
                    int z = sqlCmd1.ExecuteNonQuery();
                    conn.Close();
                }
                int checkExists = checkEmailFind(user.Email);
                if (checkExists == 1)
                {

                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var random = new Random();

                    for (int j = 0; j < stringChars.Length; j++)
                    {
                        stringChars[j] = chars[random.Next(chars.Length)];
                    }
                    Id = GetLastIdUserPass() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                    SqlCommand sqlCmd1 = new SqlCommand(sqlText, conn);
                    sqlCmd1.Parameters.AddWithValue("@Id", Id);
                    // sqlCmd1.Parameters.AddWithValue("@UserName", user.UserName);
                    sqlCmd1.Parameters.AddWithValue("@Password", new String(stringChars));
                    sqlCmd1.Parameters.AddWithValue("@Email", user.Email);
                    Console.WriteLine("insert to pass");


                    // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                    conn.Open();
                    i = sqlCmd1.ExecuteNonQuery();
                    conn.Close();
                    try
                    {
                        CatchExeption("line 980");

                        using (MailMessage mail = new MailMessage())
                        {

                            CatchExeption("line 986");
                            var loginUrl2 = "http://jewish-studies.b2story.com/UserPass/3/";
                            var loginUrl1 = "http://jewish-studies.b2story.com/UserPass/3/";
                            mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                            //  mail.To.Add("tamy@bytes2story.com");
                            mail.To.Add(user.Email);
                            if (user.Language == "English")
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                if (user.selectedTitle == "Other")
                                    mail.Body = "<div style='direction: ltr;'>Dear  " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                else mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";

                            }
                            else if (user.Language == "Hebrew")

                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            else
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";
                            }
                            mail.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                CatchExeption(" line 1012");
                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                CatchExeption(" line 1389");

                                sendMailAppend = true;

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CatchExeption(ex.ToString() + " line 1023");
                    }
                }
                else
                {
                    SqlDataReader rdr = null;
                    myConnection = new SqlConnection();
                    myConnection.ConnectionString = connectionString;

                    SqlCommand sqlCmd11 = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    UserPass Eexists = new UserPass();
                    sqlCmd11.CommandText = "select  * from UserPeass where Email='" + user.Email + "'";
                    sqlCmd11.Connection = myConnection;
                    myConnection.Open();
                    rdr = sqlCmd11.ExecuteReader();
                    while (rdr.Read())
                    {

                        var a = rdr.GetInt32(0);
                        var b = rdr.GetString(1);
                        var c = rdr.GetString(2);
                        Eexists = new webApi.classes.UserPass { Email = c, Password = b };
                    }
                    try
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            var loginUrl2 = "http://jewish-studies.b2story.com/UserPass/3/";
                            var loginUrl1 = "http://jewish-studies.b2story.com/UserPass/3/";


                            mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                            //  mail.To.Add("tamy@bytes2story.com");
                            mail.To.Add(user.Email);
                            if (user.Language == "English")
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                if (user.selectedTitle == "Other")
                                    mail.Body = "<div style='direction: ltr;'>Dear  " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                else mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";

                            }
                            else if (user.Language == "Hebrew")

                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            else
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                if (user.selectedTitle == "Other")
                                    mail.Body = "<div style='direction: ltr;'>Dear  " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                else mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";
                            }
                            mail.IsBodyHtml = true;
                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                sendMailAppend = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CatchExeption(ex.ToString() + " line 1088");
                    }


                }
                var finalString = new String(stringChars);
                if (sendMailAppend == false)
                {



                    //LoginUserName = user.Email;

                    /* try
                     {
                         using (MailMessage mail = new MailMessage())
                         {
                             mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                             //  mail.To.Add("tamy@bytes2story.com");
                             mail.To.Add(user.Email);
                             mail.Subject = "Test send email";
                             var loginUrl = "http://localhost:4000/UserPass/5";
                             var loginUrlNew = "http://localhost:4000/UserPass/2";
                             mail.Body = "<h3>your Details:</h3><br/><span> password:" + finalString + "</pan> <br/> <span> user Name: " + user.Email + "</span><br/>";

                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a>for a 2021 Congressional Lecture Proposal", loginUrl);
                             mail.Body = mail.Body + "<br/>";
                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a> to buy books", loginUrlNew);

                             mail.IsBodyHtml = true;
                             using (SmtpClient smtp = new SmtpClient("10.0.0.71", 25))
                             {
                                 smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                 //smtp.EnableSsl = true;
                                 smtp.Send(mail);


                             }
                     }
                     catch (Exception ex)
                     {
                         // Console.log(ex.message)
                     }*/
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }


        [Route("api/Home/RegistrationHebrew")]
        [HttpPost]
        public void RegistrationHebrew(webApi.classes.User user)
        {
            Console.WriteLine("registration");

            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                int ID = 0;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "select Id from Users where Email like N'%" + user.Email + "%'";

                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));

                }
                myConnection.Close();
                string sqlText = "";
                myConnection.ConnectionString = connectionString;
                DateTime datenow = DateTime.Now;

                if (ID != 0)
                {
                    sqlText = "Update Users set FirstNameEnglish=@FirstNameEnglish , LastNameEnglish=@LastNameEnglish,";
                    sqlText = sqlText + " FirstNameHebrew=@FirstNameHebrew , LastNameHebrew=@LastNameHebrew,";
                    sqlText = sqlText + " City=@City , Street=@Street , NumberHome=@NumberHome , NumberPhone1=@NumberPhone1,";
                    sqlText = sqlText + " NumberPhone2=@NumberPhone2 , selectedCountry=@selectedCountry,";
                    sqlText = sqlText + " selectedTitle=@selectedTitle , PostCode=@PostCode,";
                    sqlText = sqlText + " Email=@Email , Bio=@Bio , Students=@Students , WithoutStudemt=@WithoutStudemt,";
                    sqlText = sqlText + " EAJS=@EAJS , AJS=@AJS , Hebrew=@Hebrew , English=@English ,  Both=@Both ,Address=@Address,MemberShip=@MemberShip, Language=@Language,DateMember= @DateMember where Email like '%" + user.Email + "%'";
                }
                bool sendMailAppend = false;
                int Id = GetLastIdUser() + 1;

                if (ID == 0)
                {
                    Console.WriteLine("insert to user");

                    webApi.classes.UserPass Eexists = new webApi.classes.UserPass();
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew"
                       + ",NumberPhone1,selectedCountry,selectedTitle,PostCode," +
                       "Email,Bio,Students,WithoutStudemt,EAJS,AJS,Hebrew,English,Both,Address,MemberShip,Language,DateMember) " +
                       "values(@Id,@FirstNameEnglish,@LastNameEnglish,@FirstNameHebrew,@LastNameHebrew"
                       + ",@NumberPhone1,@selectedCountry,@selectedTitle,@PostCode," +
                       "@Email,@Bio,@Students,@WithoutStudemt,@EAJS,@AJS,@Hebrew,@English,@Both,@Address,@MemberShip,@Language,@DateMember)";
                }

                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", (object)Id ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FirstNameEnglish", (object)user.FirstNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameEnglish", (object)user.LastNameEnglish ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@FirstNameHebrew", (object)user.FirstNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@LastNameHebrew", (object)user.LastNameHebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@City", (object)user.City ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Street", (object)user.Street ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberHome", (object)user.NumberHome ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone1", (object)user.NumberPhone1 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@NumberPhone2", (object)user.NumberPhone2 ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedCountry", (object)user.selectedCountry ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@selectedTitle", (object)user.selectedTitle ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@PostCode", (object)user.PostCode ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Bio", (object)user.Bio ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Students", (object)user.Students ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@WithoutStudemt", (object)user.WithoutStudemt ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@EAJS", (object)user.EAJS ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@AJS", (object)user.AJS ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Hebrew", (object)user.Hebrew ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@English", (object)user.English ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Both", (object)user.Both ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@MemberShip", (object)user.MemberShip ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Language", (object)user.Language ?? DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@DateMember", DateTime.Now);

                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.ConnectionString = connectionString;

                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                var stringChars = new char[8];
                if (ID == 0)
                {

                    Console.WriteLine("insert to draft");

                    int Id11;
                    string sqlText11;
                    Id11 = GetLastIdDraft() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText11 = "Insert into ProposalDrafts (Id,UserName) values(@Id,@UserName)";
                    SqlCommand sqlCmd1 = new SqlCommand(sqlText11, conn);
                    sqlCmd1.Parameters.AddWithValue("@Id", Id11);
                    sqlCmd1.Parameters.AddWithValue("@UserName", user.Email);
                    conn.Close();

                    conn.Open();
                    int z = sqlCmd1.ExecuteNonQuery();
                    conn.Close();
                }
                int checkExists = checkEmailFind(user.Email);
                if (checkExists == 1)
                {

                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var random = new Random();

                    for (int j = 0; j < stringChars.Length; j++)
                    {
                        stringChars[j] = chars[random.Next(chars.Length)];
                    }
                    Id = GetLastIdUserPass() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                    SqlCommand sqlCmd1 = new SqlCommand(sqlText, conn);
                    sqlCmd1.Parameters.AddWithValue("@Id", Id);
                    // sqlCmd1.Parameters.AddWithValue("@UserName", user.UserName);
                    sqlCmd1.Parameters.AddWithValue("@Password", new String(stringChars));
                    sqlCmd1.Parameters.AddWithValue("@Email", user.Email);
                    Console.WriteLine("insert to pass");


                    // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                    conn.Open();
                    i = sqlCmd1.ExecuteNonQuery();
                    conn.Close();
                    try
                    {
                        CatchExeption("line 980");


                        using (MailMessage mail = new MailMessage())
                        {

                            CatchExeption("line 986");
                            var loginUrl2 = "http://jewish-studies.b2story.com/UserPass/3/";
                            var loginUrl1 = "http://jewish-studies.b2story.com/UserPass/3/";
                            mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                            //  mail.To.Add("tamy@bytes2story.com");
                            mail.To.Add(user.Email);
                            if (user.Language == "English")
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                if (user.selectedTitle == "Other")
                                    mail.Body = "<div style='direction: ltr;'>Dear  " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                else mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";

                            }
                            else if (user.Language == "Hebrew")

                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            else
                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            mail.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                CatchExeption(" line 1012");
                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                CatchExeption(" line 1389");

                                sendMailAppend = true;

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CatchExeption(ex.ToString() + " line 1023");
                    }
                }
                else
                {
                    SqlDataReader rdr = null;
                    myConnection = new SqlConnection();
                    myConnection.ConnectionString = connectionString;

                    SqlCommand sqlCmd11 = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    UserPass Eexists = new UserPass();
                    sqlCmd11.CommandText = "select  * from UserPeass where Email='" + user.Email + "'";
                    sqlCmd11.Connection = myConnection;
                    myConnection.Open();
                    rdr = sqlCmd11.ExecuteReader();
                    while (rdr.Read())
                    {

                        var a = rdr.GetInt32(0);
                        var b = rdr.GetString(1);
                        var c = rdr.GetString(2);
                        Eexists = new webApi.classes.UserPass { Email = c, Password = b };
                    }
                    try
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            var loginUrl2 = "http://jewish-studies.b2story.com/UserPass/3/";
                            var loginUrl1 = "http://jewish-studies.b2story.com/UserPass/3/";


                            mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                            //  mail.To.Add("tamy@bytes2story.com");
                            mail.To.Add(user.Email);
                            if (user.Language == "English")
                            {
                                mail.Subject = "Registration for Membership in the World Union of Jewish Studies";
                                if (user.selectedTitle == "Other")
                                    mail.Body = "<div style='direction: ltr;'>Dear  " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                else mail.Body = "<div style='direction: ltr;'>Dear  " + user.selectedTitle + " " + user.FirstNameEnglish + " " + user.LastNameEnglish + "<br/>";
                                mail.Body = mail.Body + "Thank you for registering for membership in the World Union of Jewish Studies. <br/>";
                                mail.Body = mail.Body + "Your login details: <br/>";
                                mail.Body = mail.Body + "Email address: " + user.Email + " <br/>";
                                mail.Body = mail.Body + "Password: " + new String(stringChars) + " <br/></div>";
                                mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("If you have not yet paid your membership dues for 2020, you may pay them  <a href='{0}'>online</a>", loginUrl2);
                                mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions. <br /><br/> ";
                                mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";

                            }
                            else if (user.Language == "Hebrew")

                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            else
                            {
                                mail.Subject = "הרשמה כחבר באיגוד העולמי למדעי היהדות";
                                if (user.selectedTitle == "אחר")
                                    mail.Body = "שלום  " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                else mail.Body = "שלום  " + user.selectedTitle + " " + user.FirstNameHebrew + " " + user.LastNameHebrew + "<br/>";
                                mail.Body = mail.Body + "תודה על הרשמתך כחבר באיגוד העולמי למדעי היהדות.   <br/>";
                                mail.Body = mail.Body + "להלן פרטי כניסה לחשבונך: <br/>";
                                mail.Body = mail.Body + "שם משתמש:  " + user.Email + " <br/>";
                                mail.Body = mail.Body + "סיסמה: " + new String(stringChars) + " <br/>";
                                mail.Body = mail.Body + string.Format("אם טרם הסדרת את תשלום דמי החבר שלך לשנת 2020: ניתן לשלם .  <a href='{0}'>בקישור</a>", loginUrl1);
                                mail.Body = mail.Body + "<br /> לכל שאלה ניתן לפנות אלינו.  <br /> ";
                                mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";
                            }
                            mail.IsBodyHtml = true;
                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                                sendMailAppend = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CatchExeption(ex.ToString() + " line 1088");
                    }


                }
                var finalString = new String(stringChars);
                if (sendMailAppend == false)
                {



                    //LoginUserName = user.Email;

                    /* try
                     {
                         using (MailMessage mail = new MailMessage())
                         {
                             mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                             //  mail.To.Add("tamy@bytes2story.com");
                             mail.To.Add(user.Email);
                             mail.Subject = "Test send email";
                             var loginUrl = "http://localhost:4000/UserPass/5";
                             var loginUrlNew = "http://localhost:4000/UserPass/2";
                             mail.Body = "<h3>your Details:</h3><br/><span> password:" + finalString + "</pan> <br/> <span> user Name: " + user.Email + "</span><br/>";

                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a>for a 2021 Congressional Lecture Proposal", loginUrl);
                             mail.Body = mail.Body + "<br/>";
                             mail.Body = mail.Body + string.Format("Click <a href='{0}'>here</a> to buy books", loginUrlNew);

                             mail.IsBodyHtml = true;
                             using (SmtpClient smtp = new SmtpClient("10.0.0.71", 25))
                             {
                                 smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                 //smtp.EnableSsl = true;
                                 smtp.Send(mail);


                             }
                     }
                     catch (Exception ex)
                     {
                         // Console.log(ex.message)
                     }*/
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }


        [Route("api/Home/CheckUserPassword")]
        [HttpPost]
        public bool CheckUserPassword(UserPass item)
        {
            try
            {
                SqlDataReader rdr = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "select  * from UserPeass where Email like N'%" + item.Email + "%' and Password like N'%" + item.Password + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {

                    var a = rdr.GetInt32(0);
                    var b = rdr.GetString(1);
                    var c = rdr.GetString(2);
                    //LoginUserName = item.Email;

                    return true;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
            return false;
        }

        [Route("api/Home/GetDivisionEnglish")]
        [HttpGet]
        public List<string> GetDivisionEnglish()

        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Divisim = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct  Division from DivisionsEnglish";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Divisim.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Divisim;
        }


        [Route("api/Home/GetDivisionHebrew")]
        [HttpGet]
        public List<string> GetDivisionHebrew()
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Divisim = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct  Division from DivisionsHebrew";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Divisim.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Divisim;
        }

        [Route("api/Home/GetSubDivisionEnglish")]
        [HttpGet]
        public List<string> GetSubDivisionEnglish(string Div)
        {
            //Division = Div;
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> SubDivisim = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct SubDivision   from DivisionsEnglish where Division LIKE N'" + Div + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    SubDivisim.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return SubDivisim;
        }

        [Route("api/Home/GetSubDivisionHebrew")]
        [HttpGet]
        public List<string> GetSubDivisionHebrew(string Div)
        {
            //Division = Div;
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> SubDivisim = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct SubDivision   from DivisionsHebrew where Division LIKE'" + Div + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    SubDivisim.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return SubDivisim;
        }


        [Route("api/Home/GetLanguageEnglish")]

        [HttpGet]
        public List<string> GetLanguageEnglish(string SubDiv, string Division)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Language = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct Language   from DivisionsEnglish where SubDivision LIKE N'%" + SubDiv + "%' and  Division LIKE N'%" + Division + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Language.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Language;
        }
        [Route("api/Home/GetLanguageHebrew")]

        [HttpGet]
        public List<string> GetLanguageHebrew(string SubDiv, string Division)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Language = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct Language   from DivisionsHebrew where SubDivision LIKE N'%" + SubDiv + "%' and  Division LIKE N'%" + Division + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Language.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Language;
        }
        [Route("api/Home/GetLanguageEnglishSession")]

        [HttpGet]
        public List<string> GetLanguageEnglishSession(string SubDiv, string Division)
        {
            CatchExeption("line 1757 subdiv:" + SubDiv + "division: " + Division);

            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Language = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct Language   from DivisionsEnglish where SubDivision LIKE'" + SubDiv + "%' and  Division LIKE'" + Division + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Language.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Language;
        }
        [Route("api/Home/GetLanguageHebrewSession")]

        [HttpGet]
        public List<string> GetLanguageHebrewSession(string SubDiv, string Division)
        {
            CatchExeption("line 1757 subdiv:" + SubDiv + "division: " + Division);

            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<string> Language = new List<string>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select distinct Language   from DivisionsHebrew where SubDivision LIKE'" + SubDiv + "%' and  Division LIKE'" + Division + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    Language.Add(rdr.GetString(0));
                    //var a = rdr.GetInt32(0);
                    //var b = rdr.GetString(1);
                    //var c = rdr.GetString(2);
                    //LoginUserName = item.UserName;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Language;
        }

        //[Route("api/Home/GetLanguageHebrew")]

        //[HttpGet]
        //public List<string> GetLanguageHebrew(string SubDiv , string Division)
        //{
        //    SqlDataReader rdr = null;
        //    SqlConnection myConnection = new SqlConnection();
        //    myConnection.ConnectionString = connectionString;
        //    List<string> Language = new List<string>();
        //    SqlCommand sqlCmd = new SqlCommand();
        //    sqlCmd.CommandType = CommandType.Text;
        //    try
        //    {
        //        sqlCmd.CommandText = "select distinct Language   from DivisionsHebrew where SubDivision LIKE'" + SubDiv + "%' and  Division LIKE'" + Division + "%'";
        //        sqlCmd.Connection = myConnection;
        //        myConnection.Open();
        //        rdr = sqlCmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            Language.Add(rdr.GetString(0));
        //            //var a = rdr.GetInt32(0);
        //            //var b = rdr.GetString(1);
        //            //var c = rdr.GetString(2);
        //            //LoginUserName = item.UserName;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        conn.Close();
        //        myConnection.Close();
        //        CatchExeption(ex.ToString());
        //    }
        //    return Language;
        //}
        [Route("api/Home/checkDrafeByUser")]

        [HttpGet]
        public int checkDrafeByUser(string LoginUserName)
        {
            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select top(1) Id from ProposalDrafts where UserName='" + LoginUserName + "'"; ;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 0;
        }

        [Route("api/Home/enterSecondDraft")]
        [HttpPost]
        public void enterSecondDraft(Proposals prop)
        {
            int Count = 1;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select count(Id) from ProposalDrafts where UserName LIKE N'" + prop.UserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Count = Convert.ToInt32(reader.GetValue(0));

                }
                myConnection.Close();

                int Id;
                string sqlText;
                if (Count == 1)
                {
                    Id = GetLastIdDraft() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,TitleEnglish,TitleHebrew,Proposal,Language,Keywords) values(@Id,@UserName,@Division,@SubDivision,@TitleEnglish"
                       + ",@TitleHebrew,@Proposal,@Language,@Keywords)";
                }
                else
                {
                    Id = GetLastIdDraftSecond(prop.UserName);
                    conn.ConnectionString = connectionString;
                    sqlText = "UPDATE ProposalDrafts SET Division = @Division, SubDivision = @SubDivision, TitleEnglish=@TitleEnglish"
                   + ",TitleHebrew=@TitleHebrew,Proposal=@Proposal,Language=@Language,Keywords=@Keywords where UserName = '" + prop.UserName + "' and Id=@Id";
                }
                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", Id);
                sqlCmd.Parameters.AddWithValue("@UserName", prop.UserName);
                sqlCmd.Parameters.AddWithValue("@Division", prop.Division);
                sqlCmd.Parameters.AddWithValue("@SubDivision", prop.SubDivision);
                sqlCmd.Parameters.AddWithValue("@TitleEnglish", prop.TitleEnglish);
                sqlCmd.Parameters.AddWithValue("@TitleHebrew", prop.TitleHebrew);
                sqlCmd.Parameters.AddWithValue("@Proposal", prop.Proposal);
                sqlCmd.Parameters.AddWithValue("@Language", prop.Language);
                sqlCmd.Parameters.AddWithValue("@Keywords", prop.Keywords);


                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
        }
        [Route("api/Home/enterDraft")]
        [HttpPost]
        public void enterDraft(Proposals prop)
        {
            int Id;
            string sqlText;
            try
            {
                Id = checkDrafeByUser(prop.UserName);
                if (Id == 0)
                {
                    Id = GetLastIdDraft() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,TitleEnglish,TitleHebrew,Proposal,Language,Keywords) values(@Id,@UserName,@Division,@SubDivision,@TitleEnglish"
                       + ",@TitleHebrew,@Proposal,@Language,@Keywords)";
                }
                else
                {
                    conn.ConnectionString = connectionString;
                    sqlText = "update ProposalDrafts SET Division = @Division , SubDivision = @SubDivision , TitleEnglish=@TitleEnglish"
                   + ", TitleHebrew=@TitleHebrew , Proposal=@Proposal , Language=@Language , Keywords=@Keywords where UserName Like N'%" + prop.UserName + "%'";
                }
                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", Id);
                sqlCmd.Parameters.AddWithValue("@UserName", prop.UserName ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Division", prop.Division ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@SubDivision", prop.SubDivision ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@TitleEnglish", prop.TitleEnglish ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@TitleHebrew", prop.TitleHebrew ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Proposal", prop.Proposal ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Language", prop.Language ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Keywords", prop.Keywords ?? (object)DBNull.Value);


                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                //SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                string Lang = "";
                string First = "";
                string FirstE = "";
                string selectedTitle = "";
                string LastNameEnglish = "";
                string LastNameHebrew = "";
                string Pass = "";
                sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish,selectedTitle,LastNameEnglish,LastNameHebrew FROM Users  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Lang = Convert.ToString(reader.GetValue(0));
                    First = Convert.ToString(reader.GetValue(1));
                    FirstE = Convert.ToString(reader.GetValue(2));
                    selectedTitle = Convert.ToString(reader.GetValue(3));
                    LastNameEnglish = Convert.ToString(reader.GetValue(4));
                    LastNameHebrew = Convert.ToString(reader.GetValue(5));
                }
                myConnection.Close();
                sqlCmd.CommandText = "SELECT Password FROM UserPeass  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Pass = Convert.ToString(reader.GetValue(0));

                }
                myConnection.Close();
                try
                {
                    CatchExeption("line 980");
                    using (MailMessage mail = new MailMessage())
                    {
                        CatchExeption("line 986");
                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        mail.To.Add(prop.UserName);
                        if (Lang == "English")
                        {
                            mail.Subject = "Proposal Draft Saved";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mail.Body = mail.Body + "<br/> A draft version of your lecture proposal has been saved. ";
                            mail.Body = mail.Body + "<br/><span style='font - weight:bold;'>Please note: You have not yet submitted your proposal. </span>";
                            mail.Body = mail.Body + "<br/>Your proposal will be forwarded to the academic committee once you have completed the submission by clicking the SUBMIT button on the registration page.";
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>Password: " + Pass + "</div>";
                            mail.Body = mail.Body + "<br /> <div style='direction: ltr;'>Feel free to contact us if you have any questions.  <br /><br/> ";
                            mail.Body = mail.Body + "<br/>Best regards,<br/>WUJS staff </div>";
                        }
                        else if (Lang == "Hebrew")
                        {
                            mail.Subject = "קבלת הצעת הרצאה במערכת- טיוטה";
                            mail.Body = "<div>שלום   " + selectedTitle + " " + First + " " + LastNameHebrew + "<br/>";
                            mail.Body = mail.Body + "הטיוטה של הצעתך נקלטה בהצלחה במערכת. ";
                            mail.Body = mail.Body + "<br/><span style='font - weight:bold;'>שים לב: שמירת ההצעה כטיוטה לא מהווה הגשה! </span>";
                            mail.Body = mail.Body + "<br/>הצעתך תועבר לוועדה האקדמית רק לאחר ביצוע הגשה סופית.";
                            mail.Body = mail.Body + "<br/>להלן פרטי כניסה לחשבונך:";
                            mail.Body = mail.Body + "<br/>שם משתמש: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>סיסמה: " + Pass + "</div>";
                            mail.Body = mail.Body + "<div><br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                            mail.Body = mail.Body + "בברכה,<br/>צוות האיגוד</div>";
                        }
                        else
                        {
                            mail.Subject = "Proposal Draft Saved";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mail.Body = mail.Body + "<br/> A draft version of your lecture proposal has been saved. ";
                            mail.Body = mail.Body + "<br/><span style='font - weight:bold;'>Please note: You have not yet submitted your proposal. </span>";
                            mail.Body = mail.Body + "<br/>Your proposal will be forwarded to the academic committee once you have completed the submission by clicking the SUBMIT button on the registration page.";
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>Password: " + Pass + "</div>";
                            mail.Body = mail.Body + "<br /> <div style='direction: ltr;'>Feel free to contact us if you have any questions.  <br /><br/> ";
                            mail.Body = mail.Body + "<br/>Best regards,<br/>WUJS staff </div>";
                        }
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            CatchExeption(" line 1012");
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                            CatchExeption(" line 1389");


                        }

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchExeption(ex.ToString());
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }
        [Route("api/Home/enterNewDraft")]
        [HttpPost]
        public void enterNewDraft(NewProp prop)
        {
            int Id;
            int IdUser;
            string sqlText;
            try
            {
                CatchExeption("line 2304 new today");


                IdUser = checkEmailFind(prop.UserName);
                Id = checkDrafeByUser(prop.UserName);
                if (IdUser == 1)
                {
                    CatchExeption("line 986");
                    CatchExeption("line 2313 new today");
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int j = 0; j < stringChars.Length; j++)
                    {
                        stringChars[j] = chars[random.Next(chars.Length)];
                    }

                    var finalString = new String(stringChars);
                    string sqlText111;
                    int Id1 = GetLastIdUserPass() + 1;
                    int IDUsers = GetLastIdUser() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText111 = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                    SqlCommand sqlCmd111 = new SqlCommand(sqlText111, conn);
                    sqlCmd111.Parameters.AddWithValue("@Id", Id1);
                    sqlCmd111.Parameters.AddWithValue("@Password", finalString);
                    sqlCmd111.Parameters.AddWithValue("@Email", prop.UserName);
                    conn.Open();
                    int z1 = sqlCmd111.ExecuteNonQuery();
                    conn.Close();
                    sqlText111 = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew,Email,Title) values(@Id,@FirstNameEnglish,@LastNameEnglish,@FirstNameHebrew,@LastNameHebrew,@Email,@Title)";
                    sqlCmd111 = new SqlCommand(sqlText111, conn);
                    sqlCmd111.Parameters.AddWithValue("@Id", IDUsers);
                    sqlCmd111.Parameters.AddWithValue("@FirstNameEnglish", prop.FirstNameEnglish ?? (object)DBNull.Value);
                    sqlCmd111.Parameters.AddWithValue("@LastNameEnglish", prop.LastNameEnglish ?? (object)DBNull.Value);
                    sqlCmd111.Parameters.AddWithValue("@FirstNameHebrew", prop.FirstNameHebrew ?? (object)DBNull.Value);
                    sqlCmd111.Parameters.AddWithValue("@LastNameHebrew", prop.LastNameHebrew ?? (object)DBNull.Value);
                    sqlCmd111.Parameters.AddWithValue("@Email", prop.UserName ?? (object)DBNull.Value);
                    sqlCmd111.Parameters.AddWithValue("@Title", prop.Title ?? (object)DBNull.Value);

                    conn.Open();
                    //open new draft
                    int z11 = sqlCmd111.ExecuteNonQuery();
                    conn.Close();

                    CatchExeption("line 2350 new today");

                }
                if (Id == 0)
                {
                    CatchExeption("line 2355 new today");

                    Id = GetLastIdDraft() + 1;
                    conn.ConnectionString = connectionString;
                    sqlText = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,TitleEnglish,TitleHebrew,Proposal,Language,Keywords) values(@Id,@UserName,@Division,@SubDivision,@TitleEnglish"
                       + ",@TitleHebrew,@Proposal,@Language,@Keywords)";
                }
                else
                {
                    CatchExeption("line 2364 new today");

                    conn.ConnectionString = connectionString;
                    sqlText = "update ProposalDrafts SET Division = @Division , SubDivision = @SubDivision , TitleEnglish=@TitleEnglish"
                   + ", TitleHebrew=@TitleHebrew , Proposal=@Proposal , Language=@Language , Keywords=@Keywords where UserName Like N'%" + prop.UserName + "%'";
                }
                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", Id);
                CatchExeption("line 2364 new today" + prop.UserName);

                sqlCmd.Parameters.AddWithValue("@UserName", prop.UserName ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.Division);

                sqlCmd.Parameters.AddWithValue("@Division", prop.Division ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.SubDivision);

                sqlCmd.Parameters.AddWithValue("@SubDivision", prop.SubDivision ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.TitleEnglish);

                sqlCmd.Parameters.AddWithValue("@TitleEnglish", prop.TitleEnglish ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.TitleHebrew);

                sqlCmd.Parameters.AddWithValue("@TitleHebrew", prop.TitleHebrew ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.Proposal);

                sqlCmd.Parameters.AddWithValue("@Proposal", prop.Proposal ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.Language);

                sqlCmd.Parameters.AddWithValue("@Language", prop.Language ?? (object)DBNull.Value);
                CatchExeption("line 2364 new today" + prop.Keywords);

                sqlCmd.Parameters.AddWithValue("@Keywords", prop.Keywords ?? (object)DBNull.Value);

                CatchExeption("line 2381 new today");

                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                CatchExeption("line 2387 new today");

                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                //SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                string Lang = "";
                string First = "";
                string FirstE = "";
                string selectedTitle = "";
                string LastNameEnglish = "";
                string LastNameHebrew = "";
                string Pass = "";
                sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish,selectedTitle,LastNameEnglish,LastNameHebrew FROM Users  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Lang = Convert.ToString(reader.GetValue(0));
                    First = Convert.ToString(reader.GetValue(1));
                    FirstE = Convert.ToString(reader.GetValue(2));
                    selectedTitle = Convert.ToString(reader.GetValue(3));
                    LastNameEnglish = Convert.ToString(reader.GetValue(4));
                    LastNameHebrew = Convert.ToString(reader.GetValue(5));
                }
                myConnection.Close();
                sqlCmd.CommandText = "SELECT Password FROM UserPeass  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Pass = Convert.ToString(reader.GetValue(0));

                }
                myConnection.Close();
                try
                {
                    CatchExeption("line 980");
                    using (MailMessage mail = new MailMessage())
                    {
                        CatchExeption("line 986");
                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        mail.To.Add(prop.UserName);
                        if (Lang == "English   ")
                        {
                            mail.Subject = "Proposal Draft Saved";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mail.Body = mail.Body + "<br/> A draft version of your lecture proposal has been saved. ";
                            mail.Body = mail.Body + "<br/><span style='font - weight:bold;'>Please note: You have not yet submitted your proposal. </span>";
                            mail.Body = mail.Body + "<br/>Your proposal will be forwarded to the academic committee once you have completed the submission by clicking the SUBMIT button on the registration page.";
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>Password: " + Pass + "</div>";
                            mail.Body = mail.Body + "<br /> <div style='direction: ltr;'>Feel free to contact us if you have any questions.  <br /><br/> ";
                            mail.Body = mail.Body + "<br/>Best regards,<br/>WUJS staff </div>";
                        }
                        else if (Lang == "Hebrew    ")
                        {
                            mail.Subject = "קבלת הצעת הרצאה במערכת- טיוטה";
                            mail.Body = "<div>שלום   " + selectedTitle + " " + First + " " + LastNameHebrew + "<br/>";
                            mail.Body = mail.Body + "הטיוטה של הצעתך נקלטה בהצלחה במערכת. ";
                            mail.Body = mail.Body + "<br/><span style='font - weight:bold;'>שים לב: שמירת ההצעה כטיוטה לא מהווה הגשה! </span>";
                            mail.Body = mail.Body + "<br/>הצעתך תועבר לוועדה האקדמית רק לאחר ביצוע הגשה סופית.";
                            mail.Body = mail.Body + "<br/>להלן פרטי כניסה לחשבונך:";
                            mail.Body = mail.Body + "<br/>שם משתמש: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>סיסמה: " + Pass + "</div>";
                            mail.Body = mail.Body + "<div><br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                            mail.Body = mail.Body + "בברכה,<br/>צוות האיגוד</div>";
                        }
                        else
                        {
                            string mess = "";
                            mail.Subject = "Proposal Draft Saved";
                            mess = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mess = mess + "<br/> A draft version of your lecture proposal has been saved. ";
                            mess = mess + "<br/><span style='font - weight:bold;'>Please note: You have not yet submitted your proposal. </span>";
                            mess = mess + "<br/>Your proposal will be forwarded to the academic committee once you have completed the submission by clicking the SUBMIT button on the registration page.";
                            mess = mess + "<br/>Your login details:";
                            mess = mess + "<br/>User name: " + prop.UserName;
                            mess = mess + "<br/>Password: " + Pass + "</div>";
                            mess = mess + "<br /> <div style='direction: ltr;'>Feel free to contact us if you have any questions.  <br /><br/> ";
                            mess = mess + "<br/>Best regards,<br/>WUJS staff </div>";

                            mail.Body = mess;
                        }
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            CatchExeption(" line 1012");
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                            CatchExeption(" line 1389");


                        }

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchExeption(ex.ToString());
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }
        [Route("api/Home/enterProposal")]
        [HttpPost]
        public void enterProposal(Proposals prop)
        {
            try
            {
                string Pass = "";
                int Id = GetLastIdProposal() + 1;
                conn.ConnectionString = connectionString;
                string sqlText = "Insert into Proposal (Id,UserName,Division,SubDivision,TitleEnglish,TitleHebrew,Proposal,Language,Keywords,SessionName,Chairman,ChairmanEmail,SessionId,Creator)" +
                    " values(@Id,@UserName,@Division,@SubDivision,@TitleEnglish"
                    + ",@TitleHebrew,@Proposal,@Language,@Keywords,@SessionName,@Chairman,@ChairmanEmail,@SessionId,@Creator)";
                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", Id);
                sqlCmd.Parameters.AddWithValue("@UserName", prop.UserName ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Division", prop.Division ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@SubDivision", prop.SubDivision ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@TitleEnglish", prop.TitleEnglish ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@TitleHebrew", prop.TitleHebrew ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Proposal", prop.Proposal ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Language", prop.Language ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Keywords", prop.Keywords ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@SessionName", prop.Keywords ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Chairman", prop.Chairman ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@ChairmanEmail", prop.ChairmanEmail ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@SessionId", prop.SessionId);
                sqlCmd.Parameters.AddWithValue("@Creator", prop.Creator);


                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                conn.Close();


                int IdJ = GetLastIdJudges() + 1;
                conn.ConnectionString = connectionString;
                sqlText = "Insert into Judges (Id,IdProposal,Status) values(@Id,@IdProposal,@Status)";
                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", IdJ);
                sqlCmd.Parameters.AddWithValue("@IdProposal", Id);
                sqlCmd.Parameters.AddWithValue("@Status", "Pending");

                conn.Open();
                i = sqlCmd.ExecuteNonQuery();
                conn.Close();
                conn.ConnectionString = connectionString;
                sqlText = "delete from ProposalDrafts where UserName like N'%" + prop.UserName + "%' and Division like N'%" + prop.Division + "%'";
                sqlCmd = new SqlCommand(sqlText, conn);

                conn.Open();
                i = sqlCmd.ExecuteNonQuery();
                conn.Close();


                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;
                //SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                string Lang = "";
                string First = "";
                string FirstE = "";
                string selectedTitle = "";
                string LastNameEnglish = "";
                string LastNameHebrew = "";

                sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish,selectedTitle,LastNameEnglish,LastNameHebrew FROM Users  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Lang = Convert.ToString(reader.GetValue(0));
                    First = Convert.ToString(reader.GetValue(1));
                    FirstE = Convert.ToString(reader.GetValue(2));
                    selectedTitle = Convert.ToString(reader.GetValue(3));
                    LastNameEnglish = Convert.ToString(reader.GetValue(4));
                    LastNameHebrew = Convert.ToString(reader.GetValue(5));
                }
                myConnection.Close();
                sqlCmd.CommandText = "SELECT Password FROM UserPeass  where Email ='" + prop.UserName + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Pass = Convert.ToString(reader.GetValue(0));

                }
                myConnection.Close();
                try
                {
                    CatchExeption("line 980");


                    using (MailMessage mail = new MailMessage())
                    {

                        CatchExeption("line 986");

                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        //  mail.To.Add("tamy@bytes2story.com");
                        mail.To.Add(prop.UserName);

                        if (Lang == "English")
                        {
                            mail.Subject = "Lecture proposal submitted";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mail.Body = mail.Body + "<br/>  Thank you for submitting your abstract to the 18th World Congress of Jewish Studies, to be held at The Hebrew University of Jerusalem, August 1-5, 2021.";
                            mail.Body = mail.Body + "<br/> Your Lecture Title is: " + prop.TitleEnglish;
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>Password: " + Pass + "</div>";
                            mail.Body = mail.Body + "<div style='direction: ltr;'><br/> The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021.  <br/><br/> ";
                            mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions.  <br /><br/> ";
                            mail.Body = mail.Body + "<br/>Best regards,<br/>WUJS staff </div>";
                        }
                        else if (Lang == "Hebrew")
                        {
                            mail.Subject = "קבלת הצעת ההרצאה במערכת – הגשה סופית";
                            mail.Body = "<div >שלום   " + selectedTitle + " " + First + " " + LastNameHebrew + "<br/>";
                            mail.Body = mail.Body + "תודה על הגשת הצעתך לקונגרס העולמי ה-18 למדעי היהדות שיתקיים באוניברסיטה העברית בירושלים בתאריכים כ'ג אב-כ'ז אב תשפ'א, 5 - 1 באוגוסט, 2021.";
                            mail.Body = mail.Body + "<br/>כותרת הרצאתך היא: " + prop.TitleHebrew;
                            mail.Body = mail.Body + "<br/> החלטות בדבר קבלה או דחיה של הצעות מתקבלות על ידי הועדה האקדמית של האיגוד העולמי למדעי היהדות. תשובות ינתנו למרצים עד פברואר 2021. ";
                            mail.Body = mail.Body + "<br/>להלן פרטי כניסה לחשבונך:";
                            mail.Body = mail.Body + "<br/>שם משתמש: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>סיסמה: " + Pass + "</div>";
                            mail.Body = mail.Body + "<br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                            mail.Body = mail.Body + "בברכה,<br/>צוות האיגוד</div>";
                        }
                        else
                        {
                            mail.Subject = "Lecture proposal submitted";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                            mail.Body = mail.Body + "<br/>  Thank you for submitting your abstract to the 18th World Congress of Jewish Studies, to be held at The Hebrew University of Jerusalem, August 1-5, 2021.";
                            mail.Body = mail.Body + "<br/> Your Lecture Title is: " + prop.TitleEnglish;
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + prop.UserName;
                            mail.Body = mail.Body + "<br/>Password: " + Pass + "</div>";
                            mail.Body = mail.Body + "<div style='direction: ltr;'><br/> The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021.  <br/><br/> ";
                            mail.Body = mail.Body + "<br /> Feel free to contact us if you have any questions.  <br /><br/> ";
                            mail.Body = mail.Body + "<br/>Best regards,<br/>WUJS staff </div>";
                        }


                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            CatchExeption(" line 1012");
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                            CatchExeption(" line 1389");


                        }

                    }
                }
                catch (Exception ex)
                {
                    CatchExeption(ex.ToString() + " line 1023");
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }

        [Route("api/Home/selectDraft")]
        [HttpGet]
        public Proposals selectDraft(string LoginUserName)
        {
            int noDraft = 0;
            Proposals dict = new Proposals();

            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT top(1) * FROM ProposalDrafts  where UserName Like '%" + LoginUserName + "%' ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    CatchExeption("line 2510");

                    noDraft = 1;
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    string c = "";
                    c = reader.IsDBNull(2) ? null : reader.GetString(2);
                    //var e = rdr.GetDouble(3);
                    string d = "";
                    d = reader.IsDBNull(3) ? null : reader.GetString(3);
                    string e = "";
                    e = reader.IsDBNull(4) ? null : reader.GetString(4);
                    string f = "";
                    f = reader.IsDBNull(5) ? null : reader.GetString(5);
                    string g = "";
                    g = reader.IsDBNull(6) ? null : reader.GetString(6);
                    string h = "";
                    h = reader.IsDBNull(7) ? null : reader.GetString(7);
                    string i = "";
                    i = reader.IsDBNull(8) ? null : reader.GetString(8);
                    string j = "";
                    j = reader.IsDBNull(9) ? null : reader.GetString(9);
                    string l = "";
                    l = reader.IsDBNull(10) ? null : reader.GetString(10);
                    string m = "";
                    m = reader.IsDBNull(11) ? null : reader.GetString(11);
                    int k = 0;
                    k = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                    int kh = 0;
                    kh = reader.IsDBNull(13) ? 0 : reader.GetInt32(13);
                    //Image d;
                    var r = new Proposals { Division = c, SubDivision = d, TitleEnglish = e, TitleHebrew = f, Proposal = g, Language = h, Keywords = i, SessionName = j, SessionId = k, Chairman = l, ChairmanEmail = m, Creator = kh };
                    dict = r;
                    //Image d;

                }
                myConnection.Close();
                if (noDraft == 0)
                {
                    sqlCmd.CommandText = "SELECT top(1) * FROM Proposal  where UserName Like '%" + LoginUserName + "%' ";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        CatchExeption("line 2555");

                        noDraft = 1;
                        var a = reader.GetInt32(0);
                        var b = reader.GetString(1);
                        string c = "";
                        c = reader.IsDBNull(2) ? null : reader.GetString(2);
                        //var e = rdr.GetDouble(3);
                        string d = "";
                        d = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string e = "";
                        e = reader.IsDBNull(4) ? null : reader.GetString(4);
                        string f = "";
                        f = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string g = "";
                        g = reader.IsDBNull(6) ? null : reader.GetString(6);
                        string h = "";
                        h = reader.IsDBNull(7) ? null : reader.GetString(7);
                        string i = "";
                        i = reader.IsDBNull(8) ? null : reader.GetString(8);
                        string j = "";
                        j = reader.IsDBNull(9) ? null : reader.GetString(9);
                        string l = "";
                        l = reader.IsDBNull(10) ? null : reader.GetString(10);
                        string n = "";
                        n = reader.IsDBNull(11) ? null : reader.GetString(11);
                        var k = reader.GetInt32(12);
                        CatchExeption("line 2583 " + c);

                        //Image d;
                        var m = "0" + c;
                        var r = new Proposals { Division = m, SubDivision = d, TitleEnglish = e, TitleHebrew = f, Proposal = g, Language = h, Keywords = i, SessionName = j, SessionId = k, Chairman = l, ChairmanEmail = n };
                        dict = r;
                        //Image d;

                    }
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return dict;

        }



        [Route("api/Home/selectSecondDraft")]
        [HttpGet]
        public Proposals selectSecondDraft(string LoginUserName)
        {


            int Count = 1;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            Proposals dict = new Proposals();

            try
            {
                sqlCmd.CommandText = "select count(Id) from ProposalDrafts where UserName LIKE N'" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Count = Convert.ToInt32(reader.GetValue(0));

                }
                myConnection.Close();

                if (Count == 2)
                {
                    reader = null;
                    myConnection = new SqlConnection();
                    myConnection.ConnectionString = connectionString;
                    sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT top(1) * FROM ProposalDrafts  where UserName ='" + LoginUserName + "' Order By Id Desc ";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var a = reader.GetInt32(0);
                        var b = reader.GetString(1);
                        string c = "";
                        c = reader.IsDBNull(2) ? null : reader.GetString(2);
                        //var e = rdr.GetDouble(3);
                        string d = "";
                        d = reader.IsDBNull(3) ? null : reader.GetString(3);
                        string e = "";
                        e = reader.IsDBNull(4) ? null : reader.GetString(4);
                        string f = "";
                        f = reader.IsDBNull(5) ? null : reader.GetString(5);
                        string g = "";
                        g = reader.IsDBNull(6) ? null : reader.GetString(6);
                        string h = "";
                        h = reader.IsDBNull(7) ? null : reader.GetString(7);
                        string i = "";
                        i = reader.IsDBNull(8) ? null : reader.GetString(8);
                        string j = "";
                        j = reader.IsDBNull(9) ? null : reader.GetString(9);
                        //Image d;
                        var r = new Proposals { Division = c, SubDivision = d, TitleEnglish = e, TitleHebrew = f, Proposal = g, Language = h, Keywords = i, SessionName = j };
                        dict = r;
                        //Image d;

                    }
                    myConnection.Close();
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return dict;

        }




        [Route("api/Home/forgetPass")]
        [HttpGet]

        public int forgetPass(string email)
        {
            UserPass dict = new UserPass();
            int aa = 0;
            string Lang = "";
            string First = "";
            string FirstE = "";
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM UserPeass  where Email ='" + email + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    aa = 1;
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    var c = reader.GetString(2);
                    //var d = reader.GetString(3);
                    var r = new UserPass { Password = b, Email = c };
                    dict = r;
                }
                myConnection.Close();
                sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish FROM Users  where Email ='" + email + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Lang = Convert.ToString(reader.GetValue(0));
                    First = Convert.ToString(reader.GetValue(1));
                    FirstE = Convert.ToString(reader.GetValue(2));
                }
                myConnection.Close();

                try
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        //  mail.To.Add("tamy@bytes2story.com");
                        mail.To.Add(email);

                        mail.Subject = "Forgot password";
                        mail.Body = "<div style='direction: ltr;'>Dear  " + FirstE + "<br/> Thank you for your inquiry. <br/> Your password: :" + dict.Password + "<br/>Feel free to contact us if you have any questions. <br/><br/>";
                        mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";


                        //mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                        mail.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Console.log(ex.message)
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            if (aa == 1)
                return 1;
            else return 2;
        }
        [Route("api/Home/forgetPassHebrew")]
        [HttpGet]

        public int forgetPassHebrew(string email)
        {
            UserPass dict = new UserPass();
            int aa = 0;
            string Lang = "";
            string First = "";
            string FirstE = "";
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT * FROM UserPeass  where Email ='" + email + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    aa = 1;
                    var a = reader.GetInt32(0);
                    var b = reader.GetString(1);
                    var c = reader.GetString(2);
                    //var d = reader.GetString(3);
                    var r = new UserPass { Password = b, Email = c };
                    dict = r;
                }
                myConnection.Close();
                sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish FROM Users  where Email ='" + email + "'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Lang = Convert.ToString(reader.GetValue(0));
                    First = Convert.ToString(reader.GetValue(1));
                    FirstE = Convert.ToString(reader.GetValue(2));
                }
                myConnection.Close();

                try
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        //  mail.To.Add("tamy@bytes2story.com");
                        mail.To.Add(email);


                        mail.Subject = "שחזור סיסמה";
                        mail.Body = " שלום " + First + "<br/> הסיסמה שלך שוחזרה בהצלחה. <br/> סיסמתך:" + dict.Password;
                        mail.Body = mail.Body + "<br/><br/>" + "בברכה,<br/>צוות האיגוד";


                        //mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                        mail.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Console.log(ex.message)
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            if (aa == 1)
                return 1;
            else return 2;
        }


        [Route("api/Home/InviteMembers")]
        [HttpPost]
        public void InviteMembers(Invited[] arrInvited)
        {
            CatchExeption("line 2785 invite today");

            try
            {
                int IdSession = GetLastIdSession() + 1;

                CatchExeption("line 2280 " + arrInvited.Length);
                int CreatorSession = -1;
                string FirstCreator = "";
                string FirstECreator = "";
                string selectedTitleCreator = "";
                string LastNameHebrewCreator = "";
                string LastNameEnglishCreator = "";
                string LangCreator = "";
                string PassCreator = "";
                string FirstChairman = arrInvited[0].FirstNameChair;
                string FirstEChairman = arrInvited[0].FirstNameChair;
                string selectedTitleChairman = "";
                string LastNameHebrewChairman = arrInvited[0].LastNameChair;
                string LastNameEnglishChairman = arrInvited[0].LastNameChair;
                string LangChairman = "";
                string PassChairman = "";
                string Lang = "";
                string First = "";
                string FirstE = "";
                string selectedTitle = "";
                string LastNameHebrew = "";
                string LastNameEnglish = "";
                string Pass = "";

                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = connectionString;

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;

                sqlCmd.CommandText = "select top(1) Id, FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew,selectedTitle,Language from Users where Email like N'%" + arrInvited[4].Email + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    CreatorSession = Convert.ToInt32(reader.GetValue(0));
                    FirstECreator = Convert.ToString(reader.GetValue(1));
                    LastNameEnglishCreator = Convert.ToString(reader.GetValue(2));
                    FirstCreator = Convert.ToString(reader.GetValue(3));
                    LastNameHebrewCreator = Convert.ToString(reader.GetValue(4));
                    selectedTitleCreator = Convert.ToString(reader.GetValue(5));
                    LangCreator = Convert.ToString(reader.GetValue(6));
                }
                myConnection.Close();
                if (CreatorSession != -1)
                {
                    sqlCmd.CommandText = "select Password from UserPeass where Email like N'%" + arrInvited[4].Email + "%'";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PassCreator = Convert.ToString(reader.GetValue(0));

                    }
                    myConnection.Close();
                }
                else
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();
                    conn.ConnectionString = connectionString;

                    for (int j = 0; j < stringChars.Length; j++)
                    {
                        stringChars[j] = chars[random.Next(chars.Length)];
                    }

                    var finalString = new String(stringChars);
                    //                       string[] words = arrInvited[k].Name.Split(' ');
                    string sqlText111;

                    int Id1 = GetLastIdUserPass() + 1;
                    int IDUsers = GetLastIdUser() + 1;
                    conn.ConnectionString = connectionString;
                    PassCreator = finalString;
                    //open count in userpass
                    CreatorSession = IDUsers;
                    FirstECreator = arrInvited[4].FirstName;
                    LastNameEnglishCreator = arrInvited[4].LastName;
                    FirstCreator = arrInvited[4].FirstName;
                    LastNameHebrewCreator = arrInvited[4].LastName;
                    selectedTitleCreator = arrInvited[4].Title;
                    LangCreator = "";
                    sqlText111 = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                    SqlCommand sqlCmd111 = new SqlCommand(sqlText111, conn);
                    sqlCmd111.Parameters.AddWithValue("@Id", Id1);
                    sqlCmd111.Parameters.AddWithValue("@Password", finalString);
                    sqlCmd111.Parameters.AddWithValue("@Email", arrInvited[4].Email);
                    conn.Open();
                    int z1 = sqlCmd111.ExecuteNonQuery();
                    conn.Close();
                    //open count in users
                    sqlText111 = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,Email,Title) values(@Id,@FirstNameEnglish,@LastNameEnglish,@Email,@Title)";
                    sqlCmd111 = new SqlCommand(sqlText111, conn);
                    sqlCmd111.Parameters.AddWithValue("@Id", IDUsers);
                    sqlCmd111.Parameters.AddWithValue("@FirstNameEnglish", arrInvited[4].FirstName);
                    sqlCmd111.Parameters.AddWithValue("@LastNameEnglish", arrInvited[4].LastName);
                    sqlCmd111.Parameters.AddWithValue("@Email", arrInvited[4].Email);
                    sqlCmd111.Parameters.AddWithValue("@Title", arrInvited[4].Title ?? (object)DBNull.Value);

                    conn.Open();
                    //open new draft
                    int z11 = sqlCmd111.ExecuteNonQuery();
                    conn.Close();

                }
                sqlCmd.CommandText = "select top(1) Id, FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew,selectedTitle,Language from Users where Email like N'%" + arrInvited[0].ChairmanEmail + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    CreatorSession = Convert.ToInt32(reader.GetValue(0));
                    FirstEChairman = Convert.ToString(reader.GetValue(1));
                    LastNameEnglishChairman = Convert.ToString(reader.GetValue(2));
                    FirstChairman = Convert.ToString(reader.GetValue(3));
                    LastNameHebrewChairman = Convert.ToString(reader.GetValue(4));
                    selectedTitleChairman = Convert.ToString(reader.GetValue(5));
                    LangChairman = Convert.ToString(reader.GetValue(6));
                }
                myConnection.Close();
                sqlCmd.CommandText = "select Password from UserPeass where Email like N'%" + arrInvited[0].ChairmanEmail + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    PassChairman = Convert.ToString(reader.GetValue(0));

                }
                myConnection.Close();


                try
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        //  mail.To.Add("tamy@bytes2story.com");
                        mail.To.Add(arrInvited[4].Email);
                        if (LangCreator == "English   ")
                        {
                            mail.Subject = "Session submission – confirmation email ";
                            mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator + "<br/>";
                            mail.Body = mail.Body + "<br/>Thank you for submitting a session proposal to the 18th World Congress of Jewish Studies, to be held at The Hebrew University of Jerusalem, August 1-5, 2021.";
                            mail.Body = mail.Body + "<br/>In order to complete the submission of the session proposal, all lecturers are required to log into their personal accounts and submit the details of their lecture proposal until December 1, 2020.";
                            mail.Body = mail.Body + "  Your Session Title is: " + arrInvited[2].SessionNameEnglish;
                            mail.Body = mail.Body + " <br/> Session Participants: ";
                            mail.Body = mail.Body + " <br/> Session chair: " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair;
                            mail.Body = mail.Body + " <br/>Lecturers: ";
                            mail.Body = mail.Body + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                            mail.Body = mail.Body + "<br/>Your login details:";
                            mail.Body = mail.Body + "<br/>User name: " + arrInvited[4].Email;
                            mail.Body = mail.Body + "<br/>Password: " + PassCreator + "<br/></div>";
                            mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("For the lecture submission page  <a href='{0}'>click here:</a> ", loginUrl);
                            mail.Body = mail.Body + "<br/>The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021. <br/> ";
                            mail.Body = mail.Body + "<div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                            mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";
                        }
                        else if (LangCreator == "Hebrew    ")
                        {
                            mail.Subject = "מגיש המושב";
                            mail.Body = "<div style='direction: rtl;' >שלום   " + selectedTitle + " " + First + " " + LastNameHebrew + "<br/>";
                            mail.Body = mail.Body + "<br/>תודה על הגשת מושב לקונגרס העולמי ה-18 למדעי היהדות שיתקיים באוניברסיטה העברית בירושלים בתאריכים כ'ג אב-כ'ז אב תשפ'א, 5 - 1 באוגוסט, 2021.";
                            mail.Body = mail.Body + "<br/>  כותרת המושב היא: " + arrInvited[2].SessionName;
                            mail.Body = mail.Body + " <br/>משתתפי המושב: ";
                            mail.Body = mail.Body + " <br/> יו'ר: " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair;
                            mail.Body = mail.Body + " <br/>מרצים: ";
                            mail.Body = mail.Body + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                            mail.Body = mail.Body + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                            mail.Body = mail.Body + "<br/>למשתתפי המושב נשלחה הודעה עם פרטי כניסה לחשבון אישי ועם בקשה להגיש הצעה להרצאה במסגרת המושב.";
                            mail.Body = mail.Body + "<br/>על מנת להשלים את תהליך ההגשה, על כל המרצים להיכנס לחשבונם באתר האיגוד העולמי למדעי היהדות ולהגיש את הצעתם להרצאה עד ה1 בדצמבר 2020.";
                            mail.Body = mail.Body + "<br/>החלטות בדבר קבלה או דחיה של הצעות מתקבלות על ידי הועדה האקדמית של האיגוד העולמי למדעי היהדות. תשובות ינתנו למרצים עד פברואר 2021.";
                            mail.Body = mail.Body + "<br/>להלן פרטי כניסה לחשבונך:";
                            mail.Body = mail.Body + "<br/>שם משתמש:  " + arrInvited[4].Email;
                            mail.Body = mail.Body + "<br/>סיסמה: " + PassCreator + "<br/></div>";
                            mail.Body = mail.Body + "<div style='direction: rtl;'>" + string.Format("  <a href='{0}'>קישור למערכת ההגשות</a> ", loginUrl);
                            mail.Body = mail.Body + "<div style='direction: rtl;'><br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                            mail.Body = mail.Body + "בברכה,<br/>צוות האיגוד</div>";
                        }
                        else
                        {
                            string mess = "";
                            mail.Subject = "Session submission – confirmation email ";
                            mess = "<div style='direction: ltr;'>Dear  " + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator + "<br/>";
                            mess = mess + "<br/>Thank you for submitting a session proposal to the 18th World Congress of Jewish Studies, to be held at The Hebrew University of Jerusalem, August 1-5, 2021.";
                            mess = mess + "<br/>In order to complete the submission of the session proposal, all lecturers are required to log into their personal accounts and submit the details of their lecture proposal until December 1, 2020.";
                            mess = mess + "  Your Session Title is: " + arrInvited[2].SessionNameEnglish;
                            mess = mess + " <br/> Session Participants: ";
                            mess = mess + " <br/> Session chair: " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair;
                            mess = mess + " <br/>Lecturers: ";
                            mess = mess + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                            mess = mess + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                            mess = mess + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                            mess = mess + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                            mess = mess + "<br/>Your login details:";
                            mess = mess + "<br/>User name: " + arrInvited[4].Email;
                            mess = mess + "<br/>Password: " + PassCreator + "<br/></div>";
                            mail.Body = mail.Body + "<div style='direction: ltr;'>" + string.Format("For the lecture submission page  <a href='{0}'>click here:</a> ", loginUrl);
                            mess = mess + "<br/>The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021. <br/> ";
                            mess = mess + "<div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                            mess = mess + "Best regards,<br/>WUJS staff </div>";

                            mail.Body = mess;
                        }
                        //   mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                        mail.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchExeption(ex.ToString());
                }
                if (arrInvited[4].Email != arrInvited[0].ChairmanEmail && arrInvited[4].Email != arrInvited[1].ChairmanEmail && arrInvited[4].Email != arrInvited[2].ChairmanEmail && arrInvited[4].Email != arrInvited[3].ChairmanEmail)
                {
                    try
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            string massage = "";
                            var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                            mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                            //  mail.To.Add("tamy@bytes2story.com");
                            mail.To.Add(arrInvited[0].ChairmanEmail);
                            if (LangChairman == "English   ")
                            {
                                mail.Subject = "You have been added to a session";
                                massage = "<div style='direction: ltr;'>Dear  " + arrInvited[2].TitleChair + " " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair + "<br/>";
                                massage = massage + "<br/>" + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator;
                                massage = massage + "  invited you to  serve as chair of a session that was submitted for the 18th World Congress of Jewish Studies that will be held August 1-5, 2021 in Jerusalem.<br/>";
                                massage = massage + " Session details: <br/>";
                                massage = massage + "   Title : " + arrInvited[2].SessionNameEnglish;
                                massage = massage + " <br/> Session Participants: ";
                                massage = massage + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                                massage = massage + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                                massage = massage + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                                massage = massage + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                                massage = massage + "<br/>The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021.  <br/></div> ";
                                massage = massage + "<div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                                massage = massage + "Best regards,<br/>WUJS staff </div>";
                            }
                            else
                            if (LangChairman == "Hebrew   ")
                            {
                                mail.Subject = "מייל ליו'ר המושב ";
                                massage = "<div style='direction: rtl;' >שלום   " + selectedTitleChairman + " " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair + "<br/>";
                                massage = massage + "<br/>" + selectedTitleCreator + " " + FirstCreator + " " + LastNameHebrewCreator + " ";
                                massage = massage + " ביקש לצרף אותך כיושב ראש למושב שהוגש לקונגרס העולמי ה-18 למדעי היהדות שיתקיים בתאריכים 5-1 באוגוסט 2021, כ'ג אב-כ'ז אב תשפ'א.";
                                massage = massage + "<br/> פרטי המושב: ";
                                massage = massage + "<br/>  כותרת המושב היא: " + arrInvited[2].SessionName;
                                massage = massage + " <br/>משתתפים: ";
                                massage = massage + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                                massage = massage + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                                massage = massage + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                                massage = massage + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                                massage = massage + "<br/>החלטות בדבר קבלה או דחיה של הצעות מתקבלות על ידי הועדה האקדמית של האיגודהעולמי למדעי היהדות.תשובות ינתנו למרצים עד פברואר";
                                massage = massage + "<br/>אם המושב יתקבל, ישלח אליך הודעה עם בקשה להירשם לקונגרס.</div>";
                                massage = massage + "<div style='direction: rtl;'><br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                                massage = massage + "בברכה,<br/>צוות האיגוד</div>";
                            }
                            else
                            {
                                mail.Subject = "You have been added to a session";

                                massage = "<div style='direction: ltr;'>Dear  " + arrInvited[2].TitleChair + " " + arrInvited[2].FirstNameChair + " " + arrInvited[2].LastNameChair + "<br/>";
                                massage = massage + "<br/>" + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator;
                                massage = massage + " invited you to  serve as chair of a session that was submitted for the 18th World Congress of Jewish Studies that will be held August 1-5, 2021 in Jerusalem.<br/>";
                                massage = massage + " Session details: <br/>";
                                massage = massage + "   Title : " + arrInvited[2].SessionNameEnglish;
                                massage = massage + " <br/> Session Participants: ";
                                massage = massage + "<br/>" + arrInvited[0].FirstName + " " + arrInvited[0].LastName;
                                massage = massage + "<br/>" + arrInvited[1].FirstName + " " + arrInvited[1].LastName;
                                massage = massage + "<br/>" + arrInvited[2].FirstName + " " + arrInvited[2].LastName;
                                massage = massage + "<br/>" + arrInvited[3].FirstName + " " + arrInvited[3].LastName;
                                massage = massage + "<br/>The World Union of Jewish Studies’ academic committee will review and accept or decline lecture proposals. Lecturers will be informed of the committee’s decision by February 2021.  <br/></div> ";
                                massage = massage + "<div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                                massage = massage + "Best regards,<br/>WUJS staff </div>";


                            }

                            mail.Body = massage;
                            //   mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                            mail.To.Add(arrInvited[0].ChairmanEmail);
                            mail.IsBodyHtml = true;
                            using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                            {
                                smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                //smtp.EnableSsl = true;
                                smtp.Send(mail);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchExeption(ex.ToString());
                    }
                }





                conn.ConnectionString = connectionString;
                //int IdSession = GetLastIdSession();
                string sessionName = arrInvited[1].SessionName;
                string sqlText1;
                sqlText1 = "Insert into Sessions (SessionName,ABS) values(@SessionName,@ABS)";
                SqlCommand sqlCmd11 = new SqlCommand(sqlText1, conn);
                sqlCmd11.Parameters.AddWithValue("@SessionName", sessionName ?? (object)DBNull.Value);
                sqlCmd11.Parameters.AddWithValue("@ABS", arrInvited[0].ABS ?? (object)DBNull.Value);
                CatchExeption("line 3120 invite today");

                //int IdSession = GetLastIdSession() + 1;
                //string sqlText1 = "Insert into Sessions (Id,SessionName) values(@Id,@SessionName)";
                //SqlCommand sqlCmd11 = new SqlCommand(sqlText1, conn);
                //sqlCmd11.Parameters.AddWithValue("@Id", IdSession);
                //sqlCmd11.Parameters.AddWithValue("@SessionName", arrInvited[1].SessionName);
                conn.Open();
                int z12 = sqlCmd11.ExecuteNonQuery();
                conn.Close();
                CatchExeption("line 2130 invite today");

                for (int k = 0; k <= arrInvited.Length - 2 && arrInvited[k].Email != null; k++)
                {

                    int checkExists = checkEmailFind(arrInvited[k].Email);
                    if (checkExists == 1)
                    {
                        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        var stringChars = new char[8];
                        var random = new Random();

                        for (int j = 0; j < stringChars.Length; j++)
                        {
                            stringChars[j] = chars[random.Next(chars.Length)];
                        }

                        var finalString = new String(stringChars);
                        //                       string[] words = arrInvited[k].Name.Split(' ');

                        int Id1 = GetLastIdUserPass() + 1;
                        int IDUsers = GetLastIdUser() + 1;
                        conn.ConnectionString = connectionString;
                        //open count in userpass
                        sqlText1 = "Insert into UserPeass (Id,Password,Email) values(@Id,@Password,@Email)";
                        sqlCmd11 = new SqlCommand(sqlText1, conn);
                        sqlCmd11.Parameters.AddWithValue("@Id", Id1);
                        sqlCmd11.Parameters.AddWithValue("@Password", finalString);
                        sqlCmd11.Parameters.AddWithValue("@Email", arrInvited[k].Email);
                        conn.Open();
                        int z1 = sqlCmd11.ExecuteNonQuery();
                        conn.Close();
                        //open count in users
                        sqlText1 = "Insert into Users (Id,FirstNameEnglish,LastNameEnglish,Email,Title) values(@Id,@FirstNameEnglish,@LastNameEnglish,@Email,@Title)";
                        sqlCmd11 = new SqlCommand(sqlText1, conn);
                        sqlCmd11.Parameters.AddWithValue("@Id", IDUsers);
                        sqlCmd11.Parameters.AddWithValue("@FirstNameEnglish", arrInvited[k].FirstName);
                        sqlCmd11.Parameters.AddWithValue("@LastNameEnglish", arrInvited[k].LastName);
                        sqlCmd11.Parameters.AddWithValue("@Email", arrInvited[k].Email);
                        sqlCmd11.Parameters.AddWithValue("@Title", arrInvited[k].Title ?? (object)DBNull.Value);

                        conn.Open();
                        //open new draft
                        int z11 = sqlCmd11.ExecuteNonQuery();
                        conn.Close();

                        int Id11;
                        string sqlText11;
                        Id11 = GetLastIdDraft() + 1;
                        conn.ConnectionString = connectionString;
                        sqlText11 = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,SessionName,Chairman,ChairmanEmail,SessionId,Creator) values(@Id,@UserName,@Division,@Subdivision,@SessionName,@Chairman,@ChairmanEmail,@SessionId,@Creator)";
                        SqlCommand sqlCmd1 = new SqlCommand(sqlText11, conn);
                        sqlCmd1.Parameters.AddWithValue("@Id", Id11);
                        sqlCmd1.Parameters.AddWithValue("@UserName", arrInvited[k].Email);
                        sqlCmd1.Parameters.AddWithValue("@Division", arrInvited[k].Division);
                        sqlCmd1.Parameters.AddWithValue("@SubDivision", arrInvited[k].SubDivision);
                        sqlCmd1.Parameters.AddWithValue("@SessionName", arrInvited[k].SessionName);
                        sqlCmd1.Parameters.AddWithValue("@SessionNameEnglish", arrInvited[k].SessionNameEnglish);
                        sqlCmd1.Parameters.AddWithValue("@Chairman", arrInvited[k].FirstNameChair + "  " + arrInvited[k].LastNameChair);
                        sqlCmd1.Parameters.AddWithValue("@ChairmanEmail", arrInvited[k].ChairmanEmail);
                        sqlCmd1.Parameters.AddWithValue("@SessionId", IdSession);
                        sqlCmd1.Parameters.AddWithValue("@Creator", CreatorSession);
                        conn.Close();
                        conn.Open();
                        int z = sqlCmd1.ExecuteNonQuery();
                        conn.Close();


                        //SqlDataReader reader = null;
                        //SqlConnection myConnection = new SqlConnection();
                        myConnection.ConnectionString = connectionString;
                        //SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;


                        //sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish,selectedTitle,LastNameEnglish,LastNameHebrew FROM Users  where Email ='" + arrInvited[k].Email + "'";
                        //sqlCmd.Connection = myConnection;
                        //myConnection.Open();
                        //reader = sqlCmd.ExecuteReader();
                        //while (reader.Read())
                        //{
                        //    Lang = Convert.ToString(reader.GetValue(0));
                        //    First = Convert.ToString(reader.GetValue(1));
                        //    FirstE = Convert.ToString(reader.GetValue(2));
                        //    selectedTitle= Convert.ToString(reader.GetValue(3));
                        //    LastNameEnglish = Convert.ToString(reader.GetValue(4));
                        //    LastNameHebrew = Convert.ToString(reader.GetValue(5));
                        //}
                        //myConnection.Close();

                        try
                        {
                            string message = "";
                            using (MailMessage mail = new MailMessage())
                            {
                                var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                                mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                //  mail.To.Add("tamy@bytes2story.com");
                                mail.To.Add(arrInvited[k].Email);

                                mail.Subject = "You have been added to a session";
                                message = "<div style='direction: ltr;'>Dear  " + arrInvited[k].Title + " " + arrInvited[k].FirstName + " " + arrInvited[k].LastName + "<br/>";
                                message = message + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator;
                                message = message + " invited you to join the session " + arrInvited[k].SessionNameEnglish;
                                message = message + "<br/> to be submitted for the 18th World Congress of Jewish Studies that will be held August 1 - 5, 2021 in Jerusalem.";
                                message = message + "<br/>In order to complete the submission of the session proposal, please enter the registration page and submit the details of your lecture proposal until December 1, 2020.";
                                message = message + "<br/>Your login details:";
                                message = message + "<br/>User name: " + arrInvited[k].Email;
                                message = message + "<br/>Password: " + finalString + "<br/>";
                                message = message + string.Format("Please submit your lecture proposal <a href='{0}'>here:</a> ", loginUrl);
                                message = message + "</div><div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                                message = message + "Best regards,<br/>WUJS staff </div>";

                                mail.Body = message;
                                //   mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                                mail.IsBodyHtml = true;
                                using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                                {
                                    smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                    //smtp.EnableSsl = true;
                                    smtp.Send(mail);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            CatchExeption(ex.ToString());
                        }

                    }

                    else
                    {
                        int IdProp = 0;
                        int Id11;
                        Id11 = GetLastIdDraft() + 1;
                        myConnection.ConnectionString = connectionString;
                        //SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.CommandText = "SELECT Id FROM ProposalDrafts  where UserName Like N'%" + arrInvited[k].Email + "%'";
                        sqlCmd.Connection = myConnection;
                        myConnection.Open();
                        reader = sqlCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            IdProp = Convert.ToInt32(reader.GetValue(0));

                        }
                        myConnection.Close();
                        conn.Close();
                        string sqlText1211;
                        conn.ConnectionString = connectionString;
                        if (IdProp == 0)
                        {
                            sqlText1211 = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,SessionName,Chairman,ChairmanEmail,SessionId,Creator) values(@Id,@UserName,@Division,@Subdivision,@SessionName,@Chairman,@ChairmanEmail,@SessionId,@Creator)";
                        }
                        else
                        {
                            sqlText1211 = "Update ProposalDrafts set Division=@Division , SubDivision=@SubDivision" +
                            ",SessionName=@SessionName , Chairman=@Chairman , ChairmanEmail=@ChairmanEmail ,SessionId=@SessionId,Creator=@Creator where" +
                            " UserName like N'%" + arrInvited[k].Email + "%'";
                        }
                        SqlCommand sqlCmd12 = new SqlCommand(sqlText1211, conn);
                        sqlCmd12.Parameters.AddWithValue("@Id", Id11);
                        sqlCmd12.Parameters.AddWithValue("@UserName", arrInvited[k].Email);
                        sqlCmd12.Parameters.AddWithValue("@Division", arrInvited[k].Division);
                        sqlCmd12.Parameters.AddWithValue("@SubDivision", arrInvited[k].SubDivision);
                        sqlCmd12.Parameters.AddWithValue("@SessionName", arrInvited[k].SessionName);
                        sqlCmd12.Parameters.AddWithValue("@SessionNameEnglish", arrInvited[k].SessionNameEnglish);
                        sqlCmd12.Parameters.AddWithValue("@Chairman", arrInvited[k].FirstNameChair + "  " + arrInvited[k].LastNameChair);
                        sqlCmd12.Parameters.AddWithValue("@ChairmanEmail", arrInvited[k].ChairmanEmail);
                        sqlCmd12.Parameters.AddWithValue("@SessionId", IdSession);
                        sqlCmd12.Parameters.AddWithValue("@Creator", CreatorSession);



                        conn.Open();
                        int z1 = sqlCmd12.ExecuteNonQuery();
                        conn.Close();
                        //SqlDataReader reader = null;
                        //SqlConnection myConnection = new SqlConnection();
                        myConnection.ConnectionString = connectionString;
                        //SqlCommand sqlCmd = new SqlCommand();
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.CommandText = "SELECT Language,FirstNameHebrew,FirstNameEnglish,selectedTitle,LastNameEnglish,LastNameHebrew FROM Users  where Email ='" + arrInvited[k].Email + "'";
                        sqlCmd.Connection = myConnection;
                        myConnection.Open();
                        reader = sqlCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Lang = Convert.ToString(reader.GetValue(0));
                            First = Convert.ToString(reader.GetValue(1));
                            FirstE = Convert.ToString(reader.GetValue(2));
                            selectedTitle = Convert.ToString(reader.GetValue(3));
                            LastNameEnglish = Convert.ToString(reader.GetValue(4));
                            LastNameHebrew = Convert.ToString(reader.GetValue(5));
                        }
                        myConnection.Close();
                        sqlCmd.CommandText = "SELECT Password FROM UserPeass  where Email ='" + arrInvited[k].Email + "'";
                        sqlCmd.Connection = myConnection;
                        myConnection.Open();
                        reader = sqlCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Pass = Convert.ToString(reader.GetValue(0));

                        }
                        myConnection.Close();
                        if (arrInvited[k].Email != arrInvited[4].Email)
                        {
                            try
                            {
                                using (MailMessage mail = new MailMessage())
                                {
                                    var loginUrl = "http://jewish-studies.b2story.com/UserPass/1";

                                    mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                                    //  mail.To.Add("tamy@bytes2story.com");
                                    mail.To.Add(arrInvited[k].Email);
                                    if (Lang == "English")
                                    {
                                        mail.Subject = "You have been added to a session";
                                        mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                                        mail.Body = mail.Body + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator;
                                        mail.Body = mail.Body + "  invited you to join the session " + arrInvited[k].SessionNameEnglish;
                                        mail.Body = mail.Body + "<br/> to be submitted for the 18th World Congress of Jewish Studies that will be held August 1 - 5, 2021 in Jerusalem.";
                                        mail.Body = mail.Body + "<br/>In order to complete the submission of the session proposal, please enter the registration page and submit the details of your lecture proposal until December 1, 2020.";
                                        mail.Body = mail.Body + "<br/>Your login details:";
                                        mail.Body = mail.Body + "<br/>User name: " + arrInvited[k].Email;
                                        mail.Body = mail.Body + "<br/>Password: " + Pass + "<br/>";
                                        mail.Body = mail.Body + string.Format("Please submit your lecture proposal <a href='{0}'>here:</a> ", loginUrl);
                                        mail.Body = mail.Body + "</div><div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                                        mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";
                                    }
                                    else
                                    if (Lang == "Hebrew")
                                    {
                                        mail.Subject = "צורפת למושב בקונגרס העולמי למדעי היהדות";
                                        mail.Body = "<div >שלום   " + selectedTitle + " " + First + " " + LastNameHebrew + "<br/>";
                                        mail.Body = mail.Body + selectedTitleCreator + " " + FirstCreator + " " + LastNameHebrewCreator;
                                        mail.Body = mail.Body + "צירף אותך למושב " + arrInvited[k].SessionName;
                                        mail.Body = mail.Body + "<br/> לקונגרס העולמי ה-18 למדעי היהדות שיתקיים בתאריכים 5-1 באוגוסט 2021, כ'ג אב-כ'ז אב תשפ'א. ";
                                        mail.Body = mail.Body + "<br/>על מנת להשלים את תהליך ההגשה, עליך להיכנס לחשבונך באתר האיגוד העולמי למדעי היהדות ולהגיש את הצעתך להרצאה עד ה1 בדצמבר 2020.";
                                        mail.Body = mail.Body + "<br/>להלן פרטי כניסה לחשבונך:";
                                        mail.Body = mail.Body + "<br/>שם משתמש: " + arrInvited[k].Email;
                                        mail.Body = mail.Body + "<br/>סיסמה: " + Pass + "<br/>";
                                        mail.Body = mail.Body + string.Format("קישור למערכת ההגשות <a href='{0}'>כאן:</a> ", loginUrl);

                                        mail.Body = mail.Body + "</div><div style='direction: rtl;'><br/> לכל שאלה ניתן לפנות אלינו <br/><br/> ";
                                        mail.Body = mail.Body + "בברכה,<br/>צוות האיגוד</div>";
                                    }
                                    else
                                    {
                                        mail.Subject = "You have been added to a session";
                                        mail.Body = "<div style='direction: ltr;'>Dear  " + selectedTitle + " " + FirstE + " " + LastNameEnglish + "<br/>";
                                        mail.Body = mail.Body + selectedTitleCreator + " " + FirstECreator + " " + LastNameEnglishCreator;
                                        mail.Body = mail.Body + "  invited you to join the session " + arrInvited[k].SessionNameEnglish;
                                        mail.Body = mail.Body + "<br/> to be submitted for the 18th World Congress of Jewish Studies that will be held August 1 - 5, 2021 in Jerusalem.";
                                        mail.Body = mail.Body + "<br/>In order to complete the submission of the session proposal, please enter the registration page and submit the details of your lecture proposal until December 1, 2020.";
                                        mail.Body = mail.Body + "<br/>Your login details:";
                                        mail.Body = mail.Body + "<br/>User name: " + arrInvited[k].Email;
                                        mail.Body = mail.Body + "<br/>Password: " + Pass + "<br/>";
                                        mail.Body = mail.Body + string.Format("Please submit your lecture proposal <a href='{0}'>here:</a> ", loginUrl);
                                        mail.Body = mail.Body + "</div><div style='direction: ltr;'><br/> Feel free to contact us if you have any questions. <br/><br/> ";
                                        mail.Body = mail.Body + "Best regards,<br/>WUJS staff </div>";
                                    }
                                    //   mail.Body = mail.Body + "<br/><span>You must first fill in all your details and then you will be moved to fulfill your offer.</span>";
                                    mail.IsBodyHtml = true;
                                    using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                                    {
                                        smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                                        //smtp.EnableSsl = true;
                                        smtp.Send(mail);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                CatchExeption(ex.ToString());
                            }
                        }
                    }
                    conn.Close();
                }

                //int Id = GetLastIdDraft() + 1;
                //conn.ConnectionString = connectionString;
                //string sqlText = "Insert into ProposalDrafts (Id,UserName,Division,SubDivision,SessionName,Chairman,ChairmanEmail) values(@Id,@UserName,@Division,@SubDivision,@SessionName,@Chairman,@ChairmanEmail )";
                //SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                //sqlCmd.Parameters.AddWithValue("@Id", Id);
                //sqlCmd.Parameters.AddWithValue("@UserName", arrInvited[3].Email);
                //sqlCmd.Parameters.AddWithValue("@Division", arrInvited[1].Division);
                //sqlCmd.Parameters.AddWithValue("@SubDivision", arrInvited[1].SubDivision);

                //sqlCmd.Parameters.AddWithValue("@SessionName", arrInvited[1].SessionName);
                //sqlCmd.Parameters.AddWithValue("@Chairman", arrInvited[1].ChairmanName);
                //sqlCmd.Parameters.AddWithValue("@ChairmanEmail", arrInvited[1].ChairmanEmail);


                //conn.Open();
                //int i = sqlCmd.ExecuteNonQuery();
                //conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }
        }

        [Route("api/Home/checkEmailFind")]
        [HttpGet]
        public int checkEmailFind(string email)
        {

            int ID;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select Id from  UserPeass where Email Like N'%" + email + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader.GetValue(0));
                    myConnection.Close();
                    return ID;

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return 1;

        }


        [Route("api/Home/GetJudgeInterface")]
        [HttpGet]
        public List<JudgesInterface> GetJudgeInterface(Invited[] arrInvited)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<JudgesInterface> judges = new List<JudgesInterface>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "SELECT  a.UserName, a.Division, a.SubDivision, a.TitleEnglish," +
                                      "a.TitleHebrew, a.Proposal , a.Language, a.Keywords, " +
                                      "a.SessionName, a.Chairman, Judges.Status, Judges.Remarks, Judges.IdProposal," +
                                       "Users.Email, Users.FirstNameEnglish AS FirstName," +
                                       "Users.LastNameEnglish AS LastName , a.SessionId," +
                                       "(select count(SessionId) from Proposal where SessionId = a.SessionId and SessionId!=0) as NumOfProposals" +
                                        " FROM Proposal  a INNER JOIN" +
                                        " Judges ON a.Id = Judges.IdProposal INNER JOIN" +
                                        " Users ON a.UserName = Users.Email";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    int q = 0;
                    q = rdr.GetInt32(12);
                    string a = "";
                    a = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                    string b = "";
                    b = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    string c = "";
                    c = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                    string d = "";
                    d = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                    string e = "";
                    e = rdr.IsDBNull(4) ? null : rdr.GetString(4);
                    string h = "";
                    h = rdr.IsDBNull(5) ? null : rdr.GetString(5);
                    string i = "";
                    i = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    string j = "";
                    j = rdr.IsDBNull(7) ? null : rdr.GetString(7);
                    string k = "";
                    k = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    string l = "";
                    l = rdr.IsDBNull(9) ? null : rdr.GetString(9);
                    string m = "";
                    m = rdr.IsDBNull(10) ? null : rdr.GetString(10);
                    string n = "";
                    n = rdr.IsDBNull(11) ? null : rdr.GetString(11);
                    string o = "";
                    o = rdr.IsDBNull(14) ? null : rdr.GetString(14);
                    string p = "";
                    p = rdr.IsDBNull(15) ? null : rdr.GetString(15);
                    int se = 0;
                    se = rdr.IsDBNull(16) ? 0 : rdr.GetInt32(16);
                    int pnum = 0;
                    pnum = rdr.IsDBNull(17) ? 0 : rdr.GetInt32(17);
                    var r = new webApi.classes.JudgesInterface
                    {
                        IdProposal = q,
                        UserName = a,
                        Division = b,
                        SubDivision = c,
                        TitleEnglish = d,
                        TitleHebrew = e,
                        Proposal = h,
                        Language = i,
                        Keywords = j,
                        SessionName = k,
                        Chairman = l,
                        Status = m,
                        Remarks = n,
                        FirstNameEnglish = o,
                        LastNameEnglish = p,
                        SessionId = se,
                        NumOfProposals = pnum
                    };
                    judges.Add(r);

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return judges;
        }

        [Route("api/Home/GetDrafts")]
        [HttpGet]
        public List<Drafts> GetDrafts(Invited[] arrInvited)
        {
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<Drafts> judges = new List<Drafts>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select   * from W_DraftsNotProposal order by SessionName";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    string a = "";
                    a = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                    string b = "";
                    b = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    string c = "";
                    c = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                    string d = "";
                    d = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                    string e = "";
                    e = rdr.IsDBNull(4) ? null : rdr.GetString(4);
                    string h = "";
                    h = rdr.IsDBNull(5) ? null : rdr.GetString(5);
                    string i = "";
                    i = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    string j = "";
                    j = rdr.IsDBNull(7) ? null : rdr.GetString(7);
                    string k = "";
                    k = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    string l = "";
                    l = rdr.IsDBNull(9) ? null : rdr.GetString(9);
                    string o = "";
                    o = rdr.IsDBNull(10) ? null : rdr.GetString(10);
                    string p = "";
                    p = rdr.IsDBNull(11) ? null : rdr.GetString(11);
                    var r = new webApi.classes.Drafts
                    {
                        UserName = a,
                        Division = b,
                        SubDivision = c,
                        TitleEnglish = d,
                        TitleHebrew = e,
                        Proposal = h
                        ,
                        Language = i,
                        Keywords = j,
                        SessionName = k,
                        Chairman = l,
                        FirstNameEnglish = o,
                        LastNameEnglish = p
                    };
                    judges.Add(r);

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return judges;
        }




        [Route("api/Home/GetJudgeInterfaceBiIdProp")]
        [HttpGet]
        public JudgesInterface GetJudgeInterfaceBiIdProp(int Id)
        {
            JudgesInterface r = new webApi.classes.JudgesInterface { };
            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<JudgesInterface> judges = new List<JudgesInterface>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select   * from W_JudgesInterface where IdProposal=" + Id;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();

                while (rdr.Read())
                {
                    int q = 0;
                    q = rdr.GetInt32(12);
                    string a = "";
                    a = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                    string b = "";
                    b = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    string c = "";
                    c = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                    string d = "";
                    d = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                    string e = "";
                    e = rdr.IsDBNull(4) ? null : rdr.GetString(4);
                    string h = "";
                    h = rdr.IsDBNull(5) ? null : rdr.GetString(5);
                    string i = "";
                    i = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    string j = "";
                    j = rdr.IsDBNull(7) ? null : rdr.GetString(7);
                    string k = "";
                    k = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    string l = "";
                    l = rdr.IsDBNull(9) ? null : rdr.GetString(9);
                    string m = "";
                    m = rdr.IsDBNull(10) ? null : rdr.GetString(10);
                    string n = "";
                    n = rdr.IsDBNull(11) ? null : rdr.GetString(11);
                    string o = "";
                    o = rdr.IsDBNull(14) ? null : rdr.GetString(14);
                    string p = "";
                    p = rdr.IsDBNull(15) ? null : rdr.GetString(15);
                    r = new webApi.classes.JudgesInterface
                    {
                        IdProposal = q,
                        UserName = a,
                        Division = b,
                        SubDivision = c,
                        TitleEnglish = d,
                        TitleHebrew = e,
                        Proposal = h
                       ,
                        Language = i,
                        Keywords = j,
                        SessionName = k,
                        Chairman = l,
                        Status = m,
                        Remarks = n,
                        FirstNameEnglish = o,
                        LastNameEnglish = p
                    };

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return r;
        }

        [Route("api/Home/getId_W_ProposalsSession")]
        [HttpGet]
        public List<JudgesInterface> getId_W_ProposalsSession(int sessionId)
        {
            JudgesInterface r = new webApi.classes.JudgesInterface { };
            List<JudgesInterface> rArr = new List<JudgesInterface>();

            SqlDataReader rdr = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;
            List<JudgesInterface> judges = new List<JudgesInterface>();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select   * from W_JudgesInterface where SessionId = " + sessionId;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                rdr = sqlCmd.ExecuteReader();

                while (rdr.Read())
                {
                    int q = 0;
                    q = rdr.GetInt32(12);
                    string a = "";
                    a = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                    string b = "";
                    b = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    string c = "";
                    c = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                    string d = "";
                    d = rdr.IsDBNull(3) ? null : rdr.GetString(3);
                    string e = "";
                    e = rdr.IsDBNull(4) ? null : rdr.GetString(4);
                    string h = "";
                    h = rdr.IsDBNull(5) ? null : rdr.GetString(5);
                    string i = "";
                    i = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    string j = "";
                    j = rdr.IsDBNull(7) ? null : rdr.GetString(7);
                    string k = "";
                    k = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    string l = "";
                    l = rdr.IsDBNull(9) ? null : rdr.GetString(9);
                    string m = "";
                    m = rdr.IsDBNull(10) ? null : rdr.GetString(10);
                    string n = "";
                    n = rdr.IsDBNull(11) ? null : rdr.GetString(11);
                    string o = "";
                    o = rdr.IsDBNull(14) ? null : rdr.GetString(14);
                    string p = "";
                    p = rdr.IsDBNull(15) ? null : rdr.GetString(15);
                    int pp = 0;
                    pp = rdr.IsDBNull(16) ? 0 : rdr.GetInt32(16);
                    r = new webApi.classes.JudgesInterface
                    {
                        IdProposal = q,
                        UserName = a,
                        Division = b,
                        SubDivision = c,
                        TitleEnglish = d,
                        TitleHebrew = e,
                        Proposal = h,
                        Language = i,
                        Keywords = j,
                        SessionName = k,
                        Chairman = l,
                        Status = m,
                        Remarks = n,
                        FirstNameEnglish = o,
                        LastNameEnglish = p,
                        SessionId = pp
                    };
                    rArr.Add(r);

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return rArr;
        }



        [Route("api/Home/GetValuesByField")]

        [HttpGet]
        public List<string> GetValuesByField(string field)
        {
            List<string> Values = new List<string>();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select " + field + " from W_JudgesInterface ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values.Add(reader.IsDBNull(0) ? null : reader.GetString(0));

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Values;
        }



        [Route("api/Home/GetValuesByFieldDrafts")]

        [HttpGet]
        public List<string> GetValuesByFieldDrafts(string field)
        {
            List<string> Values = new List<string>();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select " + field + " from W_DraftsNotProposal ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values.Add(reader.IsDBNull(0) ? null : reader.GetString(0));

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Values;
        }




        [Route("api/Home/UpdateProp")]

        [HttpPost]

        public void UpdateProp(JudgesInterface newprop)
        {

            try
            {
                conn.ConnectionString = connectionString;
                string sqlText = "Update Proposal set Division = @Division,  SubDivision = @SubDivision , SessionName = @SessionName";
                sqlText = sqlText + " , Chairman = @Chairman where Id = @Id";
                SqlCommand sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", newprop.IdProposal);
                sqlCmd.Parameters.AddWithValue("@Division", newprop.Division ?? (object)DBNull.Value‏);
                sqlCmd.Parameters.AddWithValue("@SubDivision", newprop.SubDivision ?? (object)DBNull.Value‏);
                sqlCmd.Parameters.AddWithValue("@SessionName", newprop.SessionName ?? (object)DBNull.Value‏);
                sqlCmd.Parameters.AddWithValue("@Chairman", newprop.Chairman ?? (object)DBNull.Value);

                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.Open();
                int i = sqlCmd.ExecuteNonQuery();
                //db.SaveChanges();
                conn.Close();

                sqlText = "Update Judges set Status = @Status,  Remarks = @Remarks where IdProposal = @Id";
                sqlCmd = new SqlCommand(sqlText, conn);
                sqlCmd.Parameters.AddWithValue("@Id", newprop.IdProposal);
                sqlCmd.Parameters.AddWithValue("@Status", newprop.Status ?? (object)DBNull.Value);
                sqlCmd.Parameters.AddWithValue("@Remarks", newprop.Remarks ?? (object)DBNull.Value);


                // sqlCmd.Parameters.AddWithValue("@UserName", userName);
                conn.Open();
                i = sqlCmd.ExecuteNonQuery();
                //db.SaveChanges();
                conn.Close();


            }
            catch (Exception ex)
            {
                conn.Close();
                CatchExeption(ex.ToString());
            }

        }


        [Route("api/Home/GetUserNameLogin")]

        [HttpGet]

        public string GetUserNameLogin(string LoginUserName)
        {

            return LoginUserName;


        }



        [Route("api/Home/getNumProduct")]

        [HttpGet]
        public int getNumProduct(string LoginUserName)
        {
            int num = 0;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select sum(Quantity) from ShoppingCart where UserName like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Close();

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    num = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return num;
        }

        [Route("api/Home/getUserDetails")]

        [HttpGet]
        public webApi.classes.User getUserDetails(string LoginUserName)
        {
            webApi.classes.User Values = new webApi.classes.User();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select FirstNameEnglish,LastNameEnglish," +
                    "FirstNameHebrew,LastNameHebrew,selectedTitle,selectedCountry,Address,MemberShip,Language,DateMember from  Users where Email like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values.FirstNameEnglish = reader.IsDBNull(0) ? null : reader.GetString(0);
                    Values.LastNameEnglish = reader.IsDBNull(1) ? null : reader.GetString(1);
                    Values.FirstNameHebrew = reader.IsDBNull(2) ? null : reader.GetString(2);
                    Values.LastNameHebrew = reader.IsDBNull(3) ? null : reader.GetString(3);
                    Values.selectedTitle = reader.IsDBNull(4) ? null : reader.GetString(4);
                    Values.selectedCountry = reader.IsDBNull(5) ? null : reader.GetString(5);
                    Values.Address = reader.IsDBNull(6) ? null : reader.GetString(6);
                    Values.MemberShip = reader.IsDBNull(7) ? -1 : reader.GetInt32(7);
                    if (reader.GetDateTime(8) != null)
                        Values.DateMember = reader.GetDateTime(8);
                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Values;
        }

        [Route("api/Home/getName")]

        [HttpGet]
        public Name getName(string LoginUserName)
        {
            Name Values = new Name();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select FirstNameEnglish,LastNameEnglish,FirstNameHebrew,LastNameHebrew,Title from  Users where Email like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values.FirstName = reader.IsDBNull(0) ? null : reader.GetString(0);
                    Values.LastName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    Values.FirstNameHebrew = reader.IsDBNull(2) ? null : reader.GetString(2);
                    Values.LastNameHebrew = reader.IsDBNull(3) ? null : reader.GetString(3);
                    Values.selectedTitle = reader.IsDBNull(4) ? null : reader.GetString(4);

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Values;
        }

        [Route("api/Home/getNameHebrew")]

        [HttpGet]
        public Name getNameHebrew(string LoginUserName)
        {
            Name Values = new Name();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select FirstNameHebrew,LastNameHebrew from  Users where Email like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values.FirstName = reader.IsDBNull(0) ? null : reader.GetString(0);
                    Values.LastName = reader.IsDBNull(1) ? null : reader.GetString(1);

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return Values;
        }

        [Route("api/Home/CheckIfInSession")]

        [HttpGet]
        public int CheckIfInSession(string LoginUserName)
        {
            string Values = "";
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select SessionName from  ProposalDrafts where UserName like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    Values = reader.IsDBNull(0) ? null : reader.GetString(0);

                }
                myConnection.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            if (Values == null)
                return 1;
            else
                return 2;
        }



        [Route("api/Home/CheckIfInDraft")]

        [HttpGet]
        public int CheckIfInDraft(string LoginUserName)
        {
            Proposals r = new Proposals();
            int flag = 0;
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select * from  ProposalDrafts where UserName like N'%" + LoginUserName + "%'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    r.Division = reader.IsDBNull(2) ? null : reader.GetString(2);
                    r.SubDivision = reader.IsDBNull(3) ? null : reader.GetString(3);
                    r.TitleEnglish = reader.IsDBNull(4) ? null : reader.GetString(4);
                    r.TitleHebrew = reader.IsDBNull(5) ? null : reader.GetString(5);
                    r.SessionName = reader.IsDBNull(9) ? null : reader.GetString(9);
                    r.SessionId = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);

                }
                myConnection.Close();
                if (r.SessionId != 0)
                {
                    flag = 1;
                }
                else
                    if (r.Division != null || r.SubDivision != null || r.TitleEnglish != null || r.TitleHebrew != null)
                {
                    flag = 2;
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return flag;
        }

        [Route("api/Home/getNameOfDraftsGroup")]

        [HttpGet]
        public List<string> getNameOfDraftsGroup(int sessionId)
        {
            List<string> names = new List<string>();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select [FirstNameEnglish],[LastNameEnglish],[FirstNameHebrew],[LastNameHebrew] from  W_NamesOfDrafts where [SessionId]= " + sessionId;
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    string a = reader.IsDBNull(0) ? null : reader.GetString(0);
                    a = a + " " + (string)(reader.IsDBNull(1) ? "" : reader.GetString(1));
                    a = a + " | " + (string)(reader.IsDBNull(2) ? "" : reader.GetString(2));
                    a = a + " " + (string)(reader.IsDBNull(3) ? "" : reader.GetString(3));
                    names.Add(a);
                }
                myConnection.Close();


            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            return names;
        }



        [Route("api/Home/sendMail")]

        [HttpGet]
        public void sendMail()
        {
            List<UserPass> names = new List<UserPass>();
            var ab = new UserPass();
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = connectionString;

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.CommandText = "select Email,Password from  UserPeass ";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    string a = reader.IsDBNull(0) ? null : reader.GetString(0);
                    string b = reader.IsDBNull(1) ? null : reader.GetString(1);
                    ab.Email = a;
                    ab.Password = b;
                    names.Add(ab);
                }
                myConnection.Close();


            }
            catch (Exception ex)
            {
                conn.Close();
                myConnection.Close();
                CatchExeption(ex.ToString());
            }
            var i = 0;
            string MSG = "";
            while (i < names.Count)
            {
                try
                {
                    CatchExeption("line 980");

                    using (MailMessage mail = new MailMessage())
                    {

                        CatchExeption("line 986");
                        var loginUrl2 = "http://jewish-studies.b2story.com/UserPass/3/";
                        var loginUrl1 = "http://jewish-studies.b2story.com/UserPass/3/";
                        mail.From = new MailAddress("Worldunion.jewishstudies@b2story.com");
                        //  mail.To.Add("tamy@bytes2story.com");
                        mail.To.Add(names[1].Email);
                        mail.Subject = "פרטי כניסה שלך לאתר האיגוד Your Login Details to the WUJS website";
                        MSG = "<div style='direction: rtl;'>לחברי האיגוד שלום רב,  < br/>";
                        MSG = MSG + "במסגרת התחדשות האיגוד באתר חדש,  <br/>";
                        MSG = MSG + "נשמח לעדכן אתכם בפרטי המשתמש שלכם לרכישה בחנות הספרים של האיגוד. <br/>";
                        MSG = MSG + "בהזנת פרטי המשתמש תוכלו להנות מ20% הנחה על שלל הספרים באתר, המגיעה לכם בתור חברי האיגוד.<br/>";
                        MSG = MSG + "שם המשתמש: " + names[i].Email + " <br/>";
                        MSG = MSG + "סיסמה: " + names[i].Password + " <br/>";
                        MSG = MSG + "הנחה תשמש אתכם כמובן גם לרכישת ספרי האיגוד החדשים העתידים לצאת בשנה הקרובה,  <br/>";
                        MSG = MSG + "עליהם נעדכן בנפרד ברגע שאלה יצאו לאור. <br/>";
                        MSG = MSG + "לכל בעיה או בירור,  <br/>";
                        MSG = MSG + "אנו זמינים לשירותכם בכתובת המייל: <br/>";
                        MSG = MSG + "worldunion.jewishstudies@gmail.com < br />";
                        MSG = MSG + "איחולי שנה בריאה ופורייה. <br/>";
                        MSG = MSG + "צוות האיגוד. <br/></div>";
                        MSG = MSG + "<div style='direction: ltr;'>Dear WUJS members,  < br/>";
                        MSG = MSG + "With the launch of our new website, we would like to send you your login details for purchases in our new online bookstore.  <br/>";
                        MSG = MSG + "With those details, you can identify yourself as a member and receive a 20% discount on every purchase and all books sold on the site.  <br/>";
                        MSG = MSG + "Your username: " + names[i].Email + " <br/>";
                        MSG = MSG + "Your password: " + names[i].Password + " <br/>";
                        MSG = MSG + "The discount will also be valid for any new books published throughout the year.  <br/>";
                        MSG = MSG + "We will update you about any new publications. <br/>";
                        MSG = MSG + "For any questions or inquiries, please do not hesitate to contact us:  <br/>";
                        MSG = MSG + "worldunion.jewishstudies@gmail.com < br />";
                        MSG = MSG + "With best wishes for a healthy and prosperous New Year, <br/>";
                        MSG = MSG + "WUJS staff. <br/></div>";




                        mail.Body = MSG;
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("91.205.154.240", 25))
                        {
                            CatchExeption(" line 1012");
                            smtp.Credentials = new System.Net.NetworkCredential("Worldunion.jewishstudies@b2story.com", "Jweishstudies99");
                            //smtp.EnableSsl = true;
                            smtp.Send(mail);
                            CatchExeption(" line 1389");


                        }

                    }
                }
                catch (Exception ex)
                {
                    CatchExeption(ex.ToString() + " line 1023");
                }
                i++;
            }
        }



    }

}

