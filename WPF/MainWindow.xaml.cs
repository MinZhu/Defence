using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace WPF
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		Map _map = null;
		Imp _imp = null;
		List<PathCell> _pathCells;
		Storyboard _storyboard;
		MediaPlayer _gunSound = new MediaPlayer();
		public MainWindow()
		{
			InitializeComponent();
			_map = new Map(MapCanvas);
			_map.Load(System.Environment.CurrentDirectory+@"\Map\level1.bmp");
			MapCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(ActiveCell_MouseLeftButtonUp);

			_pathCells = _map.PathCells;
			initImp();
			initSound();
			DispatcherTimer timer = new DispatcherTimer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			timer.Start();

		}
		protected void initSound()
		{
			_gunSound.Open(new Uri(System.Environment.CurrentDirectory + @"\Sound\gun.mp3", UriKind.RelativeOrAbsolute));
		}
		protected void initImp()
		{
			PathCell pc = _pathCells[0];
			_imp = new Imp(pc.X, pc.Y);
			_imp.Position = 0;
			MapCanvas.Children.Add(_imp);
		}
		private void timer_Tick(object sender, EventArgs e)
		{
			// Move the monster
			if((_imp.Position+1)<_pathCells.Count){
				_imp.Position++;
				PathCell pc = _pathCells[_imp.Position];
				_imp.MoveTo(pc.X, pc.Y);
			}

			// Fire the monster
			foreach (GunCell gunCell in _map.GunCells)
			{
				// Fire checking
				if(
					((gunCell.X - 1) == _imp.X && ((gunCell.Y >= (_imp.Y - 1)) && (gunCell.Y <= (_imp.Y + 1)))) ||
					((gunCell.X + 1) == _imp.X && ((gunCell.Y >= (_imp.Y - 1)) && (gunCell.Y <= (_imp.Y + 1)))) ||
					((gunCell.X == _imp.X) && ((gunCell.Y >= (_imp.Y - 1)) && (gunCell.Y <= (_imp.Y + 1))))
				)
				{
					gunCell.Fire(_imp);

					Ellipse gunLine = new Ellipse();
					Canvas.SetLeft(gunLine, Canvas.GetLeft(gunCell) + gunCell.Width / 2);
					Canvas.SetTop(gunLine, Canvas.GetTop(gunCell) + gunCell.Height / 2);
					gunLine.StrokeThickness = 2;
					gunLine.Stroke = Brushes.Red;
					gunLine.Width = 5;
					gunLine.Height = 5;
					MapCanvas.Children.Add(gunLine);
					//创建移动动画
					_storyboard = new Storyboard();
					_storyboard.Completed += new EventHandler(storyboard_Completed);
					//创建X轴方向动画
					DoubleAnimation doubleAnimation = new DoubleAnimation(
						Canvas.GetLeft(gunCell) + gunCell.Width / 2,
						Canvas.GetLeft(_imp) + _imp.Width / 2,
						new Duration(TimeSpan.FromMilliseconds(100))
					);
					Storyboard.SetTarget(doubleAnimation, gunLine);
					Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
					_storyboard.Children.Add(doubleAnimation);
					//创建Y轴方向动画
					doubleAnimation = new DoubleAnimation(
						Canvas.GetTop(gunCell) + gunCell.Height / 2,
					  Canvas.GetTop(_imp) + _imp.Height / 2,
					  new Duration(TimeSpan.FromMilliseconds(100))
					);
					Storyboard.SetTarget(doubleAnimation, gunLine);
					Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
					_storyboard.Children.Add(doubleAnimation);
					//将动画动态加载进资源内
					if (!Resources.Contains("rectAnimation"))
					{
						Resources.Add("rectAnimation", _storyboard);
					}
					//动画播放
					_storyboard.Begin();
					_gunSound.Play();
					if (_imp.Health <= 0)
					{
						MapCanvas.Children.Remove(_imp);
						initImp();
					}
				}
			}

		}
		private void storyboard_Completed(object sender, EventArgs e)
		{
			Ellipse gunEllipse = Storyboard.GetTarget(_storyboard.Children[0]) as Ellipse;
			MapCanvas.Children.Remove(gunEllipse);
			_gunSound.Pause();
			_gunSound.Position = new TimeSpan(0);
		}
		private void ActiveCell_MouseLeftButtonUp(object sender, MouseEventArgs e)
		{
			foreach (UIElement ue in MapCanvas.Children)
			{
				if (ue is LandCell) {
					LandCell lc = (LandCell)ue;
					if (lc.Clicked)
					{
						GunCell gc = new GunCell(lc.X, lc.Y);
						MapCanvas.Children.Add(gc);
						_map.AddGunCell(gc);
						MapCanvas.Children.Remove(ue);
						break;
					}
				}
			}
		}
	}
}
