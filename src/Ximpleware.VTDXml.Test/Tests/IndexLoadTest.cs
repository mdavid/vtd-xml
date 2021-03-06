using System;
using com.ximpleware;
using NUnit.Framework;

//This example shows how to load a VTD+XML index
// into memory then run an XPath against it
// vg.loadIndex returns an instance of VTDNav
namespace Ximpleware.VTDXml
{
    partial class Test
    {
        [Test]
        public static void IndexLoad()
        {
            try
            {
                VTDGen vg = new VTDGen();
                VTDNav vn = vg.loadIndex("./XmlDataFiles/po.vxl");
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

