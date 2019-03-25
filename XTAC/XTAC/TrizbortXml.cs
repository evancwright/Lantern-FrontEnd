/* 
 Licensed under the Apache License, Version 2.0
    
 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TrizbortXml
{
    [XmlRoot(ElementName = "info")]
    public class Info
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "author")]
        public string Author { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "history")]
        public string History { get; set; }
    }

    [XmlRoot(ElementName = "room")]
    public class Room
    {
        [XmlElement(ElementName = "objects")]
        public string Objects { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "x")]
        public string X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public string Y { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "h")]
        public string H { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "isDark")]
        public string isDark { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "dock")]
    public class Dock
    {
        [XmlAttribute(AttributeName = "index")]
        public string Index { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "port")]
        public string Port { get; set; }
    }

    [XmlRoot(ElementName = "line")]
    public class Line
    {
        [XmlElement(ElementName = "dock")]
        public List<Dock> Dock { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "startText")]
        public string StartText { get; set; }
        [XmlAttribute(AttributeName = "endText")]
        public string EndText { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "flow")]
        public string flow { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "map")]
    public class Map
    {
        [XmlElement(ElementName = "room")]
        public List<Room> Room { get; set; }
        [XmlElement(ElementName = "line")]
        public List<Line> Line { get; set; }
    }

    [XmlRoot(ElementName = "colors")]
    public class Colors
    {
        [XmlElement(ElementName = "canvas")]
        public string Canvas { get; set; }
        [XmlElement(ElementName = "fill")]
        public string Fill { get; set; }
        [XmlElement(ElementName = "border")]
        public string Border { get; set; }
        [XmlElement(ElementName = "line")]
        public string Line { get; set; }
        [XmlElement(ElementName = "selectedLine")]
        public string SelectedLine { get; set; }
        [XmlElement(ElementName = "hoverLine")]
        public string HoverLine { get; set; }
        [XmlElement(ElementName = "largeText")]
        public string LargeText { get; set; }
        [XmlElement(ElementName = "smallText")]
        public string SmallText { get; set; }
        [XmlElement(ElementName = "lineText")]
        public string LineText { get; set; }
        [XmlElement(ElementName = "grid")]
        public string Grid { get; set; }
    }

    [XmlRoot(ElementName = "object")]
    public class Object
    {
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "fonts")]
    public class Fonts
    {
        [XmlElement(ElementName = "room")]
        public Room Room { get; set; }
        [XmlElement(ElementName = "object")]
        public Object Object { get; set; }
        [XmlElement(ElementName = "line")]
        public Line Line { get; set; }
    }

    [XmlRoot(ElementName = "grid")]
    public class Grid
    {
        [XmlElement(ElementName = "snapTo")]
        public string SnapTo { get; set; }
        [XmlElement(ElementName = "visible")]
        public string Visible { get; set; }
        [XmlElement(ElementName = "showOrigin")]
        public string ShowOrigin { get; set; }
        [XmlElement(ElementName = "size")]
        public string Size { get; set; }
    }

    [XmlRoot(ElementName = "lines")]
    public class Lines
    {
        [XmlElement(ElementName = "width")]
        public string Width { get; set; }
        [XmlElement(ElementName = "handDrawn")]
        public string HandDrawn { get; set; }
        [XmlElement(ElementName = "arrowSize")]
        public string ArrowSize { get; set; }
        [XmlElement(ElementName = "textOffset")]
        public string TextOffset { get; set; }
    }

    [XmlRoot(ElementName = "rooms")]
    public class Rooms
    {
        [XmlElement(ElementName = "darknessStripeSize")]
        public string DarknessStripeSize { get; set; }
        [XmlElement(ElementName = "objectListOffset")]
        public string ObjectListOffset { get; set; }
        [XmlElement(ElementName = "connectionStalkLength")]
        public string ConnectionStalkLength { get; set; }
        [XmlElement(ElementName = "preferredDistanceBetweenRooms")]
        public string PreferredDistanceBetweenRooms { get; set; }
    }

    [XmlRoot(ElementName = "ui")]
    public class Ui
    {
        [XmlElement(ElementName = "handleSize")]
        public string HandleSize { get; set; }
        [XmlElement(ElementName = "snapToElementSize")]
        public string SnapToElementSize { get; set; }
    }

    [XmlRoot(ElementName = "keypadNavigation")]
    public class KeypadNavigation
    {
        [XmlElement(ElementName = "creationModifier")]
        public string CreationModifier { get; set; }
        [XmlElement(ElementName = "unexploredModifier")]
        public string UnexploredModifier { get; set; }
    }

    [XmlRoot(ElementName = "settings")]
    public class Settings
    {
        [XmlElement(ElementName = "colors")]
        public Colors Colors { get; set; }
        [XmlElement(ElementName = "fonts")]
        public Fonts Fonts { get; set; }
        [XmlElement(ElementName = "grid")]
        public Grid Grid { get; set; }
        [XmlElement(ElementName = "lines")]
        public Lines Lines { get; set; }
        [XmlElement(ElementName = "rooms")]
        public Rooms Rooms { get; set; }
        [XmlElement(ElementName = "ui")]
        public Ui Ui { get; set; }
        [XmlElement(ElementName = "keypadNavigation")]
        public KeypadNavigation KeypadNavigation { get; set; }
    }

    [XmlRoot(ElementName = "trizbort")]
    public class Trizbort
    {
        [XmlElement(ElementName = "info")]    
        public Info Info { get; set; }
        [XmlElement(ElementName = "map")]
        public Map Map { get; set; }
        [XmlElement(ElementName = "settings")]
        public Settings Settings { get; set; }
    }

}
