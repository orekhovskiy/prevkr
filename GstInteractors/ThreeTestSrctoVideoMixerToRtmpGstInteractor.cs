using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNetCoreConsole.GstInteractors
{
    class ThreeTestSrctoVideoMixerToRtmpGstInteractor : AbstractGstInteractor
    {
        public override void Interact()
        {
            var source1 = Gst.ElementFactory.Make("videotestsrc", "source1");
            var capsfilter1 = Gst.ElementFactory.Make("capsfilter", "capsfilter1");
            var filtercaps1 = Gst.Global.CapsFromString("video/x-raw, width=200, height=100");
            capsfilter1.SetProperty("caps", new GLib.Value(filtercaps1));
            var alpha1 = Gst.ElementFactory.Make("alpha", "alpha1");
            alpha1.SetProperty("alpha", new GLib.Value(1.0));
            var videobox1 = Gst.ElementFactory.Make("videobox", "videobox1");
            //videobox1.SetProperty("bottom", new GLib.Value(-100));
            var videomixer = Gst.ElementFactory.Make("videomixer", "mix");
            var queue = Gst.ElementFactory.Make("queue", "queue");
            var videoconvert = Gst.ElementFactory.Make("videoconvert", "videoconvert");
            var x264enc = Gst.ElementFactory.Make("x264enc", "x264enc");
            var flvmux = Gst.ElementFactory.Make("flvmux", "flvmux");
            var queue1 = Gst.ElementFactory.Make("queue", "queue1");
            var rtmpsink = Gst.ElementFactory.Make("rtmpsink", "rtmpsink");

            var source2 = Gst.ElementFactory.Make("videotestsrc", "source2");
            var capsfilter2 = Gst.ElementFactory.Make("capsfilter", "capsfilter2");
            var filtercaps2 = Gst.Global.CapsFromString("video/x-raw, width=200, height=100");
            capsfilter2.SetProperty("caps", new GLib.Value(filtercaps2));
            var alpha2 = Gst.ElementFactory.Make("alpha", "alpha2");
            alpha2.SetProperty("alpha", new GLib.Value(1.0));
            var videobox2 = Gst.ElementFactory.Make("videobox", "videobox2");
            //.SetProperty("top", new GLib.Value(-100));

            flvmux.SetProperty("streamable", new GLib.Value(true));
            rtmpsink.SetProperty("location", new GLib.Value("rtmp://localhost/live"));

            _pipeline.Add(source1, capsfilter1, alpha1, videobox1, videomixer,  queue, videoconvert, x264enc, flvmux, queue1, rtmpsink);
            if (!Gst.Element.Link(source1, capsfilter1, alpha1, videobox1, videomixer,  queue, videoconvert, x264enc, flvmux, queue1, rtmpsink))
            {
                Log("Not all elements could be linked");
            }

            _pipeline.Add(source2, capsfilter2, alpha2, videobox2);
            if (!Gst.Element.Link(source2, capsfilter2, alpha2, videobox2))
            {
                Log("Not all elements could be linked");
            }

            var mixerSinkPadTemplate1 = videomixer.GetPadTemplate("sink_%u");
            var mixerSinkPadTemplate2 = videomixer.GetPadTemplate("sink_%u");
            var mixerSinkPad2 = videomixer.RequestPad(mixerSinkPadTemplate2);
            mixerSinkPad2.SetProperty("ypos", new GLib.Value(100));
            mixerSinkPad2.SetProperty("xpos", new GLib.Value(0));
            var srcpad2 = videobox2.GetStaticPad("src");
            srcpad2.Link(mixerSinkPad2);

            Play(); 
        }
    }
}
