﻿
//ctrl k c
using LiveCharts.Wpf;
using LiveCharts;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System;

namespace grafy
{
    public partial class Form1 : Form
    {
        public int checker = 1;
        private SeriesCollection series;
        public Form1()
        {
            InitializeComponent();
            //InitData();

        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            datovybodBindingSource.DataSource = new List<Datovybod>();
            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Měsíc",
                Labels = new [] {"Leden","Unor","Březen","Duben"}
            }) ;
            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Hodnota",
                LabelFormatter = value => value.ToString("C")
            }) ;
            cartesianChart1.LegendLocation = LegendLocation.Right;

            }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            
        }

        private void datovybodBindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            UpgradeGraph();
        
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpgradeGraph();
          
        }

        private void InitData()
        {
            //TODO: zprovoznit!!!!
            dataGridView1.Rows[0].Cells[0].Value = "2020";
            dataGridView1.Rows[0].Cells[1].Value = "10";
            dataGridView1.Rows[0].Cells[2].Value = "15";
        }

        private void UpgradeGraph()
        {
          
            if (datovybodBindingSource.DataSource == null) return;

            cartesianChart1.Series.Clear();
            series = new SeriesCollection();
            var years = (from o in datovybodBindingSource.DataSource as List<Datovybod>
                         select new { Year = o.year }).Distinct();
            foreach (var year in years)
            {
                List<double> values = new List<double>();
                for (int month = 1; month <= 12; month++)
                {
                    double value = 0;
                    var data = from o in datovybodBindingSource.DataSource as List<Datovybod>
                               where o.year.Equals(year.Year) && o.month.Equals(month)
                               orderby o.month ascending
                               select new { o.value, o.month };
                    //TODO: ošetřit více měsíců v jednom roce
                    if (data.SingleOrDefault() != null)
                        value = data.SingleOrDefault().value;
                        values.Add(value);
                }
               
                switch (checker)
                {
                    case 1: 
                        series.Add(new ColumnSeries() { 
                            Title = year.Year.ToString(), 
                            Values = new ChartValues<double>(values)
                        });
                        cartesianChart1.Series = series;
                        break;
                    case 2: 
                        series.Add(new LineSeries() { 
                            Title = year.Year.ToString(), 
                            Values = new ChartValues<double>(values)
                        });
                        cartesianChart1.Series = series;
                        break;
                    case 3: 
                        series.Add(new PieSeries { 
                            Title = year.Year.ToString(), 
                            Values = new ChartValues<double>(values),
                            DataLabels = true,
                        });
                        pieChart1.Series = series;
                        break;
                    default: MessageBox.Show("není vybrán graf"); break;
                }
                
               
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checker = 1;
            UpgradeGraph();
            cartesianChart1.Visible = true;
            pieChart1.Visible = false;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checker = 2;
            UpgradeGraph();
            cartesianChart1.Visible = true;
            pieChart1.Visible = false;

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            checker = 3;
            UpgradeGraph();
            cartesianChart1.Visible = false;
            pieChart1.Visible = true;
        }
    }

}
