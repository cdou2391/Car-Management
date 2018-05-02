using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using NetFrame.Net.TCP.Sock.Asynchronous;
using System.Threading;
using System.IO.Ports;

namespace Car_Management
{
    
    public partial class Cars_and_Clients : Form
    {
        
        private long totalnum1 = 0x00;
        private long totalnum2 = 0x00;
        private long totaltime = 0x00;
        private const int listView_md_epc_Num = 0;
        private const int listView_md_epc_AntID = 1;
        private const int listView_md_epc_EPC = 2;
        private const int listView_md_epc_PC = 3;
        private const int listView_md_epc_Rssi = 4;
        private const int listView_md_epc_Count = 5;
        private const int listView_md_epc_IP = 6;
        private const int listView_md_epc_Last_Time = 7;
        private const int listView_md_epc_Direction = 8;
        private volatile List<_epc_t> epcs_list = new List<_epc_t>(1000);

        private string portname = "";
        private int baudRate = 230400;
        private int dataBits = 8;
        private Parity parity = Parity.None;
        private StopBits stopbits = StopBits.One;
        string error;
        public Cars_and_Clients()
        {
            InitializeComponent();
            //this.listView_oper_log.Columns.Add("序号/NUM", 80, HorizontalAlignment.Left);//Num
            //this.listView_oper_log.Columns.Add("时间/Time", 150, HorizontalAlignment.Left);//Time
            //this.listView_oper_log.Columns.Add("执行结果/Result", 450, HorizontalAlignment.Left);//Operation Result
            //this.listView_oper_log.GridLines = true;
            //this.listView_oper_log.FullRowSelect = true;
            //this.listView_oper_log.MultiSelect = false;


            //this.listView_md_addr.Columns.Add("Num", 30, HorizontalAlignment.Left);
            //this.listView_md_addr.Columns.Add("IP", 100, HorizontalAlignment.Left);
            //this.listView_md_addr.Columns.Add("Port", 50, HorizontalAlignment.Left);
            //this.listView_md_addr.Columns.Add("ID", 50, HorizontalAlignment.Left);
            //this.listView_md_addr.Columns.Add("State", 50, HorizontalAlignment.Left);
            //this.listView_md_addr.GridLines = true;
            //this.listView_md_addr.FullRowSelect = true;
            //this.listView_md_addr.MultiSelect = false;


            Control.CheckForIllegalCrossThreadCalls = false;

            //comboBox_mb.SelectedIndex = 1;//epc
            //filterbox.SelectedIndex = 0;  //不过滤
            //comboBox3.SelectedIndex = 1;
            //comboBox4.SelectedIndex = 1;
            //timer_scan.Enabled = true;

            //language_cb.SelectedIndex = 0;
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            //UpDataMainFormUILanguage();

            ReaderControllor.cmd.MultiEPC_Event += ShowEPC;

            this.tabPage2.Parent = null;
        }

        private void Cars_and_Clients_Load(object sender, EventArgs e)
        {
            loadData();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            New_Car frmNewCar = new New_Car();
            frmNewCar.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 frm1 = new Form1();
            frm1.Show();
        }

        private void bntNewClient_Click(object sender, EventArgs e)
        {
            New_Client frmNewClient = new New_Client();
            frmNewClient.Show();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            try
            {

                PortConfig SerialPortForm = new PortConfig();
                SerialPortForm.ShowDialog();
                if (SerialPortForm.result == true)
                {
                    textBox1.Text = SerialPortForm.PortName;
                    portname = textBox1.Text;
                    baudRate = SerialPortForm.BuadRate;
                    dataBits = SerialPortForm.dataBits;
                    parity = SerialPortForm.parity;
                    stopbits = SerialPortForm.stopbits;
                }
            }
            catch(Exception ex)
            {
                new LogWriter(ex.ToString());
            }
            
        }
        bool serialisstart = false;
        bool serverisstart = false;

        private Reader ReaderControllor = new Reader();
        private AsyncSocketState currentclient;

