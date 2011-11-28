// 
// Main.cs
//  
// Author:
//       M. David Peterson <${AuthorEmail}>
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
	class ShortenUrlTest
	{

		public static void Main (string[] args)
		{
			
			string url = args [0];
			string baseDomain = args [1];
			Console.WriteLine ("Computing short URL of {0} using the base domain {1}", url, baseDomain);

			bool validateUrl = url.IsValidUrlString();
			Console.WriteLine ("Valid URL? {0}", validateUrl);
			
			string cleanUrl = url.CleanUrlString();
			Console.WriteLine ("Clean URI: {0}", cleanUrl);
			
			byte[] md5Hash = cleanUrl.GetMD5Hash();
			Console.WriteLine ("MD5 Hex Digest: {0}", md5Hash.GetStringValue());
			
			int base16Int = md5Hash.ToBase16Integer();
			Console.WriteLine ("Base 16 Integer: {0}", base16Int);
			
			int bigEndianInt = base16Int.ToBigEndianByteOrder();
			Console.WriteLine ("BigEndian from Integer: {0}", bigEndianInt);

			string urlSafeStringFromBytes = bigEndianInt.GetBytes().GetBase64UrlSafeEncoding(); 
			Console.WriteLine ("String from Byte Array: {0}", urlSafeStringFromBytes);

			string shortUrl = String.Format ("{0}{1}", baseDomain, urlSafeStringFromBytes);
			Console.WriteLine ("shorturl: {0} from original URL of: {1}", shortUrl, url);
			
			string singleCallToShorten = url.Shorten();
			Console.WriteLine ("As Single Call To Shorten Method: {0}{1}", baseDomain, singleCallToShorten);

		}
	}
}
