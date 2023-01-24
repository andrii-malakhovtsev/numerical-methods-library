using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NumericalMethods
{
    public partial class GraphicsForm : Form
    {
        public GraphicsForm() { InitializeComponent(); }

        private GraphicsForm(double[] x, double[] y, string title)
        {
            SetStandartValues();
            GraphicsChart.Titles[0].Text = title;
            var firstSeries = new Series();
            int width = x.Length - 1;
            double functionMin = double.MaxValue, functionMax = double.MinValue;
            for (int i = 0; i <= width; i++)
            {
                firstSeries.Points.AddXY(x[i], y[i]);
                functionMin = Math.Min(functionMin, y[i]); functionMax = Math.Max(functionMax, y[i]);
            }
            SetStandrartOptions(firstSeries, 
                chartType: SeriesChartType.Spline, 
                markerStyle: MarkerStyle.None, 
                borderWidth: 3, 
                color: Color.DarkBlue,
                axisXMin: x[0], 
                axisXMax: x[width],
                axisYMin: functionMin, 
                axisYMax: functionMax);
            GraphicsChart.Invalidate();
        }

        private GraphicsForm(Table table)
        {
            SetStandartValues();
            var firstSeries = new Series(); 
            int width = table.Length - 1;
            double functionMin = double.MaxValue, functionMax = double.MinValue;
            for (int i = 0; i <= width; i++)
            {
                firstSeries.Points.AddXY(table.X(i), table.F(i));
                functionMin = Math.Min(functionMin, table.F(i)); functionMax = Math.Max(functionMax, table.F(i));
            }
            SetStandrartOptions(firstSeries, 
                chartType: SeriesChartType.Point,
                markerStyle: MarkerStyle.Circle, 
                borderWidth: 3, 
                color: Color.DarkBlue,
                axisXMin: table.X(0), 
                axisXMax: table.X(width),
                axisYMin: functionMin, 
                axisYMax: functionMax);
            GraphicsChart.Invalidate();
        }

        private GraphicsForm(Table firstTable, Table secondTable, string title)
        {
            SetStandartValues();
            GraphicsChart.Titles[0].Text = title;
            var fisrtSeries = new Series();
            int firstTableWidth = SetSeries(firstTable, fisrtSeries, markerColor: Color.Red,
                markerBorderColor: Color.DarkRed);
            GraphicsChart.Series[0] = fisrtSeries;
            var secondSeries = new Series();
            int secondTableWidth = SetSeries(secondTable, secondSeries, markerColor: Color.Blue,
                markerBorderColor: Color.DarkBlue);
            GraphicsChart.Series[1] = secondSeries;
            double fMin = Math.Min(firstTable.Min.F, secondTable.Min.F),
                   fMax = Math.Max(firstTable.Max.F, secondTable.Max.F),
                   xMin = Math.Min(firstTable.Points[0].X, secondTable.Points[0].X),
                   xMax = Math.Max(firstTable.Points[firstTableWidth].X, secondTable.Points[secondTableWidth].X);
            GraphicsChart.ChartAreas[0].AxisX.Minimum = Math.Floor(xMin);
            GraphicsChart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(xMax);
            GraphicsChart.ChartAreas[0].AxisY.Minimum = Math.Floor(fMin);
            GraphicsChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(fMax);
            if ((xMax - xMin) < 1.0)
                GraphicsChart.ChartAreas[0].AxisX.Interval = 0.1;
            else
                GraphicsChart.ChartAreas[0].AxisX.Interval = Math.Ceiling((xMax - xMin) / 10);
            if ((fMax - fMin) < 1.0)
                GraphicsChart.ChartAreas[0].AxisY.Interval = 0.1;
            else
                GraphicsChart.ChartAreas[0].AxisY.Interval = Math.Ceiling((fMax - fMin) / 10);
            GraphicsChart.Invalidate();
        }

        private static int SetSeries(Table table, Series series, Color markerColor, Color markerBorderColor)
        {
            const int markerSize = 5;
            int tableWidth = table.Length - 1;
            for (int i = 0; i <= tableWidth; i++)
            {
                series.Points.AddXY(table.Points[i].X, table.Points[i].F);
            }
            series.ChartType = SeriesChartType.Point;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = markerSize;
            series.MarkerColor = markerColor;
            series.MarkerBorderColor = markerBorderColor;
            return tableWidth;
        }

        private void SetStandartValues()
        {
            InitializeComponent();
            GraphicsChart.Series[0].Points.Clear();
            GraphicsChart.Series[1].Points.Clear();
            GraphicsChart.ChartAreas[0].AxisX.Interval = 1.0;
            GraphicsChart.ChartAreas[0].AxisY.Interval = 1.0;
        }

        private void SetStandrartOptions(Series firstSeries, SeriesChartType chartType,
            MarkerStyle markerStyle, int borderWidth, Color color, double axisXMin,
            double axisXMax, double axisYMin, double axisYMax)
        {
            firstSeries.ChartType = chartType;
            firstSeries.MarkerStyle = markerStyle;
            firstSeries.BorderWidth = borderWidth;
            firstSeries.Color = color;
            GraphicsChart.Series[0] = firstSeries;
            GraphicsChart.ChartAreas[0].AxisX.Minimum = Math.Floor(axisXMin);
            GraphicsChart.ChartAreas[0].AxisX.Maximum = Math.Ceiling(axisXMax);
            GraphicsChart.ChartAreas[0].AxisY.Minimum = Math.Floor(axisYMin);
            GraphicsChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(axisYMax);
        }

        public static void SingleGraphic(double[] X, double[] Y, string title, int Nx, int Ny)
        {
            var singleGraphic = new GraphicsForm(X, Y, title);
            SetGraphic(singleGraphic, Nx, Ny);
        }

        public static void SingleGraphic(Table table, int Nx, int Ny)
        {
            if (table == null) return;
            var singleGraphic = new GraphicsForm(table);
            SetGraphic(singleGraphic, Nx, Ny);
        }

        public static void DoublyGraphic(Table firstTable, Table secondTable, string title, int Nx, int Ny)
        {
            if (firstTable == null || secondTable == null) return;
            var doublyGraphic = new GraphicsForm(firstTable, secondTable, title);
            SetGraphic(doublyGraphic, Nx, Ny);
        }

        private static void SetGraphic(GraphicsForm graphic, int Nx, int Ny)
        {
            graphic.StartPosition = FormStartPosition.Manual;
            graphic.Location = new Point(Nx, Ny);
            graphic.ShowDialog();
        }
    }
}
