using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Views;

namespace ThumbnailPicker.TestCases;

public class VideoTextureViewTest : IImageTest
{
    VideoTextureView videoTextureView;

    public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
    {
        videoTextureView = new VideoTextureView(context, source, videoWidth, videoHeight);
    }

    public Bitmap? RunTest(long currentTargetMs)
    {
        return videoTextureView.GetBitmapAtTime(currentTargetMs);
    }

    public void Destroy()
    {

    }

    internal class VideoTextureView : TextureView, TextureView.ISurfaceTextureListener
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        Surface? surface;
        //Bitmap? bitmapCache;
        Dictionary<long, TaskCompletionSource<Bitmap?>> tasks = new Dictionary<long, TaskCompletionSource<Bitmap?>>();
        int videoWidth;
        int videoHeight;

        public VideoTextureView(Context context, Android.Net.Uri source, int videoWidth, int videoHeight) : base(context)
        {
            this.videoWidth = videoWidth;
            this.videoHeight = videoHeight;

            mediaPlayer.SetDataSource(context, source);
            SurfaceTextureListener = this;
            mediaPlayer.Prepare();

            var isAvailable = this.IsAvailable;
            System.Diagnostics.Debugger.Break();

            /*
            var bitmapConfig = Android.Graphics.Bitmap.Config.Argb8888;
            if (bitmapConfig != null)
            {
                bitmapCache = Bitmap.CreateBitmap(videoWidth, videoHeight, bitmapConfig);
            }
            */
        }

        public Bitmap? GetBitmapAtTime(long positionMs)
        {
            var task = new TaskCompletionSource<Bitmap?>();
            tasks[positionMs] = task;

            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                mediaPlayer.SeekTo(positionMs, MediaPlayerSeekMode.Closest);
            }
            else
            {
                mediaPlayer.SeekTo((int)positionMs);
            }

            return task.Task.GetAwaiter().GetResult();
        }

        #region TextureView.ISurfaceTextureListener
        public void OnSurfaceTextureAvailable(SurfaceTexture surfaceTexture, int width, int height)
        {
            if (surface != null)
            {
                surface.Release();
                surface.Dispose();
                surface = null;
            }

            surface = new Surface(surfaceTexture);
            mediaPlayer.SetSurface(surface);
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            var currentTimestamp = (int)surface.Timestamp;
            if (tasks.ContainsKey(currentTimestamp))
            {
                var result = tasks[currentTimestamp];
                tasks.Remove(currentTimestamp);
                result.TrySetResult(GetBitmap(videoWidth, videoHeight));
            }
        }
        #endregion
    }
}

