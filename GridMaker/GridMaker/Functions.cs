﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GridMaker
{
    static class Functions
    {
        public static Point HoverLocation { get; set; } = Point.Empty;
        private static readonly List<IndexTile> TileList = new List<IndexTile>();

        #region Tile Iteration

        /// <summary>
        /// Populate a list of tiles
        /// </summary>
        /// <param name="form"></param>
        public static void MakeGrid(IndexSelector form)
        {
            TileList.Clear();
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            Size cellSize = new Size(bitmap.Width / form.GridSize.Width, bitmap.Height / form.GridSize.Height);
            for (int i = 0; i < form.GridSize.Width; i++)
            {
                for (int j = 0; j < form.GridSize.Height; j++)
                {
                    Rectangle rectangle = new Rectangle(i * cellSize.Width, j * cellSize.Height, cellSize.Width, cellSize.Height);
                    IndexTile tile = new IndexTile(rectangle, i, form.GridSize.Height - j - 1);
                    if (form.SkippedIndices.Contains(tile.Location)) tile.ChangeColor();
                    TileList.Add(tile);
                }
            }
            DrawGrid(form);
            DrawTiles(form);
        }

        /// <summary>
        /// Toggle the highlight param for the tile under the cursor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="form"></param>
        public static void HighlightTile(Point position, IndexSelector form)
        {
            Point zoomPos = ZoomMousePos(position, form);
            TileList.ForEach(x => x.Highlight = x.Rectangle.Contains(zoomPos));     
            DrawTiles(form);
        }

        /// <summary>
        /// Change the color of the tile under the cursor
        /// </summary>
        /// <param name="click"></param>
        /// <param name="dragType"></param>
        /// <param name="form"></param>
        /// <param name="waitForExit"></param>
        public static void ClickTile(Point click, ref IndexSelector.DragType dragType, IndexSelector form, bool waitForExit = false)
        {
            Point zoomClick = ZoomMousePos(click, form);
            foreach (IndexTile tile in TileList.Where(x => x.Rectangle.Contains(zoomClick)))
            {
                if (tile.Rectangle.Contains(zoomClick))
                {
                    if (waitForExit && tile.Highlight)
                        return;
                    switch (dragType)
                    {
                        case IndexSelector.DragType.Skip:
                            tile.SetState(false);
                            dragType = IndexSelector.DragType.Skip;
                            break;
                        case IndexSelector.DragType.Include:
                            tile.SetState(true);
                            dragType = IndexSelector.DragType.Include;
                            break;
                        case IndexSelector.DragType.Unknown:
                            tile.ChangeColor();
                            dragType = tile.Skipped ? IndexSelector.DragType.Skip : IndexSelector.DragType.Include;
                            break;
                        case IndexSelector.DragType.Null:
                            dragType = IndexSelector.DragType.Null;
                            break;
                        default:
                            break;
                    }
                    DrawTiles(form);
                    return;
                }
            }
        }

        public static void SetAllTiles(bool state, IndexSelector form)
        {
            foreach (IndexTile tile in TileList)
                tile.SetState(state);
            DrawTiles(form);
        }

        #endregion

        #region Drawing

        private static void DrawGrid(IndexSelector form)
        {
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (IndexTile tile in TileList)
                {
                    g.DrawRectangle(Pens.Black, tile.Rectangle);
                }
            }
            form.pictureBox.Image = bitmap;
        }

        private static void DrawTiles(IndexSelector form)
        {
            Bitmap bitmap = (Bitmap)form.pictureBox.BackgroundImage.Clone();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IndexTile tileUnderCursor = null;
                foreach (IndexTile tile in TileList.Where(x => x.NeedsUpdate || x.Highlight))
                {
                    if (tile.Highlight)
                    {
                        // Take advantage of the fact that a cell is highlighted when it changes color
                        g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(90, Color.Black)), tile.Rectangle);
                        tileUnderCursor = tile;
                    }
                    else
                    {
                        // Look for previously highlighted cells
                        double A = bitmap.GetPixel(tile.Rectangle.X, tile.Rectangle.Y).A;
                        if (A != 100)
                            g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                    }
                }

                if (tileUnderCursor != null)
                    HoverLocation = new Point(tileUnderCursor.Location.X + 1, tileUnderCursor.Location.Y + 1);
            }
            form.pictureBox.BackgroundImage = bitmap;
        }

        #endregion

        /// <summary>
        /// Copy paste method for adjusting mouse pos to pictureBox set to Zoom
        /// </summary>
        /// <param name="click"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Point ZoomMousePos(Point click, IndexSelector form)
        {
            PictureBox pbx = form.pictureBox;
            float BackgroundImageAspect = pbx.BackgroundImage.Width / (float)pbx.BackgroundImage.Height;
            float controlAspect = pbx.Width / (float)pbx.Height;
            PointF pos = new PointF(click.X, click.Y);
            if (BackgroundImageAspect > controlAspect)
            {
                float ratioWidth = pbx.BackgroundImage.Width / (float)pbx.Width;
                pos.X *= ratioWidth;
                float scale = pbx.Width / (float)pbx.BackgroundImage.Width;
                float displayHeight = scale * pbx.BackgroundImage.Height;
                float diffHeight = pbx.Height - displayHeight;
                diffHeight /= 2;
                pos.Y -= diffHeight;
                pos.Y /= scale;
            }
            else
            {
                float ratioHeight = pbx.BackgroundImage.Height / (float)pbx.Height;
                pos.Y *= ratioHeight;
                float scale = pbx.Height / (float)pbx.BackgroundImage.Height;
                float displayWidth = scale * pbx.BackgroundImage.Width;
                float diffWidth = pbx.Width - displayWidth;
                diffWidth /= 2;
                pos.X -= diffWidth;
                pos.X /= scale;
            }
            return new Point((int)pos.X, (int)pos.Y);
        }

        public static List<Point> GetSkippedTileLocations() => TileList.Where(x => x.Skipped).Select(x => x.Location).ToList();
    }
}
