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
using static Tayyab_Cloth_House.LoginForm;

namespace Tayyab_Cloth_House
{
    public partial class Dashboard : Form
    {

        private string ConnectionString = LoginForm.ConnectionString;
      

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            gettotalsales();
            gettotalexpenses();
            gettotalprofit();
        }

        public void gettotalsales()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    // Query to get the most recent profit distribution datetime
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

                    // Query to calculate the total sales after the last profit calculation datetime
                    salesQuery = "SELECT SUM(total) FROM sale WHERE date >= @ReferenceDateTime";
                    salesCommand = new SqlCommand(salesQuery, sqlConnection);
                    salesCommand.Parameters.AddWithValue("@ReferenceDateTime", referenceDateTime);

                    var totalSalesResult = salesCommand.ExecuteScalar();
                    totalsalelabel.Text = totalSalesResult != DBNull.Value ? totalSalesResult.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void gettotalexpenses()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    // Query to get the most recent profit distribution datetime
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

                    // Query to calculate the total expenses after the last profit calculation datetime
                    expenseQuery = "SELECT SUM(expenseP) FROM sale WHERE date >= @ReferenceDateTime";
                    expenseCommand = new SqlCommand(expenseQuery, sqlConnection);
                    expenseCommand.Parameters.AddWithValue("@ReferenceDateTime", referenceDateTime);

                    var totalExpenseResult = expenseCommand.ExecuteScalar();
                    totalexpenselabel.Text = totalExpenseResult != DBNull.Value ? totalExpenseResult.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string gettotalprofit()
        {
            decimal totalsales = Convert.ToDecimal(totalsalelabel.Text);
            decimal totalexpenses = Convert.ToDecimal(totalexpenselabel.Text);

            decimal profit = totalsales - totalexpenses;

            // Update the lastprofitlabel (optional, if you still need to show it somewhere)
            lastprofitlabel.Text = Convert.ToString(profit);

            // Return the profit as a string
            return profit.ToString();
        }


    }
}
