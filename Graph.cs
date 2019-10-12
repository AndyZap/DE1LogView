using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Drawing.Drawing2D;

namespace DE1LogView
{
    /*
    GraphPainter G_Orders = null;

    // in FormLoad --
    G_Orders = new GraphPainter(splitContainer3.Panel1, this.Font);
    G_Orders.SetAxisTitles("BTC_ETH price", "Order Volume");



    // paint utils
    private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
    {
    //if(G_Orders != null)
    //    G_Orders.Plot(e.Graphics);
    }
    private void splitContainer3_Panel1_MouseLeave(object sender, EventArgs e)
    {
    //labMktGraphPos.Text = "";
    }
    private void splitContainer3_Panel1_MouseMove(object sender, MouseEventArgs e)
    {
    //labMktGraphPos.Text = G_Orders.ToDataX(e.X).ToString("0.######") + ", " + G_Orders.ToDataY(e.Y).ToString("0.#");
    }

    // Plotting/printing market data as it arrives
    private void PlotMktData()
    {
    G_Orders.data.Clear();

    if(MktResultsAll.Count == 0)
    {
    G_Orders.SetAutoLimits();
    return;
    }

    foreach (ConsolidatedOrderBookEntry entry in MktResultsAll)
    {
    List<double> x = new List<double>();
    List<double> y = new List<double>();

    x.Add(entry.price);
    y.Add(0.0);
    x.Add(entry.price);
    y.Add(entry.is_bid ? entry.volume : -entry.volume);

    if (entry.exch == ExchangeEnum.KRK)
    G_Orders.SetData(G_Orders.data.Count, x, y, Color.Blue, 3, GraphPainter.Style.Solid);
    else if (entry.exch == ExchangeEnum.PLN)
    G_Orders.SetData(G_Orders.data.Count, x, y, Color.Green, 3, GraphPainter.Style.Solid);
    }

    G_Orders.SetAutoLimits();

    List<double> x0 = new List<double>();
    List<double> y0 = new List<double>();
    x0.Add(G_Orders.xmin);
    y0.Add(0);
    x0.Add(G_Orders.xmax);
    y0.Add(0);
    G_Orders.SetData(G_Orders.data.Count, x0, y0, Color.Black, 2, GraphPainter.Style.Solid);

    }
    */

    public class GraphPainter
    {
        public class Data
        {
            public List<double> x = new List<double>();
            public List<double> y = new List<double>();

            public Color color;
            public int size;
            public DashStyle style;
        }

        public Panel panel = null;                  // panel to paint
        public Font font = null;
        public double xmin, xmax, ymin, ymax;       // data limits to paint
        public List<Data> data = new List<Data>();  // the data itself

        public int border_x1 = 50, border_y1 = 50;         // graph borders
        public int border_x2 = 20, border_y2 = 30;
        int g_size_x, g_size_y;                     // size of graph in pixels
        double x_scale = 1, y_scale = 1;            // ratio of graph size to data size
        string x_title = "", y_title = "";

        public GraphPainter(Panel p, Font f)
        {
            panel = p;
            font = f;
        }

        public void SetAxisTitles(string xt, string yt)
        {
            x_title = xt;
            y_title = yt;
        }

        // NB: set "pusition" to value >= data.Count to add data instead of updating it
        public void SetData(int position, List<double> x, List<double> y, Color c, int s, DashStyle st)
        {
            if (x.Count() == 0 || x.Count() != y.Count())
                return;

            Data g = null;
            if (position < data.Count)
                g = data[position];
            else
            {
                g = new Data();
                data.Add(g);
            }
            g.x.Clear();
            g.y.Clear();
            foreach (double d in x)
                g.x.Add(d);
            foreach (double d in y)
                g.y.Add(d);

            g.color = c;
            g.size = s;
            g.style = st;
        }

        public void SetAutoLimits()
        {
            if (data.Count == 0)
                return;

            xmin = double.MaxValue;
            ymin = double.MaxValue;
            xmax = double.MinValue;
            ymax = double.MinValue;

            foreach (Data g in data)
            {
                foreach (double x in g.x)
                {
                    xmin = Math.Min(xmin, x);
                    xmax = Math.Max(xmax, x);
                }
                foreach (double y in g.y)
                {
                    ymin = Math.Min(ymin, y);
                    ymax = Math.Max(ymax, y);
                }
            }
            if (xmin == xmax)
            {
                if (xmin == 0.0)
                {
                    xmin -= 0.1;
                    xmax += 0.1;
                }
                else
                {
                    xmin *= 0.9;
                    xmax *= 1.1;
                }
            }
            if (ymin == ymax)
            {
                if (ymin == 0.0)
                {
                    ymin -= 0.1;
                    ymax += 0.1;
                }
                else
                {
                    ymin *= 0.9;
                    ymax *= 1.1;
                }
            }
        }

        public void SetAutoYLimits(double _xmin, double _xmax)
        {
            xmin = _xmin;
            xmax = _xmax;

            if (data.Count == 0)
                return;

            ymin = double.MaxValue;
            ymax = double.MinValue;

            foreach (Data g in data)
            {
                for (int i = 0; i < g.x.Count; i++)
                {
                    if (g.x[i] > xmax || g.x[i] < xmin)
                        continue;

                    ymin = Math.Min(ymin, g.y[i]);
                    ymax = Math.Max(ymax, g.y[i]);
                }
            }
            if (ymin == ymax)
            {
                if (ymin == 0.0)
                {
                    ymin -= 0.1;
                    ymax += 0.1;
                }
                else
                {
                    ymin *= 0.9;
                    ymax *= 1.1;
                }
            }
        }
        public void SetManualLimits(double x1, double x2, double y1, double y2)
        {
            xmin = x1;
            xmax = x2;
            ymin = y1;
            ymax = y2;

            if (xmin >= xmax || ymin >= ymax)
                SetAutoLimits();
        }

