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

namespace generare_nr_aleatoare
{
    public partial class Form1 : Form
    {
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
        static private double Gauss(double x, int m, int sigma)
        {
            double pow = ((m - x) * (m - x)) / (2 * sigma * sigma);
            return Math.Pow(Math.E, -1 * pow);
        }

        private const int WIDTH = 200;
        private const int HEIGHT = 200;
        private const int NUM_POINTS = 1000;
        private const int NUM_X_NEURONS = 10;
        private const int NUM_Y_NEURONS = 10;
        private Zone[] zones = new Zone[] {
            new Zone { m_x = 80, m_y = 50, sigma_x =  5, sigma_y = 5, pen = Pens.Red} ,
            new Zone { m_x = -80, m_y = 80, sigma_x =  5, sigma_y = 5, pen = Pens.Green } ,
            new Zone { m_x = 00, m_y = 00,sigma_x =  5, sigma_y = 5, pen = Pens.Blue},

            //new Zone { m_x = 250, m_y = 250, sigma_x =  15, sigma_y = 5, pen = Pens.Red} ,
            //new Zone { m_x = -150, m_y = 250, sigma_x =  5, sigma_y = 15, pen = Pens.Green } ,
            //new Zone { m_x = 10, m_y = 10,sigma_x =  5, sigma_y = 5, pen = Pens.Blue},
            //new Zone { m_x = 250, m_y = -250, sigma_x =  15, sigma_y = 5, pen = Pens.Black} ,
            //new Zone { m_x = -150, m_y = -250, sigma_x =  5, sigma_y = 15, pen = Pens.Gold } ,
        };

        private Random rand = new Random();
        private PaulPoint[] pts = new PaulPoint[NUM_POINTS];

        public Form1()
        {
            InitializeComponent();

            generatePoints();
            writePoints();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WindowState = FormWindowState.Maximized;

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

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

        private void xorBtn_Click(object sender, EventArgs e)
        {
            NeuralNet nn = new NeuralNet(2, 2, 1);
            double[][] xor_examples = new double[][]
            {
                new double[] { 0.1, 0.1 }, // 0 0
                new double[] { 0.1, 0.9 }, // 0 1
                new double[] { 0.9, 0.1 }, // 1 0
                new double[] { 0.9, 0.9 }, // 1 1
            };

            double[][] xor_targets = new double[][]
            {
                new double[] { 0.1 }, // 0
                new double[] { 0.9 }, // 1
                new double[] { 0.9 }, // 1
                new double[] { 0.1 }, // 0
            };

            nn.train(xor_examples, xor_targets, Math.Pow(10, -20));

            Debug.WriteLine("done learning");

            Func<double, int> decodeOutput = x => {
                if (x >= 0.5)
                {
                    return 1;
                }

                return 0;
            };

            for (int j = 0; j < xor_examples.Length; j++)
            {
                double[] output = nn.run(xor_examples[j]);
                for (int i = 0; i < output.Length; i++)
                {
                    Debug.WriteLine(i + ": " + output[i] + " => " + decodeOutput(output[i]));
                }
            }
        }

        private void learnPctBtn_Click(object sender, EventArgs e)
        {
            NeuralNet nn = new NeuralNet(2, 6, 3);

            double[][] examples = new double[pts.Length][];

            for (int i = 0; i < pts.Length; i++)
            {
                examples[i] = new double[2];
                examples[i][0] = pts[i].p.X;
                examples[i][1] = pts[i].p.Y;
            }

            double[][] targets = new double[pts.Length][];

            for (int i = 0; i < pts.Length; i++)
            {
                targets[i] = new double[zones.Length];

                for (int j = 0; j < zones.Length; j++)
                {
                    targets[i][j] = 0.1;
                }

                targets[i][pts[i].zone] = 0.9;
            }

            nn.train(examples, targets, 20);// Math.Pow(10, -1));

            Debug.WriteLine("done learning");

            Action<int, int, double[]> decodeOutput = (x, y, output) => {
                double max = 0;
                int pos = 0;

                for (int i = 0; i < output.Length; i++)
                {
                    if(output[i] > max)
                    {
                        max = output[i];
                        pos = i;
                    }
                }

                Point p = cartesianToScreenCoords(new Point(x, y));
                ((Bitmap)pictureBox1.Image).SetPixel(p.X, p.Y, zones[pos].pen.Color);

                //Graphics g = pictureBox1.CreateGraphics();
                //g.DrawEllipse(zones[pos].pen, p.X, p.Y, 3, 3);
                //pictureBox1.Refresh();
            };

            for (int i = -WIDTH / 2; i < WIDTH/2; i+=1)
            {
                for (int j = -WIDTH/2; j < WIDTH/2; j+=1)
                {
                    double[] output = nn.run(new double[] { j, i });

                    decodeOutput(j, i, output);
                }
            }


            pictureBox1.Refresh();
        }
    }
}
