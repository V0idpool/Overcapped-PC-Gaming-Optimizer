using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Overcapped_Optimizer.Core;

namespace Overcapped_Optimizer.Helpers
{
    public static class ReportGenerator
    {
        private static readonly Lazy<string> EmbeddedLogoDataUri = new(BuildLogoDataUri);

        public static string GenerateHtmlReport(FpsSessionSummary summary)
        {
            Directory.CreateDirectory(AppSettings.BenchmarksDirectory);

            string safeGame = string.Concat(summary.GameName.Split(Path.GetInvalidFileNameChars())).Trim();
            if (string.IsNullOrWhiteSpace(safeGame)) safeGame = "GameSession";
            safeGame = safeGame.Replace(' ', '_');

            string stamp = summary.EndedAt.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
            string filePath = Path.Combine(AppSettings.BenchmarksDirectory, $"{safeGame}_{stamp}.html");

            string html = BuildHtml(summary);
            File.WriteAllText(filePath, html, Encoding.UTF8);
            return filePath;
        }

        public static string? GetLatestReportPath()
        {
            if (!Directory.Exists(AppSettings.BenchmarksDirectory))
                return null;

            return new DirectoryInfo(AppSettings.BenchmarksDirectory)
                .EnumerateFiles("*.html")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault()?.FullName;
        }

        private static string BuildLogoDataUri()
        {
            try
            {
                using Bitmap? logo = Properties.Resources.OptimizerLogo;
                if (logo == null) return string.Empty;

                using var ms = new MemoryStream();
                logo.Save(ms, ImageFormat.Png);
                return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogError(ex);
                return string.Empty;
            }
        }

        private static string BuildHtml(FpsSessionSummary s)
        {
            string gameEnc = WebUtility.HtmlEncode(s.GameName);
            string exeEnc = WebUtility.HtmlEncode(s.ExecutableName);
            string presetEnc = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(s.PresetName) ? "Default" : s.PresetName);
            string engineEnc = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(s.EngineName) ? "Generic" : s.EngineName);

            string cpuEnc = WebUtility.HtmlEncode($"{HardwareDetector.GetCpuName()} ({HardwareDetector.GetCpuSpeedInGhz()} GHz)");
            string gpuEnc = WebUtility.HtmlEncode($"{HardwareDetector.GetGpuName()} ({HardwareDetector.GetGpuVramInGB():F0} GB VRAM)");
            string ramEnc = WebUtility.HtmlEncode($"{HardwareDetector.GetRamInfo()} ({HardwareDetector.GetTotalMemoryInGB():F0} GB)");

            string durationText = s.Duration.TotalHours >= 1
                ? $"{(int)s.Duration.TotalHours}h {s.Duration.Minutes}m {s.Duration.Seconds}s"
                : $"{s.Duration.Minutes}m {s.Duration.Seconds}s";

            double smoothnessRatio = s.AvgFps > 0 ? Math.Clamp((s.OnePercentLowFps / s.AvgFps) * 100.0, 0, 100) : 0;
            string smoothnessLabel = smoothnessRatio >= 75 ? "Excellent Pacing" : (smoothnessRatio >= 55 ? "Good Pacing" : "Stutter Detected");
            string smoothnessColor = smoothnessRatio >= 75 ? "#10b981" : (smoothnessRatio >= 55 ? "#f59e0b" : "#f43f5e");

            string chartSvg = BuildTimelineSvg(s.Timeline);
            string activeOptimizationsHtml = BuildActiveOptimizationBadges();
            string logoDataUri = EmbeddedLogoDataUri.Value;
            string logoImgHtml = !string.IsNullOrEmpty(logoDataUri)
                ? $"<img src=\"{logoDataUri}\" alt=\"Overcapped Logo\" class=\"nav-logo\">"
                : string.Empty;

            var inv = CultureInfo.InvariantCulture;

