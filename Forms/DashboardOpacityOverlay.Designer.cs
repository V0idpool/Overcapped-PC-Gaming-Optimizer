using System.Drawing;
using System.Windows.Forms;

namespace Overcapped_Optimizer.Forms
{
    partial class DashboardOpacityOverlay
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlRoot = new Panel();
            pnlAccent = new Panel();
            lblTitle = new Label();
            pnlTrack = new Panel();
            pnlFill = new Panel();
            lblPercent = new Label();
            pnlRoot.SuspendLayout();
            pnlTrack.SuspendLayout();
            SuspendLayout();
            // 
            // pnlRoot
            // 
            pnlRoot.BackColor = Color.FromArgb(9, 9, 12);
            pnlRoot.BorderStyle = BorderStyle.FixedSingle;
            pnlRoot.Controls.Add(lblPercent);
            pnlRoot.Controls.Add(pnlTrack);
            pnlRoot.Controls.Add(lblTitle);
            pnlRoot.Controls.Add(pnlAccent);
            pnlRoot.Dock = DockStyle.Fill;
            pnlRoot.Location = new Point(0, 0);
            pnlRoot.Name = "pnlRoot";
            pnlRoot.Size = new Size(310, 36);
            pnlRoot.TabIndex = 0;
            // 
            // pnlAccent
            // 
            pnlAccent.BackColor = Color.FromArgb(214, 28, 52);
            pnlAccent.Dock = DockStyle.Left;
            pnlAccent.Location = new Point(0, 0);
            pnlAccent.Name = "pnlAccent";
            pnlAccent.Size = new Size(4, 34);
            pnlAccent.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(162, 162, 176);
            lblTitle.Location = new Point(12, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(101, 13);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "WINDOW OPACITY";
            // 
            // pnlTrack
            // 
            pnlTrack.BackColor = Color.FromArgb(30, 30, 40);
            pnlTrack.Controls.Add(pnlFill);
            pnlTrack.Cursor = Cursors.Hand;
            pnlTrack.Location = new Point(118, 13);
            pnlTrack.Name = "pnlTrack";
            pnlTrack.Size = new Size(134, 8);
            pnlTrack.TabIndex = 2;
            pnlTrack.MouseDown += Track_MouseDown;
            pnlTrack.MouseMove += Track_MouseMove;
            pnlTrack.MouseUp += Track_MouseUp;
            // 
            // pnlFill
            // 
            pnlFill.BackColor = Color.FromArgb(214, 28, 52);
            pnlFill.Cursor = Cursors.Hand;
            pnlFill.Dock = DockStyle.Left;
            pnlFill.Location = new Point(0, 0);
            pnlFill.Name = "pnlFill";
            pnlFill.Size = new Size(108, 8);
            pnlFill.TabIndex = 0;
            pnlFill.MouseDown += Track_MouseDown;
            pnlFill.MouseMove += Track_MouseMove;
            pnlFill.MouseUp += Track_MouseUp;
            // 
            // lblPercent
            // 
            lblPercent.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblPercent.ForeColor = Color.FromArgb(242, 242, 246);
            lblPercent.Location = new Point(256, 9);
            lblPercent.Name = "lblPercent";
            lblPercent.Size = new Size(44, 16);
            lblPercent.TabIndex = 3;
            lblPercent.Text = "85%";
            lblPercent.TextAlign = ContentAlignment.MiddleRight;
            // 
            // DashboardOpacityOverlay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 9, 12);
            ClientSize = new Size(310, 36);
            Controls.Add(pnlRoot);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DashboardOpacityOverlay";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "Window Opacity";
            TopMost = true;
            pnlRoot.ResumeLayout(false);
            pnlRoot.PerformLayout();
            pnlTrack.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlRoot;
        private Panel pnlAccent;
        private Label lblTitle;
        private Panel pnlTrack;
        private Panel pnlFill;
        private Label lblPercent;
    }
}