using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using LibraryXsd;

/* The XmlRootAttribute allows you to set an alternate name 
   (PurchaseOrder) of the XML element, the element namespace; by 
   default, the XmlSerializer uses the class name. The attribute 
   also allows you to set the XML namespace for the element.  Lastly,
   the attribute sets the IsNullable property, which specifies whether 
   the xsi:null attribute appears if the class instance is set to 
   a null reference. */
[XmlRootAttribute("PurchaseOrder", Namespace = "http://example.org/jk/library",
IsNullable = false)]

public class Test
{
    public static void Main() {
        Test t = new Test();
        t.ReadPo("C:/Users/VickersZhu/Documents/GitHub/XML_Lab/ClassLibrary_1/Library.xml");
        //t.VerifyXml("C:/Users/VickersZhu/Documents/GitHub/XML_Lab/ClassLibrary_1/Library.xml");
        Console.ReadKey();
    }

    private void ReadPo(string filename) {
        XmlSerializer serializer = new XmlSerializer(typeof(library));

        serializer.UnknownNode += new
        XmlNodeEventHandler(serializer_UnknownNode);
        serializer.UnknownAttribute += new
        XmlAttributeEventHandler(serializer_UnknownAttribute);

        FileStream fs = new FileStream(filename, FileMode.Open);
        library lb;

        lb = (LibraryXsd.library)serializer.Deserialize(fs);

        BookType[] books = lb.book;
        foreach (BookType book in books)
        {
            Console.WriteLine(book.language);
        }


    }

    private void serializer_UnknownNode
    (object sender, XmlNodeEventArgs e)
    {
        Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
    }
    private void serializer_UnknownAttribute
   (object sender, XmlAttributeEventArgs e)
    {
        System.Xml.XmlAttribute attr = e.Attr;
        Console.WriteLine("Unknown attribute " +
        attr.Name + "='" + attr.Value + "'");
    }

    private void VerifyXml(string filename)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Parse;
        XmlReader reader = XmlReader.Create(filename, settings);

        reader.MoveToContent();
        // Parse the file and display each of the nodes.
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    Console.Write("<{0}>", reader.Name);
                    break;
                case XmlNodeType.Text:
                    Console.Write(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    Console.Write("<![CDATA[{0}]]>", reader.Value);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                    break;
                case XmlNodeType.Comment:
                    Console.Write("<!--{0}-->", reader.Value);
                    break;
                case XmlNodeType.XmlDeclaration:
                    Console.Write("<?xml version='1.0'?>");
                    break;
                case XmlNodeType.Document:
                    break;
                case XmlNodeType.DocumentType:
                    Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    Console.Write(reader.Name);
                    break;
                case XmlNodeType.EndElement:
                    Console.Write("</{0}>", reader.Name);
                    break;
            }
        }
    }
}
