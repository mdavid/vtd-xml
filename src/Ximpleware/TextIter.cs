/* 
* Copyright (C) 2002-2011 XimpleWare, info@ximpleware.com
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
using System;
namespace com.ximpleware
{
    /// <summary> This class iterates through all text nodes of an element.
    /// VTDNav has getText() which is inadequate for mixed content style of XML.
    /// text nodes include character_data and CDATA.
    /// Creation date: (12/5/03 5:53:41 PM)
    /// </summary>
    public class TextIter
    {
        /// <summary> Get the index vals for the text nodes in document order.
        /// Creation date: (12/5/03 6:11:50 PM)
        /// </summary>
        /// <returns> int  (-1 if no more left)
        /// </returns>
        /// <param name="action">int
        /// </param>
        /**
 * Get the index vals for the text nodes in document order.
 * Creation date: (12/5/03 6:11:50 PM)
 * @return int  (-1 if no more left)
 */
        public int getNext()
        {
            if (vn == null)
                throw new System.ArgumentException(" VTDNav instance can't be null");
            if (vn.shallowDepth)
            {
                switch (depth)
                {
                    case -1: return handleDocumentNode();
                    case 0:
                        return handleLevel0();
                    case 1:
                        return handleLevel1();
                    case 2:
                        return handleLevel2();
                    default:
                        return handleDefault();
                }
            }
            else
            {

                switch (depth)
                {

                    case -1:
                        return handleDocumentNode();
                    case 0:
                        return handleLevel0();
                    case 1:
                        return handleLevel1();
                    case 2:
                        return _handleLevel2();
                    case 3:
                        return handleLevel3();
                    case 4:
                        return handleLevel4();
                    default:
                        return handleDefault();
                }
            }
            //prevLocation = vtdSize-1;
            //return -1;
        }

        private int prevLocation; //previous location of text node
        protected internal int depth;
        protected internal int index; // this is index for the element

        protected internal VTDNav vn;

        private int lcIndex;
        private int lcLower;
        private int lcUpper;
        private int sel_type;
        private String piName;
        /// <summary> TextIter constructor comment.</summary>
        public TextIter()
            : base()
        {
            vn = null;
            vn = null;
            sel_type = 0;
            piName = null;
            /*sel_char_data = true;
            sel_comment = true;
            sel_cdata = true;*/
        }
        /// <summary> Test whether a give token type is a TEXT.
        /// Creation date: (12/11/03 3:46:10 PM)
        /// </summary>
        /// <returns> boolean
        /// </returns>
        /// <param name="type">int
        /// </param>
        private bool isText(int index)
        {
            int type = vn.getTokenType(index);
            if (sel_type == 0)
            {
                return (type == VTDNav.TOKEN_CHARACTER_DATA
                    // || type == vn.TOKEN_COMMENT
                || type == VTDNav.TOKEN_CDATA_VAL);
            }

            if (sel_type == 1)
            {
                return (type == VTDNav.TOKEN_COMMENT);
            }

            if (sel_type == 2)
                return (type == VTDNav.TOKEN_PI_NAME);
            try
            {
                return (vn.matchRawTokenString(index, piName));
            }
            catch (NavException e)
            {
                return false;
            }
        }
        /// <summary> Obtain the current navigation position and element info from VTDNav.
        /// So one can instantiate it once and use it for many different elements
        /// Creation date: (12/5/03 6:20:44 PM)
        /// </summary>
        /// <param name="vn">com.ximpleware.VTDNav
        /// </param>
        public void touch(VTDNav v)
        {
            if (v == null)
                throw new System.ArgumentException(" VTDNav instance can't be null");

            depth = v.context[0];
            if (depth == -1)
                index = 0;
            else
                index = (depth != 0) ? v.context[depth] : v.rootIndex;


            vn = v;
            prevLocation = -1;
            lcIndex = -1;
            lcUpper = -1;
            lcLower = -1;
        }

        private int increment(int sp)
        {

            int type = vn.getTokenType(sp);
            int vtdSize = vn.vtdBuffer.size_Renamed_Field;
            int i = sp + 1;
            while (i < vtdSize && depth == vn.getTokenDepth(i)
                && type == vn.getTokenType(i)
                && (vn.getTokenOffset(i - 1) + (int)((vn.vtdBuffer.longAt(i - 1) & VTDNav.MASK_TOKEN_FULL_LEN) >> 32)
                    == vn.getTokenOffset(i)))
            {
                i++;
            }
            return i;
        }

        
     /// <summary>
        /// Ask textIter to return character data or CDATA nodes
     /// </summary>        
        public void selectText()
        {
            sel_type = 0;
        }

        
         /// <summary>
         ///  Ask textIter to return comment nodes
         /// </summary>
        public void selectComment()
        {
            sel_type = 1;
        }

        /// <summary>
        /// Ask TextIter to return processing instruction name 
        ///  no value
        /// </summary>        
        public void selectPI0()
        {
            sel_type = 2;
        }

        /// <summary>
        /// Ask TextIter to return processing instruction of given name
        /// </summary>
        /// <param name="s"></param>
        public void selectPI1(String s)
        {
            sel_type = 3;
            piName = s;
        }

        private int handleDefault()
        {
            //int curDepth = vn.context[0];
            int sp = (prevLocation != -1) ? increment(prevLocation) : index + 1;
            if (sp >= vn.vtdSize) return -1;
            int d = vn.getTokenDepth(sp);
            int type = vn.getTokenType(sp);
            while (d >= depth
                && !(d == depth && type == VTDNav.TOKEN_STARTING_TAG))
            {
                if (isText(sp) == true && d == depth)
                {
                    prevLocation = sp;
                    return sp;
                }
                sp++;
                if (sp >= vn.vtdSize)
                    return -1;

                d = vn.getTokenDepth(sp);
                type = vn.getTokenType(sp);
            }
            return -1;
        }

        private int handleDocumentNode()
        {
            if (sel_type == 0)
                return -1;
            int sp = (prevLocation != -1) ? increment(prevLocation) : index + 1;
            if (sp >= vn.vtdSize) return -1;
            //int d = vn.getTokenDepth(sp);
            //int type = vn.getTokenType(sp);
            //while (d == -1/*&& !(d == depth && type == VTDNav.TOKEN_STARTING_TAG)*/) {
            while (true)
            {
                if (sp < vn.rootIndex)
                {
                    if (isText(sp))
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    else
                        sp++;
                }
                else
                {
                    // rewind to the end of document
                    if (sp == vn.rootIndex)
                    {
                        sp = vn.vtdSize - 1;
                        while (vn.getTokenDepth(sp) == -1)
                        {
                            sp--;
                        }
                        sp++;
                    }
                    if (sp >= vn.vtdSize)
                    {
                        return -1;
                    }
                    else if (isText(sp))
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    else
                        sp++;
                }

            }
        }

        private int handleLevel0()
        {
            // scan forward, if none found, jump to level 1 element and scan backward until one is found
            // if there isn't a level-one element, jump to the end of vtd buffer and scan backward

            int sp = (prevLocation != -1) ? increment(prevLocation) : index + 1;
            if (vn.l1Buffer.size_Renamed_Field != 0)
            {
                int temp1 = vn.l1Buffer.upper32At(0);
                int temp2 = vn.l1Buffer.upper32At(vn.l1Buffer.size_Renamed_Field - 1);
                lcIndex = (lcIndex != -1) ? lcIndex : 0;
                while (sp < vn.vtdSize)
                {
                    if (sp >= temp1 && sp < temp2)
                    {
                        int s = vn.l1Buffer.upper32At(lcIndex);
                        if (sp == s)
                        { // get to the next l1 element then do a rewind
                            lcIndex++;
                            sp = vn.l1Buffer.upper32At(lcIndex) - 1;
                            Boolean b = false;
                            while (vn.getTokenDepth(sp) == 0
                                /*&& vn.getTokenType(sp) != VTDNav.TOKEN_STARTING_TAG*/)
                            { //probe depth in here
                                b = true;
                                sp--;
                            }
                            if (b)
                                sp++; // point to the first possible node  
                        }
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 0)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 0)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        if (sp == temp2)
                        { // get to the end of the document and do a rewind
                            sp = vn.vtdBuffer.size_Renamed_Field - 1;
                            while (vn.getTokenDepth(sp) <= 0)
                            {
                                sp--;
                            }
                            sp++;
                            //continue;
                        }
                        if (sp >= vn.vtdSize)
                            return -1;
                        else if (isText(sp) == true && vn.getTokenDepth(sp) == 0)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        else if (vn.getTokenDepth(sp) > 1)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
                // found nothing
            }
            else
            {
                // no child element for root, just scan right forward
                while (sp < vn.vtdSize)
                {
                    if (isText(sp) == true && vn.getTokenDepth(sp) == 0)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                }
                return -1;
            }
        }

        private int handleLevel1()
        {
            int sp;
            if (prevLocation != -1)
            {
                sp = increment(prevLocation);
            }
            else
            {
                // fetch lclower and lcupper
                lcLower = vn.l1Buffer.lower32At(vn.l1index);
                if (lcLower != -1)
                {
                    lcUpper = vn.l2Buffer.size_Renamed_Field - 1;
                    int size = vn.l1Buffer.size_Renamed_Field;
                    for (int i = vn.l1index + 1; i < size; i++)
                    {
                        int temp = vn.l1Buffer.lower32At(i);
                        if (temp != -1)
                        {
                            lcUpper = temp - 1;
                            break;
                        }
                    }
                }
                sp = index + 1;
            } // check for l2lower and l2upper

            if (lcLower != -1)
            { // have at least one child element
                int temp1 = vn.l2Buffer.upper32At(lcLower);
                int temp2 = vn.l2Buffer.upper32At(lcUpper);
                lcIndex = (lcIndex != -1) ? lcIndex : lcLower;
                while (sp < vn.vtdSize)
                {
                    int s = vn.l2Buffer.upper32At(lcIndex);
                    if (sp >= temp1 && sp < temp2)
                    {
                        if (sp == s)
                        {
                            lcIndex++;
                            sp = vn.l2Buffer.upper32At(lcIndex) - 1;
                            //boolean b = false;
                            while (vn.getTokenDepth(sp) == 1)
                            {
                                //b = true;
                                sp--;
                            }
                            //if (b)
                            sp++;
                            //continue; 
                        }
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 1)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        //if (sp == temp2) { // last child element
                        //} else

                        if (isText(sp) == true && vn.getTokenDepth(sp) == 1)
                        {
                            //System.out.println("depth ->"+vn.getTokenDepth(sp));
                            prevLocation = sp;
                            return sp;
                        }
                        else if ((vn.getTokenType(sp) == VTDNav.TOKEN_STARTING_TAG
                                && vn.getTokenDepth(sp) < 2) || vn.getTokenDepth(sp) < 1)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
            }
            else
            { // no child element
                if (sp >= vn.vtdSize) return -1;
                int d = vn.getTokenDepth(sp);
                int type = vn.getTokenType(sp);
                while (sp < vn.vtdSize
                    && d >= 1
                    && !(d == 1 && type == VTDNav.TOKEN_STARTING_TAG))
                {
                    if (isText(sp) == true)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                    d = vn.getTokenDepth(sp);
                    type = vn.getTokenType(sp);

                }
                //prevLocation = vtdSize-1;
                return -1;
            }
        }

        private int handleLevel2()
        {
            int sp;
            if (prevLocation != -1)
            {
                sp = increment(prevLocation);
            }
            else
            {
                // fetch lclower and lcupper
                lcLower = vn.l2Buffer.lower32At(vn.l2index);
                if (lcLower != -1)
                {
                    lcUpper = vn.l3Buffer.size_Renamed_Field - 1;
                    int size = vn.l2Buffer.size_Renamed_Field;
                    for (int i = vn.l2index + 1; i < size; i++)
                    {
                        int temp = vn.l2Buffer.lower32At(i);
                        if (temp != -1)
                        {
                            lcUpper = temp - 1;
                            break;
                        }
                    }
                }
                sp = index + 1;
            } // check for l3lower and l3upper

            if (lcLower != -1)
            { // at least one child element
                int temp1 = vn.l3Buffer.intAt(lcLower);
                int temp2 = vn.l3Buffer.intAt(lcUpper);
                lcIndex = (lcIndex != -1) ? lcIndex : lcLower;
                while (sp < vn.vtdSize)
                {
                    int s = vn.l3Buffer.intAt(lcIndex);
                    //int s = vn.l2Buffer.upper32At(lcIndex);
                    if (sp >= temp1 && sp < temp2)
                    {
                        if (sp == s)
                        {
                            lcIndex++;
                            sp = vn.l3Buffer.intAt(lcIndex) - 1;
                            //boolean b = false;
                            while (vn.getTokenDepth(sp) == 2)
                            {
                                sp--;
                                //  b = true;
                            }
                            //if (b)
                            sp++;
                            //continue;
                        }
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        //if (sp == temp2) { // last child element
                        //} else                 
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        else if ((vn.getTokenType(sp) == VTDNav.TOKEN_STARTING_TAG
                                && vn.getTokenDepth(sp) < 3) || vn.getTokenDepth(sp) < 2)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
            }
            else
            { // no child elements
                if (sp >= vn.vtdSize) return -1;
                int d = vn.getTokenDepth(sp);
                int type = vn.getTokenType(sp);
                while (sp < vn.vtdSize
                    && d >= 2
                    && !(d == 2 && type == VTDNav.TOKEN_STARTING_TAG))
                {
                    // the last condition indicates the start of the next sibling element
                    if (isText(sp) == true && vn.getTokenDepth(sp) == 2)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                    d = vn.getTokenDepth(sp);
                    type = vn.getTokenType(sp);

                }
                //prevLocation = vtdSize-1;
                return -1;
            }
        }

        private int _handleLevel2()
        {
            int sp;
            VTDNav_L5 vnl = (VTDNav_L5)vn;
            if (prevLocation != -1)
            {
                sp = increment(prevLocation);
            }
            else
            {
                // fetch lclower and lcupper
                lcLower = vnl.l2Buffer.lower32At(vnl.l2index);
                if (lcLower != -1)
                {
                    lcUpper = vnl.l3Buffer.size_Renamed_Field - 1;
                    int size = vnl.l2Buffer.size_Renamed_Field;
                    for (int i = vnl.l2index + 1; i < size; i++)
                    {
                        int temp = vnl.l2Buffer.lower32At(i);
                        if (temp != 0xffffffff)
                        {
                            lcUpper = temp - 1;
                            break;
                        }
                    }
                }
                sp = index + 1;
            } // check for l3lower and l3upper

            if (lcLower != -1)
            { // at least one child element
                int temp1 = vnl.l3Buffer.upper32At(lcLower);
                int temp2 = vnl.l3Buffer.upper32At(lcUpper);
                lcIndex = (lcIndex != -1) ? lcIndex : lcLower;
                while (sp < vnl.vtdSize)
                {
                    int s = vnl.l3Buffer.upper32At(lcIndex);
                    //int s = vn.l2Buffer.upper32At(lcIndex);
                    if (sp >= temp1 && sp < temp2)
                    {
                        if (sp == s)
                        {
                            lcIndex++;
                            sp = vnl.l3Buffer.upper32At(lcIndex) - 1;
                            //boolean b = false;
                            while (vnl.getTokenDepth(sp) == 2)
                            {
                                sp--;
                                //  b = true;
                            }
                            //if (b)
                            sp++;
                            //continue;
                        }
                        if (isText(sp) == true && vnl.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true && vnl.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        //if (sp == temp2) { // last child element
                        //} else                 
                        if (isText(sp) == true && vnl.getTokenDepth(sp) == 2)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        else if ((vnl.getTokenType(sp) == VTDNav.TOKEN_STARTING_TAG
                                && vnl.getTokenDepth(sp) < 3) || vnl.getTokenDepth(sp) < 2)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
            }
            else
            { // no child elements
                if (sp >= vn.vtdSize) return -1;
                int d = vn.getTokenDepth(sp);
                int type = vn.getTokenType(sp);
                while (sp < vn.vtdSize
                    && d >= 2
                    && !(d == 2 && type == VTDNav.TOKEN_STARTING_TAG))
                {
                    // the last condition indicates the start of the next sibling element
                    if (isText(sp) == true && vn.getTokenDepth(sp) == 2)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                    d = vn.getTokenDepth(sp);
                    type = vn.getTokenType(sp);

                }
                //prevLocation = vtdSize-1;
                return -1;
            }
        }
        private int handleLevel3()
        {

            int sp;
            VTDNav_L5 vnl = (VTDNav_L5)vn;
            if (prevLocation != -1)
            {
                sp = increment(prevLocation);
            }
            else
            {
                // fetch lclower and lcupper
                lcLower = vnl.l3Buffer.lower32At(vnl.l3index);
                if (lcLower != -1)
                {
                    lcUpper = vnl.l4Buffer.size_Renamed_Field - 1;
                    int size = vnl.l3Buffer.size_Renamed_Field;
                    for (int i = vnl.l3index + 1; i < size; i++)
                    {
                        int temp = vnl.l3Buffer.lower32At(i);
                        if (temp != 0xffffffff)
                        {
                            lcUpper = temp - 1;
                            break;
                        }
                    }
                }
                sp = index + 1;
            } // check for l3lower and l3upper
            if (lcLower != -1)
            { // at least one child element
                int temp1 = vnl.l4Buffer.upper32At(lcLower);
                int temp2 = vnl.l4Buffer.upper32At(lcUpper);
                lcIndex = (lcIndex != -1) ? lcIndex : lcLower;
                while (sp < vn.vtdSize)
                {
                    int s = vnl.l4Buffer.upper32At(lcIndex);
                    //int s = vn.l2Buffer.upper32At(lcIndex);
                    if (sp >= temp1 && sp < temp2)
                    {
                        if (sp == s)
                        {
                            lcIndex++;
                            sp = vnl.l4Buffer.upper32At(lcIndex) - 1;
                            //boolean b = false;
                            while (vn.getTokenDepth(sp) == 2)
                            {
                                sp--;
                                //  b = true;
                            }
                            //if (b)
                            sp++;
                            //continue;
                        }
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 3)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 3)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        //if (sp == temp2) { // last child element
                        //} else                 
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 3)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        else if ((vn.getTokenType(sp) == VTDNav.TOKEN_STARTING_TAG
                                && vn.getTokenDepth(sp) < 4) || vn.getTokenDepth(sp) < 3)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
            }
            else
            { // no child elements
                if (sp >= vn.vtdSize) return -1;
                int d = vn.getTokenDepth(sp);
                int type = vn.getTokenType(sp);
                while (sp < vn.vtdSize
                    && d >= 3
                    && !(d == 3 && type == VTDNav.TOKEN_STARTING_TAG))
                {
                    // the last condition indicates the start of the next sibling element
                    if (isText(sp) == true && vn.getTokenDepth(sp) == 3)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                    d = vn.getTokenDepth(sp);
                    type = vn.getTokenType(sp);

                }
                //prevLocation = vtdSize-1;
                return -1;
            }

        }
        private int handleLevel4()
        { //l2
            int sp;
            VTDNav_L5 vnl = (VTDNav_L5)vn;
            if (prevLocation != -1)
            {
                sp = increment(prevLocation);
            }
            else
            {
                // fetch lclower and lcupper
                lcLower = vnl.l4Buffer.lower32At(vnl.l4index);
                if (lcLower != -1)
                {
                    lcUpper = vnl.l5Buffer.size_Renamed_Field - 1; //5
                    int size = vnl.l4Buffer.size_Renamed_Field; //4
                    for (int i = vnl.l4index + 1; i < size; i++)
                    {//4
                        int temp = vnl.l4Buffer.lower32At(i); //4
                        if (temp != 0xffffffff)
                        {
                            lcUpper = temp - 1;
                            break;
                        }
                    }
                }
                sp = index + 1;
            } // check for l3lower and l3upper

            if (lcLower != -1)
            { // at least one child element
                int temp1 = vnl.l5Buffer.intAt(lcLower);
                int temp2 = vnl.l5Buffer.intAt(lcUpper);
                lcIndex = (lcIndex != -1) ? lcIndex : lcLower;
                while (sp < vn.vtdSize)
                {
                    int s = vnl.l5Buffer.intAt(lcIndex);
                    //int s = vn.l2Buffer.upper32At(lcIndex);
                    if (sp >= temp1 && sp < temp2)
                    {
                        if (sp == s)
                        {
                            lcIndex++;
                            sp = vnl.l5Buffer.intAt(lcIndex) - 1;
                            //boolean b = false;
                            while (vn.getTokenDepth(sp) == 4)
                            {
                                sp--;
                                //  b = true;
                            }
                            //if (b)
                            sp++;
                            //continue;
                        }
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 4)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else if (sp < temp1)
                    {
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 4)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        sp++;
                    }
                    else
                    {
                        //if (sp == temp2) { // last child element
                        //} else                 
                        if (isText(sp) == true && vn.getTokenDepth(sp) == 4)
                        {
                            prevLocation = sp;
                            return sp;
                        }
                        else if ((vn.getTokenType(sp) == VTDNav.TOKEN_STARTING_TAG
                                && vn.getTokenDepth(sp) < 5) || vn.getTokenDepth(sp) < 4)
                        {
                            break;
                        }
                        sp++;
                    }
                }
                //prevLocation = vtdSize-1;
                return -1;
            }
            else
            { // no child elements
                if (sp >= vn.vtdSize) return -1;
                int d = vn.getTokenDepth(sp);
                int type = vn.getTokenType(sp);
                while (sp < vn.vtdSize
                    && d >= 4
                    && !(d == 4 && type == VTDNav.TOKEN_STARTING_TAG))
                {
                    // the last condition indicates the start of the next sibling element
                    if (isText(sp) == true && vn.getTokenDepth(sp) == 4)
                    {
                        prevLocation = sp;
                        return sp;
                    }
                    sp++;
                    d = vn.getTokenDepth(sp);
                    type = vn.getTokenType(sp);

                }
                //prevLocation = vtdSize-1;
                return -1;
            }
        }
    
    }
}