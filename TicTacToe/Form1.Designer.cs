using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System;
using System.Threading;

namespace TicTacToe
{
    partial class Form1
    {

        Point top_left = new Point(75, 75);
        Point top_right = new Point(150, 75);
        Point bottom_left = new Point(75, 125);
        Point bottom_right = new Point(150, 125);

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void panel1_MouseClick(Object sender, MouseEventArgs e)
        {

            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.Append(getBox(new Point(e.X, e.Y)));
            MessageBox.Show(messageBoxCS.ToString(), "MouseClick Event");
        }

        public Tuple<int, int> getBox(Point p)
        {
            if (p.X < top_left.X && p.Y < top_left.Y)
                return Tuple.Create(0, 0);
            else if (p.X < top_left.X && p.Y > top_left.Y && p.Y < bottom_left.Y)
                return Tuple.Create(0, 1);
            else if (p.X < top_left.X && p.Y > bottom_left.Y)
                return Tuple.Create(0, 2);

            else if (p.X > top_left.X && p.X < top_right.X && p.Y < top_right.Y)
                return Tuple.Create(1, 0);
            else if (p.X > top_left.X && p.X < top_right.X && p.Y > top_right.Y && p.Y < bottom_left.Y)
                return Tuple.Create(1, 1);
            else if (p.X > top_left.X && p.X < top_right.X && p.Y > bottom_right.Y)
                return Tuple.Create(1, 2);

            else if (p.X > top_right.X && p.Y < top_right.Y)
                return Tuple.Create(2, 0);
            else if (p.X > top_right.X && p.Y > top_right.Y && p.Y < bottom_right.Y)
                return Tuple.Create(2, 1);
            else if (p.X > top_right.X && p.Y > bottom_right.Y)
                return Tuple.Create(2, 2);

            return Tuple.Create(3, 3);
        }

        private void drawX(int x, int y, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
            int startLocationX = 30;
            int startLocationY = 30;
            x += 1;
            y += 1;

            drawX_atOrigin(x * 30 + (30 * (x - 1)), y * 30 + (20 * (y - 1)), g);
        }

        private void drawO(int x, int y, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
            int startLocationX = 30;
            int startLocationY = 30;
            x += 1;
            y += 1;

            drawO_atOrigin(x * 30 + (30 * (x - 1)), y * 30 + (20 * (y - 1)), g);
        }

        private void drawX_atOrigin(int x, int y, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);

            g.DrawLine(blackpen, new Point(x, y), new Point(x + 30, y + 30));
            g.DrawLine(blackpen, new Point(x, y + 30), new Point(x + 30, y));
        }

        private void drawO_atOrigin(int x, int y, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);

            g.DrawEllipse(blackpen, x, y, 40, 40);
        }

        private void drawBoard(char[,] board, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 'x')
                        drawX(i, j, g);
                    else
                        drawO(i, j, g);
                }
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen blackpen = new Pen(Color.Black, 3);
            Graphics g = e.Graphics;
            int offset = 40;
            g.DrawLine(blackpen, new Point(top_left.X, top_left.Y - offset), new Point(bottom_left.X, bottom_left.Y + offset));
            g.DrawLine(blackpen, new Point(top_right.X, top_right.Y - offset), new Point(bottom_right.X, bottom_right.Y + offset));
            g.DrawLine(blackpen, new Point(top_left.X - offset, top_left.Y), new Point(top_right.X + offset, top_right.Y));
            g.DrawLine(blackpen, new Point(bottom_left.X - offset, bottom_left.Y), new Point(bottom_right.X + offset, bottom_right.Y));
            drawO(2,2, g);
            g.Dispose();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.  
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 264);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseClick += new MouseEventHandler(panel1_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 266);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
    }
}

