using System;
using Android.Content;
using Android.Runtime;
using Android.Util;

namespace ThumbnailPicker;

public class SixteenByNineImageView : ImageView
{
    public SixteenByNineImageView(Context context) : base(context)
    {
    }

    public SixteenByNineImageView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
    }

    public SixteenByNineImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
    }

    public SixteenByNineImageView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
    {
    }

    protected SixteenByNineImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

        int height = (int)Math.Round(MeasuredWidth / (1920.0 / 1080.0));

        SetMeasuredDimension(MeasuredWidth, height);
    }
}

