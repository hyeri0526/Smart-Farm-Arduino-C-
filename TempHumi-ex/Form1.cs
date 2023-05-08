using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;


namespace TempHumi_ex
{
    public partial class Form1 : Form
    {
        static string Conn = "Server=localhost;port=3307;Database=smart_farm;Uid=root;Pwd=1234;Charset=utf8";
        MySqlConnection conn = new MySqlConnection(Conn);
        MySqlCommand cmd;  //sql문장을 실행시킬때    
        MySqlDataReader reader;   //sql문장을 실행시키고 결과받을때

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                MessageBox.Show("입력하세요.");
            }
            else {
                serialPort1.PortName = comboBox1.Text;
                if (serialPort1.IsOpen == false)
                {
                    serialPort1.Open();
                    conn.Open();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string led_state = "0";
            string motor_state = "0";            

            this.Invoke(new MethodInvoker(delegate ()
            {
                string rawdata = serialPort1.ReadLine();               
                string[] data = rawdata.Split('/');
                string date = "";

                //배열의 길이가 5개일때만 수행하기
                if (data.Length == 5)
                {
                    //MessageBox.Show(int.Parse(data[0]).ToString());
                    if (int.Parse(data[0]) <= 1023)
                    {                        
                        label9.Text = data[0];             // cds값
                        label5.Text = data[1];             // 온도값            
                        label4.Text = data[2];             // 습도값
                        led_state = data[3].ToString();    //LED상태값
                        motor_state = data[4].ToString();  //선풍기상태값

                        if (led_state == "1")              //LED 켜진 상태
                            label12.Text = "ON";                            
                        else
                            label12.Text = "OFF";

                        if (motor_state == "1")            //선풍기 켜진 상태
                            label13.Text = "ON";
                        else
                            label13.Text = "OFF";

                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        
                        label7.Text = date;

                        //DB에 insert
                        string insert_sql = "insert into smart_farm(cds,temp, humi, date) values (@cds,@temp, @humi, @date)";
                        if (reader != null) reader.Close();
                        cmd = new MySqlCommand();
                        cmd.Connection = conn;

                        cmd.CommandText = insert_sql;
                        cmd.Parameters.AddWithValue("@cds", data[0]);
                        cmd.Parameters.AddWithValue("@humi", data[1]);
                        cmd.Parameters.AddWithValue("@temp", data[2]);
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.ExecuteNonQuery();
                    }
                } 
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //1. sql 문을 만든다(조회)
            //2. 연결된 conn에 sql을 요청한다.
            //3. 실행요청해서 나온 결과를 그리드에 출력한다.

            dataGridView1.Rows.Clear();

            String sql1 = "SELECT num, cds, temp, humi, date FROM smart_farm ";
            if (reader != null) reader.Close();
            cmd = new MySqlCommand();  //cmd sql위한 준비작업
            cmd.Connection = conn;
            cmd.CommandText = sql1;   //실행시킬 sql문장이 무엇인지 지정
            
            reader = cmd.ExecuteReader();
            int i = 0;
            while (reader.Read() == true)
            {
                //read해서 data가 읽히면 계속 작업
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (int)reader["num"];
                dataGridView1.Rows[i].Cells[1].Value = (int)reader["cds"];
                dataGridView1.Rows[i].Cells[2].Value = (double)reader["temp"];
                dataGridView1.Rows[i].Cells[3].Value = (double)reader["humi"];
                dataGridView1.Rows[i].Cells[4].Value = (String)reader["date"];
                i++;
            }
            if (i == 0)
            {
                MessageBox.Show("조회될 data가 없습니다.");
            }
        }

    }
}
