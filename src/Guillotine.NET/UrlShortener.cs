// 
// UrlShortener.cs
//  
// Author:
//       M. David Peterson <m.david@3rdandurban.com>
// 
// Copyright (c) 2011 M. David Peterson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Nuxleus.Web.Utility
{
	public static class UrlShortener
	{
		static Encoding mEncoding = new UTF8Encoding ();
		static HashAlgorithm mAlgorithm = MD5.Create ();

		static UrlShortener ()
		{
		}
		
		/// <summary>
		/// Shorten the specified url using the algorithm specified as part of the guillotine project from github.com used as the basis of
		/// their git.io URI shortening service. 
		/// 
		/// The algorithm specified in the <see cref="https://github.com/technoweenie/guillotine/blob/master/lib/guillotine.rb">guillotine.rb</see>
		/// file is as follows:
		/// 
		///# Public: Shortens a given URL to a short code.
		///#
		///# 1) MD5 hash the URL to the hexdigest
		///# 2) Convert it to a Bignum
		///# 3) Pack it into a bitstring as a big-endian int
		///# 4) base64-encode the bitstring, remove the trailing junk
		///#
		///# url - String URL to shorten.
		///#
		///# Returns a unique String code for the URL.
		///
		/// </summary>
		/// <param name='url'>
		/// The URL to shorten
		/// </param>
		public static string Shorten (this string url)
		{
			return url.IsValidUrlString () ? url.CleanUrlString ().GetMD5Hash ().ToBase16Integer ().ToBigEndianByteOrder ().GetBytes ().GetBase64UrlSafeEncoding () : "URL is Invalid.";
					
		}

		public static bool IsValidUrlString (this string url)
		{
			return Regex.IsMatch (url, @"((https?|rtmp|magnet):((//)|(\\\\)|(\?))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");
		}
		
		public static string CleanUrlString (this string url)
		{
			return Regex.Replace (Regex.Replace (url, @"/\s/", String.Empty), @"/(\#|\?).*/", String.Empty);
		}

		public static byte[] GetMD5Hash (this string str)
		{
			return mAlgorithm.ComputeHash (mEncoding.GetBytes (str));
		}
		
		public static int ToBase16Integer (this byte[] byteArray)
		{
			return Convert.ToInt32 (GetStringValue (byteArray), 16);
		}
		
		public static string GetStringValue (this byte[] byteArray)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var element in byteArray) {
				sb.Append (element.ToString ("X2"));
			}
			return sb.ToString ();
		}

		public static string GetBase64UrlSafeEncoding (this byte[] byteArray)
		{
			return GetBase64Encoding (byteArray)
				.Replace ("+", "-")
				.Replace ("=", String.Empty)
				.Replace ("/", "_");
		}

		public static string GetBase64Encoding (this byte[] byteArray, bool insertLineBreaks = false)
		{
			return Convert.ToBase64String (byteArray, insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
		}

		public static byte[] GetBytes (this string intString)
		{
			return mEncoding.GetBytes (intString);
		}

		public static byte[] GetBytes (this int integer)
		{
			return BitConverter.GetBytes (integer);
		}

		public static short ToBigEndianByteOrder (this short v)
		{
			return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
		}

		public static int ToBigEndianByteOrder (this int v)
		{
			return (int)(((ToBigEndianByteOrder ((short)v) & 0xffff) << 0x10) |
				(ToBigEndianByteOrder ((short)(v >> 0x10)) & 0xffff));
		}
	}
}

