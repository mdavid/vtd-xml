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
using com.ximpleware;
namespace com.ximpleware.xpath
{
	/// <summary> 
	/// This class implements the argumentList for FuncExpr
	/// which is basically a linked-list of expressions
	/// </summary>
	public class Alist
	{
		
		public Expr e;
		public Alist next;
		public Alist()
		{
			next = null;
		}
		public override System.String ToString()
		{
			Alist temp = this;
			System.String s = "";
			while (temp != null)
			{
				s = s + temp.e;
				temp = temp.next;
				if (temp != null)
					s = s + " ,";
			}
			return s;
		}
		
		public void  reset(VTDNav vn)
		{
			Alist temp = this;
			while (temp != null)
			{
				temp.e.reset(vn);
				temp = temp.next;
			}
		}
	}
}