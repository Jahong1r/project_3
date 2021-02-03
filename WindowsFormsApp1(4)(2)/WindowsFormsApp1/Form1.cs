using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace WindowsFormsApp1
{
	public delegate void Delegat1();
	public delegate void DelAsync1();

	public partial class Form1 : Form
	{
		public event Delegat1 Dvig1;
		public event Delegat1 Dvig2;
		Ball[] ball = new Ball[2];
	    
		public int x_mouse = 0;
		public int y_mouse = 0;


		public Form1()
		{
			

			Korzina korzina = new Korzina();
			Tablo tablo = new Tablo();
			for(int i=0;i<2;i++)
			{
				ball[i] = new Ball(i * 20 + 40, i * 20 + 40);
				this.Dvig1 += new Delegat1(ball[i].Start);
				this.Paint += new PaintEventHandler(ball[i].DrawBall);
				ball[i].evTablo += new Delegat1(tablo.plus);
			}									

			//this.Paint += new PaintEventHandler(ball1.DrawBall);
			//this.Paint += new PaintEventHandler(ball2.DrawBall); 

			//ball1.evTablo += new Delegat1(tablo.plus);
			//ball2.evTablo += new Delegat1(tablo.plus);
			
			this.Paint += new PaintEventHandler(korzina.DrawKorzina);
			this.Paint += new PaintEventHandler(tablo.risov_tablo);
			this.MouseClick += new MouseEventHandler(this.Form1_MouseClick);


			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
			timer.Interval = 100;

			timer.Tick += new EventHandler(this.Pereris_Okno);

			timer.Start();
			InitializeComponent();
			
		}		

		public void Pereris_Okno(object s, EventArgs a)
		{
			Invalidate();
		}

		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{	
			

			Action[] action = new Action[2];
			for (int i = 0; i < 2; i++)
			{
				action[i] = new Action(ball[i].DvigBall);
			}


			if ((110 < e.X) && (e.X < 130) && (15 < e.Y) && (35 > e.Y) && Tablo.moshno_tabsl == true)
			{					

				Tablo.sleva = true;
				Dvig1();			
				Task.Factory.StartNew(() =>
				{
					Parallel.Invoke(action);
				});
			}
			
			
			if ((150 < e.X) && (e.X < 170) && (15 < e.Y) && (35 > e.Y) && Tablo.moshno_tabpr == true)
			{

				Tablo.sleva = false;
				Dvig1();
				Task.Factory.StartNew(() =>
				{
					Parallel.Invoke(action);
				});
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////// 
    class Ball
	{
		object q;
		public Ball(int X, int Y)
		{
			x = X;
			y = Y;

			xconst = X;
			yconst = Y;
		}

		

		public int xconst;
		public int yconst;


		public event Delegat1 evTablo;		

		public bool start = false;
		bool live = true;

		public int x;
		public int y;
		int w = 30;
		int h = 30;
		int speed = 5;

		SolidBrush brush = new SolidBrush(Color.Orange);

		Tablo tablo = new Tablo();

		public void Start()
		{
			start = true;
			
		}
		public void DvigBall()
		{
			live = true;
			while (live)
			{
				while ((start == true) && (x < 210) && (y < 220))
				{
					tablo.tab_nelzya();
					x = x + speed;
					y = y + speed;
					Thread.Sleep(100);

					if (x == 210)
					{
						start = false;
						x = xconst;
						y = yconst;
						evTablo();
						live = false;
					}
				}
			}
		}


		public void DrawBall(object o, PaintEventArgs e)


		{
			e.Graphics.FillEllipse(brush, x, y, w, h);
		}


		
	}


	//////////////////////////////////////////////////////////////////////////////////////////////// 
	class Korzina
	{
		public int x_korzina;
		public int y_korzina;
		int w;
		int h;

		public Korzina()
		{
			x_korzina = 200;
			y_korzina = 200;
			w = 40;
			h = 40;
		}


		public void DrawKorzina(object o, PaintEventArgs e)
		{
			Pen pen = new Pen(Brushes.Red);
			pen.Width = 4;
			e.Graphics.DrawEllipse(pen, x_korzina, y_korzina, w, h);
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////// 
	class Tablo
	{

		public static int nol_sleva = 0;
		public static int nol_sprava = 0;
		public static bool sleva;

		public static void TabSlevTrue()
		{
			sleva = true;
		}

		public void plus()
		{
			if (sleva == true)
				nol_sleva++;
			else
				nol_sprava++;
			moshno_tabsl = true;
			moshno_tabpr = true;
		}
		public void tab_nelzya()
		{
			moshno_tabsl = false;
			moshno_tabpr = false;
		}

		public static bool moshno_tabsl = true;
		public static bool moshno_tabpr = true;

		int wT = 20;
		int hT = 20;

		float xtablosl = 110;
		float ytablosl = 15;
		float xtablopr = 150;
		float ytablopr = 15;

		public void risov_tablo(object o, PaintEventArgs e)
		{
			Font drawFont = new Font("Arial", 16);
			SolidBrush drawBrush = new SolidBrush(Color.Black);
			StringFormat drawFormat = new StringFormat();
			e.Graphics.DrawString(Convert.ToString(nol_sleva), drawFont, drawBrush, xtablosl, ytablosl);

			e.Graphics.DrawString(Convert.ToString(nol_sprava), drawFont, drawBrush, xtablopr, ytablopr);
			e.Graphics.DrawRectangle(Pens.Black, xtablosl, ytablosl, wT, hT);
			e.Graphics.DrawRectangle(Pens.Black, xtablopr, ytablopr,

			wT, hT);
			drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
		}

	}
}
