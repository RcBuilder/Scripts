using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace VideoCompressionApp
{
    /*           
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

    /*
        USING
        -----

        /// $@"{AppDomain.CurrentDomain.BaseDirectory}..\..\FFmpeg\"
        var ffmpegFolderPath = @"E:\Scripts\Utilities\FFmpeg\FFmpeg\";
        var ffmpegWrapper = new FFmpegWrapper(ffmpegFolderPath);

        /// $@"{AppDomain.CurrentDomain.BaseDirectory}..\..\Resources\adv.mp4"
        var mp4FilePath = @"D:\TEMP\VideoCompress\adv1.mp4";
           
        var duration = ffmpegWrapper.GetMp4VideoDuration(mp4FilePath);
        Console.WriteLine(duration);

        var snapshot1 = ffmpegWrapper.TakeSnapshotFromMp4Video(mp4FilePath, 3);
        Console.WriteLine(snapshot1.Size);

        var snapshot2 = ffmpegWrapper.TakeSnapshotFromMp4Video(mp4FilePath, 8);
        Console.WriteLine(snapshot2.Size);

        var partial = ffmpegWrapper.SliceMp4Video(mp4FilePath, 0, 11);
        Console.WriteLine(partial);

        var merged = ffmpegWrapper.JoinMp4Videos(mp4FilePath, mp4FilePath);
        Console.WriteLine(merged);

        var compressed = ffmpegWrapper.Compress(mp4FilePath);
        Console.WriteLine(compressed); 
    */

    public class FFmpegWrapper
    {
        private string FFMPEG_FOLDER_PATH { get; set; }

        private string FFPROBE { get { 
                return $"{FFMPEG_FOLDER_PATH}ffprobe.exe";
            } 
        }

        private string FFMPEG {
            get {
                return $"{FFMPEG_FOLDER_PATH}ffmpeg.exe";
            }
        }

        public FFmpegWrapper(string ffmpegFolderPath) {
            this.FFMPEG_FOLDER_PATH = ffmpegFolderPath;
        }

        public string GetMp4VideoDuration(string mp4FilePath) {           
            
            // get all video info
            var arguments = $"-hide_banner -show_format -show_streams -pretty \"{mp4FilePath}\"";

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

            var metaData = ExecuteProcess(this.FFPROBE, arguments);
            
            var pattern = "duration=(?<duration>\\d+:\\d+:\\d+)";
            var match = Regex.Match(metaData, pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            return match.Groups["duration"].Value;
        }

        public Bitmap TakeSnapshotFromMp4Video(string mp4FilePath, int second) {            
            var snapshotImagePath = $@"{Path.GetDirectoryName(mp4FilePath)}\{Guid.NewGuid()}.jpeg";

            // capture a photo located in the {second} second of the video {mp4FilePath}, set format as JPEG image (-f)
            var arguments = $"-hide_banner -ss {TimeSpan.FromSeconds(second)} -i \"{mp4FilePath}\" -r 1 -t 1 -f image2 \"{snapshotImagePath}\"";

            ExecuteProcess(this.FFMPEG, arguments);

            if (File.Exists(snapshotImagePath))
            {
                var fileData = File.ReadAllBytes(snapshotImagePath);
                return new Bitmap(new MemoryStream(fileData));
            }

            return null;
        }

        public string SliceMp4Video(string mp4FilePath, int startSecond, int seconds) {
            var partialMp4FilePath = $@"{Path.GetDirectoryName(mp4FilePath)}\{Guid.NewGuid()}{Path.GetExtension(mp4FilePath)}";

            // slice the {mp4FilePath} video, get {seconds} frames (-t) starting the {startSecond} position (-ss) 
            var arguments = $"-hide_banner -i \"{mp4FilePath}\" -ss {startSecond} -t {seconds} -r 1 -c copy \"{partialMp4FilePath}\"";

            ExecuteProcess(this.FFMPEG, arguments);
            
            return partialMp4FilePath;
        }

        public string JoinMp4Videos(string mp4FilePath1, string mp4FilePath2) {
            var mergedMp4FilePath = $@"{Path.GetDirectoryName(mp4FilePath1)}\{Path.GetFileNameWithoutExtension(mp4FilePath1)}_{Path.GetFileNameWithoutExtension(mp4FilePath2)}{Path.GetExtension(mp4FilePath1)}";

            // join 2 videos, filter_complex concat: n = num of videos to concat, v = video(true), a = audio(false), set format as mp4 (-f), override if file exists (-y)
            var arguments = $" -hide_banner -i \"{mp4FilePath1}\" -i \"{mp4FilePath2}\" -filter_complex concat=n=2:v=1:a=0 -f mp4 -y \"{mergedMp4FilePath}\"";

            ExecuteProcess(this.FFMPEG, arguments);
            
            return mergedMp4FilePath;
        }

        public string Compress(string mp4FilePath, bool isOverride = false)
        {
            var compressedMp4FilePath = $@"{Path.GetDirectoryName(mp4FilePath)}\{Path.GetFileNameWithoutExtension(mp4FilePath)}_compressed{Path.GetExtension(mp4FilePath)}";

            var arguments = $"-i \"{mp4FilePath}\" -vcodec libx264 -crf 28 \"{compressedMp4FilePath}\"";
                        
            ExecuteProcess(this.FFMPEG, arguments);

            if (isOverride) {
                try
                {
                    File.Delete(mp4FilePath);
                    File.Move(compressedMp4FilePath, mp4FilePath);
                    compressedMp4FilePath = mp4FilePath;
                }
                catch {}
            }

            return compressedMp4FilePath;
        }

        // --- 

        private string ExecuteProcess(string FileName, string Arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    WorkingDirectory = new FileInfo(FileName).DirectoryName,
                    FileName = FileName,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Dispose();

            return result;
        }
    }
}
