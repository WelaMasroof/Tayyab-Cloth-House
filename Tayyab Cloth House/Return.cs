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
    public partial class Return : Form
    {
        
        private string ConnectionString = LoginForm.ConnectionString;

        public Return()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string searchQuery = pname.Text.Trim();
            string filterCategory = filter.SelectedItem?.ToString();
            string filterValue = category.Text.Trim();
            string query = "SELECT id as ID, name, price,quantity as available_stock, gender, color FROM stock WHERE 1=1"; // Select columns with quantity aliased

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

        public void UpdateStockAndAddToItemReturn(DataGridView dataGridView)
        {
            int Quantity = Convert.ToInt32(quantity.Text); // Assuming this is a TextBox named `quantity` for entering quantity

            // Assume the selected row is the one containing the data you want to use
            if (dataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells["ID"].Value); // Replace "ID" with your actual column name
                string pname = selectedRow.Cells["name"].Value.ToString(); // Replace "name" with your actual column name
                decimal amount = Convert.ToDecimal(price.Text); // Replace "price" with your actual column name
                DateTime date = DateTime.Now;

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Begin a transaction
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Update the quantity in the stock table
                        string updateStockQuery = "UPDATE stock SET quantity = quantity + @Quantity WHERE id = @id";
                        using (SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, connection, transaction))
                        {
                            updateStockCmd.Parameters.AddWithValue("@Quantity", Quantity); // Use the `Quantity` variable
                            updateStockCmd.Parameters.AddWithValue("@id", id);
                            updateStockCmd.ExecuteNonQuery();
                        }

                        // Insert into the itemreturn table
                        string insertItemReturnQuery = "INSERT INTO itemreturn (itemname, price, quantity, rdate) VALUES (@pname, @amount, @Quantity, @date)";
                        using (SqlCommand insertItemReturnCmd = new SqlCommand(insertItemReturnQuery, connection, transaction))
                        {
                            insertItemReturnCmd.Parameters.AddWithValue("@pname", pname);
                            insertItemReturnCmd.Parameters.AddWithValue("@amount", amount);
                            insertItemReturnCmd.Parameters.AddWithValue("@Quantity", Quantity); // Use the `Quantity` variable
                            insertItemReturnCmd.Parameters.AddWithValue("@date", date);
                            insertItemReturnCmd.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        MessageBox.Show("Stock updated and item return record added successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if there is an error
                        transaction.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row from the grid view.");
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
                string stockName = row.Cells["name"].Value.ToString();

                // Retrieve the value from the 'purchase_price' column
                

                string id = row.Cells["ID"].Value.ToString();
                // Display the retrieved name in the pname TextBox
                pname.Text = stockName;

                // Display the retrieved purchase price in the purchaseprice TextBox
                

             

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            UpdateStockAndAddToItemReturn(dataGridView1);
        }

        private void price_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
