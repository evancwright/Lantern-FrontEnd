using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerLib
{
    class SentenceTable
    {
        List<Tuple<int, Delegate>> defaultSentences = new List<Tuple<int, Delegate>>();
        
        SentenceTable()
        {

        }
    }

    class Sentence
    {
        const int NOTHING = 255;
        const int ANY = 254;
 
        public int verb, dobj, prep, iobj;
        public string name;
        public string english;

        public Sentence(int verb, int dobj, int prep, int iobj, string name)
        {
            

            this.verb = verb;
            this.dobj = dobj;
            this.prep = prep;
            this.iobj = iobj;
            this.name = name;
            
            if (this.prep == 255)
            {
                this.prep = -1;
            }

            this.english = ToEnglish();
        }

        string ToEnglish()
        {
            Game g = Game.GetInstance();
            string english = g.GetVerb(verb);

            if (dobj == -1) return english;

            if (dobj == 254)
                english += " * ";
            else
                english += " " + g.GetObjectName(dobj);

            if (prep == -1) return english;

            english += g.GetPrep(prep);


            if (iobj == 254)
                english += " * ";
            else
                english += " " + g.GetObjectName(iobj);

            return english;
        }

    }

     
}
