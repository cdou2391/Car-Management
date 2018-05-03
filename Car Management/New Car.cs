using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Car_Management
{
    public partial class New_Car : Form
    {
        public New_Car()
        {
            InitializeComponent();
        }
        string error;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string connected;
                DatabaseConnection check = new DatabaseConnection();
                connected = check.checkDatabase();
                long i;
                if (connected == "true")
                {
                    if (txtSrlNum.Text != "" & txtPltNum.Text != "")
                    {
                        MessageBox.Show((long.TryParse(txtPltNum.Text, out i)).ToString());
                        string names = comboUsers.Text;
                        long PhoneNum = Convert.ToInt64(txtPhnNum.Text);
                        long srlNum = Convert.ToInt64(txtSrlNum.Text);
                        string plateNum = txtPltNum.Text;
                        string address = txtAddress.Text;
                        long IDnum = Convert.ToInt64(txtID.Text);
                        long account = Convert.ToInt64(txtAcc.Text);
                        string tech = Global.Technician.Names + " " + Global.Technician.Surnames;
                        using (SqlConnection conn = new SqlConnection(DatabaseConnection.connectionStr))
                        {
                            conn.Open();
                            string querry = "INSERT INTO Contacts(Names,PhoneNumber,SerialNumber,NumberPlates,Address,ID,Account,Technician) "
                                           + "Values('" + names + "','" + PhoneNum + "','" + srlNum + "','" + plateNum + "','" + address + "','" + IDnum + "','" + account + "','"+ tech+"')";
                            using (SqlCommand cmd = new SqlCommand(querry, conn))
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("New Car added!");
                            }
                            conn.Close();
                        }
                    }
                    else
                    {
                        error = "Please make sure you have entered all the required information";
                        MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        new LogWriter(error);
                    }
                }
                else
                {
                    error = "Not Connected";
                    MessageBox.Show(error);
                    new LogWriter(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                new LogWriter(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cars_and_Clients frmCars = (Cars_and_Clients)Application.OpenForms["Cars_and_Clients"];
            frmCars.loadData();
            frmCars.dataGridView1.Update();
            frmCars.dataGridView1.Refresh();
            this.Close();
        }

        private void New_Car_Load(object sender, EventArgs e)
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
                        SqlCommand cmd = new SqlCommand("Select Name,Surname FROM Users", conn);
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
                        while (reader.Read())
                        {
                            MyCollection.Add(reader.GetString(0) +" " +  reader.GetString(1));
                        }
                        comboUsers.DataSource = MyCollection;
                        conn.Close();
                    }
                }
                else
                {
                    error = "Connection to the database was not established.";
                    MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    new LogWriter(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                new LogWriter(ex.ToString());
            }
        }
    }
}
