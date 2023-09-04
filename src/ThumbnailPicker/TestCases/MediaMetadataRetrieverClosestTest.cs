using System;
using Android.Content;
using Android.Graphics;
using Android.Media;

namespace ThumbnailPicker.TestCases
{
	public class MediaMetadataRetrieverClosestTest : IImageTest
    {
        public static string Title => "MediaMetadataRetriever Closest";

        Android.Media.MediaMetadataRetriever retriever = new Android.Media.MediaMetadataRetriever();

        public Bitmap? RunTest(long currentTargetMs)
        {
            return retriever.GetFrameAtTime(currentTargetMs * 1000, Option.Closest);
        }

        public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
        {
            retriever.SetDataSource(context, source);
        }

        public void Destroy()
        {
            
        }
    }
}

