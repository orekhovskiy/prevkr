using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNetCoreConsole.GstWorkers
{
    class MainWorker
    {
        private Gst.Pipeline _pipeline;
        private Gst.Element videomixer;
        private Gst.Element queue;
        private Gst.Element videoconvert;
        private Gst.Element x264enc;
        private Gst.Element flvmux;
        private Gst.Element queue1;
        private Gst.Element rtmpsink;
        public void Work()
        {
            _pipeline = new Gst.Pipeline("pipeline");
            videomixer = Gst.ElementFactory.Make("videomixer", "mix");
            queue = Gst.ElementFactory.Make("queue", "queue");
            videoconvert = Gst.ElementFactory.Make("videoconvert", "videoconvert");
            x264enc = Gst.ElementFactory.Make("x264enc", "x264enc");
            flvmux = Gst.ElementFactory.Make("flvmux", "flvmux");
            queue1 = Gst.ElementFactory.Make("queue", "queue1");
            rtmpsink = Gst.ElementFactory.Make("rtmpsink", "rtmpsink");
            flvmux.SetProperty("streamable", new GLib.Value(true));
            rtmpsink.SetProperty("location", new GLib.Value("rtmp://localhost/live"));

            _pipeline.Add(videomixer, queue, videoconvert, x264enc, flvmux, queue1, rtmpsink);
            if (!Gst.Element.Link(videomixer, queue, videoconvert, x264enc, flvmux, queue1, rtmpsink))
            {
                Log("Not all elements could be linked");
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void RequestPad()
        {

        }

        private void Log(object msg)
        {
            Debugger.Log(0, "", msg.ToString());
        }
    }
}
