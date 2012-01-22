using System;
using com.ximpleware;
using NUnit.Framework;

//This example shows how to load a VTD+XML index
// into memory then run an XPath against it
// vg.loadIndex returns an instance of VTDNav
namespace Ximpleware.Test
{
    partial class Performance
    {
        [Test]
        public static void IndexLoad(string[] args)
        {
            try
            {
                VTDGen vg = new VTDGen();
                VTDNav vn = vg.loadIndex("po.vxl");
                AutoPilot ap = new AutoPilot(vn);
                ap.selectXPath("//items");
                int i;
                while ((i = ap.evalXPath()) != -1)
                {
                }
                ap.resetXPath();
            }
            catch (Exception e)
            {
            }
        }
    }
}

