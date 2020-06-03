using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using LibraryXsd;
using System.Xml.Schema;

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
        t.Run();
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

    private void Run() 
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        // Validator settings
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

        // Here we add xsd files to namespaces we want to validate
        // (It's like XML -> Schemas setting in Visual Studio)
        settings.Schemas.Add("http://www.example.org/bookstore2", "bookstore.xsd");

        // Processing XSI Schema Location attribute
        // (Disabled by default as it is a security risk). 
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;

        // A function delegate that will be called when 
        // validation error or warning occurs
        settings.ValidationEventHandler += ValidationHandler;

        XmlReader reader = XmlReader.Create("bookstore1.xml", settings);

        // Read method reads next element or attribute from the document
        // It will call ValidationEventHandler if some invalid
        // part occurs
        while (reader.Read())
        {
        }
    }
    private static void ValidationHandler(Object sender, ValidationEventArgs args)
    {
        if (args.Severity == XmlSeverityType.Warning)
            Console.WriteLine("Warning: {0}", args.Message);
        else
            Console.WriteLine("Error: {0}", args.Message);
    }

}
