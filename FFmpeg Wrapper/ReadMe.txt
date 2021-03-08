USING:

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