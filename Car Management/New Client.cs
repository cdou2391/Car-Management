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
    public partial class New_Client : Form
    {
        public New_Client()
        {
            InitializeComponent();
        }
        string error;
        private void btnExit_Click(object sender, EventArgs e)
        {
            Cars_and_Clients frmCars = (Cars_and_Clients)Application.OpenForms["Cars_and_Clients"];
            frmCars.loadData();
            frmCars.dgvClients.Update();
            frmCars.dgvClients.Refresh();
            this.Close();
        }

        private void btnAddClient_Click(object sender, EventArgs e)
        {
            try
            {
                string connected;
                DatabaseConnection check = new DatabaseConnection();
                connected = check.checkDatabase();
                if (connected == "true")
                {
                    if (txtIDNum.Text != "" & txtName.Text != "")
                    {
                        string name = txtName.Text;
                        string surname = txtSurname.Text;
                        long PhoneNum = Convert.ToInt64(txtPhnNum.Text);
                        long IDNum = Convert.ToInt64(txtIDNum.Text);
                        string email = txtEmail.Text;
                        string tech = Global.Technician.Names + " " + Global.Technician.Surnames;
                        using (SqlConnection conn = new SqlConnection(DatabaseConnection.connectionStr))
                        {
                            conn.Open();
                            string querry = "INSERT INTO Users(ID,Name,Surname,Email,PhoneNumber,Technician) "
                                           + "Values('" + IDNum + "','" + name + "','" + surname + "','" + email + "','" + PhoneNum + "','" + tech + "')";
                            using (SqlCommand cmd = new SqlCommand(querry, conn))
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("New Client added!");
                                Cars_and_Clients frmCars = new Cars_and_Clients();
                                frmCars.loadData();
                                frmCars.dgvClients.Update();
                                frmCars.dgvClients.Refresh();
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
    }
}
