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
        private Gst.Element _videomixer;
        private Gst.Element _queue;
        private Gst.Element _videoconvert;
        private Gst.Element _x264enc;
        private Gst.Element _flvmux;
        private Gst.Element _queue1;
        private Gst.Element _rtmpsink;
        public void Work()
        {
            _pipeline = new Gst.Pipeline("pipeline");
            _videomixer = Gst.ElementFactory.Make("videomixer", "mix");
            _queue = Gst.ElementFactory.Make("queue", "queue");
            _videoconvert = Gst.ElementFactory.Make("videoconvert", "videoconvert");
            _x264enc = Gst.ElementFactory.Make("x264enc", "x264enc");
            _flvmux = Gst.ElementFactory.Make("flvmux", "flvmux");
            _queue1 = Gst.ElementFactory.Make("queue", "queue1");
            _rtmpsink = Gst.ElementFactory.Make("rtmpsink", "rtmpsink");
            _flvmux.SetProperty("streamable", new GLib.Value(true));
            _rtmpsink.SetProperty("location", new GLib.Value("rtmp://localhost/live"));

            _pipeline.Add(_videomixer, _queue, _videoconvert, _x264enc, _flvmux, _queue1, _rtmpsink);
            if (!Gst.Element.Link(_videomixer, _queue, _videoconvert, _x264enc, _flvmux, _queue1, _rtmpsink))
            {
                Log("Not all elements could be linked");
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Gst.Pad RequestPad()
        {
            var mixerSinkPadTemplate = _videomixer.GetPadTemplate("sink_%u");
            var pad = _videomixer.RequestPad(mixerSinkPadTemplate);
            return pad;
        }

        private void Log(object msg)
        {
            Debugger.Log(0, "", msg.ToString());
        }
    }
}
