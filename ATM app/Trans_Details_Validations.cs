using System.Data.SqlClient;

namespace ATM_app
{
    public class Trans_Details_Validations
    {
        public SqlConnection SQLCon()
        {
            string conString = "Data Source=localhost;Initial Catalog=ATM_MACHINE;Integrated Security=True";
            SqlConnection con = new SqlConnection(conString);
            return con;
        }
        public bool ValidateCardPIN(string card_No, string PINstr)
        {           
            SqlConnection checkDetailsCon = SQLCon();

            string Val_Card_No_PIN = "Execute Validate_PINAndCard @Card_No";
            checkDetailsCon.Open();


            SqlCommand checkDetailsCmd = new SqlCommand(Val_Card_No_PIN, checkDetailsCon);
            checkDetailsCmd.Parameters.AddWithValue("@Card_No", card_No);
            SqlDataReader reader = checkDetailsCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if(reader[0].ToString() == card_No && reader[1].ToString() == PINstr)
                    {
                        return true;
                    }  
                }
            }
            return false;
        }


        public double GetBalance(string card_Number)
        {
            SqlConnection getBalanceCon = SQLCon();
            string getBalance = "Execute SP_GetBalance @Card_No";
            SqlCommand getBalanceCmd = new SqlCommand(getBalance, getBalanceCon);

            getBalanceCon.Open();

            getBalanceCmd.Parameters.AddWithValue("@Card_No", card_Number);            
            SqlDataReader balanceReader = getBalanceCmd.ExecuteReader();

            double balance = 0;
            if (balanceReader.HasRows)
            {
                while (balanceReader.Read())
                {
                    balance = Convert.ToDouble(balanceReader[0]);                    
                }
                return balance;
            }
            return balance;
        }


        public double CheckEnoughBalance(string card_Number, double amount)
        {
            double accBalance = GetBalance(card_Number);
            double amountToWithdraw = amount;
            if(amountToWithdraw <= accBalance)
            {
                return accBalance;
            }
            return 0;
        }

        public bool InsertTransaction(string card_Number, string card_Id,string Trans_Type)
        {
            int UserID = 0;
            int Acc_No = 0;
            SqlConnection ITcon = SQLCon();
            string ITquery = "exec SP_GetUserIdAcc_No @Card_No";
            SqlCommand ITcmd = new SqlCommand(ITquery, ITcon);
            ITcmd.Parameters.AddWithValue("@Card_No", card_Number);
            ITcon.Open();
            SqlDataReader reader1 = ITcmd.ExecuteReader();
            if (reader1.HasRows)
            {
                while (reader1.Read())
                {
                    UserID = int.Parse(reader1[0].ToString());
                    Acc_No = int.Parse(reader1[1].ToString());
                }
            }

            ITcon.Close();

            string query2 = "exec SP_InsertTransaction @Trans_Type, @UserID, @Card_Id, @Acc_No";
            SqlCommand cmd2 = new SqlCommand(query2, ITcon);
            ITcon.Open();
            cmd2.Parameters.AddWithValue("@Trans_Type", Trans_Type);
            cmd2.Parameters.AddWithValue("@UserID", UserID);
            cmd2.Parameters.AddWithValue("@Card_Id", card_Id);
            cmd2.Parameters.AddWithValue("@Acc_No", Acc_No);            
            int noOfRowsInserted = cmd2.ExecuteNonQuery();
            ITcon.Close();
            if (noOfRowsInserted > 0)
            {
                return true;
            }
            return false;
            
        }

        public bool WithdrawMoney(string card_Number, double amount, double DBBalance)
        {
            SqlConnection withdrawMoneyCon = SQLCon();
            string withdrawMoney = "Execute SP_UpdateBalance @Card_No, @Balance";
            SqlCommand withdrawCmd = new SqlCommand(withdrawMoney, withdrawMoneyCon);
            withdrawMoneyCon.Open();
            withdrawCmd.Parameters.AddWithValue("@Card_No", card_Number);
            double updatedBalance = DBBalance - amount;
            withdrawCmd.Parameters.AddWithValue("@Balance", updatedBalance);
            int noOfRowsUpdated = withdrawCmd.ExecuteNonQuery();
            if(noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }


        public bool DepositMoney(string card_Number,double amount,double DBBalance)
        {
            SqlConnection withdrawMoneyCon = SQLCon();
            string withdrawMoney = "Execute SP_UpdateBalance @Card_No, @Balance";
            SqlCommand withdrawCmd = new SqlCommand(withdrawMoney, withdrawMoneyCon);
            withdrawMoneyCon.Open();
            withdrawCmd.Parameters.AddWithValue("@Card_No", card_Number);
            double updatedBalance = DBBalance + amount;
            withdrawCmd.Parameters.AddWithValue("@Balance", updatedBalance);
            int noOfRowsUpdated = withdrawCmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }


        public List<string> GetPrevTrans(string card_Number)
        {
            SqlConnection GetPrevTranscon = SQLCon();
            string GetPrevTransquery = "Exec SP_GetTransactions @Card_No";
            SqlCommand GetPrevTranscmd = new SqlCommand(GetPrevTransquery, GetPrevTranscon);

            GetPrevTranscon.Open();
            GetPrevTranscmd.Parameters.AddWithValue("@Card_No", card_Number);
            SqlDataReader reader = GetPrevTranscmd.ExecuteReader();
            List<string> output = new List<string>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    output.Add(reader[0].ToString());
                    output.Add(reader[1].ToString());
                    output.Add(reader[2].ToString());
                    output.Add(reader[3].ToString());
                    output.Add(reader[4].ToString());
                    output.Add(reader[5].ToString());            
                }
                return output;
            }
            else
            {
                return null;
            }
            
        }

        public string GetOldName(string card_Number)
        {
            string oldName = string.Empty;
            SqlConnection oldNameCon = SQLCon();
            string getOldNameQuery = "exec SP_GetOldName @Card_No";
            SqlCommand cmd = new SqlCommand(getOldNameQuery, oldNameCon);

            oldNameCon.Open();
            cmd.Parameters.AddWithValue("@Card_No", card_Number);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    oldName = reader[0].ToString();
                }               
            }
            return oldName;
        }


        public bool UpdateAccName(string card_Number, string newName)
        {
            SqlConnection updateNameCon = SQLCon();
            string updateNameQuery = "exec SP_UpdateFullName @FullName,@Card_No";
            SqlCommand updateNameCmd = new SqlCommand(updateNameQuery, updateNameCon);
            updateNameCmd.Parameters.AddWithValue("@FullName", newName);
            updateNameCmd.Parameters.AddWithValue("@Card_No", card_Number);
            updateNameCon.Open();
            int noOfRowsUpdated = updateNameCmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }


        public string GetOldAddress(string card_Number)
        {
            string address = "";
            SqlConnection getAddressCon = SQLCon();
            string query1 = "exec SP_GetOldAddress @Card_No";
            SqlCommand cmd = new SqlCommand(query1, getAddressCon);
            getAddressCon.Open();
            cmd.Parameters.AddWithValue("@Card_No", card_Number);           
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    address = Convert.ToString(reader[0]);
                }
            }
            return address;
        }


        public bool UpdateAccAddress(string card_Number, string newAddress)
        {
            SqlConnection updateAddressCon = SQLCon();
            string updateAddressQuery = "exec SP_UpdateAddress @Address,@Card_No";
            SqlCommand updateAddressCmd = new SqlCommand(updateAddressQuery, updateAddressCon);
            updateAddressCon.Open();
            updateAddressCmd.Parameters.AddWithValue("@Address", newAddress);
            updateAddressCmd.Parameters.AddWithValue("@Card_No", card_Number);            
            int noOfRowsUpdated = updateAddressCmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }



        public DateTime GetOldDOB(string card_Number)
        {
            DateTime oldDOB = new DateTime();
            SqlConnection getDOBCon = SQLCon();
            string getDOBQuery = "exec SP_GetOldDOB @Card_No";
            SqlCommand getDOBcmd = new SqlCommand(getDOBQuery, getDOBCon);
            getDOBcmd.Parameters.AddWithValue("@Card_No", card_Number);
            getDOBCon.Open();
            SqlDataReader reader = getDOBcmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    oldDOB = Convert.ToDateTime(reader[0]);
                }
            }
            return oldDOB;
        }


        public bool UpdateAccDOB(string card_Number, DateTime newDOB)
        {
            SqlConnection updateDOBCon = SQLCon();
            string updateDOBQuery = "exec SP_UpdateDOB @DateOfBirth,@Card_No";
            SqlCommand updateDOBCmd = new SqlCommand(updateDOBQuery, updateDOBCon);
            updateDOBCon.Open();
            updateDOBCmd.Parameters.AddWithValue("@DateOfBirth", newDOB);
            updateDOBCmd.Parameters.AddWithValue("@Card_No", card_Number);
            
            int noOfRowsUpdated = updateDOBCmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }



        public string GetOldPhNo(string card_Number)
        {
            string oldPhNo = string.Empty;
            SqlConnection getPhNoCon = SQLCon();
            string getDOBQuery = "exec SP_GetOldDOB @Card_No";
            SqlCommand getPhNocmd = new SqlCommand(getDOBQuery, getPhNoCon);
            getPhNocmd.Parameters.AddWithValue("@Card_No", card_Number);
            getPhNoCon.Open();
            SqlDataReader reader = getPhNocmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    oldPhNo = Convert.ToString(reader[0]);
                }
            }
            return oldPhNo;
        }


        public bool UpdatePhNo(string card_Number, string newPhNo)
        {
            
            SqlConnection updatePhNoCon = SQLCon();
            string updatePhNoQuery = "exec SP_SetPhNo @Ph_no,@Card_No";
            SqlCommand getPhNoCmd = new SqlCommand(updatePhNoQuery, updatePhNoCon);
            getPhNoCmd.Parameters.AddWithValue("@Ph_no", newPhNo);
            getPhNoCmd.Parameters.AddWithValue("@Card_No", card_Number);
            updatePhNoCon.Open();
            int noOfRowsUpdated = getPhNoCmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }
        

        public string GetOldEmailId(string card_Number)
        {
            string oldEmail = string.Empty;
            SqlConnection oldEmailCon = SQLCon();
            string getEmailQuery = "exec SP_GetEmailId @Card_No";
            SqlCommand GetOldEmamilCmd = new SqlCommand(getEmailQuery, oldEmailCon);

            oldEmailCon.Open();
            GetOldEmamilCmd.Parameters.AddWithValue("@Card_No", card_Number);
            SqlDataReader reader = GetOldEmamilCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    oldEmail = reader[0].ToString();
                }
            }
            return oldEmail;
        }


        public bool UpdateEmail(string card_Number, string newEmailId)
        {
            SqlConnection setEmailCon = SQLCon();
            string setEmailQuery = "exec SP_SetEmailId @EmailId,@Card_No";
            SqlCommand setEmailcmd = new SqlCommand(setEmailQuery, setEmailCon);
            setEmailcmd.Parameters.AddWithValue("@EmailId", newEmailId);
            setEmailcmd.Parameters.AddWithValue("@Card_No", card_Number);
            setEmailCon.Open();
            int noOfRowsUpdated = setEmailcmd.ExecuteNonQuery();
            if (noOfRowsUpdated > 0)
            {
                return true;
            }
            return false;
        }

    }


    

    



}
