using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Tayyab_Cloth_House.LoginForm;

namespace Tayyab_Cloth_House
{
    public partial class Add_Item : Form
    {
        
        private string ConnectionString = LoginForm.ConnectionString;
        public Add_Item()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name =pname.Text.Trim();
            string color=colortextbox.Text.Trim();
            string gender = comboBox1.SelectedItem?.ToString();
            // Inline validation for inputs

            // Check if any input is null or empty
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(color))
            {
                MessageBox.Show("All fields are required.");
                return;
            }
            // Check if category is null or empty
            if (string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Please Select a gender from the gender ComboBox");
                return;
            }


            if (!Regex.IsMatch(colortextbox.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Color must contain only letters.");
                return;
            }

            try
            {
                
                // Define the insert query
                string query = "INSERT INTO stock (name,color,gender) VALUES (@name,@color,@gender)";

                // Create a connection object
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a command object
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Define the parameters and set their values
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@color", color);
                        command.Parameters.AddWithValue("@gender", gender);
                        int rowsAffected = command.ExecuteNonQuery();
                        
                    }
                    MessageBox.Show("Item Added Successfully");
                    LoadDataintoDataGridView();
                    ClearData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while inserting data: " + ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid input format: " + ex.Message);
            }
        }

        private void ClearData()
        {
            pname.Text = null;
            colortextbox.Text = null;
            comboBox1.SelectedItem = null;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LoadDataintoDataGridView();


        }


        private void LoadDataintoDataGridView()
        {
            string query = "SELECT id as 'Number', name,color,gender FROM stock ";

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
            Delete.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            // Prompt user for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete the selected row(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete selected rows from the database and DataGridView
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int number = Convert.ToInt32(row.Cells["Number"].Value);
              
                    // Delete row from database
                    DeleteRowFromDatabase(number);

                    // Remove row from DataGridView
                    dataGridView1.Rows.Remove(row);
                }

                MessageBox.Show("Selected row(s) deleted successfully.", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteRowFromDatabase(int number)
        {
            // Delete row from database based on the given Idsss
            string query = "DELETE FROM stock WHERE id=@number";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@number", number);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void colortextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void pname_TextChanged(object sender, EventArgs e)
        {

        }
    }
    }
