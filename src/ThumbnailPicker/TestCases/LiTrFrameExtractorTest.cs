using System;
using Android.Content;
using Android.Graphics;
using Xam.Plugin.LinkedIn.LiTr.Filter;
using Xam.Plugin.LinkedIn.LiTr.Filter.Video.GL;
using Xam.Plugin.LinkedIn.LiTr.FrameExtract;
using Xam.Plugin.LinkedIn.LiTr.Render;

namespace ThumbnailPicker.TestCases
{
	public class LiTrFrameExtractorTest : Java.Lang.Object, IImageTest, IFrameExtractListener
    {

        public static string Title => "LiTr FrameExtractor";

        VideoFrameExtractor? videoFrameExtractor;
        GlSingleFrameRenderer renderer = new GlSingleFrameRenderer(new IGlFilter[] { new DefaultVideoFrameRenderFilter() });
        Android.Net.Uri? videoUri;
        Dictionary<string, TaskCompletionSource<Bitmap?>> tasks = new Dictionary<string, TaskCompletionSource<Bitmap?>>();

        public Bitmap? RunTest(long currentTargetMs)
        {
            if (videoFrameExtractor == null)
            {
                return null;
            }

            videoFrameExtractor.StopAll();

            var requestId = Guid.NewGuid().ToString();
            tasks[requestId] = new TaskCompletionSource<Bitmap?>();


            var currentTargetUs = currentTargetMs * 1000;
            
            var frameExtractParameters = new FrameExtractParameters(videoUri, currentTargetUs, renderer, FrameExtractMode.Exact);

            videoFrameExtractor.Extract(requestId, frameExtractParameters, this);

            return tasks[requestId].Task.GetAwaiter().GetResult();
        }

        public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
        {
            videoUri = source;
            videoFrameExtractor = new VideoFrameExtractor(context);
        }

        public void Destroy()
        {
            videoFrameExtractor?.StopAll();
            videoFrameExtractor = null;
        }

        #region IFrameExtractListener
        public void OnCancelled(string id, long timestampUs)
        {
            if (tasks.ContainsKey(id))
            {
                tasks[id].TrySetResult(null);
                tasks.Remove(id);
            }
        }

        public void OnError(string id, long timestampUs, Java.Lang.Throwable cause)
        {
            if (tasks.ContainsKey(id))
            {
                tasks[id].TrySetResult(null);
                tasks.Remove(id);
            }
        }

        public void OnExtracted(string id, long timestampUs, Bitmap bitmap)
        {
            if (tasks.ContainsKey(id))
            {
                tasks[id].TrySetResult(bitmap);
                tasks.Remove(id);
            }
        }

        public void OnStarted(string id, long timestampUs)
        {

        }
        #endregion IFrameExtractListener
    }
}

