using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows;

namespace WPF
{
	public class Cell:Label
	{
		public static int CellWidth = 40;
		public static int CellHeight = 40;
		public int X { get { return _x; } }
		public int Y { get { return _y; } }
		public bool Clicked { get { return _clicked; } }
		protected int _x;
		protected int _y;
		protected bool _clicked = false;
		public Cell(int x,int y)
		{
			_x = x;
			_y = y;
			Width = CellWidth;
			Height = CellHeight;
			Canvas.SetLeft(this,x*CellWidth);
			Canvas.SetTop(this,y*CellHeight);
			HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
			VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
			Content = "";
		}
	}

	public class PathCell: Cell
	{
		public PathCell(int x, int y) : base(x, y)
		{
			Background = Brushes.White;
		}
	}
	public class TreeCell: Cell
	{
		public TreeCell(int x, int y) : base(x, y)
		{
			Background = Brushes.Green;
			Foreground = Brushes.White;
			Content = "树";
		}
	}
	public class BaseCell: Cell 
	{
		public BaseCell(int x, int y) : base(x, y)
		{
			Background = Brushes.Black;
			Foreground = Brushes.White;
			Content = "基地";
		}
	}
	public class ActiveCell : Cell
	{
		protected Thickness _noneBorder = new Thickness(0);
		protected Thickness _hoverBorder = new Thickness(3);
		public ActiveCell(int x, int y)
			: base(x, y)
		{
			BorderBrush = Brushes.Blue;
			MouseMove += new MouseEventHandler(ActiveCell_MouseMove);
			MouseLeave += new MouseEventHandler(ActiveCell_MouseLeave);
			MouseLeftButtonUp += new MouseButtonEventHandler(ActiveCell_MouseLeftButtonUp);
		}
		private void ActiveCell_MouseLeftButtonUp(object sender, MouseEventArgs e)
		{
			_clicked = true;
		}
		private void ActiveCell_MouseMove(object sender, MouseEventArgs e)
		{
			BorderThickness = _hoverBorder;
		}
		private void ActiveCell_MouseLeave(object sender, MouseEventArgs e)
		{
			BorderThickness = _noneBorder;
		}
	}
	public class GunCell : ActiveCell
	{
		protected bool _circleAdded = false;
		protected DoubleCollection _circleDash = new DoubleCollection();
		public int Power { get { return 1; } }
		public GunCell(int x, int y)
			: base(x, y)
		{
			Background = Brushes.Blue;
			MouseMove += new MouseEventHandler(GunCell_MouseMove);
			MouseLeave += new MouseEventHandler(GunCell_MouseLeave);
			_circleDash.Add(3); _circleDash.Add(1); _circleDash.Add(3); _circleDash.Add(1);
		}
		public int Fire(Monster monster)
		{
			monster.Health -= this.Power;
			return monster.Health;
		}
		private void GunCell_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!_circleAdded) return;
			Canvas c = this.Parent as Canvas;
			foreach (UIElement ue in c.Children)
			{
				Ellipse el = ue as Ellipse;
				if (el != null)
				{
					c.Children.Remove(el);
					_circleAdded = false;
					break;
				}
			}
		}
		private void GunCell_MouseMove(object sender, MouseEventArgs e)
		{
			if (_circleAdded) return;
			Canvas c = this.Parent as Canvas;
			System.Windows.Shapes.Ellipse rangeCircle = new Ellipse();
			rangeCircle.Width = this.Width * 3;
			rangeCircle.Height = this.Height * 3;
			rangeCircle.Stroke = Brushes.Red;
			rangeCircle.StrokeThickness = 3;
			rangeCircle.StrokeDashArray = _circleDash;
			Canvas.SetLeft(rangeCircle, Canvas.GetLeft(this)-this.Width);
			Canvas.SetTop(rangeCircle, Canvas.GetTop(this) - this.Height);
			c.Children.Add(rangeCircle);
			_circleAdded = true;
		}
	}

	public class LandCell : ActiveCell
	{
		public LandCell(int x, int y):base(x,y)
		{
			Background = Brushes.SandyBrown;
		}
	}


	public class Monster : Cell
	{
		public Monster(int x, int y):base(x,y){}
		public int Position { get; set; }
		public int Health
		{
			get { return _health; }
			set
			{
			this._health = value;
			//this.Opacity = this._health / 10.0;
			Content = this.Health;
		} }
		protected int _health;
		public void MoveTo(int x, int y)
		{
			_x = x;
			_y = y;
			Canvas.SetLeft(this,x*CellWidth);
			Canvas.SetTop(this,y*CellHeight);
		}
	}
	public class Imp : Monster
	{
		public Imp(int x, int y) : base(x, y) {
			BorderBrush = Brushes.White;
			BorderThickness = new Thickness(3);
			Background = Brushes.Yellow;
			this.Health = 10;
			Content = this.Health;
		}
	}

}
