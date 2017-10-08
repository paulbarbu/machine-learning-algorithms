using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: if we want to know if a centroid has learned:
// compute the density (boundingbox/num_points assigned to the current centromer)
// lower the bounding box around the centromer 10 times and compute the density once more
// we expect the density to be approx. the same or even increase if the algorithm learned,
//since we have more points around the center point
namespace generare_nr_aleatoare
{
    public partial class Form1 : Form
    {
        public Func<Point, Point, double> distanceFunc;

        public double euclidianDist(Point p, Point other)
        {
            double x = Math.Pow(p.X - other.X, 2) + Math.Pow(p.Y - other.Y, 2);
            return Math.Sqrt(x);
        }
        public double manhattanDist(Point p, Point other)
        {
            return Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y);
        }
        public double cosDist(Point p, Point other)
        {
            // normalization, make all points positive
            // divide each coord. to the sum in order for the resulting vector to be on the trigonometric circle
            double x = p.X + 300;
            double y = p.Y + 300;

            double sum = x + y;
            x = x / sum;
            y = y / sum;

            double ox = other.X + 300;
            double oy = other.Y + 300;
            double osum = ox + oy;
            ox = ox / osum;
            oy = oy / osum;

            double scalarProd = (x * ox) + (y * oy);
            double norma = Math.Sqrt((x * x) + (y * y)) * Math.Sqrt((ox * ox )+ (oy*oy));

            // for the cosine function we want the maximum, not the minimum, cos(90) = 1 and distance is large, whereas cos(0) = 0 and the distance is small (the vectors are close when talking about the angle)
            return 1 - scalarProd/norma;
        }

        struct Zone {
            // zone's "center" coordinates
            public int m_x;
            public int m_y;

            // the zone's dispersion on each axis
            public int sigma_x;
            public int sigma_y;

            public Pen pen;
        }

        class PaulPoint
        {
            public int zone;
            public Point p;

            public PaulPoint()
            {
                p = new Point();
            }
            public PaulPoint(int x, int y, int z)
            {
                p = new Point(x, y);
                zone = z;
            }

            public override string ToString()
            {
                return string.Format("{0} {1} {2}", p.X, p.Y, zone);
            }

        }

        class Centroid : PaulPoint
        {
            public List<PaulPoint> points = new List<PaulPoint>();

            public Point MassCenter()
            {
                Point center = new Point();

                if (points.Count > 0)
                {
                    foreach (PaulPoint p in points)
                    {
                        center.X += p.p.X;
                        center.Y += p.p.Y;
                    }

                    center.X /= points.Count;
                    center.Y /= points.Count;
                }
                else
                {
                    center = p;
                }

                return center;
            }
        }

        static private double Gauss(double x, int m, int sigma)
        {
            double pow = ((m - x) * (m - x)) / (2 * sigma * sigma);
            return Math.Pow(Math.E, -1 * pow);
        }

        private const int WIDTH = 600;
        private const int HEIGHT = 600;
        private const int NUM_POINTS = 3000;
        private Zone[] zones = new Zone[] {
            new Zone { m_x = 250, m_y = 250, sigma_x =  15, sigma_y = 5, pen = Pens.Red} ,
            new Zone { m_x = 150, m_y = 250, sigma_x =  5, sigma_y = 15, pen = Pens.Green } ,
            new Zone { m_x = 10, m_y = 10,sigma_x =  5, sigma_y = 5, pen = Pens.Blue}};

        private Random rand = new Random();
        private PaulPoint[] pts = new PaulPoint[NUM_POINTS];

        Centroid[] centroids;
        Color[] centroid_colors = new Color[] {  Color.Purple, Color.Crimson, Color.DarkOrange, Color.Brown, Color.Fuchsia, Color.Navy, Color.Black, Color.BlueViolet, Color.DarkKhaki, Color.ForestGreen, Color.MediumTurquoise };

        public Form1()
        {
            distanceFunc = euclidianDist;
            InitializeComponent();

            generatePoints();
            writePoints();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;

            Debug.WriteLine(cartesianToScreenCoords(new Point(0, 0)));
            Debug.WriteLine(cartesianToScreenCoords(new Point(-100, 0)));
            Debug.WriteLine(cartesianToScreenCoords(new Point(-300, -30)));
        }

        private void drawAxes(Graphics graphics)
        {
            Rectangle rectangle = new Rectangle(0, 0, WIDTH, HEIGHT);

            graphics.DrawRectangle(Pens.Black, rectangle);

            //x axis
            graphics.DrawLine(Pens.Black, new Point(0, HEIGHT / 2), new Point(WIDTH, HEIGHT / 2));

            //y axis
            graphics.DrawLine(Pens.Black, new Point(WIDTH / 2, 0), new Point(WIDTH / 2, HEIGHT));
        }

        private Point cartesianToScreenCoords(Point p)
        {
            return new Point(WIDTH / 2 + p.X, HEIGHT/2 - p.Y);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            drawAxes(e.Graphics);
            drawPoints(e.Graphics, pts, zones.Select(z => z.pen.Color).ToArray());

            //if this is not the first step, then the k-means button has been pressed
            if (centroids != null)
            {
                //color the points according to the center points they have been assigned to
                for(int i=0; i<centroids.Length; i++ )
                {
                    centroids[i].points.ForEach(p => p.zone = 0);
                    drawPoints(e.Graphics, centroids[i].points.ToArray(), new Color[] { centroid_colors[i] });
                }

                drawCentroids(e.Graphics, centroids, centroid_colors);
            }
        }

