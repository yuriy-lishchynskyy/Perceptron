using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            Perceptron p = new Perceptron();
            string file_name = "...//data.csv";

            p.ReadData(file_name);
            p.TrainData();
            p.Output();

            double[] x1_test = { 1, 575, 275 };
            double[] x2_test = { 1, 575, 375 };

            Vector test_in;
            double res;

            test_in = new Vector(x1_test);
            res = p.ClassifyPoint(test_in, 1);
            test_in = new Vector(x2_test);
            res = p.ClassifyPoint(test_in, 1);
        }
    }

    class Perceptron
    {
        // data
        private Vector v_inputs1;
        private Vector v_inputs2;
        private Vector v_weights = new Vector(3); // initialise weights vector, by default 0's
        private Vector v_outputs;

        private int count = 0;
        private double alpha = 0.05; // learning rate for w1/w2
        private int max_steps = 10000000; // maximum iterations

        public double Alpha { get => alpha; set => alpha = value; }
        public int Max_steps { get => max_steps; set => max_steps = value; }

        // constructors
        public Perceptron()
        {

        }

        // methods
        public void ReadData(string filename)
        {
            StreamReader sr = null;
            string temp = null; // used to read each line
            char[] char_separators = new char[] { ',' };
            string[] output = null; // store each line by breaking each row of 4 items into 4 pieces and storing each piece in array

            List<double> list_id = new List<double>(); // create dummy lists to store csv data - use to size vectors
            List<double> list_rpm = new List<double>();
            List<double> list_vib = new List<double>();
            List<double> list_stat = new List<double>();

            double a;

            try // enclose problematic code in try block to throw exception if any part fails
            {
                sr = new StreamReader(filename);

                do
                {
                    temp = sr.ReadLine();

                    if (temp == null)
                    {
                        break; // stop if line is empty (ie end of csv)
                    }

                    output = temp.Split(char_separators); // separates "temp" string by each item in array (in this case - comma, comma, comma etc. If 2 items in array, would alternate)

                    bool success = double.TryParse(output[1], out a); // use to ignore text-based header row

                    if (success)
                    {
                        list_id.Add(Convert.ToDouble(output[0])); // assign row data to lists
                        list_rpm.Add(Convert.ToDouble(output[1]));
                        list_vib.Add(Convert.ToDouble(output[2]));
                        list_stat.Add(Convert.ToDouble(output[3]));
                    }

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error {0}", e.Message); // print error messag
            }
            finally
            {
                if (sr != null) // only close file if no errors after reading file
                    sr.Close();
            }

            this.v_inputs1 = new Vector(list_rpm.ToArray()); // convert list to array and feed into corresponding vector
            this.v_inputs2 = new Vector(list_vib.ToArray());
            this.v_outputs = new Vector(list_stat.ToArray());
        }

        public void TrainData()
        {
            int error = 1;
            double e;
            double hx;
            Vector xi = new Vector(3);

            Console.WriteLine("Training Perceptron...");

            while (error > 0 && this.count < this.max_steps) // while not all points classified correctly, and max iterations not exceeded
            {
                error = 0;
                this.count++;

                for (int i = 0; i < this.v_inputs1.Length; i++) // for each set of inputs [1, x0_i, x1_i]
                {
                    xi[0] = 1; // create input vector
                    xi[1] = this.v_inputs1[i];
                    xi[2] = this.v_inputs2[i];

                    hx = this.ClassifyPoint(xi, 0); // classify point as 1 or 0

                    e = this.v_outputs[i] - hx; // error term

                    if (e != 0) // if calculated output not equal to expected output, adjust weights
                    {
                        this.v_weights = this.v_weights + xi * (this.alpha * e); // dot product and sum of vectors to get new weight vector
                        error++;
                    }
                }
            }
        }

        public double ClassifyPoint(Vector x, int option) // method to classify a point (input vector) using weighted sum and step function (if option == 1, print result in console)
        {
            double x_out = (x * this.v_weights); // dot product of input vector with weights
            double hx;

            if (x_out > 0) // if H(x) > 0, output = 1
            {
                hx = 1;
            }
            else // if H(x) <= 0, output = 0
            {
                hx = 0;
            }

            if (option == 1) // optionally print classification of point
            {
                Console.WriteLine("The point ({0},{1}) is classified as {2}", x[1], x[2], hx);
            }

            return hx;
        }

        public void Output() // output no. of iterations taken to converge, and the final value of weights
        {
            if (this.count == this.max_steps) // if max steps reached - perceptron has not converged
            {
                Console.WriteLine("\nPerceptron weights have not converged after {0} iterations", this.max_steps);
            }
            else
            {
                Console.WriteLine("\nPerceptron weights have converged after {0} iterations:", this.count);
                Console.WriteLine("w0 = {0:0.00}\nw1 = {1:0.00}\nw2 = {2:0.00}", this.v_weights[0], this.v_weights[1], this.v_weights[2]);
                Console.WriteLine("");
            }
        }
    }

    class Vector
    {
        // data
        private double[] vec;
        private int length;

        public int Length { get => this.length; }

        // constructors
        public Vector()
        {

        }

        public Vector(int l) // create zero vector of length l
        {
            if (l > 0)
            {
                this.length = l;
                this.vec = new double[this.length];

                for (int i = 0; i < this.length; i++)
                {
                    this.vec[i] = 0;
                }
            }
            else
            {
                Console.WriteLine("Invalid input for Vector length");
            }
        }

        public Vector(double[] a) // create vector from an array
        {
            this.length = a.GetLength(0);
            this.vec = new double[this.length];

            for (int i = 0; i < this.length; i++)
            {
                this.vec[i] = a[i];
            }
        }

        public Vector(Vector a) // create vector from another vector
        {
            this.length = a.Length;
            this.vec = new double[this.length];

            for (int i = 0; i < this.length; i++)
            {
                this.vec[i] = a[i];
            }
        }

        // methods

        // overloaded methods
        public double this[int i] // indexing operator
        {
            get
            {
                return this.vec[i];
            }
            set
            {
                this.vec[i] = value;
            }
        }

        public static Vector operator +(Vector a, Vector b) // addition of 2 vectors
        {
            int i;

            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                Console.WriteLine("Cannot add vectors: Lengths not equal!");
                return null;
            }

            Vector temp = new Vector(a_l);

            for (i = 0; i < a_l; i++) // add corresponding elements
            {
                temp[i] = a[i] + b[i];
            }

            return temp;
        }

        public static Vector operator -(Vector a, Vector b) // subtraction of 2 vectors
        {
            int i;

            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                Console.WriteLine("Cannot subtract vectors: Lengths not equal!");
                return null;
            }

            Vector temp = new Vector(a_l);

            for (i = 0; i < a_l; i++) // subtract corresponding elements
            {
                temp[i] = a[i] - b[i];
            }

            return temp;
        }

        public static double operator *(Vector a, Vector b) // dot product multiplication of 2 vectors
        {
            int i;

            int a_l = a.Length;
            int b_l = b.Length;

            if (a_l != b_l) // check if vectors same size
            {
                Console.WriteLine("Cannot dot multiply vectors: Lengths not equal!");
                return 0;
            }

            double sum = 0;

            for (i = 0; i < a_l; i++) // running sum of multiplied terms
            {
                sum = sum + (a[i] * b[i]);
            }

            return sum;
        }

        public static Vector operator *(Vector a, double x) // multiplication of vector by constant
        {
            int i;
            int a_l = a.Length;

            Vector temp = new Vector(a_l);

            for (i = 0; i < a_l; i++) // scale each term by constant
            {
                temp[i] = a[i] * x;
            }

            return temp;
        }
    }
}