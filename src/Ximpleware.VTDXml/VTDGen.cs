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
using com.ximpleware.parser;

namespace com.ximpleware
{

		//import java.io.*;
		/// <summary> VTD Generator implementation.
		/// Current support built-in entities only
		/// It parses DTD, but doesn't resolve declared entities
		/// </summary>
		public class VTDGen
		{
				/// <summary> The entity ignorant version of getCharAfterS.</summary>
				/// <returns> int
				/// </returns>
				/// <throws>  ParseException  </throws>
				/// <throws>  EncodingException  </throws>
				/// <throws>  com.ximpleware.EOFException  </throws>
				private int CharAfterS
				{
						get
						{
								int n;
								do
								{
										n = r.Char;
										if (n == ' ' || n == '\n' || n == '\t' || n == '\r')
										{
										}
										else
												return n;
										n = r.Char;
										if (n == ' ' || n == '\n' || n == '\t' || n == '\r')
										{
										}
										else
												return n;
								} while (true);
								//throw new EOFException("should never come here");
						}

				}
				/// <summary> The entity aware version of getCharAfterS</summary>
				/// <returns> int
				/// </returns>
				/// <throws>  ParseException Super class for any exception during parsing. </throws>
				/// <throws>  EncodingException UTF/native encoding exception. </throws>
				/// <throws>  com.ximpleware.EOFException End of file exception. </throws>
				//private int CharAfterSe
				//{
				//    get
				//    {
				//        int n = 0;
				//        int temp; //offset saver
				//        while (true)
				//        {
				//            n = r.Char;
				//            if (!XMLChar.isSpaceChar(n))
				//            {
				//                if (n != '&')
				//                    return n;
				//                else
				//                {
				//                    temp = offset;
				//                    if (!XMLChar.isSpaceChar(entityIdentifier()))
				//                    {
				//                        offset = temp; // rewind
				//                        return '&';
				//                    }
				//                }
				//            }
				//        }
				//    }

				//}
				/// <summary> This method returns the VTDNav object after parsing, it also cleans 
				/// internal state so VTDGen can process the next file.
				/// </summary>
				/// <returns> com.ximpleware.VTDNav
				/// </returns>
				public VTDNav getNav()
				{
						// call VTDNav constructor
						VTDNav vn;
						if (shallowDepth)
								vn = new VTDNav(rootIndex, encoding, ns, VTDDepth,
												new UniByteBuffer(XMLDoc), VTDBuffer, l1Buffer, l2Buffer,
												l3Buffer, docOffset, docLen);
						else
								vn = new VTDNav_L5(rootIndex, encoding, ns, VTDDepth,
												new UniByteBuffer(XMLDoc), VTDBuffer, l1Buffer, l2Buffer,
												_l3Buffer, _l4Buffer, _l5Buffer, docOffset, docLen);
						clear();
						return vn;


				}
				/// <summary> Get the offset value of previous character.</summary>
				/// <returns> int
				/// </returns>
				/// <throws>  ParseException Super class for exceptions during parsing. </throws>
				private int PrevOffset
				{
						get
						{
								int prevOffset = offset;
								int temp;
								switch (encoding)
								{

										case FORMAT_UTF8:
												do
												{
														prevOffset--;
												}
												while ((XMLDoc[prevOffset]) > 127
														&& ((XMLDoc[prevOffset] & ((byte)0xc0)) == (byte)0x80));
												return prevOffset;

										case FORMAT_ASCII:
										case FORMAT_ISO_8859_1:
										case FORMAT_ISO_8859_2:
										case FORMAT_ISO_8859_3:
										case FORMAT_ISO_8859_4:
										case FORMAT_ISO_8859_5:
										case FORMAT_ISO_8859_6:
										case FORMAT_ISO_8859_7:
										case FORMAT_ISO_8859_8:
										case FORMAT_ISO_8859_9:
										case FORMAT_ISO_8859_10:
										case FORMAT_WIN_1250:
										case FORMAT_WIN_1251:
										case FORMAT_WIN_1252:
										case FORMAT_WIN_1253:
										case FORMAT_WIN_1254:
										case FORMAT_WIN_1255:
										case FORMAT_WIN_1256:
										case FORMAT_WIN_1257:
										case FORMAT_WIN_1258:
												return offset - 1;

										case FORMAT_UTF_16LE:
												temp = (XMLDoc[offset + 1]) << 8 | (XMLDoc[offset]);
												if (temp < 0xd800 || temp > 0xdfff)
												{
														return offset - 2;
												}
												else
														return offset - 4;
										//goto case FORMAT_UTF_16BE;

										case FORMAT_UTF_16BE:
												temp = (XMLDoc[offset]) << 8 | (XMLDoc[offset + 1]);
												if (temp < 0xd800 || temp > 0xdfff)
												{
														return offset - 2;
												}
												else
														return offset - 4;
										//goto default;

										default:
												throw new ParseException("Other Error: Should never happen");

								}
						}

				}
				// internal parser state

				private const int STATE_LT_SEEN = 0; // encounter the first <
				private const int STATE_START_TAG = 1;
				private const int STATE_END_TAG = 2;
				private const int STATE_ATTR_NAME = 3;
				private const int STATE_ATTR_VAL = 4;
				private const int STATE_TEXT = 5;
				private const int STATE_DOC_START = 6; // beginning of document
				private const int STATE_DOC_END = 7; // end of document 
				private const int STATE_PI_TAG = 8;
				private const int STATE_PI_VAL = 9;
				private const int STATE_DEC_ATTR_NAME = 10;
				private const int STATE_COMMENT = 11;
				private const int STATE_CDATA = 12;
				private const int STATE_DOCTYPE = 13;
				private const int STATE_END_COMMENT = 14;
				// comment appear after the last ending tag
				private const int STATE_END_PI = 15;
				//private  static int STATE_END_PI_VAL = 17;

				// token type
				public const int TOKEN_STARTING_TAG = 0;
				public const int TOKEN_ENDING_TAG = 1;
				public const int TOKEN_ATTR_NAME = 2;
				public const int TOKEN_ATTR_NS = 3;
				public const int TOKEN_ATTR_VAL = 4;
				public const int TOKEN_CHARACTER_DATA = 5;
				public const int TOKEN_COMMENT = 6;
				public const int TOKEN_PI_NAME = 7;
				public const int TOKEN_PI_VAL = 8;
				public const int TOKEN_DEC_ATTR_NAME = 9;
				public const int TOKEN_DEC_ATTR_VAL = 10;
				public const int TOKEN_CDATA_VAL = 11;
				public const int TOKEN_DTD_VAL = 12;
				public const int TOKEN_DOCUMENT = 13;

				// encoding format
				public const int FORMAT_UTF8 = 2;
				public const int FORMAT_ASCII = 0;
				public const int FORMAT_ISO_8859_1 = 1;
				public const int FORMAT_ISO_8859_2 = 3;
				public const int FORMAT_ISO_8859_3 = 4;
				public const int FORMAT_ISO_8859_4 = 5;
				public const int FORMAT_ISO_8859_5 = 6;
				public const int FORMAT_ISO_8859_6 = 7;
				public const int FORMAT_ISO_8859_7 = 8;
				public const int FORMAT_ISO_8859_8 = 9;
				public const int FORMAT_ISO_8859_9 = 10;
				public const int FORMAT_ISO_8859_10 = 11;
				public const int FORMAT_ISO_8859_11 = 12;
				public const int FORMAT_ISO_8859_12 = 13;
				public const int FORMAT_ISO_8859_13 = 14;
				public const int FORMAT_ISO_8859_14 = 15;
				public const int FORMAT_ISO_8859_15 = 16;
				public const int FORMAT_ISO_8859_16 = 17;

				public const int FORMAT_WIN_1250 = 18;
				public const int FORMAT_WIN_1251 = 19;
				public const int FORMAT_WIN_1252 = 20;
				public const int FORMAT_WIN_1253 = 21;
				public const int FORMAT_WIN_1254 = 22;
				public const int FORMAT_WIN_1255 = 23;
				public const int FORMAT_WIN_1256 = 24;
				public const int FORMAT_WIN_1257 = 25;
				public const int FORMAT_WIN_1258 = 26;


				public const int FORMAT_UTF_16LE = 64;
				public const int FORMAT_UTF_16BE = 63;




				//namespace aware flag
				protected internal bool ns, is_ns;
				protected internal int VTDDepth; // Maximum Depth of VTDs
				protected internal int encoding;
				private int last_depth;
				private int last_l1_index;
				private int last_l2_index;
				private int last_l3_index;
				//private int last_l3_index;
				private int last_l4_index;
				private int increment;
				private bool BOM_detected;
				private bool must_utf_8;
				private int ch;
				private int ch_temp;
				private int length1, length2;

				protected internal int offset; // this is byte offset, not char offset as encoded in VTD
				private bool ws; // to prserve whitespace or not, default to false
				private int temp_offset;
				protected internal int depth;


				protected internal int prev_offset;
				protected internal int rootIndex;
				protected internal byte[] XMLDoc;
				protected internal FastLongBuffer VTDBuffer;
				protected internal FastLongBuffer l1Buffer;
				protected internal FastLongBuffer l2Buffer;
				protected internal FastIntBuffer l3Buffer;
				protected internal FastLongBuffer _l3Buffer;
				protected internal FastLongBuffer _l4Buffer;
				protected internal FastIntBuffer _l5Buffer;


				protected FastIntBuffer nsBuffer1;
				protected FastLongBuffer nsBuffer2;
				protected FastLongBuffer nsBuffer3;
				protected long currentElementRecord;


				protected internal bool br; //buffer reuse


				protected internal int docLen;
				// again, in terms of byte, not char as encoded in VTD
				protected internal int endOffset;
				protected internal long[] tag_stack;
				private long[] attr_name_array;
				private int attr_count;
				private long[] prefixed_attr_name_array;
				private int[] prefix_URL_array;
				private int prefixed_attr_count;
				public const int MAX_DEPTH = 254; // maximum depth value
				protected internal int docOffset;

				// attr_name_array size
				private const int ATTR_NAME_ARRAY_SIZE = 16;
				// tag_stack size
				private const int TAG_STACK_SIZE = 256;
				// max prefix length
				public const int MAX_PREFIX_LENGTH = (1 << 9) - 1;
				// max Qname length
				public const int MAX_QNAME_LENGTH = (1 << 11) - 1;
				// max Token length
				public const int MAX_TOKEN_LENGTH = (1 << 20) - 1;

				public EOFException e = null;

				protected short LcDepth;
				protected bool singleByteEncoding;
				protected internal bool shallowDepth; // true if lc depth is 3


				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_2Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_2Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_2.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_2Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_2.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}


