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
using System.Threading;
using System.Windows.Threading;

namespace Threading
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void CmdMonster_Click(object sender, RoutedEventArgs e)
		{
			Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
			newWindowThread.SetApartmentState(ApartmentState.STA);
			newWindowThread.IsBackground = true;
			newWindowThread.Start();
		}

		private void ThreadStartingPoint()
		{
			Monster[] _monsters = new Monster[5];
			for (int i = 0; i < 5; i++)
			{
				Monster ml = new Monster();
				ml.Content = i;
				ml.Width = 20;
				ml.Height = ml.Width;
				ml.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
				_monsters[i] = ml;
				MapCanvas.Children.Add(ml);
			}
			while (true)
			{
				Thread.Sleep(100);
				for (int i = 0; i < 5; i++)
				{
					Monster ml = _monsters[i];
					ml.Move();
				}
			}
		}
	}

	class Monster : Label
	{
		public void Move()
		{
			Canvas.SetLeft(this, (new Random().NextDouble()) * 500);
			Canvas.SetTop(this, (new Random().NextDouble()) * 300);
		}
	}
}
