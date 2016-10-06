using System;
using System.IO;
using System.Xml;

namespace com.lothlorien.ebaysdkbook
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class XMLExample
	{
		[STAThread]
		static void Main(string[] args)
		{
			string xml = "<?xml version=\"1.0\"?><request><RequestUserId>^userid^</RequestUserId><RequestPassword>^password^</RequestPassword><DetailLevel>1</DetailLevel><ErrorLevel>1</ErrorLevel><SiteId>0</SiteId><Verb>GetCategories</Verb><CategoryParent>^parent^</CategoryParent><LevelLimit>^limit^</LevelLimit></request>";
			StringReader reader;
			XmlTextReader parser;
			string result;

			xml = xml.Replace( "^userid^", "sandy2718" );
			xml = xml.Replace( "^password^", "magic" );
			xml = xml.Replace( "^parent^", "2" );
			xml = xml.Replace( "^limit^", "1" );

			System.Console.WriteLine( xml );

			reader = new StringReader( xml );
			parser = new XmlTextReader( reader );

			while( parser.Read() )
			{
				if ( parser.NodeType == XmlNodeType.Element ) 
				{
					result = parser.Name + ":";
					parser.Read();
					result += parser.Value;
					System.Console.WriteLine( result );
				}
			}
			System.Console.ReadLine();
			parser.Close();
		}
	}
}
