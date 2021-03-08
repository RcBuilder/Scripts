using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace TESTConsole
{
    public class FFmpegWrapper
    {
        /*  properties:  
         
            ffprobe.exe
            -----------
            -hide_banner // Hides the banner at the beginning of the command-line output
            -show_format // Outputs general information about the video file
            -show_streams // Outputs information about every stream in the video file
            -pretty // Formats the output in a MS INI format with [/...] end tags
            {file} // The input file - has to be at the end 
           
            ffmpeg.exe
            ----------
            -hide_banner // Hides the banner at the beginning of the command-line output
            -ss {hh:mm:ss.fff} or {n} // Jumps to the specified position in the video - if this is defined before the input the input video is seeked, if defined before the output the input is decoded up to the position
            -i {file} // Defines the input file
            -r {n} // Sets the forced frame rate 
            -t {hh:mm:ss.fff} or {n} // Sets the length of frames to output
            -f {format} // Sets the forced format to use for input or output 
            -y // override created file if exists
            -filter_complex concat // concat files (n=[num of files]:v=[bool:video]:a=[bool:audio])
            {file} // The output file - has to be at the end
        */

        public static string GetMp4VideoDuration(string mp4FilePath) {           
            var ffprobeExePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\FFmpeg\\ffprobe.exe");            

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

                #region MetaData
                /*
                [STREAM]
                index=0
                codec_name=h264
                codec_long_name=H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10
                profile=Constrained Baseline
                codec_type=video
                codec_time_base=1001/60000
                codec_tag_string=avc1
                codec_tag=0x31637661
                width=854
                height=480
                coded_width=864
                coded_height=480
                has_b_frames=0
                sample_aspect_ratio=0:1
                display_aspect_ratio=0:1
                pix_fmt=yuv420p
                level=31
                color_range=N/A
                color_space=unknown
                color_transfer=unknown
                color_primaries=unknown
                chroma_location=left
                timecode=N/A
                refs=2
                is_avc=true
                nal_length_size=4
                id=N/A
                r_frame_rate=2997/100
                avg_frame_rate=2997/100
                time_base=1/29970
                start_pts=0
                start_time=0:00:00.000000
                duration_ts=577000
                duration=0:00:19.252586
                bit_rate=693.856000 Kbit/s
                max_bit_rate=N/A
                bits_per_raw_sample=8
                nb_frames=577
                nb_read_frames=N/A
                nb_read_packets=N/A
                DISPOSITION:default=1
                DISPOSITION:dub=0
                DISPOSITION:original=0
                DISPOSITION:comment=0
                DISPOSITION:lyrics=0
                DISPOSITION:karaoke=0
                DISPOSITION:forced=0
                DISPOSITION:hearing_impaired=0
                DISPOSITION:visual_impaired=0
                DISPOSITION:clean_effects=0
                DISPOSITION:attached_pic=0
                TAG:creation_time=2015-09-01 18:05:20
                TAG:language=und
                TAG:handler_name=VideoHandler
                TAG:encoder=AVC Coding
                [/STREAM]
                [STREAM]
                index=1
                codec_name=aac
                codec_long_name=AAC (Advanced Audio Coding)
                profile=LC
                codec_type=audio
                codec_time_base=1/48000
                codec_tag_string=mp4a
                codec_tag=0x6134706d
                sample_fmt=fltp
                sample_rate=48 KHz
                channels=2
                channel_layout=stereo
                bits_per_sample=0
                id=N/A
                r_frame_rate=0/0
                avg_frame_rate=0/0
                time_base=1/48000
                start_pts=0
                start_time=0:00:00.000000
                duration_ts=924672
                duration=0:00:19.264000
                bit_rate=170.613000 Kbit/s
                max_bit_rate=192 Kbit/s
                bits_per_raw_sample=N/A
                nb_frames=903
                nb_read_frames=N/A
                nb_read_packets=N/A
                DISPOSITION:default=1
                DISPOSITION:dub=0
                DISPOSITION:original=0
                DISPOSITION:comment=0
                DISPOSITION:lyrics=0
                DISPOSITION:karaoke=0
                DISPOSITION:forced=0
                DISPOSITION:hearing_impaired=0
                DISPOSITION:visual_impaired=0
                DISPOSITION:clean_effects=0
                DISPOSITION:attached_pic=0
                TAG:creation_time=2015-09-01 18:05:20
                TAG:language=und
                TAG:handler_name=SoundHandler
                [/STREAM]
                [FORMAT]
                filename=C:\Users\RcBuilder\Desktop\bin\adv.mp4
                nb_streams=2
                nb_programs=0
                format_name=mov,mp4,m4a,3gp,3g2,mj2
                format_long_name=QuickTime / MOV
                start_time=0:00:00.000000
                duration=0:00:19.263979
                size=1.992171 Mibyte
                bit_rate=867.502000 Kbit/s
                probe_score=100
                TAG:major_brand=mp42
                TAG:minor_version=0
                TAG:compatible_brands=mp41isom
                TAG:creation_time=2015-09-01 18:05:20
                [/FORMAT] 
            */
                #endregion

                metaData = p.StandardOutput.ReadToEnd();                
            }
            var pattern = "duration=(?<duration>\\d+:\\d+:\\d+)";
            var match = Regex.Match(metaData, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            return match.Groups["duration"].Value;
        }

        public static Bitmap TakeSnapshotFromMp4Video(string mp4FilePath, int second) {
            var ffmpegExePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\FFmpeg\\ffmpeg.exe");
            var snapshotImagePath = string.Concat(Path.GetDirectoryName(mp4FilePath), "\\", Guid.NewGuid(), ".jpeg");

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

        public static string SliceMp4Video(string mp4FilePath, int startSecond, int seconds) {
            var ffmpegExePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\FFmpeg\\ffmpeg.exe");
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

        public static string JoinMp4Videos(string mp4FilePath1, string mp4FilePath2) {
            var ffmpegExePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\FFmpeg\\ffmpeg.exe");
            var mergedMp4FilePath = string.Concat(Path.GetDirectoryName(mp4FilePath1), "\\", Guid.NewGuid(), Path.GetExtension(mp4FilePath1));

            using (var p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = ffmpegExePath;
                // join 2 videos, filter_complex concat: n = num of videos to concat, v = video(true), a = audio(false), set format as mp4 (-f), override if file exists (-y)
                p.StartInfo.Arguments = string.Format("-hide_banner -i {0} -i {1} -filter_complex concat=n=2:v=1:a=0 -f mp4 -y {2}", mp4FilePath1, mp4FilePath2, mergedMp4FilePath);
                p.Start();
                p.WaitForExit();
            }

            return mergedMp4FilePath;
        }
    }
}