        private void btnStartPort_Click(object sender, EventArgs e)
        {
            if (btnStartPort.Text == "Start")
            {
                portname = textBox1.Text;
                try
                {
                    ReaderControllor.ComStart(portname, baudRate, dataBits, parity, stopbits);
                    if (timer_md_query_Tick.Enabled == false)
                    {
                        timer_md_query_Tick.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    // UpdateLog("Error:" + ex.ToString());
                    new LogWriter(ex.ToString());
                }
                serialisstart = true;
                btnStartPort.Text = "Stop";
                //UpdateLog(openserial + success);
            }
            else
            {
                serialisstart = false;
                ReaderControllor.SerialPortClose();
                if (serverisstart == false && serialisstart == false && timer_md_query_Tick.Enabled == true)
                {
                    timer_md_query_Tick.Enabled = false;
                }
                btnStartPort.Text = "Start";
                //UpdateLog(closeserial + success);
            }
        }

        private void btnMultiEPC_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxMulti.Checked == true)
                {
                    if (checkBoxSingle.Checked == true)
                    {
                        ReaderControllor.SingleEPC();
                    }
                    else
                    {
                        ReaderControllor.SatrtMultiEPC();
                    }
                }
                else
                {
                    if (checkBoxSingle.Checked == true)
                    {
                        ReaderControllor.SingleEPC(currentclient);
                    }
                    else
                    {
                        ReaderControllor.SatrtMultiEPC(currentclient);
                    }
                }
                //UpdateLog(start + multiepc + success);
            }
            catch (Exception ex)
            {
                //UpdateLog(ex.ToString());
                new LogWriter(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBoxMulti.Checked == true)
                {
                    if (checkBoxSingle.Checked == true)
                    {
                        ;
                    }
                    else
                    {
                        ReaderControllor.StopMultiEPC();
                        //UpdateLog(stop + multiepc + success);
                    }
                }
                else
                {
                    if (checkBoxSingle.Checked == true)
                    {
                        ;
                    }
                    else
                    {
                        ReaderControllor.StopMultiEPC(currentclient);
                        //UpdateLog(stop + multiepc + success);
                    }
                }
            }
            catch (Exception ex)
            {
                //UpdateLog(ex.ToString());
                new LogWriter(ex.ToString());
            }
        }
        public void loadData()
        {
            string connected;
            DatabaseConnection check = new DatabaseConnection();
            connected = check.checkDatabase();
            try
            {
                if (connected == "true")
                {
                    using (SqlConnection conn = new SqlConnection(DatabaseConnection.connectionStr))
                    {
                        var select = "SELECT * FROM Contacts ";
                        var dataAdapter = new SqlDataAdapter(select, conn);
                        var select2 = "SELECT * FROM Users ";
                        var dataAdapter2 = new SqlDataAdapter(select2, conn);

                        var commandBuilder = new SqlCommandBuilder(dataAdapter);
                        var commandBuilder2 = new SqlCommandBuilder(dataAdapter2);

                        var ds = new DataSet();
                        dataAdapter.Fill(ds);
                        dataGridView1.ReadOnly = true;
                        dataGridView1.DataSource = ds.Tables[0];
                        dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                        var ds2 = new DataSet();
                        dataAdapter2.Fill(ds2);
                        dgvClients.ReadOnly = true;
                        dgvClients.DataSource = ds2.Tables[0];
                        dgvClients.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
        public void ShowEPC(object sender, Command.ShowEPCEventArgs e)
        {
            try
            {
                _epc_t MultiID = e.MultiEPC;
                bool isexit = false;
                for (int index = 0; index < epcs_list.Count; index++)
                {
                    if ((epcs_list[index].epc == MultiID.epc) && (epcs_list[index].dev == MultiID.dev))
                    {
                        MultiID.count = epcs_list[index].count + 1;
                        epcs_list[index] = MultiID;
                        isexit = true;
                        break;
                    }
                }
                if (!isexit)
                {
                    epcs_list.Add(MultiID);
                }
            }
            catch(Exception ex)
            {
                new LogWriter(ex.ToString());
            }
        }
        

        private void timer_md_query_Tick_Tick_1(object sender, EventArgs e)
        {
            try
            {
                totalnum1 = 0;
                totaltime++;
                //label10.Text = totaltime.ToString();
                //epcs_list = ReaderControllor.GetMultiEPC();
                //label26_total.Text = epcs_list.Count.ToString();
                for (int index = 0; index < epcs_list.Count; index++)
                {
                    //转换成string
                    string str_epc = epcs_list[index].epc;
                    string str_pc = epcs_list[index].PC.ToString("X2");
                    string str_read_cnt = epcs_list[index].count.ToString();
                    string str_ant_id = epcs_list[index].antID.ToString();
                    string str_dev = epcs_list[index].dev;
                    string str_ip = epcs_list[index].ClientIP;
                    string str_time = epcs_list[index].time;
                    string str_rssi = epcs_list[index].RSSI.ToString("f1");
                    string direction = epcs_list[index].direction.ToString();
                    totalnum1 += epcs_list[index].count;
                    bool Exist = false;
                    int item_index = 0;
                    //判断标签是否被重复扫描
                    foreach (ListViewItem viewitem in this.listView_md_epc.Items)
                    {
                        if ((viewitem.SubItems[listView_md_epc_EPC].Text == str_epc) && (viewitem.SubItems[listView_md_epc_IP].Text == str_dev))
                        {
                            viewitem.SubItems[listView_md_epc_AntID].Text = str_ant_id;
                            viewitem.SubItems[listView_md_epc_Count].Text = str_read_cnt;
                            viewitem.SubItems[listView_md_epc_Last_Time].Text = str_time;
                            viewitem.SubItems[listView_md_epc_PC].Text = str_pc;
                            viewitem.SubItems[listView_md_epc_Rssi].Text = str_rssi;
                            viewitem.SubItems[listView_md_epc_Direction].Text = direction;
                            Exist = true;
                            break;
                        }
                        item_index++;
                    }
                    if (!Exist)
                    {
                        ListViewItem item = new ListViewItem((this.listView_md_epc.Items.Count + 1).ToString());
                        item.SubItems.Add(str_ant_id);
                        item.SubItems.Add(str_epc);
                        item.SubItems.Add(str_pc);
                        item.SubItems.Add(str_rssi);
                        item.SubItems.Add(str_read_cnt);
                        item.SubItems.Add(str_dev);
                        item.SubItems.Add(str_time);
                        item.SubItems.Add(direction);
                        this.listView_md_epc.Items.Add(item);
                        this.listView_md_epc.Items[this.listView_md_epc.Items.Count - 1].EnsureVisible();
                        this.listView_md_epc.Items[this.listView_md_epc.Items.Count - 1].Selected = true;
                        // this.listView_md_epc.Items[this.listView_md_epc.Items.Count - 1].BackColor = System.Drawing.Color.FromArgb(red, green, blue);
                    }
                }
                //label8.Text = (totalnum1 - totalnum2).ToString();
                totalnum2 = totalnum1;
            }
            catch(Exception ex)
            {
                new LogWriter(ex.ToString());
            }
        }
    }
}