        public void CalcScales()
        {
            g_size_x = panel.Width - border_x1 - border_x2;
            g_size_y = panel.Height - border_y1 - border_y2;

            if (xmin == double.MaxValue || xmax == double.MinValue)
                x_scale = 1;
            else
                x_scale = g_size_x / (xmax - xmin);

            if (ymin == double.MaxValue || ymax == double.MinValue)
                y_scale = 1;
            else
                y_scale = g_size_y / (ymax - ymin);
        }

        public int ToGraphX(double x)
        {
            if (g_size_x < 2)
                return 0;

            return (int)(0.5 + border_x1 + (x - xmin) * x_scale);
        }
        public int ToGraphY(double y)
        {
            if (g_size_y < 2)
                return 0;

            return (int)(0.5 + panel.Height - border_y1 - (y - ymin) * y_scale);
        }
        public double ToDataX(int i)
        {
            if (g_size_x < 2)
                return 0;

            return xmin + ((i - border_x1) / x_scale);
        }
        public double ToDataY(int i)
        {
            if (g_size_y < 2)
                return 0;

            return ymin - ((i - panel.Height + border_y1) / y_scale);
        }

        public void Plot(Graphics g)
        {
            g.Clear(Color.White);
            CalcScales();

            if (xmax <= xmin || ymax <= ymin)
                return;

            PlotXAxis(g);
            PlotYAxis(g);

            if (g_size_x < 1 || g_size_y < 1 || data.Count == 0)
                return;

            foreach (Data d in data)
                PlotSeries(g, d);

            PlotXAxisLineOnly(g); // to cover 0 target lines
        }

        double GetNiceTickDistance(double range)
        {
            double power10_guess = Math.Pow(10, (int)Math.Log10(range / 5));

            int num_ticks = (int)(range / power10_guess);
            if (num_ticks > 15)
                return power10_guess * 5;
            else if (num_ticks > 8)
                return power10_guess * 2;
            else
                return power10_guess;
        }


        public void PlotXAxis(Graphics g)
        {
            Pen p = new Pen(Color.Black, 2);
            Pen p_grid = new Pen(Color.Silver, 1); p_grid.DashStyle = DashStyle.Dash;

            g.DrawLine(p, border_x1, border_y2 + g_size_y,
            border_x1 + g_size_x, border_y2 + g_size_y);

            if (x_title != "")
            {
                Brush b = new SolidBrush(Color.Black);
                g.DrawString(x_title, font, b, (float)(border_x1 + g_size_x / 2.0), (float)(panel.Height - border_y1 / 2.0));
            }

            // plot ticks
            double tick = GetNiceTickDistance(xmax - xmin);
            double first_tick = ((int)(xmin / tick)) * tick;
            for (double t = first_tick; t <= xmax; t += tick)
            {
                if (t < xmin)
                    continue;

                g.DrawLine(p, ToGraphX(t), border_y2 + g_size_y,
                ToGraphX(t), border_y2 + g_size_y + 5);

                Brush b = new SolidBrush(Color.Black);
                SizeF sf = g.MeasureString(t.ToString(), font);
                g.DrawString(t.ToString(), font, b, (float)(ToGraphX(t) - sf.Width / 2), (float)(border_y2 + g_size_y + 7));


                g.DrawLine(p_grid, ToGraphX(t), border_y2,
                ToGraphX(t), border_y2 + g_size_y);
            }
        }

        public void PlotXAxisLineOnly(Graphics g)
        {
            Pen p = new Pen(Color.Black, 2);
            g.DrawLine(p, border_x1, border_y2 + g_size_y,
            border_x1 + g_size_x, border_y2 + g_size_y);
        }

        public void PlotYAxis(Graphics g)
        {
            Pen p = new Pen(Color.Black, 2);
            Pen p_grid = new Pen(Color.Silver, 1); p_grid.DashStyle = DashStyle.Dash;
            g.DrawLine(p, border_x1, border_y2,
            border_x1, border_y2 + g_size_y);

            if (y_title != "")
            {
                Brush b = new SolidBrush(Color.Black);
                g.DrawString(y_title, font, b, 2f, 2f);
            }

            // plot ticks
            double tick = GetNiceTickDistance(ymax - ymin);
            double first_tick = ((int)(ymin / tick)) * tick;
            for (double t = first_tick; t <= ymax; t += tick)
            {
                if (t < ymin)
                    continue;

                g.DrawLine(p, border_x1 - 5, ToGraphY(t),
                border_x1, ToGraphY(t));

                Brush b = new SolidBrush(Color.Black);
                SizeF sf = g.MeasureString(t.ToString(), font);
                g.DrawString(t.ToString(), font, b, (float)(border_x1 - 8 - sf.Width), (float)(ToGraphY(t) - sf.Height / 2));

                g.DrawLine(p_grid, border_x1, ToGraphY(t),
                border_x1 + g_size_x, ToGraphY(t));
            }
        }
        public void PlotSeries(Graphics g, Data d)
        {
            Pen p = new Pen(d.color, d.size);
            p.DashStyle = d.style;
            List<Point> points = new List<Point>();
            for (int i = 0; i < d.x.Count; i++)
            {
                if (d.x[i] < xmin || d.x[i] > xmax)
                    continue;
                points.Add(new Point(ToGraphX(d.x[i]), ToGraphY(d.y[i])));
            }

            g.DrawLines(p, points.ToArray());
        }
    }
}
