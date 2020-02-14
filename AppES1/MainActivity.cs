using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;

namespace AppES1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme",MainLauncher =true)]
    public class MainActivity : AppCompatActivity
    {
        private CanvasView canvasView;
        private Button clearButton, sure_button, upload_button;
        private string imageName = "testImageName";
        private string base64Code = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            canvasView = (CanvasView)FindViewById(Resource.Id.canvas_view);
            canvasView.KeepScreenOn = true; // 保持屏幕常亮

            clearButton = (Button)FindViewById(Resource.Id.clear_button);
            sure_button = (Button)FindViewById(Resource.Id.sure_button);
            upload_button = (Button)FindViewById(Resource.Id.upload_button);

            // 清除屏幕
            clearButton.Click += (sender, e) => { canvasView.Clean(); };
            // 上传
            upload_button.Click += (sender, e) => {
                //Intent intent = new Intent(this, typeof(ListViewActivity));
                //StartActivity(intent);
            };
        }
    }
}