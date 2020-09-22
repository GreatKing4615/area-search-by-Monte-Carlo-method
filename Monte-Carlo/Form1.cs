using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Monte_Carlo
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        int num = 0;

        public Form1()
        {
            InitializeComponent();
        }
     

        private void print_Figure()
        {


            chart1.Series[0].Points.Clear();
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart1.Series[0].ChartType = SeriesChartType.Line; // Тип графика - линия
            chart1.Series[0].BorderWidth = 5;

            List<List<int>> data_about_figure = input_data();
           switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                       
                        print_polygon(data_about_figure); 
                        break;
                    }
                case 1:
                    {

                        if (textBox1.Text == "")
                        {
                            MessageBox.Show("Введите радиус");
                        }
                        else
                        {
                            chart1.Series[0].ChartType = SeriesChartType.Point;
                            print_circle(Convert.ToInt32(textBox1.Text));
                        }
                        break;
                    }            
                default: break;
            }


        }

        private void print_polygon(List<List<int>> data_about_figure)
        {
            int[] temp = new int[2];
            
            chart1.Series[0].ChartType = SeriesChartType.Line; // Тип графика - линия
            foreach (List<int> coords in data_about_figure)
            {
                foreach (int x in coords)
                {
                    temp = coords.ToArray();
                    chart1.Series[0].Points.AddXY(temp[0], temp[1]);
                }
                
            }
            int[] finish = data_about_figure[0].ToArray();
            chart1.Series[0].Points.AddXY(finish[0], finish[1]);
        }

        //**РИСУНОК КРУГА**/
        private void print_circle( int R)
        {            
            for (double i = -R; i <= R; i+=0.01)
            {
                chart1.Series[0].Points.AddXY(i, Math.Sqrt((Math.Pow(R, 2) - Math.Pow(i, 2))));
                chart1.Series[0].Points.AddXY(i, -Math.Sqrt((Math.Pow(R, 2) - Math.Pow(i, 2))));
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            List<List<int>> data_about_figure = input_data();

            Monte_Carlo(data_about_figure);
        }

        private void Monte_Carlo(List<List<int>> data_about_figure)
        {

            int [] coords =print_square(data_about_figure);
            //coords[0]=дальний
            //coords[1]=высший
            //coords[2]=ближний
            //coords[3]=нижний

            select_scale(coords);
            
            if (random_textbox.Text == "")
            {
                MessageBox.Show("Введите количество точек");
            }
            else {

                print_random_point(coords);
                find_square(coords);
                


            }
        }

      
        private bool hit_check(double [] xy_point)
        {
            List<List<int>> all_coords = input_data();
            int[][] xy_coords = new int[all_coords.Count][];
            int[] xy_last_coords = new int[2];
            int i = 0;
            foreach(List<int> coord in all_coords)
            {xy_coords[i] = coord.ToArray();
                i++;
                // (((yp         <= y           and     y     < yp_prev       ) or (yp_prev         <=  y           and y          <    yp))      and (x        > (xp_prev          - xp)         * (y          -         yp)/(yp_prev          - yp)               + xp))
               // if (((xy_coords[1]<=xy_point[1] && xy_point[1]<xy_last_coords[1])||(xy_last_coords[1]<= xy_point[1] && xy_point[1] < xy_coords[1]))&&(xy_point[0]>(xy_last_coords[0]-xy_coords[0])*(xy_point[1]-xy_coords[1])/(xy_last_coords[1]-xy_coords[1])+xy_coords[0]))
            }

            return false;

        }

        private void find_square(int[] coords)
        {
            double square = (coords[0] - coords[2])* (coords[1] - coords[3]);
            textBox2.Text+= "Основная фигура=" + square.ToString();

        }

        private void print_random_point(int[] coords)
        {
            int random_count = Convert.ToInt32(random_textbox.Text);
            chart1.Series[2].BorderWidth = 10;
            chart1.Series[2].ChartType = SeriesChartType.Point;            
            double[] temp_point = new double[2];

            chart1.Series[2].Color = Color.Red;

            chart1.Series[3].BorderWidth = 10;
            chart1.Series[3].ChartType = SeriesChartType.Point;
            chart1.Series[3].Color = Color.DarkBlue;
            for (int i = 0; i < random_count; i++)
            {
                temp_point[0] = GetRandomNumber(coords[2], coords[0]);
                temp_point[1] = GetRandomNumber(coords[3], coords[1]);
                if (hit_check(temp_point))
                {
                    
                    chart1.Series[3].Points.AddXY(temp_point[0], temp_point[1]);
                }
                else
                {
                    
                    chart1.Series[2].Points.AddXY(temp_point[0], temp_point[1]);
                }
                //textBox2.Text = Convert.ToString(temp_point[0]) + "-" + temp_point[1].ToString();
                
                
            }

        }

        private void select_scale(int[] coords)
        {

            int scale;
            if (coords[0] - coords[2] >= coords[1] - coords[3])
            {
                scale = (coords[0] - coords[2]) / 2;
            }
            else
            {
                scale = (coords[1] - coords[3]) / 2;
            }
            chart1.ChartAreas[0].AxisX.Minimum = coords[2]; // настройка минимума и максимума оси X
            chart1.ChartAreas[0].AxisX.Maximum = coords[0];
            chart1.ChartAreas[0].AxisY.Minimum = coords[3];
            chart1.ChartAreas[0].AxisY.Maximum = coords[1];            
        }

        static double  GetRandomNumber(double minimum, double maximum)
        {
            RNGCryptoServiceProvider rNGCrypto = new RNGCryptoServiceProvider();
            Random random = new Random();
            byte[] byte_answer = new byte[1];
            rNGCrypto.GetBytes(byte_answer);
            double random_multiplyer = byte_answer[0] / 255d;
            double difference = maximum - minimum;
            double result=(double)(minimum + (random_multiplyer * difference));            
            return result;
        }

        private int [] print_square(List<List<int>> data_about_figure)
        {
            List<int> x_coord = new List<int>();
            List<int> y_coord = new List<int>();
            int[] temp = new int[2];
            foreach(List<int> coords in data_about_figure)
            {
                foreach(int x in coords)
                {
                    temp = coords.ToArray();
                    x_coord.Add(temp[0]);
                    y_coord.Add(temp[1]);
                }
            }
            x_coord.Sort();
            y_coord.Sort();

            chart1.Series[1].BorderWidth = 5;
            chart1.Series[1].ChartType = SeriesChartType.Line; // Тип графика - линия
            chart1.Series[1].Color=Color.Red;            
            
            //передаю координаты максимальных отклонений фигуры
            int[] answer = new int[4];
            if (comboBox1.SelectedIndex == 0)
            {
                chart1.Series[1].Points.AddXY(x_coord[0], y_coord[0]);
                chart1.Series[1].Points.AddXY(x_coord[x_coord.Count - 1], y_coord[0]);
                chart1.Series[1].Points.AddXY(x_coord[x_coord.Count - 1], y_coord[y_coord.Count - 1]);
                chart1.Series[1].Points.AddXY(x_coord[0], y_coord[y_coord.Count - 1]);
                chart1.Series[1].Points.AddXY(x_coord[0], y_coord[0]);

                //передаю координаты максимальных отклонений фигуры
                answer[0] = x_coord[x_coord.Count - 1];
                answer[1] = y_coord[y_coord.Count - 1];
                answer[2] = y_coord[0];
                answer[3] = y_coord[0];
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                chart1.Series[1].Points.AddXY(-Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox1.Text));
                chart1.Series[1].Points.AddXY(-Convert.ToInt32(textBox1.Text), -Convert.ToInt32(textBox1.Text));
                chart1.Series[1].Points.AddXY(Convert.ToInt32(textBox1.Text), -Convert.ToInt32(textBox1.Text));
                chart1.Series[1].Points.AddXY(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox1.Text));
                chart1.Series[1].Points.AddXY(-Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox1.Text));

                //передаю координаты максимальных отклонений фигуры
                answer[0] = Convert.ToInt32(textBox1.Text);
                answer[1] = Convert.ToInt32(textBox1.Text);
                answer[2] = -Convert.ToInt32(textBox1.Text);
                answer[3] = -Convert.ToInt32(textBox1.Text);
            }

            return answer;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            input_data();
            print_Figure();
            
        }

        //ВВод данных в лист
        private List<List<int>> input_data()
        {
            List<List<int>> data_about_figure = new List<List<int>>();
            for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
                data_about_figure.Add(new List<int> { Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value), Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value) });
            return data_about_figure;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // label2.Visible = false;
            //textBox1.Visible = false;
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            dataGridView1.Rows.Clear();

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {                       
                        break;
                    }
                case 1:
                    {                       
                        label2.Visible = true;
                        textBox1.Visible = true;                        
                        break;
                    }

                default: break;
            }
        }


      

       
    }
}
