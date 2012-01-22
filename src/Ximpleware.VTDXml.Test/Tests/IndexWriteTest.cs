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
                if (vg.parseFile("./XmlTestFiles/po.xml", true))
                {
                    vg.writeIndex("./XmlTestFiles/po.vxl");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}

