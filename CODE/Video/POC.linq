<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Drawing.dll</Reference>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	var mp4FilePath = @"E:\Scripts\FFmpeg Wrapper\Resources\adv.mp4"; 	             
	var duration = GetMp4VideoDuration(mp4FilePath);
	Console.WriteLine($"duration: {duration}");
	
	var snapshot1 = TakeSnapshotFromMp4Video(mp4FilePath, 6);
	Console.WriteLine($"snapshot1: {snapshot1.Size}");
		
	var snapshot2 = TakeSnapshotFromMp4Video(mp4FilePath, 8);
	Console.WriteLine($"snapshot2: {snapshot2.Size}");
	
	var partial = SliceMp4Video(mp4FilePath, 0, 11);
	Console.WriteLine(partial); 	
}

string GetMp4VideoDuration(string mp4FilePath){
	var ffprobeExePath = @"E:\Scripts\FFmpeg Wrapper\FFmpeg\ffprobe.exe";            

    var metaData = string.Empty;
    using (var p = new Process())
    {
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = ffprobeExePath;
        // get all video info
        p.StartInfo.Arguments = string.Format("-hide_banner -show_format -show_streams -pretty \"{0}\"", mp4FilePath);
        p.Start();
        p.WaitForExit();
        metaData = p.StandardOutput.ReadToEnd();    		 
    }
    var pattern = "duration=(?<duration>\\d+:\\d+:\\d+)";
    var match = Regex.Match(metaData, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
    return match.Groups["duration"].Value;
}

Bitmap TakeSnapshotFromMp4Video(string mp4FilePath, int second) {    
	var ffmpegExePath = @"E:\Scripts\FFmpeg Wrapper\FFmpeg\ffmpeg.exe";
    var snapshotImagePath = string.Concat(Path.GetDirectoryName(mp4FilePath), "\\", Guid.NewGuid(), ".jpeg");
	Console.WriteLine(snapshotImagePath);
    using (var p = new Process())
    {
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = ffmpegExePath;
        // capture a photo located in the {second} second of the video {mp4FilePath}, set format as JPEG image (-f)
        p.StartInfo.Arguments = string.Format("-hide_banner -ss {0} -i \"{1}\" -r 1 -t 1 -f image2 \"{2}\"", TimeSpan.FromSeconds(second), mp4FilePath, snapshotImagePath);
        p.Start();
        p.WaitForExit();
    }

    if (File.Exists(snapshotImagePath))
    {
        var fileData = File.ReadAllBytes(snapshotImagePath);
        return new Bitmap(new MemoryStream(fileData));
    }

    return null;
}

string SliceMp4Video(string mp4FilePath, int startSecond, int seconds) {
    var ffmpegExePath = @"E:\Scripts\FFmpeg Wrapper\FFmpeg\ffmpeg.exe";
    var partialMp4FilePath = string.Concat(Path.GetDirectoryName(mp4FilePath), "\\", Guid.NewGuid(), Path.GetExtension(mp4FilePath));

    using (var p = new Process())
    {
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.FileName = ffmpegExePath;
        // slice the {mp4FilePath} video, get {seconds} frames (-t) starting the {startSecond} position (-ss) 
        p.StartInfo.Arguments = string.Format("-hide_banner -i \"{2}\" -ss {0} -t {1} -r 1 -c copy \"{3}\"", startSecond, seconds, mp4FilePath, partialMp4FilePath);		
        p.Start();
        p.WaitForExit();
    }

    return partialMp4FilePath;
}