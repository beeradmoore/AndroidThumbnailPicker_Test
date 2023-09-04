using System;
using Android.Content;
using Android.Views;

namespace ThumbnailPicker;

public class MainListViewAdapter : BaseAdapter<Type>
{
    Context context;
    Type[] types;

    public MainListViewAdapter(Context context, Type[] types)
    {
        this.context = context;
        this.types = types;
    }

    public override Type this[int position] => types[position];

    public override int Count => types.Length;

    public override long GetItemId(int position)
    {
        return position;
    }

    public override View? GetView(int position, View? convertView, ViewGroup? parent)
    {
        if (convertView == null)
        {
            var inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            if (inflater != null)
            {
                convertView = inflater.Inflate(Resource.Layout.main_listview_item, null);
            }
        }

        var textView = convertView?.FindViewById<TextView>(Resource.Id.listview_item_textview);
        if (textView != null)
        {
            var typeString = types[position].ToString();
            textView.Text = typeString.Substring(typeString.LastIndexOf(".") + 1);
        }

        return convertView;
    }
}

