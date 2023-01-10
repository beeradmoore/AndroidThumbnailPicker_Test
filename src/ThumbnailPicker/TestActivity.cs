namespace ThumbnailPicker;

[Activity(Label = "@string/app_name")]
public class TestActivity : Activity
{
    SixteenByNineImageView? standardImageView = null;
    SixteenByNineImageView? testImageView = null;
    Button? pickNearButton = null;
    Button? pickFarButton = null;
    TextView? resultTextView = null;


    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_test);

        standardImageView = FindViewById<SixteenByNineImageView>(Resource.Id.standardImageView);
        testImageView = FindViewById<SixteenByNineImageView>(Resource.Id.testImageView);
        pickNearButton = FindViewById<Button>(Resource.Id.pickNearButton);
        pickFarButton = FindViewById<Button>(Resource.Id.pickFarButton);
        resultTextView = FindViewById<TextView>(Resource.Id.resultTextView);
    }

    /*
    public override void OnBackPressed()
    {
        base.OnBackPressed();
    }
    */
}
