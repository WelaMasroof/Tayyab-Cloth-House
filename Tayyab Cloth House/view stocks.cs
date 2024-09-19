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
    public partial class view_stocks : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;
        public view_stocks()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ensure necessary fields are trimmed
            string searchQuery = searchtextbox.Text.Trim();
            string filterCategory = filter.SelectedItem?.ToString();
            string filterValue = category.Text.Trim();

            // Build the query based on user input
            string query = "SELECT id, name, price, quantity AS available_stock, gender, color FROM stock WHERE 1=1"; // Select columns with quantity aliased

            // Add name search if specified
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " AND name LIKE @searchQuery";
            }

            // Add filter based on the selected category
            if (!string.IsNullOrEmpty(filterCategory) && !string.IsNullOrEmpty(filterValue))
            {
                switch (filterCategory.ToLower())
                {
                    case "gender":
                    case "color":
                        // Use LIKE for partial match with text-based values
                        query += $" AND {filterCategory} LIKE @filterValue";
                        break;
                    case "quantity":
                        // Validate if the filterValue for quantity is a valid number
                        if (!decimal.TryParse(filterValue, out decimal numericValue))
                        {
                            MessageBox.Show("Please provide a valid numeric value for quantity.");
                            return;
                        }
                        // Allow user to filter quantities less than the provided value
                        query += " AND quantity < @filterValue";
                        break;
                    default:
                        MessageBox.Show("Invalid filter category selected.");
                        return;
                }
            }

            // Fetch the data from the database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        command.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
                    }
                    if (!string.IsNullOrEmpty(filterValue))
                    {
                        if (filterCategory.ToLower() == "quantity")
                        {
                            // For quantity (numeric value)
                            command.Parameters.AddWithValue("@filterValue", filterValue);
                        }
                        else
                        {
                            // For text values (gender, color)
                            command.Parameters.AddWithValue("@filterValue", "%" + filterValue + "%");
                        }
                    }

                    // Execute query and populate DataGridView
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string searchQuery = searchtextbox.Text.Trim();
            string filterCategory = filter.SelectedItem?.ToString();
            string filterValue = category.Text.Trim();

            // Build the query based on user input
            string query = "SELECT * FROM stock WHERE 1=1"; // Start with a default where condition

            // Add name search if specified
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " AND name LIKE @searchQuery";
            }

            // Add filter based on the selected category
            if (!string.IsNullOrEmpty(filterCategory) && !string.IsNullOrEmpty(filterValue))
            {
                switch (filterCategory.ToLower())
                {
                    case "gender":
                        query += " AND gender = @filterValue";
                        break;
                    case "color":
                        query += " AND color = @filterValue";
                        break;
                    case "quantity":
                        // For quantity, assuming we want items less than the given value
                        query += " AND quantity < @filterValue";
                        break;
                    default:
                        MessageBox.Show("Invalid filter category selected.");
                        return;
                }
            }

            // Fetch the data from the database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        command.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
                    }
                    if (!string.IsNullOrEmpty(filterValue))
                    {
                        command.Parameters.AddWithValue("@filterValue", filterValue);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void view_stocks_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
