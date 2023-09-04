using System;
using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Util;
using AndroidHUD;
using ThumbnailPicker.TestCases;

namespace ThumbnailPicker;

[Activity(Label = "@string/app_name")]
public class TestActivity : Activity
{
    SixteenByNineImageView? standardImageView = null;
    SixteenByNineImageView? testImageView = null;
    Button? pickNearButton = null;
    Button? pickFarButton = null;
    TextView? baseTextResultTextView = null;
    TextView? actualTextResultTextView = null;

    int videoDurationMs = -1;
    int videoWidth = -1;
    int videoHeight = -1;
    int videoFrameCount = -1;
    double avarageFramesPerSecond = -1;
    double frameDuration = -1;
    Random rand = new Random();
    long currentTargetMs = 0;

    IImageTest? standardImageTest;
    IImageTest? imageTest;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_test);


        /*
        var displayMetrics = new DisplayMetrics();
        WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
        var height = displayMetrics.HeightPixels; //	height	2678	
        var width = displayMetrics.WidthPixels; //	width	1440	
        var density = displayMetrics.Density; //	density	3.5
        */



        var rawUri = Android.Net.Uri.Parse($"android.resource://{PackageName}/raw/door_30fps_30sec");
        if (rawUri == null)
        {
            return;
        }

        var retriever = new MediaMetadataRetriever();
        retriever.SetDataSource(this, rawUri);
        videoDurationMs = Int32.Parse(retriever.ExtractMetadata(MetadataKey.Duration) ?? "-1");
        videoWidth = Int32.Parse(retriever.ExtractMetadata(MetadataKey.VideoWidth) ?? "-1");
        videoHeight = Int32.Parse(retriever.ExtractMetadata(MetadataKey.VideoHeight) ?? "-1");
        videoFrameCount = Int32.Parse(retriever.ExtractMetadata(MetadataKey.VideoFrameCount) ?? "-1");

        avarageFramesPerSecond = (double)videoFrameCount / (double)videoDurationMs;
        frameDuration = 1.0 / avarageFramesPerSecond;

        standardImageView = FindViewById<SixteenByNineImageView>(Resource.Id.standardImageView);
        testImageView = FindViewById<SixteenByNineImageView>(Resource.Id.testImageView);
        pickNearButton = FindViewById<Button>(Resource.Id.pickNearButton);
        pickFarButton = FindViewById<Button>(Resource.Id.pickFarButton);
        baseTextResultTextView = FindViewById<TextView>(Resource.Id.baseTextResult);
        actualTextResultTextView = FindViewById<TextView>(Resource.Id.actualTextResult);

        if (pickNearButton != null && pickFarButton != null)
        {
            pickNearButton.Click += (sender, e) =>
            {
                // Move within 500ms of the current target.
                var nearRange = 250;
                var minTarget = currentTargetMs - nearRange;
                var maxTarget = currentTargetMs + nearRange;

                if (minTarget < 0)
                {
                    minTarget = 0;
                    maxTarget = nearRange * 2;
                }
                else if (maxTarget > videoDurationMs)
                {
                    maxTarget = videoDurationMs;
                    minTarget = videoDurationMs - (nearRange * 2);
                }

                // As we are dealing with a 30sec video we are not concerned about int range here
                currentTargetMs = rand.Next((int)minTarget, (int)maxTarget);

                RunTest();

            };
            pickFarButton.Click += (sender, e) =>
            {
                // Move at least 10 seconds away
                var newTargetMs = 0L;
                do
                {
                    newTargetMs = rand.Next(0, videoDurationMs);
                }
                while (Math.Abs(currentTargetMs - newTargetMs) < 10);

                currentTargetMs = newTargetMs;

                RunTest();
            };
        }

        // Standard test, always accurate, always slow.
        standardImageTest = new TestCases.MediaMetadataRetrieverClosestTest();

        var testTypeString = Intent?.GetStringExtra("test_type") ?? String.Empty;
        var testType = Type.GetType(testTypeString);
        if (testType != null)
        {
            var testToRun = Activator.CreateInstance(testType) as IImageTest;
            if (testToRun != null)
            {
                imageTest = testToRun;
            }
        }

        standardImageTest?.Load(this, rawUri, videoWidth, videoHeight);
        imageTest?.Load(this, rawUri, videoWidth, videoHeight);
    }

    void RunTest()
    {
        // Null check 
        if (pickNearButton == null || pickFarButton == null || baseTextResultTextView == null || actualTextResultTextView == null)
        {
            return;
        }

        pickNearButton.Enabled = false;
        pickFarButton.Enabled = false;

        AndHUD.Shared.Show(this, $"Running Base Test", -1, MaskType.Clear);

        ThreadPool.QueueUserWorkItem((obj) =>
        {
            var generateStandardStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var standardBitmap = standardImageTest?.RunTest(currentTargetMs);
            var generateStandardDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - generateStandardStartTime;

            RunOnUiThread(() =>
            {
                standardImageView?.SetImageBitmap(standardBitmap);


                AndHUD.Shared.Show(this, $"Running Actual Test", -1);
            });

            var generateTestStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var testBitmap = imageTest?.RunTest(currentTargetMs);
            var generateTestDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - generateTestStartTime;

            RunOnUiThread(() =>
            {
                testImageView?.SetImageBitmap(testBitmap);
            });

            /*
            var estimatedFrame = (int)Math.Ceiling((currentTargetMs / 1000.0) * avarageFramesPerSecond * 1000.0); // frameDuration * currentTargetMs;// / 1000.0);
            var stringBuilder = new System.Text.StringBuilder();
            //stringBuilder.AppendLine($"Target time: {currentTargetMs / 1000.0:0.00}sec");
            //stringBuilder.AppendLine($"Estimated frame: {estimatedFrame}");
            stringBuilder.AppendLine($"Base test duration: {generateStandardDuration / 1000.0:0.00}sec");
            stringBuilder.AppendLine($"Actual test duration: {generateTestDuration / 1000.0:0.00}sec");
            */

            RunOnUiThread(() =>
            {
                baseTextResultTextView.Text = $"{generateStandardDuration / 1000.0:0.00}sec";
                actualTextResultTextView.Text = $"{generateTestDuration / 1000.0:0.00}sec";

                AndHUD.Shared.Dismiss();

                pickNearButton.Enabled = true;
                pickFarButton.Enabled = true;
            });
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Clean up if there is anything specific to cleanup.
        standardImageTest?.Destroy();
        imageTest?.Destroy();
    }
}
