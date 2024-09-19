using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using static Tayyab_Cloth_House.LoginForm;
using static Tayyab_Cloth_House.Dashboard;

namespace Tayyab_Cloth_House
{
    public partial class ProfitDistribution : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;
       
        private List<Investor> investors = new List<Investor>();
        public class Investor
        {
            public string Name { get; set; }
            public decimal InvestmentAmount { get; set; }

            public Investor(string name, decimal investmentAmount)
            {
                Name = name;
                InvestmentAmount = investmentAmount;
            }
        }
        public ProfitDistribution()
        {
            InitializeComponent();
            ShowProfit();


        }
        public decimal GetTotalSales()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    // Query to get the most recent profit distribution date
                    string lastProfitDateQuery = "SELECT MAX(pdate) FROM profitDistribution";
                    object result = new SqlCommand(lastProfitDateQuery, sqlConnection).ExecuteScalar();

                    string salesQuery;
                    SqlCommand salesCommand;

                    DateTime referenceDateTime;

                    if (result != DBNull.Value && result != null)
                    {
                        referenceDateTime = Convert.ToDateTime(result);
                    }
                    else
                    {
                        // Set a valid fallback date within SQL Server DATETIME range
                        referenceDateTime = new DateTime(1753, 1, 1); // January 1, 1753
                    }

                    // Query to calculate the total sales after the last profit calculation date
                    salesQuery = "SELECT SUM(total) FROM sale WHERE date >= @ReferenceDateTime";
                    salesCommand = new SqlCommand(salesQuery, sqlConnection);
                    salesCommand.Parameters.AddWithValue("@ReferenceDateTime", referenceDateTime);

