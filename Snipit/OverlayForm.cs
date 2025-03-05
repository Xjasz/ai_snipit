using System;
using System.Drawing;
using System.Windows.Forms;

namespace Snipit
{
    public class OverlayForm : Form
    {
        private bool _isDragging = false;
        private Point _startPoint;
        private Point _endPoint;
        private Rectangle _dragRect;
        private MainForm _mainForm;

        public OverlayForm(MainForm mainForm)
        {
            _mainForm = mainForm;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LightGray;
            Opacity = 0.3;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            MouseDown += OverlayForm_MouseDown;
            MouseMove += OverlayForm_MouseMove;
            MouseUp += OverlayForm_MouseUp;
            Paint += OverlayForm_Paint;
            KeyDown += OverlayForm_KeyDown;
            Activated += (s, e) => Focus();
        }

        public void ShowOverlay()
        {
            _dragRect = Rectangle.Empty;
            _isDragging = false;
            if (!_mainForm.useLiveExtraction)
            {
                SetBackgroundScreenshot();
            }
            else
            {
                SetLightGrayOverlay();
            }
            Show();
            BringToFront();
            Focus();
        }

        private void SetLightGrayOverlay()
        {
            BackgroundImage = null;
            BackColor = Color.LightGray;
            Opacity = 0.3;
        }

        private void SetBackgroundScreenshot()
        {
            try
            {
                var screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                }
                BackgroundImage = screenshot;
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screenshot: {ex.Message}");
            }
        }

        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _startPoint = e.Location;
                _endPoint = e.Location;
                _dragRect = new Rectangle(_startPoint, new Size(0, 0));
            }
            else if (e.Button == MouseButtons.Right)
            {
                Hide();
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _endPoint = e.Location;
                _dragRect = new Rectangle(
                    Math.Min(_startPoint.X, _endPoint.X),
                    Math.Min(_startPoint.Y, _endPoint.Y),
                    Math.Abs(_endPoint.X - _startPoint.X),
                    Math.Abs(_endPoint.Y - _startPoint.Y)
                );
                Invalidate();
            }
        }

        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                if (_dragRect.Width * _dragRect.Height >= 50)
                {
                    Program.CaptureScreenshot(_dragRect);
                }
                Hide();
            }
        }

        private void OverlayForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }

        private void OverlayForm_Paint(object sender, PaintEventArgs e)
        {
            if (_isDragging)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(100, Color.Blue)))
                {
                    e.Graphics.FillRectangle(brush, _dragRect);
                }
            }
        }
    }
}
