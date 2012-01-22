using System;
using com.ximpleware;

// This example shows you how to use NodeRecorder feature.
// Node recorder basically saves the internal representation
// of VTDNav into NodeRecorder's internal buffer.
// It is worthnoting that the amount of storage per node is 
// a strong function of structural complexity of the document
// and saving every node of a document into nodeRecorder is
// *not* a good idea!
using NUnit.Framework;

namespace Ximpleware.VTDXml
{
    partial class Test
    {
        [Test]
        public static void TestNodeRecorder()
        {
            VTDGen vg = new VTDGen();
            int i;
            if (vg.parseFile("./XmlTestFiles/po.xml", true))
            {
                // instantiate a node recorder here
                NodeRecorder nr = new NodeRecorder();
                AutoPilot ap = new AutoPilot();
                VTDNav vn = vg.getNav();
                ap.bind(vn);
                // bind node recorder to vn
                nr.bind(vn);
                ap.selectXPath("(/*/*/*)[position()=1 or position()=10]");
                while ((i = ap.evalXPath()) != -1)
                {
                    nr.record(); // save the selected nodes into nr
                }
                ap.resetXPath(); // a good practice
                nr.resetPointer(); // get into nr's read mode
                // iterating over the nodes recorded by nr
                while ((i = nr.iterate()) != -1)
                {
                    Console.WriteLine("string ==>" + vn.toString(i));
                }

                nr.clear(); //remove all the nodes in nr, buffer is however reused
            }
        }
    }
}

