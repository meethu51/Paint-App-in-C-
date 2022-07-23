using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Method to set the default form size and to draw on bitmap//
            
            this.Width = 1000;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm; 
        }

        private void pic_color_Click(object sender, EventArgs e)
        {

        }

        //Declare Global Variables//

        Bitmap bm;
        Graphics g;
        bool paint = false;
        string strText;
        Brush brush;
        Point px, py;
        Point startP = new Point(0, 0);
        Point endP = new Point(0, 0);
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 10);//Pen Erase color Variable Name//
        int index;
        int x, y, sX, sY, cX, cY;

        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void btn_rect_Click(object sender, EventArgs e)
        {
            
            //If Rectangle tool button is press,this will activate if(index == 4) which was created in pic_MouseUp//
            index = 4;
            
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            
            //If Line tool button is press,this will activate if(index == 5) which was created in pic_MouseUp//
            index = 5;
            
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            //Method to draw the selected index and display positions if bool paint value is true//
            Graphics g = e.Graphics;

            if(paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            //If Clear button is pressed this code will ensure the drawing is reset back to a white screen//
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            
            //Code for When Color Button is pressed//
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
            

        }

        //Method to set and return color pallette image point//
        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));

        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            //Code for picking colors//
            //If User selects any color from color_picker img then set that color to new _color,pen color and pic color//
            Point point = set_point(color_picker, e.Location);
            pic_color.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
            new_color = pic_color.BackColor;
            p.Color = pic_color.BackColor;

        }

        //CREATING METHOD TO VALIDATE PIXEL old_color BEFORE FILLING THE SHAPE to new_color//
        private void validate(Bitmap bm, Stack<Point>sp, int x, int y,Color old_color,Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
                 
            }
        }

        //Creating FloodFill Function to validate fill Method//
        public void Fill(Bitmap bm, int x, int y , Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;

            //This method will get ht e old pixel color and fill new color from the clicked point till the stack count>0//
            //If old_color is equal to new_color then return and do nothing//
            while(pixel.Count>0)
            {
                Point pt = (Point)pixel.Pop();
                if(pt.X>0 && pt.Y>0 && pt.X<bm.Width-1 && pt.Y<bm.Height-1)
                {
                    //Here it will 1st validate then fill the stack points//
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 7 )
            {
                //Method for Fill Tool//
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
                
            }
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            
            //If fill tool button is pressed,this will activate if(index == 7) which was created in pic_MouseClick and public void Fill//
            index = 7;
            
        }

        //Method to Save the drawing//
        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width,pic.Height),bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Image Saved Sucessfully!");


            }
        }
        
        //Method to Load an image//
        private void btn_load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofdlg = new OpenFileDialog())
            {
                ofdlg.Title = "Open Image";
                ofdlg.Filter = "bmp files (*.BMP)|*.BMP|All files (*.*)|*.*";
                if (ofdlg.ShowDialog() == DialogResult.OK)
                {
                    pic.Load(ofdlg.FileName);
                    Image img = new Bitmap(ofdlg.FileName);
                    g = Graphics.FromImage(bm);
                    g.DrawImage(img, pic.ClientRectangle);
                    g.Dispose();
                    pic.Invalidate();
                }
            }

        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            
            //If ellipse tool button is press,this will activate if(index == 3) which was created in pic_MouseUp//
            index = 3;
            
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            
            if ( index == 8 )
            {
                strText = txtBoxText.Text;//Allows the text to be obtained and pasted later on//
                g = Graphics.FromImage(bm);
                Font font = new Font((FontFamily)FontSelect.SelectedValue, float.Parse(pen_size.Text)*10);//Apply new Font and Size to the Text//
                brush = new SolidBrush(pic_color.BackColor);//Apply new Color to the Text//
                g.DrawString(strText, font, brush, e.X, e.Y);//Drawing(paste) the Text on the Canvas//
                g.Dispose();
                pic.Invalidate();
            }
            
            
            //if user clicks on the canvas(pic),it will set the paint bool value as true and assign point click to pY//
            paint = true;
            py = e.Location;
            //if mouse is down then it will set the X,Y coordinates to draw using the following below//
            cX = e.X;
            cY = e.Y;

        }

        //Method for drag and drop image//
        private void pic_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                    pic.Image = Image.FromFile(fileNames[0]);
            }
        }
        //Code for Drag and drog//
        private void Form1_Load(object sender, EventArgs e)
        {
            
            pic.AllowDrop = true;
            //To apply font selection into the FontSelect (ComboBox)//
            FontSelect.DataSource = new InstalledFontCollection().Families;
            FontSelect.DisplayMember = "Name";
            
        }

        private void pic_DragEnter(object sender, DragEventArgs e)
        {
            //Code for Drag and Drop Effect//
            e.Effect = DragDropEffects.Copy;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Code for Abour Section//
            MessageBox.Show("Made By Bhuvan Shrivastava(203652R) from Group E1");
        }

        private void Text_Click(object sender, EventArgs e)
        {
            index = 8;
        }

        private void Pencil2_Click(object sender, EventArgs e)
        {
            index = 9;
        }

        int prevX = 0;
        int prevY = 0;


        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if(paint)
            {
                

                if (index == 1)
                {
                    //Method of Pencil to draw free-form lines//
                    px = e.Location;
                    Pen p = new Pen(pic_color.BackColor, float.Parse(pen_size.Text));
                    g = Graphics.FromImage(bm);
                    g.DrawLine(p,py,px);
                    py = px;
                    

                }
                if (index == 2)
                {
                    //Method of Eraser to erase free-form lines//
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                    Pen p = new Pen(pic_color.BackColor, float.Parse(pen_size.Text));
                    g.DrawLine(erase, new Point(prevX, prevY), new Point(x, y));
                    prevX = x;
                    prevY = y;
                    
                }
                if (index == 9)
                {
                    px = e.Location;
                    g = Graphics.FromImage(bm);
                    g.DrawImage(Properties.Resources.IntBlol05_removebg_preview, e.X, e.Y, float.Parse(pen_size.Text), float.Parse(pen_size.Text));
                    py = px;
                }
            }
            //if mouse is moving then set the start and end points to get the height and width//

            pic.Refresh();

            x = e.X; ;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;

            

        }
        

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            
            //If pencil button is press,this will activate if(index == 1) which was created in pic_MouseMove//
            index = 1;
            
        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            
            //If eraser button is press,this will activate if(index == 2) which was created in pic_MouseMove//
            index = 2;
            
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            //Controls the Shape thickness//
            Pen p = new Pen(pic_color.BackColor, float.Parse(pen_size.Text));
            //To allow circles to be drawn properly to ensure that the parameter is not valid error does not occur//
            g = Graphics.FromImage(bm);

            if (index == 3)
            {
                
                //Method to draw the ellipse//
                g.DrawEllipse(p, cX, cY, sX, sY);
                
            }
            if(index == 4)
            {
                //Method to draw the Rectangle//
                g.DrawRectangle(p,cX,cY,sX,sY);
                
            }
            if(index == 5)
            {
                //Method to draw a Line//
                g.DrawLine(p,cX,cY,x,y);
                
            }

        }

        
    }
}
