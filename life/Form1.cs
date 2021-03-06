﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;
        private bool[,] field;  //true - alive, false - dead
        private int rows;
        private int cols;
        private uint currentGeneration;
        public Form1()
        {
            InitializeComponent();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Draw();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);
            bool[,] newfield = new bool[cols, rows];
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    int numberOfNeighbours = CountNeighbours(in x, in y);
                    if (!field[x, y] && numberOfNeighbours == 3)
                        newfield[x, y] = true;
                    else if (field[x, y] && (numberOfNeighbours < 2 || numberOfNeighbours > 3))
                        newfield[x, y] = false;
                    else
                        newfield[x, y] = field[x, y];
                    if (field[x, y])
                    {
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }
            Text = $"Generation {++currentGeneration}";
            field = newfield;
            pictureBox1.Refresh();
        }

        private int CountNeighbours(in int x, in int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j+=2)
                {
                    if (field[(x + i + cols) % cols, (y + j + rows) % rows])
                        count++;
                }

            if (field[(x - 1 + cols) % cols, (y + rows) % rows])
                count++;

            if (field[(x + 1 + cols) % cols, (y + rows) % rows])
                count++;

            //we don't need to check the cell itself while counting all the
            //neighbours, so i've decided to check all the cells below and above,
            //and then the ones on the left and on the right, so, instead of 9 checks
            //through nested loops, we make only 8

            return count;
        }

        private void bStartNew_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
                return;

            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";
            resolution = (int)udResolution.Value + 1;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            field = new bool[cols, rows];
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            Random rand = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = rand.Next((int)udDensity.Value) == 0;
                }
            }
            timer1.Start();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled && !timer2.Enabled)
                return;
            timer1.Stop();
            timer2.Start();
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            timer1.Start();
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            field = new bool[cols, rows];
        }

        private void Draw() //didn't know how to else call this method
        {
            graphics.Clear(Color.Black);
            var newfield = new bool[cols, rows];
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    newfield[x, y] = field[x, y];
                    if (field[x, y])
                    {
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }
            field = newfield;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled && !timer2.Enabled)
                return;
            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                try
                {
                    field[x, y] = true;
                }
                catch { }
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                try
                {
                    field[x, y] = false;
                }
                catch { }
            }
        }
    }
}
