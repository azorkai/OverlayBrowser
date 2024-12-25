using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

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

        private dynamic settings;

        public Form1()
        {
            InitializeComponent();

            // Ayarlarý yükle veya oluþtur
            LoadOrCreateSettings();

            SetupForm();
            SetupDragArea();

            // WebView2 baþlatma iþlemini asenkron çalýþtýr
            SetupWebViewAsync().ConfigureAwait(false);

            // Mevcut panel1'i estetik hale getir
            var aestheticPanel = new AestheticPanel();
            aestheticPanel.ApplyStyles(panel1);

            // Global hotkey kaydý
            RegisterHotKeys();
        }

        private void LoadOrCreateSettings()
        {
            string settingsFile = "settings.json";

            if (File.Exists(settingsFile))
            {
                string json = File.ReadAllText(settingsFile);
                settings = JsonConvert.DeserializeObject(json);
            }
            else
            {
                settings = new
                {
                    Opacity = 80,
                    StartUrl = "https://google.com",
                    HotkeyModifier = MOD_ALT,
                    HotkeyKey = "M"
                };

                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(settingsFile, json);
            }
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None; // Kenarlýklarý kaldýr
            this.TopMost = true; // Her zaman üstte
            this.StartPosition = FormStartPosition.CenterScreen; // Baþlangýç pozisyonu ekran ortasý

            // Þeffaflýk seviyesini ayarla
            SetFormOpacity((int)settings.Opacity);
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
            // Ayarlardan hotkey al
            uint modifier = (uint)settings.HotkeyModifier;
            Keys key = (Keys)Enum.Parse(typeof(Keys), settings.HotkeyKey.ToString());

            // Global hotkey kaydý
            RegisterHotKey(this.Handle, HOTKEY_ID_TOGGLE, modifier, (uint)key);
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
                        this.Activate(); // Formu etkinleþtir
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

        private async Task SetupWebViewAsync()
        {
            // Ayarlardan baþlangýç URL'sini al
            string startUrl = settings.StartUrl;

            try
            {
                // webView21 elementini kontrol et ve baþlat
                await webView21.EnsureCoreWebView2Async();

                if (webView21.CoreWebView2 != null)
                {
                    webView21.CoreWebView2.Navigate(startUrl);
                }
                else
                {
                    MessageBox.Show("WebView2 baþlatýlamadý!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 baþlatýlýrken bir hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
