using Android.Views;

namespace ThumbnailPicker;


[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity, AdapterView.IOnItemClickListener
{
    string[] items = new string[] { "one", "two", "three" };

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        var mainListView = FindViewById<ListView>(Resource.Id.main_list_view);
        if (mainListView != null)
        {
            mainListView.OnItemClickListener = this;

            var arrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.main_listview_item, Resource.Id.listview_item_textview, items);
            mainListView.Adapter = arrayAdapter;
        }
    }

    public void OnItemClick(AdapterView? parent, View? view, int position, long id)
    {
        StartActivity(typeof(TestActivity));
    }

}
