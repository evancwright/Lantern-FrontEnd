using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLL
{
    public class AsmCommon
    {
        protected Dictionary<string, int> attrIndexes = new Dictionary<string, int>();
        protected Dictionary<string, int> propIndexes = new Dictionary<string, int>(); //bit masks
        protected Dictionary<string, string> propBytes = new Dictionary<string, string>();  //which byte to look in
        protected Dictionary<string, string> propBits = new Dictionary<string, string>(); //bit masks

        public AsmCommon()
        {
            attrIndexes.Add("id", 0);
            attrIndexes.Add("holder", 1);
            attrIndexes.Add("parent", 1);
            attrIndexes.Add("initial_description", 2);
            attrIndexes.Add("initialdescription", 2);
            attrIndexes.Add("description", 3);
            attrIndexes.Add("n", 4);
            attrIndexes.Add("s", 5);
            attrIndexes.Add("e", 6);
            attrIndexes.Add("w", 7);
            attrIndexes.Add("ne", 8);
            attrIndexes.Add("se", 9);
            attrIndexes.Add("sw", 10);
            attrIndexes.Add("nw", 11);
            attrIndexes.Add("u", 12);
            attrIndexes.Add("up", 12);
            attrIndexes.Add("down", 13);
            attrIndexes.Add("d", 13);
            attrIndexes.Add("in", 14);
            attrIndexes.Add("out", 15);
            attrIndexes.Add("mass", 16);

            //prop indexes
/*            ;	.db SCENERY_MASK; equ 1
;	.db SUPPORTER_MASK; equ 2
;	.db CONTAINER_MASK; equ 4
;	.db TRANSPARENT_MASK; equ 8
;	.db OPENABLE_MASK; equ 16
;	.db OPEN_MASK; equ 32
;	.db LOCKABLE_MASK; equ 64
;	.db LOCKED_MASK; equ 128 */

            propIndexes.Add("scenery", 1);
            propIndexes.Add("supporter", 2);
            propIndexes.Add("container", 3);
            propIndexes.Add("transparent", 4);
            propIndexes.Add("openable", 5);
            propIndexes.Add("open", 6);
            propIndexes.Add("lockable", 7);
            propIndexes.Add("locked", 8);
            /*            PORTABLE_MASK equ 1
                        EDIBLE_MASK equ 2
                        DRINKABLE_MASK equ 4
                        FLAMMABLE_MASK equ 8
                        LIGHTABLE_MASK equ 16
                        LIT_MASK equ 32
                        EMITTING_LIGHT_MASK equ 32
                        DOOR_MASK equ 64
                        UNUSED_MASK equ 128
            */
            
            propIndexes.Add("portable", 9);
            propIndexes.Add("backdrop", 10);
            propIndexes.Add("wearable", 11);
            propIndexes.Add("beingworn", 12);
            propIndexes.Add("worn", 12);
            propIndexes.Add("user1", 13);
            propIndexes.Add("lit", 14);
            propIndexes.Add("emittinglight", 14);
            propIndexes.Add("door", 15);
            propIndexes.Add("user2", 16);
            //16 is unused

            /* BYTE1
             * SCENERY_MASK equ 1
            SUPPORTER_MASK equ 2
            CONTAINER_MASK equ 4
            TRANSPARENT_MASK equ 8
            OPENABLE_MASK equ 16
            OPEN_MASK equ 32
            LOCKABLE_MASK equ 64
            LOCKED_MASK equ 128
             */

            propBytes.Add("scenery", "PROPERTY_BYTE_1");
            propBytes.Add("supporter", "PROPERTY_BYTE_1");
            propBytes.Add("container", "PROPERTY_BYTE_1");
            propBytes.Add("transparent", "PROPERTY_BYTE_1");
            propBytes.Add("openable", "PROPERTY_BYTE_1");
            propBytes.Add("open", "PROPERTY_BYTE_1");
            propBytes.Add("lockable", "PROPERTY_BYTE_1");
            propBytes.Add("locked", "PROPERTY_BYTE_1");



            /*
            PORTABLE_MASK equ 1
            EDIBLE_MASK equ 2
            DRINKABLE_MASK equ 4
            FLAMMABLE_MASK equ 8
            LIGHTABLE_MASK equ 16
            LIT_MASK equ 32	
            EMITTING_LIGHT_MASK equ 32
            DOOR_MASK equ 64
            UNUSED_MASK equ 128
            */
            propBytes.Add("portable", "PROPERTY_BYTE_2");
            propBytes.Add("backdrop", "PROPERTY_BYTE_2");
            propBytes.Add("wearable", "PROPERTY_BYTE_2");
            propBytes.Add("beingworn", "PROPERTY_BYTE_2");
            propBytes.Add("worn", "PROPERTY_BYTE_2");
            propBytes.Add("lightable", "PROPERTY_BYTE_2");
            propBytes.Add("lit", "PROPERTY_BYTE_2");
            propBytes.Add("emittinglight", "PROPERTY_BYTE_2");
            propBytes.Add("door", "PROPERTY_BYTE_2");

            //bit masks

            /*
            SCENERY_MASK equ 1
            SUPPORTER_MASK equ 2
            CONTAINER_MASK equ 4
            TRANSPARENT_MASK equ 8
            OPENABLE_MASK equ 16
            OPEN_MASK equ 32
            LOCKABLE_MASK equ 64
            LOCKED_MASK equ 128
             */

            propBits.Add("scenery", "1");
            propBits.Add("supporter", "2");
            propBits.Add("container", "4");
            propBits.Add("transparent", "8");
            propBits.Add("openable", "16");
            propBits.Add("open", "32");
            propBits.Add("lockable", "64");
            propBits.Add("locked", "128");
            /*
             PORTABLE_MASK equ 1
            EDIBLE_MASK equ 2
            DRINKABLE_MASK equ 4
            FLAMMABLE_MASK equ 8
            LIGHTABLE_MASK equ 16
            LIT_MASK equ 32	
            EMITTING_LIGHT_MASK equ 32 
            DOOR_MASK equ 64
            UNUSED_MASK equ 128
             */
            propBits.Add("portable", "1");
            propBits.Add("edible", "2");
            propBits.Add("wearable", "4");
            propBits.Add("beingworn", "8");
            propBits.Add("lightable", "16");
            propBits.Add("lit", "32");
            propBits.Add("emittinglight", "32");
            propBits.Add("door", "64");
            propBits.Add("unused", "128");
        }

        protected bool IsAttribute(string attr)
        {
            return attrIndexes.Keys.Contains(attr);
        }

        protected bool IsProperty(string attr)
        {
            return propBits.Keys.Contains(attr);
        }

        protected int GetBitPos(string propName)
        {

            double d = Convert.ToDouble(propBits[propName]);
            return (int)Math.Log(d, 2);
        }

        public virtual void BeginElse() { }
        public virtual void EndBody() { }

        public string FixVarName(string v)
        {
            if (v == "verb")
                return "VerbId";
            if (v == "dobj")
                return "DobjId";
            if (v == "prep")
                return "PrepId";
            if (v == "iobj")
                return "IobjId";
            return v;
        }
    }
}
