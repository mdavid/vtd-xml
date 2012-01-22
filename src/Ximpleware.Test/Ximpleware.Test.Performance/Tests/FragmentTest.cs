using System;
using com.ximpleware;
using NUnit.Framework;

namespace Ximpleware.Test
{

		partial class Performance
		{
				[Test]
				public static void FragmentTest(String[] args)
				{
						{
								// instantiate VTDGen and XMLModifier
								VTDGen vg = new VTDGen();
								XMLModifier xm = new XMLModifier();
								AutoPilot ap = new AutoPilot();
								AutoPilot ap2 = new AutoPilot();
								ap.selectXPath("(/*/*/*)[position()>1 and position()<4]");
								ap2.selectXPath("/*/*/*");
								if (vg.parseFile("soap2.xml", true))
								{
										VTDNav vn = vg.getNav();
										xm.bind(vn);
										ap2.bind(vn);
										ap.bind(vn);
										ap2.evalXPath();
										ElementFragmentNs ef = vn.getElementFragmentNs();
										int i = -1;
										while ((i = ap.evalXPath()) != -1)
										{
												xm.insertAfterElement(ef);
										}
										xm.output("new_soap.xml");
								}
						}
				}
		}
}
