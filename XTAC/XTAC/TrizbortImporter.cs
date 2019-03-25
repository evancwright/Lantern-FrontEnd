/*This class reads a Trizbort XML file and loads it into a Game object
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using TrizbortXml;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace XTAC
{
    class TrizbortImporter
    {

        public void Import(Xml project, string fileName)
        {
            Trizbort triz;
            Dictionary<int, int> idToIndex = new Dictionary<int,int>();
            Dictionary<string,int> nameCounts = new Dictionary<string, int>();

            System.Xml.Serialization.XmlSerializer reader = new XmlSerializer(typeof(Trizbort));

            // Read the XML file.
            StreamReader file = new StreamReader(fileName);
             
            // Deserialize the content of the file into an object.
            triz = (Trizbort)reader.Deserialize(file);
            
            file.Close();

            try
            {
                project.Project.Author = triz.Info.Author;
            } catch  { }

            try
            {
                string title = triz.Info.Title;
                title = title.Replace(' ', '_');
                project.Project.ProjName = title;
            } catch { }

            try
            { 
                string temp = triz.Info.Description;
                temp = temp.Substring(0, Math.Min(255,temp.Length));
                temp = temp.Replace('\n', ' ');
                temp = temp.Replace('\r', ' ');
                project.Project.Welcome = temp;
            }
            catch { }

            project.Project.Objects.Object.RemoveAt(2);  // remove default room
            int index = 2; //start index
            foreach (Room r in triz.Map.Room)
            {
                int _id = Convert.ToInt32(r.Id);
                idToIndex[_id] =  index;
                index++;
            }

            //count the names to see which ones are unique
            CountNames(triz.Map.Room, nameCounts);
            
            int id=2;
            //convert the file into a Xml project
            foreach (Room r in triz.Map.Room)
            {
                Object obj = CreateObject();

                obj.Name = r.Name.Replace(' ','_').Trim();

                if (nameCounts[obj.Name] > 1)
                {//append the index
                    obj.Name += id;
                }

                obj.PrintedName = r.Name;

                if (r.Name.Equals(""))
                {
                    obj.Name = "Unamed Room";
                }

                    obj.Id = "" + id;
                    obj.Description = r.Description.Trim();

                    if (obj.Description.Length == 0)
                    {
                        obj.Description = "Description not set.";
                    }

                    obj.Holder = "0";
              
                    if (r.isDark == "yes")
                        obj.Flags.Emittinglight = "0";
                    else
                        obj.Flags.Emittinglight = "1";

                    obj.Initialdescription = "";
                    obj.Synonyms = new Synonyms();
                    obj.Synonyms.Names = "";

                    project.Project.Objects.Object.Add(obj);
                    ++id;
            }

            //create the connections between rooms
            int i = 3;
            foreach (Line l in triz.Map.Line)
            {
                try
                {
                    int id1;
                    int id2;

                    id1 = Convert.ToInt32(l.Dock[0].Id);
                    id2 = Convert.ToInt32(l.Dock[1].Id);
                    
                    int index1 = idToIndex[id1];
                    int index2 = idToIndex[id2];

                    Object r1 = project.Project.Objects.Object.ElementAt(index1); //convert triz ids to internal ids
                    Object r2 = project.Project.Objects.Object.ElementAt(index2);


                    if (l.StartText == "up")
                    {//up / down
                        SetRoomConnection(r1, "up", index2);

                        if (l.flow == null || !l.flow.Equals("oneWay"))
                        {
                            SetRoomConnection(r2, "down", index1);
                        }
                        continue;
                    }
                    else if (l.StartText == "down")
                    {
                        SetRoomConnection(r1, "down", index2);

                        if (l.flow != null &&   !l.flow.Equals("oneWay"))
                        {
                            SetRoomConnection(r2, "up", index1);
                        }
                        continue;
                    }
                    else
                    {//normal connnection
                        SetRoomConnection(r1, l.Dock[0].Port, index2);
                    }        

                    //make reverse connection
                    if (l.flow != null)
                    {
                        if (!l.flow.Equals("oneWay"))
                        {
                            SetRoomConnection(r2, l.Dock[1].Port, index1);
                        }
                    }
                    else
                    {
                        SetRoomConnection(r2, l.Dock[1].Port, index1 );
                    }
                }
                catch (Exception ex)
                {
                //    throw new Exception("Import failed", ex);
                }
            }
           

            //create object entries for the objects in each room
            //create the connections between rooms
            i = 2;

            foreach (Room r in triz.Map.Room)
            {
                string objects = r.Objects;
                if (objects != null)
                {
                    string[] objs = objects.Split('|');

                    foreach (string s in objs)
                    {
                        if (!s.Equals(""))
                        {
                            Object thing = CreateObject();

                            int idNum = project.Project.Objects.Object.Count;

                            string name = s.Replace(' ', '_').Trim();
                            if (nameCounts[name] == 1)
                                thing.Name = s.Replace(' ', '_').Trim();
                            else
                                thing.Name = s.Replace(' ', '_').Trim() + idNum;
                            thing.PrintedName = s.Trim();
                            
                            thing.Id = "" + id;
                            thing.Description = "You notice nothing unexpected.";
                            thing.Initialdescription = "";
                            thing.Synonyms = new Synonyms();
                            thing.Synonyms.Names = "";
                            thing.Holder = "" + i;
                            project.Project.Objects.Object.Add(thing);
                            id++;
                        }
                    }
                }
                i++;
            }

            
        }

        Object CreateObject()
        {
                Object obj = new Object();
                obj.Directions = new Directions();
                obj.Flags = new Flags();
                obj.Nogo = new Nogo();
                obj.Holder = "offscreen";
                
                obj.Directions.N = "255";
                obj.Directions.S = "255";
                obj.Directions.E = "255";
                obj.Directions.W = "255";
                obj.Directions.Ne = "255";
                obj.Directions.Nw = "255";
                obj.Directions.Se = "255";
                obj.Directions.Sw = "255";
                obj.Directions.Up = "255";
                obj.Directions.Down = "255"; 
                obj.Directions.In = "255";
                obj.Directions.Out = "255";
                obj.Directions.Mass = "0";
                return obj;
        }

        void SetRoomConnection(Object room1, string direction, int rid)
        {
            
            if (direction == "n")
                room1.Directions.N = ""+rid;
            else if (direction == "s")
                room1.Directions.S = "" + rid;
            else if (direction == "e")
                room1.Directions.E = "" + rid;
            else if (direction == "w")
                room1.Directions.W = "" + rid;
            else if (direction == "ne")
                room1.Directions.Ne = "" + rid;
            else if (direction == "se")
                room1.Directions.Se = "" +rid;
            else if (direction == "sw")
                room1.Directions.Sw = "" + rid;
            else if (direction == "Ne")
                room1.Directions.Ne = "" + rid;
            else if (direction == "nw")
                room1.Directions.Nw = "" + rid;
            else if (direction == "in")
                room1.Directions.In = "" + rid;
            else if (direction == "out")
                room1.Directions.Out = ""+rid;
            else if (direction == "up")
                room1.Directions.Up = "" + rid;
            else if (direction == "down")
                room1.Directions.Down = "" + rid;

        }

        void CountNames(List<Room> rooms , Dictionary<string, int> dict)
        {
            foreach (Room r in rooms)
            {
                string n = r.Name.Replace(' ', '_').Trim();
                int c = 0;
                
                if (dict.TryGetValue(n, out c))
                {
                    dict[n]++;
                }
                else
                {
                    dict[n] = 1;
                }

                string objects = r.Objects;
                if (objects != null)
                {
                    string[] objs = objects.Split('|');

                    foreach (string s in objs)
                    {
                        if (!s.Equals(""))
                        {   
                            string name = s.Replace(' ', '_').Trim(); 
                            if (dict.TryGetValue(name, out c))
                            {
                                dict[name]++;
                            }
                            else
                            {
                                dict[name] = 1;
                            }
                        }
                    }
                }
            }
        }
    }
}
