#pragma warning disable WFO1000
#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Overcapped_Optimizer.Helpers;

namespace Overcapped_Optimizer.GUI
{
    // Name: Overcapped Optimizer - Obsidian Crimson Theme
    // Target: .NET 8 / .NET 10 WinForms
    // Site: https://voidtechstudios.us/
    // Copyright © 2026 Void Tech Studios

    public static class ROPalette
    {
        public static readonly Color BgDeep = Color.FromArgb(9, 9, 12);
        public static readonly Color SurfaceDark = Color.FromArgb(15, 15, 20);
        public static readonly Color SurfaceCard = Color.FromArgb(22, 22, 29);
        public static readonly Color SurfaceHover = Color.FromArgb(32, 32, 42);
        public static readonly Color SurfaceInput = Color.FromArgb(11, 11, 15);
        public static readonly Color BorderSubtle = Color.FromArgb(42, 42, 54);
        public static readonly Color BorderFocus = Color.FromArgb(225, 29, 56);
        public static readonly Color AccentRust = Color.FromArgb(214, 28, 52);
        public static readonly Color AccentRustBright = Color.FromArgb(248, 54, 78);
        public static readonly Color AccentAmber = Color.FromArgb(142, 18, 42);
        public static readonly Color AccentCrimson = Color.FromArgb(244, 63, 94);
        public static readonly Color HudGreen = Color.FromArgb(16, 185, 129);
        public static readonly Color TextPrimary = Color.FromArgb(242, 242, 246);
        public static readonly Color TextSecondary = Color.FromArgb(162, 162, 176);
        public static readonly Color TextMuted = Color.FromArgb(105, 105, 120);
    }

    public abstract class ThemeModule : ContainerControl
    {
        internal static Graphics? G;
        public static readonly Bitmap TextBitmap = new(1, 1);
        public static readonly Graphics TextGraphics = Graphics.FromImage(TextBitmap);

