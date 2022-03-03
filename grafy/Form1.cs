
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace grafy
{
    public partial class Form1 : Form
    {
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
                Labels = new[] { "Leden", "Unor", "Březen", "Duben", "Květen", "Červen", "Červenec", "Srpen", "Září", "Říjen", "Listopad", "Prosinec" }
            });
            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Hodnota",
                LabelFormatter = value => value.ToString("C")
            });
            cartesianChart1.LegendLocation = LegendLocation.Right;
            radioButton1.Checked = true;
            UpdateGraph();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void datovybodBindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGraph();
        }

        private void InitData()
        {
            //TODO: zprovoznit!!!!
            dataGridView1.Rows[0].Cells[0].Value = "2020";
            dataGridView1.Rows[0].Cells[1].Value = "10";
            dataGridView1.Rows[0].Cells[2].Value = "15";
        }

        private void UpdateGraph    ()
        {
            if (datovybodBindingSource.DataSource == null) return;

            cartesianChart1.Series.Clear();
            SeriesCollection series = new SeriesCollection();
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

                    try
                    {
                        if (data.SingleOrDefault() != null)
                            value = data.SingleOrDefault().value;
                        values.Add(value);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Zadali jste 2x stejný měsíc");
                    }
                }
                //series.Add(new LineSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) });
                //series.Add(new ColumnSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) });
                if (radioButton1.Checked)
                {
                    series.Add(new LineSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) });
                }
                if (radioButton2.Checked)
                {
                    series.Add(new ColumnSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) });
                }
                if (radioButton3.Checked)
                {
                    series.Add(new StepLineSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) });
                }
            }
            cartesianChart1.Series = series;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        //EXPORT CSV
        private void expButton_Click(object sender, EventArgs e)
        {
            string CsvFpath = @"D:\Grafy.csv";
            try
            {
                System.IO.StreamWriter csvFileWriter = new StreamWriter(CsvFpath, false);

                string columnHeaderText = "";

                int countColumn = dataGridView1.ColumnCount - 1;

                if (countColumn >= 0)
                {
                    columnHeaderText = dataGridView1.Columns[0].HeaderText;
                }
                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + ',' + dataGridView1.Columns[i].HeaderText;
                }

                csvFileWriter.WriteLine(columnHeaderText);

                foreach (DataGridViewRow dataRowObject in dataGridView1.Rows)
                {
                    if (!dataRowObject.IsNewRow)
                    {
                        string dataFromGrid = "";

                        dataFromGrid = dataRowObject.Cells[0].Value.ToString();

                        for (int i = 1; i <= countColumn; i++)
                        {
                            dataFromGrid = dataFromGrid + ',' + dataRowObject.Cells[i].Value.ToString();
                        }
                        csvFileWriter.WriteLine(dataFromGrid);
                    }
                }


                csvFileWriter.Flush();
                csvFileWriter.Close();
            }
            catch (Exception exceptionObject)
            {
                MessageBox.Show(exceptionObject.ToString());
            }
            UpdateGraph();
        }


        //IMPORT CSV
        private void importButton_Click(object sender, EventArgs e)
        {
            UpdateGraph();
            openFileDialog1.ShowDialog();
            dataGridView1.Text = openFileDialog1.FileName;
            BindData(dataGridView1.Text);
            
        }
        private void BindData(string pathToFile)
        {
            DataTable dt = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(pathToFile);
            if (lines.Length > 0)
            {
                
                string firstLine = lines[0];
                string[] headerLabels = firstLine.Split(',');
                foreach (string headerWord in headerLabels)
                {
                    dt.Columns.Add(new DataColumn(headerWord));
                }
                
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] dataWords = lines[i].Split(',');
                    DataRow dr = dt.NewRow();
                    int columnIndex = 0;
                    foreach (string headerWord in headerLabels)
                    {
                        dr[headerWord] = dataWords[columnIndex++];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
            }
        }
    }

}
