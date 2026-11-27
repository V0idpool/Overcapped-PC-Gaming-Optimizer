using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using Overcapped_Optimizer.Core;

namespace Overcapped_Optimizer.Helpers
{
    public static class ReportGenerator
    {
        private static readonly Lazy<string> EmbeddedLogoDataUri = new(BuildLogoDataUri);

        private static readonly JsonSerializerOptions ReportJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

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
                using Bitmap? source = Properties.Resources.OptimizerLogo;
                if (source == null) return string.Empty;

                const int targetSize = 72;
                using var resized = new Bitmap(targetSize, targetSize, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(resized))
                {
                    g.Clear(Color.Transparent);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    float scale = Math.Min((float)targetSize / source.Width, (float)targetSize / source.Height);
                    int drawW = Math.Max(1, (int)Math.Round(source.Width * scale));
                    int drawH = Math.Max(1, (int)Math.Round(source.Height * scale));
                    int offsetX = (targetSize - drawW) / 2;
                    int offsetY = (targetSize - drawH) / 2;

                    g.DrawImage(source, offsetX, offsetY, drawW, drawH);
                }

                using var ms = new MemoryStream();
                resized.Save(ms, ImageFormat.Png);
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
            var inv = CultureInfo.InvariantCulture;

            string presetRaw = string.IsNullOrWhiteSpace(s.PresetName) ? "Default" : s.PresetName;
            string engineRaw = string.IsNullOrWhiteSpace(s.EngineName) ? "Generic" : s.EngineName;

            string cpuRaw = $"{HardwareDetector.GetCpuName()} ({HardwareDetector.GetCpuSpeedInGhz()} GHz)";
            string gpuRaw = $"{HardwareDetector.GetGpuName()} ({HardwareDetector.GetGpuVramInGB():F0} GB VRAM)";
            string driverRaw = HardwareDetector.GetGpuDriverVersion();
            string ramRaw = $"{HardwareDetector.GetRamInfo()} ({HardwareDetector.GetTotalMemoryInGB():F0} GB)";
            string osRaw = HardwareDetector.GetWindowsVersionString();
            string displayRaw = s.DisplayWidth > 0 && s.DisplayHeight > 0 && s.DisplayRefreshHz > 0
                ? $"{s.DisplayWidth}x{s.DisplayHeight} @ {s.DisplayRefreshHz} Hz"
                : "Not Detected";

            List<HardwareHealthWarning> healthWarnings;
            try { healthWarnings = HardwareDetector.RunHardwareHealthChecks(); }
            catch { healthWarnings = new List<HardwareHealthWarning>(); }

            string gameEnc = WebUtility.HtmlEncode(s.GameName);
            string exeEnc = WebUtility.HtmlEncode(s.ExecutableName);
            string presetEnc = WebUtility.HtmlEncode(presetRaw);
            string engineEnc = WebUtility.HtmlEncode(engineRaw);
            string cpuEnc = WebUtility.HtmlEncode(cpuRaw);
            string gpuEnc = WebUtility.HtmlEncode(gpuRaw);
            string driverEnc = WebUtility.HtmlEncode(driverRaw);
            string ramEnc = WebUtility.HtmlEncode(ramRaw);
            string osEnc = WebUtility.HtmlEncode(osRaw);
            string displayEnc = WebUtility.HtmlEncode(displayRaw);

            string durationText = s.Duration.TotalHours >= 1
                ? $"{(int)s.Duration.TotalHours}h {s.Duration.Minutes}m {s.Duration.Seconds}s"
                : $"{s.Duration.Minutes}m {s.Duration.Seconds}s";

            double smoothnessRatio = s.AvgFps > 0 ? Math.Clamp((s.OnePercentLowFps / s.AvgFps) * 100.0, 0, 100) : 0;
            string smoothnessLabel = smoothnessRatio >= 75 ? "Excellent Pacing" : (smoothnessRatio >= 55 ? "Good Pacing" : "Stutter Detected");
            string smoothnessColor = smoothnessRatio >= 75 ? "#10b981" : (smoothnessRatio >= 55 ? "#f59e0b" : "#f43f5e");

            string activeOptimizationsHtml = BuildActiveOptimizationBadges(s);
            string healthHtml = BuildHealthWarningsHtml(healthWarnings);
            string diagnosticsHtml = BuildDiagnosticNotesHtml(s, smoothnessRatio, healthWarnings);

            string logoDataUri = EmbeddedLogoDataUri.Value;
            string logoImgHtml = !string.IsNullOrEmpty(logoDataUri)
                ? $"<img src=\"{logoDataUri}\" alt=\"Overcapped Logo\" class=\"nav-logo\">"
                : string.Empty;

            var payload = new
            {
                schemaVersion = 2,
                gameName = s.GameName,
                executableName = s.ExecutableName,
                presetName = presetRaw,
                engineName = engineRaw,
                startedAt = s.StartedAt.ToString("yyyy-MM-dd HH:mm:ss", inv),
                endedAt = s.EndedAt.ToString("yyyy-MM-dd HH:mm:ss", inv),
                durationSeconds = Math.Round(s.Duration.TotalSeconds, 1),
                durationText,
                totalFrames = s.TotalFrames,
                avgFps = Math.Round(s.AvgFps, 2),
                medianFps = Math.Round(s.MedianFps, 2),
                onePercentLowFps = Math.Round(s.OnePercentLowFps, 2),
                pointOnePercentLowFps = Math.Round(s.PointOnePercentLowFps, 2),
                minFps = Math.Round(s.MinFps, 2),
                maxFps = Math.Round(s.MaxFps, 2),
                avgFrameTimeMs = Math.Round(s.AvgFrameTimeMs, 2),
                frameTimeStdDevMs = Math.Round(s.FrameTimeStdDevMs, 2),
                p95FrameTimeMs = Math.Round(s.P95FrameTimeMs, 2),
                p99FrameTimeMs = Math.Round(s.P99FrameTimeMs, 2),
                p99_9FrameTimeMs = Math.Round(s.P99_9FrameTimeMs, 2),
                maxFrameTimeMs = Math.Round(s.MaxFrameTimeMs, 2),
                stutterFrames33Ms = s.StutterFrames33Ms,
                severeStutterFrames50Ms = s.SevereStutterFrames50Ms,
                smoothnessRatio = Math.Round(smoothnessRatio, 1),
                worstDipSec = s.WorstDipElapsedSeconds,
                worstDipFps = Math.Round(s.WorstDipFps, 1),
                displayText = displayRaw,
                displayHz = s.DisplayRefreshHz,
                pctAtOrAboveRefresh = Math.Round(s.PctAtOrAboveRefresh, 1),
                totalRamGb = Math.Round(s.TotalRamGb, 1),
                minFreeRamGb = Math.Round(s.MinFreeRamGb, 2),
                peakUsedRamGb = Math.Round(s.PeakUsedRamGb, 2),
                ramFlushCount = s.RamFlushCount,
                ramFlushSeconds = s.RamFlushSeconds ?? new List<int>(),
                hardware = new
                {
                    cpu = cpuRaw,
                    gpu = gpuRaw,
                    driver = driverRaw,
                    ram = ramRaw,
                    os = osRaw,
                    display = displayRaw
                },
                timeline = s.Timeline.Select(p => new
                {
                    t = p.ElapsedSeconds,
                    avg = Math.Round(p.AvgFps, 1),
                    low = Math.Round(p.OnePercentLowFps, 1)
                }).ToList()
            };

            string safeJson = JsonSerializer.Serialize(payload, ReportJsonOptions).Replace("</", "<\\/");

            string worstDipText = s.WorstDipElapsedSeconds >= 0 && s.WorstDipFps > 0
                ? $"{FormatMmSs(s.WorstDipElapsedSeconds)} ({s.WorstDipFps.ToString("F1", inv)} FPS)"
                : "N/A";

            string ramPeakText = s.PeakUsedRamGb > 0
                ? $"{s.PeakUsedRamGb.ToString("F1", inv)} / {s.TotalRamGb.ToString("F0", inv)} GB"
                : "Not Sampled";

            string ramMinFreeText = s.MinFreeRamGb > 0
                ? $"{s.MinFreeRamGb.ToString("F1", inv)} GB"
                : "Not Sampled";

