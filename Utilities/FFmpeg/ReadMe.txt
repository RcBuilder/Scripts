FFMPEG - CLI Video and Audio Editor

about
-----
this CLI tool allow us to edit video files (mp4, avi etc.), audio files, streaming and more.
we can use it to get video attributes such as duration.
we can take snapshots from a video.
we can slice a video or join several videos into one. 
and more ... 

sources
-------
https://www.ffmpeg.org/
https://www.ffmpeg.org/documentation.html
-
https://unix.stackexchange.com/questions/28803/how-can-i-reduce-a-videos-size-with-ffmpeg
https://stackoverflow.com/questions/18468334/compressing-mp4-files

files
-----
ffmpeg.exe
ffplay.exe
ffprobe.exe

capabilities
------------
decode, encode, transcode, mux, demux, stream, filter and play, compress

commands
--------
https://www.ffmpeg.org/documentation.html
> ffmpeg -i input.avi -r 24 output.avi
> ffmpeg -i input.mp4 -vcodec libx264 -crf 28 output.mp4   // compress a video
> ffmpeg -i input.mp4 -b 800k output.mp4   // compress a video using custom bitrate of 800 kbit/s

FFmpegWrapper.cs
----------------
C# class uses ffmpeg & ffprobe

var mp4FilePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Resources\\adv.mp4"); 
             
var duration = FFmpegWrapper.GetMp4VideoDuration(mp4FilePath);
Console.WriteLine(duration);

var snapshot1 = FFmpegWrapper.TakeSnapshotFromMp4Video(mp4FilePath, 3);
Console.WriteLine(snapshot1.Size);

var snapshot2 = FFmpegWrapper.TakeSnapshotFromMp4Video(mp4FilePath, 5.443);
Console.WriteLine(snapshot2.Size); 
             
var partial = FFmpegWrapper.SliceMp4Video(mp4FilePath, 0, 11);
Console.WriteLine(partial); 
             
var merged = FFmpegWrapper.JoinMp4Videos(mp4FilePath, mp4FilePath);
Console.WriteLine(merged); 