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
    public partial class Sales : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;

        private int rowNumber = 1;
        private decimal totalBill = 0; // Variable to store the total sum
        private decimal totalItems = 0;     // Variable to store the total number of rows


        private string ID;
        private int ID2;
        public Sales()
        {
            InitializeComponent();
            SetupDataGridView2();
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        }


        private void SetupDataGridView2()
        {
            // Set AutoGenerateColumns to false to manually define columns
            dataGridView2.AutoGenerateColumns = false;

            // Create and add the 'Number' column


            DataGridViewTextBoxColumn idcolumn = new DataGridViewTextBoxColumn();
            idcolumn.Name = "ID";
            idcolumn.HeaderText = "ID";
            idcolumn.DataPropertyName = "ID";
            dataGridView2.Columns.Add(idcolumn);

            // Create and add the 'name' column
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "name";
            nameColumn.HeaderText = "Name";
            nameColumn.DataPropertyName = "Name";
            dataGridView2.Columns.Add(nameColumn);

            // Create and add the 'quantity' column
            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.Name = "quantity";
            quantityColumn.HeaderText = "Quantity";
            quantityColumn.DataPropertyName = "Quantity";
            dataGridView2.Columns.Add(quantityColumn);

            // Create and add the 'price' column
            DataGridViewTextBoxColumn priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.Name = "price";
            priceColumn.HeaderText = "Price";
            priceColumn.DataPropertyName = "Price";
            dataGridView2.Columns.Add(priceColumn);

            // Create and add the 'Total' column
            DataGridViewTextBoxColumn totalColumn = new DataGridViewTextBoxColumn();
            totalColumn.Name = "Total";
            totalColumn.HeaderText = "Total";
            totalColumn.DataPropertyName = "Total";
            dataGridView2.Columns.Add(totalColumn);
            // Create and add the 'Total' column
            DataGridViewTextBoxColumn totalexpenseColumn = new DataGridViewTextBoxColumn();
            totalexpenseColumn.Name = "totalexpense";
            totalexpenseColumn.HeaderText = "totalexpense";
            totalexpenseColumn.DataPropertyName = "totalexpense";
            dataGridView2.Columns.Add(totalexpenseColumn);


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
                string purchasePrice = row.Cells["price"].Value.ToString();

                string id = row.Cells["ID"].Value.ToString();
                // Display the retrieved name in the pname TextBox
                pname.Text = stockName;

                // Display the retrieved purchase price in the purchaseprice TextBox
                aa.Text = purchasePrice;

                ID = id;

            }

        }

        private void Sales_Load(object sender, EventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a product from the list.", "No Product Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected row
            DataGridViewRow selectedRow = dataGridView1.CurrentRow;

            // Retrieve the available stock from the selected row
            if (!decimal.TryParse(selectedRow.Cells["available_stock"].Value.ToString(), out decimal availableStock))
            {
                MessageBox.Show("The available stock value is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the entered quantity is a valid decimal number
            if (!decimal.TryParse(Quantity.Text, out decimal enteredQuantity))
            {
                MessageBox.Show("Please enter a valid number for the quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Check if the entered quantity is greater than the available stock
            if (enteredQuantity > availableStock)
            {
                MessageBox.Show("Entered quantity is greater than the available stock.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the sell price from the sellprice TextBox
            if (!decimal.TryParse(sellprice.Text, out decimal sellPrice))
            {
                MessageBox.Show("Please enter a valid number for the sell price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the purchase price from the purchaseprice TextBox
            if (!decimal.TryParse(aa.Text, out decimal purchasePrice))
            {
                MessageBox.Show("Please enter a valid number for the purchase price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            else
            {

                // Calculate the total price
                decimal totalPrice = sellPrice * enteredQuantity;

                // Calculate the profit
                decimal profit = (sellPrice - purchasePrice) * enteredQuantity;
                decimal PurcahsePrice = Convert.ToDecimal(aa.Text);
                decimal expenseP = PurcahsePrice * enteredQuantity;

                // Display the total price in the Total TextBox
                Total.Text = totalPrice.ToString("0");

                // If the quantity is valid, proceed with the sale process
                // Deduct the enteredQuantity from the available stock
                availableStock -= enteredQuantity;

                // Update the stock value in the DataGridView
                selectedRow.Cells["available_stock"].Value = availableStock;

                // Optionally, update the stock value in the database here
                AddToDataGridView2();
                CalculateTotalAndItemCount();


          
            }

        }

        private void CalculateTotalAndItemCount()
        {
            // Reset totals before calculation
            totalBill = 0;
            totalItems = 0;

            // Iterate through each row in the DataGridView2
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Ensure the row is not a new row (which is empty)
                if (!row.IsNewRow)
                {
                    // Retrieve and parse the value in the 'Total' column
                    if (decimal.TryParse(row.Cells["Total"].Value?.ToString(), out decimal totalValue))
                    {
                        totalBill += totalValue; // Add to the total bill
                    }
                    totalItems++; // Increment the total items count
                }
            }
            UpdateTotalAndItemCountUI();
        }
        private void DeleteRowByNumber()
        {
            // Ensure a row is selected in the DataGridView2
            if (dataGridView2.CurrentRow != null)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dataGridView2.CurrentRow;

                // Retrieve and parse the value from the 'ID' column
                if (int.TryParse(selectedRow.Cells["ID"].Value?.ToString(), out int numberToDelete))
                {
                    // Iterate through each row in the DataGridView2
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        // Ensure the row is not a new row (which is empty)
                        if (!row.IsNewRow)
                        {
                            // Retrieve and parse the ID value from the row
                            if (int.TryParse(row.Cells["ID"].Value?.ToString(), out int rowNumber))
                            {
                                // Check if the ID matches the one to delete
                                if (rowNumber == numberToDelete)
                                {
                                    // Retrieve and parse the Total value from the row
                                    if (decimal.TryParse(row.Cells["Total"].Value?.ToString(), out decimal totalValue))
                                    {
                                        // Subtract the row's Total value from the total bill
                                        totalBill -= totalValue;

                                        // Decrement the total items count
                                        totalItems--;
                                    }

                                    // Remove the row from the DataGridView
                                    dataGridView2.Rows.Remove(row);
                                    MessageBox.Show("Row deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Update the UI with the new total and item count (if needed)
                                    UpdateTotalAndItemCountUI();

                                    return; // Exit after deleting the row
                                }
                            }
                        }
                    }

                    // If no matching row was found, notify the user
                    MessageBox.Show("No row found with the specified ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("The ID value is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Method to update the UI with the new total bill and item count
        private void UpdateTotalAndItemCountUI()
        {
            // Update any labels or textboxes that display the total bill and item count
            totalbill.Text = totalBill.ToString();
            totalitems.Text = totalItems.ToString();
        }

        private void AddToDataGridView2()
        {
            // Retrieve values from the TextBoxes
            string productName = pname.Text;
            string sellPriceText = sellprice.Text;
            string quantityText = Quantity.Text; // Ensure you have this TextBox for entering the quantity
            string expenseP = aa.Text;

            // Check if the sell price and quantity are valid numbers
            if (!decimal.TryParse(sellPriceText, out decimal sellPrice))
            {
                MessageBox.Show("Please enter a valid sell price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(expenseP, out decimal expense))
            {
                MessageBox.Show("Please enter a valid sell price.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(quantityText, out decimal quantity))
            {
                MessageBox.Show("Please enter a valid quantity.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate the total price
            decimal totalPrice = sellPrice * quantity;
            decimal TotalExpense = expense * quantity;

            // Add a new row to datagridview2 with the values
            dataGridView2.Rows.Add(ID, productName, quantity, sellPrice, totalPrice, TotalExpense);

            // Increment the row number for the next entry
            rowNumber++;
        }

        private void x(object sender, EventArgs e)
        {

        }

        private void quantity_TextChanged(object sender, EventArgs e)
        {

        }

        private void search_Click(object sender, EventArgs e)
        {
            SerchFunction();
        }


        private void SerchFunction()
        {
            // Ensure necessary fields are trimmed
            string searchQuery = pname.Text.Trim();
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
                        // Use LIKE for partial match
                        query += " AND gender LIKE @filterValue";
                        break;
                    case "color":
                        // Use LIKE for partial match
                        query += " AND color LIKE @filterValue";
                        break;
                    case "quantity":
                        // Validate if the filterValue for quantity is a valid number
                        if (!decimal.TryParse(filterValue, out _))
                        {
                            MessageBox.Show("Please provide a valid numeric value for quantity.");
                            return;
                        }
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
                        // Use wildcard for category filters that use LIKE
                        command.Parameters.AddWithValue("@filterValue", "%" + filterValue + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteRowByNumber();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell's row index is valid
            if (e.RowIndex >= 0)
            {
                // Get the current row that was clicked
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Retrieve the value from the 'ID' column and parse it to int
                if (int.TryParse(row.Cells["ID"].Value?.ToString(), out int id))
                {
                    // Store the ID in a variable
                    ID2 = id;

                    // Optionally, display the retrieved ID in the pname TextBox
                    pname.Text = id.ToString();
                }
                else
                {
                    MessageBox.Show("The ID value is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        private void UpdateStockQuantities()
        {
            // Create a list to hold the IDs and quantities to update
            var updates = new List<(int id, decimal quantity)>();

            // Iterate through each row in DataGridView2
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Ensure the row is not a new row (which is empty)
                if (!row.IsNewRow)
                {
                    // Retrieve and parse the ID and quantity from the row
                    if (int.TryParse(row.Cells["ID"].Value?.ToString(), out int id) &&
                        decimal.TryParse(row.Cells["quantity"].Value?.ToString(), out decimal quantity))
                    {
                        // Add the ID and quantity to the list of updates
                        updates.Add((id, quantity));
                    }
                }
            }

            // Update the stock quantities in the database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                foreach (var update in updates)
                {
                    using (SqlCommand command = new SqlCommand("UPDATE stock SET quantity = quantity - @quantity WHERE ID = @id", connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@quantity", update.quantity);
                        command.Parameters.AddWithValue("@id", update.id);

                        // Execute the command
                        command.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Stock quantities updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void InsertSaleDetails()
        {
            string customerName = customername.Text.Trim(); // Ensure customer name is trimmed
            decimal enteredQuantity = 0; // Initialize entered quantity as a decimal for precision
            decimal purchasePrice = 0; // Initialize with a default value for the item being inserted
            decimal expenseP = 0; // Initialize total expense for the current sale

            // Validate that the quantity is a valid decimal
            if (!decimal.TryParse(Quantity.Text, out enteredQuantity))
            {
                MessageBox.Show("Invalid quantity. Please enter a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the purchase price is a valid decimal
            if (!decimal.TryParse(aa.Text, out purchasePrice))
            {
                MessageBox.Show("Invalid purchase price. Please enter a valid number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            expenseP = purchasePrice * enteredQuantity; // Calculate the total expense

            // List to hold sales information for each item
            var sales = new List<(string pname, string customername, decimal purchasePrice, decimal sellPrice, decimal quantity, decimal total, decimal profit, string date, decimal expenseP)>();

            // Iterate through each row in DataGridView2 to collect the sales information
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Ensure the row is not a new row (which is empty)
                if (!row.IsNewRow)
                {
                    // Retrieve and parse the required values from the row
                    string pname = row.Cells["name"].Value?.ToString() ?? "Unknown Product"; // Default to "Unknown Product" if pname is NULL
                    decimal quantity = 0;
                    decimal sellPrice = 0;
                    decimal purchasePriceForRow = 0; // Initialize purchase price for each row
                    decimal totalExpenseForRow = 0;  // Total expense for this row

                    // Check and parse quantity, sellPrice, and totalExpenseForRow, defaulting to 0 if they are invalid decimals
                    decimal.TryParse(row.Cells["quantity"].Value?.ToString(), out quantity);
                    decimal.TryParse(row.Cells["Price"].Value?.ToString(), out sellPrice);
                    decimal.TryParse(row.Cells["totalexpense"].Value?.ToString(), out totalExpenseForRow); // Parse the total expense for each item in the row

                    string date = Date.Value.ToString("yyyy-MM-dd HH:mm:ss"); // Use the current date

                    // Calculate total (sale price * quantity) and profit (sale total - purchase total)
                    decimal totalSaleForRow = sellPrice * quantity;   // Total sale amount for the current row
                    decimal profitForRow = totalSaleForRow - totalExpenseForRow; // Profit for the current row (sale total - purchase total)

                    // Add the details of the current row to the list
                    sales.Add((pname, customerName, purchasePriceForRow, sellPrice, quantity, totalSaleForRow, profitForRow, date, totalExpenseForRow));
                }
            }

            // Insert the sale details into the database
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    foreach (var sale in sales)
                    {
                        // Insert each sale entry into the sale table
                        using (SqlCommand command = new SqlCommand(@"
                INSERT INTO sale (name, customername, price, quantity, total, profit, date, expenseP)
                VALUES (@name, @customername, @Price, @quantity, @total, @profit, @date, @expenseP)", connection))
                        {
                            // Add parameters to prevent SQL injection
                            command.Parameters.AddWithValue("@name", sale.pname);
                            command.Parameters.AddWithValue("@customername", sale.customername);
                            command.Parameters.AddWithValue("@Price", sale.sellPrice);
                            command.Parameters.AddWithValue("@quantity", sale.quantity);
                            command.Parameters.AddWithValue("@total", sale.total);
                            command.Parameters.AddWithValue("@profit", sale.profit);
                            command.Parameters.AddWithValue("@date", sale.date);
                            command.Parameters.AddWithValue("@expenseP", sale.expenseP);

                            // Execute the command to insert the sale details
                            command.ExecuteNonQuery();
                        }
                    }

                    // Success message
                    MessageBox.Show("Sale details inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show an error message
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            InsertSaleDetails();
            UpdateStockQuantities();
            clearData();
            dataGridView2.Rows.Clear();
        }

        private void clearData()
        {
            aa.Text = null;
            sellprice.Text = null;
            Quantity.Text = null;
            Total.Text = null;
            customername.Text = null;
            totalitems.Text = null;
            totalbill.Text = null;
            pname.Text = null;
            filter.SelectedItem = -1;
            dataGridView2.ClearSelection();
            if (dataGridView2.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                {
                    dataGridView2.Rows.Remove(row);
                }
            }
        }

        private void customername_TextChanged(object sender, EventArgs e)
        {

        }

        private void totalitems_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void category_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pname_TextChanged(object sender, EventArgs e)
        {

        }

        private void Quantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal Sellprice = Convert.ToDecimal(sellprice.Text);
                decimal qunatity=Convert.ToDecimal(Quantity.Text);

                decimal totalprice = Sellprice * qunatity;

                Total.Text = Convert.ToString(totalprice);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            clearData();
            SerchFunction();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
