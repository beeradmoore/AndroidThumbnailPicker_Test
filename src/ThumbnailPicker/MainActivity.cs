using System.Runtime.Serialization.Formatters.Binary;
using Android.Content;
using Android.Views;
using AndroidHUD;
using static Java.Util.Jar.Attributes;

namespace ThumbnailPicker;


[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity, AdapterView.IOnItemClickListener
{
    Type[] items = new Type[] {
        typeof(TestCases.MediaMetadataRetrieverClosestTest),
        typeof(TestCases.MediaMetadataRetrieverClosestSyncTest),
        typeof(TestCases.LiTrFrameExtractorTest),
        typeof(TestCases.VideoSurfaceViewTest),
        typeof(TestCases.VideoTextureViewTest),
        typeof(TestCases.FFmpegKitTest)
    };

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        var mainListView = FindViewById<ListView>(Resource.Id.main_list_view);
        if (mainListView != null)
        {
            mainListView.OnItemClickListener = this;

            //var arrayAdapter = new ArrayAdapter<Type>(this, Resource.Layout.main_listview_item, Resource.Id.listview_item_textview, items);
            mainListView.Adapter = new MainListViewAdapter(this, items);
        }

    }

    public void OnItemClick(AdapterView? parent, View? view, int position, long id)
    {
        var intent = new Intent(this, typeof(TestActivity));
        intent.PutExtra("test_type", items[position].ToString());
        StartActivity(intent);
    }
}
