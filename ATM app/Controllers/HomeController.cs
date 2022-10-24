using ATM_app.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ATM_app.Controllers
{
    public class HomeController : Controller
    {
        private Trans_Details_Validations tdvObj = new Trans_Details_Validations();
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        

        public IActionResult Index()
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

        [HttpPost]
        public bool GetCardDetails(string card_No, int PIN, string card_Type)
        {

            string PINStr = PIN.ToString();

            if (tdvObj.ValidateCardPIN(card_No, PINStr))
            {
                //this session is a predefined in MVC which can make a session for storing user
                //sensitive data and from here we an access it anywhere. same we are doing with card_id
                HttpContext.Session.SetString("card_No", card_No);                                
                HttpContext.Session.SetString("card_Id", card_Type);
                return true;
            }
            return false;                              
        }


        public IActionResult All_Transactions()
        {
            return View();
        }


        [HttpPost]
        public string WithdrawAmount(double withdrawAmount)
        {
            string output = string.Empty;
            //we are accessing global variable card_Number which we have made temporarily here
            //just to move forward but we need card_No
            //how to access the card_No here????

            string card_Number = HttpContext.Session.GetString("card_No");
            string card_Id = HttpContext.Session.GetString("card_Id");

            double DBBalance = tdvObj.CheckEnoughBalance(card_Number, withdrawAmount);
            if (DBBalance!=0)
            {
                if (tdvObj.WithdrawMoney(card_Number, withdrawAmount, DBBalance))
                {
                    string Trans_Type = "Withdrawal";
                    bool IsIT = tdvObj.InsertTransaction(card_Number, card_Id, Trans_Type);
                    if (IsIT)
                    {
                       return output = "Withdraw amount succesfull and Transaction  updated in history.";
                    }
                    return output = "Deposition succesfull but Trnsaction was updated in history. Please contact your administrator for this.";
                }
                return output = "Withdrawal Unsuccesfull";
            }
            return output = "Not enough balance present in your account. Deposit some amount first!!";                       
        }


        



        [HttpPost]
        public string DepositAmount(double amount)
        {
            string output = string.Empty;
            string card_Number = HttpContext.Session.GetString("card_No");
            string card_Id = HttpContext.Session.GetString("card_Id");

            double DBBalance = tdvObj.GetBalance(card_Number);
            if (tdvObj.DepositMoney(card_Number, amount, DBBalance))
            {
                string Trans_Type = "Deposition";
                bool IsIT = tdvObj.InsertTransaction(card_Number, card_Id, Trans_Type);
                if (IsIT)
                {
                    return output = "Deposition of amount succesfull and Transaction  updated in history.";
                }
                return output = "Deposition succesfull but Trnsaction was updated in history. Please contact your administrator for this..";
            }
            return output = "Deposition Unsuccesfull";
        }


        


        public IActionResult _CheckBalance()
        {
            string output = string.Empty;
            string card_Number = HttpContext.Session.GetString("card_No");
            double balance = tdvObj.GetBalance(card_Number);
            ViewBag.BalanceResult = balance;
            return PartialView();
        }


        //[HttpGet]
        //public IActionResult PrevTransactionsView()  //returning full view
        //{
        //    List<string> output = new List<string>();
        //    string card_Number = HttpContext.Session.GetString("card_No");
        //    output = tdvObj.GetPrevTrans(card_Number);
        //    ViewData["Prev5Trans"] = output;
        //    return View();                      
        //}

        public IActionResult _PrevTransactionsView()
        {
            List<string> output = new List<string>();
            string card_Number = HttpContext.Session.GetString("card_No");
            output = tdvObj.GetPrevTrans(card_Number);
            ViewData["Prev5Trans"] = output;
            return PartialView();
        }




        [HttpPost]
        public bool UpdateName(string newName)
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            return tdvObj.UpdateAccName(card_Number, newName);
        }


        [HttpPost]
        public string ShowOldName()
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            string oldName = tdvObj.GetOldName(card_Number);
            return oldName;
        }


        [HttpPost]
        public string ShowOldAddress()
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            string oldAddress = tdvObj.GetOldAddress(card_Number);
            return oldAddress;
        }


        [HttpPost]
        public bool UpdateAddress(string newAddress)
        {           
            string card_Number = HttpContext.Session.GetString("card_No");
            return tdvObj.UpdateAccAddress(card_Number, newAddress);
        }

        [HttpPost]
        public DateTime ShowOldDOB()
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            DateTime oldDOB = tdvObj.GetOldDOB(card_Number);
            return oldDOB;
        }


        [HttpPost]
        public bool UpdateDOB(DateTime newDOB)
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            return tdvObj.UpdateAccDOB(card_Number, newDOB);
        }


        [HttpPost]
        public string ShowPhNo()
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            string oldPhNo = tdvObj.GetOldPhNo(card_Number);
            return oldPhNo;
        }


        [HttpPost]
        public bool UpdatePhNo(string newPhNo)
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            return tdvObj.UpdatePhNo(card_Number, newPhNo);
        }


        [HttpPost]
        public string ShowEmailId()
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            string oldEmailId = tdvObj.GetOldEmailId(card_Number);
            return oldEmailId;
        }


        [HttpPost]
        public bool UpdateEmailId(string newEmailId)
        {
            string card_Number = HttpContext.Session.GetString("card_No");
            return tdvObj.UpdateEmail(card_Number, newEmailId);
        }
    }

}