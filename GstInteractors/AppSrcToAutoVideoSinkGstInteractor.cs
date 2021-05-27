using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNetCoreConsole.GstInteractors
{
    class AppSrcToAutoVideoSinkGstInteractor : AbstractGstInteractor
    {
        public Delegate NeedDataHandler;
        private static Gst.Element appsrc;
        private void PushData(object o, System.EventArgs args)
        {
            ulong mseconds = 0;
            if (appsrc.Clock != null)
                mseconds = appsrc.Clock.Time / 1000000;
            Gst.Buffer buffer = DrawData(mseconds);
            appsrc.Emit("push-buffer", buffer);
        }

        private Gst.Buffer DrawData(ulong mseconds)
        {
            byte[] buffer = new byte[640*480*4];
            Cairo.ImageSurface img = new Cairo.ImageSurface(buffer, Cairo.Format.Argb32, 640, 480, 640 * 4);
            using (Cairo.Context context = new Cairo.Context(img))
            {
                double dx = (double)(mseconds % 2180) / 5;
                context.SetSourceColor(new Cairo.Color(1.0, 1.0, 0));
                context.Paint();
                context.MoveTo(300, 10 + dx);
                context.LineTo(500 - dx, 400);
                context.LineWidth = 4.0;
                context.SetSourceColor(new Cairo.Color(0, 0, 1.0));
                context.Stroke();
            }
            img.Dispose();
            return new Gst.Buffer(buffer);
        }
        public override void Interact()
        {
            appsrc = Gst.ElementFactory.Make("appsrc", "source");
            var conv = Gst.ElementFactory.Make("videoconvert", "conv");
            var videosink = Gst.ElementFactory.Make("autovideosink", "videosink");

            var caps = Gst.Global.CapsFromString("video/x-raw, format=RGBA, width=640, height=480, framerate=4/1");
            appsrc.SetProperty("caps", new GLib.Value(caps));
            _pipeline.Add(appsrc, conv, videosink);
            Gst.Element.Link(appsrc, conv, videosink);
            appsrc.SetProperty("stream-type", new GLib.Value(0));
            appsrc.SetProperty("format", new GLib.Value(Gst.Format.Time));
            appsrc.SetProperty("is-live", new GLib.Value(true));
            var needDataDelegate = new Action<object, EventArgs>(PushData);
            appsrc.AddSignalHandler("need-data", needDataDelegate);

            Play();
            // https://searchcode.com/codesearch/view/11976789/
        }
    }
}
