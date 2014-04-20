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

        Node node = new Node();

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
            var selection = getBox(new Point(e.X, e.Y));

            //MessageBox.Show(messageBoxCS.ToString(), "MouseClick Event");

            node = node.playerMove(selection.Item1, selection.Item2);
            this.panel1.Invalidate();
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
            x += 1;
            y += 1;

            drawX_atOrigin(x * 30 + (30 * (x - 1)), y * 30 + (20 * (y - 1)), g);
        }

        private void drawO(int x, int y, Graphics g)
        {
            Pen blackpen = new Pen(Color.Black, 2);
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
                    else if (board[i, j] == 'o')
                        drawO(i, j, g);
                }
            }
        }

        private bool validateMove(Tuple<int, int> location, char[,] board)
        {
            if (board[location.Item1, location.Item2] != 'x' || board[location.Item1, location.Item2] != 'o')
                return true;
            return false;
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

            Debug.WriteLine("DRAWING");
            drawBoard(node.board, g);

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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
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
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(318, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nodes Generated";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(416, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "0";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(321, 50);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(54, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Trace";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(321, 86);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(318, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Depth Level";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 266);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label2;
        private CheckBox checkBox1;
        private NumericUpDown numericUpDown1;
        private Label label3;
    }
}