        public static GraphicsPath GetResponsiveRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int safeRadius = Math.Max(1, Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2)));
            int curveSize = safeRadius * 2;

            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180f, 90f);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270f, 90f);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0f, 90f);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90f, 90f);
            path.CloseFigure();
            return path;
        }

        public static void DrawText(IDeviceContext dc, string? text, Font font, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            TextRenderer.DrawText(dc, text, font, bounds, foreColor, flags | TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, string? text, Font font, Point pt, Color foreColor)
        {
            TextRenderer.DrawText(dc, text, font, new Rectangle(pt.X, pt.Y, 4096, 4096), foreColor, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPrefix);
        }
    }

    public class ColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => ROPalette.BgDeep;
        public override Color MenuStripGradientEnd => ROPalette.BgDeep;
        public override Color ToolStripDropDownBackground => ROPalette.SurfaceCard;
        public override Color ImageMarginGradientBegin => ROPalette.SurfaceCard;
        public override Color ImageMarginGradientMiddle => ROPalette.SurfaceCard;
        public override Color ImageMarginGradientEnd => ROPalette.SurfaceCard;
        public override Color MenuItemSelected => ROPalette.SurfaceHover;
        public override Color MenuItemSelectedGradientBegin => ROPalette.SurfaceHover;
        public override Color MenuItemSelectedGradientEnd => ROPalette.SurfaceHover;
        public override Color MenuItemPressedGradientBegin => ROPalette.SurfaceInput;
        public override Color MenuItemPressedGradientEnd => ROPalette.SurfaceInput;
        public override Color MenuItemPressedGradientMiddle => ROPalette.SurfaceInput;
        public override Color MenuItemBorder => ROPalette.AccentRust;
        public override Color MenuBorder => ROPalette.BorderSubtle;
        public override Color SeparatorDark => ROPalette.BorderSubtle;
        public override Color SeparatorLight => ROPalette.SurfaceDark;
    }

    public class ROMenuRenderer : ToolStripProfessionalRenderer
    {
        public ROMenuRenderer() : base(new ColorTable()) { RoundedEdges = false; }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Color bg = e.ToolStrip is MenuStrip ? ROPalette.BgDeep : ROPalette.SurfaceCard;
            using SolidBrush b = new(bg);
            e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using Pen p = new(ROPalette.BorderSubtle, 1f);
            if (e.ToolStrip is MenuStrip)
                e.Graphics.DrawLine(p, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
            else
                e.Graphics.DrawRectangle(p, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected && !e.Item.Pressed) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new(2, 2, e.Item.Width - 5, e.Item.Height - 5);
            Color fill = e.Item.Pressed ? Color.FromArgb(48, 16, 24) : ROPalette.SurfaceHover;
            Color border = e.Item.Pressed ? ROPalette.AccentRustBright : Color.FromArgb(140, ROPalette.AccentRust);

            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 4);
            using SolidBrush b = new(fill);
            using Pen p = new(border, 1f);
            e.Graphics.FillPath(b, path);
            e.Graphics.DrawPath(p, path);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled
                ? (e.Item.Selected ? Color.White : ROPalette.TextPrimary)
                : ROPalette.TextMuted;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using Pen p = new(ROPalette.BorderSubtle, 1f);
            int y = e.Item.Height / 2;
            e.Graphics.DrawLine(p, 8, y, e.Item.Width - 8, y);
        }
    }

    public class ROLabel : Control
    {
        private string _value1 = "VOID";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Value1 { get => _value1; set { _value1 = value; Invalidate(); } }

        private string _value2 = "TECH";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Value2 { get => _value2; set { _value2 = value; Invalidate(); } }

        public ROLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            Size = new Size(140, 24);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            SizeF sz1 = g.MeasureString(_value1, Font, Width, StringFormat.GenericTypographic);
            float y = Height / 2f - sz1.Height / 2f;
            using SolidBrush pBrush = new(ROPalette.TextPrimary);
            using SolidBrush aBrush = new(ROPalette.AccentRustBright);
            g.DrawString(_value1, Font, pBrush, 0f, y);
            g.DrawString(_value2, Font, aBrush, sz1.Width + 2f, y);
        }
    }

    public class ROGroupBox : ContainerControl
    {
        private string _title = "SECTION";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        private string _subTitle = "";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SubTitle { get => _subTitle; set { _subTitle = value; Invalidate(); } }

        protected override Size DefaultSize => new(280, 140);

        public ROGroupBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            Padding = new Padding(14, 42, 14, 14);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 6 || Height <= 6) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath cardPath = ThemeModule.GetResponsiveRoundedPath(rect, 5);

            using (SolidBrush bg = new(ROPalette.SurfaceDark))
                g.FillPath(bg, cardPath);

            int headerH = 34;
            GraphicsState state = g.Save();
            g.SetClip(cardPath);

            using (LinearGradientBrush hdrBg = new(new Rectangle(0, 0, Width, headerH),
                Color.FromArgb(26, 18, 22), ROPalette.SurfaceDark, 0f))
            using (Pen hdrDivider = new(ROPalette.BorderSubtle, 1f))
            {
                g.FillRectangle(hdrBg, 0, 0, Width, headerH);
                g.DrawLine(hdrDivider, 0, headerH, Width, headerH);
            }

            GraphicsState accentState = g.Save();
            g.SetClip(cardPath);
            using (LinearGradientBrush topAccent = new(new Rectangle(0, 0, Width, 2),
                ROPalette.AccentRustBright, ROPalette.AccentAmber, 0f))
            {
                g.FillRectangle(topAccent, 0, 0, Width, 2);
            }
            g.Restore(accentState);

            using (SolidBrush dot = new(ROPalette.AccentRustBright))
            {
                Point[] diamond = [new(14, 14), new(18, 17), new(14, 20), new(10, 17)];
                g.FillPolygon(dot, diamond);
            }

            using (Font titleFont = new("Segoe UI", 8.5f, FontStyle.Bold))
            {
                ThemeModule.DrawText(g, _title.ToUpperInvariant(), titleFont,
                    new Rectangle(24, 1, Width - 38, headerH), ROPalette.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            if (!string.IsNullOrWhiteSpace(_subTitle))
            {
                using Font subFont = new("Segoe UI", 7.75f, FontStyle.Regular);
                ThemeModule.DrawText(g, _subTitle, subFont,
                    new Rectangle(24, 1, Width - 38, headerH), ROPalette.TextMuted,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            using (Pen borderPen = new(ROPalette.BorderSubtle, 1f))
            g.DrawPath(borderPen, cardPath);
        }
    }

    [DefaultEvent("Click")]
    public class ROSidebarItem : Control
    {
        private bool _isHovered;
        private bool _isSelected;

        [Category("Behavior Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Control? LinkedPanel { get; set; }

        private string _textChar = "◆";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TextCharacter { get => _textChar; set { _textChar = value; Invalidate(); } }

        private bool _isHorizontalTab = true;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsHorizontalTab { get => _isHorizontalTab; set { _isHorizontalTab = value; Invalidate(); } }

        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                if (_isSelected) ActivateLinkedPanel();
                Invalidate();
            }
        }

        public ROSidebarItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 8.75f, FontStyle.Bold);
            Size = new Size(130, 36);
            Cursor = Cursors.Hand;
        }

        private void ActivateLinkedPanel()
        {
            if (Parent != null)
            {
                foreach (Control ctrl in Parent.Controls)
                {
                    if (ctrl is ROSidebarItem other && other != this)
                    {
                        if (other._isSelected) { other._isSelected = false; other.Invalidate(); }
                        if (other.LinkedPanel != null && other.LinkedPanel != LinkedPanel)
                            other.LinkedPanel.Visible = false;
                    }
                }
            }

            if (LinkedPanel != null)
            {
                LinkedPanel.Visible = true;
                LinkedPanel.BringToFront();
            }
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnClick(EventArgs e) { Selected = true; base.OnClick(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 6 || Height <= 6) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            Color bg = _isSelected
                ? Color.FromArgb(40, 15, 22)
                : (_isHovered ? ROPalette.SurfaceCard : Color.Transparent);
            Color border = _isSelected
                ? Color.FromArgb(165, ROPalette.AccentRust)
                : (_isHovered ? ROPalette.BorderSubtle : Color.Transparent);

            if (bg != Color.Transparent)
            {
                using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5);
                using SolidBrush bgBrush = new(bg);
                g.FillPath(bgBrush, path);
                if (border != Color.Transparent)
                {
                    using Pen borderPen = new(border, 1f);
                    g.DrawPath(borderPen, path);
                }
            }

            if (_isSelected)
            {
                if (_isHorizontalTab)
                {
                    int barW = Math.Max(26, (int)(Width * 0.56f));
                    int barX = (Width - barW) / 2;
                    Rectangle barRect = new(barX, Height - 3, barW, 3);
                    using LinearGradientBrush accent = new(barRect, ROPalette.AccentRustBright, ROPalette.AccentRust, 0f);
                    g.FillRectangle(accent, barRect);
                }
                else
                {
                    int barH = (int)(Height * 0.52f);
                    int barY = (Height - barH) / 2;
                    using GraphicsPath pill = ThemeModule.GetResponsiveRoundedPath(new Rectangle(4, barY, 3, barH), 2);
                    using SolidBrush accent = new(ROPalette.AccentRustBright);
                    g.FillPath(accent, pill);
                }
            }

            Color glyphCol = _isSelected ? ROPalette.AccentRustBright : (_isHovered ? ROPalette.TextPrimary : ROPalette.TextMuted);
            Color textCol = _isSelected ? Color.White : (_isHovered ? ROPalette.TextPrimary : ROPalette.TextSecondary);

            if (_isHorizontalTab)
            {
                using Font glyphFont = new("Segoe UI Symbol", 9.25f, FontStyle.Regular);
                Size glyphSz = TextRenderer.MeasureText(g, _textChar, glyphFont, new Size(40, Height), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                Size textSz = TextRenderer.MeasureText(g, Text, Font, new Size(Width - 20, Height), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

                int totalW = glyphSz.Width + 5 + textSz.Width;
                int startX = Math.Max(6, (Width - totalW) / 2);

                ThemeModule.DrawText(g, _textChar, glyphFont, new Rectangle(startX, 0, glyphSz.Width + 2, Height - 2), glyphCol,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, Text, Font, new Rectangle(startX + glyphSz.Width + 5, 0, Width - (startX + glyphSz.Width + 8), Height - 2), textCol,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            else
            {
                using Font glyphFont = new("Segoe UI Symbol", 9.5f, FontStyle.Regular);
                ThemeModule.DrawText(g, _textChar, glyphFont, new Rectangle(14, 0, 22, Height), glyphCol,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, Text, Font, new Rectangle(40, 0, Width - 48, Height), textCol,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }
    }

    [DefaultEvent("Click")]
    public class ROModernButton : Control
    {
        private bool _isPrimaryAction;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsPrimaryAction { get => _isPrimaryAction; set { _isPrimaryAction = value; Invalidate(); } }

        private Color _accentColor = ROPalette.AccentRust;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor { get => _accentColor; set { _accentColor = value; Invalidate(); } }

        private int _cornerRadius = 5;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(2, value); Invalidate(); } }

        private bool _isHovered;
        private bool _isMouseDown;

        public ROModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            Size = new Size(135, 34);
            Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            ForeColor = ROPalette.TextPrimary;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isMouseDown = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _isMouseDown = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isMouseDown = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Cursor = Enabled ? Cursors.Hand : Cursors.Default; Invalidate(); base.OnEnabledChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, _cornerRadius);

            Color textCol;
            if (!Enabled)
            {
                using SolidBrush bg = new(ROPalette.SurfaceDark);
                using Pen pen = new(ROPalette.BorderSubtle, 1f);
                g.FillPath(bg, path);
                g.DrawPath(pen, path);
                textCol = ROPalette.TextMuted;
            }
            else if (_isPrimaryAction)
            {
                Color c1 = _isMouseDown ? Color.FromArgb(156, 16, 36) : (_isHovered ? ROPalette.AccentRustBright : _accentColor);
                Color c2 = _isMouseDown ? Color.FromArgb(104, 10, 26) : (_isHovered ? Color.FromArgb(182, 22, 46) : ROPalette.AccentAmber);
                using LinearGradientBrush grad = new(rect, c1, c2, 18f);
                using Pen border = new(_isHovered ? Color.FromArgb(200, 255, 255, 255) : Color.FromArgb(95, 255, 140, 155), 1f);
                g.FillPath(grad, path);
                g.DrawPath(border, path);
                textCol = Color.White;
            }
            else
            {
                Color fill = _isMouseDown ? ROPalette.SurfaceInput : (_isHovered ? ROPalette.SurfaceHover : ROPalette.SurfaceCard);
                Color border = _isHovered ? ROPalette.AccentRustBright : ROPalette.BorderSubtle;
                using SolidBrush bg = new(fill);
                using Pen pen = new(border, 1f);
                g.FillPath(bg, path);
                g.DrawPath(pen, path);
                textCol = _isHovered ? Color.White : ForeColor;
            }

            int pressY = (_isMouseDown && Enabled) ? 1 : 0;
            ThemeModule.DrawText(g, Text, Font, new Rectangle(8, pressY, Width - 16, Height), textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    [DefaultEvent("Click")]
    public class ROLaunchButton : Control
    {
        private bool _isHovered;
        private bool _isPressed;

        private string _subText = "SELECT A GAME";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string SubText { get => _subText; set { _subText = value; Invalidate(); } }

        public ROLaunchButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            Size = new Size(170, 42);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            Text = "LAUNCH GAME";
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 6 || Height <= 6) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle outer = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(outer, 6);

            Color leftCol = _isPressed ? Color.FromArgb(156, 16, 36) : (_isHovered ? ROPalette.AccentRustBright : ROPalette.AccentRust);
            Color rightCol = _isPressed ? Color.FromArgb(100, 10, 26) : (_isHovered ? Color.FromArgb(178, 22, 46) : ROPalette.AccentAmber);

            using (LinearGradientBrush grad = new(outer, leftCol, rightCol, 12f))
                g.FillPath(grad, path);

            Rectangle inner = new(1, 1, Width - 3, Height - 3);
            using (GraphicsPath innerPath = ThemeModule.GetResponsiveRoundedPath(inner, 5))
            using (Pen sheen = new(Color.FromArgb(_isHovered ? 130 : 65, 255, 200, 210), 1f))
                g.DrawPath(sheen, innerPath);

            int pressY = _isPressed ? 1 : 0;

            int badgeSz = 24;
            int bx = 10;
            int by = (Height - badgeSz) / 2 + pressY;
            using (SolidBrush badgeBg = new(Color.FromArgb(90, 9, 9, 12)))
                g.FillEllipse(badgeBg, bx, by, badgeSz, badgeSz);

            int cy = Height / 2 + pressY;
            Point[] tri = [new Point(bx + 9, cy - 4), new Point(bx + 9, cy + 4), new Point(bx + 17, cy)];
            g.FillPolygon(Brushes.White, tri);

            int textX = bx + badgeSz + 8;
            int textW = Width - textX - 8;

            using Font subFont = new("Segoe UI", 6.85f, FontStyle.Bold);
            int titleH = TextRenderer.MeasureText(Text, Font).Height;
            int subH = TextRenderer.MeasureText(_subText, subFont).Height;
            int topY = (Height - (titleH + subH - 3)) / 2 + pressY;

            ThemeModule.DrawText(g, Text, Font, new Rectangle(textX, topY, textW, titleH), Color.White,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            ThemeModule.DrawText(g, _subText, subFont, new Rectangle(textX, topY + titleH - 3, textW, subH), Color.FromArgb(255, 215, 222),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }

    [DefaultEvent("Click")]
    public class ROActionCard : Control
    {
        private string _title = "Profile Name";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        private string _subtitle = "Description text";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Subtitle { get => _subtitle; set { _subtitle = value; Invalidate(); } }

        private string _badgeGlyph = "⚡";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string BadgeCharacter { get => _badgeGlyph; set { _badgeGlyph = value; Invalidate(); } }

        private bool _selected;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected { get => _selected; set { _selected = value; Invalidate(); } }

        private bool _isHovered;

        public ROActionCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(310, 68);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 6 || Height <= 6) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            Color fill = _selected ? Color.FromArgb(42, 16, 22) : (_isHovered ? ROPalette.SurfaceHover : ROPalette.SurfaceCard);
            Color border = _selected ? ROPalette.AccentRustBright : (_isHovered ? Color.FromArgb(150, ROPalette.AccentRust) : ROPalette.BorderSubtle);

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5))
            {
                using (SolidBrush b = new(fill))
                    g.FillPath(b, path);

                if (_selected || _isHovered)
                {
                    GraphicsState st = g.Save();
                    g.SetClip(path);
                    using SolidBrush leftEdge = new(_selected ? ROPalette.AccentRustBright : ROPalette.AccentRust);
                    g.FillRectangle(leftEdge, 0, 0, 3, Height);
                    g.Restore(st);
                }

                using Pen p = new(border, _selected ? 1.5f : 1f);
                g.DrawPath(p, path);
            }

            int badgeSz = Math.Min(36, Height - 18);
            int pad = (Height - badgeSz) / 2;
            Rectangle badgeRect = new(14, pad, badgeSz, badgeSz);

            using (GraphicsPath bp = ThemeModule.GetResponsiveRoundedPath(badgeRect, 5))
            using (SolidBrush bb = new(_selected ? Color.FromArgb(70, ROPalette.AccentRust) : ROPalette.SurfaceInput))
            using (Pen bPen = new(_selected ? ROPalette.AccentRustBright : ROPalette.BorderSubtle, 1f))
            {
                g.FillPath(bb, bp);
                g.DrawPath(bPen, bp);
            }

            using (Font gf = new("Segoe UI Symbol", 11.5f, FontStyle.Bold))
            {
                ThemeModule.DrawText(g, _badgeGlyph, gf, badgeRect,
                    _selected ? Color.White : ROPalette.AccentRustBright,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            int textX = badgeRect.Right + 12;
            int textW = Width - textX - 12;
            if (textW <= 10) return;

            using Font tf = new("Segoe UI", 9.25f, FontStyle.Bold);
            using Font sf = new("Segoe UI", 8f, FontStyle.Regular);
            ThemeModule.DrawText(g, _title, tf, new Rectangle(textX, pad - 2, textW, 20), ROPalette.TextPrimary, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            ThemeModule.DrawText(g, _subtitle, sf, new Rectangle(textX, pad + 18, textW, Height - pad - 18), ROPalette.TextSecondary, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }
    }

    [DefaultEvent("CheckedChanged")]
    public class ROToggle : Control
    {
        private bool _checked;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set { if (_checked == value) return; _checked = value; CheckedChanged?.Invoke(this, EventArgs.Empty); Invalidate(); }
        }

        public event EventHandler? CheckedChanged;

        public ROToggle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(46, 22);
            Cursor = Cursors.Hand;
        }

        protected override void OnClick(EventArgs e) { Checked = !Checked; base.OnClick(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 4);

            if (_checked)
            {
                using LinearGradientBrush trackGrad = new(rect, ROPalette.AccentRustBright, ROPalette.AccentAmber, 0f);
                using Pen border = new(ROPalette.AccentRustBright, 1f);
                g.FillPath(trackGrad, path);
                g.DrawPath(border, path);
            }
            else
            {
                using SolidBrush trackBg = new(ROPalette.SurfaceInput);
                using Pen border = new(ROPalette.BorderSubtle, 1f);
                g.FillPath(trackBg, path);
                g.DrawPath(border, path);
            }

            int pad = 3;
            int thumbSz = Height - pad * 2;
            int thumbX = _checked ? (Width - thumbSz - pad) : pad;
            Rectangle thumbRect = new(thumbX, pad, thumbSz, thumbSz);

            using (GraphicsPath tp = ThemeModule.GetResponsiveRoundedPath(thumbRect, 3))
            using (SolidBrush tb = new(_checked ? Color.White : ROPalette.TextSecondary))
            {
                g.FillPath(tb, tp);
            }
        }
    }

    public class ROStatPill : Control
    {
        private string _prefix = "CPU:";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Prefix { get => _prefix; set { _prefix = value; Invalidate(); } }

        private Color _dotColor = ROPalette.AccentRust;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DotColor { get => _dotColor; set { _dotColor = value; Invalidate(); } }

        public ROStatPill()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(300, 64);
            Text = "Detecting...";
        }

        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 6 || Height <= 6) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5);

            using (LinearGradientBrush bg = new(rect, Color.FromArgb(20, 20, 27), ROPalette.SurfaceInput, 90f))
                g.FillPath(bg, path);

            // Tall card mode (>= 52px) uses a top accent bar; compact bar mode uses a left vertical bar
            bool cardMode = Height >= 52;
            GraphicsState st = g.Save();
            g.SetClip(path);
            if (cardMode)
            {
                using SolidBrush topBar = new(_dotColor);
                g.FillRectangle(topBar, 0, 0, Width, 3);
            }
            else
            {
                using SolidBrush barGlow = new(Color.FromArgb(40, _dotColor));
                g.FillRectangle(barGlow, 0, 0, 12, Height);
                using SolidBrush bar = new(_dotColor);
                g.FillRectangle(bar, 0, 0, 4, Height);
            }
            g.Restore(st);

            using (Pen border = new(ROPalette.BorderSubtle, 1f))
                g.DrawPath(border, path);

            if (cardMode)
            {
                using Font pf = new("Segoe UI", 8f, FontStyle.Bold);
                using Font vf = new("Segoe UI", 9.25f, FontStyle.Bold);

                ThemeModule.DrawText(g, _prefix.ToUpperInvariant(), pf, new Rectangle(12, 8, Width - 24, 18), _dotColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                ThemeModule.DrawText(g, Text, vf, new Rectangle(12, 28, Width - 24, Height - 34), ROPalette.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            else
            {
                using Font pf = new("Segoe UI", 8.25f, FontStyle.Bold);
                using Font vf = new("Segoe UI", 9f, FontStyle.Regular);

                int textX = 16;
                Size pSize = TextRenderer.MeasureText(_prefix, pf);
                ThemeModule.DrawText(g, _prefix, pf, new Rectangle(textX, 0, pSize.Width + 4, Height), _dotColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                int valX = textX + pSize.Width + 8;
                ThemeModule.DrawText(g, Text, vf, new Rectangle(valX, 0, Math.Max(10, Width - valX - 10), Height), ROPalette.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }
    }

    [DefaultEvent("TextChanged")]
    public class ROTextBox : Control
    {
        public readonly TextBox Base;

        [DllImport("user32.dll")]
        private static extern nint SendMessage(nint hWnd, int wMsg, nint wParam, nint lParam);
        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        protected override Size DefaultSize => new(240, 34);

        public ROTextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;

            Base = new TextBox
            {
                Font = new Font("Segoe UI", 9.25f, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                ForeColor = ROPalette.TextPrimary,
                BackColor = ROPalette.SurfaceInput,
                Location = new Point(10, 8)
            };

            Controls.Add(Base);
            Base.TextChanged += (_, _) => { base.Text = Base.Text; };
            Base.KeyDown += (_, e) => { if (e.Control && e.KeyCode == Keys.A) { Base.SelectAll(); e.SuppressKeyPress = true; } };
            Base.Enter += (_, _) => Invalidate();
            Base.Leave += (_, _) => Invalidate();
            UpdateBaseSize();
        }

        public void ScrollToBottom() { if (Base.IsHandleCreated) SendMessage(Base.Handle, WM_VSCROLL, SB_BOTTOM, nint.Zero); }
        public void AppendText(string text) => Base.AppendText(text);
        public void Clear() => Base.Clear();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HorizontalAlignment TextAlign { get => Base.TextAlign; set => Base.TextAlign = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxLength { get => Base.MaxLength; set => Base.MaxLength = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly { get => Base.ReadOnly; set => Base.ReadOnly = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseSystemPasswordChar { get => Base.UseSystemPasswordChar; set => Base.UseSystemPasswordChar = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Multiline
        {
            get => Base.Multiline;
            set { Base.Multiline = value; Base.WordWrap = value; Base.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None; UpdateBaseSize(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text { get => base.Text; set { base.Text = value; Base.Text = value; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font { get => base.Font; set { base.Font = value; Base.Font = value; UpdateBaseSize(); } }

        private void UpdateBaseSize()
        {
            if (Base == null) return;
            Base.Width = Math.Max(10, Width - 20);
            if (!Multiline)
            {
                Height = Math.Max(32, Base.Height + 14);
                Base.Top = Math.Max(4, (Height - Base.Height) / 2);
            }
            else
            {
                Base.Top = 8;
                Base.Height = Math.Max(10, Height - 16);
            }
        }

        protected override void OnResize(EventArgs e) { UpdateBaseSize(); Invalidate(); base.OnResize(e); }
        protected override void OnMouseDown(MouseEventArgs e) { Base.Focus(); base.OnMouseDown(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 4 || Height <= 4) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5);
            using SolidBrush bg = new(Base.BackColor);
            using Pen border = new(Base.Focused ? ROPalette.BorderFocus : ROPalette.BorderSubtle, Base.Focused ? 1.5f : 1f);
            e.Graphics.FillPath(bg, path);
            e.Graphics.DrawPath(border, path);
        }
    }

    public class ROComboBox : ComboBox
    {
        public ROComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            BackColor = ROPalette.SurfaceCard;
            ForeColor = ROPalette.TextPrimary;
            Font = new Font("Segoe UI", 9.25f, FontStyle.Regular);
            ItemHeight = 26;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5))
            using (SolidBrush bg = new(ROPalette.SurfaceInput))
            using (Pen border = new(Focused ? ROPalette.BorderFocus : ROPalette.BorderSubtle, 1f))
            {
                e.Graphics.FillPath(bg, path);
                e.Graphics.DrawPath(border, path);
            }

            if (SelectedIndex != -1)
            {
                ThemeModule.DrawText(e.Graphics, Text, Font, new Rectangle(10, 0, Math.Max(10, Width - 34), Height),
                    ROPalette.TextPrimary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            using Pen arrowPen = new(ROPalette.AccentRustBright, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            int ax = Width - 18, ay = Height / 2 - 2;
            e.Graphics.DrawLine(arrowPen, ax, ay, ax + 4, ay + 4);
            e.Graphics.DrawLine(arrowPen, ax + 4, ay + 4, ax + 8, ay);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (SolidBrush bg = new(isSelected ? ROPalette.AccentRust : ROPalette.SurfaceCard))
                e.Graphics.FillRectangle(bg, e.Bounds);

            string itemText = Items[e.Index]?.ToString() ?? string.Empty;
            ThemeModule.DrawText(e.Graphics, itemText, Font,
                new Rectangle(e.Bounds.X + 10, e.Bounds.Y, Math.Max(10, e.Bounds.Width - 16), e.Bounds.Height),
                isSelected ? Color.White : ROPalette.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }
    }

    [DefaultEvent("ValueChanged")]
    public class RONumericUpDown : Control
    {
        public event EventHandler? ValueChanged;

        private decimal _minimum = 1;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Minimum { get => _minimum; set { _minimum = value; if (_value < _minimum) Value = _minimum; } }

        private decimal _maximum = 240;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Maximum { get => _maximum; set { _maximum = value; if (_value > _maximum) Value = _maximum; } }

        private decimal _value = 15;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public decimal Value
        {
            get => _value;
            set
            {
                decimal clamped = Math.Clamp(value, _minimum, _maximum);
                if (_value == clamped) return;
                _value = clamped;
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        private Rectangle _minusRect;
        private Rectangle _plusRect;
        private int _hoverBtn;

        public RONumericUpDown()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Size = new Size(108, 32);
            Cursor = Cursors.Hand;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            int bw = 28;
            _minusRect = new Rectangle(2, 2, bw, Height - 4);
            _plusRect = new Rectangle(Width - bw - 2, 2, bw, Height - 4);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = _minusRect.Contains(e.Location) ? -1 : (_plusRect.Contains(e.Location) ? 1 : 0);
            if (h != _hoverBtn) { _hoverBtn = h; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { _hoverBtn = 0; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_minusRect.Contains(e.Location)) Value -= 1;
                else if (_plusRect.Contains(e.Location)) Value += 1;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Value += e.Delta > 0 ? 1 : -1;
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5))
            using (SolidBrush bg = new(ROPalette.SurfaceInput))
            using (Pen border = new(ROPalette.BorderSubtle, 1f))
            {
                g.FillPath(bg, path);
                g.DrawPath(border, path);
            }

            DrawStepBtn(g, _minusRect, "−", _hoverBtn == -1);
            DrawStepBtn(g, _plusRect, "+", _hoverBtn == 1);

            Rectangle valRect = new(_minusRect.Right, 0, _plusRect.Left - _minusRect.Right, Height);
            ThemeModule.DrawText(g, _value.ToString("0"), Font, valRect, ROPalette.TextPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static void DrawStepBtn(Graphics g, Rectangle r, string symbol, bool hover)
        {
            using GraphicsPath p = ThemeModule.GetResponsiveRoundedPath(r, 4);
            using SolidBrush b = new(hover ? ROPalette.AccentRust : ROPalette.SurfaceCard);
            g.FillPath(b, p);
            using Font f = new("Segoe UI", 10f, FontStyle.Bold);
            ThemeModule.DrawText(g, symbol, f, r, hover ? Color.White : ROPalette.TextSecondary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class AboutLinkTile : Control
    {
        private string _title = "GitHub Repository";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        [Category("Behavior Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Url { get; set; } = "";

        private Image? _icon;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? Icon { get => _icon; set { _icon = value; Invalidate(); } }

        private bool _isHovered;

        public AboutLinkTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(220, 52);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!string.IsNullOrWhiteSpace(Url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogError(ex);
                    MessageBox.Show($"Could not open link: {ex.Message}", "Overcapped Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle rect = new(0, 0, Width - 1, Height - 1);
            Color surfaceColor = _isHovered ? ROPalette.SurfaceHover : ROPalette.SurfaceCard;
            Color borderColor = _isHovered ? ROPalette.AccentRustBright : ROPalette.BorderSubtle;

            using (GraphicsPath path = ThemeModule.GetResponsiveRoundedPath(rect, 5))
            using (SolidBrush brush = new(surfaceColor))
            using (Pen pen = new(borderColor, 1f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            int iconBox = (int)(Height * 0.65f);
            int pad = (Height - iconBox) / 2;
            Rectangle iconRect = new(pad, pad, iconBox, iconBox);

            using (GraphicsPath iconBg = ThemeModule.GetResponsiveRoundedPath(iconRect, 4))
            using (SolidBrush iconBrush = new(ROPalette.BgDeep))
            using (Pen iconPen = new(ROPalette.BorderSubtle, 1f))
            {
                g.FillPath(iconBrush, iconBg);
                g.DrawPath(iconPen, iconBg);
            }

            if (_icon != null)
            {
                int imgSize = (int)(iconBox * 0.7f);
                int offset = (iconBox - imgSize) / 2;
                g.DrawImage(_icon, new Rectangle(iconRect.X + offset, iconRect.Y + offset, imgSize, imgSize));
            }

            int textX = iconRect.Right + 12;
            using (Font font = new("Segoe UI", 9.25f, FontStyle.Bold))
            {
                ThemeModule.DrawText(g, _title, font, new Rectangle(textX, 0, Width - textX - 30, Height),
                    ROPalette.TextPrimary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            using (Font arrowFont = new("Segoe UI", 10f, FontStyle.Regular))
            {
                Color arrowColor = _isHovered ? ROPalette.AccentRustBright : ROPalette.TextMuted;
                ThemeModule.DrawText(g, "↗", arrowFont, new Point(Width - 24, (Height - arrowFont.Height) / 2), arrowColor);
            }
        }
    }

    public class ROPatchNotesBox : Control
    {
        private string _title = "PATCH NOTES / CHANGELOG";
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        private Color _accentColor = ROPalette.AccentRustBright;
        [Category("Appearance Overcapped_Optimizer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor { get => _accentColor; set { _accentColor = value; Invalidate(); } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; _scrollOffset = 0; Invalidate(); }
        }

        private int _scrollOffset;
        private int _maxScroll;
        private bool _isDraggingThumb;
        private int _dragStartY;
        private int _dragStartScroll;
        private Rectangle _thumbRect;
        private Rectangle _trackRect;
        private bool _hoverThumb;

        public ROPatchNotesBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, true);

            BackColor = Color.Transparent;
            ForeColor = ROPalette.TextPrimary;
            Font = new Font("Segoe UI", 9.25f, FontStyle.Regular);
            Size = new Size(448, 120);
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); Focus(); }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_maxScroll <= 0) return;
            _scrollOffset = Math.Clamp(_scrollOffset + (e.Delta > 0 ? -24 : 24), 0, _maxScroll);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.Button == MouseButtons.Left && _maxScroll > 0)
            {
                if (_thumbRect.Contains(e.Location))
                {
                    _isDraggingThumb = true;
                    _dragStartY = e.Y;
                    _dragStartScroll = _scrollOffset;
                    Invalidate();
                }
                else if (_trackRect.Contains(e.Location))
                {
                    float ratio = (e.Y - _trackRect.Y) / (float)Math.Max(1, _trackRect.Height);
                    _scrollOffset = Math.Clamp((int)(ratio * _maxScroll), 0, _maxScroll);
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDraggingThumb && _maxScroll > 0)
            {
                int trackScrollable = Math.Max(1, _trackRect.Height - _thumbRect.Height);
                int scrollDelta = (int)Math.Round(((e.Y - _dragStartY) / (double)trackScrollable) * _maxScroll);
                _scrollOffset = Math.Clamp(_dragStartScroll + scrollDelta, 0, _maxScroll);
                Invalidate();
                return;
            }

            bool overThumb = _maxScroll > 0 && _thumbRect.Contains(e.Location);
            if (overThumb != _hoverThumb)
            {
                _hoverThumb = overThumb;
                Cursor = overThumb ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDraggingThumb) { _isDraggingThumb = false; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 10 || Height <= 10) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle outerRect = new(0, 0, Width - 1, Height - 1);
            using GraphicsPath outerPath = ThemeModule.GetResponsiveRoundedPath(outerRect, 5);
            using (SolidBrush bgBrush = new(ROPalette.SurfaceInput))
                g.FillPath(bgBrush, outerPath);

            int headerH = string.IsNullOrWhiteSpace(_title) ? 0 : 26;
            Rectangle viewport = new(2, headerH + 2, Width - 16, Height - headerH - 4);
            if (viewport.Width <= 10 || viewport.Height <= 10) return;

            string rawText = string.IsNullOrWhiteSpace(Text)
                ? "- A new version of Overcapped Optimizer is ready to download."
                : Text;

            string[] lines = rawText.Replace("\r\n", "\n").Split('\n');

            using Font regularFont = new(Font.FontFamily, Font.Size, FontStyle.Regular);
            using Font boldFont = new(Font.FontFamily, Font.Size, FontStyle.Bold);

            int padX = 10;
            int textMaxW = Math.Max(20, Width - 28);
            int totalContentHeight = 6;

            var parsedLines = new List<(string Text, bool IsBullet, bool IsHeader, int Height, int AvailW)>(lines.Length);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    parsedLines.Add((string.Empty, false, false, 8, textMaxW));
                    totalContentHeight += 8;
                    continue;
                }

                bool isBullet = line.StartsWith("- ") || line.StartsWith("* ") || line.StartsWith("• ");
                bool isHeader = !isBullet && (line.StartsWith('#') || line.EndsWith(':') || (line.StartsWith('[') && line.EndsWith(']')));

                string cleanText = isBullet ? line[2..].Trim() : line.TrimStart('#', ' ');
                int availW = isBullet ? Math.Max(20, textMaxW - 16) : textMaxW;
                Font measureFont = isHeader ? boldFont : regularFont;

                SizeF measured = g.MeasureString(cleanText, measureFont, availW);
                int rowH = Math.Max(18, (int)Math.Ceiling(measured.Height) + 2);

                parsedLines.Add((cleanText, isBullet, isHeader, rowH, availW));
                totalContentHeight += rowH + 6;
            }

            totalContentHeight += 6;
            _maxScroll = Math.Max(0, totalContentHeight - viewport.Height);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, _maxScroll);

            GraphicsState state = g.Save();
            g.SetClip(viewport);

            using SolidBrush primaryBrush = new(ROPalette.TextPrimary);
            using SolidBrush secondaryBrush = new(ROPalette.TextSecondary);
            using SolidBrush accentBrush = new(_accentColor);

            int curY = viewport.Y + 6 - _scrollOffset;

            foreach (var item in parsedLines)
            {
                if (string.IsNullOrEmpty(item.Text)) { curY += item.Height; continue; }

                if (curY + item.Height >= viewport.Top && curY <= viewport.Bottom)
                {
                    if (item.IsBullet)
                    {
                        g.FillEllipse(accentBrush, new Rectangle(padX + 2, curY + 6, 5, 5));
                        g.DrawString(item.Text, regularFont, primaryBrush, new RectangleF(padX + 16, curY, item.AvailW, item.Height + 4));
                    }
                    else if (item.IsHeader)
                    {
                        g.DrawString(item.Text, boldFont, accentBrush, new RectangleF(padX, curY, item.AvailW, item.Height + 4));
                    }
                    else
                    {
                        g.DrawString(item.Text, regularFont, secondaryBrush, new RectangleF(padX, curY, item.AvailW, item.Height + 4));
                    }
                }

                curY += item.Height + 6;
            }

            g.Restore(state);

            if (headerH > 0)
            {
                GraphicsState hdrState = g.Save();
                g.SetClip(outerPath);

                Rectangle headerRect = new(0, 0, Width, headerH);
                using (SolidBrush headerBg = new(ROPalette.SurfaceCard))
                using (Pen linePen = new(ROPalette.BorderSubtle, 1f))
                {
                    g.FillRectangle(headerBg, headerRect);
                    g.DrawLine(linePen, 1, headerH, Width - 2, headerH);
                }

                g.FillRectangle(accentBrush, new Rectangle(10, 7, 3, 12));
                using Font headerFont = new("Segoe UI", 7.75f, FontStyle.Bold);
                using StringFormat hdrSf = new() { LineAlignment = StringAlignment.Center };
                g.DrawString(_title, headerFont, secondaryBrush, new RectangleF(18, 1, Width - 24, headerH), hdrSf);

                g.Restore(hdrState);
            }

            using (Pen borderPen = new(ROPalette.BorderSubtle, 1f))
                g.DrawPath(borderPen, outerPath);

            if (_maxScroll > 0)
            {
                _trackRect = new Rectangle(Width - 10, headerH + 6, 5, Height - headerH - 12);
                using (GraphicsPath trackPath = ThemeModule.GetResponsiveRoundedPath(_trackRect, 2))
                using (SolidBrush trackBrush = new(ROPalette.SurfaceCard))
                    g.FillPath(trackBrush, trackPath);

                float viewRatio = (float)viewport.Height / Math.Max(1, totalContentHeight);
                int thumbH = Math.Max(22, (int)(_trackRect.Height * viewRatio));
                int maxThumbTravel = Math.Max(1, _trackRect.Height - thumbH);
                int thumbY = _trackRect.Y + (int)((_scrollOffset / (float)_maxScroll) * maxThumbTravel);

                _thumbRect = new Rectangle(_trackRect.X, thumbY, _trackRect.Width, thumbH);
                Color thumbCol = (_isDraggingThumb || _hoverThumb) ? _accentColor : ROPalette.BorderSubtle;

                using GraphicsPath thumbPath = ThemeModule.GetResponsiveRoundedPath(_thumbRect, 2);
                using SolidBrush thumbBrush = new(thumbCol);
                g.FillPath(thumbBrush, thumbPath);
            }
            else
            {
                _thumbRect = Rectangle.Empty;
                _trackRect = Rectangle.Empty;
            }
        }
    }

    public class ROSeparator : Control
    {
        public ROSeparator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Height = 8;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using Pen p = new(ROPalette.BorderSubtle, 1f);
            e.Graphics.DrawLine(p, 0, Height / 2, Width, Height / 2);
        }
    }

    // Backward-compatible aliases
    public class ROButton : ROModernButton { }
    public class NSModernButton : ROModernButton { }
    public class ROSeperator : ROSeparator { }
    public class ResponsiveToggle : ROToggle { }
    public class ResponsiveStatPill : ROStatPill { }
    public class ResponsiveLaunchButton : ROLaunchButton { }
    public class ResponsiveActionCard : ROActionCard { }
    public class ResponsiveSidebarItem : ROSidebarItem { }
}