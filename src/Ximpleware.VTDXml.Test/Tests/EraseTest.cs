using System.IO;
using System.Text;
using com.ximpleware;
using NUnit.Framework;

namespace Ximpleware.VTDXml
{
    partial class Test
    {
        [Test]
        public static void Erase()
        {
            VTDGen vg = new VTDGen();
            AutoPilot ap = new AutoPilot();
            Encoding eg = System.Text.Encoding.GetEncoding("utf-8");
            //ap.selectXPath("/*/*/*");
            AutoPilot ap2 = new AutoPilot();
            ap2.selectXPath("//@*");
            if (vg.parseFile("./XmlDataFiles/soap.xml", true))
            {
                FileStream fs = new FileStream("./XmlDataFiles/output.xml", System.IO.FileMode.OpenOrCreate);
                VTDNav vn = vg.getNav();
                ap.bind(vn);
                ap2.bind(vn);
                //ap.evalXPath();
                int i;
                while ((i = ap2.evalXPath()) != -1)
                {
                    //System.out.println("attr name ---> "+ i+ " "+vn.toString(i)+"  value ---> "+vn.toString(i+1));
                    vn.overWrite(i + 1, eg.GetBytes(""));
                }
                byte[] ba = vn.getXML().getBytes();
                fs.Write(ba, 0, ba.Length);
                fs.Close();
            }
        }
    }
}

