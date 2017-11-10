﻿using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace PacmanServer {
    public partial class Form1 : Form {
        public delegate void MovePacman(String pacmanName, KeyConfiguration.KEYS key);
        public MovePacman movePacmanDel;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;
        //player speed
        int speed = 5;

        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;
        
        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;

        Dictionary<String, PictureBox> pacmans;
        public Form1(int pacmanNumbers, int roundTime) {
            pacmans = new Dictionary<string, PictureBox>();
            createPacman(pacmanNumbers);
            InitializeComponent();
            timer1.Interval = roundTime;
            label2.Visible = false;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            //move ghosts
            redGhost.Left += ghost1;
            yellowGhost.Left += ghost2;

            // if the red ghost hits the picture box 4 then wereverse the speed
            if (redGhost.Bounds.IntersectsWith(pictureBox1.Bounds))
                ghost1 = -ghost1;
            // if the red ghost hits the picture box 3 we reverse the speed
            else if (redGhost.Bounds.IntersectsWith(pictureBox2.Bounds))
                ghost1 = -ghost1;
            // if the yellow ghost hits the picture box 1 then wereverse the speed
            if (yellowGhost.Bounds.IntersectsWith(pictureBox3.Bounds))
                ghost2 = -ghost2;
            // if the yellow chost hits the picture box 2 then wereverse the speed
            else if (yellowGhost.Bounds.IntersectsWith(pictureBox4.Bounds))
                ghost2 = -ghost2;

            pinkGhost.Left += ghost3x;
            pinkGhost.Top += ghost3y;

            if (pinkGhost.Left < boardLeft ||
                pinkGhost.Left > boardRight ||
                (pinkGhost.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                (pinkGhost.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                (pinkGhost.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                (pinkGhost.Bounds.IntersectsWith(pictureBox4.Bounds)))
            {
                ghost3x = -ghost3x;
            }
            if (pinkGhost.Top < boardTop || pinkGhost.Top + pinkGhost.Height > boardBottom - 2)
            {
                ghost3y = -ghost3y;
            }
            //moving ghosts and bumping with the walls end

            foreach (PictureBox pacman in pacmans.Values)
            {
                //for loop to check walls, ghosts and points
                foreach (Control x in this.Controls)
                {
                    // checking if the player hits the wall or the ghost, then game is over
                    if (x is PictureBox && (string)x.Tag == "wall" || (string)x.Tag == "ghost")
                    {
                        if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds))
                            pacman.Dispose();
                      
                    }
                    if (x is PictureBox && (string)x.Tag == "coin")
                    {
                        if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds))
                            this.Controls.Remove(x);
                        
                    }
                }
            }
        }
        private void createPacman(int numberOfPacmans)
        {
            PictureBox pacman;
            for (int i = 0; i < numberOfPacmans; i++)
            {
                pacman = new PictureBox();
                ((System.ComponentModel.ISupportInitialize)(pacman)).BeginInit();
                pacman.BackColor = System.Drawing.Color.Transparent;
                pacman.Image = global::PacmanServer.Properties.Resources.Left;
                pacman.Location = new System.Drawing.Point(8, 40 * (i + 1));
                pacman.Margin = new System.Windows.Forms.Padding(0);
                pacman.Name = "pacman" + i;
                pacman.Size = new System.Drawing.Size(25, 25);
                pacman.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                pacman.TabIndex = 149;
                pacman.TabStop = false;
                this.Controls.Add(pacman);
                ((System.ComponentModel.ISupportInitialize)(pacman)).EndInit();

                pacmans.Add(pacman.Name, pacman);
            }
        }
        private void movePacman(String pacmanName, KeyConfiguration.KEYS key)
        {
            PictureBox pacman1 = pacmans[pacmanName];
            //move player
            switch (key)
            {
                case KeyConfiguration.KEYS.LEFT_KEY:
                    pacman1.Image = Properties.Resources.Left;
                    if (pacman1.Left > (boardLeft))
                        pacman1.Left -= speed;
                    break;
                case KeyConfiguration.KEYS.RIGHT_KEY:
                    pacman1.Image = Properties.Resources.Right;
                    if (pacman1.Left < (boardRight))
                        pacman1.Left += speed;
                    break;
                case KeyConfiguration.KEYS.UP_KEY:
                    pacman1.Image = Properties.Resources.Up;
                    if (pacman1.Top > (boardTop))
                        pacman1.Top -= speed;
                    break;
                case KeyConfiguration.KEYS.DOWN_KEY:
                    pacman1.Image = Properties.Resources.down;
                    if (pacman1.Top < (boardBottom))
                        pacman1.Top += speed;
                    break;
                
            }
        }
    }
}