            var sb = new StringBuilder(16384);
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\">");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            sb.AppendLine($"<title>Overcapped Benchmark - {gameEnc}</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(@"
                :root {
                    --bg: #09090c;
                    --surface: #121218;
                    --card: #16161d;
                    --border: #262633;
                    --text: #f2f2f6;
                    --muted: #9a9ab0;
                    --crimson: #d61c34;
                    --crimson-bright: #f8364e;
                    --green: #10b981;
                    --amber: #f59e0b;
                }
                * { box-sizing: border-box; margin: 0; padding: 0; }
                body {
                    background: var(--bg);
                    color: var(--text);
                    font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
                    line-height: 1.5;
                    padding-bottom: 48px;
                }
                .site-header {
                    position: sticky;
                    top: 0;
                    z-index: 50;
                    width: 100%;
                    background: rgba(9, 9, 12, 0.92);
                    backdrop-filter: blur(10px);
                    border-bottom: 1px solid var(--border);
                    margin-bottom: 24px;
                }
                .header-accent-bar {
                    width: 100%;
                    height: 2px;
                    background: linear-gradient(90deg, var(--crimson), var(--crimson-bright), transparent);
                }
                .site-nav {
                    max-width: 1040px;
                    margin: 0 auto;
                    padding: 0 20px;
                    height: 64px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    gap: 16px;
                }
                .nav-left {
                    display: flex;
                    align-items: center;
                    gap: 18px;
                }
                .brand-lockup {
                    display: flex;
                    align-items: center;
                    gap: 12px;
                    text-decoration: none;
                }
                .nav-logo {
                    width: 36px;
                    height: 36px;
                    object-fit: contain;
                    border-radius: 6px;
                    display: block;
                }
                .brand-text {
                    display: flex;
                    flex-direction: column;
                    line-height: 1;
                }
                .brand-title {
                    font-size: 20px;
                    font-weight: 800;
                    letter-spacing: -0.025em;
                }
                .brand-title .over { color: #ffffff; }
                .brand-title .capped { color: var(--crimson-bright); }
                .brand-sub {
                    font-size: 9px;
                    font-weight: 700;
                    letter-spacing: 0.22em;
                    text-transform: uppercase;
                    color: var(--muted);
                    margin-top: 3px;
                }
                .nav-divider {
                    height: 24px;
                    width: 1px;
                    background: var(--border);
                }
                .nav-tag {
                    font-size: 12px;
                    font-weight: 700;
                    letter-spacing: 0.08em;
                    text-transform: uppercase;
                    color: var(--text);
                }
                .nav-date {
                    font-size: 12px;
                    color: var(--muted);
                    font-weight: 600;
                }
                .wrap { max-width: 1040px; margin: 0 auto; padding: 0 20px; }
                .header {
                    background: linear-gradient(135deg, #16161d 0%, #1d1218 100%);
                    border: 1px solid var(--border);
                    border-left: 4px solid var(--crimson);
                    border-radius: 10px;
                    padding: 22px 26px;
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    flex-wrap: wrap;
                    gap: 16px;
                    margin-bottom: 20px;
                }
                .brand { font-size: 11px; letter-spacing: 1.8px; text-transform: uppercase; color: var(--crimson-bright); font-weight: 700; }
                h1 { font-size: 26px; font-weight: 700; margin-top: 2px; }
                .meta { color: var(--muted); font-size: 13px; margin-top: 4px; }
                .pill {
                    display: inline-block;
                    padding: 5px 12px;
                    border-radius: 999px;
                    font-size: 12px;
                    font-weight: 700;
                    border: 1px solid var(--border);
                    background: rgba(255,255,255,0.03);
                }
                .grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
                    gap: 14px;
                    margin-bottom: 20px;
                }
                .kpi {
                    background: var(--card);
                    border: 1px solid var(--border);
                    border-radius: 10px;
                    padding: 16px 18px;
                }
                .kpi-label { font-size: 11px; text-transform: uppercase; letter-spacing: 0.9px; color: var(--muted); font-weight: 600; }
                .kpi-value { font-size: 28px; font-weight: 800; margin-top: 6px; }
                .kpi-sub { font-size: 12px; color: var(--muted); margin-top: 2px; }
                .section {
                    background: var(--card);
                    border: 1px solid var(--border);
                    border-radius: 10px;
                    padding: 20px 22px;
                    margin-bottom: 20px;
                }
                .section-title {
                    font-size: 14px;
                    font-weight: 700;
                    text-transform: uppercase;
                    letter-spacing: 0.8px;
                    margin-bottom: 14px;
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                }
                .legend { display: flex; gap: 16px; font-size: 12px; color: var(--muted); font-weight: 500; text-transform: none; letter-spacing: 0; }
                .dot { display: inline-block; width: 10px; height: 10px; border-radius: 50%; margin-right: 6px; }
                .bar-row { display: grid; grid-template-columns: 185px 1fr 66px; align-items: center; gap: 12px; margin-bottom: 10px; font-size: 13px; }
                .bar-track { height: 10px; background: #0e0e14; border-radius: 999px; overflow: hidden; border: 1px solid #232330; }
                .bar-fill { height: 100%; border-radius: 999px; }
                .two-col { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 18px; }
                .spec-row { padding: 8px 0; border-bottom: 1px solid #20202c; font-size: 13px; }
                .spec-row:last-child { border-bottom: none; }
                .spec-key { color: var(--muted); font-weight: 600; display: inline-block; width: 70px; }
                .badges { display: flex; flex-wrap: wrap; gap: 8px; }
                .badge-on {
                    background: rgba(16, 185, 129, 0.12);
                    border: 1px solid rgba(16, 185, 129, 0.35);
                    color: #6ee7b7;
                    padding: 5px 10px;
                    border-radius: 6px;
                    font-size: 12px;
                    font-weight: 600;
                }
                .badge-off {
                    background: rgba(255, 255, 255, 0.03);
                    border: 1px solid var(--border);
                    color: var(--muted);
                    padding: 5px 10px;
                    border-radius: 6px;
                    font-size: 12px;
                }
                .footer { text-align: center; color: var(--muted); font-size: 12px; margin-top: 24px; }
                svg { width: 100%; height: auto; display: block; }
                @media (max-width: 640px) {
                    .nav-divider, .nav-tag { display: none; }
                }
            ");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<header class=\"site-header\">");
            sb.AppendLine("  <div class=\"header-accent-bar\"></div>");
            sb.AppendLine("  <nav class=\"site-nav\">");
            sb.AppendLine("    <div class=\"nav-left\">");
            sb.AppendLine("      <div class=\"brand-lockup\">");
            if (!string.IsNullOrEmpty(logoImgHtml))
            sb.AppendLine($"        {logoImgHtml}");
            sb.AppendLine("        <div class=\"brand-text\">");
            sb.AppendLine("          <span class=\"brand-title\"><span class=\"over\">OVER</span><span class=\"capped\">CAPPED</span></span>");
            sb.AppendLine("          <span class=\"brand-sub\">PC Gaming Optimizer</span>");
            sb.AppendLine("        </div>");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <div class=\"nav-divider\"></div>");
            sb.AppendLine("      <span class=\"nav-tag\">Session Benchmark Report</span>");
            sb.AppendLine("    </div>");
            sb.AppendLine($"    <div class=\"nav-date\">{s.EndedAt:MM-dd-yyyy HH:mm}</div>");
            sb.AppendLine("  </nav>");
            sb.AppendLine("</header>");
            sb.AppendLine("<div class=\"wrap\">");
            sb.AppendLine("<div class=\"header\">");
            sb.AppendLine("  <div>");
            sb.AppendLine("    <div class=\"brand\">Zero-Injection ETW Telemetry</div>");
            sb.AppendLine($"    <h1>{gameEnc}</h1>");
            sb.AppendLine($"    <div class=\"meta\">{exeEnc} &bull; Engine: {engineEnc} &bull; Preset: {presetEnc}</div>");
            sb.AppendLine($"    <div class=\"meta\">{s.StartedAt:yyyy-MM-dd HH:mm} &rarr; {s.EndedAt:HH:mm:ss} &bull; Duration: {durationText} &bull; {s.TotalFrames:N0} frames captured</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine($"  <div class=\"pill\" style=\"color:{smoothnessColor};border-color:{smoothnessColor}55;\">{smoothnessLabel} ({smoothnessRatio:F0}% 1% Low Ratio)</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"grid\">");
            AppendKpi(sb, "Average FPS", s.AvgFps.ToString("F1", inv), $"Median: {s.MedianFps.ToString("F1", inv)} FPS", "#10b981");
            AppendKpi(sb, "1% Low FPS", s.OnePercentLowFps.ToString("F1", inv), $"99th pct: {s.P99FrameTimeMs.ToString("F1", inv)} ms", s.OnePercentLowFps >= 60 ? "#10b981" : (s.OnePercentLowFps >= 30 ? "#f59e0b" : "#f43f5e"));
            AppendKpi(sb, "0.1% Low FPS", s.PointOnePercentLowFps.ToString("F1", inv), $"Worst frame: {s.MaxFrameTimeMs.ToString("F1", inv)} ms", s.PointOnePercentLowFps >= 45 ? "#10b981" : "#f59e0b");
            AppendKpi(sb, "Avg Frametime", $"{s.AvgFrameTimeMs.ToString("F2", inv)} ms", $"Peak: {s.MaxFps.ToString("F0", inv)} FPS", "#f2f2f6");
            AppendKpi(sb, "Stutter Spikes", s.StutterFrames33Ms.ToString("N0", inv), $">50 ms hitches: {s.SevereStutterFrames50Ms:N0}", s.StutterFrames33Ms <= 5 ? "#10b981" : "#f8364e");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"section\">");
            sb.AppendLine("  <div class=\"section-title\">");
            sb.AppendLine("    <span>FPS &amp; 1% Low Timeline</span>");
            sb.AppendLine("    <div class=\"legend\"><span><span class=\"dot\" style=\"background:#10b981;\"></span>Avg FPS</span><span><span class=\"dot\" style=\"background:#f8364e;\"></span>1% Low FPS</span></div>");
            sb.AppendLine("  </div>");
            sb.AppendLine(chartSvg);
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"section\">");
            sb.AppendLine("  <div class=\"section-title\"><span>Frametime Distribution</span></div>");
            AppendDistributionBar(sb, "&lt; 8.3 ms (120+ FPS)", s.PctUnder8_3Ms, "#10b981");
            AppendDistributionBar(sb, "8.3 &ndash; 16.7 ms (60&ndash;120 FPS)", s.Pct8_3To16_7Ms, "#38bdf8");
            AppendDistributionBar(sb, "16.7 &ndash; 33.3 ms (30&ndash;60 FPS)", s.Pct16_7To33_3Ms, "#f59e0b");
            AppendDistributionBar(sb, "&gt; 33.3 ms (&lt; 30 FPS / Stutter)", s.PctOver33_3Ms, "#f43f5e");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"two-col\">");
            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine("    <div class=\"section-title\"><span>Hardware Snapshot</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">CPU</span> {cpuEnc}</div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">GPU</span> {gpuEnc}</div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">RAM</span> {ramEnc}</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine("    <div class=\"section-title\"><span>Active Overcapped Optimizations</span></div>");
            sb.AppendLine($"    <div class=\"badges\">{activeOptimizationsHtml}</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"footer\">Generated automatically by Overcapped Optimizer &bull; Void Tech Studios</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static void AppendKpi(StringBuilder sb, string label, string value, string sub, string color)
        {
            sb.AppendLine("<div class=\"kpi\">");
            sb.AppendLine($"  <div class=\"kpi-label\">{label}</div>");
            sb.AppendLine($"  <div class=\"kpi-value\" style=\"color:{color};\">{value}</div>");
            sb.AppendLine($"  <div class=\"kpi-sub\">{sub}</div>");
            sb.AppendLine("</div>");
        }

        private static void AppendDistributionBar(StringBuilder sb, string label, double pct, string color)
        {
            string widthStr = Math.Clamp(pct, 0, 100).ToString("F1", CultureInfo.InvariantCulture);
            sb.AppendLine("<div class=\"bar-row\">");
            sb.AppendLine($"  <div>{label}</div>");
            sb.AppendLine($"  <div class=\"bar-track\"><div class=\"bar-fill\" style=\"width:{widthStr}%;background:{color};\"></div></div>");
            sb.AppendLine($"  <div style=\"text-align:right;font-weight:700;\">{pct.ToString("F1", CultureInfo.InvariantCulture)}%</div>");
            sb.AppendLine("</div>");
        }

        private static string BuildActiveOptimizationBadges()
        {
            var c = AppSettings.Current;
            var items = new (string Label, bool Enabled)[]
            {
                ("Physical Cores Only", c.PCoresOnly),
                ("Auto Power Plan", c.AutoPowerPlan),
                ("Game Mode / DVR Off", c.GameModeAndDisableDvr),
                ("FSO Disabled", c.DisableFSO),
                ("Background App Throttle", c.ThrottleBackgroundApps),
                ("Delivery Opt P2P Off", c.DisableDeliveryOptimization),
                ("NIC Power Saving Off", c.DisableNicPowerSaving),
                ("Auto Standby RAM Flush", c.AutoFlushEnabled)
            };

            var sb = new StringBuilder();
            foreach (var (label, enabled) in items)
            {
                string cls = enabled ? "badge-on" : "badge-off";
                string prefix = enabled ? "&#10003; " : "&times; ";
                sb.AppendLine($"<span class=\"{cls}\">{prefix}{WebUtility.HtmlEncode(label)}</span>");
            }
            return sb.ToString();
        }

        private static string BuildTimelineSvg(List<FpsTimelinePoint> points)
        {
            if (points.Count < 2)
                return "<div style=\"color:#9a9ab0;font-size:13px;padding:18px 0;\">Session was too short to plot a multi-second FPS timeline.</div>";

            const double width = 960;
            const double height = 220;
            const double padLeft = 46;
            const double padRight = 14;
            const double padTop = 14;
            const double padBottom = 26;
            double plotW = width - padLeft - padRight;
            double plotH = height - padTop - padBottom;

            double maxFps = Math.Max(60.0, points.Max(p => p.AvgFps) * 1.1);
            maxFps = Math.Ceiling(maxFps / 30.0) * 30.0;

            var inv = CultureInfo.InvariantCulture;
            var avgPts = new StringBuilder();
            var lowPts = new StringBuilder();

            for (int i = 0; i < points.Count; i++)
            {
                double x = padLeft + (plotW * i / (points.Count - 1));
                double yAvg = padTop + plotH - (Math.Clamp(points[i].AvgFps, 0, maxFps) / maxFps) * plotH;
                double yLow = padTop + plotH - (Math.Clamp(points[i].OnePercentLowFps, 0, maxFps) / maxFps) * plotH;

                avgPts.Append($"{x.ToString("F1", inv)},{yAvg.ToString("F1", inv)} ");
                lowPts.Append($"{x.ToString("F1", inv)},{yLow.ToString("F1", inv)} ");
            }

            var svg = new StringBuilder();
            svg.AppendLine($"<svg viewBox=\"0 0 {width.ToString("F0", inv)} {height.ToString("F0", inv)}\" xmlns=\"http://www.w3.org/2000/svg\">");
            svg.AppendLine($"  <rect x=\"{padLeft.ToString("F0", inv)}\" y=\"{padTop.ToString("F0", inv)}\" width=\"{plotW.ToString("F0", inv)}\" height=\"{plotH.ToString("F0", inv)}\" fill=\"#0e0e14\" rx=\"4\" />");

            for (int step = 0; step <= 4; step++)
            {
                double val = maxFps * step / 4.0;
                double y = padTop + plotH - (plotH * step / 4.0);
                svg.AppendLine($"  <line x1=\"{padLeft.ToString("F0", inv)}\" y1=\"{y.ToString("F1", inv)}\" x2=\"{(padLeft + plotW).ToString("F0", inv)}\" y2=\"{y.ToString("F1", inv)}\" stroke=\"#232330\" stroke-width=\"1\" />");
                svg.AppendLine($"  <text x=\"{(padLeft - 8).ToString("F0", inv)}\" y=\"{(y + 4).ToString("F1", inv)}\" fill=\"#9a9ab0\" font-size=\"11\" text-anchor=\"end\">{val.ToString("F0", inv)}</text>");
            }

            svg.AppendLine($"  <polyline fill=\"none\" stroke=\"#10b981\" stroke-width=\"2.2\" stroke-linejoin=\"round\" points=\"{avgPts}\" />");
            svg.AppendLine($"  <polyline fill=\"none\" stroke=\"#f8364e\" stroke-width=\"1.8\" stroke-linejoin=\"round\" stroke-dasharray=\"5,3\" points=\"{lowPts}\" />");

            int totalSec = points[^1].ElapsedSeconds;
            svg.AppendLine($"  <text x=\"{padLeft.ToString("F0", inv)}\" y=\"{(height - 6).ToString("F0", inv)}\" fill=\"#9a9ab0\" font-size=\"11\">0:00</text>");
            svg.AppendLine($"  <text x=\"{(padLeft + plotW / 2).ToString("F1", inv)}\" y=\"{(height - 6).ToString("F0", inv)}\" fill=\"#9a9ab0\" font-size=\"11\" text-anchor=\"middle\">{FormatMmSs(totalSec / 2)}</text>");
            svg.AppendLine($"  <text x=\"{(padLeft + plotW).ToString("F0", inv)}\" y=\"{(height - 6).ToString("F0", inv)}\" fill=\"#9a9ab0\" font-size=\"11\" text-anchor=\"end\">{FormatMmSs(totalSec)}</text>");
            svg.AppendLine("</svg>");
            return svg.ToString();
        }

        private static string FormatMmSs(int totalSeconds)
        {
            int mins = totalSeconds / 60;
            int secs = totalSeconds % 60;
            return $"{mins}:{secs:D2}";
        }
    }
}