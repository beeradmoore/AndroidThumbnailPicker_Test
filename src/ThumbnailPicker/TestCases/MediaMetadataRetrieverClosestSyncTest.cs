using System;
using Android.Content;
using Android.Graphics;

namespace ThumbnailPicker.TestCases;

public class MediaMetadataRetrieverClosestSyncTest : IImageTest
{
    public static string Title => "MediaMetadataRetriever ClosestSync";

    Android.Media.MediaMetadataRetriever retriever = new Android.Media.MediaMetadataRetriever();

    public Bitmap? RunTest(long currentTargetMs)
    {
        return retriever.GetFrameAtTime(currentTargetMs * 1000, Android.Media.Option.ClosestSync);
    }

    public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
    {
        retriever.SetDataSource(context, source);
    }

    public void Destroy()
    {

    }
}

