using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Path = Android.Graphics.Path;

namespace AppES1
{
    public class CanvasView : View
    {
        public static int STROKE_WIDTH = 5; // dp
        private Paint paint;
        private Path path;
        private int specW;
        private int specH;
        public CanvasView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context);
        }

        public CanvasView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context);
        }

        private void Initialize(Context context)
        {
            SetBackgroundColor(Color.White);
            paint = new Paint();
            paint.Color = Color.Black;

            //paint.StrokeWidth = TypedValue.ApplyDimension(TypedValue.COMPLEX_UNIT_DIP, STROKE_WIDTH,context.Resources.DisplayMetrics);
            paint.StrokeWidth = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, STROKE_WIDTH, Context.Resources.DisplayMetrics);
            paint.SetStyle(Paint.Style.Stroke); // 设置画笔空心
            paint.AntiAlias = true; // 消除锯齿

            path = new Path();
            int screenWidth = context.Resources.DisplayMetrics.WidthPixels;
            int screenHeight = context.Resources.DisplayMetrics.HeightPixels;
            specW = MeasureSpec.MakeMeasureSpec(screenWidth, MeasureSpecMode.Exactly);
            specH = MeasureSpec.MakeMeasureSpec(screenHeight, MeasureSpecMode.Exactly);
        }

        //protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        //{
        //    base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        //}

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            canvas.DrawPath(path, paint);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            base.OnTouchEvent(e);
            float x = e.GetX();
            float y = e.GetY();
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    path.MoveTo(x, y);
                    break;
                case MotionEventActions.Move:
                    path.LineTo(x, y);
                    break;
            }
            Invalidate(); // view更新
            return true;
        }

        public void Clean()
        {
            path.Reset();
            Invalidate();
        }

        // 将view生成图片
        public Bitmap CreateBitmap(View v)
        {
            int w = v.Width;
            int h = v.Height;
            // 生成图片
            Bitmap bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bitmap);
            c.DrawColor(Color.White);
            v.Layout(0, 0, w, h);
            v.Draw(c);
            // 旋转图片
            //return CompressScale(RotateToDegrees(bitmap,-90));//第一个参数是bitmap对象，第二个是角度
            // 压缩图片
            return CompressScale(bitmap);
        }

        /// <summary>
        /// 图片旋转
        /// </summary>
        /// <param name="tmpBitmap"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Bitmap RotateToDegrees(Bitmap tmpBitmap, float degrees)
        {
            Matrix matrix = new Matrix();
            matrix.Reset();
            matrix.SetRotate(degrees);
            Bitmap rBitmap = Bitmap.CreateBitmap(tmpBitmap, 0, 0, tmpBitmap.Width, tmpBitmap.Height, matrix,
                true);
            return rBitmap;
        }

        public static Bitmap CompressScale(Bitmap image)
        {
            MemoryStream baos = new MemoryStream();
            image.Compress(Bitmap.CompressFormat.Jpeg, 100, baos);
            // 判断如果图片大于1M,进行压缩避免在生成图片时溢出  
            if (baos.ToArray().Length / 1024 > 1024)
            {
                baos = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Jpeg, 80, baos);
            }
            MemoryStream isBm = new MemoryStream(baos.ToArray());
            BitmapFactory.Options newOpts = new BitmapFactory.Options();
            // 开始读入图片，此时把options.inJustDecodeBounds 设回true了  
            newOpts.InJustDecodeBounds = true;
            Bitmap bitmap = BitmapFactory.DecodeStream(isBm, null, newOpts);
            newOpts.InJustDecodeBounds = false;
            int w = newOpts.OutWidth;//原始宽高 
            int h = newOpts.OutHeight;
            // 缩放比。由于是固定比例缩放，只用高或者宽其中一个数据进行计算即可  (可根据原始高度计算)
            int be = 4;// be=1表示不缩放 ，缩放比为1/be ,这里缩小为原来的四分之一
            newOpts.InSampleSize = be; // 设置缩放比例  
            // newOpts.inPreferredConfig = Config.RGB_565;//降低图片从ARGB888到RGB565  
            // 重新读入图片，注意此时已经把options.inJustDecodeBounds 设回false了  
            isBm = new MemoryStream(baos.ToArray());
            bitmap = BitmapFactory.DecodeStream(isBm, null, newOpts);
            return bitmap;
        }
    }
}