
    /* 
     Licensed under the Apache License, Version 2.0
    
     http://www.apache.org/licenses/LICENSE-2.0
     */
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    namespace XTAC
    {
        [XmlRoot(ElementName = "preps")]
        public class Preps
        {
            [XmlElement(ElementName = "prep")]
            public List<string> Prep { get; set; }
        }

        [XmlRoot(ElementName = "builtinverbs")]
        public class Builtinverbs
        {
            [XmlElement(ElementName = "verb")]
            public List<string> Verb { get; set; }
        }

        [XmlRoot(ElementName = "userverbs")]
        public class Userverbs
        {
            [XmlElement(ElementName = "verb")]
            public List<string> Verb { get; set; }
        }

        [XmlRoot(ElementName = "verbs")]
        public class Verbs
        {
            [XmlElement(ElementName = "builtinverbs")]
            public Builtinverbs Builtinverbs { get; set; }
            [XmlElement(ElementName = "userverbs")]
            public Userverbs Userverbs { get; set; }
        }

        [XmlRoot(ElementName = "directions")]
        public class Directions
        {
            [XmlAttribute(AttributeName = "n")]
            public string N { get; set; }
            [XmlAttribute(AttributeName = "s")]
            public string S { get; set; }

            [XmlAttribute(AttributeName = "e")]
            public string E { get; set; }
            [XmlAttribute(AttributeName = "w")]
            public string W { get; set; }
            [XmlAttribute(AttributeName = "ne")]
            public string Ne { get; set; }
            [XmlAttribute(AttributeName = "se")]
            public string Se { get; set; }
            [XmlAttribute(AttributeName = "sw")]
            public string Sw { get; set; }
            [XmlAttribute(AttributeName = "nw")]
            public string Nw { get; set; }
            [XmlAttribute(AttributeName = "up")]
            public string Up { get; set; }
            [XmlAttribute(AttributeName = "down")]
            public string Down { get; set; }
            [XmlAttribute(AttributeName = "in")]
            public string In { get; set; }
            [XmlAttribute(AttributeName = "out")]
            public string Out { get; set; }
            [XmlAttribute(AttributeName = "mass")]
            public string Mass { get; set; }
        }

        [XmlRoot(ElementName = "flags")]
        public class Flags
        {
            [XmlAttribute(AttributeName = "scenery")]
            public string Scenery { get; set; }
            [XmlAttribute(AttributeName = "portable")]
            public string Portable { get; set; }
            [XmlAttribute(AttributeName = "container")]
            public string Container { get; set; }
            [XmlAttribute(AttributeName = "supporter")]
            public string Supporter { get; set; }
            [XmlAttribute(AttributeName = "transparent")]
            public string Transparent { get; set; }
            [XmlAttribute(AttributeName = "openable")]
            public string Openable { get; set; }
            [XmlAttribute(AttributeName = "open")]
            public string Open { get; set; }
            [XmlAttribute(AttributeName = " ")]
            public string Backdrop { get; set; }
            [XmlAttribute(AttributeName = "wearable")]
            public string Wearable { get; set; }
            [XmlAttribute(AttributeName = "emittinglight")]
            public string Emittinglight { get; set; }
            [XmlAttribute(AttributeName = "locked")]
            public string Locked { get; set; }
            [XmlAttribute(AttributeName = "lockable")]
            public string Lockable { get; set; }
            [XmlAttribute(AttributeName = "beingworn")]
            public string BeingWorn { get; set; }
            [XmlAttribute(AttributeName = "user1")]
            public string User1 { get; set; }
            [XmlAttribute(AttributeName = "door")]
            public string Door { get; set; }
            [XmlAttribute(AttributeName = "user2")]
            public string User2 { get; set; }
            [XmlAttribute(AttributeName = "user3")]
            public string User3 { get; set; }
    }

        [XmlRoot(ElementName = "object")]
        public class Object
        {
            [XmlElement(ElementName = "description")]
            public string Description { get ; set; }
            [XmlElement(ElementName = "initialdescription")]
            public string Initialdescription { get; set; }
            [XmlElement(ElementName = "directions")]
            public Directions Directions { get; set; }
            [XmlElement(ElementName = "flags")]
            public Flags Flags { get; set; }
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
            [XmlAttribute(AttributeName = "holder")]
            public string Holder { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "printedname")]
            public string PrintedName { get; set; }
            [XmlElement(ElementName = "synonyms")]
            public Synonyms Synonyms { get; set; }
            [XmlElement(ElementName = "nogo")]
            public Nogo Nogo { get; set; }
            [XmlElement(ElementName = "backdrop")]
            public Backdrop Backdrop { get; set; }
        }

        [XmlRoot(ElementName = "synonyms")]
        public class Synonyms
        {
            [XmlAttribute(AttributeName = "names")]
            public string Names { get; set; }
        }

        [XmlRoot(ElementName = "nogo")]
        public class Nogo
        {
            [XmlElement(ElementName = "s")]
            public string S { get; set; }
            [XmlElement(ElementName = "w")]
            public string W { get; set; }
            [XmlElement(ElementName = "ne")]
            public string Ne { get; set; }
            [XmlElement(ElementName = "nw")]
            public string Nw { get; set; }
            [XmlElement(ElementName = "e")]
            public string E { get; set; }
            [XmlElement(ElementName = "se")]
            public string Se { get; set; }
            [XmlElement(ElementName = "sw")]
            public string Sw { get; set; }
            [XmlElement(ElementName = "n")]
            public string N { get; set; }
            [XmlElement(ElementName = "up")]
            public string Up { get; set; }
            [XmlElement(ElementName = "down")]
            public string Down { get; set; }
        }

        [XmlRoot(ElementName = "backdrop")]
        public class Backdrop
        {
            [XmlAttribute(AttributeName = "rooms")]
            public string Rooms { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "objects")]
        public class Objects
        {
            [XmlElement(ElementName = "object")]
            public List<Object> Object { get; set; }
        }

        [XmlRoot(ElementName = "check")]
        public class Check
        {
            [XmlAttribute(AttributeName = "verb")]
            public string Verb { get; set; }
            [XmlAttribute(AttributeName = "check")]
            public string _check { get; set; }
        }

        [XmlRoot(ElementName = "checks")]
        public class Checks
        {
            [XmlElement(ElementName = "check")]
            public List<Check> Check { get; set; }
        }

        [XmlRoot(ElementName = "sentence")]
        public class Sentence
        {
            [XmlAttribute(AttributeName = "verb")]
            public string Verb { get; set; }
            [XmlAttribute(AttributeName = "do")]
            public string Do { get; set; }
            [XmlAttribute(AttributeName = "prep")]
            public string Prep { get; set; }
            [XmlAttribute(AttributeName = "io")]
            public string Io { get; set; }
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }
            [XmlAttribute(AttributeName = "sub")]
            public string Sub { get; set; }
        }

        [XmlRoot(ElementName = "sentences")]
        public class Sentences
        {
            [XmlElement(ElementName = "sentence")]
            public List<Sentence> Sentence { get; set; }
        }

        [XmlRoot(ElementName = "routine")]
        public class Routine
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "routines")]
        public class Routines
        {
            [XmlElement(ElementName = "routine")]
            public List<Routine> Routine { get; set; }
        }

        [XmlRoot(ElementName = "event")]
        public class Event
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "events")]
        public class Events
        {
            [XmlElement(ElementName = "event")]
            public List<Event> Event { get; set; }
        }

        [XmlRoot(ElementName = "var")]
        public class Var
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "addr")]
            public string Addr { get; set; }
            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "builtin")]
        public class Builtin
        {
            [XmlElement(ElementName = "var")]
            public List<Var> Var { get; set; }
        }

        [XmlRoot(ElementName = "user")]
        public class User
        {
            [XmlElement(ElementName = "var")]
            public List<Var> Var { get; set; }
        }

        [XmlRoot(ElementName = "variables")]
        public class Variables
        {
            [XmlElement(ElementName = "builtin")]
            public Builtin Builtin { get; set; }
            [XmlElement(ElementName = "user")]
            public User User { get; set; }
        }

        [XmlRoot(ElementName = "project")]
        public class Project
        {
            [XmlElement(ElementName = "projname")]
            public string ProjName { get; set; }
            [XmlElement(ElementName = "welcome")]
            public string Welcome { get; set; }
            [XmlElement(ElementName = "author")]
            public string Author { get; set; }
            [XmlElement(ElementName = "version")]
            public string Version { get; set; }
            [XmlElement(ElementName = "output")]
            public string Output { get; set; }
            [XmlElement(ElementName = "preps")]
            public Preps Preps { get; set; }
            [XmlElement(ElementName = "verbs")]
            public Verbs Verbs { get; set; }
            [XmlElement(ElementName = "objects")]
            public Objects Objects { get; set; }
            [XmlElement(ElementName = "checks")]
            public Checks Checks { get; set; }
            [XmlElement(ElementName = "sentences")]
            public Sentences Sentences { get; set; }
            [XmlElement(ElementName = "routines")]
            public Routines Routines { get; set; }
            [XmlElement(ElementName = "events")]
            public Events Events { get; set; }
            [XmlElement(ElementName = "variables")]
            public Variables Variables { get; set; }
            [XmlElement(ElementName = "uservars")]
            public string Uservars { get; set; }
            [XmlElement(ElementName = "walkthrough")]
            public string walkthrough { get; set; }
        [XmlElement(ElementName = "filename")]
        public string filename { get; set; }

    }

    [XmlRoot(ElementName = "xml")]
        public class Xml
        {
            [XmlElement(ElementName = "project")]
            public Project Project { get; set; }
        }

    }
