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

        public String getBox(Point p)
        {
            if (p.X < top_left.X && p.Y < top_left.Y)
                return "TL";
            else if (p.X < top_left.X && p.Y > top_left.Y && p.Y < bottom_left.Y)
                return "ML";
            else if (p.X < top_left.X && p.Y > bottom_left.Y)
                return "BL";

            else if (p.X > top_left.X && p.X < top_right.X && p.Y < top_right.Y)
                return "MT";
            else if (p.X > top_left.X && p.X < top_right.X && p.Y > top_right.Y && p.Y < bottom_left.Y)
                return "MM";
            else if (p.X > top_left.X && p.X < top_right.X && p.Y > bottom_right.Y)
                return "MB";

            else if (p.X > top_right.X && p.Y < top_right.Y)
                return "TR";
            else if (p.X > top_right.X && p.Y > top_right.Y && p.Y < bottom_right.Y)
                return "MR";
            else if (p.X > top_right.X && p.Y > bottom_right.Y)
                return "BR";

            return "ERROR";
        }

        private void drawX(Point p, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
            g.DrawLine(blackpen, new Point(p.X - 18, p.Y - 18), new Point(p.X + 18, p.Y + 18));
            g.DrawLine(blackpen, new Point(p.X - 18, p.Y + 18), new Point(p.X + 18, p.Y - 18));
        }

        private void drawO(Point p, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
            g.DrawEllipse(blackpen, p.X, p.Y, 10, 10);
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
            drawX(bottom_left, g);
            g.Dispose();
        }

        public void drawBoard(char[,] board, Graphics g)
        {
            if (board[0, 0] == 'x')
                drawX(new Point(top_left.X - 20, top_left.Y - 20), g);
            else if (board[0, 0] == 'y')
                drawO(new Point(top_left.X - 20, top_left.Y - 20), g);
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

