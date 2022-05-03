using System;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MapCreation
{ 
    public partial class Form1 : Form
    {
        private RNG rand = new RNG();
        private MapGeneration Map;
        private int currentSeed;
        private int waterLevel = 0;
        public Form1()
        {
            InitializeComponent();
            currentSeed = rand.Seed;
            label2.Text = "current seed: " + currentSeed; //displays the seed, so the user can recreate the map
            Map = new MapGeneration(rand, waterLevel); //generates an initial completely random map on startup
            pictureBox1.Image = Map.Map;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; //informs he user that the map is loading
            RNG rand = new RNG(textBox1.Text); //allows the user to input their own seed
            currentSeed = rand.Seed;
            label2.Text = "current seed: " + currentSeed; //displays seed in number form
            Map = new MapGeneration(rand, waterLevel); //generates the map
            pictureBox1.Image = Map.Map;
            Cursor = Cursors.Arrow; //resets the cursor to normal
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog sfd = new SaveFileDialog()) //allows the user to save maps as images to their computer
            {
                sfd.Filter = "PNG Files|*.png"; //defaults the file type to png
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Map.Map.Save(sfd.FileName, ImageFormat.Png); //if the save button is pressed, the image is saved
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) //allows user to generate archipelagos
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                waterLevel = -50;
            }
            else
                waterLevel = 0;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) //allows use to generate maps that aren't islands
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                waterLevel = 10;
            }
            else
                waterLevel = 0;
        }

        private void button3_Click(object sender, EventArgs e) //copies seed to clipboard
        {
            Clipboard.SetText(Convert.ToString(currentSeed));
        }
    }
}