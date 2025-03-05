using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace Snipit
{
    public static class Program
    {

        [DllImport("user32.dll")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "System Reference")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Point lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr _mouseHookID = IntPtr.Zero;
        private static IntPtr _keyhookID = IntPtr.Zero;
        private static LowLevelProc _mouseProc = MouseHookCallback;
        private static LowLevelProc _keyproc = KeyHookCallback;

        private const uint KEYEVENT_DOWNKEY = 0x0001; // Keyboard down event
        private const uint KEYEVENT_UPKEY = 0x0002; // Keyboard up event

        private const int KEYBOARD_SNIPIT_KEY = 189; // Keyboard Snipit key = |
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;

        private const int WH_KEYBOARD_LL = 13;

        private static bool _lControlKey = false;

        private static readonly Thread _SnipitThread = new Thread(SnipitThread.BackgroundThread) { IsBackground = true, Name = "SnipitThread" };
        private static readonly Bitmap _screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        private static MainForm _mainForm;
        private static OverlayForm _overlayForm;

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string rootDirectory = Path.Combine(userDirectory, ".snipit");
        public static string gptDirectory = Path.Combine(rootDirectory, "GPT_Responses");
        public static string ocrDirectory = Path.Combine(rootDirectory, "OCR_Extracts");
        public static string snipsDirectory = Path.Combine(rootDirectory, "Snips");

        public static string currentImagePath;
        public static DateTime currentImageTime;


        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202, WM_MOUSEMOVE = 0x0200, WM_MOUSEWHEEL = 0x020A, WM_RBUTTONDOWN = 0x0204, WM_RBUTTONUP = 0x0205
        }

        public static void Main()
        {
            Debug.WriteLine("Main");
            _mainForm = new MainForm();
            _overlayForm = new OverlayForm(_mainForm);
            EnsureDirectoriesExist();
            _overlayForm.Hide();
            SnipitThread.StartThread();
            System.Windows.Forms.Application.Run(_mainForm);
            SnipitThread.StopThread();
            DisableHelper();
        }

        public static void EnableHelper()
        {
            Debug.WriteLine("EnableHelper");
            _keyhookID = SetHook(_keyproc, "KEY");
        }

        public static void DisableHelper()
        {
            Debug.WriteLine("DisableHelper");
            if (_keyhookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyhookID);
                _keyhookID = IntPtr.Zero;
            }
        }

        public static void UpdateSnipitScreenshot()
        {
            Debug.WriteLine("UpdateSnipitScreenshot");
        }

        private static void EnsureDirectoriesExist()
        {
            Debug.WriteLine("EnsureDirectoriesExist");
            try
            {
                if (!Directory.Exists(rootDirectory))
                    Directory.CreateDirectory(rootDirectory);
                if (!Directory.Exists(ocrDirectory))
                    Directory.CreateDirectory(ocrDirectory);
                if (!Directory.Exists(gptDirectory))
                    Directory.CreateDirectory(gptDirectory);
                if (!Directory.Exists(snipsDirectory))
                    Directory.CreateDirectory(snipsDirectory);
                Debug.WriteLine("Directories ensured.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create directories: {ex.Message}");
            }
        }

        private static IntPtr SetHook(LowLevelProc proc, string hookType)
        {
            Debug.WriteLine("SetHook");
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                if (hookType == "MOUSE")
                {
                    return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
                if (hookType == "KEY")
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);

                }
                return IntPtr.Zero;
            }
        }

        private static IntPtr KeyHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            var vkeycode = (Keys)vkCode;
            if (nCode >= 0 && wParam == 0x0100)
            {
                if (vkeycode == Keys.LControlKey || vkeycode == Keys.RControlKey)
                {
                    _lControlKey = true;
                }
                if (_lControlKey && vkeycode == Keys.OemQuestion)
                {
                    _mainForm.Invoke(new Action(ShowOverlay));
                    return 1;
                }
            }
            if (nCode >= 0 && wParam == 0x0101)
            {
                if (vkeycode == Keys.LControlKey || vkeycode == Keys.RControlKey)
                {
                    _lControlKey = false;
                }
            }
            return CallNextHookEx(_keyhookID, nCode, wParam, lParam);
        }

        public static void ShowOverlay()
        {
            _overlayForm.ShowOverlay();
        }

        private static void SaveOCRResult(string text)
        {
            try
            {
                string ocrFilePath = Path.Combine(ocrDirectory, $"{currentImageTime:yyyyMMdd_HHmmss}.txt");
                File.WriteAllText(ocrFilePath, text);
                Debug.WriteLine($"OCR result saved to: {ocrFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save OCR result: {ex.Message}");
            }
        }

        public static void CaptureScreenshot(Rectangle dragArea)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(dragArea.Width, dragArea.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(dragArea.Location, Point.Empty, dragArea.Size);
                    }
                    currentImageTime = DateTime.Now;
                    currentImagePath = Path.Combine(snipsDirectory, $"{currentImageTime:yyyyMMdd_HHmmss}.png");
                    bitmap.Save(currentImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    Debug.WriteLine($"Screenshot saved to {currentImagePath}");
                    _mainForm.UpdatePictureBox(new Bitmap(bitmap));
                    if (_mainForm.useTextExtraction)
                    {
                        var extractedText = PerformOCR(bitmap);
                        Debug.WriteLine($"Extracted Text: {extractedText}");
                        SaveOCRResult(extractedText);
                        _mainForm.UpdateResponseLabel(extractedText);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private static void SnipScreenShotEvent()
        {
            Debug.WriteLine("SnipScreenShotEvent");
            PressKey(Keys.Enter, true);
            Thread.Sleep(10);
            SendStringEvent("Snip ScreenShot Event");
            PressKey(Keys.Enter, true);
            Thread.Sleep(10);
        }

        private static void PressKey(Keys key, bool isTrue)
        {
            var byteVal = Convert.ToByte(key);
            keybd_event(byteVal, 0, KEYEVENT_DOWNKEY, 0);
        }

        private static string PerformOCR(Bitmap bitmap)
        {
            try
            {
                var tessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tesseract_files");
                var tessDataPath = Path.Combine(tessPath, "tessdata");
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    using (var pixImage = Pix.LoadFromMemory(BitmapToBytes(bitmap)))
                    {
                        using (var page = engine.Process(pixImage))
                        {
                            var text = page.GetText();
                            return text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during OCR: {ex.Message}");
                return string.Empty;
            }
        }

        private static byte[] BitmapToBytes(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private static void SendStringEvent(string theString)
        {
            theString = theString.ToUpper();
            foreach (char c in theString)
            {
                Keys key = Keys.Space;
                var keyFound = true;
                if (char.IsLetter(c))
                {
                    if (!Enum.TryParse(c.ToString(), true, out key))
                    {
                        keyFound = false;
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (!Enum.TryParse("D" + c.ToString(), true, out key))
                    {
                        keyFound = false;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case ' ':
                            key = Keys.Space;
                            break;
                        case '/':
                            key = Keys.OemQuestion;
                            break;
                        default:
                            keyFound = false;
                            break;
                    }
                }
                if (keyFound)
                {
                    PressKey(key, true);
                }
                else
                {
                    Debug.WriteLine($"Unrecognized character: {c}");
                }
            }
        }


        private static class SnipitThread
        {
            public static void BackgroundThread()
            {
                Debug.WriteLine(string.Format("Thread {0} has started", _SnipitThread.Name));
                try
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (_mainForm.AppRunningState)
                        {
                            //CheckPixel();
                            //CheckPixelTest();
                            Thread.Sleep(500);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine($"Thread {_SnipitThread.Name} has been canceled");
                }
                Debug.WriteLine(string.Format("Thread {0} has stoped", _SnipitThread.Name));
            }

            public static void StartThread()
            {
                _SnipitThread.Start();
            }

            public static void StopThread()
            {
                _cancellationTokenSource.Cancel();
                _SnipitThread.Join();
            }

            private static void CheckPixelTest()
            {
                Debug.WriteLine("CheckPixelTest");
                var location = new Point(0, 0);
                GetCursorPos(ref location);
                var locationColor = GetColorAt(location);
                var stringLocation = string.Format("({0},{1})", location.X.ToString(), location.Y.ToString());
                var stringColor = "(" + locationColor.R.ToString() + "," + locationColor.G.ToString() + "," + locationColor.B.ToString() + ")";
                var formatedText = string.Format("Location: {0}  Color: {1}", stringLocation, stringColor);
                Debug.WriteLine(formatedText);
            }

            private static void CheckPixel()
            {
                Debug.WriteLine("CheckPixel");
            }

            private static Color GetColorAt(Point location)
            {
                Debug.WriteLine("GetColorAt");
                using (Graphics gdest = Graphics.FromImage(_screenPixel))
                {
                    using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        var hSrcDC = gsrc.GetHdc();
                        var hDC = gdest.GetHdc();
                        var retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                        gdest.ReleaseHdc();
                        gsrc.ReleaseHdc();
                    }
                }
                return _screenPixel.GetPixel(0, 0);
            }
        }
    }
}
