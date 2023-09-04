using System;
using Android.Content;
using Android.Graphics;

namespace ThumbnailPicker;

public interface IImageTest
{
	static string Title { get; } = String.Empty;
	Bitmap? RunTest(long currentTargetMs);
	void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight);
	void Destroy();
}

