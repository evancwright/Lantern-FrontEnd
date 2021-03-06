﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;
using XMLtoAdv;

namespace XTAC
{
    public partial class Lantern : Form
    {
        public static Xml xproject;
        string fileName = "";
        List<Keys> lastFunctionKeys = new List<Keys>();
        List<Keys> lastEventKeys = new List<Keys>();
        string homeDir;

        List<Tuple<string, string>> recentFiles = new List<Tuple<string, string>>();
        Dictionary<string, string> prefs = new Dictionary<string, string>();

        //the list of verbs checks
        string[] allChecks = new string[]
        {
            "check_see_dobj", "check_have_dobj", "check_dobj_supplied", "check_iobj_supplied", "check_see_iobj", "check_dont_have_dobj","check_not_self_or_child",
            "check_dobj_open", "check_dobj_closed", "check_dobj_opnable", "check_dobj_portable", "check_dobj_locked","check_dobj_unlocked",
            "check_iobj_container", "check_light","check_put"
        };

        static Lantern()
        {
            xproject = new Xml();

           

        }

        public Lantern()
        {
            InitializeComponent();

            LoadRecents();

            homeDir = Directory.GetCurrentDirectory();

            //add fixed menu items
            sentenceTypeComboBox.Items.Add("before");
            sentenceTypeComboBox.Items.Add("instead");
            sentenceTypeComboBox.Items.Add("after");
            sentenceTypeComboBox.SelectedIndex = 1;

            foreach (String s in allChecks)
            {
                allChecksListBox.Items.Add(s);
            }

            //load prefs
            LoadPrefs();

            NewProject();
            ShowProject();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select an XML File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadProject(openFileDialog1.FileName);

                var results = recentFiles.Find((t) =>  t.Item1 == openFileDialog1.SafeFileName );
                if (results == null)
                {
                    recentFiles.Insert(0,new Tuple<string, string>(openFileDialog1.SafeFileName,openFileDialog1.FileName));
                }

                if (recentFiles.Count() > 5)
                {
                    recentFiles.RemoveAt(5);
                }
            }
        }

        void LoadProject(string fileToOpen)
        {
            System.Xml.Serialization.XmlSerializer reader = new XmlSerializer(typeof(Xml));
            userVerbListView.Clear();
            builtinVarsListBox.Items.Clear();
            verbChecksListBox.Items.Clear();

            // Read the XML file.
            StreamReader file = new StreamReader(fileToOpen);
            fileName = fileToOpen;

            try
            {
                // Deserialize the content of the file into a project object.
                xproject = (Xml)reader.Deserialize(file);
                
                FixVariableNames();
                FixPrintedNames();
                FixFunctions();
                FixDescriptions();
                FixOutputName();
                FixVerbs();
                FixEmptyObjects();
                FixChecks();
                FixDoors();              
                FixEnter();
                FixBuildSettings();
                ShowProject();
                Text = "Lantern (" + fileName + ")";
            }
            catch (Exception ex)
            {
                string msg = UnrollMessage(ex);
                ExceptionForm ef = new ExceptionForm();
                ef.ErrText = "Error reading XML file.  Make sure you are not trying to open a Trizbort file.\r\n" + msg;
                ef.Show();
            }
            finally
            {
                file.Close();
            }
        }

        void ShowProject()
        {
            textBox4.Text = xproject.Project.ProjName;
            authorTextBox.Text = xproject.Project.Author;
            welcomeTextBox.Text = xproject.Project.Welcome;
            versionTextBox.Text = xproject.Project.Version;
            outputTextBox.Text = xproject.Project.Output;
            walkThroughTextBox.Text = xproject.Project.walkthrough;

            PopulateObjectDropDowns();

            if (objectsComboBox.Items.Count > 0)
            {
                objectsComboBox.SelectedIndex = 0;
            }

            LoadFunctions();
            PopulateVerbs();
            PopulateSentenceVerbs();
            PopulateEventRules();
            PopulateSentences();
            PopulatePrepositions();
            PopulateVars();
            PopulateBuildSettings();
            sentenceIndirectObjComboBox.SelectedIndex = 0;
        }

        void PopulateObjectDropDowns()
        {
            PopulateListBox(objectsComboBox, false, false);
            PopulateListBox(nComboBox);
            PopulateListBox(sComboBox);
            PopulateListBox(eComboBox);
            PopulateListBox(wComboBox);
            PopulateListBox(neComboBox);
            PopulateListBox(nwComboBox);
            PopulateListBox(seComboBox);
            PopulateListBox(swComboBox);
            PopulateListBox(upComboBox);
            PopulateListBox(downComboBox);
            PopulateListBox(enterComboBox);
            PopulateListBox(outComboBox);
            PopulateListBox(parentComboBox, false);
            PopulateListBox(sentenceObjectComboBox, addEmpty: true, anyObject: true);
            PopulateListBox(sentenceIndirectObjComboBox, addEmpty: true, anyObject: true);

        }

        void PopulateSentenceVerbs()
        {
            verbComboBox.Items.Clear();
            foreach (String v in xproject.Project.Verbs.Builtinverbs.Verb)
            {
                verbComboBox.Items.Add(v);
            }
            foreach (String v in xproject.Project.Verbs.Userverbs.Verb)
            {
                verbComboBox.Items.Add(v);
            }

            verbComboBox.SelectedIndex = 0;
        }

        void LoadFunctions()
        {
            functionsListBox.Items.Clear();
            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                functionsListBox.Items.Add(r.Name);
            }

            if (functionsListBox.Items.Count > 0)
            {
                functionsListBox.SelectedIndices.Add(0);
                codeTextBox.Text = xproject.Project.Routines.Routine[0].Text;
            }

