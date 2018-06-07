using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Car_Management
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string error;
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                string connected;
                DatabaseConnection check = new DatabaseConnection();
                connected = check.checkDatabase();
                if (connected == "true")
                {
                    using (SqlConnection conn = new SqlConnection(DatabaseConnection.connectionStr))
                    {
                        DataTable table = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(@"select * from Contacts", conn);
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            Global.Car = new Cars
                            {
                                serialNumber = table.Rows[0]["SerialNumber"].ToString(),
                                Names = table.Rows[0]["Names"].ToString(),
                                plateNumber = table.Rows[0]["NumberPlates"].ToString(),
                                phoneNumber = table.Rows[0]["PhoneNumber"].ToString(),
                                address = table.Rows[0]["Address"].ToString(),
                                clientID = table.Rows[0]["ID"].ToString(),
                                clientAccount = table.Rows[0]["Account"].ToString(),
                                Technician = table.Rows[0]["Technician"].ToString(),
                                Count = table.Rows[0]["Count"].ToString(),
                            };
                        }
                        DataTable table1 = new DataTable();
                        SqlDataAdapter adapter1 = new SqlDataAdapter(@"select * from Technicians where Email = '" + txtEmail.Text + "' and Password = '" + txtPassword.Text + "'", conn);
                        adapter1.Fill(table1);
                        if (table.Rows.Count > 0)
                        {
                            Global.Technician = new Technicians
                            {
                                ID = table1.Rows[0]["ID"].ToString(),
                                Email = table1.Rows[0]["Email"].ToString(),
                                Password = table1.Rows[0]["Password"].ToString(),
                                Role = table1.Rows[0]["Role"].ToString(),
                                Names = table1.Rows[0]["Name"].ToString(),
                                Surnames = table1.Rows[0]["Surname"].ToString(),
                                Position = table1.Rows[0]["Position"].ToString(),
                            };
                            if (Global.Technician.Role == "Admin")
                            {
                                Cars_and_Clients frmCar = new Cars_and_Clients();
                                frmCar.Show();
                                this.Hide();
                                new loginLogs(Global.Technician.Surnames + " " + Global.Technician.Names,
                                    Global.Technician.ID,Global.Technician.Email,Global.Technician.Position);
                            }
                            else
                            {
                                MessageBox.Show("Normal user connected!");
                            }
                            
                        }
                        DataTable table2 = new DataTable();
                        SqlDataAdapter adapter2 = new SqlDataAdapter(@"select * from Clients", conn);
                        adapter2.Fill(table2);

                        if (table2.Rows.Count > 0)
                        {
                            Global.Client = new Clients
                            {
                                Names = table2.Rows[0]["Name"].ToString(),
                                Surnames = table2.Rows[0]["Surname"].ToString(),
                                Email = table2.Rows[0]["Email"].ToString(),
                                ID = table2.Rows[0]["ID"].ToString(),
                                phoneNumber = table2.Rows[0]["PhoneNumber"].ToString(),
                                Technician = table.Rows[0]["Technician"].ToString(),
                            };
                        }
                    }

                }
                else
                {
                    throw new Exception("Connection to the database was not established.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please provide your correct email and password in order to log in!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                new LogWriter(ex);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + @"Car Management\Logs\playing.txt";
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Car Management\Logs"));
                if (File.Exists(path) == true)
                {

                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.Close();
                    }
                }
                this.ActiveControl = txtEmail;
                string connected;
                DatabaseConnection check = new DatabaseConnection();
                connected = check.checkDatabase();
                if (connected == "true")
                {
                    using (SqlConnection conn = new SqlConnection(DatabaseConnection.connectionStr))
                    {
                        SqlCommand cmd = new SqlCommand("Select Email FROM Technicians", conn);
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
                        while (reader.Read())
                        {
                            MyCollection.Add(reader.GetString(0));
                        }
                        txtEmail.AutoCompleteCustomSource = MyCollection;
                        conn.Close();
                    }
                }
                else
                {
                    throw new Exception("Connection to the database was not established.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                new LogWriter(ex);
                this.Close();
            }
        }
    }
}
