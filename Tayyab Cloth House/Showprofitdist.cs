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
    public partial class Showprofitdist : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;
        public Showprofitdist()
        {
            InitializeComponent();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void Showprofitdist_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Format the date from the DateTimePicker
            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");
            string name = pname.Text.Trim();

            // Define the SQL query with a parameter
            string query = "SELECT * FROM Investors WHERE pdate = @date and InvestorName LIKE @name";

            // Create a new SqlConnection
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                // Create a SqlDataAdapter to fetch data
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                {
                    // Add parameter to the SqlDataAdapter
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@date", date);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("name", "%"+name+ "%");

                    // Create a DataTable to hold the fetched data
                    DataTable dataTable = new DataTable();

                    try
                    {
                        // Open the connection
                        connection.Open();

                        // Fill the DataTable with data from the database
                        dataAdapter.Fill(dataTable);

                        // Set the DataSource of the DataGridView to the DataTable
                        dataGridView1.DataSource = dataTable;
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that might occur
                        MessageBox.Show("An error occurred while loading data: " + ex.Message);
                    }
                }
            }

        }
    }
}
