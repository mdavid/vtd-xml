using System;
using com.ximpleware;

// This example shows you how to create an index
using NUnit.Framework;

namespace Ximpleware.VTDXml
{
    partial class Test
    {
        [Test]
        public static void IndexWrite()
        {
            try
            {
                VTDGen vg = new VTDGen();
                if (vg.parseFile("./XmlDataFiles/po.xml", true))
                {
                    vg.writeIndex("./XmlDataFiles/po.vxl");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}