            string flushSummaryText = s.RamFlushCount > 0
                ? $"{s.RamFlushCount} flush{(s.RamFlushCount == 1 ? "" : "es")} ({string.Join(", ", s.RamFlushSeconds.Take(5).Select(FormatMmSs))}{(s.RamFlushSeconds.Count > 5 ? "..." : "")})"
                : "0 flushes";

            string refreshTargetText = s.DisplayRefreshHz > 0
                ? $"{s.PctAtOrAboveRefresh.ToString("F1", inv)}% at {s.DisplayRefreshHz} Hz+"
                : "Display Hz N/A";

            string refreshColor = s.DisplayRefreshHz <= 0
                ? "#9a9ab0"
                : (s.PctAtOrAboveRefresh >= 80 ? "#10b981" : (s.PctAtOrAboveRefresh >= 50 ? "#f59e0b" : "#f43f5e"));

            var sb = new StringBuilder(32768);
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\">");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            sb.AppendLine($"<title>Overcapped Benchmark - {gameEnc}</title>");
            sb.AppendLine("<meta property=\"og:image\" content=\"https://overcapped.us/img/logo.png\" />");
            sb.AppendLine("<meta property=\"og:image:type\" content=\"image/png\" />");
            if (!string.IsNullOrEmpty(logoDataUri))
            {
                sb.AppendLine($"<link rel=\"icon\" type=\"image/png\" href=\"{logoDataUri}\" />");
            }
            sb.AppendLine("<style>");
            sb.AppendLine(ReportCss);
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div id=\"drop-overlay\" class=\"drop-overlay\">");
            sb.AppendLine("  <div class=\"drop-box\">");
            sb.AppendLine("    <div class=\"drop-title\">Drop Benchmark Report to Compare</div>");
            sb.AppendLine("    <div class=\"drop-sub\">Supports any Overcapped .html or exported .json report</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<input type=\"file\" id=\"compare-file-input\" accept=\".html,.htm,.json\" style=\"display:none\" />");
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
            sb.AppendLine("      <span class=\"nav-tag\">Session Telemetry</span>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"nav-actions no-print\">");
            sb.AppendLine("      <button type=\"button\" id=\"btn-compare\" class=\"btn btn-accent\" title=\"Drop or pick another report to compare side-by-side\">Compare Report...</button>");
            sb.AppendLine("      <button type=\"button\" id=\"btn-copy-discord\" class=\"btn\" title=\"Copy formatted summary for Discord or Reddit\">Copy Discord</button>");
            sb.AppendLine("      <button type=\"button\" id=\"btn-export-csv\" class=\"btn\" title=\"Download per-second timeline as CSV\">CSV</button>");
            sb.AppendLine("      <button type=\"button\" id=\"btn-export-json\" class=\"btn\" title=\"Download raw report telemetry as JSON\">JSON</button>");
            sb.AppendLine("      <button type=\"button\" id=\"btn-print\" class=\"btn\" title=\"Print or Save as PDF\">Print</button>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </nav>");
            sb.AppendLine("</header>");
            sb.AppendLine("<div class=\"wrap\">");
            sb.AppendLine("<div id=\"compare-banner\" class=\"compare-banner\" style=\"display:none;\">");
            sb.AppendLine("  <div class=\"compare-top\">");
            sb.AppendLine("    <div>");
            sb.AppendLine("      <div class=\"compare-eyebrow\">BEFORE / AFTER COMPARISON ACTIVE</div>");
            sb.AppendLine("      <div id=\"compare-label\" class=\"compare-label\"></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"compare-btns no-print\">");
            sb.AppendLine("      <button type=\"button\" id=\"btn-swap-compare\" class=\"btn\">Swap A / B</button>");
            sb.AppendLine("      <button type=\"button\" id=\"btn-clear-compare\" class=\"btn\">Clear</button>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div id=\"compare-table-wrap\" class=\"compare-table-wrap\"></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"header\">");
            sb.AppendLine("  <div>");
            sb.AppendLine("    <div class=\"brand\">Zero-Injection ETW Telemetry</div>");
            sb.AppendLine($"    <h1>{gameEnc}</h1>");
            sb.AppendLine($"    <div class=\"meta\">{exeEnc} &bull; Engine: {engineEnc} &bull; Preset: {presetEnc} &bull; Display: {displayEnc}</div>");
            sb.AppendLine($"    <div class=\"meta\">{s.StartedAt:yyyy-MM-dd HH:mm} &rarr; {s.EndedAt:HH:mm:ss} &bull; Active Duration: {durationText} &bull; {s.TotalFrames:N0} frames captured</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div class=\"header-pills\">");
            sb.AppendLine($"    <div class=\"pill\" style=\"color:{smoothnessColor};border-color:{smoothnessColor}55;\">{smoothnessLabel} ({smoothnessRatio:F0}% 1% Low Ratio)</div>");
            sb.AppendLine($"    <div class=\"pill\" style=\"color:{refreshColor};border-color:{refreshColor}55;\">{WebUtility.HtmlEncode(refreshTargetText)}</div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"grid\">");
            AppendKpi(sb, "kpi-avg", "Average FPS", s.AvgFps.ToString("F1", inv), $"Median: {s.MedianFps.ToString("F1", inv)} FPS", "#10b981");
            AppendKpi(sb, "kpi-1low", "1% Low FPS", s.OnePercentLowFps.ToString("F1", inv), $"99th pct: {s.P99FrameTimeMs.ToString("F1", inv)} ms", s.OnePercentLowFps >= 60 ? "#10b981" : (s.OnePercentLowFps >= 30 ? "#f59e0b" : "#f43f5e"));
            AppendKpi(sb, "kpi-01low", "0.1% Low FPS", s.PointOnePercentLowFps.ToString("F1", inv), $"Worst frame: {s.MaxFrameTimeMs.ToString("F1", inv)} ms", s.PointOnePercentLowFps >= 45 ? "#10b981" : "#f59e0b");
            AppendKpi(sb, "kpi-ft", "Avg Frametime", $"{s.AvgFrameTimeMs.ToString("F2", inv)} ms", $"Jitter (StdDev): &plusmn;{s.FrameTimeStdDevMs.ToString("F2", inv)} ms", "#f2f2f6");
            AppendKpi(sb, "kpi-stutter", "Stutter Spikes", s.StutterFrames33Ms.ToString("N0", inv), $">50 ms hitches: {s.SevereStutterFrames50Ms:N0}", s.StutterFrames33Ms <= 5 ? "#10b981" : "#f8364e");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"section\">");
            sb.AppendLine("  <div class=\"section-title wrap-title\">");
            sb.AppendLine("    <div>");
            sb.AppendLine("      <span id=\"chart-heading\">FPS &amp; 1% Low Timeline</span>");
            sb.AppendLine("      <span class=\"chart-hint no-print\">Hover to inspect &bull; Drop another report anywhere to compare</span>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"chart-controls\">");
            sb.AppendLine("      <div class=\"seg-toggle no-print\">");
            sb.AppendLine("        <button type=\"button\" id=\"mode-fps\" class=\"seg-btn active\">FPS</button>");
            sb.AppendLine("        <button type=\"button\" id=\"mode-ms\" class=\"seg-btn\">Frametime (ms)</button>");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <div id=\"chart-legend\" class=\"legend\">");
            sb.AppendLine("        <span><span class=\"dot\" style=\"background:#10b981;\"></span>Avg</span>");
            sb.AppendLine("        <span><span class=\"dot\" style=\"background:#f8364e;\"></span>1% Low</span>");
            sb.AppendLine("        <span id=\"legend-cmp-avg\" style=\"display:none;\"><span class=\"dot\" style=\"background:#38bdf8;\"></span>Compare Avg</span>");
            sb.AppendLine("        <span id=\"legend-cmp-low\" style=\"display:none;\"><span class=\"dot\" style=\"background:#c084fc;\"></span>Compare 1% Low</span>");
            sb.AppendLine("      </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div id=\"chart-stage\" class=\"chart-stage\">");
            sb.AppendLine("    <div id=\"chart-container\"></div>");
            sb.AppendLine("    <div id=\"chart-tooltip\" class=\"chart-tooltip\" style=\"display:none;\"></div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"section\">");
            sb.AppendLine("  <div class=\"section-title\"><span>Pacing Percentiles &amp; Memory Telemetry</span></div>");
            sb.AppendLine("  <div class=\"mini-grid\">");
            AppendMiniStat(sb, "P95 Frametime", $"{s.P95FrameTimeMs.ToString("F2", inv)} ms", "#f2f2f6");
            AppendMiniStat(sb, "P99 Frametime", $"{s.P99FrameTimeMs.ToString("F2", inv)} ms", s.P99FrameTimeMs <= 16.7 ? "#10b981" : (s.P99FrameTimeMs <= 33.3 ? "#f59e0b" : "#f43f5e"));
            AppendMiniStat(sb, "P99.9 Frametime", $"{s.P99_9FrameTimeMs.ToString("F2", inv)} ms", s.P99_9FrameTimeMs <= 33.3 ? "#10b981" : "#f59e0b");
            AppendMiniStat(sb, "Peak / Min FPS", $"{s.MaxFps.ToString("F0", inv)} / {s.MinFps.ToString("F0", inv)}", "#f2f2f6");
            AppendMiniStat(sb, "Worst Dip", worstDipText, "#f8364e");
            AppendMiniStat(sb, "Peak RAM Used", ramPeakText, "#38bdf8");
            AppendMiniStat(sb, "Min Free RAM", ramMinFreeText, s.MinFreeRamGb > 0 && s.MinFreeRamGb < 2.0 ? "#f59e0b" : "#10b981");
            AppendMiniStat(sb, "Standby Flushes", flushSummaryText, s.RamFlushCount > 0 ? "#a78bfa" : "#9a9ab0");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"section\">");
            sb.AppendLine("  <div class=\"section-title\"><span>Frametime Distribution</span></div>");
            AppendDistributionBar(sb, "&lt; 8.3 ms (120+ FPS)", s.PctUnder8_3Ms, "#10b981");
            AppendDistributionBar(sb, "8.3 &ndash; 16.7 ms (60&ndash;120 FPS)", s.Pct8_3To16_7Ms, "#38bdf8");
            AppendDistributionBar(sb, "16.7 &ndash; 33.3 ms (30&ndash;60 FPS)", s.Pct16_7To33_3Ms, "#f59e0b");
            AppendDistributionBar(sb, "&gt; 33.3 ms (&lt; 30 FPS / Stutter)", s.PctOver33_3Ms, "#f43f5e");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"two-col\" style=\"margin-bottom:20px;\">");
            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine("    <div class=\"section-title\"><span>System &amp; Hardware Snapshot</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">CPU</span><span>{cpuEnc}</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">GPU</span><span>{gpuEnc}</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">Driver</span><span>{driverEnc}</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">RAM</span><span>{ramEnc}</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">Display</span><span>{displayEnc}</span></div>");
            sb.AppendLine($"    <div class=\"spec-row\"><span class=\"spec-key\">OS</span><span>{osEnc}</span></div>");
            sb.AppendLine("  </div>");

            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine($"    <div class=\"section-title\"><span>Effective Optimizations ({(s.HasGameProfile ? "Per-Game Resolved" : "Global")})</span></div>");
            sb.AppendLine($"    <div class=\"badges\">{activeOptimizationsHtml}</div>");
            if (s.HasGameProfile)
            {
                string fpsCapLabel = s.ConfiguredFpsLimit > 0 ? $"{s.ConfiguredFpsLimit} FPS" : "Unlimited";
                string screenLabel = s.ConfiguredScreenMode == 0 ? "Borderless" : "Exclusive Fullscreen";
                sb.AppendLine("    <div class=\"profile-meta\">");
                sb.AppendLine($"      <div><span class=\"spec-key\">FPS Cap</span> {WebUtility.HtmlEncode(fpsCapLabel)} &bull; <span style=\"color:var(--muted);\">Mode:</span> {WebUtility.HtmlEncode(screenLabel)}</div>");
                if (!string.IsNullOrWhiteSpace(s.LaunchArguments))
                    sb.AppendLine($"      <div style=\"margin-top:4px;\"><span class=\"spec-key\">Args</span> <code>{WebUtility.HtmlEncode(s.LaunchArguments)}</code></div>");
                sb.AppendLine("    </div>");
            }
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class=\"two-col\">");
            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine("    <div class=\"section-title\"><span>Session Diagnostic Notes</span></div>");
            sb.AppendLine(diagnosticsHtml);
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div class=\"section\" style=\"margin-bottom:0;\">");
            sb.AppendLine("    <div class=\"section-title\"><span>Hardware Health Snapshot</span></div>");
            sb.AppendLine(healthHtml);
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class=\"footer\">Generated automatically by Overcapped Optimizer &bull; Void Tech Studios</div>");
            sb.AppendLine("</div>");

            sb.AppendLine($"<script type=\"application/json\" id=\"oc-data\">{safeJson}</script>");
            sb.AppendLine("<script>");
            sb.AppendLine(ReportJs);
            sb.AppendLine("</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static void AppendKpi(StringBuilder sb, string id, string label, string value, string sub, string color)
        {
            sb.AppendLine($"<div class=\"kpi\" id=\"{id}\">");
            sb.AppendLine("  <div class=\"kpi-top\">");
            sb.AppendLine($"    <div class=\"kpi-label\">{label}</div>");
            sb.AppendLine($"    <div class=\"kpi-delta\" id=\"{id}-delta\"></div>");
            sb.AppendLine("  </div>");
            sb.AppendLine($"  <div class=\"kpi-value\" style=\"color:{color};\">{value}</div>");
            sb.AppendLine($"  <div class=\"kpi-sub\">{sub}</div>");
            sb.AppendLine("</div>");
        }

        private static void AppendMiniStat(StringBuilder sb, string label, string value, string color)
        {
            sb.AppendLine("<div class=\"mini-stat\">");
            sb.AppendLine($"  <div class=\"mini-label\">{WebUtility.HtmlEncode(label)}</div>");
            sb.AppendLine($"  <div class=\"mini-val\" style=\"color:{color};\">{WebUtility.HtmlEncode(value)}</div>");
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

        private static string BuildActiveOptimizationBadges(FpsSessionSummary s)
        {
            var c = AppSettings.Current;
            var items = new List<(string Label, bool Enabled)>
            {
                ("Physical Cores Only", s.HasGameProfile ? s.EffectivePCoresOnly : c.PCoresOnly),
                ("Auto Power Plan", s.HasGameProfile ? s.EffectiveAutoPowerPlan : c.AutoPowerPlan),
                ("Background App Throttle", s.HasGameProfile ? s.EffectiveThrottleApps : c.ThrottleBackgroundApps),
                ("Game Mode / DVR Off", c.GameModeAndDisableDvr),
                ("FSO Disabled", c.DisableFSO),
                ("HAGS Enabled", HardwareDetector.IsHagsEnabled()),
                ("Pause Win Search", c.PauseWindowsSearchWhileGaming),
                ("Telemetry Reduced", c.ReduceWindowsTelemetry),
                ("Delivery Opt P2P Off", c.DisableDeliveryOptimization),
                ("NIC Power Saving Off", c.DisableNicPowerSaving),
                ("NIC Int. Mod Off", c.DisableInterruptModeration),
                ("NIC RSS Enabled", c.EnableNicRss),
                ("Auto Standby RAM Flush", c.AutoFlushEnabled)
            };

            if (s.HasGameProfile)
            {
                items.Insert(0, ("IFEO High Priority", s.HighPriorityIfeo));
                items.Insert(1, ("High-Perf GPU Binding", s.HighPerfGpuBinding));
                items.Insert(2, ("QoS DSCP 46", s.QosDscpEnabled));
            }

            var sb = new StringBuilder();
            foreach (var (label, enabled) in items)
            {
                string cls = enabled ? "badge-on" : "badge-off";
                string prefix = enabled ? "&#10003; " : "&times; ";
                sb.AppendLine($"<span class=\"{cls}\">{prefix}{WebUtility.HtmlEncode(label)}</span>");
            }
            return sb.ToString();
        }

        private static string BuildHealthWarningsHtml(List<HardwareHealthWarning> warnings)
        {
            if (warnings.Count == 0)
            {
                return "<div class=\"note note-ok\"><strong>All Checks Passed</strong><div>Monitor refresh rates, RAM XMP/dual-channel speed, Windows Page File, GPU Resizable BAR, HAGS/Game Mode, Power Plan, and system drive space look healthy.</div></div>";
            }

            var sb = new StringBuilder();
            foreach (var w in warnings)
            {
                string cls = w.Severity == HardwareHealthSeverity.Warning ? "note-warn" : "note-info";
                sb.AppendLine($"<div class=\"note {cls}\">");
                sb.AppendLine($"  <strong>[{WebUtility.HtmlEncode(w.Category)}] {WebUtility.HtmlEncode(w.Title)}</strong>");
                sb.AppendLine($"  <div>{WebUtility.HtmlEncode(w.Details)}</div>");
                sb.AppendLine("</div>");
            }
            return sb.ToString();
        }

        private static string BuildDiagnosticNotesHtml(FpsSessionSummary s, double smoothnessRatio, List<HardwareHealthWarning> healthWarnings)
        {
            var notes = new List<(string Level, string Title, string Body)>();
            var inv = CultureInfo.InvariantCulture;

            if (s.AvgFps >= 75 && s.StutterFrames33Ms >= 15)
            {
                notes.Add(("warn", "High Average FPS With Stutter Spikes",
                    $"Average FPS was strong ({s.AvgFps.ToString("F0", inv)} FPS), but {s.StutterFrames33Ms:N0} frames exceeded 33.3 ms. This usually points to shader compilation, asset streaming, or background RAM/disk paging rather than raw GPU speed."));
            }

            if (s.FrameTimeStdDevMs >= 6.0 || smoothnessRatio < 55.0)
            {
                notes.Add(("warn", "Uneven Frame Pacing Detected",
                    $"Frametime jitter averaged ±{s.FrameTimeStdDevMs.ToString("F2", inv)} ms (1% Low ratio: {smoothnessRatio.ToString("F0", inv)}%). Try enabling Background App Throttling, testing Physical Cores Only per-game, or capping FPS slightly below your average."));
            }

            if (s.DisplayRefreshHz >= 100 && s.PctAtOrAboveRefresh < 50.0)
            {
                notes.Add(("info", $"Below {s.DisplayRefreshHz} Hz Monitor Target",
                    $"Only {s.PctAtOrAboveRefresh.ToString("F1", inv)}% of frames hit your {s.DisplayRefreshHz} Hz refresh rate. Stepping down one preset or adjusting shadows/Lumen can bring 1% Lows closer to your panel's target."));
            }

            int[] commonCaps = { 60, 90, 120, 144, 165, 180, 240, 300 };
            bool tightlyCapped = s.FrameTimeStdDevMs < 2.0 && (s.MaxFps - s.AvgFps) < 8.0;
            bool nearCommonCap = commonCaps.Any(c => Math.Abs(s.AvgFps - c) <= 2.5) ||
                                 (s.ConfiguredFpsLimit > 0 && Math.Abs(s.AvgFps - s.ConfiguredFpsLimit) <= 3.0);
            if (tightlyCapped && nearCommonCap)
            {
                notes.Add(("info", "Frame Rate Cap or VSync Active",
                    $"Frame times stayed flat around {s.AvgFps.ToString("F0", inv)} FPS (±{s.FrameTimeStdDevMs.ToString("F2", inv)} ms). If you are benchmarking uncapped performance, check VSync or in-game FPS limiters."));
            }

            if (s.MinFreeRamGb > 0 && s.MinFreeRamGb < 2.5)
            {
                string action = AppSettings.Current.AutoFlushEnabled
                    ? $"Smart Standby RAM Auto-Flush ran {s.RamFlushCount} time(s) during the session."
                    : "Enabling Smart Standby RAM Auto-Flush can help prevent late-session paging hitches.";
                notes.Add(("warn", $"Low Free RAM During Session ({s.MinFreeRamGb.ToString("F1", inv)} GB min)", action));
            }
            else if (s.RamFlushCount > 0)
            {
                notes.Add(("info", $"Standby RAM Flushed {s.RamFlushCount} Time(s)",
                    "Standby flush events are marked on the timeline above so you can verify frame pacing after each purge."));
            }

            if (notes.Count == 0)
            {
                string extra = healthWarnings.Count > 0
                    ? " Review the Hardware Health Snapshot on the right for any system-level optimizations."
                    : " Frame pacing and stutter counts stayed clean across the session.";
                notes.Add(("ok", "Smooth Session Performance",
                    $"Maintained {s.AvgFps.ToString("F1", inv)} Avg FPS with {s.OnePercentLowFps.ToString("F1", inv)} FPS 1% Lows and only {s.StutterFrames33Ms:N0} stutter frame(s).{extra}"));
            }

            var sb = new StringBuilder();
            foreach (var (level, title, body) in notes)
            {
                string cls = level switch
                {
                    "warn" => "note-warn",
                    "ok" => "note-ok",
                    _ => "note-info"
                };
                sb.AppendLine($"<div class=\"note {cls}\">");
                sb.AppendLine($"  <strong>{WebUtility.HtmlEncode(title)}</strong>");
                sb.AppendLine($"  <div>{WebUtility.HtmlEncode(body)}</div>");
                sb.AppendLine("</div>");
            }
            return sb.ToString();
        }

        private static string FormatMmSs(int totalSeconds)
        {
            if (totalSeconds < 0) return "0:00";
            int mins = totalSeconds / 60;
            int secs = totalSeconds % 60;
            return $"{mins}:{secs:D2}";
        }

        private const string ReportCss = """
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
                --cyan: #38bdf8;
                --purple: #c084fc;
            }
            * { box-sizing: border-box; margin: 0; padding: 0; }
            body {
                background: var(--bg);
                color: var(--text);
                font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif;
                line-height: 1.5;
                padding-bottom: 48px;
            }
            code {
                background: #0e0e14;
                border: 1px solid var(--border);
                padding: 1px 6px;
                border-radius: 4px;
                font-family: Consolas, 'Cascadia Code', monospace;
                font-size: 12px;
                color: #e2e8f0;
            }
            .site-header {
                position: sticky;
                top: 0;
                z-index: 50;
                width: 100%;
                background: rgba(9, 9, 12, 0.94);
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
                max-width: 1080px;
                margin: 0 auto;
                padding: 0 20px;
                min-height: 64px;
                display: flex;
                align-items: center;
                justify-content: space-between;
                flex-wrap: wrap;
                gap: 12px;
            }
            .nav-left { display: flex; align-items: center; gap: 16px; }
            .brand-lockup { display: flex; align-items: center; gap: 12px; }
            .nav-logo { width: 36px; height: 36px; object-fit: contain; border-radius: 6px; display: block; }
            .brand-text { display: flex; flex-direction: column; line-height: 1; }
            .brand-title { font-size: 20px; font-weight: 800; letter-spacing: -0.025em; }
            .brand-title .over { color: #ffffff; }
            .brand-title .capped { color: var(--crimson-bright); }
            .brand-sub { font-size: 9px; font-weight: 700; letter-spacing: 0.22em; text-transform: uppercase; color: var(--muted); margin-top: 3px; }
            .nav-divider { height: 24px; width: 1px; background: var(--border); }
            .nav-tag { font-size: 12px; font-weight: 700; letter-spacing: 0.08em; text-transform: uppercase; color: var(--text); }
            .nav-actions { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
            .btn {
                background: var(--card);
                color: var(--text);
                border: 1px solid var(--border);
                border-radius: 6px;
                padding: 6px 12px;
                font-size: 12px;
                font-weight: 700;
                cursor: pointer;
                transition: border-color 0.15s, background 0.15s;
            }
            .btn:hover { border-color: var(--crimson-bright); background: #1e1e28; }
            .btn-accent { background: rgba(214, 28, 52, 0.18); border-color: rgba(248, 54, 78, 0.55); color: #fff; }
            .btn-accent:hover { background: var(--crimson); border-color: var(--crimson-bright); }
            .wrap { max-width: 1080px; margin: 0 auto; padding: 0 20px; }
            .drop-overlay {
                position: fixed;
                inset: 0;
                z-index: 999;
                background: rgba(9, 9, 12, 0.88);
                display: none;
                align-items: center;
                justify-content: center;
                padding: 24px;
            }
            .drop-overlay.active { display: flex; }
            .drop-box {
                border: 2px dashed var(--crimson-bright);
                background: rgba(22, 22, 29, 0.96);
                border-radius: 14px;
                padding: 44px 56px;
                text-align: center;
            }
            .drop-title { font-size: 22px; font-weight: 800; color: #fff; }
            .drop-sub { font-size: 13px; color: var(--muted); margin-top: 6px; }
            .compare-banner {
                background: linear-gradient(135deg, #131926 0%, #16161d 100%);
                border: 1px solid rgba(56, 189, 248, 0.4);
                border-left: 4px solid var(--cyan);
                border-radius: 10px;
                padding: 16px 20px;
                margin-bottom: 18px;
            }
            .compare-top { display: flex; justify-content: space-between; align-items: center; gap: 12px; flex-wrap: wrap; }
            .compare-eyebrow { font-size: 10px; font-weight: 800; letter-spacing: 1.5px; color: var(--cyan); }
            .compare-label { font-size: 14px; font-weight: 700; color: #fff; margin-top: 2px; }
            .compare-btns { display: flex; gap: 8px; }
            .compare-table-wrap { margin-top: 12px; overflow-x: auto; }
            .cmp-table { width: 100%; border-collapse: collapse; font-size: 12.5px; }
            .cmp-table th, .cmp-table td { padding: 7px 10px; border-bottom: 1px solid #222636; text-align: left; }
            .cmp-table th { color: var(--muted); font-size: 11px; text-transform: uppercase; letter-spacing: 0.7px; }
            .cmp-table tr:last-child td { border-bottom: none; }
            .delta-pos { color: var(--green); font-weight: 700; }
            .delta-neg { color: var(--crimson-bright); font-weight: 700; }
            .delta-flat { color: var(--muted); font-weight: 600; }
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
            .header-pills { display: flex; flex-direction: column; align-items: flex-end; gap: 8px; }
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
                grid-template-columns: repeat(auto-fit, minmax(175px, 1fr));
                gap: 14px;
                margin-bottom: 20px;
            }
            .kpi {
                background: var(--card);
                border: 1px solid var(--border);
                border-radius: 10px;
                padding: 16px 18px;
            }
            .kpi-top { display: flex; justify-content: space-between; align-items: center; gap: 6px; }
            .kpi-label { font-size: 11px; text-transform: uppercase; letter-spacing: 0.9px; color: var(--muted); font-weight: 600; }
            .kpi-delta { font-size: 11px; font-weight: 800; }
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
                gap: 12px;
            }
            .wrap-title { flex-wrap: wrap; }
            .chart-hint {
                display: block;
                font-size: 11px;
                font-weight: 500;
                text-transform: none;
                letter-spacing: 0;
                color: var(--muted);
                margin-top: 2px;
            }
            .chart-controls { display: flex; align-items: center; gap: 16px; flex-wrap: wrap; }
            .seg-toggle {
                display: inline-flex;
                background: #0e0e14;
                border: 1px solid var(--border);
                border-radius: 6px;
                padding: 2px;
            }
            .seg-btn {
                background: transparent;
                border: none;
                color: var(--muted);
                font-size: 11.5px;
                font-weight: 700;
                padding: 4px 10px;
                border-radius: 4px;
                cursor: pointer;
            }
            .seg-btn.active { background: var(--crimson); color: #fff; }
            .legend { display: flex; gap: 14px; font-size: 12px; color: var(--muted); font-weight: 500; text-transform: none; letter-spacing: 0; flex-wrap: wrap; }
            .dot { display: inline-block; width: 10px; height: 10px; border-radius: 50%; margin-right: 6px; }
            .chart-stage { position: relative; width: 100%; }
            .chart-tooltip {
                position: absolute;
                pointer-events: none;
                background: rgba(12, 12, 18, 0.96);
                border: 1px solid #353548;
                border-radius: 8px;
                padding: 10px 12px;
                font-size: 12px;
                color: var(--text);
                min-width: 185px;
                box-shadow: 0 10px 26px rgba(0,0,0,0.6);
                z-index: 20;
            }
            .tt-time { font-weight: 800; color: #fff; margin-bottom: 5px; border-bottom: 1px solid var(--border); padding-bottom: 4px; }
            .tt-row { display: flex; justify-content: space-between; gap: 12px; margin-top: 2px; }
            .tt-tag { margin-top: 6px; padding-top: 4px; border-top: 1px solid var(--border); color: var(--purple); font-weight: 700; font-size: 11px; }
            .mini-grid {
                display: grid;
                grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
                gap: 10px;
            }
            .mini-stat {
                background: #101016;
                border: 1px solid #222230;
                border-radius: 8px;
                padding: 11px 14px;
            }
            .mini-label { font-size: 11px; text-transform: uppercase; letter-spacing: 0.6px; color: var(--muted); font-weight: 600; }
            .mini-val { font-size: 15px; font-weight: 700; margin-top: 3px; }
            .bar-row { display: grid; grid-template-columns: 195px 1fr 66px; align-items: center; gap: 12px; margin-bottom: 10px; font-size: 13px; }
            .bar-track { height: 10px; background: #0e0e14; border-radius: 999px; overflow: hidden; border: 1px solid #232330; }
            .bar-fill { height: 100%; border-radius: 999px; }
            .two-col { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 18px; }
            .spec-row { padding: 8px 0; border-bottom: 1px solid #20202c; font-size: 13px; display: flex; gap: 10px; }
            .spec-row:last-child { border-bottom: none; }
            .spec-key { color: var(--muted); font-weight: 600; min-width: 68px; flex-shrink: 0; }
            .profile-meta { margin-top: 14px; padding-top: 12px; border-top: 1px solid #20202c; font-size: 12.5px; color: var(--text); }
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
            .note {
                border-radius: 8px;
                padding: 11px 14px;
                margin-bottom: 10px;
                font-size: 12.5px;
                border: 1px solid var(--border);
            }
            .note:last-child { margin-bottom: 0; }
            .note strong { display: block; margin-bottom: 3px; font-size: 13px; }
            .note div { color: var(--muted); }
            .note-ok { background: rgba(16, 185, 129, 0.09); border-color: rgba(16, 185, 129, 0.35); }
            .note-ok strong { color: #6ee7b7; }
            .note-warn { background: rgba(245, 158, 11, 0.1); border-color: rgba(245, 158, 11, 0.38); }
            .note-warn strong { color: #fcd34d; }
            .note-info { background: rgba(56, 189, 248, 0.09); border-color: rgba(56, 189, 248, 0.35); }
            .note-info strong { color: #7dd3fc; }
            .footer { text-align: center; color: var(--muted); font-size: 12px; margin-top: 24px; }
            svg { width: 100%; height: auto; display: block; user-select: none; }
            @media (max-width: 720px) {
                .nav-divider, .nav-tag { display: none; }
                .header-pills { align-items: flex-start; }
                .bar-row { grid-template-columns: 145px 1fr 56px; font-size: 12px; }
            }
            @media print {
                body { background: #09090c !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; padding-bottom: 0; }
                .no-print, .drop-overlay, .chart-tooltip { display: none !important; }
                .site-header { position: static; }
                .section, .kpi, .header, .compare-banner { break-inside: avoid; }
            }
            """;

        private const string ReportJs = """
            (function () {
                const rawEl = document.getElementById('oc-data');
                if (!rawEl) return;

                let currentData = JSON.parse(rawEl.textContent);
                let compareData = null;
                let chartMode = 'fps';

                const chartContainer = document.getElementById('chart-container');
                const tooltip = document.getElementById('chart-tooltip');
                const stage = document.getElementById('chart-stage');
                const fileInput = document.getElementById('compare-file-input');
                const dropOverlay = document.getElementById('drop-overlay');

                function fmtTime(sec) {
                    const s = Math.max(0, Math.round(sec || 0));
                    const m = Math.floor(s / 60);
                    const r = s % 60;
                    return m + ':' + String(r).padStart(2, '0');
                }

                function esc(str) {
                    return String(str ?? '').replace(/[&<>"']/g, function (c) {
                        return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
                    });
                }

                function toMs(fps) {
                    return fps > 0.1 ? 1000.0 / fps : 0;
                }

                function findNearestPoint(list, targetSec) {
                    if (!list || list.length === 0) return null;
                    let best = list[0];
                    let bestDist = Math.abs(list[0].t - targetSec);
                    for (let i = 1; i < list.length; i++) {
                        const d = Math.abs(list[i].t - targetSec);
                        if (d < bestDist) {
                            best = list[i];
                            bestDist = d;
                        }
                    }
                    return best;
                }

                function renderChart() {
                    const basePts = currentData.timeline || [];
                    const cmpPts = compareData ? (compareData.timeline || []) : [];

                    if (basePts.length < 2) {
                        chartContainer.innerHTML = '<div style="color:#9a9ab0;font-size:13px;padding:18px 0;">Session was too short to plot a multi-second timeline.</div>';
                        return;
                    }

                    const width = 980, height = 246;
                    const padL = 54, padR = 18, padT = 18, padB = 30;
                    const plotW = width - padL - padR;
                    const plotH = height - padT - padB;

                    const maxSec = Math.max(
                        basePts[basePts.length - 1].t,
                        cmpPts.length > 0 ? cmpPts[cmpPts.length - 1].t : 0,
                        1
                    );

                    const valOf = function (fps) {
                        return chartMode === 'fps' ? fps : toMs(fps);
                    };

                    let peakVal = 0;
                    basePts.forEach(function (p) {
                        peakVal = Math.max(peakVal, valOf(p.avg), valOf(p.low));
                    });
                    cmpPts.forEach(function (p) {
                        peakVal = Math.max(peakVal, valOf(p.avg), valOf(p.low));
                    });

                    let maxY;
                    if (chartMode === 'fps') {
                        maxY = Math.ceil(Math.max(60, peakVal * 1.12) / 30) * 30;
                    } else {
                        const cappedPeak = Math.min(peakVal, 120);
                        maxY = Math.ceil(Math.max(20, cappedPeak * 1.15) / 5) * 5;
                    }

                    const xOf = function (t) { return padL + (plotW * Math.min(t, maxSec) / maxSec); };
                    const yOf = function (v) {
                        const clamped = Math.max(0, Math.min(v, maxY));
                        return padT + plotH - (clamped / maxY) * plotH;
                    };

                    const buildPointsAttr = function (pts, field) {
                        return pts.map(function (p) {
                            return xOf(p.t).toFixed(1) + ',' + yOf(valOf(p[field])).toFixed(1);
                        }).join(' ');
                    };

                    let svg = '';
                    svg += '<svg id="oc-chart-svg" viewBox="0 0 ' + width + ' ' + height + '" xmlns="http://www.w3.org/2000/svg">';
                    svg += '<rect x="' + padL + '" y="' + padT + '" width="' + plotW + '" height="' + plotH + '" fill="#0e0e14" rx="4" />';

                    for (let step = 0; step <= 4; step++) {
                        const val = (maxY * step) / 4;
                        const y = padT + plotH - (plotH * step) / 4;
                        const lbl = chartMode === 'fps' ? Math.round(val) : val.toFixed(0) + ' ms';
                        svg += '<line x1="' + padL + '" y1="' + y.toFixed(1) + '" x2="' + (padL + plotW) + '" y2="' + y.toFixed(1) + '" stroke="#232330" stroke-width="1" />';
                        svg += '<text x="' + (padL - 8) + '" y="' + (y + 4).toFixed(1) + '" fill="#9a9ab0" font-size="11" text-anchor="end">' + lbl + '</text>';
                    }

                    if (currentData.displayHz > 0) {
                        const targetVal = chartMode === 'fps' ? currentData.displayHz : (1000.0 / currentData.displayHz);
                        if (targetVal > 0 && targetVal <= maxY) {
                            const ty = yOf(targetVal);
                            svg += '<line x1="' + padL + '" y1="' + ty.toFixed(1) + '" x2="' + (padL + plotW) + '" y2="' + ty.toFixed(1) + '" stroke="#38bdf8" stroke-opacity="0.4" stroke-width="1.2" stroke-dasharray="3,4" />';
                            svg += '<text x="' + (padL + plotW - 6) + '" y="' + (ty - 4).toFixed(1) + '" fill="#7dd3fc" font-size="10.5" text-anchor="end">' + currentData.displayHz + ' Hz Target</text>';
                        }
                    }

                    const flushes = currentData.ramFlushSeconds || [];
                    flushes.forEach(function (fs) {
                        if (fs >= 0 && fs <= maxSec) {
                            const fx = xOf(fs);
                            svg += '<line x1="' + fx.toFixed(1) + '" y1="' + padT + '" x2="' + fx.toFixed(1) + '" y2="' + (padT + plotH) + '" stroke="#a78bfa" stroke-opacity="0.55" stroke-width="1.2" stroke-dasharray="2,3" />';
                            svg += '<circle cx="' + fx.toFixed(1) + '" cy="' + (padT + 6) + '" r="3.5" fill="#a78bfa" />';
                        }
                    });

                    if (cmpPts.length >= 2) {
                        svg += '<polyline fill="none" stroke="#38bdf8" stroke-width="2.0" stroke-opacity="0.9" stroke-linejoin="round" points="' + buildPointsAttr(cmpPts, 'avg') + '" />';
                        svg += '<polyline fill="none" stroke="#c084fc" stroke-width="1.5" stroke-opacity="0.85" stroke-linejoin="round" stroke-dasharray="4,3" points="' + buildPointsAttr(cmpPts, 'low') + '" />';
                    }

                    svg += '<polyline fill="none" stroke="#10b981" stroke-width="2.3" stroke-linejoin="round" points="' + buildPointsAttr(basePts, 'avg') + '" />';
                    svg += '<polyline fill="none" stroke="#f8364e" stroke-width="1.8" stroke-linejoin="round" stroke-dasharray="5,3" points="' + buildPointsAttr(basePts, 'low') + '" />';

                    if (currentData.worstDipSec >= 0 && currentData.worstDipFps > 0) {
                        const dipPt = findNearestPoint(basePts, currentData.worstDipSec);
                        if (dipPt) {
                            const dx = xOf(dipPt.t);
                            const dy = yOf(valOf(dipPt.low));
                            svg += '<circle cx="' + dx.toFixed(1) + '" cy="' + dy.toFixed(1) + '" r="6" fill="rgba(248,54,78,0.22)" stroke="#f8364e" stroke-width="1.5" />';
                        }
                    }

                    svg += '<line id="chart-crosshair" x1="0" y1="' + padT + '" x2="0" y2="' + (padT + plotH) + '" stroke="#ffffff" stroke-opacity="0.35" stroke-width="1" style="display:none;" />';
                    svg += '<circle id="dot-base-avg" r="4" fill="#10b981" stroke="#09090c" stroke-width="1.5" style="display:none;" />';
                    svg += '<circle id="dot-base-low" r="3.5" fill="#f8364e" stroke="#09090c" stroke-width="1.5" style="display:none;" />';
                    svg += '<circle id="dot-cmp-avg" r="3.5" fill="#38bdf8" stroke="#09090c" stroke-width="1.5" style="display:none;" />';

                    svg += '<text x="' + padL + '" y="' + (height - 8) + '" fill="#9a9ab0" font-size="11">0:00</text>';
                    svg += '<text x="' + (padL + plotW * 0.25).toFixed(1) + '" y="' + (height - 8) + '" fill="#9a9ab0" font-size="11" text-anchor="middle">' + fmtTime(maxSec * 0.25) + '</text>';
                    svg += '<text x="' + (padL + plotW * 0.5).toFixed(1) + '" y="' + (height - 8) + '" fill="#9a9ab0" font-size="11" text-anchor="middle">' + fmtTime(maxSec * 0.5) + '</text>';
                    svg += '<text x="' + (padL + plotW * 0.75).toFixed(1) + '" y="' + (height - 8) + '" fill="#9a9ab0" font-size="11" text-anchor="middle">' + fmtTime(maxSec * 0.75) + '</text>';
                    svg += '<text x="' + (padL + plotW) + '" y="' + (height - 8) + '" fill="#9a9ab0" font-size="11" text-anchor="end">' + fmtTime(maxSec) + '</text>';

                    svg += '<rect id="chart-hit" x="' + padL + '" y="' + padT + '" width="' + plotW + '" height="' + plotH + '" fill="transparent" style="cursor:crosshair;" />';
                    svg += '</svg>';

                    chartContainer.innerHTML = svg;

                    const svgEl = document.getElementById('oc-chart-svg');
                    const hit = document.getElementById('chart-hit');
                    const crosshair = document.getElementById('chart-crosshair');
                    const dotAvg = document.getElementById('dot-base-avg');
                    const dotLow = document.getElementById('dot-base-low');
                    const dotCmp = document.getElementById('dot-cmp-avg');

                    hit.addEventListener('mousemove', function (e) {
                        const rect = svgEl.getBoundingClientRect();
                        const svgX = ((e.clientX - rect.left) / rect.width) * width;
                        const ratio = Math.max(0, Math.min(1, (svgX - padL) / plotW));
                        const sec = ratio * maxSec;
                        const pt = findNearestPoint(basePts, sec);
                        if (!pt) return;

                        const px = xOf(pt.t);
                        crosshair.setAttribute('x1', px.toFixed(1));
                        crosshair.setAttribute('x2', px.toFixed(1));
                        crosshair.style.display = 'block';

                        dotAvg.setAttribute('cx', px.toFixed(1));
                        dotAvg.setAttribute('cy', yOf(valOf(pt.avg)).toFixed(1));
                        dotAvg.style.display = 'block';

                        dotLow.setAttribute('cx', px.toFixed(1));
                        dotLow.setAttribute('cy', yOf(valOf(pt.low)).toFixed(1));
                        dotLow.style.display = 'block';

                        let html = '<div class="tt-time">Time ' + fmtTime(pt.t) + '</div>';
                        html += '<div class="tt-row"><span style="color:#10b981;">Avg FPS</span><strong>' + pt.avg.toFixed(1) + ' (' + toMs(pt.avg).toFixed(1) + ' ms)</strong></div>';
                        html += '<div class="tt-row"><span style="color:#f8364e;">1% Low</span><strong>' + pt.low.toFixed(1) + ' (' + toMs(pt.low).toFixed(1) + ' ms)</strong></div>';

                        if (cmpPts.length >= 2) {
                            const cpt = findNearestPoint(cmpPts, pt.t);
                            if (cpt) {
                                dotCmp.setAttribute('cx', xOf(cpt.t).toFixed(1));
                                dotCmp.setAttribute('cy', yOf(valOf(cpt.avg)).toFixed(1));
                                dotCmp.style.display = 'block';
                                html += '<div class="tt-row" style="margin-top:5px;padding-top:4px;border-top:1px solid #262633;"><span style="color:#38bdf8;">Compare Avg</span><strong>' + cpt.avg.toFixed(1) + '</strong></div>';
                                html += '<div class="tt-row"><span style="color:#c084fc;">Compare 1%</span><strong>' + cpt.low.toFixed(1) + '</strong></div>';
                            }
                        } else {
                            dotCmp.style.display = 'none';
                        }

                        const nearFlush = flushes.some(function (fs) { return Math.abs(fs - pt.t) <= 2; });
                        if (nearFlush) {
                            html += '<div class="tt-tag">↻ Standby RAM Auto-Flush</div>';
                        }

                        tooltip.innerHTML = html;
                        tooltip.style.display = 'block';

                        const stageRect = stage.getBoundingClientRect();
                        let left = e.clientX - stageRect.left + 14;
                        if (left + tooltip.offsetWidth > stageRect.width - 8) {
                            left = e.clientX - stageRect.left - tooltip.offsetWidth - 14;
                        }
                        tooltip.style.left = Math.max(8, left) + 'px';
                        tooltip.style.top = '12px';
                    });

                    hit.addEventListener('mouseleave', function () {
                        crosshair.style.display = 'none';
                        dotAvg.style.display = 'none';
                        dotLow.style.display = 'none';
                        dotCmp.style.display = 'none';
                        tooltip.style.display = 'none';
                    });
                }

                function formatDelta(diff, suffix, higherIsBetter, decimals) {
                    const eps = Math.pow(10, -(decimals + 1));
                    if (Math.abs(diff) < eps) return { text: '0' + suffix, cls: 'delta-flat' };
                    const sign = diff > 0 ? '+' : '';
                    const good = higherIsBetter ? diff > 0 : diff < 0;
                    return {
                        text: sign + diff.toFixed(decimals) + suffix,
                        cls: good ? 'delta-pos' : 'delta-neg'
                    };
                }

                function renderComparison() {
                    const banner = document.getElementById('compare-banner');
                    const legAvg = document.getElementById('legend-cmp-avg');
                    const legLow = document.getElementById('legend-cmp-low');
                    const kpiIds = ['kpi-avg-delta', 'kpi-1low-delta', 'kpi-01low-delta', 'kpi-ft-delta', 'kpi-stutter-delta'];

                    if (!compareData) {
                        banner.style.display = 'none';
                        legAvg.style.display = 'none';
                        legLow.style.display = 'none';
                        kpiIds.forEach(function (id) {
                            const el = document.getElementById(id);
                            if (el) { el.textContent = ''; el.className = 'kpi-delta'; }
                        });
                        renderChart();
                        return;
                    }

                    banner.style.display = 'block';
                    legAvg.style.display = 'inline';
                    legLow.style.display = 'inline';

                    document.getElementById('compare-label').textContent =
                        'Current: ' + currentData.gameName + ' (' + currentData.presetName + ')  vs.  Baseline: ' +
                        compareData.gameName + ' (' + compareData.presetName + ' • ' + compareData.endedAt + ')';

                    const setKpiDelta = function (id, diff, suffix, higherIsBetter, dec) {
                        const el = document.getElementById(id);
                        if (!el) return;
                        const d = formatDelta(diff, suffix, higherIsBetter, dec);
                        el.textContent = d.text;
                        el.className = 'kpi-delta ' + d.cls;
                    };

                    setKpiDelta('kpi-avg-delta', currentData.avgFps - compareData.avgFps, ' FPS', true, 1);
                    setKpiDelta('kpi-1low-delta', currentData.onePercentLowFps - compareData.onePercentLowFps, ' FPS', true, 1);
                    setKpiDelta('kpi-01low-delta', currentData.pointOnePercentLowFps - compareData.pointOnePercentLowFps, ' FPS', true, 1);
                    setKpiDelta('kpi-ft-delta', currentData.avgFrameTimeMs - compareData.avgFrameTimeMs, ' ms', false, 2);
                    setKpiDelta('kpi-stutter-delta', currentData.stutterFrames33Ms - compareData.stutterFrames33Ms, '', false, 0);

                    const rows = [
                        ['Average FPS', currentData.avgFps.toFixed(1) + ' FPS', compareData.avgFps.toFixed(1) + ' FPS', currentData.avgFps - compareData.avgFps, compareData.avgFps, ' FPS', true, 1],
                        ['1% Low FPS', currentData.onePercentLowFps.toFixed(1) + ' FPS', compareData.onePercentLowFps.toFixed(1) + ' FPS', currentData.onePercentLowFps - compareData.onePercentLowFps, compareData.onePercentLowFps, ' FPS', true, 1],
                        ['0.1% Low FPS', currentData.pointOnePercentLowFps.toFixed(1) + ' FPS', compareData.pointOnePercentLowFps.toFixed(1) + ' FPS', currentData.pointOnePercentLowFps - compareData.pointOnePercentLowFps, compareData.pointOnePercentLowFps, ' FPS', true, 1],
                        ['Avg Frametime', currentData.avgFrameTimeMs.toFixed(2) + ' ms', compareData.avgFrameTimeMs.toFixed(2) + ' ms', currentData.avgFrameTimeMs - compareData.avgFrameTimeMs, compareData.avgFrameTimeMs, ' ms', false, 2],
                        ['Frametime Jitter (StdDev)', '±' + currentData.frameTimeStdDevMs.toFixed(2) + ' ms', '±' + compareData.frameTimeStdDevMs.toFixed(2) + ' ms', currentData.frameTimeStdDevMs - compareData.frameTimeStdDevMs, compareData.frameTimeStdDevMs, ' ms', false, 2],
                        ['Stutter Spikes (>33.3 ms)', String(currentData.stutterFrames33Ms), String(compareData.stutterFrames33Ms), currentData.stutterFrames33Ms - compareData.stutterFrames33Ms, 0, '', false, 0]
                    ];

                    let tbl = '<table class="cmp-table"><thead><tr><th>Metric</th><th>Current</th><th>Baseline</th><th>Delta</th></tr></thead><tbody>';
                    rows.forEach(function (r) {
                        const d = formatDelta(r[3], r[5], r[6], r[7]);
                        let pctStr = '';
                        if (r[4] > 0.01) {
                            const pct = (r[3] / r[4]) * 100;
                            pctStr = ' (' + (pct > 0 ? '+' : '') + pct.toFixed(1) + '%)';
                        }
                        tbl += '<tr><td>' + esc(r[0]) + '</td><td><strong>' + esc(r[1]) + '</strong></td><td>' + esc(r[2]) + '</td><td class="' + d.cls + '">' + esc(d.text + pctStr) + '</td></tr>';
                    });
                    tbl += '</tbody></table>';
                    document.getElementById('compare-table-wrap').innerHTML = tbl;

                    renderChart();
                }

                async function handleCompareFile(file) {
                    if (!file) return;
                    try {
                        const text = await file.text();
                        let parsed = null;
                        if (file.name.toLowerCase().endsWith('.json')) {
                            parsed = JSON.parse(text);
                        } else {
                            const match = text.match(/<script[^>]*id=["']oc-data["'][^>]*>([\s\S]*?)<\/script>/i);
                            if (match && match[1]) {
                                parsed = JSON.parse(match[1].trim());
                            }
                        }

                        if (!parsed || !Array.isArray(parsed.timeline)) {
                            alert('That file does not contain Overcapped v2.1+ benchmark telemetry data.');
                            return;
                        }

                        compareData = parsed;
                        renderComparison();
                    } catch (err) {
                        alert('Could not read comparison report: ' + err.message);
                    }
                }

                document.getElementById('mode-fps').addEventListener('click', function () {
                    chartMode = 'fps';
                    this.classList.add('active');
                    document.getElementById('mode-ms').classList.remove('active');
                    document.getElementById('chart-heading').textContent = 'FPS & 1% Low Timeline';
                    renderChart();
                });

                document.getElementById('mode-ms').addEventListener('click', function () {
                    chartMode = 'ms';
                    this.classList.add('active');
                    document.getElementById('mode-fps').classList.remove('active');
                    document.getElementById('chart-heading').textContent = 'Frametime (ms) Timeline';
                    renderChart();
                });

                document.getElementById('btn-compare').addEventListener('click', function () {
                    fileInput.value = '';
                    fileInput.click();
                });
                fileInput.addEventListener('change', function () {
                    if (this.files && this.files[0]) handleCompareFile(this.files[0]);
                });
                document.getElementById('btn-clear-compare').addEventListener('click', function () {
                    compareData = null;
                    renderComparison();
                });
                document.getElementById('btn-swap-compare').addEventListener('click', function () {
                    if (!compareData) return;
                    const tmp = currentData;
                    currentData = compareData;
                    compareData = tmp;
                    renderComparison();
                });

                let dragDepth = 0;
                window.addEventListener('dragenter', function (e) {
                    e.preventDefault();
                    dragDepth++;
                    dropOverlay.classList.add('active');
                });
                window.addEventListener('dragover', function (e) {
                    e.preventDefault();
                });
                window.addEventListener('dragleave', function (e) {
                    e.preventDefault();
                    dragDepth = Math.max(0, dragDepth - 1);
                    if (dragDepth === 0) dropOverlay.classList.remove('active');
                });
                window.addEventListener('drop', function (e) {
                    e.preventDefault();
                    dragDepth = 0;
                    dropOverlay.classList.remove('active');
                    if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files[0]) {
                        handleCompareFile(e.dataTransfer.files[0]);
                    }
                });

                document.getElementById('btn-copy-discord').addEventListener('click', function () {
                    const btn = this;
                    const d = currentData;
                    const lines = [
                        '**Overcapped Benchmark - ' + d.gameName + '** (' + d.presetName + ' | ' + d.engineName + ')',
                        '```',
                        'Avg FPS:      ' + d.avgFps.toFixed(1) + ' FPS (Median: ' + d.medianFps.toFixed(1) + ')',
                        '1% Low:       ' + d.onePercentLowFps.toFixed(1) + ' FPS (P99: ' + d.p99FrameTimeMs.toFixed(1) + ' ms)',
                        '0.1% Low:     ' + d.pointOnePercentLowFps.toFixed(1) + ' FPS (Worst: ' + d.maxFrameTimeMs.toFixed(1) + ' ms)',
                        'Frametime:    ' + d.avgFrameTimeMs.toFixed(2) + ' ms (Jitter ±' + d.frameTimeStdDevMs.toFixed(2) + ' ms)',
                        'Stutters:     ' + d.stutterFrames33Ms + ' (>33ms) | ' + d.severeStutterFrames50Ms + ' (>50ms)',
                        'Duration:     ' + d.durationText + ' (' + d.totalFrames.toLocaleString() + ' frames)',
                        'Display:      ' + d.displayText + (d.displayHz > 0 ? ' (' + d.pctAtOrAboveRefresh.toFixed(1) + '% at target)' : ''),
                        'Rig:          ' + d.hardware.cpu + ' | ' + d.hardware.gpu,
                        'Driver / RAM: ' + d.hardware.driver + ' | ' + d.hardware.ram,
                        '```'
                    ];
                    const text = lines.join('\n');

                    const done = function () {
                        const prev = btn.textContent;
                        btn.textContent = 'Copied!';
                        setTimeout(function () { btn.textContent = prev; }, 1600);
                    };

                    if (navigator.clipboard && navigator.clipboard.writeText) {
                        navigator.clipboard.writeText(text).then(done).catch(function () {
                            fallbackCopy(text);
                            done();
                        });
                    } else {
                        fallbackCopy(text);
                        done();
                    }
                });

                function fallbackCopy(text) {
                    const ta = document.createElement('textarea');
                    ta.value = text;
                    ta.style.position = 'fixed';
                    ta.style.opacity = '0';
                    document.body.appendChild(ta);
                    ta.select();
                    try { document.execCommand('copy'); } catch (_) { }
                    document.body.removeChild(ta);
                }

                function downloadFile(filename, content, mime) {
                    const blob = new Blob([content], { type: mime });
                    const url = URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = filename;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
                }

                document.getElementById('btn-export-csv').addEventListener('click', function () {
                    const safe = (currentData.gameName || 'Overcapped').replace(/[^a-z0-9_-]+/gi, '_');
                    const rows = ['ElapsedSeconds,Time,AvgFps,OnePercentLowFps,AvgFrameTimeMs,OnePercentLowFrameTimeMs'];
                    (currentData.timeline || []).forEach(function (p) {
                        rows.push([
                            p.t,
                            fmtTime(p.t),
                            p.avg.toFixed(1),
                            p.low.toFixed(1),
                            toMs(p.avg).toFixed(2),
                            toMs(p.low).toFixed(2)
                        ].join(','));
                    });
                    downloadFile(safe + '_timeline.csv', rows.join('\n'), 'text/csv;charset=utf-8');
                });

                document.getElementById('btn-export-json').addEventListener('click', function () {
                    const safe = (currentData.gameName || 'Overcapped').replace(/[^a-z0-9_-]+/gi, '_');
                    downloadFile(safe + '_benchmark.json', JSON.stringify(currentData, null, 2), 'application/json;charset=utf-8');
                });

                document.getElementById('btn-print').addEventListener('click', function () {
                    window.print();
                });

                renderChart();
            })();
            """;
    }
}