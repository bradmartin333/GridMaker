using System;
using System.Drawing;

namespace GridMaker
{
    public class IndexTile
    {
        private static readonly Color[] Colors = new Color[] { Color.Green, Color.White };

        private bool _NeedsUpdate = true;
        /// <summary>
        /// Parameter that allows for reduced number of drawing iterations
        /// </summary>
        public bool NeedsUpdate
        {
            get 
            {
                if (_NeedsUpdate)
                {
                    _NeedsUpdate = false;
                    return true;
                }
                return false; 
            }
        }

        private Rectangle _Rectangle;
        public Rectangle Rectangle 
        {
            get { return _Rectangle; }
        }

        private Color _Color = Colors[0];
        public Color Color 
        {
            get { return _Color; }
        }
        public bool Skipped { get => Color == Colors[1]; }

        private bool _LastHighlight;
        private bool _Highlight;
        public bool Highlight 
        { 
            get { return _Highlight; }
            set 
            { 
                _Highlight = value;

                if (_LastHighlight != _Highlight)
                {
                    _NeedsUpdate = true;
                    _LastHighlight = _Highlight;
                }
            }
        }

        private Point _Location;
        public Point Location
        {
            get { return _Location; }
        }

        public IndexTile(Rectangle rectangle, int row, int col)
        {
            _Rectangle = rectangle;
            _Location = new Point(row, col);
        }

        /// <summary>
        /// Cycle the tile's color through the DiscoTile Colors array
        /// </summary>
        public void ChangeColor()
        {
            int colorIdx = Array.IndexOf(Colors, _Color);
            _Color = colorIdx < Colors.Length - 1 ? Colors[colorIdx + 1] : Colors[0];
            _NeedsUpdate = true;
        }

        /// <summary>
        /// Green if true, White if false.
        /// Invalidates color cycling ability
        /// </summary>
        /// <param name="state"></param>
        public void SetState(bool state)
        {
            _Color = state ? Colors[0] : Colors[1];
            _NeedsUpdate = true;
        }
    }
}