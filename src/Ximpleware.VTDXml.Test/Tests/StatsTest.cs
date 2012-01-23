
/* 
 * Copyright (C) 2002-2007 XimpleWare, info@ximpleware.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
/* 
 * In this C# program, we demonstrate how to process a complex
 * xml file describing DNA information
 * We are going to evaluate the xpath expression
 * "/bix/package/command/parlist"
 * then count the total number of child elements, and number of 
 * child elements with the name "par"
 */
using System;
using com.ximpleware;
using NUnit.Framework;

namespace Ximpleware.VTDXml
{
    public partial class Test
    {
        [Test]
        public static void BioInfoStats()
        {
            try
            {
                //File f = new File("bioinfo.xml");
                // counting child elements of parlist
                int count = 0;
                // counting child elements of parlist named "par"
                int par_count = 0;
                VTDGen vg = new VTDGen();
                if (vg.parseFile("./XmlDataFiles/bioinfo.xml", true))
                {
                    VTDNav vn = vg.getNav();
                    AutoPilot ap = new AutoPilot();
                    ap.bind(vn);
                    ap.selectXPath("/bix/package/command/parlist");
                    while (ap.evalXPath() != -1)
                        count++;

                    ap.selectXPath("/bix/package/command/parlist/par");
                    while (ap.evalXPath() != -1)
                        par_count++;

                    // print out the results
                    Console.WriteLine(" count ====> " + count);
                    Console.WriteLine(" par_count ==> " + par_count);

                    // verify results using iterators
                    int v = 0;
                    vn.toElement(VTDNav.ROOT);
                    ap = new AutoPilot(vn);
                    ap.selectElement("par");
                    while (ap.iterate())
                    {
                        if (vn.getCurrentDepth() == 4)
                        {
                            v++;
                        }
                    }
                    Console.WriteLine(" verify ==> " + v);
                }
            }
            catch (NavException e)
            {
                Console.WriteLine(" Exception during navigation " + e);
            }
            catch (XPathParseException e)
            {

            }
            catch (XPathEvalException e)
            {

            }
        }
    }
}
