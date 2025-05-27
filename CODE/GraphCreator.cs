using System;
using System.IO;
using System.Linq;
using System.Text;

// Install-Package SkiaSharp -Version 2.88.6
using SkiaSharp;

namespace GraphCreator
{
    class Program
    {
        public static string BASE_FOLDER = $"{AppDomain.CurrentDomain.BaseDirectory}";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0) {
                    Console.WriteLine("no file has provided, using a sample file...");
                    args = new string[] {
                        $"{BASE_FOLDER}ConnStr.txt",    // ConnStr
                        $"{BASE_FOLDER}chart_{DateTime.Now.ToString("yyyyMMddHHmmss")}.png",               // DestFolder
                    };
                }

                Console.WriteLine($"args: {string.Join(" | ", args)}");

                if (args.Length == 0)
                    throw new Exception("missing args: ConnStr file path");
                if (args.Length == 1)
                    throw new Exception("missing args: DestFolder path");

                var connStr = File.ReadAllText($"{args[0]?.Trim()}", Encoding.UTF8);
                var destPath = args[1].Trim();

                var dal = new PervasiveDAL(connStr);
                var data = dal.GetGraphData();

                double[] values = data?.Select(x => x.Y)?.ToArray(); /// { 3.35, 3.46, 3.5, 1.79 };
                string[] months = data?.Select(x => x.X)?.ToArray(); /// { "1", "3", "4", "4" };

                byte[] chartData = ChartGenerator.GenerateConsumptionChart(values, months);
                File.WriteAllBytes($"{destPath}", chartData);
            }
            catch (Exception ex) {
                Console.WriteLine($"[Error]: {ex.Message}");
                Logger.WriteError("ERROR", ex.Message);
                /// Console.WriteLine(ex.StackTrace);
            }

            Console.ReadKey();
        }

        public class ChartGenerator
        {
            /// <summary>
            /// Generates a consumption distribution chart using SkiaSharp (works for web and desktop)
            /// </summary>
            /// <param name="values">The consumption values to display</param>
            /// <param name="months">The month labels (optional)</param>
            /// <param name="title">Chart title (defaults to "התפלגות צריכה")</param>
            /// <param name="width">Chart width in pixels</param>
            /// <param name="height">Chart height in pixels</param>
            /// <returns>Byte array containing the chart image</returns>
            public static byte[] GenerateConsumptionChart(
                double[] values,
                string[] months = null,
                string title = "Consumption Chart",
                int width = 600,
                int height = 400)
            {
                // Create bitmap and canvas
                using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    // Calculate dimensions
                    int margin = 60;
                    int chartWidth = width - 2 * margin;
                    int chartHeight = height - 2 * margin;
                    int footerHeight = 40;

                    // Find max value for scaling
                    double maxValue = 0;
                    foreach (var value in values)
                    {
                        maxValue = Math.Max(maxValue, value);
                    }

                    // Round up max value for better chart scaling
                    maxValue = Math.Ceiling(maxValue * 10) / 10 + 0.1;

                    // Create paints
                    var barPaint = new SKPaint
                    {
                        Color = new SKColor(255, 69, 0), // OrangeRed
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill
                    };

                    var textPaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 14,
                        TextAlign = SKTextAlign.Center,
                        Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
                    };

                    var titlePaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 18,
                        TextAlign = SKTextAlign.Center
                    };

                    var axisLabelPaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 12,
                        TextAlign = SKTextAlign.Right
                    };

                    var axisLinePaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1
                    };

                    var gridLinePaint = new SKPaint
                    {
                        Color = SKColors.LightGray,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1,
                        PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0)
                    };

                    // Draw title (right-to-left)
                    /// canvas.DrawText(title, width - margin, margin - 30, titlePaint);

                    // Draw Y-axis label
                    /// canvas.Save();
                    /// canvas.RotateDegrees(-90);
                    /// canvas.DrawText("צריכה", -height / 2, 20, axisLabelPaint);
                    /// canvas.Restore();

                    // Draw Y-axis and grid lines
                    int yDivisions = 5;
                    for (int i = 0; i <= yDivisions; i++)
                    {
                        float y = margin + chartHeight - (i * chartHeight / yDivisions);
                        double value = (maxValue * i) / yDivisions;

                        // Draw grid line
                        canvas.DrawLine(margin, y, width - margin, y, i == 0 ? axisLinePaint : gridLinePaint);

                        // Draw y-axis label
                        canvas.DrawText(value.ToString("F2"), margin - 5, y + 5, axisLabelPaint);
                    }

                    // Draw X-axis
                    canvas.DrawLine(margin, margin + chartHeight, width - margin, margin + chartHeight, axisLinePaint);

                    // Draw bars
                    int barCount = values.Length;
                    float barWidth = chartWidth / (barCount * 2);

                    for (int i = 0; i < barCount; i++)
                    {
                        float x = margin + (i * 2 + 1) * barWidth;
                        float barHeight = (float)(values[i] / maxValue * chartHeight);
                        float y = margin + chartHeight - barHeight;

                        // Draw bar
                        canvas.DrawRect(x - barWidth / 2, y, barWidth, barHeight, barPaint);

                        // Draw value above bar
                        canvas.DrawText(values[i].ToString("F2"), x, y - 10, textPaint);

                        // Draw month label (x-axis)
                        string label = (months != null && i < months.Length) ? months[i] : (i + 1).ToString();
                        canvas.DrawText(label, x, margin + chartHeight + 20, textPaint);
                        
                        /// if (i == barCount - 1) canvas.DrawText("date", width / 2, height - margin + 30, textPaint);                        
                    }

                    // Create image from surface
                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }
    }
}
