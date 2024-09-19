using System;
using System.Collections;
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
    public partial class Investor : Form
    {
        private static string ConnectionString = LoginForm.ConnectionString;
        public Investor()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Addbutton_Click(object sender, EventArgs e)
        {
            string name = Investornametextbox.Text.Trim();
            string INAmount = Pricetextbox.Text;
            if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(INAmount))
            {
                MessageBox.Show("Please fill out the required filled");
            }
        

            // Check if the entered quantity is a valid number
            if  (!decimal.TryParse(Pricetextbox.Text, out decimal Amount) || Amount<=0)
            {
                MessageBox.Show("Please enter a valid number for the quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO investor (Investorname, amount) VALUES (@name, @INAmount)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@INAmount", INAmount);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Investor added successfully!");
            clearData();
            LoadInvestorData();
        }

        private void clearData()
        {
            Investornametextbox.Text = null;
            Pricetextbox.Text = null;
        }

        private void Investor_Load(object sender, EventArgs e)
        {
            
            LoadInvestorData();
           
            
        }

        private void LoadInvestorData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT id as 'Number', Investorname as 'InvestorName' , amount as 'Investment' FROM investor";

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Bind the data to the DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Updatebtn_Click(object sender, EventArgs e)
        {
            // Ensure the Amount is a valid decimal
            decimal Amount = Convert.ToDecimal(Pricetextbox.Text); // Input amount to add

            // Query to add the input amount to the existing amount
            string Query = "UPDATE investor SET amount = amount + @Amount WHERE id = @ID";

            // Fetch the data from the database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    // Get the ID from the selected row in the DataGridView (assuming the ID is in the first column)
                    int ID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Amount", Amount); // Input amount
                    command.Parameters.AddWithValue("@ID", ID);         // Investor ID from DataGridView

                    // Execute the query
                    command.ExecuteNonQuery();

                    // Close the connection and show success message
                    connection.Close();
                    MessageBox.Show("Updated Successfully", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload the investor data after update
                    LoadInvestorData();
                }
            }

        }
        public int ID;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Get the DataGridViewRow corresponding to the clicked cell
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Extract the data from the selected row
                 ID = Convert.ToInt32(selectedRow.Cells["Number"].Value);
                 string Name =Convert.ToString(selectedRow.Cells["InvestorName"].Value);
                 Decimal InvestmentAmount= Convert.ToDecimal(selectedRow.Cells["Investment"].Value);
                Pricetextbox.Text =Convert.ToString( InvestmentAmount);
                Investornametextbox.Text = Name;
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {


            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        // Get the ID from the selected row (assuming the ID is in the first column)
                        int InvestorID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Number"].Value);

                        conn.Open();
                        string Query = "DELETE from investor where id=@InvestorID";
                        using (SqlCommand cmd = new SqlCommand(Query, conn))
                        {
                            // Add the ID parameter
                            cmd.Parameters.AddWithValue("@InvestorID", InvestorID);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Deleted Successfully");
                                LoadInvestorData();  // Refresh the DataGridView after deletion
                            }
                            else
                            {
                                MessageBox.Show("No rows were deleted. Please check the ID.");
                            }

                            LoadInvestorData();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a row to delete.");
                    }
                }
               
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }


        
        }
    }
}
