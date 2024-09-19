using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Tayyab_Cloth_House.LoginForm;

namespace Tayyab_Cloth_House
{
    public partial class Defected : Form
    {
        private string ConnectionString=LoginForm.ConnectionString;
        public Defected()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Addbutton_Click(object sender, EventArgs e)
        {
            string name = nametextbox.Text.Trim();
            string price = pricetextbox.Text.Trim();
            string status=statuscombobox.SelectedItem?.ToString();
            string problem=problemtextbox.Text.Trim();
            string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            // Check if category is null or empty
            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please Select a status from the status ComboBox");
                return;
            }


            // Check if expense, price, or date is null or empty
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(problem) )
            {
                MessageBox.Show("Please fill in the required fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if price is zero or negative
            if (!decimal.TryParse(price, out decimal priceValue) || priceValue <= 0)
            {
                MessageBox.Show("Price must be a positive non-zero value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO defected  (name, date, price,problem,status) VALUES (@name, @date, @price,@problem, @status)", sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@problem", problem);
                        cmd.Parameters.AddWithValue("@status", status);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(" Saved Successfully");
                    LoadDefectedData();
                }

                ClearData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ClearData()
        {
            nametextbox.Text = "";
            pricetextbox.Text = "";
            problemtextbox.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            statuscombobox.SelectedItem = null;
        }

        private void showbutton_Click(object sender, EventArgs e)
        {
            LoadDefectedData();
        }

        private void LoadDefectedData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT id as 'Number', name as 'Name' , date as 'Date',price as 'Amount',problem as 'Problem',status as 'Status' FROM defected";

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

        private void deletebutton_Click(object sender, EventArgs e)
        {
            // Prompt user for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete the selected row(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete selected rows from the database and DataGridView
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int number = Convert.ToInt32(row.Cells["Number"].Value);
                    string Name = Convert.ToString(row.Cells["Name"].Value);
                    string date = Convert.ToString(row.Cells["Date"].Value);
                    decimal Price = Convert.ToDecimal(row.Cells["Amount"].Value);
                    string status = Convert.ToString(row.Cells["Status"].Value);
                    string Problem = Convert.ToString(row.Cells["Problem"].Value);

                    // Delete row from database
                    DeleteRowFromDatabase(number, Name, Price, status,Problem,date);

                    // Remove row from DataGridView
                    dataGridView1.Rows.Remove(row);
                }

                MessageBox.Show("Selected row(s) deleted successfully.", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteRowFromDatabase(int number,string name,decimal price,string status,string problem,string date)
        {
            // Delete row from database based on the given Idsss
            string query = "DELETE FROM defected WHERE id=@number";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@number", number);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           deletebutton.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        private void Defected_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