        private void generatePoints()
        {
            for(int i=0;i <pts.Length; ++i)
            {
                int zone_index = rand.Next(3);

                int x = genCoord(zones[zone_index].m_x, zones[zone_index].sigma_x);
                int y = genCoord(zones[zone_index].m_y, zones[zone_index].sigma_y);

                pts[i] = new PaulPoint(x, y, zone_index);
            }
        }

        private int genCoord(int m, int sigma)
        {
            double G;
            double pa;
            int coord;

            do
            {
                coord = rand.Next(-WIDTH / 2, WIDTH / 2 + 1);
                G = Gauss(coord, m, sigma);

                pa = rand.Next(0, 7000)/7000.0;
            }
            while (G <= pa);

            return coord;
        }

        private void writePoints()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("points.txt"))
            {
                for (int i = 0; i < pts.Length; i++)
                {
                    file.WriteLine(pts[i].ToString());
                }
            }
        }

        private void drawPoints(Graphics g, PaulPoint[] ps, Color[] colors)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                Point screenPoint = cartesianToScreenCoords(ps[i].p);
                g.DrawEllipse(new Pen(colors[ps[i].zone]), screenPoint.X, screenPoint.Y, 3, 3);
            }
        }

        private void drawCentroids(Graphics g, PaulPoint[] ps, Color[] colors)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                Point screenPoint = cartesianToScreenCoords(ps[i].p);

                //g.DrawString("X", new Font("Arial", 20), new SolidBrush(colors[pts[i].zone]), screenPoint.X, screenPoint.Y);
                g.DrawRectangle(Pens.Black, screenPoint.X-5, screenPoint.Y-5, 10, 10);
                g.FillRectangle(new SolidBrush(colors[ps[i].zone]), screenPoint.X-5, screenPoint.Y-2, 10, 4);
                g.FillRectangle(new SolidBrush(colors[ps[i].zone]), screenPoint.X - 2, screenPoint.Y - 5, 4, 10);
                g.DrawRectangle(Pens.White, screenPoint.X - 6, screenPoint.Y - 6, 12, 12);

                //new Rectangle()
                //g.FillRectangle(new Rectangle())
            }
        }

        private int getSimilarCentroid(Point p)
        {
            double min = double.MaxValue;
            int pos = -1;


            double dist;
            for (int i = 0; i < centroids.Length; i++)
            {
                dist = distanceFunc(centroids[i].p, p);
                if(dist < min)
                {
                    pos = i;
                    min = dist;
                }

                if(dist == Double.NaN)
                {

                    Debug.WriteLine("BUG");
                }
            }

            return pos;
        }


        double E = double.MaxValue;
        double lastE = 0;

        private double calcE()
        {
            double r = 0;
            foreach(Centroid c in centroids)
            {
                foreach(PaulPoint p in c.points)
                {
                    r += distanceFunc(c.p, p.p);
                }
            }

            return r;
        }

        private void kmeansBtn_Click(object sender, EventArgs e)
        {
            // start
            if(centroids == null)
            {
                // step 1
                int k = rand.Next(2, 11);


                //step 2
                centroids = new Centroid[k];

                for (int i = 0; i < centroids.Length; i++)
                {
                    centroids[i] = new Centroid();
                    centroids[i].p.X = rand.Next(-WIDTH / 2, WIDTH / 2 + 1);
                    centroids[i].p.Y = rand.Next(-WIDTH / 2, WIDTH / 2 + 1);
                    centroids[i].zone = i;
                }
            }

            // step 3
            foreach (Centroid c in centroids)
            {
                c.points.Clear();
            }

            for (int i = 0; i < pts.Length; i++)
            {
                int similar_centroid = getSimilarCentroid(pts[i].p);
                centroids[similar_centroid].points.Add(pts[i]);
            }

            for (int i = 0; i < centroids.Length; i++)
            {
                Debug.WriteLine("{0} -> {1}", centroids[i].p, centroids[i].MassCenter());
                centroids[i].p = centroids[i].MassCenter();
            }

            pictureBox1.Refresh();

            lastE = E;
            E = calcE();
            Debug.WriteLine("E: {0}, LastE: {1}, Diff: {2}", E, lastE, Math.Abs(lastE - E));

            if (lastE - E == 0)
            {
                kmeansBtn.Enabled = false;
            }
        }

        private void euclidRadio_CheckedChanged(object sender, EventArgs e)
        {
            if(euclidRadio.Checked)
            {
                distanceFunc = euclidianDist;
            }
        }

        private void manhattanRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (manhattanRadio.Checked)
            {
                distanceFunc = manhattanDist;
            }
        }

        private void cosRadio_CheckedChanged(object sender, EventArgs e)
        {
            if(cosRadio.Checked)
            {
                distanceFunc = cosDist;
            }
        }
    }
}
