using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OverlayBrowser
{
    public partial class Form1 : Form
    {
        // Windows API ile global hotkey tan�m�
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Modifiers
        private const uint MOD_ALT = 0x0001; // Alt tu�u

        // Key IDs
        private const int HOTKEY_ID_TOGGLE = 1; // Minimize ve restore i�in hotkey ID

        // Windows API �effafl�k tan�m�
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

            // Global hotkey kayd�
            RegisterHotKeys();
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // Kenarl�klar� kald�r
            this.TopMost = true; // Her zaman �stte
            this.StartPosition = FormStartPosition.CenterScreen; // Ba�lang�� pozisyonu ekran ortas�

            // �effafl�k seviyesini ayarla (%80 g�r�n�rl�k, %20 �effafl�k)
            SetFormOpacity(80);
        }

        private void SetFormOpacity(int opacityPercentage)
        {
            if (opacityPercentage < 0 || opacityPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(opacityPercentage), "�effafl�k y�zdesi 0 ile 100 aras�nda olmal�d�r.");

            IntPtr hwnd = this.Handle;
            int currentStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, currentStyle | WS_EX_LAYERED); // Layered pencere ayar�
            byte alpha = (byte)(opacityPercentage * 255 / 100); // Y�zdeyi 0-255 aral���na �evir
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
                        this.WindowState = FormWindowState.Minimized; // Formu simge durumuna k���lt
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal; // Formu normale d�nd�r
                        this.BringToFront(); // Formu �ne getir
                    }
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Hotkey kay�tlar�n� kald�r
            UnregisterHotKeys();
            base.OnFormClosing(e);
        }

        private void SetupDragArea()
        {
            // panel1 elementini s�r�kleme alan� olarak kullan
            panel1.Cursor = Cursors.SizeAll; // S�r�kleme i�aret�isi
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
            webView21.EnsureCoreWebView2Async(null); // WebView2'nin ba�lat�lmas�
        }

        private void WebView21_CoreWebView2InitializationCompleted(object sender, EventArgs e)
        {
            webView21.CoreWebView2.Navigate("https://google.com"); // Hedef URL'yi buraya yazabilirsiniz
        }

        // ESC tu�uyla uygulamay� kapatma
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
