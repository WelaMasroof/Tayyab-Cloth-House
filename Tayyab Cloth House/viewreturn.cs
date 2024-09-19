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
    public partial class viewreturn : Form
    {
        private string ConnectionString = LoginForm.ConnectionString;

        public viewreturn()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = pname.Text.Trim();
            DateTime selectedDate = dateTimePicker.Value; // Use .Value to get the selected date from dateTimePicker
           

            // Build the query based on user input
            string query = "SELECT * FROM itemreturn WHERE 1=1"; // Selecting all columns from sale table

            // Add name search if specified
            if (!string.IsNullOrEmpty(name))
            {
                query += "AND itemname LIKE @name";
            }

            // Add customer name search if specified
            

            // Add date search if specified
            if (selectedDate != DateTime.MinValue) // Check if date is valid
            {
                query += " AND CONVERT(date, [rdate]) = @selectedDate";
            }

            // Fetch the data from the database
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    if (!string.IsNullOrEmpty(name))
                    {
                        command.Parameters.AddWithValue("@name", "%" + name + "%");
                    }
                    
                    if (selectedDate != DateTime.MinValue)
                    {
                        command.Parameters.AddWithValue("@selectedDate", selectedDate.Date); // Ensure only the date part is used
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }
    }
}
