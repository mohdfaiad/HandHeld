using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace PI_Models
{

    [XmlRoot(ElementName = "ASN")]
    public class ASNxml
    {
        public string Number { get; set; }
        public string CreationDate { get; set; }
        public string ShipmentDate { get; set; }
        public string SupplierID { get; set; }
        public string ShipToCustomerID { get; set; }

        [XmlArray("Pallets")]
        public List<PalletXml> pallets { get; set; }

    }

    [XmlType("Pallet")]
    public class PalletXml
    {
        public string epc { get; set; }
        [XmlArray("Cartons")]
        public List<CartonXml> cartons { get; set; }       
    }

    [XmlType("Carton")]
    public class CartonXml
    {
        public string epc { get; set; }
        [XmlArray("LineItems")]
        public List<LineItemXml> lineitems { get; set; }     
    }


    [XmlType("LineItem")]
    public class LineItemXml
    {
        public string Component { get; set; }
        public string PONumber { get; set; }
        public string POLineItem { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public string ProductID { get; set; }
        public string GTIN { get; set; }
        public string QuantityShipped { get; set; }
        public string QuantityOrdered { get; set; }
        public string LotNumber { get; set; }
        public string ExpirationDate { get; set; }
        [XmlArray("IndividualItems")]
        public List<IndividualItemXml> individualitems { get; set; }
    }


    [XmlType("IndividualItem")]
    public class IndividualItemXml
    {
        public string epc { get; set; }
    }


    //XmlFormatter<ASNxml>(asn);

    public class XmlFormatter
    {
        public string Serialize<T>(T dataToSerialize)
        {
            var stringwriter = new System.IO.StringWriter();
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            var serializer = new XmlSerializer(typeof(T));
            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            serializer.Serialize(stringwriter, dataToSerialize, emptyNs);
            return stringwriter.ToString();

        }

    }


    public class XmlWriter
    {


        public void WriteToFile(ASNxml asn)
        {
            string path = string.Format(@"C:\ASN\Asn{0}.xml", asn.Number);
            XmlSerializer x = new XmlSerializer(asn.GetType());
            StreamWriter writer = new StreamWriter(path);
            x.Serialize(writer, asn);

        }



    }



}
