using IronOcr;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvitoTelegramBot.Avito
{
    internal static class AutoMessenger
    {
        private const string Browser = "chrome";
        private const string AvitoScreenshotFileName = "temp.png";

        private const int MouseX = 1070;

        private const int MouseYwithDelivery = 750;
        private const int MouseYwithoutDelivery = 500;

        private const string Message = "Здравствуйте! Когда можно забрать?";

        public static void SendAvitoMessage(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return;

            var proc = Process.Start("explorer", uri);
            Task.Delay(WaitTimeAfterTabLoaded).Wait();
            if (WithDelivery)
                OpenChat(MouseX, MouseYwithDelivery);
            else
                OpenChat(MouseX, MouseYwithoutDelivery);

            Task.Delay(WaitTimeForChat).Wait();
            TypeMessage(Message, proc);
            Task.Delay(WaitTimeForMessageDelivery).Wait();
            foreach (Process process in Process.GetProcessesByName(Browser))
                process.Kill();
        }

        private static IronTesseract s_tesseractInstance;
        private static IronTesseract Tesseract
        {
            get
            {
                if (s_tesseractInstance == null)
                {
                    s_tesseractInstance = new IronTesseract();
                    s_tesseractInstance.Language = OcrLanguage.Russian;
                }

                return s_tesseractInstance;
            }
        }

        private static bool WithDelivery
        {
            get
            {
                GetScreenShot().Save(AvitoScreenshotFileName);
                using var input = new OcrInput(AvitoScreenshotFileName);
                var screenInfo = Tesseract.Read(input);
                return screenInfo.Text.Contains("Купить с доставкой") && screenInfo.Text.Contains("Добавить в корзину");
            }
        }

        private static TimeSpan WaitTimeAfterTabLoaded
        {
            get
            {
                return TimeSpan.FromSeconds(6);
            }
        }

        private static TimeSpan WaitTimeForChat
        {
            get
            {
                return TimeSpan.FromSeconds(2);
            }
        }

        private static TimeSpan WaitTimeForMessageDelivery
        {
            get
            {
                return TimeSpan.FromSeconds(2);
            }
        }

        //This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        private static void OpenChat(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);

        [STAThread]
        private static void TypeMessage(string message, Process proc)
        {
            SetForegroundWindow(proc.Handle);
            SendKeys.SendWait(message);
            SendKeys.SendWait("{ENTER}");
        }

        private static Bitmap GetScreenShot()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                Screen.PrimaryScreen.Bounds.Height,
                                PixelFormat.Format24bppRgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }
    }
}
