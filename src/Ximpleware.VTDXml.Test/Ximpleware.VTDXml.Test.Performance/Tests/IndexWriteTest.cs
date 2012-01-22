using System;
using com.ximpleware;

// This example shows you how to create an index
using NUnit.Framework;

namespace Ximpleware.Test
{
    partial class Performance
    {
        [Test]
        public static void IndexWrite(string[] args)
        {
            try
            {
                VTDGen vg = new VTDGen();
                if (vg.parseFile("po.xml", true))
                {
                    vg.writeIndex("po.vxl");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}