                    var totalSalesResult = salesCommand.ExecuteScalar();
                    return totalSalesResult != DBNull.Value ? Convert.ToDecimal(totalSalesResult) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // Return a default value on error
            }
        }

        public decimal GetTotalExpenses()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    // Query to get the most recent profit distribution date
                    string lastProfitDateQuery = "SELECT MAX(pdate) FROM profitDistribution";
                    object result = new SqlCommand(lastProfitDateQuery, sqlConnection).ExecuteScalar();

                    string expenseQuery;
                    SqlCommand expenseCommand;

                    DateTime referenceDateTime;

                    if (result != DBNull.Value && result != null)
                    {
                        referenceDateTime = Convert.ToDateTime(result);
                    }
                    else
                    {
                        // Set a valid fallback date within SQL Server DATETIME range
                        referenceDateTime = new DateTime(1753, 1, 1); // January 1, 1753
                    }

                    // Query to calculate the total expenses after the last profit calculation date
                    expenseQuery = "SELECT SUM(expenseP) FROM sale WHERE date >= @ReferenceDateTime";
                    expenseCommand = new SqlCommand(expenseQuery, sqlConnection);
                    expenseCommand.Parameters.AddWithValue("@ReferenceDateTime", referenceDateTime);

                    var totalExpenseResult = expenseCommand.ExecuteScalar();
                    return totalExpenseResult != DBNull.Value ? Convert.ToDecimal(totalExpenseResult) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // Return a default value on error
            }
        }

        public void ShowProfit()
        {
            // Calculate total profit
            decimal totalSales = GetTotalSales();
            decimal totalExpenses = GetTotalExpenses();
            decimal totalProfit = totalSales - totalExpenses;

            // Display profit
            weeklyprofitprice.Text = totalProfit > 0 ? totalProfit.ToString() : "0";
        }


        private void ProfitDsitribution_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(weeklyprofitprice.Text, out decimal weeklyProfit) || weeklyProfit <= 0)
            {
                MessageBox.Show("Weekly profit must be a positive non-zero value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            else
            {

                // Calculate 50% of the weekly profit for the shop and for distribution
            decimal shopProfit = weeklyProfit * 0.50m;
            decimal investmentProfit = weeklyProfit * 0.50m;

            decimal totalprofit = Convert.ToDecimal(weeklyprofitprice.Text);

            // Fetch investors from the database and calculate total investment
            investors.Clear();
            decimal totalInvestment = 0;
           

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Investorname, amount FROM investor";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                decimal investmentAmount = reader.GetDecimal(1);
                                investors.Add(new Investor(name, investmentAmount));
                                totalInvestment += investmentAmount;
                            }
                        }
                    }
                }

                // Clear previous data from DataGridView
                dataGridViewInvestors.Rows.Clear();

                // Add shop's profit to DataGridView
                dataGridViewInvestors.Rows.Add("Shop", "", shopProfit.ToString("C"));

                // Distribute 50% of the profit among investors
                foreach (var investor in investors)
                {
                    decimal profitShare = (investor.InvestmentAmount / totalInvestment) * investmentProfit;
                    dataGridViewInvestors.Rows.Add(investor.Name, investor.InvestmentAmount, profitShare.ToString("C"));
                }

                // Save shop's portion of the profit
                saveDataintoDatabse(totalprofit);

                // Clear data if needed
                ClearData();
            }

        }

        private void saveDataintoDatabse(decimal shopProfit)
        {
            string date = dateTimePicker1.Value.ToString("" +
                "yyyy-MM-dd HH:mm:ss");

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO profitDistribution (amount, pdate) VALUES (@price, @date)", sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@price", shopProfit);

                        cmd.ExecuteNonQuery();
                    }

                   
                }

                // Optionally, you might want to handle the saving of investor data separately.
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearData()
        {
            weeklyprofitprice.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }
        private void ProfitDistribution_Load(object sender, EventArgs e)
        {
            dataGridViewInvestors.Columns.Add("Investorname", "Investor Name");
            dataGridViewInvestors.Columns.Add("amount", "Investment Amount");
            dataGridViewInvestors.Columns.Add("ProfitShare", "Profit Share");
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {// Format the date for the 'pdate' column
            string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                connection.Open();

                // Begin a transaction for bulk insert
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Loop through each row in the DataGridView
                    foreach (DataGridViewRow row in dataGridViewInvestors.Rows)
                    {
                        // Skip the last row if it's a new row (it won't have data)
                        if (row.IsNewRow) continue;

                        // Get the values from the DataGridView row
                        string investorName = row.Cells["Investorname"].Value?.ToString() ?? string.Empty;

                        // Handle Investment Amount (allow empty value)
                        string investmentAmountStr = row.Cells["amount"].Value?.ToString().Replace("Rs", "").Replace(",", "").Trim();
                        decimal? investmentAmount = null;
                        if (decimal.TryParse(investmentAmountStr, out decimal parsedInvestmentAmount))
                        {
                            investmentAmount = parsedInvestmentAmount;
                        }

                        // Handle Profit Share (allow empty value)
                        string profitShareStr = row.Cells["ProfitShare"].Value?.ToString().Replace("Rs", "").Replace(",", "").Trim();
                        decimal? profitShare = null;
                        if (decimal.TryParse(profitShareStr, out decimal parsedProfitShare))
                        {
                            profitShare = parsedProfitShare;
                        }

                        // Prepare the SQL INSERT statement
                        string query = @"INSERT INTO Investors (InvestorName, InvestmentAmount, ProfitShare, pdate)
                             VALUES (@InvestorName, @InvestmentAmount, @ProfitShare, @date)";

                        // Create a SQL Command
                        using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                        {
                            // Add parameters to prevent SQL Injection
                            cmd.Parameters.AddWithValue("@InvestorName", investorName);

                            // If investmentAmount has a value, use it, otherwise set it to NULL
                            if (investmentAmount.HasValue)
                            {
                                cmd.Parameters.AddWithValue("@InvestmentAmount", investmentAmount.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@InvestmentAmount", DBNull.Value);
                            }

                            // If profitShare has a value, use it, otherwise set it to NULL
                            if (profitShare.HasValue)
                            {
                                cmd.Parameters.AddWithValue("@ProfitShare", profitShare.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@ProfitShare", DBNull.Value);
                            }

                            // Add the date
                            cmd.Parameters.AddWithValue("@date", date);

                            // Execute the query
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction if all commands execute successfully
                    transaction.Commit();
                    MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if an error occurs
                    transaction.Rollback();
                    MessageBox.Show("An error occurred while saving the data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
    }
    }
    

