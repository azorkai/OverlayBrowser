using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OverlayBrowser
{
    public partial class Form1 : Form
    {
        // Windows API ile global hotkey tanýmý
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifiers
        private const uint MOD_ALT = 0x0001; // Alt tuþu

        // Key IDs
        private const int HOTKEY_ID_TOGGLE = 1; // Minimize ve restore için hotkey ID

        // Windows API þeffaflýk tanýmý
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, byte crKey, byte bAlpha, uint dwFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_ALPHA = 0x2;

        private bool isDragging = false;
        private int currentX, currentY;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            SetupDragArea();
            SetupWebView();

            // Mevcut panel1'i estetik hale getir
            var aestheticPanel = new AestheticPanel();
            aestheticPanel.ApplyStyles(panel1);

            // Global hotkey kaydý
            RegisterHotKeys();
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // Kenarlýklarý kaldýr
            this.TopMost = true; // Her zaman üstte
            this.StartPosition = FormStartPosition.CenterScreen; // Baþlangýç pozisyonu ekran ortasý

            // Þeffaflýk seviyesini ayarla (%80 görünürlük, %20 þeffaflýk)
            SetFormOpacity(80);
        }

        private void SetFormOpacity(int opacityPercentage)
        {
            if (opacityPercentage < 0 || opacityPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(opacityPercentage), "Þeffaflýk yüzdesi 0 ile 100 arasýnda olmalýdýr.");

            IntPtr hwnd = this.Handle;
            int currentStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, currentStyle | WS_EX_LAYERED); // Layered pencere ayarý
            byte alpha = (byte)(opacityPercentage * 255 / 100); // Yüzdeyi 0-255 aralýðýna çevir
            SetLayeredWindowAttributes(hwnd, 0, alpha, LWA_ALPHA);
        }

        private void RegisterHotKeys()
        {
            // ALT + M ile minimize ve restore
            RegisterHotKey(this.Handle, HOTKEY_ID_TOGGLE, MOD_ALT, (uint)Keys.M);
        }

        private void UnregisterHotKeys()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_TOGGLE);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_TOGGLE)
                {
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        this.WindowState = FormWindowState.Minimized; // Formu simge durumuna küçült
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal; // Formu normale döndür
                        this.BringToFront(); // Formu öne getir
                    }
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Hotkey kayýtlarýný kaldýr
            UnregisterHotKeys();
            base.OnFormClosing(e);
        }

        private void SetupDragArea()
        {
            // panel1 elementini sürükleme alaný olarak kullan
            panel1.Cursor = Cursors.SizeAll; // Sürükleme iþaretçisi
            panel1.MouseDown += Panel1_MouseDown;
            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseUp += Panel1_MouseUp;
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            currentX = e.X;
            currentY = e.Y;
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Left += e.X - currentX;
                this.Top += e.Y - currentY;
            }
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void SetupWebView()
        {
            // webView21 elementini kullan
            webView21.CoreWebView2InitializationCompleted += WebView21_CoreWebView2InitializationCompleted;
            webView21.EnsureCoreWebView2Async(null); // WebView2'nin baþlatýlmasý
        }

        private void WebView21_CoreWebView2InitializationCompleted(object sender, EventArgs e)
        {
            webView21.CoreWebView2.Navigate("https://google.com"); // Hedef URL'yi buraya yazabilirsiniz
        }

        // ESC tuþuyla uygulamayý kapatma
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Application.Exit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
