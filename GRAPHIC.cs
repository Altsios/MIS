using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IndSet
{
    public partial class Graphic : Form
    {
        double tim1;
        double tim2;
        double tim3;
        double tim4;
        double tim5;
        double tim6;
        double tim7;
        double tim8;
        double tim9;
        double tim10;


        public Graphic(string tim1, string tim2, string tim3, string tim4, string tim5,
                       string tim6, string tim7, string tim8, string tim9,string tim10)
        {
            this.tim1 =Convert.ToDouble(tim1.Substring(6));
            this.tim2 = Convert.ToDouble(tim2.Substring(6));
            this.tim3 = Convert.ToDouble(tim3.Substring(6));
            this.tim4 = Convert.ToDouble(tim4.Substring(6));
            this.tim5 = Convert.ToDouble(tim5.Substring(6));
            this.tim6 = Convert.ToDouble(tim6.Substring(6));
            this.tim7 = Convert.ToDouble(tim7.Substring(6));
            this.tim8 = Convert.ToDouble(tim8.Substring(6));
            this.tim9 = Convert.ToDouble(tim9.Substring(6));
            this.tim10 = Convert.ToDouble(tim10.Substring(6));
            InitializeComponent();


        }

        private void Graphic_Load(object sender, EventArgs e)
        {
            Axis ax = new Axis();
            ax.Title = "Вершины";
            chart1.ChartAreas[0].AxisX = ax;
            Axis ay = new Axis();
            ay.Title = "Время";
            chart1.ChartAreas[0].AxisY = ay;
            int cnt1 = 1000;
            int cnt2 = 5000;
            int cnt3 = 30000;
            int cnt4 = 50000;
            int cnt5 = 100000;
            chart1.Series[0].Points.AddXY(cnt1, tim1);
            chart1.Series[0].Points.AddXY(cnt2, tim2);
            chart1.Series[0].Points.AddXY(cnt3, tim3);
            chart1.Series[0].Points.AddXY(cnt4, tim4);
            chart1.Series[0].Points.AddXY(cnt5, tim5);

            //параллельный
            chart1.Series[1].Points.AddXY(cnt1, tim6);
            chart1.Series[1].Points.AddXY(cnt2, tim7);
            chart1.Series[1].Points.AddXY(cnt3, tim8);
            chart1.Series[1].Points.AddXY(cnt4, tim9);
            chart1.Series[1].Points.AddXY(cnt5, tim10);

            //выбор максимального, чтобы график нормально смотрелся

            chart1.ChartAreas[0].AxisY.Minimum = -tim5/2;
        }
    }
}
