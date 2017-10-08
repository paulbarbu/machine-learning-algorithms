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
        public double euclidianDistDouble(PointF p, PointF other)
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

        class Neuron
        {
                public PointF p;

            public Neuron(double x, double y)
            {
                p = new PointF((float)x, (float)y);
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
        private const int NUM_X_NEURONS = 10;
        private const int NUM_Y_NEURONS = 10;
        private Zone[] zones = new Zone[] {
            new Zone { m_x = 250, m_y = 250, sigma_x =  15, sigma_y = 5, pen = Pens.Red} ,
            new Zone { m_x = -150, m_y = 250, sigma_x =  5, sigma_y = 15, pen = Pens.Green } ,
            new Zone { m_x = 10, m_y = 10,sigma_x =  5, sigma_y = 5, pen = Pens.Blue},
            new Zone { m_x = 250, m_y = -250, sigma_x =  15, sigma_y = 5, pen = Pens.Black} ,
            new Zone { m_x = -150, m_y = -250, sigma_x =  5, sigma_y = 15, pen = Pens.Gold } ,};

        private Random rand = new Random();
        private PaulPoint[] pts = new PaulPoint[NUM_POINTS];

        Neuron[][] neurons1 = new Neuron[NUM_Y_NEURONS][];

        int t = 0; // current epoch

        int N = 50; //the number of epochs we want to learn in
        int T = 50*10; //max epoch

        public Form1()
        {
            distanceFunc = euclidianDist;
            InitializeComponent();

            for(int i=0;i<10;i++)
            {
                neurons1[i] = new Neuron[NUM_X_NEURONS];
            }
            generateNeurons();
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

            drawNeurons(e.Graphics);
        }

        private void generatePoints()
        {
            for (int i = 0; i < pts.Length; ++i)
            {
                int zone_index = rand.Next(zones.Length);

                int x = genCoord(zones[zone_index].m_x, zones[zone_index].sigma_x);
                int y = genCoord(zones[zone_index].m_y, zones[zone_index].sigma_y);

                pts[i] = new PaulPoint(x, y, zone_index);
            }
        }
        private void generateNeurons()
        {

            for (int i = -NUM_Y_NEURONS/2; i < NUM_Y_NEURONS/2; ++i)
            {
                for (int j = -NUM_X_NEURONS/2; j < NUM_X_NEURONS/2; ++j)
                {
                    float offsetY = (float)(HEIGHT*0.9) / (float)NUM_Y_NEURONS;
                    float offsetX = (float)(WIDTH*0.9) / (float)NUM_X_NEURONS;
                    int x = (int)(j*offsetX + offsetX);
                    int y = (int)(i*offsetY + offsetY);

                    //neurons1[i+NUM_Y_NEURONS/2][j+NUM_X_NEURONS/2] = new Neuron(x, y);
                    neurons1[i + NUM_Y_NEURONS / 2][j + NUM_X_NEURONS / 2] = new Neuron(rand.Next(-WIDTH, WIDTH), rand.Next(-WIDTH, WIDTH));
                }
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

        private void drawNeurons(Graphics g)
        {
            for (int i = 0; i < NUM_Y_NEURONS; i++)
            {
                for (int j = 0; j < NUM_X_NEURONS; j++)
                {
                    Point p = new Point((int)neurons1[i][j].p.X, (int)neurons1[i][j].p.Y);
                    Point screenPoint = cartesianToScreenCoords(p);

                    //g.DrawEllipse(Pens.Brown, screenPoint.X - 2, screenPoint.Y - 2, 4, 4);
                }
            }

            for (int i = 0; i < NUM_Y_NEURONS; i++)
            {
                var query = from n in neurons1[i] select cartesianToScreenCoords(new Point((int)n.p.X, (int)n.p.Y));
                g.DrawLines(Pens.Brown, query.ToArray());
            }

            for(int j=0; j < NUM_X_NEURONS; j++)
            {
                PointF[] col = new PointF[NUM_Y_NEURONS];
                for(int i=0; i<NUM_Y_NEURONS; i++)
                {
                    col[i] = cartesianToScreenCoords(new Point((int)neurons1[i][j].p.X, (int)neurons1[i][j].p.Y));
                }
                g.DrawLines(Pens.Brown, col);
            }
        }

        private Neuron closestNeuron(PaulPoint p, ref int pos_x, ref int pos_y)
        {
            Neuron closest = null;
            double minDist = Double.MaxValue;

            for(int i=0; i<NUM_Y_NEURONS; i++)
            {
                for(int j=0; j< NUM_X_NEURONS; j++)
                {
                    double d = euclidianDistDouble(neurons1[i][j].p, p.p);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = neurons1[i][j];
                        pos_x = j;
                        pos_y = i;
                    }
                }
            }

            return closest;
        }

        private double alfa(int t)
        {
            return 0.07 * Math.Pow(Math.E, -t / (double)N);
        }

        private double vecinatate(int t)
        {
            return 7.0 * Math.Pow(Math.E, -t / (double)N);
        }

        private void modifyNeighbours(PaulPoint x, int pos_x, int pos_y)
        {
            double a = alfa(t);
            int v = (int)vecinatate(t);

            int stanga_y = pos_y - v;
            if(stanga_y < 0)
            {
                stanga_y = 0;
            }

            int stanga_x = pos_x - v;

            if(stanga_x < 0)
            {
                stanga_x = 0;
            }

            int dreapta_y = pos_y + v;

            if(dreapta_y > NUM_Y_NEURONS-1)
            {
                dreapta_y = NUM_Y_NEURONS - 1;
            }

            int dreapta_x = pos_x + v;
            if(dreapta_x > NUM_X_NEURONS-1)
            {
                dreapta_x = NUM_X_NEURONS - 1;
            }

            for (int i = stanga_y; i <= dreapta_y; i++)
            {
                for (int j = stanga_x; j <= dreapta_x; j++)
                {
                    neurons1[i][j].p.X = (float)(neurons1[i][j].p.X + a * (x.p.X - neurons1[i][j].p.X));
                    neurons1[i][j].p.Y = (float)(neurons1[i][j].p.Y + a * (x.p.Y - neurons1[i][j].p.Y));
                }
            }
        }

        private void runKohonenEpoch()
        {
            for (int i = 0; i < pts.Length; i++)
            {
                int pos_x = 0;
                int pos_y = 0;
                Neuron n = closestNeuron(pts[i], ref pos_x, ref pos_y);

                // the closest neauron is the one on pos_x and pos_y in the MATRIX
                // the closest neuron to pts[i]
                modifyNeighbours(pts[i], pos_x, pos_y);
            }

            t += 1;

            pictureBox1.Refresh();

            Debug.WriteLine("t = {0}", t);
            Debug.WriteLine("v = {0}", vecinatate(t));
            Debug.WriteLine("alfa = {0}", alfa(t));
        }

        private void kohonenBtn_Click(object sender, EventArgs e)
        {
            if(t < T && alfa(t) > 0.001)
            {
                runKohonenEpoch();
            }
            else
            {
                btnKohonenEpoch.Enabled = false;
                buttonKohonen.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            while (t < T && alfa(t) > 0.001)
            {
                runKohonenEpoch();
            }

            btnKohonenEpoch.Enabled = false;
            buttonKohonen.Enabled = false;

        }
    }
}
