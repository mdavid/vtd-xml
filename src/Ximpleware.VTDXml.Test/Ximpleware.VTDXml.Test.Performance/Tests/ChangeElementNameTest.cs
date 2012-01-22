using System;
using System.Text;
using com.ximpleware;
using NUnit.Framework;

namespace Ximpleware.Test
{
    partial class Performance
    {
        [Test]
        public static void ChangeElementName(String[] args)
        {

            String xml = "<aaaa> <bbbbb> <ccccc> </ccccc> <ccccc/> <ccccc></ccccc> </bbbbb> </aaaa>";
            Encoding eg = Encoding.GetEncoding("utf-8");
            VTDGen vg = new VTDGen();
            vg.setDoc(eg.GetBytes(xml));
            vg.parse(false);
            VTDNav vn = vg.getNav();
            AutoPilot ap = new AutoPilot(vn);
            ap.selectXPath("//*");
            XMLModifier xm = new XMLModifier(vn);
            while (ap.evalXPath() != -1)
            {
                xm.updateElementName("d:/lalalala");
            }
            xm.output("lala.xml");
        }
    }
}

