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
using System;
namespace com.ximpleware
{
	/// <summary> This class is the base class of all the exceptions of autopilot.
	/// </summary>
	[Serializable]
	public class PilotException:NavException
	{
		/// <summary> PilotException constructor comment.</summary>
		public PilotException():base()
		{
		}
		/// <summary> PilotException constructor comment.</summary>
		/// <param name="s">java.lang.String
		/// </param>
		public PilotException(System.String s):base(s)
		{
		}
	}
}