						public long _getChar(int offset)
						{
								int c = ISO8859_2.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_2.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_3Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_3Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_3.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_3Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_3.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_3.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_3.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_4Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_4Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_4.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_4Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_4.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_4.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_4.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_5Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_5Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_5.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_5Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_5.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_5.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_5.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_6Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_6Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_6.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_6Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_6.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_6.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_6.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_7Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_7Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_7.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_7Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_7.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_7.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_7.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_8Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_8Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_8.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_8Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_8.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = ISO8859_8.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_8.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_9Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				internal class ISO8859_9Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_9.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_9Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_9.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_9.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return ISO8859_9.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859_10Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class ISO8859_10Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_10.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_10Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_10.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = ISO8859_10.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}
						public char decode(int offset)
						{
								return ISO8859_10.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				sealed internal class ISO8859_11Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_11.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_11Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_11.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = ISO8859_11.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}
						public char decode(int offset)
						{
								return ISO8859_11.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				sealed internal class ISO8859_13Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_13.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_13Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_13.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = ISO8859_13.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}
						public char decode(int offset)
						{
								return ISO8859_13.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				sealed internal class ISO8859_14Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_14.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_14Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_14.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = ISO8859_14.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}
						public char decode(int offset)
						{
								return ISO8859_14.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				sealed internal class ISO8859_15Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return ISO8859_15.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_15Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == ISO8859_15.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = ISO8859_15.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}
						public char decode(int offset)
						{
								return ISO8859_15.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'UTF8Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class UTF8Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{
										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										int temp = Enclosing_Instance.XMLDoc[Enclosing_Instance.offset];
										//int a = 0, c = 0, d = 0, val = 0;
										if (temp < 128)
										{
												Enclosing_Instance.offset++;
												return temp;
										}
										return handleUTF8(temp);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public UTF8Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						private int handleUTF8(int temp)
						{
								int val, c, d, a, i;
								//temp = temp & 0xff;
								switch (UTF8Char.byteCount(temp))
								{

										// handle multi-byte code
										case 2:
												c = 0x1f;
												// A mask determine the val portion of the first byte
												d = 6; // 
												a = 1; //
												break;

										case 3:
												c = 0x0f;
												d = 12;
												a = 2;
												break;

										case 4:
												c = 0x07;
												d = 18;
												a = 3;
												break;

										case 5:
												c = 0x03;
												d = 24;
												a = 4;
												break;

										case 6:
												c = 0x01;
												d = 30;
												a = 5;
												break;

										default:
												throw new ParseException("UTF 8 encoding error: should never happen");

								}
								val = (temp & c) << d;
								i = a - 1;
								while (i >= 0)
								{
										temp = Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + a - i];
										if ((temp & 0xc0) != 0x80)
												throw new ParseException("UTF 8 encoding error: should never happen");
										val = val | ((temp & 0x3f) << ((i << 2) + (i << 1)));
										i--;
								}
								Enclosing_Instance.offset += a + 1;
								return val;
						}
						public bool skipChar(int ch)
						{
								//int a = 0, d = 0, val = 0;
								int temp = Enclosing_Instance.XMLDoc[Enclosing_Instance.offset];
								if (temp < 128)
										if (ch == temp)
										{
												Enclosing_Instance.offset++;
												return true;
										}
										else
										{
												return false;
										}
								return skipUTF8(temp, ch);
						}
						private bool skipUTF8(int temp, int ch)
						{
								int val, c, d, a, i;
								//temp = temp & 0xff;
								switch (UTF8Char.byteCount(temp))
								{

										// handle multi-byte code
										case 2:
												c = 0x1f;
												// A mask determine the val portion of the first byte
												d = 6; // 
												a = 1; //
												break;

										case 3:
												c = 0x0f;
												d = 12;
												a = 2;
												break;

										case 4:
												c = 0x07;
												d = 18;
												a = 3;
												break;

										case 5:
												c = 0x03;
												d = 24;
												a = 4;
												break;

										case 6:
												c = 0x01;
												d = 30;
												a = 5;
												break;

										default:
												throw new ParseException("UTF 8 encoding error: should never happen");

								}
								val = (temp & c) << d;
								i = a - 1;
								while (i >= 0)
								{
										temp = Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + a - i];
										if ((temp & 0xc0) != 0x80)
												throw new ParseException("UTF 8 encoding error: should never happen");
										val = val | ((temp & 0x3f) << ((i << 2) + (i << 1)));
										i--;
								}
								if (val == ch)
								{
										Enclosing_Instance.offset += a + 1;
										return true;
								}
								else
										return false;
						}

						public long _getChar(int offset)
						{
								int temp = Enclosing_Instance.XMLDoc[offset];
								if (temp >= 0)
								{
										if (temp == '\r')
										{
												if (Enclosing_Instance.XMLDoc[offset + 1] == '\n')
												{
														return '\n' | (2L << 32);
												}
												else
												{
														return '\n' | (1L << 32);
												}
										}
										//currentOffset++;
										return temp | (1L << 32);
								}
								return handle_utf8(temp, offset);

						}

						private long handle_utf8(int temp, int offset)
						{
								// TODO Auto-generated method stub
								int c = 0, d = 0, a = 0;

								long val;
								switch (UTF8Char.byteCount((int)temp & 0xff))
								{
										case 2:
												c = 0x1f;
												d = 6;
												a = 1;
												break;
										case 3:
												c = 0x0f;
												d = 12;
												a = 2;
												break;
										case 4:
												c = 0x07;
												d = 18;
												a = 3;
												break;
										case 5:
												c = 0x03;
												d = 24;
												a = 4;
												break;
										case 6:
												c = 0x01;
												d = 30;
												a = 5;
												break;
								}

								val = (temp & c) << d;
								int i = a - 1;
								while (i >= 0)
								{
										temp = Enclosing_Instance.XMLDoc[offset + a - i];
										val = val | ((temp & 0x3f) << ((i << 2) + (i << 1)));
										i--;
								}
								//currentOffset += a + 1;
								return val | (((long)(a + 1)) << 32);
						}

						public char decode(int offset)
						{
								return (char)0;
						}


				}



				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'UTF16BEReader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class UTF16BEReader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{
										int val = 0;
										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										int temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 1]);
										if ((temp < 0xd800) || (temp > 0xdfff))
										{
												// not a high surrogate
												Enclosing_Instance.offset += 2;
												return temp;
										}
										else
										{
												if (temp < 0xd800 || temp > 0xdbff)
														throw new EncodingException("UTF 16 BE encoding error: should never happen");
												val = temp;
												temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 2]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 3]);
												if (temp < 0xdc00 || temp > 0xdfff)
												{
														// has to be a low surrogate here
														throw new EncodingException("UTF 16 BE encoding error: should never happen");
												}
												val = ((val - 0xd800) << 10) + (temp - 0xdc00) + 0x10000;
												Enclosing_Instance.offset += 4;
												return val;
										}
								}



						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public UTF16BEReader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								// implement UTF-16BE to UCS4 conversion
								int temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 1]);
								if ((temp < 0xd800) || (temp > 0xdfff))
								{
										// not a high surrogate
										//offset += 2;
										if (temp == ch)
										{
												Enclosing_Instance.offset += 2;
												return true;
										}
										else
												return false;
								}
								else
								{
										if (temp < 0xd800 || temp > 0xdbff)
												throw new EncodingException("UTF 16 BE encoding error: should never happen");
										int val = temp;
										temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 2]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 3]);
										if (temp < 0xdc00 || temp > 0xdfff)
										{
												// has to be a low surrogate here
												throw new EncodingException("UTF 16 BE encoding error: should never happen");
										}
										val = ((val - 0xd800) << 10) + (temp - 0xdc00) + 0x10000;
										if (val == ch)
										{
												Enclosing_Instance.offset += 4;
												return true;
										}
										else
												return false;
								}
						}

						public char decode(int offset)
						{
								return (char)0;
						}

						public long _getChar(int offset)
						{
								long val;

								int temp =
										((Enclosing_Instance.XMLDoc[offset] & 0xff) << 8)
														| (Enclosing_Instance.XMLDoc[offset + 1] & 0xff);
								if ((temp < 0xd800)
										|| (temp > 0xdfff))
								{ // not a high surrogate
										if (temp == '\r')
										{
												if (Enclosing_Instance.XMLDoc[offset + 3] == '\n'
														&& Enclosing_Instance.XMLDoc[offset + 2] == 0)
												{

														return '\n' | (4L << 32);
												}
												else
												{
														return '\n' | (2L << 32);
												}
										}
										//currentOffset++;
										return temp | (2L << 32);
								}
								else
								{
										val = temp;
										temp =
												((Enclosing_Instance.XMLDoc[offset + 2] & 0xff)
														<< 8) | (Enclosing_Instance.XMLDoc[offset + 3] & 0xff);
										val = ((temp - 0xd800) << 10) + (val - 0xdc00) + 0x10000;
										//currentOffset += 2;
										return val | (4L << 32);
								}
						}
				}



				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1250Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1250Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1250.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1250Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1250.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = WIN1250.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1250.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1251Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1251Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1251.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1251Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1251.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = WIN1251.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1251.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}




				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1252Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1252Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1252.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1252Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1252.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = WIN1252.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1252.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1253Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1253Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1253.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1253Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1253.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = WIN1253.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1253.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1254Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1254Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1254.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1254Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1254.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = WIN1254.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1254.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1255Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1255Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1255.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1255Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1255.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = WIN1255.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1255.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1256Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1256Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1256.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1256Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1256.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = WIN1256.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1256.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1257Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1257Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1257.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1257Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1257.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}
						public long _getChar(int offset)
						{
								int c = WIN1257.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1257.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'WIN1258Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class WIN1258Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return WIN1258.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++]);
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public WIN1258Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == WIN1258.decode(Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]))
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = WIN1258.decode(Enclosing_Instance.XMLDoc[offset]);
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return WIN1258.decode(Enclosing_Instance.XMLDoc[offset]);
						}
				}
				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'UTF16LEReader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class UTF16LEReader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{
										int val = 0;
										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										int temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 1]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]);
										if (temp < 0xd800 || temp > 0xdfff)
										{
												// check for low surrogate
												Enclosing_Instance.offset += 2;
												return temp;
										}
										else
										{
												if (temp < 0xd800 || temp > 0xdbff)
														throw new EncodingException("UTF 16 LE encoding error: should never happen");
												val = temp;
												temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 3]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 2]);
												if (temp < 0xdc00 || temp > 0xdfff)
												{
														// has to be high surrogate
														throw new EncodingException("UTF 16 LE encoding error: should never happen");
												}
												val = ((val - 0xd800) << 10) + (temp - 0xdc00) + 0x10000;
												Enclosing_Instance.offset += 4;
												return val;
										}
								}


						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}

						public UTF16LEReader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{

								int temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 1]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset]);
								if (temp < 0xd800 || temp > 0xdfff)
								{
										// check for low surrogate
										if (temp == ch)
										{
												Enclosing_Instance.offset += 2;
												return true;
										}
										else
										{
												return false;
										}
								}
								else
								{
										if (temp < 0xd800 || temp > 0xdbff)
												throw new EncodingException("UTF 16 LE encoding error: should never happen");
										int val = temp;
										temp = (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 3]) << 8 | (Enclosing_Instance.XMLDoc[Enclosing_Instance.offset + 2]);
										if (temp < 0xdc00 || temp > 0xdfff)
										{
												// has to be high surrogate
												throw new EncodingException("UTF 16 LE encoding error: should never happen");
										}
										val = ((val - 0xd800) << 10) + (temp - 0xdc00) + 0x10000;
										if (val == ch)
										{
												Enclosing_Instance.offset += 4;
												return true;
										}
										else
												return false;
								}
						}
						public char decode(int offset)
						{
								return (char)0;
						}

						public long _getChar(int offset)
						{
								// implement UTF-16LE to UCS4 conversion
								int val, temp =
										(Enclosing_Instance.XMLDoc[offset + 1] & 0xff)
												<< 8 | (Enclosing_Instance.XMLDoc[offset] & 0xff);
								if (temp < 0xdc00 || temp > 0xdfff)
								{ // check for low surrogate
										if (temp == '\r')
										{
												if (Enclosing_Instance.XMLDoc[offset + 2] == '\n'
														&& Enclosing_Instance.XMLDoc[offset + 3] == 0)
												{
														return '\n' | (4L << 32);
												}
												else
												{
														return '\n' | (2L << 32);
												}
										}
										return temp | (2L << 32);
								}
								else
								{
										val = temp;
										temp =
												(Enclosing_Instance.XMLDoc[offset + 3] & 0xff)
														<< 8 | (Enclosing_Instance.XMLDoc[offset + 2] & 0xff);
										val = ((temp - 0xd800) << 10) + (val - 0xdc00) + 0x10000;

										return val | (4L << 32);
								}
						}
				}

				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ASCIIReader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class ASCIIReader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{
										int a;
										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										a = Enclosing_Instance.XMLDoc[Enclosing_Instance.offset];
										if (a > 127)
												throw new EncodingException("ASCII encoding error: invalid ASCII char");
										Enclosing_Instance.offset++;
										return a;
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ASCIIReader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{

								if (ch == Enclosing_Instance.XMLDoc[Enclosing_Instance.offset])
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = Enclosing_Instance.XMLDoc[offset];
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return (char)Enclosing_Instance.XMLDoc[offset];
						}
				}
				//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'ISO8859Reader' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
				sealed internal class ISO8859_1Reader : IReader
				{
						private void InitBlock(VTDGen enclosingInstance)
						{
								this.enclosingInstance = enclosingInstance;
						}
						private VTDGen enclosingInstance;
						public int Char
						{
								get
								{

										if (Enclosing_Instance.offset >= Enclosing_Instance.endOffset)
												throw enclosingInstance.e;
										return Enclosing_Instance.XMLDoc[Enclosing_Instance.offset++];
								}

						}
						public VTDGen Enclosing_Instance
						{
								get
								{
										return enclosingInstance;
								}

						}
						public ISO8859_1Reader(VTDGen enclosingInstance)
						{
								InitBlock(enclosingInstance);
						}
						public bool skipChar(int ch)
						{
								if (ch == Enclosing_Instance.XMLDoc[Enclosing_Instance.offset])
								{
										Enclosing_Instance.offset++;
										return true;
								}
								else
								{
										return false;
								}
						}

						public long _getChar(int offset)
						{
								int c = 0xff & Enclosing_Instance.XMLDoc[offset];
								if (c == '\r' && Enclosing_Instance.XMLDoc[offset + 1] == '\n')
										return (2L << 32) | '\n';
								return (1L << 32) | c;
						}

						public char decode(int offset)
						{
								return (char)(Enclosing_Instance.XMLDoc[offset] & 0xff);
						}
				}
				protected internal IReader r;

				/// <summary> VTDGen constructor method.</summary>
				public VTDGen()
				{
						attr_name_array = new long[ATTR_NAME_ARRAY_SIZE];
						prefixed_attr_name_array = new long[ATTR_NAME_ARRAY_SIZE];
						prefix_URL_array = new int[ATTR_NAME_ARRAY_SIZE];
						tag_stack = new long[TAG_STACK_SIZE];
						//scratch_buffer = new int[10];
						VTDDepth = 0;
						LcDepth = 3;
						br = false;
						e = new EOFException("permature EOF reached, XML document incomplete");
						ws = false;
						nsBuffer1 = new FastIntBuffer(4);
						nsBuffer2 = new FastLongBuffer(4);
						nsBuffer3 = new FastLongBuffer(4);
						currentElementRecord = 0;
						singleByteEncoding = true;
						shallowDepth = true;
				}
				/// <summary> Clear internal states so VTDGEn can process the next file.</summary>
				public void clear()
				{
						if (br == false)
						{
								VTDBuffer = null;
								l1Buffer = null;
								l2Buffer = null;
								l3Buffer = null;
								_l3Buffer = null;
								_l4Buffer = null;
								_l5Buffer = null;
						}
						XMLDoc = null;
						offset = temp_offset = 0;
						last_depth = last_l1_index = last_l2_index = last_l3_index = last_l4_index = 0;
						rootIndex = 0;
						depth = -1;
						increment = 1;
						BOM_detected = false;
						must_utf_8 = false;
						ch = ch_temp = 0;
						r = new UTF8Reader(this);
						nsBuffer1.size_Renamed_Field = 0;
						nsBuffer2.size_Renamed_Field = 0;
						nsBuffer3.size_Renamed_Field = 0;
						currentElementRecord = 0;
				}
				/// <summary> This method will detect whether the entity is valid or not and increment offset.</summary>
				/// <returns> int
				/// </returns>
				/// <throws>  com.ximpleware.ParseException Super class for any exception during parsing. </throws>
				/// <throws>  com.ximpleware.EncodingException UTF/native encoding exception. </throws>
				/// <throws>  com.ximpleware.EOFException End of file exception. </throws>
				private int entityIdentifier()
				{
						int ch = r.Char;
						int val = 0;

						switch (ch)
						{

								case '#':
										ch = r.Char;
										if (ch == 'x')
										{
												while (true)
												{
														ch = r.Char;
														if (ch >= '0' && ch <= '9')
														{
																val = (val << 4) + (ch - '0');
														}
														else if (ch >= 'a' && ch <= 'f')
														{
																val = (val << 4) + (ch - 'a' + 10);
														}
														else if (ch >= 'A' && ch <= 'F')
														{
																val = (val << 4) + (ch - 'A' + 10);
														}
														else if (ch == ';')
														{
																return val;
														}
														else
																throw new EntityException("Errors in char reference: Illegal char following &#x.");
												}
										}
										else
										{
												while (true)
												{
														if (ch >= '0' && ch <= '9')
														{
																val = val * 10 + (ch - '0');
														}
														else if (ch == ';')
														{
																break;
														}
														else
																throw new EntityException("Errors in char reference: Illegal char following &#.");
														ch = r.Char;
												}
										}
										if (!XMLChar.isValidChar(val))
										{
												throw new EntityException("Errors in entity reference: Invalid XML char.");
										}
										return val;
								//break;


								case 'a':
										ch = r.Char;
										if (ch == 'm')
										{
												if (r.Char == 'p' && r.Char == ';')
												{
														//System.out.println(" entity for &");
														return '&';
												}
												else
														throw new EntityException("Errors in Entity: Illegal builtin reference");
										}
										else if (ch == 'p')
										{
												if (r.Char == 'o' && r.Char == 's' && r.Char == ';')
												{
														//System.out.println(" entity for ' ");
														return '\'';
												}
												else
														throw new EntityException("Errors in Entity: Illegal builtin reference");
										}
										else
												throw new EntityException("Errors in Entity: Illegal builtin reference");
								//goto case 'q';


								case 'q':
										if (r.Char == 'u' && r.Char == 'o' && r.Char == 't' && r.Char == ';')
										{
												return '"';
										}
										else
												throw new EntityException("Errors in Entity: Illegal builtin reference");
								//goto case 'l';

								case 'l':
										if (r.Char == 't' && r.Char == ';')
										{
												return '<';
										}
										else
												throw new EntityException("Errors in Entity: Illegal builtin reference");
								//break;
								//goto case 'g';

								case 'g':
										if (r.Char == 't' && r.Char == ';')
										{
												return '>';
										}
										else
												throw new EntityException("Errors in Entity: Illegal builtin reference");
								//goto default;

								default:
										throw new EntityException("Errors in Entity: Illegal entity char");

						}
						//return val;
				}
				/// <summary> Format the string indicating the position (line number:offset)of the offset if 
				/// there is an exception.
				/// </summary>
				/// <returns> java.lang.String indicating the line number and offset of the exception
				/// </returns>
				private String formatLineNumber()
				{
						return formatLineNumber(offset);
				}
				/// <summary> Write the remaining portion of LC info
				/// 
				/// </summary>
				private void finishUp()
				{
						if (shallowDepth)
						{
								if (last_depth == 1)
								{
										l1Buffer.append(((long)last_l1_index << 32) | 0xffffffffL);
								}
								else if (last_depth == 2)
								{
										l2Buffer.append(((long)last_l2_index << 32) | 0xffffffffL);
								}
						}
						else
						{
								if (last_depth == 1)
								{
										l1Buffer.append(((long)last_l1_index << 32) | 0xffffffffL);
								}
								else if (last_depth == 2)
								{
										l2Buffer.append(((long)last_l2_index << 32) | 0xffffffffL);
								}
								else if (last_depth == 3)
								{
										_l3Buffer.append(((long)last_l3_index << 32) | 0xffffffffL);
								}
								else if (last_depth == 4)
								{
										_l4Buffer.append(((long)last_l4_index << 32) | 0xffffffffL);
								}
						}
				}


				/// <summary> A private method that detects the BOM and decides document encoding</summary>
				/// <throws>  EncodingException </throws>
				/// <throws>  ParseException </throws>
				private void decide_encoding()
				{
						if (XMLDoc.Length == 0)
								throw new EncodingException("Document is zero sized ");
						if (XMLDoc[offset] == 0xfe)
						{
								increment = 2;
								if (XMLDoc[offset + 1] == 0xff)
								{
										offset += 2;
										encoding = FORMAT_UTF_16BE;
										BOM_detected = true;
										r = new UTF16BEReader(this);
								}
								else
										throw new EncodingException("Unknown Character encoding: should be 0xff 0xfe");
						}
						else if (XMLDoc[offset] == 0xff)
						{
								increment = 2;
								if (XMLDoc[offset + 1] == 0xfe)
								{
										offset += 2;
										encoding = FORMAT_UTF_16LE;
										BOM_detected = true;
										r = new UTF16LEReader(this);
								}
								else
										throw new EncodingException("Unknown Character encoding: not UTF-16LE");
						}
						else if (XMLDoc[offset] == unchecked((byte)-17))
						{
								if (XMLDoc[offset + 1] == unchecked((byte)-69) && XMLDoc[offset + 2] == unchecked((byte)-65))
								{
										offset += 3;
										must_utf_8 = true;
								}
								else
										throw new EncodingException("Unknown Character encoding: not UTF-8");
						}
						else if (XMLDoc[offset] == 0)
						{
								if (XMLDoc[offset + 1] == 0x3c && XMLDoc[offset + 2] == 0 && XMLDoc[offset + 3] == 0x3f)
								{
										encoding = FORMAT_UTF_16BE;
										increment = 2;
										r = new UTF16BEReader(this);
								}
								else
										throw new EncodingException("Unknown Character encoding: not UTF-16BE");
						}
						else if (XMLDoc[offset] == 0x3c)
						{
								if (XMLDoc[offset + 1] == 0 && XMLDoc[offset + 2] == 0x3f && XMLDoc[offset + 3] == 0)
								{
										increment = 2;
										encoding = FORMAT_UTF_16LE;
										r = new UTF16LEReader(this);
								}
						}
						// check for max file size exception
						if (encoding < FORMAT_UTF_16BE)
						{
								if (ns)
								{
										if ((offset + (long)docLen) >= 1L << 30)
												throw new ParseException("Other error: file size too big >=1GB ");
								}
								else
								{
										if ((offset + (long)docLen) >= 1L << 31)
												throw new ParseException("Other error: file size too big >=2GB ");
								}
						}
						else
						{
								if ((offset + (long)docLen) >= 1L << 31)
										throw new ParseException("Other error: file size too large >= 2GB");
						}
						if (encoding >= FORMAT_UTF_16BE)
								singleByteEncoding = false;
				}
				/// <summary>
				/// parse a file directly
				/// </summary>
				/// <param name="fileName"> Name of the file to be parsed</param>
				/// <param name="ns"> namespace awareness</param>
				/// <returns>boolean indicating whether it is a success or not</returns>

				public bool parseFile(String fileName, bool ns)
				{
						System.IO.FileInfo f = null;
						System.IO.FileStream fis = null;
						try
						{
								f = new System.IO.FileInfo(fileName);
								//UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javaioFile'"
								fis = new System.IO.FileStream(f.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

								byte[] b = new byte[(int)f.Length];
								fis.Read(b, 0, (int)f.Length);
								int numBytesToRead = (int)f.Length;
								//int numBytesToRead = (int)s.Length;
								int numBytesRead = 0;
								while (numBytesToRead > 0)
								{
										// Read may return anything from 0 to numBytesToRead.
										int n = fis.Read(b, numBytesRead, numBytesToRead);
										// The end of the file is reached.
										if (n == 0)
												break;
										numBytesRead += n;
										numBytesToRead -= n;
								}
								this.setDoc(b);
								this.parse(ns);
								return true;
						}
						catch
						{
						}
						finally
						{
								if (fis != null)
								{
										try
										{
												fis.Close();
										}
										catch
										{
										}
								}
						}
						return false;
				}

				/// <summary>
				/// This method grabs an XML doc from the net using HTTP get
				/// </summary>
				/// <param name="URL">The URL, e.g. "http://vtd-xml.sf.net/codeSample/old.xml"</param>
				/// <param name="ns"> namespace aware or not </param>
				/// <returns>status of the parsing, true if successful, false otherwise</returns>
				public bool parseHttpUrl(String url, bool ns)
				{
						System.IO.Stream in_Renamed = null;
						System.Net.HttpWebRequest urlConnection = null;
						System.Net.HttpWebResponse response = null;
						try
						{
								urlConnection = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
								response = (System.Net.HttpWebResponse)urlConnection.GetResponse();
								in_Renamed = response.GetResponseStream();
								//in_Renamed.
								int len = (int)response.ContentLength;
								if (len > 0)
								{
										byte[] ba = new byte[len];
										int numBytesToRead = len;
										//int numBytesToRead = (int)s.Length;
										int numBytesRead = 0;
										while (numBytesToRead > 0)
										{
												// Read may return anything from 0 to numBytesToRead.
												int n = in_Renamed.Read(ba, numBytesRead, numBytesToRead);
												// The end of the file is reached.
												if (n == 0)
														break;
												numBytesRead += n;
												numBytesToRead -= n;
										}

										//char[] ca = new char[len];
										//int len2 =in_Renamed.Read(ba, 0, len);
										//Console.WriteLine(" len2 " + len2);
										//for (int k = 0; k < ba.Length; k++)
										//{
										//     ca[k] = (char)ba[k];
										//}
										//Console.WriteLine(ba.ToString());
										//String s = new String(ca,0,ca.Length);
										//Console.WriteLine(s);
										setDoc(ba);
										parse(ns);
										return true;
								}
						}
						catch (System.IO.IOException e)
						{
						}
						catch (ParseException e)
						{
								Console.WriteLine(e);
						}
						catch (System.Net.WebException e)
						{
						}
						finally
						{
								try
								{
										if (in_Renamed != null)
												in_Renamed.Close();
										if (response != null)
												response.Close();
								}
								catch (System.Exception e)
								{
								}
						}
						return false;
				}


				/// <summary> Generating VTD tokens and Location cache info.
				/// VTDGen conforms to XML namespace 1.0 spec
				/// </summary>
				/// <param name="NS">boolean Enable namespace or not
				/// </param>
				/// <throws>  ParseException Super class for any exceptions during parsing.      </throws>
				/// <throws>  EOFException End of file exception.     </throws>
				/// <throws>  EntityException Entity resolution exception. </throws>
				/// <throws>  EncodingException UTF/native encoding exception. </throws>
				public void parse(bool NS)
				{

						// define internal variables	
						ns = NS;
						length1 = length2 = 0;
						attr_count = prefixed_attr_count = 0;
						int parser_state = STATE_DOC_START;
						//boolean has_amp = false; 
						is_ns = false;
						encoding = FORMAT_UTF8;
						bool helper = false;
						bool default_ns = false; //true xmlns='abc'
						bool isXML = false;      //true only for xmlns:xml
						singleByteEncoding = true;
						//char char_temp; //holds the ' or " indicating start of attr val
						//boolean must_utf_8 = false;
						//boolean BOM_detected = false;
						//long[] tag_stack = new long[256];
						//long[] attr_name_array = new long[512]; // 512 attributes limit
						//ASCII UTF-8 UTF-16 UTF-16BE UTF-16LE ISO-8859-1
						//
						//int[] scratch_buffer = new int[10];

						// first check first several bytes to figure out the encoding
						decide_encoding();

						// enter the main finite state machine
						try
						{
								_writeVTD(0, 0, TOKEN_DOCUMENT, depth);
								while (true)
								{
										switch (parser_state)
										{

												case STATE_LT_SEEN:  //if (depth < -1)
														//    throw new ParseException("Other Errors: Invalid depth");
														temp_offset = offset;
														ch = r.Char;
														if (XMLChar.isNameStartChar(ch))
														{
																//temp_offset = offset;
																//length1++;
																depth++;
																//if (ch == ':')
																//   length2 = 0;
																parser_state = STATE_START_TAG;
														}
														else
														{
																switch (ch)
																{

																		case '/':
																				parser_state = STATE_END_TAG;
																				break;

																		case '?':
																				parser_state = process_qm_seen();
																				break;

																		case '!':  // three possibility (comment, CDATA, DOCTYPE)
																				parser_state = process_ex_seen();
																				break;

																		default:
																				throw new ParseException("Other Error: Invalid char after <" + formatLineNumber());

																}
														}
														break;


												case STATE_START_TAG:  //name space is handled by

														do
														{
																ch = r.Char;
																if (XMLChar.isNameChar(ch))
																{
																		if (ch == ':')
																		{
																				length2 = offset - temp_offset - increment;
																				if (ns && checkPrefix2(temp_offset, length2))
																						throw new ParseException(
																										"xmlns can't be an element prefix "
																										+ formatLineNumber(offset));
																		}
																}
																else
																		break;
														} while (true);
														length1 = offset - temp_offset - increment;
														if (depth > MAX_DEPTH)
														{
																throw new ParseException("Other Error: Depth exceeds MAX_DEPTH" + formatLineNumber());
														}
														//writeVTD(offset, TOKEN_STARTING_TAG, length2:length1, depth)
														long x = ((long)length1 << 32) + temp_offset;
														tag_stack[depth] = x;

														// System.out.println(
														//     " " + (temp_offset) + " " + length2 + ":" + length1 + " startingTag " + depth);
														if (depth > VTDDepth)
																VTDDepth = depth;
														if (singleByteEncoding)
														{
																if (length2 > MAX_PREFIX_LENGTH || length1 > MAX_QNAME_LENGTH)
																		throw new ParseException("Token Length Error: Starting tag prefix or qname length too long" + formatLineNumber());
																if (shallowDepth)
																		writeVTD((temp_offset), (length2 << 11) | length1, TOKEN_STARTING_TAG, depth);
																else
																		writeVTD_L5((temp_offset), (length2 << 11) | length1, TOKEN_STARTING_TAG, depth);
														}
														else
														{
																if (length2 > (MAX_PREFIX_LENGTH << 1) || length1 > (MAX_QNAME_LENGTH << 1))
																		throw new ParseException("Token Length Error: Starting tag prefix or qname length too long" + formatLineNumber());
																//writeVTD((temp_offset) >> 1, (length2 << 10) | (length1 >> 1), TOKEN_STARTING_TAG, depth);
																if (shallowDepth)
																		writeVTD((temp_offset) >> 1, (length2 << 10) | (length1 >> 1), TOKEN_STARTING_TAG, depth);
																else
																		writeVTD_L5((temp_offset) >> 1, (length2 << 10) | (length1 >> 1), TOKEN_STARTING_TAG, depth);
														}
														//offset += length1;
														if (ns)
														{
																if (length2 != 0)
																{
																		length2 += increment;
																		currentElementRecord = (((long)((length2 << 16) | length1)) << 32)
																		| temp_offset;
																}
																else
																		currentElementRecord = 0;

																if (depth <= nsBuffer1.size_Renamed_Field - 1)
																{
																		nsBuffer1.size_Renamed_Field = depth;
																		int t = nsBuffer1.intAt(depth - 1) + 1;
																		nsBuffer2.size_Renamed_Field = t;
																		nsBuffer3.size_Renamed_Field = t;
																}
														}
														length2 = 0;
														if (XMLChar.isSpaceChar(ch))
														{
																ch = CharAfterS;
																if (XMLChar.isNameStartChar(ch))
																{
																		// seen an attribute here
																		temp_offset = PrevOffset;
																		parser_state = STATE_ATTR_NAME;
																		break;
																}
														}
														helper = true;
														if (ch == '/')
														{
																depth--;
																helper = false;
																ch = r.Char;
														}
														if (ch == '>')
														{
																if (ns)
																{
																		nsBuffer1.append(nsBuffer3.size_Renamed_Field - 1);
																		if (currentElementRecord != 0)
																				qualifyElement();
																}
																if (depth != -1)
																{
																		temp_offset = offset;
																		//ch = CharAfterSe; // consume WSs
																		ch = CharAfterS;
																		if (ch == '<')
																		{
																				if (ws)
																						addWhiteSpaceRecord();
																				parser_state = STATE_LT_SEEN;
																				if (r.skipChar('/'))
																				{
																						if (helper)
																						{
																								length1 = offset - temp_offset - (increment << 1);

																								if (singleByteEncoding)
																										writeVTDText((temp_offset), length1, TOKEN_CHARACTER_DATA, depth);
																								else
																										writeVTDText((temp_offset) >> 1, (length1 >> 1), TOKEN_CHARACTER_DATA, depth);
																						}
																						parser_state = STATE_END_TAG;
																						break;
																				}
																		}
																		else if (XMLChar.isContentChar(ch))
																		{
																				//temp_offset = offset;
																				parser_state = STATE_TEXT;
																		}
																		else
																		{
																				parser_state = STATE_TEXT;
																				handleOtherTextChar2(ch);
																		}
																}
																else
																{
																		parser_state = STATE_DOC_END;
																}
																break;
														}
														throw new ParseException("Starting tag Error: Invalid char in starting tag" + formatLineNumber());


												case STATE_END_TAG:
														temp_offset = offset;
														int sos = (int)tag_stack[depth];
														int sl = (int)(tag_stack[depth] >> 32);

														offset = temp_offset + sl;

														if (offset >= endOffset)
																throw new EOFException("permature EOF reached, XML document incomplete");
														for (int i = 0; i < sl; i++)
														{
																if (XMLDoc[sos + i] != XMLDoc[temp_offset + i])
																		throw new ParseException("Ending tag error: Start/ending tag mismatch" + formatLineNumber());
														}
														depth--;
														ch = CharAfterS;
														if (ch != '>')
																throw new ParseException("Ending tag error: Invalid char in ending tag " + formatLineNumber());

														if (depth != -1)
														{
																temp_offset = offset;
																ch = CharAfterS;
																if (ch == '<')
																{
																		if (ws)
																				addWhiteSpaceRecord();
																		parser_state = STATE_LT_SEEN;
																}
																else if (XMLChar.isContentChar(ch))
																{
																		parser_state = STATE_TEXT;
																}
																else
																{
																		handleOtherTextChar2(ch);
																		parser_state = STATE_TEXT;
																}
														}
														else
																parser_state = STATE_DOC_END;
														break;
												case STATE_ATTR_NAME:

														if (ch == 'x')
														{
																if (r.skipChar('m') && r.skipChar('l') && r.skipChar('n') && r.skipChar('s'))
																{
																		ch = r.Char;
																		if (ch == '='
																		|| XMLChar.isSpaceChar(ch))
																		{
																				is_ns = true;
																				default_ns = true;
																		}
																		else if (ch == ':')
																		{
																				is_ns = true; //break;
																				default_ns = false;
																		}
																}
														}

														do
														{
																if (XMLChar.isNameChar(ch))
																{
																		if (ch == ':')
																		{
																				length2 = offset - temp_offset - increment;
																		}
																		ch = r.Char;
																}
																else
																		break;
														} while (true);
														length1 = PrevOffset - temp_offset;
														if (is_ns && ns)
														{
																// make sure postfix isn't xmlns
																if (!default_ns)
																{
																		if (increment == 1 && (length1 - length2 - 1 == 5)
																						|| (increment == 2 && (length1 - length2 - 2 == 10)))
																				disallow_xmlns(temp_offset + length2 + increment);

																		// if the post fix is xml, signal it
																		if (increment == 1 && (length1 - length2 - 1 == 3)
																						|| (increment == 2 && (length1 - length2 - 2 == 6)))
																				isXML = matchXML(temp_offset + length2 + increment);
																}
														}
														// check for uniqueness here
														checkAttributeUniqueness();
														// check for uniqueness here
														//bool unique = true;
														//bool unequal;
														//for (int i = 0; i < attr_count; i++)
														//{
														//    unequal = false;
														//    int prevLen = (int)attr_name_array[i];
														//    if (length1 == prevLen)
														//    {
														//        int prevOffset = (int)(attr_name_array[i] >> 32);
														//        for (int j = 0; j < prevLen; j++)
														//        {
														//            if (XMLDoc[prevOffset + j] != XMLDoc[temp_offset + j])
														//            {
														//                unequal = true;
														//                break;
														//            }
														//        }
														//    }
														//    else
														//        unequal = true;
														//    unique = unique && unequal;
														//}
														//if (!unique && attr_count != 0)
														//    throw new ParseException("Error in attr: Attr name not unique" + formatLineNumber());
														//unique = true;
														//if (attr_count < attr_name_array.Length)
														//{
														//    attr_name_array[attr_count] = ((long)(temp_offset) << 32) + length1;
														//    attr_count++;
														//}
														//// grow the attr_name_array by 16
														//else
														//{
														//    long[] temp_array = attr_name_array;
														//    /*System.out.println(
														//    "size increase from "
														//    + temp_array.length
														//    + "  to "
														//    + (attr_count + 16));*/
														//    attr_name_array = new long[attr_count + ATTR_NAME_ARRAY_SIZE];
														//    for (int i = 0; i < attr_count; i++)
														//    {
														//        attr_name_array[i] = temp_array[i];
														//    }
														//    attr_name_array[attr_count] = ((long)(temp_offset) << 32) + length1;
														//    attr_count++;
														//}

														// after checking, write VTD
														if (is_ns)
														{
																if (singleByteEncoding)
																{
																		if (length2 > MAX_PREFIX_LENGTH || length1 > MAX_QNAME_LENGTH)
																				throw new ParseException("Token length overflow error: Attr NS tag prefix or qname length too long" + formatLineNumber());
																		_writeVTD(temp_offset, (length2 << 11) | length1, TOKEN_ATTR_NS, depth);
																}
																else
																{
																		if (length2 > (MAX_PREFIX_LENGTH << 1) || length1 > (MAX_QNAME_LENGTH << 1))
																				throw new ParseException("Token length overflow error: Attr NS prefix or qname length too long" + formatLineNumber());
																		_writeVTD(temp_offset >> 1, (length2 << 10) | (length1 >> 1), TOKEN_ATTR_NS, depth);
																}
																// append to nsBuffer2
																if (ns)
																{
																		//unprefixed xmlns are not recorded
																		if (length2 != 0 && !isXML)
																		{
																				//nsBuffer2.append(VTDBuffer.size_Renamed_Field - 1);
																				long l = ((long)((length2 << 16) | length1)) << 32
																						| temp_offset;
																				nsBuffer3.append(l); // byte offset and byte
																				// length
																		}
																}
														}
														else
														{
																if (singleByteEncoding)
																{
																		if (length2 > MAX_PREFIX_LENGTH || length1 > MAX_QNAME_LENGTH)
																				throw new ParseException("Token Length Error: Attr name prefix or qname length too long" + formatLineNumber());
																		_writeVTD(temp_offset, (length2 << 11) | length1, TOKEN_ATTR_NAME, depth);
																}
																else
																{
																		if (length2 > (MAX_PREFIX_LENGTH << 1) || length1 > (MAX_QNAME_LENGTH << 1))
																				throw new ParseException("Token Length overflow error: Attr name prefix or qname length too long" + formatLineNumber());
																		_writeVTD(temp_offset >> 1, (length2 << 10) | (length1 >> 1), TOKEN_ATTR_NAME, depth);
																}
														}
														/*System.out.println(
														" " + temp_offset + " " + length2 + ":" + length1 + " attr name " + depth);*/
														length2 = 0;
														if (XMLChar.isSpaceChar(ch))
														{
																ch = CharAfterS;
														}
														if (ch != '=')
																throw new ParseException("Error in attr: invalid char" + formatLineNumber());
														ch_temp = CharAfterS;
														if (ch_temp != '"' && ch_temp != '\'')
																throw new ParseException("Error in attr: invalid char (should be ' or \" )" + formatLineNumber());
														temp_offset = offset;
														parser_state = STATE_ATTR_VAL;
														break;

												case STATE_ATTR_VAL:
														do
														{
																ch = r.Char;
																if (XMLChar.isValidChar(ch) && ch != '<')
																{
																		if (ch == ch_temp)
																				break;
																		if (ch == '&')
																		{
																				// as in vtd spec, we mark attr val with entities
																				if (!XMLChar.isValidChar(entityIdentifier()))
																				{
																						throw new ParseException("Error in attr: Invalid XML char" + formatLineNumber());
																				}
																		}
																}
																else
																		throw new ParseException("Error in attr: Invalid XML char" + formatLineNumber());
														} while (true);

														length1 = offset - temp_offset - increment;
														if (ns && is_ns)
														{
																if (!default_ns && length1 == 0)
																{
																		throw new ParseException(" non-default ns URL can't be empty"
																				+ formatLineNumber());
																}
																//identify nsURL return 0,1,2
																int t = identifyNsURL(temp_offset, length1);
																if (isXML)
																{//xmlns:xml
																		if (t != 1)
																				//URL points to "http://www.w3.org/XML/1998/namespace"
																				throw new ParseException("xmlns:xml can only point to"
																								+ "\"http://www.w3.org/XML/1998/namespace\""
																								+ formatLineNumber());

																}
																else
																{
																		if (!default_ns)
																				nsBuffer2.append(((long)temp_offset << 32) | length1);
																		if (t != 0)
																		{
																				if (t == 1)
																						throw new ParseException("namespace declaration can't point to"
																								+ " \"http://www.w3.org/XML/1998/namespace\""
																								+ formatLineNumber());
																				throw new ParseException("namespace declaration can't point to"
																						+ " \"http://www.w3.org/2000/xmlns/\""
																						+ formatLineNumber());
																		}
																}
																// no ns URL points to 
																//"http://www.w3.org/2000/xmlns/"

																// no ns URL points to  
																//"http://www.w3.org/XML/1998/namespace"
														}
														if (singleByteEncoding)
														{
																if (length1 > MAX_TOKEN_LENGTH)
																		throw new ParseException("Token Length Error:" + " Attr val too long (>0xfffff)" + formatLineNumber());
																_writeVTD(temp_offset, length1, TOKEN_ATTR_VAL, depth);
														}
														else
														{
																if (length1 > (MAX_TOKEN_LENGTH << 1))
																		throw new ParseException("Token Length Error:" + " Attr val too long (>0xfffff)" + formatLineNumber());
																_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_ATTR_VAL, depth);
														}

														isXML = false;
														is_ns = false;

														ch = r.Char;
														if (XMLChar.isSpaceChar(ch))
														{
																ch = CharAfterS;
																if (XMLChar.isNameStartChar(ch))
																{
																		temp_offset = offset - increment;
																		parser_state = STATE_ATTR_NAME;
																		break;
																}
														}
														helper = true;
														if (ch == '/')
														{
																depth--;
																helper = false;
																ch = r.Char;
														}

														if (ch == '>')
														{
																if (ns)
																{
																		nsBuffer1.append(nsBuffer3.size_Renamed_Field - 1);
																		if (prefixed_attr_count > 0)
																				qualifyAttributes();
																		if (prefixed_attr_count > 1)
																		{
																				checkQualifiedAttributeUniqueness();
																				prefixed_attr_count = 0;
																		}
																		if (currentElementRecord != 0)
																				qualifyElement();

																}
																attr_count = 0;
																if (depth != -1)
																{
																		temp_offset = offset;
																		//ch = CharAfterSe;
																		ch = CharAfterS;
																		if (ch == '<')
																		{
																				if (ws)
																						addWhiteSpaceRecord();
																				parser_state = STATE_LT_SEEN;
																				if (r.skipChar('/'))
																				{
																						if (helper)
																						{
																								length1 = offset - temp_offset - (increment << 1);

																								if (encoding < FORMAT_UTF_16BE)
																										writeVTDText((temp_offset), length1, TOKEN_CHARACTER_DATA, depth);
																								else
																										writeVTDText((temp_offset) >> 1, (length1 >> 1), TOKEN_CHARACTER_DATA, depth);
																						}
																						parser_state = STATE_END_TAG;
																						break;
																				}
																		}
																		else if (XMLChar.isContentChar(ch))
																		{
																				//temp_offset = offset;
																				parser_state = STATE_TEXT;
																		}
																		else
																		{
																				handleOtherTextChar2(ch);
																				parser_state = STATE_TEXT;
																		}
																}
																else
																{
																		parser_state = STATE_DOC_END;
																}
																break;
														}

														throw new ParseException("Starting tag Error: Invalid char in starting tag" + formatLineNumber());


												case STATE_TEXT:
														if (depth == -1)
																throw new ParseException("Error in text content: Char data at the wrong place" + formatLineNumber());
														do
														{
																ch = r.Char;
																if (XMLChar.isContentChar(ch))
																{
																}
																else if (ch == '<')
																{
																		break;
																}
																else
																		handleOtherTextChar(ch);
																ch = r.Char;
																if (XMLChar.isContentChar(ch))
																{
																}
																else if (ch == '<')
																{
																		break;
																}
																else
																		handleOtherTextChar(ch);
														} while (true);
														length1 = offset - increment - temp_offset;

														if (singleByteEncoding)
																writeVTDText(temp_offset, length1, TOKEN_CHARACTER_DATA, depth);
														else
																writeVTDText(temp_offset >> 1, length1 >> 1, TOKEN_CHARACTER_DATA, depth);

														//has_amp = true;
														parser_state = STATE_LT_SEEN;
														break;
												case STATE_DOC_START:
														parser_state = process_start_doc();
														break;

												case STATE_DOC_END:
														//docEnd = true;
														ch = CharAfterS;
														parser_state = process_end_doc();
														break;


												case STATE_PI_TAG:
														parser_state = process_pi_tag();
														break;
												//throw new ParseException("Error in PI: Invalid char");

												case STATE_PI_VAL:
														parser_state = process_pi_val();
														break;


												case STATE_DEC_ATTR_NAME:
														parser_state = process_dec_attr();
														break;


												case STATE_COMMENT:
														parser_state = process_comment();
														break;


												case STATE_CDATA:
														parser_state = process_cdata();
														break;


												case STATE_DOCTYPE:
														parser_state = process_doc_type();
														break;



												case STATE_END_PI:
														parser_state = process_end_pi();
														break;


												case STATE_END_COMMENT:
														parser_state = process_end_comment();
														break;


												default:
														throw new ParseException("Other error: invalid parser state" + formatLineNumber());

										}
								}
						}
						catch (EOFException e)
						{
								if (parser_state != STATE_DOC_END)
										throw e;
								finishUp();
						}
				}

				private void matchCPEncoding()
				{
						if ((r.skipChar('p') || r.skipChar('P')) && r.skipChar('1') && r.skipChar('2') && r.skipChar('5'))
						{
								if (encoding <= FORMAT_UTF_16LE)
								{
										if (must_utf_8)
												throw new EncodingException("Can't switch from UTF-8" + formatLineNumber());
										if (r.skipChar('0') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1250;
												r = new WIN1250Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('1') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1251;
												r = new WIN1251Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('2') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1252;
												r = new WIN1252Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('3') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1253;
												r = new WIN1253Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('4') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1254;
												r = new WIN1254Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('5') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1255;
												r = new WIN1255Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('6') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1256;
												r = new WIN1256Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('7') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1257;
												r = new WIN1257Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('8') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1258;
												r = new WIN1258Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
								}
								else
										throw new ParseException("XML decl error: Can't switch encoding to ISO-8859" + formatLineNumber());
						}
						throw new ParseException("XML decl error: Invalid Encoding" + formatLineNumber());
				}
				private void matchWindowsEncoding()
				{
						if ((r.skipChar('i') || r.skipChar('I')) && (r.skipChar('n') || r.skipChar('N')) && (r.skipChar('d') || r.skipChar('D')) && (r.skipChar('o') || r.skipChar('O')) && (r.skipChar('w') || r.skipChar('W')) && (r.skipChar('s') || r.skipChar('S')) && r.skipChar('-') && r.skipChar('1') && r.skipChar('2') && r.skipChar('5'))
						{
								if (encoding <= FORMAT_UTF_16LE)
								{
										if (must_utf_8)
												throw new EncodingException("Can't switch from UTF-8" + formatLineNumber());
										if (r.skipChar('0') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1250;
												r = new WIN1250Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('1') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1251;
												r = new WIN1251Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('2') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1252;
												r = new WIN1252Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('3') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1253;
												r = new WIN1253Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('4') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1254;
												r = new WIN1254Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('5') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1255;
												r = new WIN1255Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('6') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1256;
												r = new WIN1256Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('7') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1257;
												r = new WIN1257Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('8') && r.skipChar(ch_temp))
										{
												encoding = FORMAT_WIN_1258;
												r = new WIN1258Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
								}
								else
										throw new ParseException("XML decl error: Can't switch encoding to ISO-8859" + formatLineNumber());
						}
						throw new ParseException("XML decl error: Invalid Encoding" + formatLineNumber());
				}
				private void matchUTFEncoding()
				{
						if ((r.skipChar('s') || r.skipChar('S')))
								if (r.skipChar('-') && (r.skipChar('a') || r.skipChar('A')) && (r.skipChar('s') || r.skipChar('S')) && (r.skipChar('c') || r.skipChar('C')) && (r.skipChar('i') || r.skipChar('I')) && (r.skipChar('i') || r.skipChar('I')) && r.skipChar(ch_temp))
								{
										if (encoding != FORMAT_UTF_16LE && encoding != FORMAT_UTF_16BE)
										{
												if (must_utf_8)
														throw new EncodingException("Can't switch from UTF-8" + formatLineNumber());
												encoding = FORMAT_ASCII;
												r = new ASCIIReader(this);
												_writeVTD(temp_offset, 8, TOKEN_DEC_ATTR_VAL, depth);

												return;
										}
										else
												throw new ParseException("XML decl error: Can't switch encoding to US-ASCII" + formatLineNumber());
								}
								else
										throw new ParseException("XML decl error: Invalid Encoding" + formatLineNumber());

						if ((r.skipChar('t') || r.skipChar('T')) && (r.skipChar('f') || r.skipChar('F')) && r.skipChar('-'))
						{
								if (r.skipChar('8') && r.skipChar(ch_temp))
								{
										if (encoding != FORMAT_UTF_16LE && encoding != FORMAT_UTF_16BE)
										{
												//encoding = FORMAT_UTF8;
												_writeVTD(temp_offset, 5, TOKEN_DEC_ATTR_VAL, depth);

												return;
										}
										else
												throw new ParseException("XML decl error: Can't switch encoding to UTF-8" + formatLineNumber());
								}
								if (r.skipChar('1') && r.skipChar('6'))
								{
										if (r.skipChar(ch_temp))
										{
												if (encoding == FORMAT_UTF_16LE || encoding == FORMAT_UTF_16BE)
												{
														if (!BOM_detected)
																throw new EncodingException("BOM not detected for UTF-16" + formatLineNumber());
														_writeVTD(temp_offset >> 1, 6, TOKEN_DEC_ATTR_VAL, depth);
														return;
												}
												throw new ParseException("XML decl error: Can't switch encoding to UTF-16" + formatLineNumber());
										}
										else if ((r.skipChar('l') || r.skipChar('L')) && (r.skipChar('e') || r.skipChar('E')) && r.skipChar(ch_temp))
										{
												if (encoding == FORMAT_UTF_16LE)
												{
														r = new UTF16LEReader(this);
														_writeVTD(temp_offset >> 1, 8, TOKEN_DEC_ATTR_VAL, depth);
														return;
												}
												throw new ParseException("XML del error: Can't switch encoding to UTF-16LE" + formatLineNumber());
										}
										else if ((r.skipChar('b') || r.skipChar('B')) && (r.skipChar('e') || r.skipChar('E')) && r.skipChar(ch_temp))
										{
												if (encoding == FORMAT_UTF_16BE)
												{
														_writeVTD(temp_offset >> 1, 8, TOKEN_DEC_ATTR_VAL, depth);
														return;
												}
												throw new ParseException("XML del error: Can't swtich encoding to UTF-16BE" + formatLineNumber());
										}

										throw new ParseException("XML decl error: Invalid encoding" + formatLineNumber());
								}
						}
				}

				private void matchISOEncoding()
				{
						if ((r.skipChar('s') || r.skipChar('S')) && (r.skipChar('o') || r.skipChar('O')) && r.skipChar('-') && r.skipChar('8') && r.skipChar('8') && r.skipChar('5') && r.skipChar('9') && r.skipChar('-'))
						{
								if (encoding <= FORMAT_UTF_16LE)
								{
										if (must_utf_8)
												throw new EncodingException("Can't switch from UTF-8" + formatLineNumber());
										if (r.skipChar('1'))
										{
												if (r.skipChar(ch_temp))
												{
														encoding = FORMAT_ISO_8859_1;
														r = new ISO8859_1Reader(this);
														_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
														return;
												}
												else if (r.skipChar('0'))
												{
														encoding = FORMAT_ISO_8859_10;
														r = new ISO8859_10Reader(this);
														_writeVTD(temp_offset, 11,
																			 TOKEN_DEC_ATTR_VAL,
																			 depth);
												}
												else if (r.skipChar('1'))
												{
														encoding = FORMAT_ISO_8859_11;
														r = new ISO8859_11Reader(this);
														_writeVTD(temp_offset, 11,
																			 TOKEN_DEC_ATTR_VAL,
																			 depth);
												}
												else if (r.skipChar('3'))
												{
														encoding = FORMAT_ISO_8859_13;
														r = new ISO8859_13Reader(this);
														_writeVTD(temp_offset, 11,
																			 TOKEN_DEC_ATTR_VAL,
																			 depth);
												}
												else if (r.skipChar('4'))
												{
														encoding = FORMAT_ISO_8859_14;
														r = new ISO8859_14Reader(this);
														_writeVTD(temp_offset, 11,
																			 TOKEN_DEC_ATTR_VAL,
																			 depth);
												}
												else if (r.skipChar('5'))
												{
														encoding = FORMAT_ISO_8859_15;
														r = new ISO8859_15Reader(this);
														_writeVTD(temp_offset, 15,
																			 TOKEN_DEC_ATTR_VAL,
																			 depth);
												}
												else
														throw new ParseException("XML decl error: Can't switch encoding to ISO-8859" + formatLineNumber());
										}
										else if (r.skipChar('2'))
										{
												encoding = FORMAT_ISO_8859_2;
												r = new ISO8859_2Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('3'))
										{
												r = new ISO8859_3Reader(this);
												encoding = FORMAT_ISO_8859_3;
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												// break;
										}
										else if (r.skipChar('4'))
										{
												r = new ISO8859_4Reader(this);
												encoding = FORMAT_ISO_8859_4;
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('5'))
										{
												encoding = FORMAT_ISO_8859_5;
												r = new ISO8859_5Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('6'))
										{
												encoding = FORMAT_ISO_8859_6;
												r = new ISO8859_6Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('7'))
										{
												encoding = FORMAT_ISO_8859_7;
												r = new ISO8859_7Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('8'))
										{
												encoding = FORMAT_ISO_8859_8;
												r = new ISO8859_8Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
										else if (r.skipChar('9'))
										{
												encoding = FORMAT_ISO_8859_9;
												r = new ISO8859_9Reader(this);
												_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_VAL, depth);
												return;
										}
								}
								else
										throw new ParseException("XML decl error: Can't switch encoding to ISO-8859" + formatLineNumber());
								if (r.skipChar(ch_temp))
										return;
						}
						throw new ParseException("XML decl error: Invalid Encoding" + formatLineNumber());
				}
				/// <summary> This private method processes declaration attributes</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_dec_attr()
				{
						int parser_state;
						if (ch == 'v' && r.skipChar('e') && r.skipChar('r') && r.skipChar('s') && r.skipChar('i') && r.skipChar('o') && r.skipChar('n'))
						{
								ch = CharAfterS;
								if (ch == '=')
								{
										/*System.out.println(
										" " + (temp_offset - 1) + " " + 7 + " dec attr name version " + depth);*/
										if (singleByteEncoding)
												_writeVTD(temp_offset - 1, 7, TOKEN_DEC_ATTR_NAME, depth);
										else
												_writeVTD((temp_offset - 2) >> 1, 7, TOKEN_DEC_ATTR_NAME, depth);
								}
								else
										throw new ParseException("XML decl error: Invalid char" + formatLineNumber());
						}
						else
								throw new ParseException("XML decl error: should be version" + formatLineNumber());
						ch_temp = CharAfterS;
						if (ch_temp != '\'' && ch_temp != '"')
								throw new ParseException("XML decl error: Invalid char to start attr name" + formatLineNumber());
						temp_offset = offset;
						// support 1.0 or 1.1
						if (r.skipChar('1') && r.skipChar('.') && (r.skipChar('0') || r.skipChar('1')))
						{
								/*System.out.println(
								" " + temp_offset + " " + 3 + " dec attr val (version)" + depth);*/
								if (singleByteEncoding)
										_writeVTD(temp_offset, 3, TOKEN_DEC_ATTR_VAL, depth);
								else
										_writeVTD(temp_offset >> 1, 3, TOKEN_DEC_ATTR_VAL, depth);
						}
						else
								throw new ParseException("XML decl error: Invalid version(other than 1.0 or 1.1) detected" + formatLineNumber());
						if (!r.skipChar(ch_temp))
								throw new ParseException("XML decl error: version not terminated properly" + formatLineNumber());
						ch = r.Char;
						//? space or e 
						if (XMLChar.isSpaceChar(ch))
						{
								ch = CharAfterS;
								temp_offset = offset - increment;
								if (ch == 'e')
								{
										if (r.skipChar('n') && r.skipChar('c') && r.skipChar('o') && r.skipChar('d') && r.skipChar('i') && r.skipChar('n') && r.skipChar('g'))
										{
												ch = r.Char;
												if (XMLChar.isSpaceChar(ch))
														ch = CharAfterS;
												if (ch == '=')
												{
														/*System.out.println(
														" " + (temp_offset) + " " + 8 + " dec attr name (encoding) " + depth);*/
														if (singleByteEncoding)
																_writeVTD(temp_offset, 8, TOKEN_DEC_ATTR_NAME, depth);
														else
																_writeVTD(temp_offset >> 1, 8, TOKEN_DEC_ATTR_NAME, depth);
												}
												else
														throw new ParseException("XML decl error: Invalid char" + formatLineNumber());
												ch_temp = CharAfterS;
												if (ch_temp != '"' && ch_temp != '\'')
														throw new ParseException("XML decl error: Invalid char to start attr name" + formatLineNumber());
												temp_offset = offset;
												ch = r.Char;
												switch (ch)
												{

														case 'a':
														case 'A':
																if ((r.skipChar('s') || r.skipChar('S')) && (r.skipChar('c') || r.skipChar('C')) && (r.skipChar('i') || r.skipChar('I')) && (r.skipChar('i') || r.skipChar('I')) && r.skipChar(ch_temp))
																{
																		if (encoding != FORMAT_UTF_16LE && encoding != FORMAT_UTF_16BE)
																		{
																				if (must_utf_8)
																						throw new EncodingException("Can't switch from UTF-8" + formatLineNumber());
																				encoding = FORMAT_ASCII;
																				r = new ASCIIReader(this);
																				/*System.out.println(
																				" " + (temp_offset) + " " + 5 + " dec attr val (encoding) " + depth);*/

																				_writeVTD(temp_offset, 5, TOKEN_DEC_ATTR_VAL, depth);

																				break;
																		}
																		else
																				throw new ParseException("XML decl error: Can't switch encoding to ASCII" + formatLineNumber());
																}
																throw new ParseException("XML decl error: Invalid Encoding" + formatLineNumber());

														case 'c':
														case 'C':
																matchCPEncoding();
																break;

														case 'i':
														case 'I':
																matchISOEncoding();
																break;

														case 'u':
														case 'U':
																matchUTFEncoding();
																break;
														// now deal with windows encoding

														case 'w':
														case 'W':
																matchWindowsEncoding();
																break;

														default:
																throw new ParseException("XML decl Error: invalid encoding" + formatLineNumber());

												}
												ch = r.Char;
												if (XMLChar.isSpaceChar(ch))
														ch = CharAfterS;
												temp_offset = offset - increment;
										}
										else
												throw new ParseException("XML decl Error: Invalid char" + formatLineNumber());
								}

								if (ch == 's')
								{
										if (r.skipChar('t') && r.skipChar('a') && r.skipChar('n') && r.skipChar('d') && r.skipChar('a') && r.skipChar('l') && r.skipChar('o') && r.skipChar('n') && r.skipChar('e'))
										{

												ch = CharAfterS;
												if (ch != '=')
														throw new ParseException("XML decl error: Invalid char" + formatLineNumber());
												/*System.out.println(
												" " + temp_offset + " " + 3 + " dec attr name (standalone) " + depth);*/
												if (singleByteEncoding)
														_writeVTD(temp_offset, 10, TOKEN_DEC_ATTR_NAME, depth);
												else
														_writeVTD(temp_offset >> 1, 10, TOKEN_DEC_ATTR_NAME, depth);
												ch_temp = CharAfterS;
												temp_offset = offset;
												if (ch_temp != '"' && ch_temp != '\'')
														throw new ParseException("XML decl error: Invalid char to start attr name" + formatLineNumber());
												ch = r.Char;
												if (ch == 'y')
												{
														if (r.skipChar('e') && r.skipChar('s') && r.skipChar(ch_temp))
														{
																/*System.out.println(
																" " + (temp_offset) + " " + 3 + " dec attr val (standalone) " + depth);*/
																if (singleByteEncoding)
																		_writeVTD(temp_offset, 3, TOKEN_DEC_ATTR_VAL, depth);
																else
																		_writeVTD(temp_offset >> 1, 3, TOKEN_DEC_ATTR_VAL, depth);
														}
														else
																throw new ParseException("XML decl error: invalid val for standalone" + formatLineNumber());
												}
												else if (ch == 'n')
												{
														if (r.skipChar('o') && r.skipChar(ch_temp))
														{
																/*System.out.println(
																" " + (temp_offset) + " " + 2 + " dec attr val (standalone)" + depth);*/
																if (singleByteEncoding)
																		_writeVTD(temp_offset, 2, TOKEN_DEC_ATTR_VAL, depth);
																else
																		_writeVTD(temp_offset >> 1, 2, TOKEN_DEC_ATTR_VAL, depth);
														}
														else
																throw new ParseException("XML decl error: invalid val for standalone" + formatLineNumber());
												}
												else
														throw new ParseException("XML decl error: invalid val for standalone" + formatLineNumber());
										}
										else
												throw new ParseException("XML decl error" + formatLineNumber());
										ch = r.Char;
										if (XMLChar.isSpaceChar(ch))
												ch = CharAfterS;
								}
						}

						if (ch == '?' && r.skipChar('>'))
						{
								temp_offset = offset;
								ch = CharAfterS;
								if (ch == '<')
								{
										parser_state = STATE_LT_SEEN;
								}
								else
										throw new ParseException("Other Error: Invalid Char in XML" + formatLineNumber());
						}
						else
								throw new ParseException("XML decl Error: Invalid termination sequence" + formatLineNumber());
						return parser_state;
				}
				/// <summary> This private method processes PI tag</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_pi_tag()
				{
						//int length1;
						int parser_state;
						while (true)
						{
								ch = r.Char;
								if (!XMLChar.isNameChar(ch))
										break;
						}

						length1 = offset - temp_offset - increment;
						/*System.out.println(
						((char) XMLDoc[temp_offset])
						+ " "
						+ (temp_offset)
						+ " "
						+ length1
						+ " PI Target "
						+ depth); */
						if (singleByteEncoding)
						{
								if (length1 > MAX_TOKEN_LENGTH)
										throw new ParseException("Token Length Error:" + " PI name too long (>0xfffff)" + formatLineNumber());
								_writeVTD((temp_offset), length1, TOKEN_PI_NAME, depth);
						}
						else
						{
								if (length1 > (MAX_TOKEN_LENGTH << 1))
										throw new ParseException("Token Length Error:" + " PI name too long (>0xfffff)" + formatLineNumber());
								_writeVTD((temp_offset) >> 1, (length1 >> 1), TOKEN_PI_NAME, depth);
						}
						//length1 = 0;
						/*temp_offset = offset;
						if (XMLChar.isSpaceChar(ch))
						{
								ch = r.Char;
						}*/
						if (ch == '?')
						{
								if (singleByteEncoding)
								{                    
										_writeVTD((temp_offset), 0, TOKEN_PI_VAL, depth);
								}
								else
								{                    
										_writeVTD((temp_offset) >> 1, 0, TOKEN_PI_VAL, depth);
								}
								if (r.skipChar('>'))
								{
										temp_offset = offset;
										//ch = CharAfterSe;
										ch = CharAfterS;
										if (ch == '<')
										{
												if (ws)
														addWhiteSpaceRecord();
												parser_state = STATE_LT_SEEN;
										}
										else if (XMLChar.isContentChar(ch))
										{
												parser_state = STATE_TEXT;
										}
										else if (ch == '&')
										{
												//has_amp = true;
												entityIdentifier();
												parser_state = STATE_TEXT;
										}
										else if (ch == ']')
										{
												if (r.skipChar(']'))
												{
														while (r.skipChar(']'))
														{
														}
														if (r.skipChar('>'))
																throw new ParseException("Error in text content: ]]> in text content" + formatLineNumber());
												}
												parser_state = STATE_TEXT;
										}
										else
												throw new ParseException("Error in text content: Invalid char" + formatLineNumber());
										return parser_state;
								}
								else
										throw new ParseException("Error in PI: invalid termination sequence" + formatLineNumber());
						}
						parser_state = STATE_PI_VAL;
						return parser_state;
				}
				/// <summary> This private method processes PI val </summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_pi_val()
				{
						int parser_state;
						//int length1;
						if (!XMLChar.isSpaceChar(ch))
								throw new ParseException(
												"Error in PI: invalid termination sequence"
														+ formatLineNumber());
						temp_offset = offset;
						ch = r.Char;
						while (true)
						{
								if (XMLChar.isValidChar(ch))
								{
										//System.out.println(""+(char)ch);
										if (ch == '?')
												if (r.skipChar('>'))
												{
														break;
												}
										/*else
												throw new ParseException("Error in PI: invalid termination sequence for PI" + formatLineNumber());*/
								}
								else
										throw new ParseException("Errors in PI: Invalid char in PI val" + formatLineNumber());
								ch = r.Char;
						}
						length1 = offset - temp_offset - (increment << 1);
						/*System.out.println(
						((char) XMLDoc[temp_offset])
						+ " "
						+ (temp_offset)
						+ " "
						+ length1
						+ " PI val "
						+ depth);*/
						if (singleByteEncoding)
						{
								if (length1 > MAX_TOKEN_LENGTH)
										throw new ParseException("Token Length Error:" + "PI VAL too long (>0xfffff)" + formatLineNumber());
								_writeVTD(temp_offset, length1, TOKEN_PI_VAL, depth);
						}
						else
						{
								if (length1 > (MAX_TOKEN_LENGTH << 1))
										throw new ParseException("Token Length Error:" + "PI VAL too long (>0xfffff)" + formatLineNumber());
								_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_PI_VAL, depth);
						}
						//length1 = 0;
						temp_offset = offset;
						ch = CharAfterS;
						//ch = CharAfterSe;

						if (ch == '<')
						{
								if (ws)
										addWhiteSpaceRecord();
								parser_state = STATE_LT_SEEN;
						}
						else if (XMLChar.isContentChar(ch))
						{
								//temp_offset = offset;
								parser_state = STATE_TEXT;
						}
						else if (ch == '&')
						{
								//has_amp = true;
								//temp_offset = offset;
								entityIdentifier();
								parser_state = STATE_TEXT;
						}
						else if (ch == ']')
						{
								if (r.skipChar(']'))
								{
										while (r.skipChar(']'))
										{
										}
										if (r.skipChar('>'))
												throw new ParseException("Error in text content: ]]> in text content" + formatLineNumber());
								}
								parser_state = STATE_TEXT;
						}
						else
								throw new ParseException("Error in text content: Invalid char" + formatLineNumber());
						return parser_state;
				}
				/// <summary> This private method process comment</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_comment()
				{
						int parser_state, length1;
						while (true)
						{
								ch = r.Char;
								if (XMLChar.isValidChar(ch))
								{
										if (ch == '-' && r.skipChar('-'))
										{
												length1 = offset - temp_offset - (increment << 1);
												break;
										}
								}
								else
										throw new ParseException("Error in comment: Invalid Char" + formatLineNumber());
						}
						if (r.Char == '>')
						{
								//System.out.println(" " + (temp_offset) + " " + length1 + " comment " + depth);
								if (singleByteEncoding)
										writeVTDText(temp_offset, length1, TOKEN_COMMENT, depth);
								else
										writeVTDText(temp_offset >> 1, length1 >> 1, TOKEN_COMMENT, depth);
								//length1 = 0;
								temp_offset = offset;
								//ch = CharAfterSe;
								ch = CharAfterS;
								if (ch == '<')
								{
										if (ws)
												addWhiteSpaceRecord();
										parser_state = STATE_LT_SEEN;
								}
								else if (XMLChar.isContentChar(ch))
								{
										//temp_offset = offset;
										parser_state = STATE_TEXT;
								}
								else if (ch == '&')
								{
										//has_amp = true;
										//temp_offset = offset;
										entityIdentifier();
										parser_state = STATE_TEXT;
								}
								else if (ch == ']')
								{
										if (r.skipChar(']'))
										{
												while (r.skipChar(']'))
												{
												}
												if (r.skipChar('>'))
														throw new ParseException("Error in text content: ]]> in text content" + formatLineNumber());
										}
										parser_state = STATE_TEXT;
								}
								else
										throw new ParseException("Error in text content: Invalid char" + formatLineNumber());
								return parser_state;
						}
						else
								throw new ParseException("Error in comment: Invalid terminating sequence" + formatLineNumber());
				}


				private int process_start_doc()
				{
						int c = r.Char;
						if (c == '<')
						{
								temp_offset = offset;
								// xml decl has to be right after the start of the document
								if (r.skipChar('?') && (r.skipChar('x') || r.skipChar('X')) && (r.skipChar('m') || r.skipChar('M')) && (r.skipChar('l') || r.skipChar('L')))
								{
										if (r.skipChar(' ') || r.skipChar('\t') || r.skipChar('\n') || r.skipChar('\r'))
										{
												ch = CharAfterS;
												temp_offset = offset;
												return STATE_DEC_ATTR_NAME;
										}
										else if (r.skipChar('?'))
												throw new ParseException("Error in XML decl: Premature ending" + formatLineNumber());
								}
								offset = temp_offset;
								return STATE_LT_SEEN;
						}
						else
						{
								if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
								{
										if (CharAfterS == '<')
										{
												return STATE_LT_SEEN; ;
										}
								}
						}


						throw new ParseException("Other Error: XML not starting properly" + formatLineNumber());
				}

				private int process_end_doc()
				{
						// eof exception should be thrown here for premature ending
						if (ch == '<')
						{

								if (r.skipChar('?'))
								{
										// processing instruction after end tag of root element
										temp_offset = offset;
										return STATE_END_PI;
								}
								else if (r.skipChar('!') && r.skipChar('-') && r.skipChar('-'))
								{
										// comments allowed after the end tag of the root element
										temp_offset = offset;
										return STATE_END_COMMENT;
								}
						}
						throw new ParseException("Other Error: XML not terminated properly" + formatLineNumber());

				}

				private int process_qm_seen()
				{
						temp_offset = offset;
						ch = r.Char;
						if (XMLChar.isNameStartChar(ch))
						{
								//temp_offset = offset;
								if ((ch == 'x' || ch == 'X')
										&& (r.skipChar('m') || r.skipChar('M'))
										&& (r.skipChar('l') || r.skipChar('L')))
								{
										ch = r.Char;
										if (ch == '?' || XMLChar.isSpaceChar(ch))
												throw new ParseException
														("Error in PI: [xX][mM][lL] not a valid PI targetname"
																+ formatLineNumber());
										offset = PrevOffset;
								}
								return STATE_PI_TAG;
						}

						throw new ParseException("Other Error: First char after <? invalid" + formatLineNumber());
				}

				private int process_ex_seen()
				{
						int parser_state;
						bool hasDTD = false;
						ch = r.Char;
						switch (ch)
						{
								case '-':
										if (r.skipChar('-'))
										{
												temp_offset = offset;
												parser_state = STATE_COMMENT;
												break;
										}
										else
												throw new ParseException
														("Error in comment: Invalid char sequence to start a comment"
														+ formatLineNumber());
								//goto case '[';

								case '[':
										if (r.skipChar('C')
												&& r.skipChar('D')
												&& r.skipChar('A')
												&& r.skipChar('T')
												&& r.skipChar('A')
												&& r.skipChar('[')
												&& (depth != -1))
										{
												temp_offset = offset;
												parser_state = STATE_CDATA;
												break;
										}
										else
										{
												if (depth == -1)
														throw new ParseException
																("Error in CDATA: Wrong place for CDATA"
																+ formatLineNumber());
												throw new ParseException
														("Error in CDATA: Invalid char sequence for CDATA"
														+ formatLineNumber());
										}
								//goto case 'D';


								case 'D':
										if (r.skipChar('O')
												&& r.skipChar('C')
												&& r.skipChar('T')
												&& r.skipChar('Y')
												&& r.skipChar('P')
												&& r.skipChar('E')
												&& (depth == -1)
												&& !hasDTD)
										{
												hasDTD = true;
												temp_offset = offset;
												parser_state = STATE_DOCTYPE;
												break;
										}
										else
										{
												if (hasDTD == true)
														throw new ParseException
																("Error for DOCTYPE: Only DOCTYPE allowed"
																+ formatLineNumber());
												if (depth != -1)
														throw new ParseException
																("Error for DOCTYPE: DTD at wrong place"
																+ formatLineNumber());
												throw new ParseException
														("Error for DOCTYPE: Invalid char sequence for DOCTYPE"
														+ formatLineNumber());
										}
								//goto default;

								default:
										throw new ParseException("Other Error: Unrecognized char after <!" + formatLineNumber());

						}
						return parser_state;
				}
				/// <summary> This private method processes CDATA section</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_cdata()
				{
						int parser_state, length1;
						while (true)
						{
								ch = r.Char;
								if (XMLChar.isValidChar(ch))
								{
										if (ch == ']' && r.skipChar(']'))
										{
												while (r.skipChar(']'))
														;
												if (r.skipChar('>'))
												{
														break;
												}
												/*else
														throw new ParseException("Error in CDATA: Invalid termination sequence" + formatLineNumber());*/
										}
								}
								else
										throw new ParseException("Error in CDATA: Invalid Char" + formatLineNumber());
						}
						length1 = offset - temp_offset - (increment << 1) - increment;
						if (singleByteEncoding)
						{

								writeVTDText(temp_offset, length1, TOKEN_CDATA_VAL, depth);
						}
						else
						{

								writeVTDText(temp_offset >> 1, length1 >> 1, TOKEN_CDATA_VAL, depth);
						}
						//System.out.println(" " + (temp_offset) + " " + length1 + " CDATA " + depth);
						temp_offset = offset;
						//ch = CharAfterSe;
						ch = CharAfterS;
						if (ch == '<')
						{
								if (ws)
										addWhiteSpaceRecord();
								parser_state = STATE_LT_SEEN;
						}
						else if (XMLChar.isContentChar(ch))
						{
								//temp_offset = offset -1;
								parser_state = STATE_TEXT;
						}
						else if (ch == '&')
						{
								//has_amp = true;
								//temp_offset = offset-1;
								entityIdentifier();
								parser_state = STATE_TEXT;
								//temp_offset = offset;
						}
						else if (ch == ']')
						{
								//temp_offset = offset - 1;
								if (r.skipChar(']'))
								{
										while (r.skipChar(']'))
										{
										}
										if (r.skipChar('>'))
												throw new ParseException("Error in text content: ]]> in text content" + formatLineNumber());
								}
								parser_state = STATE_TEXT;
						}
						else
								throw new ParseException("Other Error: Invalid char in xml" + formatLineNumber());
						return parser_state;
				}

				/// <summary> This private method process DTD</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_doc_type()
				{
						int z = 1, length1, parser_state;
						while (true)
						{
								ch = r.Char;
								if (XMLChar.isValidChar(ch))
								{
										if (ch == '>')
												z--;
										else if (ch == '<')
												z++;
										if (z == 0)
												break;
								}
								else
										throw new ParseException("Error in DOCTYPE: Invalid char" + formatLineNumber());
						}
						length1 = offset - temp_offset - increment;
						/*System.out.println(
						" " + (temp_offset) + " " + length1 + " DOCTYPE val " + depth);*/
						if (singleByteEncoding)
						{
								if (length1 > MAX_TOKEN_LENGTH)
										throw new ParseException("Token Length Error:" + " DTD val too long (>0xfffff)" + formatLineNumber());
								_writeVTD(temp_offset, length1, TOKEN_DTD_VAL, depth);
						}
						else
						{
								if (length1 > (MAX_TOKEN_LENGTH << 1))
										throw new ParseException("Token Length Error:" + " DTD val too long (>0xfffff)" + formatLineNumber());
								_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_DTD_VAL, depth);
						}
						ch = CharAfterS;
						if (ch == '<')
						{
								parser_state = STATE_LT_SEEN;
						}
						else
								throw new ParseException("Other Error: Invalid char in xml" + formatLineNumber());
						return parser_state;
				}

				/// <summary> This private method processes PI after root document </summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				/// <throws>  EncodingException </throws>
				/// <throws>  EOFException </throws>
				private int process_end_pi()
				{
						int length1, parser_state;
						ch = r.Char;
						if (XMLChar.isNameStartChar(ch))
						{
								if ((ch == 'x' || ch == 'X') && (r.skipChar('m') || r.skipChar('M')) && (r.skipChar('l') && r.skipChar('L')))
								{
										//temp_offset = offset;
										ch = r.Char;
										if (XMLChar.isSpaceChar(ch) || ch == '?')
												throw new ParseException("Error in PI: [xX][mM][lL] not a valid PI target" + formatLineNumber());
										//offset = temp_offset;
								}

								while (true)
								{
										//ch = getChar();
										if (!XMLChar.isNameChar(ch))
										{
												break;
										}
										ch = r.Char;
								}

								length1 = offset - temp_offset - increment;
								/*System.out.println(
								""
								+ (char) XMLDoc[temp_offset]
								+ " "
								+ (temp_offset)
								+ " "
								+ length1
								+ " PI Target "
								+ depth);*/
								if (singleByteEncoding)
								{
										if (length1 > MAX_TOKEN_LENGTH)
												throw new ParseException("Token Length Error:" + "PI name too long (>0xfffff)" + formatLineNumber());
										_writeVTD(temp_offset, length1, TOKEN_PI_NAME, depth);
								}
								else
								{
										if (length1 > (MAX_TOKEN_LENGTH << 1))
												throw new ParseException("Token Length Error:" + "PI name too long (>0xfffff)" + formatLineNumber());
										_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_PI_NAME, depth);
								}
								//length1 = 0;
								temp_offset = offset;
								if (XMLChar.isSpaceChar(ch))
								{
										ch = CharAfterS;

										while (true)
										{
												if (XMLChar.isValidChar(ch))
												{
														if (ch == '?')
																if (r.skipChar('>'))
																{
																		parser_state = STATE_DOC_END;
																		break;
																}
																else
																		throw new ParseException("Error in PI: invalid termination sequence" + formatLineNumber());
												}
												else
														throw new ParseException("Error in PI: Invalid char in PI val" + formatLineNumber());
												ch = r.Char;
										}
										length1 = offset - temp_offset - (increment << 1);
										if (singleByteEncoding)
										{
												if (length1 > MAX_TOKEN_LENGTH)
														throw new ParseException("Token Length Error:" + "PI val too long (>0xfffff)" + formatLineNumber());
												_writeVTD(temp_offset, length1, TOKEN_PI_VAL, depth);
										}
										else
										{
												if (length1 > (MAX_TOKEN_LENGTH << 1))
														throw new ParseException("Token Length Error:" + "PI val too long (>0xfffff)" + formatLineNumber());
												_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_PI_VAL, depth);
										}
										//System.out.println(" " + temp_offset + " " + length1 + " PI val " + depth);
								}
								else
								{
										if (singleByteEncoding)
										{
												_writeVTD((temp_offset), 0, TOKEN_PI_VAL, depth);
										}
										else
										{
												_writeVTD((temp_offset) >> 1, 0, TOKEN_PI_VAL, depth);
										}
										if ((ch == '?') && r.skipChar('>'))
										{
												parser_state = STATE_DOC_END;
										}
										else
												throw new ParseException("Error in PI: invalid termination sequence" + formatLineNumber());
								}
								//parser_state = STATE_DOC_END;
						}
						else
								throw new ParseException("Error in PI: invalid char in PI target" + formatLineNumber());
						return parser_state;
				}
				/// <summary> This private method process the comment after the root document</summary>
				/// <returns> the parser state after which the parser loop jumps to
				/// </returns>
				/// <throws>  ParseException </throws>
				private int process_end_comment()
				{
						int parser_state;
						//int length1;
						while (true)
						{
								ch = r.Char;
								if (XMLChar.isValidChar(ch))
								{
										if (ch == '-' && r.skipChar('-'))
										{
												length1 = offset - temp_offset - (increment << 1);
												break;
										}
								}
								else
										throw new ParseException("Error in comment: Invalid Char" + formatLineNumber());
						}
						if (r.Char == '>')
						{
								//System.out.println(" " + temp_offset + " " + length1 + " comment " + depth);
								if (singleByteEncoding)
										_writeVTD(temp_offset, length1, TOKEN_COMMENT, depth);
								else
										_writeVTD(temp_offset >> 1, length1 >> 1, TOKEN_COMMENT, depth);
								//length1 = 0;
								parser_state = STATE_DOC_END;
								return parser_state;
						}
						throw new ParseException("Error in comment: '-->' expected" + formatLineNumber());
				}
				/// <summary> The buffer-reuse version of setDoc
				/// The concept is to reuse LC and VTD buffer for 
				/// XML parsing, instead of allocating every time
				/// </summary>
				/// <param name="ba">*
				/// </param>
				public void setDoc_BR(byte[] ba)
				{
						setDoc_BR(ba, 0, ba.Length);
				}

				/// <summary> The buffer-reuse version of setDoc
				/// The concept is to reuse LC and VTD buffer for 
				/// XML parsing, instead of allocating every time
				/// </summary>
				/// <param name="ba">byte[]
				/// </param>
				/// <param name="os">int (in byte)
				/// </param>
				/// <param name="len">int (in byte)
				/// 
				/// </param>
				public void setDoc_BR(byte[] ba, int os, int len)
				{
						int a;
						br = true;
						depth = -1;
						increment = 1;
						BOM_detected = false;
						must_utf_8 = false;
						ch = ch_temp = 0;
						temp_offset = 0;
						XMLDoc = ba;
						docOffset = offset = os;
						docLen = len;
						endOffset = os + len;
						last_l1_index = last_l2_index = last_l3_index = last_depth = 0;
						//int i1 = 7, i2 = 9, i3 = 11;
						currentElementRecord = 0;
						nsBuffer1.size_Renamed_Field = 0;
						nsBuffer2.size_Renamed_Field = 0;
						nsBuffer3.size_Renamed_Field = 0;
						r = new UTF8Reader(this);
						if (shallowDepth)
						{
								int i1 = 7, i2 = 9, i3 = 11;
								if (docLen <= 1024)
								{
										// a = 1024; //set the floor
										a = 6;
										i1 = 5;
										i2 = 5;
										i3 = 5;
								}
								else if (docLen <= 4096)
								{
										a = 7;
										i1 = 6;
										i2 = 6;
										i3 = 6;
								}
								else if (docLen <= 1024 * 16)
								{
										a = 8;
										i1 = 7;
										i2 = 7;
										i3 = 7;
								}
								else if (docLen <= 1024 * 16 * 4)
								{
										// a = 2048;
										a = 11;
								}
								else if (docLen <= 1024 * 256)
								{
										// a = 1024 * 4;
										a = 12;
								}
								else
								{
										// a = 1 << 15;
										a = 15;
								}

								VTDBuffer = new FastLongBuffer(a, len >> (a + 1));
								l1Buffer = new FastLongBuffer(i1);
								l2Buffer = new FastLongBuffer(i2);
								l3Buffer = new FastIntBuffer(i3);
						}
						else
						{

								int i1 = 7, i2 = 9, i3 = 11, i4 = 11, i5 = 11;
								if (docLen <= 1024)
								{
										// a = 1024; //set the floor
										a = 6;
										i1 = 5;
										i2 = 5;
										i3 = 5;
										i4 = 5;
										i5 = 5;
								}
								else if (docLen <= 4096)
								{
										a = 7;
										i1 = 6;
										i2 = 6;
										i3 = 6;
										i4 = 6;
										i5 = 6;
								}
								else if (docLen <= 1024 * 16)
								{
										a = 8;
										i1 = 7;
										i2 = 7;
										i3 = 7;
										i4 = 7;
										i5 = 7;
								}
								else if (docLen <= 1024 * 16 * 4)
								{
										// a = 2048;
										a = 11;
										i2 = 8;
										i3 = 8;
										i4 = 8;
										i5 = 8;
								}
								else if (docLen <= 1024 * 256)
								{
										// a = 1024 * 4;
										a = 12;
										i1 = 8;
										i2 = 9;
										i3 = 9;
										i4 = 9;
										i5 = 9;
								}
								else
								{
										// a = 1 << 15;
										a = 15;
								}

								VTDBuffer = new FastLongBuffer(a, len >> (a + 1));
								l1Buffer = new FastLongBuffer(i1);
								l2Buffer = new FastLongBuffer(i2);
								_l3Buffer = new FastLongBuffer(i3);
								_l4Buffer = new FastLongBuffer(i4);
								_l5Buffer = new FastIntBuffer(i5);
						}
				}

				/// <summary> Set the XMLDoc container.</summary>
				/// <param name="ba">byte[]
				/// </param>
				public void setDoc(byte[] ba)
				{
						setDoc(ba, 0, ba.Length);
				}
				/// <summary> Set the XMLDoc container. Also set the offset and len of the document 
				/// with respect to the container.
				/// </summary>
				/// <param name="ba">byte[]
				/// </param>
				/// <param name="os">int (in byte)
				/// </param>
				/// <param name="len">int (in byte)
				/// </param>
				public void setDoc(byte[] ba, int os, int len)
				{
						int a;
						br = false;
						depth = -1;
						increment = 1;
						BOM_detected = false;
						must_utf_8 = false;
						ch = ch_temp = 0;
						temp_offset = 0;
						XMLDoc = ba;
						docOffset = offset = os;
						docLen = len;
						endOffset = os + len;
						last_l1_index = last_l2_index = last_l3_index = last_depth = 0;
						//int i1 = 7, i2 = 9, i3 = 11,i4,i5;
						currentElementRecord = 0;
						nsBuffer1.size_Renamed_Field = 0;
						nsBuffer2.size_Renamed_Field = 0;
						nsBuffer3.size_Renamed_Field = 0;
						//r = new UTF8Reader();
						r = new UTF8Reader(this);
						if (shallowDepth)
						{
								int i1 = 7, i2 = 9,  i3 = 11;
								if (docLen <= 1024)
								{
										// a = 1024; //set the floor
										a = 6;
										i1 = 5;
										i2 = 5;
										i3 = 5;
								}
								else if (docLen <= 4096)
								{
										a = 7;
										i1 = 6;
										i2 = 6;
										i3 = 6;
								}
								else if (docLen <= 1024 * 16)
								{
										a = 8;
										i1 = 7;
										i2 = 7;
										i3 = 7;
								}
								else if (docLen <= 1024 * 16 * 4)
								{
										// a = 2048;
										a = 11;
										i2 = 8;
										i3 = 8;
								}
								else if (docLen <= 1024 * 256)
								{
										// a = 1024 * 4;
										a = 12;
								}
								else
								{
										// a = 1 << 15;
										a = 15;
								}
								if (VTDBuffer == null)
								{
										VTDBuffer = new FastLongBuffer(a, len >> (a + 1));
										l1Buffer = new FastLongBuffer(i1);
										l2Buffer = new FastLongBuffer(i2);
										l3Buffer = new FastIntBuffer(i3);
								}
								else
								{
										VTDBuffer.size_Renamed_Field = 0;
										l1Buffer.size_Renamed_Field = 0;
										l2Buffer.size_Renamed_Field = 0;
										l3Buffer.size_Renamed_Field = 0;
								}
						}
						else
						{
								int i1 = 7, i2 = 9, i3 = 11, i4 = 11, i5 = 11;
								if (docLen <= 1024)
								{
										// a = 1024; //set the floor
										a = 6;
										i1 = 5;
										i2 = 5;
										i3 = 5;
										i4 = 5;
										i5 = 5;
								}
								else if (docLen <= 4096)
								{
										a = 7;
										i1 = 6;
										i2 = 6;
										i3 = 6;
										i4 = 6;
										i5 = 6;
								}
								else if (docLen <= 1024 * 16)
								{
										a = 8;
										i1 = 7;
										i2 = 7;
										i3 = 7;
								}
								else if (docLen <= 1024 * 16 * 4)
								{
										// a = 2048;
										a = 11;
										i2 = 8;
										i3 = 8;
										i4 = 8;
										i5 = 8;
								}
								else if (docLen <= 1024 * 256)
								{
										// a = 1024 * 4;
										a = 12;
										i1 = 8;
										i2 = 9;
										i3 = 9;
										i4 = 9;
										i5 = 9;
								}
								else if (docLen <= 1024 * 1024)
								{
										// a = 1024 * 4;
										a = 12;
										i1 = 8;
										i3 = 10;
										i4 = 10;
										i5 = 10;
								}
								else
								{
										// a = 1 << 15;
										a = 15;
										i1 = 8;
								}
								if (VTDBuffer == null)
								{
										VTDBuffer = new FastLongBuffer(a, len >> (a + 1));
										l1Buffer = new FastLongBuffer(i1);
										l2Buffer = new FastLongBuffer(i2);
										_l3Buffer = new FastLongBuffer(i3);
										_l4Buffer = new FastLongBuffer(i4);
										_l5Buffer = new FastIntBuffer(i5);
								}
								else
								{
										VTDBuffer.size_Renamed_Field = 0;
										l1Buffer.size_Renamed_Field = 0;
										l2Buffer.size_Renamed_Field = 0;
										_l3Buffer.size_Renamed_Field = 0;
										_l4Buffer.size_Renamed_Field = 0;
										_l5Buffer.size_Renamed_Field = 0;
								}
						}
				}
				/// <summary> Write the VTD and LC into their storage container.</summary>
				/// <param name="offset">int
				/// </param>
				/// <param name="length">int
				/// </param>
				/// <param name="token_type">int
				/// </param>
				/// <param name="depth">int
				/// </param>
				private void writeVTD(int offset, int length, int token_type, int depth)
				{
						VTDBuffer.append(((long)((token_type << 28) | ((depth & 0xff) << 20) | length) << 32) | offset);
						// remember VTD depth start from zero
						if (token_type == TOKEN_STARTING_TAG)
						{
								switch (depth)
								{

										case 0:
												rootIndex = VTDBuffer.size_Renamed_Field - 1;
												break;

										case 1:
												if (last_depth == 1)
												{
														l1Buffer.append(((long)last_l1_index << 32) | 0x00000000ffffffff);
												}
												else if (last_depth == 2)
												{
														l2Buffer.append(((long)last_l2_index << 32) | 0x00000000ffffffff);
												}
												last_l1_index = VTDBuffer.size_Renamed_Field - 1;
												last_depth = 1;
												break;

										case 2:
												if (last_depth == 1)
												{
														l1Buffer.append(((long)last_l1_index << 32) + l2Buffer.size_Renamed_Field);
												}
												else if (last_depth == 2)
												{
														l2Buffer.append(((long)last_l2_index << 32) | 0x00000000ffffffff);
												}
												last_l2_index = VTDBuffer.size_Renamed_Field - 1;
												last_depth = 2;
												break;


										case 3:
												l3Buffer.append(VTDBuffer.size_Renamed_Field - 1);
												if (last_depth == 2)
												{
														l2Buffer.append(((long)last_l2_index << 32) + l3Buffer.size_Renamed_Field - 1);
												}
												last_depth = 3;
												break;

										default:
												//rootIndex = VTDBuffer.size_Renamed_Field - 1;
												break;

								}
						} /*else if (token_type == TOKEN_ENDING_TAG && (depth == 0)) {
			if (last_depth == 1) {
			l1Buffer.append(((long) last_l1_index << 32) | 0xffffffffL);
			} else if (last_depth == 2) {
			l2Buffer.append(((long) last_l2_index << 32) | 0xffffffffL);
			}
			}*/
				}



				/// <summary>
				/// This method loads the VTD+XML from an input stream
				/// </summary>
				/// <param name="is_Renamed"></param>
				/// <returns>The VTDNav object</returns>

				public VTDNav loadIndex(System.IO.Stream is_Renamed)
				{
						IndexHandler.readIndex(is_Renamed, this);
						return getNav();
				}


				/// <summary>
				/// This method loads the VTD+XML from a file 
				/// </summary>
				/// <param name="fileName"></param>
				/// <returns>the VTDNav object</returns>
				public VTDNav loadIndex(System.String fileName)
				{
						//UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javalangString'"
						System.IO.FileStream fis = null;
						try
						{
								fis = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
								return loadIndex(fis);
						}
						finally
						{
								if (fis != null)
										fis.Close();
						}

				}


				public VTDNav loadSeparateIndex(String XMLFileName, String VTDFileName)
				{
						System.IO.FileStream fis = null;
						System.IO.FileStream xis = null;
						System.IO.FileInfo f = null;
						int size;
						try
						{
								f = new System.IO.FileInfo(XMLFileName);
								size = (int)f.Length;
								fis = new System.IO.FileStream(VTDFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
								xis = new System.IO.FileStream(XMLFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
								IndexHandler.readSeparateIndex(fis, xis, size, this);
								return getNav();
						}
						finally
						{
								if (fis != null)
										fis.Close();
						}
				}

				/// <summary>
				/// This method loads the VTD+XML from a byte array, assuming the first 32
				/// bytes are not XML bytes (but instead header of the VTD+XML index)
				/// </summary>
				/// <param name="ba"></param>
				/// <returns> The VTDNav object</returns>
				public VTDNav loadIndex(byte[] ba)
				{
						IndexHandler.readIndex(ba, this);
						return getNav();
				}
				/// <summary> This method writes the VTD+XML into an output streams</summary>
				/// <param name="os">
				/// </param>
				/// <throws>  IOException </throws>
				/// <throws>  IndexWriteException </throws>
				/// <summary> 
				/// </summary>
				public void writeIndex(System.IO.Stream os)
				{
						if (shallowDepth)
						IndexHandler.writeIndex_L3(1,
								 this.encoding,
								 this.ns,
								 true,
								 this.VTDDepth,
								 3,
								 this.rootIndex,
								 this.XMLDoc,
								 this.docOffset,
								 this.docLen,
								 this.VTDBuffer,
								 this.l1Buffer,
								 this.l2Buffer,
								 this.l3Buffer,
								 os);
						else
								IndexHandler.writeIndex_L5(1,
								 this.encoding,
								 this.ns,
								 true,
								 this.VTDDepth,
								 5,
								 this.rootIndex,
								 this.XMLDoc,
								 this.docOffset,
								 this.docLen,
								 this.VTDBuffer,
								 this.l1Buffer,
								 this.l2Buffer,
								 this._l3Buffer,
								 this._l4Buffer,
								 this._l5Buffer,
								 os);
				}

				/// <summary> This method writes the VTD+XML file into a file of the given name</summary>
				/// <param name="fileName">
				/// </param>
				/// <throws>  IOException </throws>
				/// <throws>  IndexWriteException </throws>
				/// <summary> 
				/// </summary>
				public void writeIndex(System.String fileName)
				{
						//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString'"
						System.IO.FileStream fos = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
						writeIndex(fos);
						fos.Close();
				}
				/// <summary>
				/// Precompute the size of VTD+XML index without actully generating it
				/// </summary>
				/// <returns> the VTD+XML index size</returns>

				public long getIndexSize()
				{
						int size;
						if ((docLen & 7) == 0)
								size = docLen;
						else
								size = ((docLen >> 3) + 1) << 3;

						size += (VTDBuffer.size_Renamed_Field << 3) +
										(l1Buffer.size_Renamed_Field << 3) +
										(l2Buffer.size_Renamed_Field << 3);

						if ((l3Buffer.size_Renamed_Field & 1) == 0)
						{ //even
								size += l3Buffer.size_Renamed_Field << 2;
						}
						else
						{
								size += (l3Buffer.size_Renamed_Field + 1) << 2; //odd
						}
						return size + 64;
				}


				public void writeSeparateIndex(String fileName)
				{
						System.IO.FileStream fos = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
						writeSeparateIndex(fos);
						fos.Close();
				}

				public void writeSeparateIndex(System.IO.Stream os)
				{
						if (shallowDepth)
						IndexHandler.writeSeparateIndex_L3((byte)2,
								this.encoding,
								this.ns,
								true,
								this.VTDDepth,
								3,
								this.rootIndex,
								// this.XMLDoc.getBytes(),
								this.docOffset,
								this.docLen,
								(FastLongBuffer)this.VTDBuffer,
								(FastLongBuffer)this.l1Buffer,
								(FastLongBuffer)this.l2Buffer,
								(FastIntBuffer)this.l3Buffer,
								os);
						else
								IndexHandler.writeSeparateIndex_L5(1,
								this.encoding,
								this.ns,
								true,
								this.VTDDepth,
								5,
								this.rootIndex,
								//this.XMLDoc,
								this.docOffset,
								this.docLen,
								(FastLongBuffer)this.VTDBuffer,
								(FastLongBuffer)this.l1Buffer,
								(FastLongBuffer)this.l2Buffer,
								(FastLongBuffer)this._l3Buffer,
								(FastLongBuffer)this._l4Buffer,
								(FastIntBuffer)this._l5Buffer,
								os);
				}
				/// <summary>
				/// 
				/// </summary>
				private void addWhiteSpaceRecord()
				{
						if (depth > -1)
						{
								int length1 = offset - increment - temp_offset;
								if (length1 != 0)
										if (encoding < FORMAT_UTF_16BE)
												writeVTD(temp_offset, length1, TOKEN_CHARACTER_DATA, depth);
										else
												writeVTD(temp_offset >> 1, length1 >> 1,
																TOKEN_CHARACTER_DATA, depth);
						}
				}
				/// <summary>
				/// Enable the parser to collect all white spaces, including the ignored white spaces
				/// By default, ignore white spaces are ignored
				/// </summary>
				/// <param name="b"></param>
				public void enableIgnoredWhiteSpace(bool b)
				{
						ws = b;
				}

				private void checkAttributeUniqueness()
				{
						bool unique = true;
						bool unequal;
						for (int i = 0; i < attr_count; i++)
						{
								unequal = false;
								int prevLen = (int)attr_name_array[i];
								if (length1 == prevLen)
								{
										int prevOffset =
												(int)(attr_name_array[i] >> 32);
										for (int j = 0; j < prevLen; j++)
										{
												if (XMLDoc[prevOffset + j]
														!= XMLDoc[temp_offset + j])
												{
														unequal = true;
														break;
												}
										}
								}
								else
										unequal = true;
								unique = unique && unequal;
						}
						if (!unique && attr_count != 0)
								throw new ParseException(
										"Error in attr: Attr name not unique"
												+ formatLineNumber());
						unique = true;
						if (attr_count < attr_name_array.Length)
						{
								attr_name_array[attr_count] =
										((long)(temp_offset) << 32) | length1;
								attr_count++;
						}
						else // grow the attr_name_array by 16
						{
								long[] temp_array = attr_name_array;
								/*System.out.println(
										"size increase from "
												+ temp_array.length
												+ "  to "
												+ (attr_count + 16));*/
								attr_name_array =
										new long[attr_count + ATTR_NAME_ARRAY_SIZE];
								Array.Copy(temp_array, 0, attr_name_array, 0, attr_count);
								/*for (int i = 0; i < attr_count; i++) {
										attr_name_array[i] = temp_array[i];
								}*/
								attr_name_array[attr_count] =
										((long)(temp_offset) << 32) | length1;
								attr_count++;
						}

						if (ns && !is_ns && length2 != 0)
						{
								if ((increment == 1 && length2 == 3 && matchXML(temp_offset))
												|| (increment == 2 && length2 == 6 && matchXML(temp_offset)))
								{
										return;
								}
								else if (prefixed_attr_count < prefixed_attr_name_array.Length)
								{
										prefixed_attr_name_array[prefixed_attr_count] =
												((long)(temp_offset) << 32) | (length2 << 16) | length1;
										prefixed_attr_count++;
								}
								else
								{
										long[] temp_array1 = prefixed_attr_name_array;
										prefixed_attr_name_array =
												new long[prefixed_attr_count + ATTR_NAME_ARRAY_SIZE];
										prefix_URL_array =
												new int[prefixed_attr_count + ATTR_NAME_ARRAY_SIZE];
										Array.Copy(temp_array1, 0, prefixed_attr_name_array, 0, prefixed_attr_count);
										//System.arraycopy(temp_array1, 0, prefixed_attr_val_array, 0, prefixed_attr_count)
										/*for (int i = 0; i < attr_count; i++) {
												attr_name_array[i] = temp_array[i];
										}*/
										prefixed_attr_name_array[prefixed_attr_count] =
												((long)(temp_offset) << 32) | (length2 << 16) | length1;
										prefixed_attr_count++;
								}
						}
				}

				private bool checkPrefix(int os, int len)
				{
						//int i=0;
						if (encoding < FORMAT_UTF_16BE)
						{
								if (len == 4 && XMLDoc[os] == 'x'
										&& XMLDoc[os + 1] == 'm' && XMLDoc[os + 2] == 'l')
								{
										return true;
								}
						}
						else if (encoding == FORMAT_UTF_16BE)
						{
								if (len == 8 && XMLDoc[os] == 0 && XMLDoc[os + 1] == 'x'
										&& XMLDoc[os + 2] == 0 && XMLDoc[os + 3] == 'm'
										&& XMLDoc[os + 4] == 0 && XMLDoc[os + 5] == 'l')
								{
										return true;
								}
						}
						else
						{
								if (len == 8 && XMLDoc[os] == 'x' && XMLDoc[os + 1] == 0
										&& XMLDoc[os + 2] == 'm' && XMLDoc[os + 3] == 0
										&& XMLDoc[os + 4] == 'l' && XMLDoc[os + 5] == 0)
								{
										return true;
								}
						}
						return false;
				}

				private bool checkPrefix2(int os, int len)
				{
						//int i=0;
						if (encoding < FORMAT_UTF_16BE)
						{
								if (len == 5 && XMLDoc[os] == 'x'
										&& XMLDoc[os + 1] == 'm' && XMLDoc[os + 2] == 'l'
										&& XMLDoc[os + 3] == 'n' && XMLDoc[os + 4] == 's')
								{
										return true;
								}
						}
						else if (encoding == FORMAT_UTF_16BE)
						{
								if (len == 10 && XMLDoc[os] == 0 && XMLDoc[os + 1] == 'x'
										&& XMLDoc[os + 2] == 0 && XMLDoc[os + 3] == 'm'
										&& XMLDoc[os + 4] == 0 && XMLDoc[os + 5] == 'l'
										&& XMLDoc[os + 6] == 0 && XMLDoc[os + 7] == 'n'
										&& XMLDoc[os + 8] == 0 && XMLDoc[os + 9] == 's'
								)
								{
										return true;
								}
						}
						else
						{
								if (len == 10 && XMLDoc[os] == 'x' && XMLDoc[os + 1] == 0
										&& XMLDoc[os + 2] == 'm' && XMLDoc[os + 3] == 0
										&& XMLDoc[os + 4] == 'l' && XMLDoc[os + 5] == 0
										&& XMLDoc[os + 6] == 'n' && XMLDoc[os + 3] == 0
										&& XMLDoc[os + 8] == 's' && XMLDoc[os + 5] == 0
								)
								{
										return true;
								}
						}
						return false;
				}

				private long _getCharResolved(int byte_offset)
				{

						int ch = 0;
						int val = 0;
						long inc = 2 << (increment - 1);
						long l = r._getChar(byte_offset);

						ch = (int)l;

						if (ch != '&')
								return l;

						// let us handle references here
						//currentOffset++;
						byte_offset += increment;
						ch = getCharUnit(byte_offset);
						byte_offset += increment;
						switch (ch)
						{
								case '#':

										ch = getCharUnit(byte_offset);

										if (ch == 'x')
										{
												while (true)
												{
														byte_offset += increment;
														inc += increment;
														ch = getCharUnit(byte_offset);

														if (ch >= '0' && ch <= '9')
														{
																val = (val << 4) + (ch - '0');
														}
														else if (ch >= 'a' && ch <= 'f')
														{
																val = (val << 4) + (ch - 'a' + 10);
														}
														else if (ch >= 'A' && ch <= 'F')
														{
																val = (val << 4) + (ch - 'A' + 10);
														}
														else if (ch == ';')
														{
																inc += increment;
																break;
														}
												}
										}
										else
										{
												while (true)
												{
														ch = getCharUnit(byte_offset);
														byte_offset += increment;
														inc += increment;
														if (ch >= '0' && ch <= '9')
														{
																val = val * 10 + (ch - '0');
														}
														else if (ch == ';')
														{
																break;
														}
												}
										}
										break;

								case 'a':
										ch = getCharUnit(byte_offset);
										if (encoding < FORMAT_UTF_16BE)
										{
												if (ch == 'm')
												{
														if (getCharUnit(byte_offset + 1) == 'p'
																&& getCharUnit(byte_offset + 2) == ';')
														{
																inc = 5;
																val = '&';
														}
												}
												else if (ch == 'p')
												{
														if (getCharUnit(byte_offset + 1) == 'o'
																&& getCharUnit(byte_offset + 2) == 's'
																&& getCharUnit(byte_offset + 3) == ';')
														{
																inc = 6;
																val = '\'';
														}
												}
										}
										else
										{
												if (ch == 'm')
												{
														if (getCharUnit(byte_offset + 2) == 'p'
																&& getCharUnit(byte_offset + 4) == ';')
														{
																inc = 10;
																val = '&';
														}
												}
												else if (ch == 'p')
												{
														if (getCharUnit(byte_offset + 2) == 'o'
																&& getCharUnit(byte_offset + 4) == 's'
																&& getCharUnit(byte_offset + 6) == ';')
														{
																inc = 12;
																val = '\'';
														}
												}
										}
										break;

								case 'q':

										if (encoding < FORMAT_UTF_16BE)
										{
												if (getCharUnit(byte_offset) == 'u'
														&& getCharUnit(byte_offset + 1) == 'o'
														&& getCharUnit(byte_offset + 2) == 't'
														&& getCharUnit(byte_offset + 3) == ';')
												{
														inc = 6;
														val = '\"';
												}
										}
										else
										{
												if (getCharUnit(byte_offset) == 'u'
														&& getCharUnit(byte_offset + 2) == 'o'
														&& getCharUnit(byte_offset + 4) == 't'
														&& getCharUnit(byte_offset + 6) == ';')
												{
														inc = 12;
														val = '\"';
												}
										}
										break;
								case 'l':
										if (encoding < FORMAT_UTF_16BE)
										{
												if (getCharUnit(byte_offset) == 't'
														&& getCharUnit(byte_offset + 1) == ';')
												{
														//offset += 2;
														inc = 4;
														val = '<';
												}
										}
										else
										{
												if (getCharUnit(byte_offset) == 't'
														&& getCharUnit(byte_offset + 2) == ';')
												{
														//offset += 2;
														inc = 8;
														val = '<';
												}
										}
										break;
								case 'g':
										if (encoding < FORMAT_UTF_16BE)
										{
												if (getCharUnit(byte_offset) == 't'
														&& getCharUnit(byte_offset + 1) == ';')
												{
														inc = 4;
														val = '>';
												}
										}
										else
										{
												if (getCharUnit(byte_offset) == 't'
														&& getCharUnit(byte_offset + 2) == ';')
												{
														inc = 8;
														val = '>';
												}
										}
										break;
						}

						//currentOffset++;
						return val | (inc << 32);
				}

				private int getCharUnit(int byte_offset)
				{
						return (encoding <= 2)
								? XMLDoc[byte_offset] & 0xff
								: (encoding < FORMAT_UTF_16BE)
								? r.decode(byte_offset) : (encoding == FORMAT_UTF_16BE)
								? (((int)XMLDoc[byte_offset])
										<< 8 | XMLDoc[byte_offset + 1])
								: (((int)XMLDoc[byte_offset + 1])
										<< 8 | XMLDoc[byte_offset]);
				}

				private bool matchURL(int bos1, int len1, int bos2, int len2)
				{
						long l1, l2;
						int i1 = bos1, i2 = bos2, i3 = bos1 + len1, i4 = bos2 + len2;
						//System.out.println("--->"+new String(XMLDoc, bos1, len1)+" "+new String(XMLDoc,bos2,len2));
						while (i1 < i3 && i2 < i4)
						{
								l1 = _getCharResolved(i1);
								l2 = _getCharResolved(i2);
								if ((int)l1 != (int)l2)
										return false;
								i1 += (int)(l1 >> 32);
								i2 += (int)(l2 >> 32);
						}
						if (i1 == i3 && i2 == i4)
								return true;
						return false;
				}

				private bool matchXML(int byte_offset)
				{
						// TODO Auto-generated method stub
						if (encoding < FORMAT_UTF_16BE)
						{
								if (XMLDoc[byte_offset] == 'x'
											 && XMLDoc[byte_offset + 1] == 'm'
											 && XMLDoc[byte_offset + 2] == 'l')
										return true;
						}
						else
						{
								if (encoding == FORMAT_UTF_16LE)
								{
										if (XMLDoc[byte_offset] == 'x' && XMLDoc[byte_offset + 1] == 0
												&& XMLDoc[byte_offset + 2] == 'm' && XMLDoc[byte_offset + 3] == 0
												&& XMLDoc[byte_offset + 4] == 'l' && XMLDoc[byte_offset + 5] == 0)
												return true;
								}
								else
								{
										if (XMLDoc[byte_offset] == 0 && XMLDoc[byte_offset + 1] == 'x'
														&& XMLDoc[byte_offset + 2] == 0 && XMLDoc[byte_offset + 3] == 'm'
														&& XMLDoc[byte_offset + 4] == 0 && XMLDoc[byte_offset + 5] == 'l')
												return true;
								}
						}
						return false;
				}

				private void disallow_xmlns(int byte_offset)
				{
						// TODO Auto-generated method stub
						if (encoding < FORMAT_UTF_16BE)
						{
								if (XMLDoc[byte_offset] == 'x'
											 && XMLDoc[byte_offset + 1] == 'm'
											 && XMLDoc[byte_offset + 2] == 'l'
											 && XMLDoc[byte_offset + 3] == 'n'
											 && XMLDoc[byte_offset + 4] == 's')
										throw new ParseException(
														"xmlns as a ns prefix can't be re-declared"
														+ formatLineNumber(offset));

						}
						else
						{
								if (encoding == FORMAT_UTF_16LE)
								{
										if (XMLDoc[byte_offset] == 'x' && XMLDoc[byte_offset + 1] == 0
												&& XMLDoc[byte_offset + 2] == 'm' && XMLDoc[byte_offset + 3] == 0
												&& XMLDoc[byte_offset + 4] == 'l' && XMLDoc[byte_offset + 5] == 0
												&& XMLDoc[byte_offset + 6] == 'n' && XMLDoc[byte_offset + 7] == 0
												&& XMLDoc[byte_offset + 8] == 's' && XMLDoc[byte_offset + 9] == 0)
												throw new ParseException(
																"xmlns as a ns prefix can't be re-declared"
																+ formatLineNumber(offset));
								}
								else
								{
										if (XMLDoc[byte_offset] == 0 && XMLDoc[byte_offset + 1] == 'x'
														&& XMLDoc[byte_offset + 2] == 0 && XMLDoc[byte_offset + 3] == 'm'
														&& XMLDoc[byte_offset + 4] == 0 && XMLDoc[byte_offset + 5] == 'l'
														&& XMLDoc[byte_offset + 6] == 0 && XMLDoc[byte_offset + 7] == 'n'
														&& XMLDoc[byte_offset + 8] == 0 && XMLDoc[byte_offset + 9] == 's')
												throw new ParseException(
																"xmlns as a ns prefix can't be re-declared"
																+ formatLineNumber(offset));
								}
						}
				}

				private String formatLineNumber(int os)
				{
						int so = docOffset;
						int lineNumber = 0;
						int lineOffset = 0;

						if (encoding < FORMAT_UTF_16BE)
						{
								while (so <= os - 1)
								{
										if (XMLDoc[so] == '\n')
										{
												lineNumber++;
												lineOffset = so;
										}
										//lineOffset++;
										so++;
								}
								lineOffset = os - lineOffset;
						}
						else if (encoding == FORMAT_UTF_16BE)
						{
								while (so <= os - 2)
								{
										if (XMLDoc[so + 1] == '\n' && XMLDoc[so] == 0)
										{
												lineNumber++;
												lineOffset = so;
										}
										so += 2;
								}
								lineOffset = (os - lineOffset) >> 1;
						}
						else
						{
								while (so <= os - 2)
								{
										if (XMLDoc[so] == '\n' && XMLDoc[so + 1] == 0)
										{
												lineNumber++;
												lineOffset = so;
										}
										so += 2;
								}
								lineOffset = (os - lineOffset) >> 1;
						}
						return "\nLine Number: " + (lineNumber + 1) + " Offset: " + (lineOffset - 1);
				}
				private int identifyNsURL(int byte_offset, int length)
				{
						// TODO Auto-generated method stub
						// URL points to "http://www.w3.org/XML/1998/namespace" return 1
						// URL points to "http://www.w3.org/2000/xmlns/" return 2
						String URL1 = "2000/xmlns/";
						String URL2 = "http://www.w3.org/XML/1998/namespace";
						long l;
						int i, t, g = byte_offset + length;
						int os = byte_offset;
						if (length < 29
										|| (increment == 2 && length < 58))
								return 0;

						for (i = 0; i < 18 && os < g; i++)
						{
								l = _getCharResolved(os);
								//System.out.println("char ==>"+(char)l);
								if (URL2[i] != (int)l)
										return 0;
								os += (int)(l >> 32);
						}

						//store offset value 
						t = os;

						for (i = 0; i < 11 && os < g; i++)
						{
								l = _getCharResolved(os);
								if (URL1[i] != (int)l)
										break;
								os += (int)(l >> 32);
						}
						if (os == g)
								return 2;

						//so far a match
						os = t;
						for (i = 18; i < 36 && os < g; i++)
						{
								l = _getCharResolved(os);
								if (URL2[i] != (int)l)
										return 0;
								os += (int)(l >> 32);
						}
						if (os == g)
								return 1;

						return 0;
				}
				private void qualifyAttributes()
				{
						int i1 = nsBuffer3.size_Renamed_Field - 1;
						int j = 0, i = 0;
						// two cases:
						// 1. the current element has no prefix, look for xmlns
						// 2. the current element has prefix, look for xmlns:something
						while (j < prefixed_attr_count)
						{
								int preLen = (int)((prefixed_attr_name_array[j] & 0xffff0000L) >> 16);
								int preOs = (int)(prefixed_attr_name_array[j] >> 32);
								//Console.WriteLine(new String(XMLDoc, preOs, preLen)+"===");
								i = i1;
								while (i >= 0)
								{
										int t = nsBuffer3.upper32At(i);
										// with prefix, get full length and prefix length
										if ((t & 0xffff) - (t >> 16) == preLen + increment)
										{
												// doing byte comparison here
												int os = nsBuffer3.lower32At(i) + (t >> 16) + increment;
												//System.out.println(new String(XMLDoc, os, preLen)+"");
												int k = 0;
												for (; k < preLen; k++)
												{
														//System.out.println(i+" "+(char)(XMLDoc[os+k])+"<===>"+(char)(XMLDoc[preOs+k]));
														if (XMLDoc[os + k] != XMLDoc[preOs + k])
																break;
												}
												if (k == preLen)
												{
														break; // found the match
												}
										}
										/*if ( (nsBuffer3.upper32At(i) & 0xffff0000) == 0){
												return;
										}*/
										i--;
								}
								if (i < 0)
										throw new ParseException("Name space qualification Exception: prefixed attribute not qualified\n"
														+ formatLineNumber(preOs));
								else
										prefix_URL_array[j] = i;
								j++;
								// no need to check if xml is the prefix
						}
						//for (int h=0;h<prefixed_attr_count;h++)
						//	System.out.print(" "+prefix_URL_array[h]);

						//System.out.println();
						// print line # column# and full element name
						//throw new ParseException("Name space qualification Exception: Element not qualified\n"
						//		+formatLineNumber((int)pref));

				}

				private void qualifyElement()
				{
						int i = nsBuffer3.size_Renamed_Field - 1;
						// two cases:
						// 1. the current element has no prefix, look for xmlns
						// 2. the current element has prefix, look for xmlns:something


						int preLen = (int)(((ulong)currentElementRecord & 0xffff000000000000L) >> 48);
						int preOs = (int)currentElementRecord;
						while (i >= 0)
						{
								int t = nsBuffer3.upper32At(i);
								// with prefix, get full length and prefix length
								if ((t & 0xffff) - (t >> 16) == preLen)
								{
										// doing byte comparison here
										int os = nsBuffer3.lower32At(i) + (t >> 16) + increment;
										int k = 0;
										for (; k < preLen - increment; k++)
										{
												if (XMLDoc[os + k] != XMLDoc[preOs + k])
														break;
										}
										if (k == preLen - increment)
												return; // found the match
								}
								/*if ( (nsBuffer3.upper32At(i) & 0xffff0000) == 0){
										return;
								}*/
								i--;
						}
						// no need to check if xml is the prefix
						if (checkPrefix(preOs, preLen))
								return;



						// print line # column# and full element name
						throw new ParseException("Name space qualification Exception: Element not qualified\n"
										+ formatLineNumber((int)currentElementRecord));
				}

				private void checkQualifiedAttributeUniqueness()
				{
						// TODO Auto-generated method stub
						int preLen1, os1, postLen1, URLLen1, URLOs1,
								 preLen2, os2, postLen2, URLLen2, URLOs2, k;
						for (int i = 0; i < prefixed_attr_count; i++)
						{
								preLen1 = (int)((prefixed_attr_name_array[i] & 0xffff0000L) >> 16);
								postLen1 = (int)((prefixed_attr_name_array[i] & 0xffffL)) - preLen1 - increment;
								os1 = (int)(prefixed_attr_name_array[i] >> 32) + preLen1 + increment;
								URLLen1 = nsBuffer2.lower32At(prefix_URL_array[i]);
								URLOs1 = nsBuffer2.upper32At(prefix_URL_array[i]);
								for (int j = i + 1; j < prefixed_attr_count; j++)
								{
										// prefix of i matches that of j
										preLen2 = (int)((prefixed_attr_name_array[j] & 0xffff0000L) >> 16);
										postLen2 = (int)((prefixed_attr_name_array[j] & 0xffffL)) - preLen2 - increment;
										os2 = (int)(prefixed_attr_name_array[j] >> 32) + preLen2 + increment;
										//System.out.println(new String(XMLDoc,os1, postLen1)
										//	+" "+ new String(XMLDoc, os2, postLen2));
										if (postLen1 == postLen2)
										{
												k = 0;
												for (; k < postLen1; k++)
												{
														//System.out.println(i+" "+(char)(XMLDoc[os+k])+"<===>"+(char)(XMLDoc[preOs+k]));
														if (XMLDoc[os1 + k] != XMLDoc[os2 + k])
																break;
												}
												if (k == postLen1)
												{
														// found the match
														URLLen2 = nsBuffer2.lower32At(prefix_URL_array[j]);
														URLOs2 = nsBuffer2.upper32At(prefix_URL_array[j]);
														//System.out.println(" URLOs1 ===>" + URLOs1);
														//System.out.println("nsBuffer2 ===>"+nsBuffer2.longAt(i)+" i==>"+i);
														//System.out.println("URLLen2 "+ URLLen2+" URLLen1 "+ URLLen1+" ");
														if (matchURL(URLOs1, URLLen1, URLOs2, URLLen2))
																throw new ParseException(" qualified attribute names collide "
																				+ formatLineNumber(os2));
												}
										}
								}
								//System.out.println("======");
						}
				}

				private void handleOtherTextChar(int ch)
				{
						if (ch == '&')
						{
								//has_amp = true;	
								if (!XMLChar.isValidChar(entityIdentifier()))
										throw new ParseException(
												"Error in text content: Invalid char in text content "
												+ formatLineNumber());
								//parser_state = STATE_TEXT;
						}
						else if (ch == ']')
						{
								if (r.skipChar(']'))
								{
										while (r.skipChar(']'))
										{
										}
										if (r.skipChar('>'))
												throw new ParseException(
														"Error in text content: ]]> in text content"
														+ formatLineNumber());
								}
						}
						else
								throw new ParseException(
										"Error in text content: Invalid char in text content "
										+ formatLineNumber());
				}

				private void handleOtherTextChar2(int ch)
				{
						if (ch == '&')
						{
								//has_amp = true;
								//temp_offset = offset;
								entityIdentifier();
								//parser_state = STATE_TEXT;
						}
						else if (ch == ']')
						{
								if (r.skipChar(']'))
								{
										while (r.skipChar(']'))
										{
										}
										if (r.skipChar('>'))
												throw new ParseException(
														"Error in text content: ]]> in text content"
																+ formatLineNumber());
								}
								//parser_state = STATE_TEXT;
						}
						else
								throw new ParseException(
										"Error in text content: Invalid char"
												+ formatLineNumber());
				}
				private void _writeVTD(int offset, int length, int token_type, int depth)
				{
						VTDBuffer.append(((long)((token_type << 28)
										| ((depth & 0xff) << 20) | length) << 32)
										| offset);
				}

				private void writeVTDText(int offset, int length, int token_type, int depth)
				{
						if (length > MAX_TOKEN_LENGTH)
						{
								int k;
								int r_offset = offset;
								for (k = length; k > MAX_TOKEN_LENGTH; k = k - MAX_TOKEN_LENGTH)
								{
										VTDBuffer.append(((long)((token_type << 28)
														| ((depth & 0xff) << 20) | MAX_TOKEN_LENGTH) << 32)
														| r_offset);
										r_offset += MAX_TOKEN_LENGTH;
								}
								VTDBuffer.append(((long)((token_type << 28)
												| ((depth & 0xff) << 20) | k) << 32)
												| r_offset);
						}
						else
						{
								VTDBuffer.append(((long)((token_type << 28)
												| ((depth & 0xff) << 20) | length) << 32)
												| offset);
						}
				}

				private void writeVTD_L5(int offset, int length, int token_type, int depth)
				{


						VTDBuffer.append(((long)((token_type << 28)
										| ((depth & 0xff) << 20) | length) << 32)
										| offset);

						switch (depth)
						{
								case 0:
										rootIndex = VTDBuffer.size_Renamed_Field - 1;
										break;
								case 1:
										if (last_depth == 1)
										{
												l1Buffer.append(((long)last_l1_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 2)
										{
												l2Buffer.append(((long)last_l2_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 3)
										{
												_l3Buffer.append(((long)last_l3_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 4)
										{
												_l4Buffer.append(((long)last_l4_index << 32) | 0xffffffffL);
										}
										last_l1_index = VTDBuffer.size_Renamed_Field - 1;
										last_depth = 1;
										break;
								case 2:
										if (last_depth == 1)
										{
												l1Buffer.append(((long)last_l1_index << 32)
																+ l2Buffer.size_Renamed_Field);
										}
										else if (last_depth == 2)
										{
												l2Buffer.append(((long)last_l2_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 3)
										{
												_l3Buffer.append(((long)last_l3_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 4)
										{
												_l4Buffer.append(((long)last_l4_index << 32) | 0xffffffffL);
										}
										last_l2_index = VTDBuffer.size_Renamed_Field - 1;
										last_depth = 2;
										break;

								case 3:
										/*if (last_depth == 1) {
												l1Buffer.append(((long) last_l1_index << 32)
																+ l2Buffer.size);
										} else*/
										if (last_depth == 2)
										{
												l2Buffer.append(((long)last_l2_index << 32)
																+ _l3Buffer.size_Renamed_Field);
										}
										else if (last_depth == 3)
										{
												_l3Buffer.append(((long)last_l3_index << 32) | 0xffffffffL);
										}
										else if (last_depth == 4)
										{
												_l4Buffer.append(((long)last_l4_index << 32) | 0xffffffffL);
										}
										last_l3_index = VTDBuffer.size_Renamed_Field - 1;
										last_depth = 3;
										break;

								case 4:
										/*if (last_depth == 1) {
												l1Buffer.append(((long) last_l1_index << 32)
																+ l2Buffer.size);
										} else if (last_depth == 2) {
												l2Buffer.append(((long) last_l2_index << 32) | 0xffffffffL);
										} else*/
										if (last_depth == 3)
										{
												_l3Buffer.append(((long)last_l3_index << 32)
																+ _l4Buffer.size_Renamed_Field);
										}
										else if (last_depth == 4)
										{
												_l4Buffer.append(((long)last_l4_index << 32) | 0xffffffffL);
										}
										last_l4_index = VTDBuffer.size_Renamed_Field - 1;
										last_depth = 4;
										break;
								case 5:
										_l5Buffer.append(VTDBuffer.size_Renamed_Field - 1);
										if (last_depth == 4)
										{
												_l4Buffer.append(((long)last_l4_index << 32)
																+ _l5Buffer.size_Renamed_Field - 1);
										}
										last_depth = 5;
										break;

								/*default:
										break;*/
								//rootIndex = VTDBuffer.size() - 1;
						}
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="i"></param>
				public void selectLcDepth(int i)
				{
						if (i != 3 && i != 5)
								throw new System.ArgumentException("LcDepth can only take the value of 3 or 5"); 
						//ParseException("LcDepth can only take the value of 3 or 5");
						if (i == 5)
								shallowDepth = false;
				}
		}
}