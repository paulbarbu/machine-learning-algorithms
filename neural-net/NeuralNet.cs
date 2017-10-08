using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generare_nr_aleatoare
{
    class NeuralNet
    {
        //number of neurons on each of the 3 layers
        private int n_in;
        private int n_hidden;
        private int n_out;

        //prag = threshold

        double[][] w12; //the weight of the link between the neuron "h" from the hidden layer (2) with the neuron "i" from the input layer
        double[] prag2; //threshold value of the "h" neuron in the hidden layer
        double[][] w23; //the weight of the link between the neuron "o" in the output layer (3) with the neuron "h" from the hidden layer
        double[] prag3; //threshold value of the "o" neuron in the output layer

        double[] out1; //value of the neuron "i" from the input layer
        double[] out2; //value of the neuron "h" from the hidden layer
        double[] out3; //value of the neuron "o" from the output layer

        Random rand = new Random(10);

        readonly double learning_step = 0.6;// Math.Pow(10, -1);

        //activation function
        double F(double x)
        {
            //sigmoid
            double num = 1 + Math.Exp(-x);
            double res= 1.0 / num;


            if(double.IsInfinity(res) || double.IsNaN(res))
            {
                Debug.WriteLine("BUG");
            }

            return res;
        }

        double F_derived(double x)
        {
            double fx = x; //F(x)
            double res = fx * (1.0 - fx);


            if (double.IsInfinity(res) || double.IsNaN(res))
            {
                Debug.WriteLine("BUG");
            }

            return res;
        }

        public NeuralNet(int n_in, int n_hidden, int n_out)
        {
            this.n_in = n_in;
            this.n_hidden = n_hidden;
            this.n_out = n_out;

            out1 = new double[n_in];

            w12 = new double[n_hidden][];
            for(int i=0; i<n_hidden; i++)
            {
                w12[i] = new double[n_in];
            }

            prag2 = new double[n_hidden];
            out2 = new double[n_hidden];

            w23 = new double[n_out][];
            for (int i = 0; i < n_out; i++)
            {
                w23[i] = new double[n_hidden];
            }

            prag3 = new double[n_out];
            out3 = new double[n_out];

            initWeights();
        }

        private void initWeights()
        {
            for (int h = 0; h < n_hidden; h++)
            {
                prag2[h] = rand.NextDouble();
                for (int i = 0; i < n_in; i++)
                {
                    w12[h][i] = rand.NextDouble() - 0.5;
                }
            }

            for (int o = 0; o < n_out; o++)
            {
                prag3[o] = rand.NextDouble();
                for (int h = 0; h < n_hidden; h++)
                {
                    w23[o][h] = rand.NextDouble() - 0.5;
                }
            }
        }

        private void assert(double[][] examples, double[][] targets)
        {
            //the number of input examples must match the number of input neurons
            Debug.Assert(examples[0].Length == n_in);

            //the number of targets (results) must match the number of output neurons
            Debug.Assert(targets[0].Length == n_out);

            //the number of input example must match the number of results
            Debug.Assert(examples.Length == targets.Length);
        }

        public double[] run(double[] example)
        {
            Debug.Assert(example.Length == n_in);
            forward(example);
            return out3;
        }

        public void train(double[][] examples, double[][] targets, double epsilon)
        {
            assert(examples, targets);

            double E = double.MaxValue;

            //while (E > epsilon)
            int t = 0;
            while(t < 1000)
            {
                E = trainEpoch(examples, targets);

                Debug.WriteLine("E= " + E);
                t++;
            }
        }

        public double trainEpoch(double[][] examples, double[][] targets)
        {
            double E = 0;
            assert(examples, targets);

            for (int i = 0; i < examples.Length; i++)
            {
                //take each example and run it through the network
                forward(examples[i]);
                E += computeError(examples[i], targets[i]);
                backward(targets[i]);
            }

            return E;
        }

        private void forward(double[] example)
        {
            computeInputLayerOutput(example);
            computeHiddenLayerOutput();
            computeOutputLayerOutput();
        }

        private void computeInputLayerOutput(double[] example)
        {
            for (int i = 0; i < example.Length; i++)
            {
                //compute out1, which is equal to the input
                out1[i] = example[i];
            }
        }

        private void computeHiddenLayerOutput()
        {
            //compute the hidden layer outputs
            for (int h = 0; h < n_hidden; h++)
            {
                double s = 0;
                for (int i = 0; i < n_in; i++)
                {
                    s += w12[h][i] * out1[i];
                }

                s += prag2[h];

                out2[h] = F(s);
            }
        }

        private void computeOutputLayerOutput()
        {
            for (int o = 0; o < n_out; o++)
            {
                double s = 0;
                for (int h = 0; h < n_hidden; h++)
                {
                    s += w23[o][h] * out2[h];
                }
                s += prag3[o];
                out3[o] = F(s);
            }
        }

        private double computeError(double[] example, double[] target)
        {
            double error = 0;
            for (int o = 0; o < n_out; o++)
            {
                 error += Math.Pow(out3[o] - target[o], 2);
            }

            return error;
        }

        private void backward(double[] target)
        {
            updateWeightsOutputLayer(target);
            updateWeightsHiddenLayer(target);
            updateWeightsInputHiddenLayer(target);
            updateWeightsHiddenOutputLayer(target);
        }

        private void updateWeightsOutputLayer(double[] target)
        {
            for (int o = 0; o < n_out; o++)
            {
                double dw = 2 * (out3[o] - target[o]) * F_derived(out3[o]);

                prag3[o] = prag3[o] - learning_step * dw;
            }
        }

        private void updateWeightsHiddenLayer(double[] target)
        {
            for (int h = 0; h < n_hidden; h++)
            {
                double sum = 0;
                for (int o = 0; o < n_out; o++)
                {
                    sum += (out3[o] - target[o]) * F_derived(out3[o]) * w23[o][h];
                }
                double dw = 2 * sum * F_derived(out2[h]);
                prag2[h] = prag2[h] - learning_step * dw;
            }
        }

        private void updateWeightsInputHiddenLayer(double[] target)
        {
            for (int h = 0; h < n_hidden; h++)
            {
                for (int i = 0; i < n_in; i++)
                {
                    double sum = 0;
                    for (int o = 0; o < n_out; o++)
                    {
                        sum += (out3[o] - target[o]) *F_derived(out3[o]) * w23[o][h];
                    }

                    double dw = 2 * sum * F_derived(out2[h]) * out1[i];
                    w12[h][i] = w12[h][i] - learning_step * dw;
                }
            }
        }

        private void updateWeightsHiddenOutputLayer(double[] target)
        {
            for (int o = 0; o < n_out; o++)
            {
                for (int h = 0; h < n_hidden; h++)
                {
                    double dw = 2 * (out3[o] - target[o]) * F_derived(out3[o]) * out2[h];

                    w23[o][h] = w23[o][h] - learning_step * dw;
                }
            }
        }
    }
}
