using System.Collections.Generic;
using System.Xml.Serialization;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of http://www.opengis.net/wms
    // Properties that are not needed are commented out
    [XmlRoot(ElementName = "WMS_Capabilities", Namespace = "http://www.opengis.net/wms")]
    public class WmsCapabilities
    {
        [XmlElement(ElementName = "Service")]
        public Service Service { get; set; }

        [XmlElement(ElementName = "Capability")]
        public Capability Capability { get; set; }
    }

    public class Service
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Abstract")]
        public string Abstract { get; set; }

        // [XmlElement(ElementName = "KeywordList")]
        // public KeywordList KeywordList { get; set; }

        // [XmlElement(ElementName = "OnlineResource")]
        // public OnlineResource OnlineResource { get; set; }
    }

    public class KeywordList
    {
        [XmlElement(ElementName = "Keyword")]
        public List<Keyword> Keywords { get; set; }
    }

    public class Keyword
    {
        [XmlAttribute(AttributeName = "vocabulary")]
        public string Vocabulary { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class OnlineResource
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    public class Capability
    {
        // [XmlElement(ElementName = "Request")]
        // public Request Request { get; set; }

        // [XmlElement(ElementName = "Exception")]
        // public ExceptionDetails Exception { get; set; }

        // [XmlElement(ElementName = "_ExtendedCapabilities")]
        // public List<ExtendedCapability> ExtendedCapabilities { get; set; }

        [XmlElement(ElementName = "Layer")]
        public Layer Layer { get; set; }
    }

    public class Request
    {
        [XmlElement(ElementName = "GetCapabilities")]
        public OperationType GetCapabilities { get; set; }

        [XmlElement(ElementName = "GetMap")]
        public OperationType GetMap { get; set; }

        [XmlElement(ElementName = "GetFeatureInfo")]
        public OperationType GetFeatureInfo { get; set; }

        [XmlElement(ElementName = "_ExtendedOperation")]
        public List<OperationType> ExtendedOperations { get; set; }
    }

    public class OperationType
    {
        [XmlElement(ElementName = "Format")]
        public List<string> Formats { get; set; }

        [XmlElement(ElementName = "DCPType")]
        public List<DCPType> DCPTypes { get; set; }
    }

    public class DCPType
    {
        [XmlElement(ElementName = "HTTP")]
        public HTTP HTTP { get; set; }
    }

    public class HTTP
    {
        [XmlElement(ElementName = "Get")]
        public OnlineResource Get { get; set; }

        [XmlElement(ElementName = "Post")]
        public OnlineResource Post { get; set; }
    }

    public class ExceptionDetails
    {
        [XmlElement(ElementName = "Format")]
        public List<string> Formats { get; set; }
    }

    public class ExtendedCapability
    {
        // Define properties for _ExtendedCapabilities if needed
    }

    public class Layer
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Abstract")]
        public string Abstract { get; set; }

        // [XmlElement(ElementName = "KeywordList")]
        // public KeywordList KeywordList { get; set; }

        [XmlElement(ElementName = "CRS")]
        public List<string> CRS { get; set; }

        // [XmlElement(ElementName = "EX_GeographicBoundingBox")]
        // public EX_GeographicBoundingBox GeographicBoundingBox { get; set; }

        [XmlElement(ElementName = "BoundingBox")]
        public List<BoundingBox> BoundingBoxes { get; set; }

        // [XmlElement(ElementName = "Dimension")]
        // public List<Dimension> Dimensions { get; set; }

        // [XmlElement(ElementName = "Attribution")]
        // public Attribution Attribution { get; set; }

        // [XmlElement(ElementName = "AuthorityURL")]
        // public List<AuthorityURL> AuthorityURLs { get; set; }

        // [XmlElement(ElementName = "Identifier")]
        // public List<Identifier> Identifiers { get; set; }

        // [XmlElement(ElementName = "MetadataURL")]
        // public List<MetadataURL> MetadataURLs { get; set; }

        // [XmlElement(ElementName = "DataURL")]
        // public List<DataURL> DataURLs { get; set; }

        // [XmlElement(ElementName = "FeatureListURL")]
        // public List<FeatureListURL> FeatureListURLs { get; set; }

        [XmlElement(ElementName = "Style")]
        public List<Style> Styles { get; set; }

        // [XmlElement(ElementName = "MinScaleDenominator")]
        // public double? MinScaleDenominator { get; set; }

        // [XmlElement(ElementName = "MaxScaleDenominator")]
        // public double? MaxScaleDenominator { get; set; }

        [XmlElement(ElementName = "Layer")]
        public List<Layer> Layers { get; set; }

        // [XmlAttribute(AttributeName = "queryable")]
        // public bool Queryable { get; set; }

        // [XmlAttribute(AttributeName = "cascaded")]
        // public int Cascaded { get; set; }

        // [XmlAttribute(AttributeName = "opaque")]
        // public bool Opaque { get; set; }

        // [XmlAttribute(AttributeName = "noSubsets")]
        // public bool NoSubsets { get; set; }

        // [XmlAttribute(AttributeName = "fixedWidth")]
        // public int FixedWidth { get; set; }

        // [XmlAttribute(AttributeName = "fixedHeight")]
        // public int FixedHeight { get; set; }
    }

    public class EX_GeographicBoundingBox
    {
        [XmlElement(ElementName = "westBoundLongitude")]
        public double WestBoundLongitude { get; set; }
        [XmlElement(ElementName = "eastBoundLongitude")]
        public double EastBoundLongitude { get; set; }
        [XmlElement(ElementName = "southBoundLatitude")]
        public double SouthBoundLatitude { get; set; }
        [XmlElement(ElementName = "northBoundLatitude")]
        public double NorthBoundLatitude { get; set; }
    }

    public class BoundingBox
    {
        [XmlAttribute(AttributeName = "CRS")]
        public string CRS { get; set; }

        [XmlAttribute(AttributeName = "minx")]
        public double MinX { get; set; }

        [XmlAttribute(AttributeName = "miny")]
        public double MinY { get; set; }

        [XmlAttribute(AttributeName = "maxx")]
        public double MaxX { get; set; }

        [XmlAttribute(AttributeName = "maxy")]
        public double MaxY { get; set; }
    }

    public class Dimension
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "units")]
        public string Units { get; set; }

        [XmlAttribute(AttributeName = "unitSymbol")]
        public string UnitSymbol { get; set; }

        [XmlAttribute(AttributeName = "default")]
        public string Default { get; set; }

        [XmlAttribute(AttributeName = "multipleValues")]
        public bool MultipleValues { get; set; }

        [XmlAttribute(AttributeName = "nearestValue")]
        public bool NearestValue { get; set; }

        [XmlAttribute(AttributeName = "current")]
        public bool Current { get; set; }
    }

    public class Attribution
    {
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }
    }

    public class AuthorityURL
    {
        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    public class Identifier
    {
        [XmlAttribute(AttributeName = "authority")]
        public string Authority { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MetadataURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    public class DataURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }
    }

    public class FeatureListURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }
    }

    public class Style
    {
        // [XmlElement(ElementName = "Name")]
        // public string Name { get; set; }

        // [XmlElement(ElementName = "Title")]
        // public string Title { get; set; }

        // [XmlElement(ElementName = "Abstract")]
        // public string Abstract { get; set; }
        [XmlElement(ElementName = "LegendURL")]
        public List<LegendURL> LegendURLs { get; set; }

        // [XmlElement(ElementName = "StyleSheetURL")]
        // public StyleSheetURL StyleSheetURL { get; set; }

        // [XmlElement(ElementName = "StyleURL")]
        // public StyleURL StyleURL { get; set; }
    }

    public class LegendURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }

        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }

        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }

    public class StyleSheetURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }
    }

    public class StyleURL
    {
        [XmlElement(ElementName = "Format")]
        public string Format { get; set; }

        [XmlElement(ElementName = "OnlineResource")]
        public OnlineResource OnlineResource { get; set; }
    }
}