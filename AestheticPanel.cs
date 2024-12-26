using System;
using System.Drawing;
using System.Windows.Forms;

namespace OverlayBrowser
{
    public class AestheticPanel
    {
        public void ApplyStyles(Panel panel1)
        {
            // Panel yüksekliğini ve arka plan rengini ayarla
            panel1.Height = 50;
            panel1.BackColor = Color.FromArgb(45, 45, 48); // Koyu gri tonlarda arka plan
            panel1.Paint += Panel1_Paint; // Gradient efektini ekle

            // Mevcut içerikleri temizle (Eğer varsa)
            panel1.Controls.Clear();

            // Sol üst köşeye bir simge ekle
            var logo = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(10, 10),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                // Image = Properties.Resources.logo // Eğer bir simge eklemek istiyorsanız
            };
            panel1.Controls.Add(logo);

            // Başlık ekle
            var titleLabel = new Label
            {
                Text = "Overlay Browser",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 12),
                BackColor = Color.Transparent
            };
            panel1.Controls.Add(titleLabel);

            // Kapatma butonu
            var closeButton = new Button
            {
                Text = "X",
                Size = new Size(40, 30),
                Location = new Point(panel1.Width - 50, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(200, 50, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (sender, e) => Application.Exit();
            panel1.Controls.Add(closeButton);

            // Simge durumuna küçültme butonu
            var minimizeButton = new Button
            {
                Text = "_",
                Size = new Size(40, 30),
                Location = new Point(panel1.Width - 100, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.Click += (sender, e) => ((Form)panel1.TopLevelControl).WindowState = FormWindowState.Minimized;
            panel1.Controls.Add(minimizeButton);

            // Ayarlar butonu
            var settingsButton = new Button
            {
                Text = "⚙", // Ayar simgesi (Unicode)
                Size = new Size(40, 30),
                Location = new Point(panel1.Width - 150, 10), // Sağ üstte konum
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.Click += (sender, e) =>
            {
                try
                {
                    string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
                    if (File.Exists(settingsFilePath))
                    {
                        System.Diagnostics.Process.Start("notepad.exe", settingsFilePath);
                    }
                    else
                    {
                        MessageBox.Show("settings.json not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while opening settings.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            panel1.Controls.Add(settingsButton);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            var rect = panel.ClientRectangle;

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect,
                Color.FromArgb(45, 45, 45), // Başlangıç rengi
                Color.FromArgb(30, 30, 30), // Bitiş rengi
                90F)) // Dikey geçiş
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
    }
}