            //load available functions into sentence drop down
            sentenceFuncComboBox.Items.Clear();
            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                sentenceFuncComboBox.Items.Add(r.Name);
            }

            if (sentenceFuncComboBox.Items.Count > 0)
                sentenceFuncComboBox.SelectedIndex = 0;

        }

        void PopulatePrepositions()
        {
            prepositionsComboBox.Items.Clear();
            prepositionsComboBox.Items.Add(""); //add blank for "no prep"
            foreach (String s in xproject.Project.Preps.Prep)
            {
                prepositionsComboBox.Items.Add(s);
            }
            prepositionsComboBox.SelectedIndex = 0;
        }

        void PopulateListBox(ComboBox cb, bool youCantGo = true, bool addEmpty = false, bool anyObject = false)
        {
            cb.Items.Clear();
            int i = 0;

            if (addEmpty) cb.Items.Add(""); //object lists can contain a blank

            foreach (var o in xproject.Project.Objects.Object)
            {
                cb.Items.Add(o.Name + "(" + i++ + ")");
            }

            if (youCantGo)
                cb.Items.Add("You can't go that way.");

            if (anyObject)
                cb.Items.Add("*(ANY OBJECT)");

            if (cb.Items.Count > 0 && anyObject)
                cb.SelectedIndex = 0;
        }

        int GetIndex(string direction)
        {
            int index = Convert.ToInt32(direction);
            if (index == 255)
                return xproject.Project.Objects.Object.Count;
            else
                return index;
        }



        void PopulateSentences()
        {
            sentencesListBox.Items.Clear();

            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                sentencesListBox.Items.Add(BuildSentenceName(s));
            }
        }

        string BuildSentenceName(Sentence s)
        {
            string label = "(" + s.Type + ") ";

            label += s.Verb;
            if (s.Do != "")
            {
                label += " " + s.Do;
            }
            if (s.Prep != "")
            {
                label += " " + s.Prep;
            }
            if (s.Io != "")
            {
                label += " " + s.Io;
            }

            label += "--->" + s.Sub;

            return label;
        }

        void PopulateEventRules()
        {
            rulesListBox.Items.Clear();
            foreach (Event r in xproject.Project.Events.Event)
            {
                rulesListBox.Items.Add(r.Name);
            }
        }

        void PopulateVerbs()
        {
            verbCheckListBox.Items.Clear();

            builtInVerbsTextBox.Text = "";

            //add builtin verbs
            foreach (string v in xproject.Project.Verbs.Builtinverbs.Verb)
            {
                builtInVerbsTextBox.Text += v + "\r\n";
                verbCheckListBox.Items.Add(v);
            }

            //now add the user verbs
            foreach (String s in xproject.Project.Verbs.Userverbs.Verb)
            {
                userVerbListView.Items.Add(s);
                verbCheckListBox.Items.Add(s);
            }
        }



        void SetRoomDirections()
        {
            int room = objectsComboBox.SelectedIndex;
            //nComboBox.SelectedIndex = -1;
            nComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.N);
            sComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.S);
            eComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.E);
            wComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.W);
            neComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Ne);
            nwComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Nw);
            seComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Se);
            swComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Sw);
            enterComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.In);
            outComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Out);
            upComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Up);
            downComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Directions.Down);
            parentComboBox.SelectedIndex = GetIndex(xproject.Project.Objects.Object[room].Holder);
            massTextBox.Text = xproject.Project.Objects.Object[room].Directions.Mass;
        }


        /// <summary>
        /// This function does something.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool GetCheck(string s)
        {
            if (s == null || s.Equals("0"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void SetCheckBoxes()
        {
            int room = objectsComboBox.SelectedIndex;
            portableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Portable);
            openCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Open);
            openableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Openable);
            lockableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Lockable);
            lockedCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Locked);
            emittingLightCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Emittinglight);
            visibleCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Scenery);
            user3CheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Backdrop);
            user1CheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.User1);
            user2CheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.User2);
            user3CheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.User3);

            doorCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Door);
            wearableCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Wearable);
            beingwornCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.BeingWorn);
            containerCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Container);
            supporterCheckBox.Checked = GetCheck(xproject.Project.Objects.Object[room].Flags.Supporter);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "SW");
        }

        private void objectsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //update the connections and properties
            int i = objectsComboBox.SelectedIndex;

            Object o = xproject.Project.Objects.Object[i];

            objNameTextBox.Text = o.PrintedName;
            this.objectIdNameTextBox.Text = o.Name;
            objDescTextBox.Text = o.Description;
            initialDescTextBox.Text = o.Initialdescription;

            //set the holder
            int parent = Convert.ToInt32(o.Holder);
            parentComboBox.SelectedIndex = parent;

            SetRoomDirections();
            SetCheckBoxes();

            if (o.Synonyms != null)
            {
                synonymsTextBox.Text = o.Synonyms.Names;
            }
            else
            {
                o.Synonyms = new Synonyms();
                o.Synonyms.Names = "";
            }

            if (i < 2)
                objNameTextBox.ReadOnly = true;
            else
                objNameTextBox.ReadOnly = false;

            if (o.Flags.Door == "1")
            {
                movementAlertsButton.Enabled = false;
            }
            else
            {
                movementAlertsButton.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveRecents();
            Application.Exit();
        }

        private void functionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void movementAlertsButton_Click(object sender, EventArgs e)
        {
            if (GetCurObj() != null)
            {
                if (!GetCurObj().Id.Equals("1"))
                {
                    NoGoMessages ngm = new NoGoMessages(objectsComboBox.SelectedIndex);

                    ngm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Player doesn't have 'You can't go that way' messages.");
                }
            }
            else
            {
                MessageBox.Show("No object is selected.");
            }
        }

        private void newVerbButton_Click(object sender, EventArgs e)
        {
            //new verb button

            if (newVerbTextBox.Text.Equals(""))
            {
                MessageBox.Show("Verb can't be blank.");

            }
            else
            {
                string v = newVerbTextBox.Text.Trim();

                //split on commas
                string[] vs = v.Split(',');
                foreach (string vb in vs)
                {
                    //check for verb,nouns
                    string[] toks = vb.Split(' ');
                    if (toks.Length >= 3)
                    {
                        MessageBox.Show("Verb can only be one or two words (i.e. 'throw' or 'throw out').  Do not included nouns!");
                        return;
                    }

                    if (toks.Length == 2)
                    {
                        bool found = false;
                        foreach (string p in prepositionsComboBox.Items)
                        {
                            if (p == toks[1])
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            MessageBox.Show("For two word verbs, the second word must be a preposition (i.e. 'throw out')");
                            return;
                        }
                    }//end 2 words

                    //check for dupes!

                    if (IsDupeVerb(vb))
                    {
                        MessageBox.Show("The verb '" + vb + "' already exists.");
                        return;
                    }

                }//end foreach verb in comma separated list

                //must be ok
                xproject.Project.Verbs.Userverbs.Verb.Add(v);
                userVerbListView.Items.Add(v);
                verbComboBox.Items.Add(v);
                verbCheckListBox.Items.Add(v);
                newVerbTextBox.Text = "";
            }
        }

        private void newFuncButton_Click(object sender, EventArgs e)
        {
            //new function button

            if (funcNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("Function name can't be blank.");
            }
            else
            {
                //check for duplicate names
                string name = funcNameTextBox.Text;
                name = name.Replace(' ', '_');

                if (!ValidateName(name))
                    return;

                foreach (Routine rt in xproject.Project.Routines.Routine)
                {
                    if (rt.Name.ToUpper() == name.ToUpper())
                    {
                        MessageBox.Show("There is already a function with that name.");
                        return;
                    }
                }

                funcNameTextBox.Text = name;
                Routine r = new Routine();
                r.Name = name;

                r.Text = "";

                xproject.Project.Routines.Routine.Add(r);

                //update list boxes with functions
                functionsListBox.Items.Add(funcNameTextBox.Text);
                sentenceFuncComboBox.Items.Add(funcNameTextBox.Text);
                funcNameTextBox.Text = "";

                functionsListBox.SelectedIndices.Add(functionsListBox.Items.Count - 1);//select last
            }
        }

        private void addRuleButton_Click(object sender, EventArgs e)
        {
            //new verb button
            if (ruleNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("Name can't be blank.");
            }
            else
            {
                string name = ruleNameTextBox.Text.Trim().Replace(' ', '_');

                if (!ValidateName(name))
                    return;

                //check for dupes!

                foreach (string s in this.rulesListBox.Items)
                {
                    if (s.ToUpper().Equals(name.ToUpper()))
                    {
                        MessageBox.Show("There is already an event with that name.");
                        ruleNameTextBox.Clear();
                        ruleNameTextBox.Focus();
                        return;
                    }
                }

                Event evt = new Event();

                ruleNameTextBox.Text = name;

                evt.Name = ruleNameTextBox.Text;
                evt.Text = "";
                xproject.Project.Events.Event.Add(evt);
                rulesListBox.Items.Add(evt.Name);
                rulesListBox.SelectedIndex = rulesListBox.Items.Count - 1;
                ruleCodeTextBox.Text = "";
                ruleNameTextBox.Text = "";
            }
        }

        private void ruleListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void ToggleProperty(Flags flags, string propName, bool newVal)
        {
            try
            {

                string newFlag = "1";
                if (newVal) newFlag = "0";

                flags.GetType().InvokeMember(propName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                       Type.DefaultBinder, flags, new object[] { newFlag });



            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to set property " + propName);
            }
        }

        private void portableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        Object GetCurObj()
        {
            if (objectsComboBox.SelectedIndex == -1) { return null; }
            return xproject.Project.Objects.Object[objectsComboBox.SelectedIndex];
        }

        private void containerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

        }

        private void supporterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void visibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

            if (cb.Checked)
            { //scenery objects aren't portable
              //                ToggleProperty(GetCurObj().Flags, "Portable", false);
                portableCheckBox.Checked = false;
            }
        }

        //this is now user3 checkbox
        private void backdropCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void openCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void openableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void lockableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void lockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void emittingLightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void doorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);

            //clear any nogo messages

            GetCurObj().Nogo.N = "";
            GetCurObj().Nogo.S = "";
            GetCurObj().Nogo.E = "";
            GetCurObj().Nogo.W = "";
            GetCurObj().Nogo.Ne = "";
            GetCurObj().Nogo.Nw = "";
            GetCurObj().Nogo.Se = "";
            GetCurObj().Nogo.Sw = "";
            GetCurObj().Nogo.Up = "";
            GetCurObj().Nogo.Down = "";

        }

        private void edibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void flammableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void drinkableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender; ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void rulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rulesListBox.SelectedIndices.Count > 0)
            {//something selected

                int index = rulesListBox.SelectedIndices[0];
                string text = xproject.Project.Events.Event[index].Text;
                if (text != null && text.Length > 0)
                {
                    ruleCodeTextBox.Text = FormatCode(text);
                }
                else
                    ruleCodeTextBox.Text = "";
            }
        }

        private void functionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (functionsListBox.SelectedIndices.Count > 0)
            {
                codeTextBox.Text = xproject.Project.Routines.Routine[functionsListBox.SelectedIndices[0]].Text;
                //   codeTextBox.Text = FormatCode(xproject.Project.Routines.Routine[functionsListBox.SelectedIndices[0]].Text);
            }
        }


        string FormatCode(string code)
        {
            return code;
            //return CodeFormatter.Format(code);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        Event FindEventRuleByName(string name)
        {
            foreach (Event e in xproject.Project.Events.Event)
            {
                if (e.Name == name)
                {
                    return e;
                }
            }

            return null;
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void saveVarValueButton_Click(object sender, EventArgs e)
        {

            //add a new variable
            string varName = varNameTextBox.Text.Trim().Replace(' ', '_');

            if (varName.Length == 0)
            {
                MessageBox.Show("Missing variable name.");
                return;
            }

            varName = varName.Replace("$", string.Empty);

            if (!ValidateName(varName))
                return;


            //varNameTextBox.Text = varNameTextBox.Text.Trim().Replace(' ', '_');
            varNameTextBox.Text = varName;


            int val;
            if (!Int32.TryParse(valueTextBox.Text, out val))
            {
                MessageBox.Show("Variable value must be an integer 0 - 255.");
                valueTextBox.Focus();
                return;
            }

            if (val > 255)
            {
                MessageBox.Show("Variable value must be less than 256.  Yes, this is lame.");
                valueTextBox.Focus();
                return;
            }

            if (val < 0)
            {
                MessageBox.Show("Variable value must be between 0 and 256.  Yes, this is lame.");
                valueTextBox.Focus();
                return;
            }


            //check for duplicates in user and built-in variables
            IEnumerable<Var> vars = from v in xproject.Project.Variables.User.Var
                                    where v.Name.Equals(varName)
                                    select v;

            if (vars.Count<Var>() > 0)
            {
                vars.First<Var>().Value = val.ToString();
                MessageBox.Show("Variable updated.");
            }
            else
            {
                Var newVar = new Var();
                newVar.Name = varNameTextBox.Text;
                newVar.Value = val.ToString();
                newVar.Addr = Name.Replace(' ', '_');
                xproject.Project.Variables.User.Var.Add(newVar);
                userVarsListBox.Items.Add(newVar.Name);
            }

        }

        private void verbCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //populate the checks based on what is already configured for that verb
            List<string> checks = new List<string>();

            foreach (string s in allChecks)
            {
                checks.Add(s);
            }

            string curVerb = verbCheckListBox.SelectedItem.ToString();
            List<string> verbChecks = new List<string>();

            if (curVerb.IndexOf(",") != -1)
            {
                curVerb = curVerb.Substring(0, curVerb.IndexOf(","));
            }

            var list =
                from c in xproject.Project.Checks.Check
                where c.Verb.Equals(curVerb)
                //       orderby c._check ascending
                select c;

            verbChecksListBox.Items.Clear();
            foreach (var v in list)
            {
                verbChecksListBox.Items.Add(v._check);
            }


        }

        void SetDirection(ComboBox cb, string dir)
        {
            string dirAttr = cb.SelectedIndex.ToString();

            if (cb.SelectedIndex == cb.Items.Count - 1)
            {//if last is selected, then "You can't go that way"
                dirAttr = "255";
            }

            if (dir.Equals("N"))
            {
                GetCurObj().Directions.N = dirAttr;
                GetCurObj().Nogo.N = "";
            }
            else if (dir.Equals("S"))
            {
                GetCurObj().Directions.S = dirAttr;
                GetCurObj().Nogo.S = "";
            }
            else if (dir.Equals("E"))
            {
                GetCurObj().Directions.E = dirAttr;
                GetCurObj().Nogo.E = "";
            }
            else if (dir.Equals("W"))
            {
                GetCurObj().Directions.W = dirAttr;
                GetCurObj().Nogo.W = "";
            }
            else if (dir.Equals("NE"))
            {
                GetCurObj().Directions.Ne = dirAttr;
                GetCurObj().Nogo.Ne = "";
            }
            else if (dir.Equals("SE"))
            {
                GetCurObj().Directions.Se = dirAttr;
                GetCurObj().Nogo.Se = "";
            }
            else if (dir.Equals("SW"))
            {
                GetCurObj().Directions.Sw = dirAttr;
                GetCurObj().Nogo.Sw = "";
            }
            else if (dir.Equals("NW"))
            {
                GetCurObj().Directions.Nw = dirAttr;
                GetCurObj().Nogo.Nw = "";
            }
            else if (dir.Equals("UP"))
            {
                GetCurObj().Directions.Up = dirAttr;
                GetCurObj().Nogo.Up = "";
            }
            else if (dir.Equals("DOWN"))
            {
                GetCurObj().Directions.Down = dirAttr;
                GetCurObj().Nogo.Down = "";
            }
            else if (dir.Equals("IN"))
            {
                GetCurObj().Directions.In = dirAttr;
                GetCurObj().Nogo.In = "";
            }
            else if (dir.Equals("OUT"))
            {
                GetCurObj().Directions.Out = dirAttr;
                GetCurObj().Nogo.Out = "";
            }
            else
                throw new Exception("Invalid direction " + dir);

            //clear any nogo message



        }

        private void nComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "N");
        }

        private void neComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "NE");
        }

        private void eComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "E");
        }

        private void nwComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "NW");
        }

        private void wComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "W");
        }

        private void outComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "OUT");
        }

        private void sComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "S");
        }

        private void upComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "UP");
        }

        private void downComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "DOWN");
        }

        private void parentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newSel = ((ComboBox)sender).SelectedIndex;
            int curObj = objectsComboBox.SelectedIndex;

            if (curObj != 0)
            {
                if (newSel != curObj)
                {
                    GetCurObj().Holder = ((ComboBox)sender).SelectedIndex.ToString();
                }
                else
                {
                    MessageBox.Show("An object cannot contain itself!");
                    ((ComboBox)sender).SelectedIndex = 0;
                }
            }
        }

        private void backdropTextBox_TextChanged(object sender, EventArgs e)
        {
            Object o = GetCurObj();
            if (o != null)
            {
                string text = backdropTextBox.Text.Trim();

                if (text != "")
                {
                    if (o.Backdrop == null)
                        o.Backdrop = new Backdrop();

                    o.Backdrop.Rooms = text;
                }
            }
        }

        private void synonymsTextBox_TextChanged(object sender, EventArgs e)
        {
            GetCurObj().Synonyms.Names = ((TextBox)sender).Text;
        }

        void NewProject()
        {
            xproject = new Xml();
            xproject.Project = new Project();
            xproject.Project.ProjName = "NEW PROJECT";
            xproject.Project.Version = "Version 1.0";
            xproject.Project.Author = "Your name";
            xproject.Project.Language = "English";
            xproject.Project.Welcome = "Short welcome message.";
            xproject.Project.Objects = new Objects();
            xproject.Project.Objects.Object = new List<Object>();
            xproject.Project.Preps = new Preps();
            xproject.Project.Preps.Prep = new List<string>();
            xproject.Project.Variables = new Variables();
            xproject.Project.Variables.User = new User();
            xproject.Project.Variables.User.Var = new List<Var>();
            xproject.Project.Variables.Builtin = new Builtin();
            xproject.Project.Variables.Builtin.Var = new List<Var>();
            xproject.Project.Verbs = new Verbs();
            xproject.Project.Verbs.Builtinverbs = new Builtinverbs();
            xproject.Project.Verbs.Builtinverbs.Verb = new List<string>();
            xproject.Project.Verbs.Userverbs = new Userverbs();
            xproject.Project.Verbs.Userverbs.Verb = new List<string>();
            xproject.Project.Routines = new Routines();
            xproject.Project.Routines.Routine = new List<Routine>();
            xproject.Project.Checks = new Checks();
            xproject.Project.Checks.Check = new List<Check>();
            xproject.Project.Events = new Events();
            xproject.Project.Events.Event = new List<Event>();
            xproject.Project.Sentences = new Sentences();
            xproject.Project.Sentences.Sentence = new List<Sentence>();
            xproject.Project.BuildSettings = new BuildSettings();
            xproject.Project.Output = "adventure";
            AddDefaultObjects();
            AddDefaultVerbs();
            AddDefaultVerbChecks();
            AddPrepositions();
            AddDefaultVars();
            AddDefaultFunctions();
            AddDefaultSentences();
            
            //ShowProject();
        }

        Object CreateObject(string name)
        {
            Object o = new Object();
            o.Name = name;
            o.PrintedName = name;
            o.Initialdescription = "";
            o.Id = xproject.Project.Objects.Object.Count.ToString();
            o.Description = "You notice nothing unexpected.";

            o.Directions = new Directions();
            o.Directions.N = "255";
            o.Directions.S = "255";
            o.Directions.E = "255";
            o.Directions.W = "255";
            o.Directions.Ne = "255";
            o.Directions.Se = "255";
            o.Directions.Sw = "255";
            o.Directions.Nw = "255";
            o.Directions.Ne = "255";
            o.Directions.Down = "255";
            o.Directions.Up = "255";
            o.Directions.Out = "255";
            o.Directions.In = "255";
            o.Directions.Mass = "0";

            o.Holder = "0";

            o.Nogo = new Nogo();
            o.Nogo.Out = "I don't know which way that is.";
            o.Nogo.In = "You can't enter that.";

            o.Synonyms = new Synonyms();
            o.Backdrop = new Backdrop();
            o.Flags = new Flags();

            o.Flags.Backdrop = "0";
            o.Flags.Portable = "0";
            o.Flags.Scenery = "0";
            o.Flags.Open = "0";
            o.Flags.Openable = "0";
            o.Flags.Locked = "0";
            o.Flags.Lockable = "0";
            o.Flags.Emittinglight = "0";
            o.Flags.Wearable = "0";
            o.Flags.User1 = "0";
            o.Flags.Container = "0";
            o.Flags.Supporter = "0";
            o.Flags.Door = "0";
            o.Flags.BeingWorn = "0";
            o.Flags.Transparent = "0";
            o.Flags.User2 = "0";
            return o;
        }

        void AddDefaultObjects()
        {
            Object off = CreateObject("Offscreen");
            off.Description = "Offscreen.  Move objects here to remove them from the world.";
            xproject.Project.Objects.Object.Add(off);

            Object player = CreateObject("player");
            player.Holder = "2";
            player.Description = "You're a wonderful person. You shouldn't care what you look like.";
            player.Synonyms.Names = "me,self";
            xproject.Project.Objects.Object.Add(player);

            Object room1 = CreateObject("Room 1");
            room1.Holder = "0";
            room1.Name = "room_1";
            room1.PrintedName = "Room 1";
            room1.Description = "This is the end of a dirt road.";
            room1.Flags.Emittinglight = "1";
            xproject.Project.Objects.Object.Add(room1);

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete your current project.  Proceed?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                verbCheckListBox.Items.Clear();
                userVerbListView.Clear();
                ruleCodeTextBox.Clear();
                NewProject();
                ShowProject();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            xproject.Project.Objects.Object.Add(CreateObject("OBJECT " + xproject.Project.Objects.Object.Count));

            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;
        }

        private void objNameTextBox_TextChanged(object sender, EventArgs e)
        {
            /*warn about rooms/objects starting with a digit*/

            if (objNameTextBox.Text.Length > 0)
            {
                char ch = objNameTextBox.Text[0];
                if (Char.IsDigit(ch))
                {
                    MessageBox.Show("Object names can't start with digits.");
                    objNameTextBox.SelectionStart = 1;
                }
            }

        }

        private void objDescTextBox_TextChanged(object sender, EventArgs e)
        {
            int caret = objDescTextBox.SelectionStart;

            if (objDescTextBox.Text.Length > 255)
            {
                objDescTextBox.Text = objDescTextBox.Text.Substring(0, 256);
            }

            SetDescription();
            objDescTextBox.SelectionStart = caret;
        }

        private void initialDescTextBox_TextChanged(object sender, EventArgs e)
        {
            //GetCurObj().Initialdescription = initialDescTextBox.Text;
            int caretPos = initialDescTextBox.SelectionStart;
            SetInitialDesc();
            initialDescTextBox.SelectionStart = caretPos;
        }


        void CreateDefaultVar(string name, string addr, string val)
        {
            Var v = new Var();
            v.Name = name;
            v.Addr = addr;
            v.Value = val;
            xproject.Project.Variables.Builtin.Var.Add(v); //add to xml doc
            builtinVarsListBox.Items.Add(v.Name); //add to drop box
        }


        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //add sentence

            //make sure there isn't a sentence that matches that already
            Sentence s = new Sentence();
            //s.Verb = TrimVerb()
            s.Verb = TrimVerb(verbComboBox.SelectedItem.ToString());

            if (sentenceIndirectObjComboBox.SelectedItem.ToString() != "")
            {//make sure a do and a prep are supplied

                if (sentenceObjectComboBox.SelectedItem == null ||
                    sentenceObjectComboBox.SelectedItem.ToString().Equals(""))
                {
                    MessageBox.Show("If you have an indirect object, you also need a direct object and a preposition.");
                    return;
                }

            }


            s.Do = TrimNoun(sentenceObjectComboBox.SelectedItem.ToString());

            s.Prep = prepositionsComboBox.SelectedItem.ToString();
            s.Io = TrimNoun(sentenceIndirectObjComboBox.SelectedItem.ToString());

            if (this.sentenceFuncComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Function to call can't be blank.");
                return;
            }

            s.Sub = this.sentenceFuncComboBox.SelectedItem.ToString();
            s.Type = this.sentenceTypeComboBox.SelectedItem.ToString();
            if (!SentenceExists(s))
            {
                xproject.Project.Sentences.Sentence.Add(s); //add to xml
                PopulateSentences(); //refresh to list
            }
            else
            {
                MessageBox.Show("A sentence with those words already exists.");
            }
        }

        /// <summary>
        /// Returns true if there is already a sentence with these words
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool SentenceExists(Sentence newSentence)
        {
            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                if (s.Verb == newSentence.Verb &&
                    s.Do == newSentence.Do &&
                    s.Prep == newSentence.Prep &&
                    s.Io == newSentence.Io &&
                    s.Type == newSentence.Type)
                {
                    return true;
                }
            }
            return false;
        }

        //removes trailing synonyms
        string TrimVerb(string verb)
        {
            if (verb.IndexOf(",") != -1)
            {//has synonyms
                verb = verb.Substring(0, verb.IndexOf(",")).Trim();
            }
            return verb;
        }

        //removes the object number
        string TrimNoun(string name)
        {
            if (name.IndexOf('(') != -1)
            {
                name = name.Substring(0, name.IndexOf("(")).Trim();
            }
            return name;
        }


        private void builtinVarsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                varNameTextBox.Text = xproject.Project.Variables.Builtin.Var[builtinVarsListBox.SelectedIndex].Name;
                valueTextBox.Text = xproject.Project.Variables.Builtin.Var[builtinVarsListBox.SelectedIndex].Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileName = SaveAs();
        }

        string SaveAs()
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Save as...";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                Save();
            }

            return fileName;
        }

        private void authorTextBox_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.Author = authorTextBox.Text;
        }

        private void sentencesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void codeTextBox_TextChanged(object sender, EventArgs e)
        {
            //get the current function
            string name = functionsListBox.SelectedItem.ToString();

            var funcs =
                    from f in xproject.Project.Routines.Routine
                    where f.Name.Equals(name)
                    select f;


            //update its code
            string code = codeTextBox.Text;
            code = code.Replace("" + 8217, "");
            //funcs.First<Routine>().Text = CodeFormatter.Format(code);
            funcs.First<Routine>().Text = code;


        }

        bool IsDupeVerb(string v)
        {
            foreach (ListViewItem lvi in userVerbListView.Items)
            {
                string[] toks = lvi.Text.Split(',');

                foreach (string item in toks)
                {
                    if (item.ToUpper().Equals(v.ToUpper()))
                    {
                        return true;
                    }
                }

            }


            /*
            foreach (ListViewItem lvi in this.builtInVerbsTextBox)
            {
                string[] toks = lvi.Text.Split(',');

                if (toks.Contains(v))
                    return true;

            }
            */
            char[] seps = { '\r', '\n', ',' };
            string builtIn = builtInVerbsTextBox.Text;
            string[] verbs = builtIn.Split(seps);

            foreach (string s in verbs)
            {
                foreach (string item in verbs)
                {
                    if (item.ToUpper().Equals(v.ToUpper()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void PopulateVars()
        {
            builtinVarsListBox.Items.Clear();
            foreach (var v in xproject.Project.Variables.Builtin.Var)
            {
                builtinVarsListBox.Items.Add(v.Name);
            }

            userVarsListBox.Items.Clear();
            foreach (var v in xproject.Project.Variables.User.Var)
            {
                userVarsListBox.Items.Add(v.Name);
            }
        }

        void PopulateBuildSettings()
        {
            spectrumImageTextBox.Text = xproject.Project.BuildSettings.SpectrumLoadScreen;
            appleImageTextBox.Text = xproject.Project.BuildSettings.AppleLoadScreen;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //add a check to a verb
            //make sure it isn't already there first
            if (verbCheckListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a verb to add checks to.");
                return;
            }

            string verb = verbCheckListBox.SelectedItems[0].ToString();
            string check = allChecksListBox.SelectedItem.ToString();

            //is it a dupe?
            var checks = from c in xproject.Project.Checks.Check
                         where (c.Verb.Equals(verb) && c._check.Equals(check))
                         select c;

            if (checks.Count<Check>() > 0)
            {
                MessageBox.Show("That verb already has that check.");
                return;
            }

            Check ck = new Check();
            ck._check = check;
            ck.Verb = verb;
            xproject.Project.Checks.Check.Add(ck);

            verbChecksListBox.Items.Add(check);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void versionTextBox_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.Version = versionTextBox.Text.Trim();
        }

        private void welcomeTextBox_TextChanged(object sender, EventArgs e)
        {
            //
        }

        private void objNameTextBox_Leave(object sender, EventArgs e)
        {
            Object o = GetCurObj();
            objNameTextBox.Text = objNameTextBox.Text.Trim();
            o.PrintedName = objNameTextBox.Text;

            o.Name = o.PrintedName.Replace(' ', '_');
            objectIdNameTextBox.Text = o.Name;
            int oldIndex = objectsComboBox.SelectedIndex;
            PopulateObjectDropDowns();
            objectsComboBox.SelectedIndex = oldIndex;

            string[] words = o.PrintedName.Split(' ');
            if (words.Length > 3)
            {
                MessageBox.Show("Note: Exported games only support up to 3 words in a printed name.  It is HIGHLY recommended you shorten the name to 3 of fewer words. Example: Change 'can of Mountain Dew' to 'Mountain Dew can'");
            }

            CheckUniqueIdName();
        }

        private void initialDescTextBox_Leave(object sender, EventArgs e)
        {
            SetInitialDesc();
        }

        void SetInitialDesc()
        {
            //            string temp = initialDescTextBox.Text.Trim();
            string temp = initialDescTextBox.Text;
            temp = temp.Replace('\"', '\'');
            temp = temp.Replace('\r', ' ');
            temp = temp.Replace('\n', ' ');
            initialDescTextBox.Text = temp;

            GetCurObj().Initialdescription = temp.Trim();

        }

        private void objDescTextBox_Leave(object sender, EventArgs e)
        {
            if (objDescTextBox.Text.Length == 0)
            {
                objDescTextBox.Text = "You notice nothing unexpected.";
            }

            SetDescription();
        }

        void SetDescription()
        {

            string temp = objDescTextBox.Text;
            temp = temp.Replace('\"', '\'');
            temp = temp.Replace('\r', ' ');
            temp = temp.Replace('\n', ' ');


            GetCurObj().Description = temp.Trim();
            objDescTextBox.Text = temp;
        }

        private void sentenceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetCurSentence().Type=sentenceTypeComboBox.SelectedItem.ToString();
        }

        Sentence GetCurSentence()
        {
            return xproject.Project.Sentences.Sentence[sentenceTypeComboBox.SelectedIndex];
        }

        private void syntaxHelpButton_Click(object sender, EventArgs e)
        {
            SyntaxHelpForm sfh = new SyntaxHelpForm();
            sfh.Show();
        }

        private void enterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "IN");
        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void sentencesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = sentencesListBox.IndexFromPoint(e.Location);

            if (index != ListBox.NoMatches)
            {
                //                _selectedMenuItem = listBoxCollectionRounds.Items[index].ToString();
                //                collectionRoundMenuStrip.Show(Cursor.Position);
                //                collectionRoundMenuStrip.Visible = true;

                //context menu to confirm delete?
            }

        }

        private void sentencesListBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void sentencesListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (sentencesListBox.SelectedIndex != -1)
                {

                    int sel = sentencesListBox.SelectedIndex;
                    string s = sentencesListBox.Items[sel].ToString();


                    if (MessageBox.Show("Delete the sentence " + s + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        xproject.Project.Sentences.Sentence.RemoveAt(sel);
                        sentencesListBox.Items.RemoveAt(sel);

                        if (sel > 0)
                            sentencesListBox.SelectedIndex = sel - 1;
                    }

                }
            }
        }

        private void ruleCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            //save event
            try
            {
                string code = ruleCodeTextBox.Text;

                Event evnt = FindEventRuleByName(rulesListBox.SelectedItem.ToString());
                evnt.Text = code;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    msg += "\r\n" + ex.Message;
                }

                MessageBox.Show(msg);
            }
        }

        private void userVarsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                varNameTextBox.Text = xproject.Project.Variables.User.Var[userVarsListBox.SelectedIndex].Name;
                valueTextBox.Text = xproject.Project.Variables.User.Var[userVarsListBox.SelectedIndex].Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            int caretPos = textBox4.SelectionStart;
            string name = textBox4.Text;
            name = name.Replace(' ', '_');
            name = name.Replace(".", "");
            xproject.Project.ProjName = name;
            textBox4.Text = name;
            textBox4.SelectionStart = caretPos;

        }

        private void tRS80ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName != "")
            {

                Save();

                try
                {
                    File.Delete(xproject.Project.Output + ".cmd");

                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertTRS80(fileName);  //"f3xml.xml"

                    string cmdFile = Builder.BuildTRS80(xproject);
                    MessageBox.Show(
                           string.Format("TRS-80 build complete.\r\n{0} written to {1}.{2}.\r\n\r\n",
                           cmdFile,
                           converter.buildDir,
                           "\r\nSee help menu for TRS-80 specific commands."
                           )
                          );
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void coCoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //            fileName = Save();
            if (fileName != "")
            {
                Save();

                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.Convert6809(fileName);  //"f3xml.xml"

                    if (Builder.CoCoBuild(fileName, xproject.Project.Output))
                    {
                        MessageBox.Show("Export complete. Open the directory " + xproject.Project.ProjName + "_CoCo and run build.sh");
                    }

                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File is null.");
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void appleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    Save();
                    converter.ConvertApple2(fileName);  //"f3xml.xml"

                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in Cygwin and run: build.sh");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void functionsListBox_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (functionsListBox.SelectedIndex != -1)
                {

                    int sel = functionsListBox.SelectedIndex;
                    string s = functionsListBox.Items[sel].ToString();


                    string fnName = xproject.Project.Routines.Routine[sel].Name;

                    if (IsRequiredFunction(fnName))
                    {
                        MessageBox.Show("That function can't be deleted");
                        return;
                    }


                    if (IsFunctionUsed(fnName))
                    {
                        MessageBox.Show("That function is used by a sentence, please delete the sentence first.");
                        return;
                    }



                    if (MessageBox.Show("Delete the function " + s + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        xproject.Project.Routines.Routine.RemoveAt(sel);
                        functionsListBox.Items.RemoveAt(sel);

                        if (sel > 0)
                        {
                            functionsListBox.SelectedIndex = sel - 1;
                        }
                        else
                        {
                            codeTextBox.Text = "";
                        }
                    }
                }
            }
        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {//mass changed
            GetCurObj().Directions.Mass = massTextBox.Text.Trim();
        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void label49_Click(object sender, EventArgs e)
        {

        }

        private void zXSpectrumToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    //write the files
                    converter.ConvertSpectrum(fileName);
                    //assemble them
                    string tapeName = Builder.BuildSpeccy(xproject);

                    MessageBox.Show(
                        String.Format(
                            "Export complete. {0} created in {1}",
                                tapeName,
                                converter.buildDir
                        )
                    );
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void commodore64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertC64(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " and run build.sh or build.bat");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }

        }

        private void Save()
        {
            StreamWriter file = null;

            try
            {
                System.Xml.Serialization.XmlSerializer writer = new XmlSerializer(typeof(Xml));
                // Write the XML file.

                file = new StreamWriter(fileName);
                // Serialize the project
                writer.Serialize(file, xproject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        private void amstradCPC464ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertCPC464(fileName);  //"f3xml.xml"
                    string outputName = Builder.BuildCPC464(xproject, prefs["CPCDiskXP"]);
                    MessageBox.Show(
                        String.Format(
                            "Export complete.  File {0}.dsk written to {1}.\r\n{2}{3}{4}",
                            outputName,
                            converter.buildDir,
                            "\r\nTo run the game, open the disk in an emulator (such as Arnold) and type: run\"",
                            outputName,
                            "\""
                            )
                       );
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            /*            
                        SyntaxChecker checker = new SyntaxChecker();

                        try
                        {
                            checker.WriteRoutine(null, "test", codeTextBox.Text);
                            MessageBox.Show("Looks good");
                        }
                        catch (Exception ex)
                        {
                            string msg = ex.Message;
                            if (ex.InnerException != null)
                                msg += ex.InnerException.Message;

                            MessageBox.Show(msg);
                        }

                        MessageBox.Show("Not implemented");
                        */
        }

        private void welcomeTextBox_Leave(object sender, EventArgs e)
        {
            string text = welcomeTextBox.Text.Trim();

            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            text = text.Replace("\"", "'");
            text = text.Replace("" + (char)8217, "");

            if (text.Length > 253)
            {
                text = text.Substring(0, 253);
            }
            xproject.Project.Welcome = text;
            welcomeTextBox.Text = text;
        }

        private void launchPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();

                if (ValidProj())
                {
                    //create a modeless dialog
                    TestClient tc = new TestClient();

                    try
                    {
                        tc.SetFile(fileName);
                        tc.ShowDialog();
                    }
                    catch (KeyNotFoundException knf)
                    {
                        MessageBox.Show("Unknown object or value: " + knf.Message);
                    }
                    catch (Exception ex)
                    {

                        string msg = UnrollMessage(ex);

                        //MessageBox.Show(msg);
                        ExceptionForm ef = new ExceptionForm();
                        ef.ErrText = msg;
                        ef.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Project is not valid");
                }
            }
            else
            {
                SaveAs();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (fileName == "")
            {
                MessageBox.Show("There is no file loaded.");
            }
            else
            {
                Save();
            }
        }





        private void FromTrizbort()
        {
            string fileName = "";
            NewProject();
            TrizbortImporter ti = new TrizbortImporter(); //pass blank project to converter
            ti.Import(xproject, fileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Object obj = CreateObject("OBJECT " + xproject.Project.Objects.Object.Count);


            obj.Flags.Emittinglight = "1";

            xproject.Project.Objects.Object.Add(obj);

            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;

        }

        private void seComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDirection((ComboBox)sender, "SE");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Object obj = CreateObject("OBJECT " + xproject.Project.Objects.Object.Count);

            obj.Flags.Door = "1";
            obj.Flags.Locked = "1";
            obj.Flags.Lockable = "1";
            obj.Flags.Open = "0";
            obj.Flags.Openable = "1";

            xproject.Project.Objects.Object.Add(obj);

            //refresh list
            ShowProject();
            objectsComboBox.SelectedIndex = objectsComboBox.Items.Count - 1;
        }

        private void importTrizbortFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will create a new project. Make sure you've save your project.\r\nDo you wish to continue?", "Confirm Import", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "Trizbort Files|*.trizbort";
                    openFileDialog1.Title = "Select a Trizbort File";

                    // Show the Dialog.  
                    // If the user clicked OK in the dialog and  
                    // a .CUR file was selected, open it.  
                    if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        NewProject();
                        TrizbortImporter trizImp = new TrizbortImporter();
                        trizImp.Import(xproject, openFileDialog1.FileName);
                        FixDescriptions();
                        ShowProject();
                        MessageBox.Show("Import Complete. You will now be prompted to save your project.");
                        SaveAs();
                        //                        fileName = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void synonymsTextBox_Leave(object sender, EventArgs e)
        {
            string s = synonymsTextBox.Text.Trim();
            string[] toks = s.Split(',');

            string newToks = "";
            for (int i = 0; i < Math.Min(toks.Length, 3); i++)
            {
                if (i > 0)
                    newToks += ",";

                newToks += toks[i].Trim();
            }

            GetCurObj().Synonyms.Names = newToks;
            synonymsTextBox.Text = newToks;
        }

        private void funcNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void ruleNameTextBox_TextChanged(object sender, EventArgs e)
        {
            //string ruleNameTextBox.Text;
        }

        private void label73_Click(object sender, EventArgs e)
        {

        }

        private void rulesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (rulesListBox.SelectedIndex != -1)
            {
                rulesListBox.ContextMenuStrip = eventContextMenuStrip;
            }

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        //rename an event
        private void Rename_Click(object sender, EventArgs e)
        {
            RenameEventForm ren = new RenameEventForm();

            ren.OldName = rulesListBox.Items[rulesListBox.SelectedIndex].ToString();
            ren.ShowDialog();
            if (ren.result)
            {
                xproject.Project.Events.Event.ElementAt(rulesListBox.SelectedIndex).Name = ren.NewName;
                rulesListBox.Items[rulesListBox.SelectedIndex] = ren.NewName;
            }
        }

        //delete an event
        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really delete this event?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int index = rulesListBox.SelectedIndex;

                try
                {
                    ruleCodeTextBox.Clear();
                }
                catch { }
                rulesListBox.Items.RemoveAt(index);
                rulesListBox.ContextMenuStrip = null;

                //delete it from the xml
                xproject.Project.Events.Event.RemoveAt(index);
                rulesListBox.SelectedIndex = -1;
            }


        }

        private void verbContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string verb = userVerbListView.Items[userVerbListView.SelectedIndices[0]].Text.ToString();
            //delete a verb
            //is the verb used in a sentence?
            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                if (s.Verb.Equals(verb))
                {
                    MessageBox.Show("Unable to delete verb.  It is being used by a sentence.");
                    return;
                }
            }

            //delete from project
            xproject.Project.Verbs.Userverbs.Verb.RemoveAt(userVerbListView.SelectedIndices[0]);

            //delete from form
            userVerbListView.Items.RemoveAt(userVerbListView.SelectedIndices[0]);

            userVerbListView.ContextMenuStrip = null;

            //delete verb from verb check list
            for (int i = 0; i < verbCheckListBox.Items.Count; i++)
            {
                if (verbCheckListBox.Items[i].ToString().Equals(verb))
                {
                    verbCheckListBox.Items.RemoveAt(i);
                    i--;
                }
            }

            //delete verb from rules

            for (int i = 0; i < xproject.Project.Checks.Check.Count; i++)
            {
                if (xproject.Project.Checks.Check[i].Verb.Equals(verb))
                {
                    xproject.Project.Checks.Check.RemoveAt(i);
                    i--;
                }
            }
        }

        private void userVerbListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (userVerbListView.SelectedIndices.Count != 0)
            {
                userVerbListView.ContextMenuStrip = verbContextMenuStrip;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {

        }

        private void reviewTextBox_VisibleChanged(object sender, EventArgs e)
        {
            reviewTextBox.Clear();
            String s = "";
            foreach (Object o in xproject.Project.Objects.Object)
            {

                s += o.PrintedName + "\r\n";
                try
                {
                    try
                    {
                        if (o.Flags.Portable.Equals("1")) { s += "(PORTABLE)"; }
                    }
                    catch { }

                    try
                    {
                        if (o.Flags.Scenery.Equals("1")) { s += "(SCENERY)"; }
                    }
                    catch { }
                    s += "\r\n";
                    s += "Description:\r\n" + o.Description + "\r\n\r\n";

                    s += "Initial Description:\r\n" + o.Initialdescription + "\r\n";
                    s += "\r\n";
                }
                catch
                {
                    s += "\r\n";
                }
            }
            reviewTextBox.Text = s;
        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private bool ValidProj()
        {

            if (xproject.Project.Author == null || xproject.Project.Author == "")
            {
                MessageBox.Show("Author cannot be blank!");
                return false;
            }

            if (xproject.Project.Version == null || xproject.Project.Version == "")
            {
                MessageBox.Show("Version cannot be blank!");
                return false;
            }

            foreach (Object o in xproject.Project.Objects.Object)
            {
                try
                {
                    if (o.Flags.Door.Equals("1"))
                    {
                        int n = Convert.ToInt32(o.Directions.N);
                        int s = Convert.ToInt32(o.Directions.S);
                        int e = Convert.ToInt32(o.Directions.E);
                        int w = Convert.ToInt32(o.Directions.W);
                        int ne = Convert.ToInt32(o.Directions.Ne);
                        int se = Convert.ToInt32(o.Directions.Se);
                        int nw = Convert.ToInt32(o.Directions.Nw);
                        int sw = Convert.ToInt32(o.Directions.Sw);
                        int up = Convert.ToInt32(o.Directions.Up);
                        int dn = Convert.ToInt32(o.Directions.Down);

                        bool prob1 = (e == 255 && w != 255);
                        bool prob2 = (w == 255 && e != 255);

                        if ((n == 255 && s != 255) ||
                            (s == 255 && n != 255) ||
                            (e == 255 && w != 255) ||
                            (w == 255 && e != 255) ||
                            (nw == 255 && se != 255) ||
                            (se == 255 && nw != 255) ||
                            (sw == 255 && ne != 255) ||
                            (ne == 255 && sw != 255) ||
                            (up == 255 && dn != 255) ||
                            (dn == 255 && up != 255))
                        {
                            MessageBox.Show(o.Name + " is set as a door, but it is not connected to rooms on opposite sides.");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }//end foreach


            //warn about duplicate objects with the same id name
            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o1 = xproject.Project.Objects.Object[i];
                for (int j = i + 1; j < xproject.Project.Objects.Object.Count; j++)
                {
                    Object o2 = xproject.Project.Objects.Object[j];

                    if (o1.Name == o2.Name) //don't worry about case.
                    {
                        MessageBox.Show("ERROR: You have two objects with the same id name: " + o1.Name);
                        //return false;
                    }
                }
            }


            //warn about objects 
            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o1 = xproject.Project.Objects.Object[i];
                for (int j = i + 1; j < xproject.Project.Objects.Object.Count; j++)
                {
                    Object o2 = xproject.Project.Objects.Object[j];

                    if (o1.PrintedName.ToUpper().Equals(o2.PrintedName.ToUpper()))
                    {
                        try
                        {
                            if (o1.Flags.Portable.Equals("1") && o2.Flags.Portable.Equals("1"))
                            {
                                MessageBox.Show("WARNING: You have two portable objects named:  " + o1.Name + "\r\nThis is NOT a good idea. ");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            //warn about sentences calling missing functions!
            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                bool found = false;
                foreach (Routine r in xproject.Project.Routines.Routine)
                {
                    if (r.Name == s.Sub)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("You have a sentence calling a function named '" + s.Sub + "'.  However, this function doesn't exists!  Deleting the sentence will resolve this issue");
                    return false;
                }
            }


            //warn about objects which have themselves as synonyms
            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o1 = xproject.Project.Objects.Object[i];

                if (o1.Synonyms == null)
                    continue;

                if (o1.Synonyms.Names == null)
                    continue;

                string[] words = o1.PrintedName.Split(' ');
                string[] synonyms = o1.Synonyms.Names.Split(',');

                foreach (string s in synonyms)
                {
                    if (s == "") continue;

                    foreach (string t in words)
                    {
                        if (t == "") continue;

                        if (s.Trim() == t.Trim())
                        {

                            MessageBox.Show("Object " + o1.Name + " has itself as a synonym.");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void valueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void valueTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void codeTextBox_ParentChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.walkthrough = this.walkThroughTextBox.Text;
        }

        private void iBMPCXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.Convert8086(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in a CMD promt (or Cygwin) and run: build.bat");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void bBCMicroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertBBCMicro(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + "and run build.bat");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed!" + ex.Message + ex.InnerException.Message);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label59_Click_1(object sender, EventArgs e)
        {

        }

        private void objectIdNameTextBox_Leave(object sender, EventArgs e)
        {
            Object o = GetCurObj();
            int oldIndex = objectsComboBox.SelectedIndex;
            string temp = objectIdNameTextBox.Text;
            temp = temp.Replace(' ', '_');
            o.Name = temp;
            objectIdNameTextBox.Text = o.Name;

            PopulateObjectDropDowns();
            //objectsComboBox. ; //refreshes text
            objectsComboBox.SelectedIndex = oldIndex;
        }

        string RemoveSpecialChars(string s)
        {
            s.Replace("(", "");
            s.Replace(")", "");
            s.Replace("-", "_");
            s.Replace(" ", "_");
            return s;
        }

        bool IsFunctionUsed(string functionName)
        {
            foreach (Sentence s in xproject.Project.Sentences.Sentence)
            {
                if (s.Sub == functionName)
                    return true;
            }
            return false;
        }

        void FixOutputName()
        {
            if (xproject.Project.Output == null)
            {
                xproject.Project.Output = "ADVENTUR";
            }
        }

        private void fixCaseButton_Click(object sender, EventArgs e)
        {
            FixCase();
            MessageBox.Show("Done.");
        }

        private void makeABackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {

                string oldFileName = fileName;
                fileName = xproject.Project.ProjName + DateTime.Now.ToString();
                fileName = fileName.Replace("/", "_");
                fileName = fileName.Replace(":", "_");
                fileName += ".xml";
                Save();
                fileName = oldFileName;
                MessageBox.Show("Project save to " + fileName);

            }
        }

        void CheckUniqueIdName()
        {
            int count = 0;

            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o = xproject.Project.Objects.Object[i];

                if (o.Name.ToUpper() == objectIdNameTextBox.Text.ToUpper())
                {
                    count++;
                }
            }

            if (count > 1)
            {
                int index = objectsComboBox.SelectedIndex;
                string name = objectIdNameTextBox.Text;
                MessageBox.Show("Just a 'heads up'. You have two objects with the id  '" + name + "'.  The current object's id has been changed to " + name + index + " to make it unique");
                xproject.Project.Objects.Object[index].Name = name + index;
                objectIdNameTextBox.Text = name + index;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //select all button
        private void button6_Click(object sender, EventArgs e)
        {
            reviewTextBox.SelectionStart = 0;
            reviewTextBox.SelectionLength = reviewTextBox.Text.Length;
            Clipboard.SetText(reviewTextBox.Text);
        }

        /// <summary>
        /// Repo
        /// </summary>
        /// <param name="ex"></param>
        void ReportError(Exception ex)
        {
            string msg = ex.Message + "\r\n";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += "\r\n" + ex.Message;
            }

            //MessageBox.Show(msg);
            ExceptionForm ef = new ExceptionForm();
            ef.ErrText = msg;
            ef.ShowDialog();

        }

        private void copyWalkthroughButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(walkThroughTextBox.Text);
        }


        void FixMissingLanguage()
        {
            if (xproject.Project.Language == null)
            {
                xproject.Project.Language = "English";
            }
        }

        /// <summary>
        /// Strips out carriage returns, newlines, and unprintable characters.
        /// </summary>

        void FixDescriptions()
        {
            if (xproject.Project.Welcome.Contains("\n") ||
                xproject.Project.Welcome.Contains("\r"))
            {
                string temp = xproject.Project.Welcome.Replace("\n", "");
                temp = xproject.Project.Welcome.Replace("\r", "");
                xproject.Project.Welcome = temp;
            }

            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o = xproject.Project.Objects.Object[i];
                string s = o.Description;

                if (s.Contains("\n"))
                {
                    s = s.Replace("\n", "");
                    o.Description = s;
                    //   MessageBox.Show("nl deleted from description.");
                }
                if (s.Contains("\r"))
                {
                    s = s.Replace("\r", "");
                    o.Description = s;
                    //    MessageBox.Show("cr deleted from description.");
                }


                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == 8217)
                    {
                        int c = s[j];
                        MessageBox.Show(o.Name + " has control chars (" + c + ") in the description!");

                        o.Description = o.Description.Replace("" + (char)c, String.Empty);
                    }
                }


                s = o.Initialdescription;

                if (s.Contains("\n"))
                {
                    s = s.Replace("\n", "");
                    o.Initialdescription = s;
                    //   MessageBox.Show("newline deleted from initial description.");
                }
                if (s.Contains("\r"))
                {
                    s = s.Replace("\r", "");
                    o.Initialdescription = s;
                    //   MessageBox.Show("cr deleted from initial description.");
                }


                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == 8217)
                    {
                        int c = s[j];
                        MessageBox.Show(o.Name + " has control chars (" + c + ") in the initial description!");

                        o.Initialdescription = o.Initialdescription.Replace("" + (char)c, String.Empty);
                    }
                }

            }

            foreach (Routine r in xproject.Project.Routines.Routine)
            {
                if (r.Text != null)
                {
                    if (r.Text.Contains((char)8217))
                    {
                        r.Text = r.Text.Replace("" + (char)8217, "");
                        MessageBox.Show("Just FYI, unprintable characters were stripped from the function " + r.Name);
                    }
                }
            }

            foreach (Event e in xproject.Project.Events.Event)
            {
                if (e.Text != null)
                {
                    if (e.Text.Contains((char)8217))
                    {
                        e.Text = e.Text.Replace("" + (char)8217, "");
                        MessageBox.Show("Just FYI, unprintable characters were stripped from the event " + e.Name);
                    }
                }
            }
        }
        //Make door - lockable
        void FixDoors()
        {

            for (int i = 0; i < xproject.Project.Objects.Object.Count; i++)
            {
                Object o = xproject.Project.Objects.Object[i];

                if (o.Flags.Door == "1")
                {
                    o.Flags.Lockable = "1";
                }
            }
        }

        private void removeCheckButton_Click(object sender, EventArgs e)
        {

            if (verbCheckListBox.SelectedIndex == -1)
                return;

            string verb = verbCheckListBox.Items[verbCheckListBox.SelectedIndex].ToString();

            //if verb has synonyms, just grab the 1st one
            if (verb.IndexOf(",") != -1)
            {
                verb = verb.Split(',')[0];
            }

            //remove the check from the project and the verb
            if (verbChecksListBox.SelectedIndex != -1)
            {
                int checkIndex = verbChecksListBox.SelectedIndex;
                string check = verbChecksListBox.Items[checkIndex].ToString();

                for (int i = 0; i < xproject.Project.Checks.Check.Count; i++)
                {
                    Check c = xproject.Project.Checks.Check[i];
                    if (c.Verb == verb && c._check == check)
                    {
                        xproject.Project.Checks.Check.RemoveAt(i);
                        verbChecksListBox.Items.RemoveAt(checkIndex);
                        verbChecksListBox.SelectedIndex = -1;
                        break;
                    }
                }
            }
        }

        private void verbChecksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        string UnrollMessage(Exception ex)
        {
            string msg = ex.Message + "\r\n";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += "\r\n" + ex.Message;
            }
            return msg;
        }

        private void raspberryPiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertRPi(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in shell and run 'make' or 'sudo make'");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void user1CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void user2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ToggleProperty(GetCurObj().Flags, cb.Text, !cb.Checked);
        }

        private void windowsx64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (fileName != "")
            {
                Save();
                XmlToTables converter = XmlToTables.GetInstance();
                try
                {
                    converter.ConvertX64(fileName);  //"f3xml.xml"
                    MessageBox.Show("Export complete.  Open the directory " + converter.buildDir + " in a Linux shell (or Cygwin) and run: make (or 'sudo make')");
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
            */
        }

        private void cPMZ80ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (fileName != "")
            {

                Save();

                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertCPM(fileName);  //"f3xml.xml"

                    // if (Builder.Build(fileName, "_CPM", xproject.Project.Output, "com"))
                    string outputName = Builder.BuildCPM(xproject);

                    MessageBox.Show("Export complete.\r\n" +outputName+" written to " + converter.buildDir);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    Directory.SetCurrentDirectory(homeDir);
                }
            }
            else
            {
                MessageBox.Show("File name is null.  Please save your project before exporting.");
            }
        }

        private void label91_Click(object sender, EventArgs e)
        {

        }

        private void outputTextBox_TextChanged(object sender, EventArgs e)
        {
            xproject.Project.Output = outputTextBox.Text.Trim();
        }

        bool ValidateName(string s)
        {
            if (Char.IsDigit(s[0]))
            {
                MessageBox.Show("Names can't start with a digit.");
                return false;
            }


            foreach (char ch in s)
            {
                if (!Char.IsLetterOrDigit(ch) &&
                    ch != '_')
                {
                    MessageBox.Show("Names can only contain letter, digits, and underscores.");
                    return false;
                }
            }

            return true;
        }

        private void codeContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void codeTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (functionsListBox.SelectedIndex != -1)
            {
                codeTextBox.ContextMenuStrip = codeContextMenuStrip;
            }

        }

        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {

        }

        private void codeContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem == toolStripMenuItem6)
            {//kill with monster drop
                InsertCode(
                "if (weapon.holder == player)\r\n" +
                "{\r\n" +
                    "\tprintln(\"***ZAP***\");\r\n" +
                    "\tprintln(\"The alien slumps over dead.\");\r\n" +
                    "\talien.holder = offscreen; //move alien offscreen \r\n" +
                "\tdead_alien.holder = player.holder; //move body to player location\r\n" +
                "\tprintln(\"As the alien falls, it drops a thingy.\");\r\n" +
                    "\tdropped_item.holder = player.holder; //move dropped item to player location\r\n" +
                "}\r\n" +
                "else\r\n" +
                "{\r\n" +
                    "\tprintln(\"You don't have a weapon.\");\r\n" +
                "}\r\n",
                codeTextBox
                );

            }
        }

        private void killAnEnemywithBodyLeftBehindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //kill with monster drop
            InsertCode(
            "if (weapon.holder == player)\r\n" +
            "{\r\n" +
                "\tprintln(\"***ZAP***\");\r\n" +
                "\tprintln(\"The alien slumps over dead.\");\r\n" +
                "\talien.holder = offscreen; //move alien offscreen \r\n" +
            "\tdead_alien.holder = player.holder; //move body to player location\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
                "\tprintln(\"You don't have a weapon.\");\r\n" +
            "}\r\n",
            codeTextBox
            );

        }

        private void findAndItemBySearchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
            "if (item.holder == offscreen)\r\n" +
            "{\r\n" +
                "\tprint(\"You find a \");\r\n" +
                "\tprint_obj_name(item);\r\n" +
                "\titem.holder = player; //move item to player\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
                "\tprintln(\"You find nothing.\");\r\n" +
            "}\r\n",
            codeTextBox
            );

        }

        private void requireAKeycardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
            "if ( player.holder == room ) \r\n" +
            "{\r\n" +
            "\tif (keycard.holder == player)\r\n" +
            "\t{\r\n" +
            "\t\tprintln(\"You swipe the keycard.\");\r\n" +
            "\t\tmove();\r\n" +
            "\t}\r\n" +
            "\telse\r\n" +
            "\t{\r\n" +
                "\t\tprintln(\"***BEEP***.\");\r\n" +
                "\t\tprintln(\"Keycard required.\");\r\n" +
            "\t}\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
            "\tmove();\r\n" +
            "}",
            codeTextBox
            );

        }

        private void blockAnExitToolStripMenuItem_Click(object sender, EventArgs e)
        {//block an exit
            InsertCode(
            "if ( player.holder == monster.holder ) \r\n" +
            "{\r\n" +
                "\tprintln(\"The monster blocks that exit!\");\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
            "\tmove();\r\n" +
            "}",
            codeTextBox
            );

        }

        private void haveAnEnemyAttackThePlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {//event attack player
            InsertCode(
            "//change enemy to the appropriate object\r\n" +
            "//change text as needed\r\n" +
            "if (player.holder == enemy.holder)\r\n" +
"{\r\n" +
"\tif(enemy.user3 == 0)\r\n" +
"\t{\r\n" +
"\t\tprintln(\"The enemy draws his weapon!\");\r\n" +
"\t\tenemy.user3 = 1;\r\n" +
"\t}\r\n" +
"\telse\r\n" +
"\t{\r\n" +
"\t\tprintln(\"The enemy fires his thingy at you!\");\r\n" +
"\t\thealth -= 25; //change this number as needed\r\n" +
"\t\tif(health == 0 || health > 100)\r\n" +
"\t\t{\r\n" +
"\t\t\tprintln(\"***YOU HAVE DIED***\");\r\n" +
"\t\t\t//reset game or call function to reset game\r\n" +
"\t\t\tplayer.holder = 2; //player back to start room\r\n" +
"\t\t\thealth = 100;\r\n" +
"\t\t\tenemy.user3 = 0; //wait to fire\r\n" +
"\t\t\tlook();\r\n" +
"\t\t}\r\n" +
"\t}\r\n" +
"}\r\n",
            ruleCodeTextBox
            );
        }

        private void ruleCodeTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (rulesListBox.SelectedIndex != -1)
            {
                ruleCodeTextBox.ContextMenuStrip = eventCodeContextMenuStrip;
            }
        }

        private void requireDisguiseOrClothingToMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {//require a disguise
            InsertCode(
            "//change room and item names as need\r\n" +
            "if (player.holder == location && uniform.beingworn == 0 )\r\n" +
            "{\r\n" +
                "\t//change as needed. You could move/kill player as well\r\n" +
                "\tprintln(\"You're not getting past the guard dressed like a prisoner!\");\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
            "\tmove();\r\n" +
            "}",
            codeTextBox
            );
        }

        private void killAnEnemynoBodyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //kill with monster drop
            InsertCode(
            "if (weapon.holder == player)\r\n" +
            "{\r\n" +
                "\tprintln(\"***ZAP***\");\r\n" +
                "\tprintln(\"Your ray gun vaporizes the alien!\");\r\n" +
                "\talien.holder = offscreen; //move alien offscreen \r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
                "\tprintln(\"You don't have a weapon.\");\r\n" +
            "}\r\n",
            codeTextBox
            );

        }

        private void switchOnAnObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            InsertCode(
                "if ( iobj.user3 == 0 )\r\n{\r\n" +
                "\tiobj.user3 = 1; // turn it on\r\n" +
                "\tprintln(\"You turn on the machine.\"); // change me\r\n" +
                "}\r\n" +
                "else\r\n" +
                "{\r\n" +
                "\tprintln(\"It's already on.\");\r\n" +
                "}\r\n"
                ,
                codeTextBox
                );

        }

        private void switchOffAnObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
       "if ( iobj.user3 == 1 )\r\n{\r\n" +
       "\tiobj.user3 = 1; // turn it on\r\n" +
       "\tprintln(\"You turn off the machine.\"); // change me\r\n" +
       "}\r\n" +
       "else\r\n" +
       "{\r\n" +
       "\tprintln(\"It's already off.\");\r\n" +
       "}\r\n"
       ,
       codeTextBox
       );

        }

        private void openADoorUsingAKeycodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void unlockADoorUsingAKeycodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //unlock door
            InsertCode(
                "if (door.locked == 1)\r\n{\r\n" +
            "\tprintln(\"Enter keycode:\");\r\n" +
            "\task();\r\n" +
            "\tif (answer == \"12345\")\r\n" +
            "\t{\r\n" +
            "\t\tprintln(\"The door swings open.\");\r\n" +
            "\t\tdoor.open = 1;\r\n" +
            "\t\tdoor.locked = 0;\r\n" +
            "\t}\r\n" +
            "\telse\r\n" +
            "\t{\r\n" +
            "\t\tprintln(\"Invalid code.\");\r\n" +
            "\t}\r\n" +
            "}\r\n" +
            "else\r\n{\r\n" +
            "\tprintln(\"The door is unlocked.\");\r\n" +
            "}"
            ,

            codeTextBox
            );

        }

        private void moveAnObjectIntoTheWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
            "//change item to the name of object to move;\r\n" +
            "//change new_room to the name of object's new location;\r\n" +
            "item.holder = new_room;\r\n",
            codeTextBox
            );

        }

        private void moveAnObjectOutOfTheWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("//change item to the name of object to move;\r\n" +
            "item.holder = offscreen;\r\n",
            codeTextBox
            );
        }

        private void printALineOfTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("println(\"Change me!\");\r\n", codeTextBox);
        }

        private void printBlankLineMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("\tprintln(\"\");\r\n", codeTextBox);
        }

        private void switchOnALightSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("object.lit = 1; //change object\r\n", ruleCodeTextBox);
        }

        void InsertCode(string code, TextBox control)
        {
            StringBuilder text = new StringBuilder(control.Text);
            int caret = control.SelectionStart;
            text.Insert(caret, code);
            control.Text = text.ToString();
        }

        private void printLineMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("println(\"change me!\");\r\n", ruleCodeTextBox);
        }

        private void printObjectNameMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("printname(object); //change object\r\n", ruleCodeTextBox);
        }

        private void moveAnObjectIntoTheWorldMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
            "//change item to the name of object to move;\r\n" +
            "//change new_room to the name of object's new location;\r\n" +
            "item.holder = new_room;\r\n",
            ruleCodeTextBox
            );

        }

        private void switchOffALightSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
            "//change item to the turn off;\r\n" +
            "object.lit = 0;\r\n",
            ruleCodeTextBox
            );
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InsertCode(
                "if (key.holder == player)\r\n" +
            "{\r\n" +
            "\tprintln(\"After a few tries, you manage to unlock the door.\");\r\n" +
            "\tdoor.open = 1;\r\n" +
            "\tdoor.locked = 0;\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
            "\tprintln(\"You need the key.\");\r\n" +
            "}\r\n",
                codeTextBox
            );

        }

        private void blockAnExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InsertCode(
         "room.n = fail(\"The way is blocked.\"); //change room, direction, and message\r\n",
         ruleCodeTextBox
         );
        }

        private void changeAConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
         "room.n = room2; //change room1,direction, and room2\r\n",
      ruleCodeTextBox
      );
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            InsertCode("if (flashlight.lit==1)\r\n{\r\n" +
                "\tprintln(\"It's already on.\");\r\n}\r\nelse\r\n{\r\n\tprintln(\"Click.\");\r\n" +
                "\tflashlight.lit = 1;\r\n" +
                "\tlook();\r\n" +
            "}\r\n",
            codeTextBox
            );
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            InsertCode("if (flashlight.lit==0)\r\n" +
            "{\r\n" +
            "\tprintln(\"It's already off.\");\r\n" +
            "\tflashlight.lit = 0;\r\n" +
            "\tlook();\r\n" +
            "}\r\n" +
            "else\r\n" +
            "{\r\n" +
            "\tprintln(\"Click.\");\r\n" +
            "\tflashlight.lit = 0;\r\n" +
            "\tlook();\r\n" +
            "}\r\n",
            codeTextBox
            );
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            InsertCode(
                         "if (bookcase.user1 == 0) //change name of object\r\n" +
                         "{\r\n" +
                            "\tbookcase.user1 = 1;\r\n" +
                            "\tprintln(\"You push the bookcase aside revealing a passage leading east.\"); //change this!\r\n" +
                        "\tanswer = player.holder;\r\n" +
                            "\tanswer.e = room_2;  //change the direction and new room! \r\n" +
                        "\tanswer.description = \"This is a study.  A passage leads east.\"; //change this description!\r\n" +
                    "}\r\n" +
                    "else\r\n" +
                    "{\r\n" +
                     "\tprintln(\"You've already moved it.\");\r\n" +
                     "}\r\n",
                         codeTextBox
                            );
        }

        private void printAnObjectsNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("printname(object); //change object\r\n", ruleCodeTextBox);
        }

        private void unlockADoorMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("door.locked = 0; //change object\r\n", ruleCodeTextBox);
        }

        private void moveAnObjectOutOfTheWorldMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("object.holder = offscreen; //change object\r\n", ruleCodeTextBox);
        }

        private void blankLineMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("\tprintln(\"\");\r\n", ruleCodeTextBox);
        }
        
        private void killThePlayerIfInDarknessTooLongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode("if (turnsWithoutLight==5)\r\n" +
                        "{\r\n" +
                        "\tprintln(\"Crash!!! It appears the ceiling has collapsed on you.\");\r\n" +
                        "\tprintln(\"***YOU HAVE DIED***\");\r\n" +
                        "//reset the game here or call a function to do it\r\n" +
                        "\tlook();\r\n" +
                        "}\r\n",
                        ruleCodeTextBox
                        );
        }

        private void toolStripPutMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
                "if (dobj == object) //change object to the thing that goes in noun 2\r\n" +
                "{\r\n" +
                "\tif (dobj.holder != iobj)\r\n" +
                "\t{\r\n" +
                "\t\tprintln(\"Done.\");\r\n" +
                "\t\tdobj.holder = iobj;\r\n" +
                "\t}\r\n" +
                "\telse\r\n" +
                "\t{\r\n" +
                "\t\tprintln(\"It's already there.\");\r\n" +
                "\t}\r\n" +
                "}\r\n" +
                "else\r\n" +
                "{\r\n" +
                "\tprintln(\"That won't fit.\");\r\n" +
                "}\r\n"
                ,
                codeTextBox
               );
        }

        private void runDownBatteriesInAFlashlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(" //batteryLife is variable you need to add\r\n" +
                        "if (batteries.holder == flashlight && flashlight.user3 == 1 && batteryLife > 0)\r\n" +
                        "{\r\n" +
                        "\tbatteryLife--; //subtract 1 from life\r\n" +
                        "\tif (batteryLife == 0)\r\n" +
                        "\t{\r\n" +
                        "\t\tprintln(\"The flashlight has gone out.\");\r\n" +
                        "\t\tflashlight.lit = 0;\r\n" +
                        "\t\tlook();\r\n" +
                        "\t}\r\n" +
                        "}\r\n",
                        ruleCodeTextBox
                        );

        }

        private void toolStripBattOnMenuItem_Click(object sender, EventArgs e)
        {
            InsertCode(
                "//this code assume you have a variable named batteryLife\r\n" +
                "//If your batteries don't rundown, remove '&& batterLife != 0;\r\n" +
                "//otherwise add the variable throught the variable tab;\r\n" +
                "if (flashlight.user3 == 0) //already on?\r\n{\r\n" +
                "\tflashlight.user3 = 1; // turn it on\r\n" +
                 "\tprintln(\"Click!\");\r\n" +
                 "\t//check if batteries in and have life left\r\n" +
                "\tif (batteries.holder == dobj && batteryLife != 0)\r\n" +
                "\t{\r\n" +
                "\t\tflashlight.lit = 1; //now it's lit\r\n" +
                "\t\tlook();\r\n" +
                "\t}\r\n" +
                "}\r\n" +
                "else\r\n" +
                "{\r\n" +
                "\tprintln(\"It's already on.\");\r\n" +
                "}\r\n"
                ,
                codeTextBox
               );
        }

        void AlphabetizeVerbs()
        {
            //verb 
        }

        private void prepositionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        bool IsRequiredFunction(string fname)
        {
            string[] funcs = { "not_possible", "listen", "drink", "default_eat", "default_drink", "default_talk", "kill_player", "wait", "smell", "default_kill", "jump", "yell" };

            return funcs.Contains(fname);
        }

        private void portableBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //make the folder
            //move into it
            //write out the code
            //copy in the LASM shell
            //assemble it
            if (fileName != "")
            {
                Save();
                try
                {
                    XmlToTables converter = XmlToTables.GetInstance();
                    converter.ConvertVM(fileName);  //"f3xml.xml"
                    string name;
                    Builder.BuildPortable(fileName, out name);

                    MessageBox.Show("Export complete.  File '" + name + "' was created in the directory " + converter.buildDir);
                }
                catch (Exception ex)
                {
                    string msg = UnrollMessage(ex);
                    ExceptionForm ef = new ExceptionForm();
                    ef.ErrText = msg;
                    ef.Show();
                }
            }
        }

        private void LoadRecent(object sender, EventArgs e)
        {
            string file = (sender as ToolStripMenuItem).Text;
            string path = recentFiles.Find((t) => t.Item1 == file ).Item2;
            LoadProject(path);            
        }

        private void Lantern_Load(object sender, EventArgs e)
        {
        }

        void LoadRecents()
        {
            if (File.Exists("recent.txt"))
            {
                string[] lines = File.ReadAllLines("recent.txt");
                foreach (string s in lines)
                {
                    string[] parts = s.Split(',');
                    string file = parts[0].Trim();
                    string path = parts[1].Trim();

                    recentFiles.Add(new Tuple<string, string>(file, path));

                    ToolStripMenuItem item = new ToolStripMenuItem(file);
                    item.Click += LoadRecent;
                    //menuStrip1.Items[0].
                    fileToolStripMenuItem.DropDownItems.Insert
                        (
                        7,
                        item
                        );
                }
            }
        }

        void SaveRecents()
        {
            //save recents
            using (StreamWriter sw = new StreamWriter("recent.txt", false))
            {
                foreach (var t in recentFiles)
                {
                    sw.WriteLine(t.Item1 + "," + t.Item2);
                }
            }
        }

        private void Lantern_Deactivate(object sender, EventArgs e)
        {
            SaveRecents();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {

        }

        private void tRS80FreHDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExceptionForm ex = new ExceptionForm();
            ex.ErrText =
                "To launch MAME run mame64 -debug trs80l2 \r\n\r\n" +
                "You can run a .cmd file in MAME using the Quickload option.\r\n\r\n" +
                "To deploy to actual machine, attach the cmd file to a disk by either..." +
                "1) Using TRS Tools to attach the .cmd to a disk.\r\n\r\n" +
                "2) Using a FreHD, boot into DOS and use the import2 command." +
                "Example: import2\"main.cmd game / cmd\"";
            ex.ShowDialog();
            
        }

        private void textBox11_TextChanged_1(object sender, EventArgs e)
        {
            HandleAutoComplete(verbSearchTextBox, verbComboBox);
        }

        private void verbComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void noun1TextBox_TextChanged(object sender, EventArgs e)
        {
            HandleAutoComplete(noun1TextBox, sentenceObjectComboBox);
        }

        void HandleAutoComplete(TextBox tb, ComboBox cb)
        {
            List<string> verbs = new List<string>();
            foreach (object o in cb.Items)
            {
                verbs.Add(o.ToString());
            }

            var results = from v in verbs
                          where v.ToUpper().StartsWith(tb.Text.ToUpper())
                          select v;

            if (results.Count() > 0)
            {
                int i = cb.Items.IndexOf(results.First());
                cb.SelectedIndex = i;
            }
        }

        private void noun2TextBox_TextChanged(object sender, EventArgs e)
        {
            HandleAutoComplete(noun2TextBox, sentenceIndirectObjComboBox);
        }

        private void appleImageButton_Click(object sender, EventArgs e)
        {

        }

        private void spectrumImageButton_Click(object sender, EventArgs e)
        {//All files (*.*)|*.*
            GetFilePath("Spectrum screen files (*.scr)|*.scr", spectrumImageTextBox);
            xproject.Project.BuildSettings.SpectrumLoadScreen = spectrumImageTextBox.Text;
        }

        /// <summary>
        /// Asks the user to pick and image file
        /// </summary>
        /// <param name="filterStr"></param>
        /// <param name="tb"></param>
        void GetFilePath(string filterStr, TextBox tb)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filterStr;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tb.Text = ofd.FileName;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dISKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ExceptionForm ex = new ExceptionForm();
                ex.ErrText= File.ReadAllText(@"notes\kaypro.txt");
                ex.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception(@"Unable to load notes\kaypro.txt", ex);
            }
        }

        private void locataCPCDiskButton_Click(object sender, EventArgs e)
        {
            GetFilePath("CPCDiskXP.exe|CPCDiskXP.exe", cpcDiskXPTextBox);

            if (prefs.ContainsKey("CPCDiskXP"))
                prefs.Remove("CPCDiskXP");

            prefs["CPCDiskXP"] = cpcDiskXPTextBox.Text;
            SavePrefs();
        }

        private void tabPage11_Click(object sender, EventArgs e)
        {

        }

        void SavePrefs()
        {
            using (StreamWriter sw = new StreamWriter(File.Open("prefs.txt", FileMode.OpenOrCreate)))
            {
                foreach (var p in prefs)
                {
                    sw.WriteLine(p.Key + ":" + p.Value);
                }
            }
        }

        void LoadPrefs()
        {
            if (File.Exists("prefs.txt"))
            {
                string[] pairs = File.ReadAllLines("prefs.txt");

                foreach (string s in pairs)
                {
                    int ix = s.IndexOf(":");

                    string[] parts = new string[2];

                    parts[0] = s.Substring(0, ix);
                    parts[1] = s.Substring(ix+1);

                    prefs.Add(parts[0], parts[1]);

                    if (parts[0] == "CPCDiskXP")
                        cpcDiskXPTextBox.Text = parts[1];

                }
            }
        }
    }
}
