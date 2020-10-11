<Query Kind="Statements" />

var ffprobeExePath = @"E:\Projects\openbook\OpenBookProject\Website\Resources\Videos\FFmpeg\ffprobe.exe";            
var mp4FilePath = @"E:\Projects\openbook\OpenBookProject\Website\Resources\Videos\b4cd626dc94b4a4d84351f55be872fec.mp4";

var metaData = string.Empty;
using (var p = new Process())
{
    p.StartInfo.UseShellExecute = false;
    p.StartInfo.CreateNoWindow = true;
    p.StartInfo.RedirectStandardOutput = true;
    p.StartInfo.FileName = ffprobeExePath;
    // get all video info
    p.StartInfo.Arguments = string.Format("-hide_banner -show_format -show_streams -pretty {0}", mp4FilePath);
    p.Start();
    p.WaitForExit();
  
    metaData = p.StandardOutput.ReadToEnd();                
}
var pattern = "duration=(?<duration>\\d+:\\d+:\\d+)";
var match = Regex.Match(metaData, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
Console.WriteLine(match.Groups["duration"].Value);