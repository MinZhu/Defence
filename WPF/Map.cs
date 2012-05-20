using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Controls;

namespace WPF
{
	public class Map
	{
		public int Width { get { return 800; } }
		public int Height { get { return 600; } }
		public int CellWidth { get { return Cell.CellWidth; } }
		public int CellHeight { get { return Cell.CellHeight; } }
		public int Cols { get { return this.Width / this.CellWidth; } }
		public int Rows { get { return this.Height/ this.CellHeight; } }
		public List<PathCell> PathCells { get { return _pathCells; } }
		public List<GunCell> GunCells { get { return _gunCells; } }
		protected Bitmap _mapBitmap = null;
		protected List<PathCell> _pathCells = null;
		protected List<GunCell> _gunCells = null;
		protected PathCell _entryCell = null;
		protected BaseCell _baseCell = null;
		protected Canvas _mapCanvas = null;

		public Map(Canvas canvas)
		{
			_mapCanvas = canvas;
		}
		protected void releaseMapBitmap()
		{
			if (_mapBitmap == null) return;
			_mapBitmap = null;
		}
		public void AddGunCell(GunCell gc)
		{
			_gunCells.Add(gc);
		}
		protected void initGunCells()
		{
			_gunCells = new List<GunCell>();
		}
		protected void initPathCells()
		{
			_pathCells = new List<PathCell>();
			sortPathCell(_entryCell);
		}
		protected void sortPathCell(PathCell currentPC)
		{
			_pathCells.Add(currentPC);
			foreach(System.Windows.UIElement ue in _mapCanvas.Children)
			{
				PathCell pc = ue as PathCell;
				if(pc==null||_pathCells.Contains(pc))continue;
				if (
					(pc.X == (currentPC.X + 1) && pc.Y == currentPC.Y) ||
					(pc.X == (currentPC.X - 1) && pc.Y == currentPC.Y) ||
					(pc.Y == (currentPC.Y + 1) && pc.X == currentPC.X) ||
					(pc.Y == (currentPC.Y - 1) && pc.X == currentPC.X)
					)
				{
					sortPathCell(pc);
				}
			}
		}
		protected int getCellIndex(int x, int y)
		{
			if (x < 0 || x >= Cols || y < 0 || y >= Rows) { throw new Exception("地图单元坐标超过地图大小。"); }
			return y * Cols + x;
		}
		//public void Draw(Canvas canvas)
		//{
		//    if (_mapCells == null) { throw new Exception("没有加载地图。"); }
		//}
		public bool Load(string filePathName)
		{
			releaseMapBitmap();
			// 加载地图图片。
			_mapBitmap = new Bitmap(filePathName);
			if (_mapBitmap.Width < Cols || _mapBitmap.Height < Rows) { return false; }

			for (int x = 0; x < Cols; x++)
			{
				for (int y = 0; y < Rows; y++)
				{
					System.Drawing.Color c = _mapBitmap.GetPixel(x, y);
					if (c.R == 255 && c.G == 255 && c.B == 255)	// 白色，地面
					{
						_mapCanvas.Children.Add(new LandCell(x,y));
					}
					else
					{
						if (c.R == 0 && c.G == 255 && c.B == 0)	// 绿色，树
						{
							_mapCanvas.Children.Add(new TreeCell(x,y));
						}
						else
						{
							if (c.R == 0 && c.G == 0 && c.B == 255)	// 蓝色，怪物进攻路线
							{
								_mapCanvas.Children.Add(new PathCell(x,y));
							}
							else
							{
								if (c.R == 0 && c.G == 0 && c.B == 0)	// 黑色，基地
								{
									//if (_pathCells[x] != null) { throw new Exception("地图设计错误，基地只能有一个。"); }
									_baseCell = new BaseCell(x,y);
									_mapCanvas.Children.Add(_baseCell);
								}
								else
								{
									if (c.R == 255 && c.G == 0 && c.B == 0)	// 红色，怪物入口
									{
										//if (x != 0||_pathCells[0]!=null) { throw new Exception("地图设计错误，怪物入口必须在最左侧且只能有一个入口。"); }
										_entryCell = new PathCell(x, y);
										_mapCanvas.Children.Add(_entryCell);
									}
								}
							}
						}
					}
				}
			}
			initPathCells();
			initGunCells();
			return true;
		}
	}
}
