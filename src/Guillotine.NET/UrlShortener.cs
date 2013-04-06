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

		/// <summary>
		/// Determines if string is a valid URL string the specified url.
		/// </summary>
		/// <returns><c>true</c> if string is a valid URL; otherwise, <c>false</c>.</returns>
		/// <param name="url">URL string.</param>
		public static bool IsValidUrlString (this string url)
		{
			return Regex.IsMatch (url, @"((https?|rtmp|magnet):((//)|(\\\\)|(\?))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");
		}

		/// <summary>
		/// Cleans the URL string.
		/// </summary>
		/// <returns>The URL string.</returns>
		/// <param name="url">URL.</param>
		public static string CleanUrlString (this string url)
		{
			return Regex.Replace (Regex.Replace (url, @"/\s/", String.Empty), @"/(\#|\?).*/", String.Empty);
		}

		/// <summary>
		/// Gets the MD5 hash of the given string.
		/// </summary>
		/// <returns>The MD5 hash in the form of a byte array.</returns>
		/// <param name="str">The string to calculate the MD5 Hash.</param>
		public static byte[] GetMD5Hash (this string str)
		{
			return mAlgorithm.ComputeHash (mEncoding.GetBytes (str));
		}

		/// <summary>
		/// Converts the given byte array to a base 16 integer
		/// </summary>
		/// <returns>The base16 integer for the given byte array.</returns>
		/// <param name="byteArray">The byte array in which to perform the conversion.</param>
		public static int ToBase16Integer (this byte[] byteArray)
		{
			return Convert.ToInt32 (GetStringValue (byteArray), 16);
		}

		/// <summary>
		/// Gets the string value of the specified byte array.
		/// </summary>
		/// <returns>The string value of the specified byte array.</returns>
		/// <param name="byteArray">The byte array in which to perform the conversion.</param>
		public static string GetStringValue (this byte[] byteArray)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var element in byteArray) {
				sb.Append (element.ToString ("X2"));
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Gets the URL safe base64 encoding of the specified byte array.
		/// </summary>
		/// <returns>The base64 URL safe encoding.</returns>
		/// <param name="byteArray">The byte array in which to get the URL safe Base64 encoding for.</param>
		public static string GetBase64UrlSafeEncoding (this byte[] byteArray)
		{
			return GetBase64Encoding (byteArray)
				.Replace ("+", "-")
				.Replace ("=", String.Empty)
				.Replace ("/", "_");
		}

		/// <summary>
		/// Gets the base64 encoding for the specified byte array
		/// </summary>
		/// <returns>The base64 encoding of the given byte array.</returns>
		/// <param name="byteArray">Byte array.</param>
		/// <param name="insertLineBreaks">If set to <c>true</c> insert line breaks.</param>
		public static string GetBase64Encoding (this byte[] byteArray, bool insertLineBreaks = false)
		{
			return Convert.ToBase64String (byteArray, insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
		}

		/// <summary>
		/// Gets the bytes for the given string.
		/// </summary>
		/// <returns>The bytes for the given string.</returns>
		/// <param name="intString">Int in the form of a string.</param>
		public static byte[] GetBytes (this string intString)
		{
			return mEncoding.GetBytes (intString);
		}

		/// <summary>
		/// Gets the bytes for the given int.
		/// </summary>
		/// <returns>The bytes for the given int.</returns>
		/// <param name="integer">Int to get the bytes for.</param>
		public static byte[] GetBytes (this int integer)
		{
			return BitConverter.GetBytes (integer);
		}

		/// <summary>
		/// Convert the specified short to Big Endian byte order
		/// </summary>
		/// <returns>The big endian byte order of the specified short.</returns>
		/// <param name="v">The short value to convert to Big Endian byte order.</param>
		public static short ToBigEndianByteOrder (this short shrt)
		{
			return (short)(((shrt & 0xff) << 8) | ((shrt >> 8) & 0xff));
		}

		/// <summary>
		/// Convert the specified int to Big Endian byte order.
		/// </summary>
		/// <returns>The int converted to big endian byte order.</returns>
		/// <param name="integer">The int in which to convert to Big Endian byte order</param>
		public static int ToBigEndianByteOrder (this int integer)
		{
			return (((ToBigEndianByteOrder ((short)integer) & 0xffff) << 0x10) | (ToBigEndianByteOrder ((short)(integer >> 0x10)) & 0xffff));
		}
	}
}

