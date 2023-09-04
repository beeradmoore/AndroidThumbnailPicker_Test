using System;
using Android.Content;
using Android.Graphics;
using Android.Nfc;
using Android.Util;
using Ffmpegkit.Droid;

namespace ThumbnailPicker.TestCases
{
    public class FFmpegKitTest : Java.Lang.Object, IImageTest
    {
        public static string Title => "FFmpegKit";

        string inputFile = String.Empty;



        public Bitmap? RunTest(long currentTargetMs)
        {
            var outputImage = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{DateTimeOffset.Now.ToUnixTimeSeconds()}.jpg");

            var currentTargetSeconds = currentTargetMs / 1000.0;

            // -q:v 3 
            // -vf scale=411:-1 
            var ffmpegCommand = $"-hwaccel mediacodec -i {inputFile} -ss {currentTargetSeconds} -frames:v 1 {outputImage}";
            //var ffmpegCommand = $"-i {inputFile} -vf select='eq(t\\,{currentTargetSeconds})' -vsync 0 {outputImage}";
            //var ffmpegCommand = $"-i {inputFile} -vf select='eq(t\\,{currentTargetSeconds})' -vframes 1 {outputImage}";
            System.Diagnostics.Debug.WriteLine(ffmpegCommand);
            var ffmpegSession = FFmpegKit.Execute(ffmpegCommand);



            var bitmap = BitmapFactory.DecodeFile(outputImage);

            var outputInfo = new FileInfo(outputImage);

            return bitmap;


            /*
            if (ReturnCode.IsSuccess(session.ReturnCode))
            {

                // SUCCESS

            }
            */
            /*
            else if (ReturnCode.isCancel(session.getReturnCode()))
            {

                // CANCEL

            }
            else
            {

                // FAILURE
                //Log.d(TAG, String.format("Command failed with state %s and rc %s.%s", session.getState(), session.getReturnCode(), session.getFailStackTrace()));

            }
            */


            //throw new NotImplementedException();
        }

        public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
        {
            //inputFile = FFmpegKitConfig.GetSafParameterForRead(context, source);

            //System.Diagnostics.Debug.WriteLine(FFmpegKit.Execute("-hwaccels").Output);
            //System.Diagnostics.Debug.WriteLine(FFmpegKit.Execute("-codecs").Output);
            //System.Diagnostics.Debug.WriteLine(FFmpegKit.Execute("-encoders").Output);
            //System.Diagnostics.Debug.WriteLine(FFmpegKit.Execute("-decoders").Output);


            inputFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_file.mp4");


            using (var inputStream = context.Resources?.OpenRawResource(Resource.Raw.door_30fps_30sec))
            {
                if (inputStream == null)
                {
                    return;
                }

                using (var fileStream = File.Create(inputFile))
                {
                    inputStream.CopyTo(fileStream);
                }
            }

            FFmpegKit.Android.FFmpegKitConfig.IgnoreSignal(Signal.Sigxcpu);
        }

        public void Destroy()
        {
            //throw new NotImplementedException();
        }
    }
}

