
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
using System.Drawing;
using System.Drawing.Printing;

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
                    case 1: series.Add(new ColumnSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) }); break;
                    case 2: series.Add(new LineSeries() { Title = year.Year.ToString(), Values = new ChartValues<double>(values) }); break;
                    case 3: cartesianChart1.Visible = false; break;
                    default: MessageBox.Show("není vybrán graf"); break;
                }
                
               
            }
            cartesianChart1.Series = series;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checker = 1;
            UpgradeGraph();


        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checker = 2;
            UpgradeGraph();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create document
            PrintDocument _document = new PrintDocument();
            // Add print handler
            _document.PrintPage += new PrintPageEventHandler(Document_PrintPage);
            // Create the dialog to display results
            PrintPreviewDialog _dlg = new PrintPreviewDialog();
            _dlg.ClientSize = new System.Drawing.Size(Width / 2, Height / 2);
            _dlg.Location = new System.Drawing.Point(Left, Top);
            _dlg.MinimumSize = new System.Drawing.Size(375, 250);
            _dlg.UseAntiAlias = true;
            // Setting up our document
            _dlg.Document = _document;
            // Show it
            _dlg.ShowDialog(this);
            // Dispose document
            _document.Dispose();

        }
        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Create Bitmap according form size
            Bitmap _bitmap = new Bitmap(cartesianChart1.Width, cartesianChart1.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            // Draw from into Bitmap DC
            this.DrawToBitmap(_bitmap, this.DisplayRectangle);
            // Draw Bitmap into Printer DC
            e.Graphics.DrawImage(_bitmap, 0, 0);
            // No longer deeded - dispose it
            _bitmap.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Bitmap bmp = new Bitmap(cartesianChart1.Width, cartesianChart1.Height);
            cartesianChart1.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            bmp.Save("myfile.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }

}
