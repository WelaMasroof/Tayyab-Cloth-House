using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Tayyab_Cloth_House.LoginForm;

namespace Tayyab_Cloth_House
{
    public partial class Form1 : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;

        private int selectedStockId = -1; // Default to -1 to indicate no selection
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClearData()
        {
            pname.Text = null;
            pquantity.Text = null;
            pprice.Text= null;
            totaltextbox.Text = null;
        }

        private void UpdateStockRecord()
        {
            // Inline validation for inputs
            if (string.IsNullOrWhiteSpace(pquantity.Text) || string.IsNullOrWhiteSpace(pprice.Text) || string.IsNullOrWhiteSpace(pname.Text))
            {
                MessageBox.Show("All fields are required.");
                return;
            }


            // Validate that quantity contains only numbers (no letters)
            if (!Regex.IsMatch(pquantity.Text, @"^\d+(\.\d{1,2})?$"))
            {
                MessageBox.Show("Quantity must be a valid decimal number.");
                return;
            }

            // Validate that price contains only numbers and optionally a decimal point
            if (!Regex.IsMatch(pprice.Text, @"^\d+(\.\d{1,2})?$"))
            {
                MessageBox.Show("Price must be a valid number (optionally with up to two decimal places).");
                return;
            }

            // Get the selected stock ID from the DataGridView
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to update.");
                return;
            }

            // Assuming the ID column is the first column in the DataGridView
            int selectedStockId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            try
            {
                // Retrieve values from text boxes
                decimal quantityToAdd = decimal.Parse(pquantity.Text); // The quantity to add (as decimal)
                                                                       // The quantity to add
                decimal price = decimal.Parse(pprice.Text); // Assuming pprice is the TextBox for price
                string date = dateTimePicker1.Value.ToString("yyyy-MM-dd"); // Get the selected date from the date picker

                // Calculate total price


                // Call the method to calculate the total quantity for this stock ID
                decimal currentTotalQuantity = CalculateTotalQuantity(selectedStockId); // Update this method to return decimal if needed
                decimal allQuantity = quantityToAdd + currentTotalQuantity;
                decimal total = price * allQuantity;

                // Optionally, use currentTotalQuantity for further logic if needed
                totaltextbox.Text = total.ToString(); // Update the totaltextbox with the calculated total
                decimal totalstockprice = total; // This is the value we will update in the database

                // Define the update query to add the input quantity to the existing quantity, set the date, and update the total price
                string query = "UPDATE stock SET price = @price, quantity = COALESCE(quantity, 0) + @quantityToAdd, date = @date, total = @totalstockprice WHERE id = @id";

                // Create a connection object
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command object
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Define the parameters and set their values
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@quantityToAdd", quantityToAdd); // Use the quantity to add
                        command.Parameters.AddWithValue("@date", date); // Use the selected date
                        command.Parameters.AddWithValue("@totalstockprice", totalstockprice); // Update total price in the database
                        command.Parameters.AddWithValue("@id", selectedStockId); // Use the selected ID

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();
                        MessageBox.Show($"Rows updated: {rowsAffected}");
                        ClearData();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while updating data: " + ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid input format: " + ex.Message);
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            UpdateStockRecord();
            loaddataintoDatagridView();
        }


        private void show_Click(object sender, EventArgs e)
        {

            loaddataintoDatagridView();
        }

        private void loaddataintoDatagridView()
        {
            string query = "SELECT id as ID, name, price, quantity,total, color, gender FROM stock ";

            // Create a connection object
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a command object
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {     // Create a data adapter to fill the DataGridView
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the results
                            DataTable dataTable = new DataTable();

                            // Fill the DataTable with the results
                            adapter.Fill(dataTable);

                            // Bind the DataTable to the DataGridView
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("An error occurred while searching for data: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell's row index is valid
            if (e.RowIndex >= 0)
            {
                // Get the current row that was clicked
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Retrieve the value from the 'name' column
                string name = row.Cells["name"].Value.ToString();

                // Display the retrieved name in the pname TextBox
                pname.Text = name;
            }
            if (e.RowIndex >= 0)
            {
                // Get the current row that was clicked
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Retrieve the value from the 'name' column
                string selectedName = row.Cells["name"].Value.ToString();

                // Display the retrieved name in the pname TextBox
                pname.Text = selectedName;

                // Retrieve the ID from the database based on the selected name
                GetStockIdByName(selectedName);
            }
        }
        private void GetStockIdByName(string name)
        {
            // Define the query to retrieve the ID based on the name
            string query = "SELECT id FROM stock WHERE name = @name";

            // Create a connection object
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a command object
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Define the parameter and set its value
                        command.Parameters.AddWithValue("@name", name);

                        // Execute the command and retrieve the ID
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            selectedStockId = Convert.ToInt32(result);

                            // Debug message box to show the selected stock ID
                            MessageBox.Show($"Debug: Selected Stock ID: {selectedStockId}", "Debug Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No stock found with the selected name.");
                            selectedStockId = -1; // Reset if no match is found
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("An error occurred while retrieving the stock ID: " + ex.Message);
                    selectedStockId = -1; // Reset in case of error
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Retrieve the selected stock ID from the DataGridView
                int selectedStockId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);

                // Call the method to calculate the total quantity for this stock ID
                CalculateTotalQuantity(selectedStockId);
            }
        }


        private int CalculateTotalQuantity(int stockId)
        {
            try
            {
                int totalQuantity = 0;

                // SQL query to calculate the total quantity for the given stock ID
                string query = "SELECT SUM(quantity) FROM stock WHERE id = @id";

                // Create a connection object
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command object
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Define the parameter and set its value
                        command.Parameters.AddWithValue("@id", stockId);

                        // Execute the query and get the result
                        var result = command.ExecuteScalar();

                        // Check if the result is not null and not DBNull
                        if (result != DBNull.Value && result != null)
                        {
                            // Convert the result to an integer
                            totalQuantity = Convert.ToInt32(result);
                        }
                    }
                }
                return totalQuantity;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while retrieving the total quantity: " + ex.Message);
                return 0; // Return 0 in case of error
            }
        }


        private void p_Click(object sender, EventArgs e)
        {

        }
    }
}
