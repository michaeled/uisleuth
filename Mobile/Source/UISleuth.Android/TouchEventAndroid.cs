using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;

namespace UISleuth.Android
{
    internal class TouchEventAndroid : ITouchEvent
    {
        private View _rootView;
        private View RootView
        {
            get
            {
                if (_rootView != null)
                {
                    return _rootView;
                }

                var activity = (Activity) Xamarin.Forms.Forms.Context;
                _rootView = activity.Window.DecorView.RootView;

                return _rootView;
            }
        }

        public void Down(int x, int y, int duration)
        {
            var downTime = SystemClock.UptimeMillis();
            var downDoneTime = SystemClock.UptimeMillis() + duration;

            var downEvent = MotionEvent.Obtain(downTime,
                downDoneTime, MotionEventActions.Down, x, y, 0);

            var upTime = SystemClock.UptimeMillis();
            var upDoneTime = SystemClock.UptimeMillis() + duration;

            var upEvent = MotionEvent.Obtain(upTime,
                upDoneTime, MotionEventActions.Up, x, y, 0);

            RootView.DispatchTouchEvent(downEvent);
            RootView.DispatchTouchEvent(upEvent);

            downEvent.Recycle();
            upEvent.Recycle();
        }

        public void Gesture(GesturePath[] path, int duration)
        {
            if (path == null || path.Length == 0) return;
            var lastIndex = path.Length - 1;

            // down event
            var downTime = SystemClock.UptimeMillis();
            var downDoneTime = SystemClock.UptimeMillis() + duration;

            var downEvent = MotionEvent.Obtain(downTime,
                downDoneTime, MotionEventActions.Down, path[0].X, path[0].Y, 0);

            RootView.DispatchTouchEvent(downEvent);

            // move event
            for (var i = 1; i < lastIndex; i++)
            {
                var moveTime = SystemClock.UptimeMillis();
                var moveDoneTime = SystemClock.UptimeMillis() + i;

                var moveEvent = MotionEvent.Obtain(moveTime,
                    moveDoneTime, MotionEventActions.Move, path[i].X, path[i].Y, 0);

                RootView.DispatchTouchEvent(moveEvent);
                moveEvent.Recycle();
            }

            // up event
            var upTime = SystemClock.UptimeMillis();
            var upDoneTime = SystemClock.UptimeMillis() + duration;

            var upEvent = MotionEvent.Obtain(upTime,
                upDoneTime, MotionEventActions.Up, path[lastIndex].X, path[lastIndex].Y, 0);

            RootView.DispatchTouchEvent(upEvent);

            downEvent.Recycle();
            upEvent.Recycle();
        }
    }
}