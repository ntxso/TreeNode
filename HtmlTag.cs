using System;
using NTLib.Template;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;


namespace NTLib.Html
{
    public abstract class HtmlTag : NTree<HtmlTag>, ITagInfo, IVisitable
    {
        static int tagIDStatic;
        int tagID;
        static HtmlTag()
        { tagIDStatic = 0; }
        private readonly static string[] autoCloseTagNames = new string[] { "", "br", "hr", "img", "meta", "link", "!--" };
        HtmlAttries atries;
        string tagName = "";
        string atriesStr = "";
        TagInfo tagInfo;
        protected Literal literal = null;
        bool enable = true;
        bool manuelClose = false;
        //Text.LevelStringCollection fulStr;
        public TagInfo Info {
            get
            {
                if (tagInfo == null)
                    tagInfo = new TagInfo(this);
                return tagInfo;
            }
        }
        void ITagInfo.SetInfo()
        {
            if (inArray == null && children == null) return;
            if (Info != null) return;
            tagInfo = new TagInfo(this);
        }
        protected HtmlTag()
        {
            tagID = tagIDStatic++;
        }

        public HtmlTag(string tagNameOrFullString)
        {
            tagID = tagIDStatic++;
            check(tagNameOrFullString.Trim());
            Name = tagName;
            if (IsAutoClose())
                children = null;
            else
                children = new HtmlTagCollection(this);
            //childs.parent = this;
        }
        public HtmlTag(Literal value)
        {
            tagID = tagIDStatic++;
            literal = value;
            atries = null;
            children = null;
            if (value.IsScriptLiteral)
                Name = "script";
            else
                Name = "literal";
        }
        public List<HtmlTag> GetTagByAttributes(string attributes)
        {
            List<HtmlTag> temp = new List<HtmlTag>();
            foreach (HtmlTag tag in this) 
            {
                if (tag.Attributes.Text.Contains(attributes))
                {
                    temp.Add(tag);
                }
            }
            return temp;
        }
        public override string ToString()
        {
            if (IsDefine() && literal.Value.IndexOf("repeatable") != -1) return string.Empty;
            if (!enable) return string.Empty;
            if (IsLiteral() || IsScriptLiteral()) return literal.Value;
            if (atries == null)
            {
                return string.Format("<{0}{1}>", tagName, ((manuelClose) ? "/" : string.Empty));
            }
            else
            {
                if (!atries.IsChanged) return string.Format("<{0}{1}>", tagName + atriesStr, ((manuelClose) ? "/" : string.Empty));
                if (atries.Count > 0)
                {
                    return string.Format("<{0}{1}>", (tagName + " " + atries.ToString()), ((manuelClose) ? "/" : string.Empty));
                }
                else
                {
                    return string.Format("<{0}{1}>", tagName, ((manuelClose) ? "/" : string.Empty));
                }
            }

            //return string.Format("{0} {1}", tagName, ((atries != null) ? atries.ToString() : string.Empty)).Trim();
        }

        public string ToStringAll()
        {
            return getTagFulStr(this);
        }

        public string[] ToStringAllLiterals()
        {
            List<string> temp = new List<string>();
            foreach (var item in this)
            {
                if (item is TagLiteral)
                    temp.Add(item.literal.Value);
            }
            string[] result = new string[temp.Count];
            temp.CopyTo(result);
            return result;
        }
        public int RemoveFromInArray()
        {
            if (inArray == null) return 0;
            int index = this.Index;
            inArray.Remove(this);
            return index;
        }
        public static void SetAgain(HtmlTag tag, string htmlStr)
        {
            if (tag.inArray == null) return;
            HtmlTagCollection tempArray = tag.inArray as HtmlTagCollection;
            if (tempArray == null) return;
            HtmlTag tempTag;
            int index = tag.RemoveFromInArray();

            HtmlTagCollection temp = new HtmlTagCollection(htmlStr);
            if (temp.Count > 0)
                tempTag = temp[0];
            else
            {
                temp=new HtmlTagCollection();
                string tempstr = "SetAgain Error: Yeniden Set edilen bu htmltag nesnesi oluşturulamadı!";
                tempstr += Environment.NewLine + "Ayar yapılmaya çalışılan metin bilgisi:";
                tempstr += Environment.NewLine + htmlStr;
                tempTag = new DefineTag(tempstr);
                temp.Add(tempTag);
            }
            //HtmlInfo tempInfo = new HtmlInfo(temp);
            //tempInfo.Execute();
            tempArray.Insert(index, tempTag);
        }
        public HtmlTagCollection Children { get
            {
                if (children == null) children = new HtmlTagCollection(this);
                return (HtmlTagCollection)children;
            } }

        public HtmlAttries Attributes
        {
            get
            {
                //if (string.IsNullOrEmpty(tagName)) return null;
                if (atries == null) atries = new HtmlAttries(atriesStr);
                return atries;
            }
        }
        public bool Enabel { get { return enable; } set { enable = value; } }
        //public HtmlTag Parent { get { return parent; } internal set { parent = value; } }
        public string NameAttri
        {
            get
            {
                if (atries == null) return string.Empty;
                return atries["name"];
            }
            set
            {
                atries["name"] = value;
            }
        }
        /// <summary>
        /// ID value in it's attributes
        /// if there is not return empty string
        /// </summary>
        public string ID
        {
            get
            {
                if (atries == null) return string.Empty;
                return atries["id"];
            }
            set
            {
                atries["id"] = value.Trim();
            }
        }
        internal string Ntid
        {
            get
            {
                return atries["ntid"];
            }
            set
            {
                atries["ntid"] = value;
            }
        }
        public int TagID { get { return tagID; } }
        internal void removeNtid()
        {
            atries.Remove("ntid");
        }
        public bool IsAutoClose()
        {
            if (string.IsNullOrWhiteSpace(tagName)) return true;
            if (tagName[0] == '!') return true;
            if (manuelClose) return true;
            return (Array.IndexOf(autoCloseTagNames, tagName.ToLower()) != -1);
        }
        public bool IsLiteral()
        {
            return (this is TagLiteral);
        }
        public bool IsScriptLiteral()
        {
            return (this is ScriptLiteral);
        }
        public bool IsDefine()
        {
            if (tagName.Length < 3) return false;
            return (tagName.Substring(0, 3) == "!--");
        }
        public string TagName { get { return tagName; } set { if (value.Length > 15) return; tagName = value; } }
        public string CloseTag
        {
            get
            {
                if (IsAutoClose()) return string.Empty;
                return "</" + tagName + ">";
            }
        }
        void IVisitable.Accept(IVisitor visitor)
        { }
        private void check(string simpleTag)
        {
            simpleTag = simpleTag.Replace("<", string.Empty);
            simpleTag = simpleTag.Replace(">", string.Empty);
            simpleTag.Trim();
            if (simpleTag[simpleTag.Length - 1] == '/')
            {
                manuelClose = true;
                simpleTag = simpleTag.Remove(simpleTag.Length - 1, 1);
            }
            if (string.IsNullOrWhiteSpace(simpleTag))
            {
                tagName = "!-- NoNameTag --";
                children = null;
                atries = null;
                return;
            }
            //descript note tag control
            if (simpleTag[0] == '!')
            {
                tagName = simpleTag;
                children = null;
                atries = null;
                return;
            }
            if (simpleTag.IsOneWord()) tagName = simpleTag;
            else
            {
                int index;
                tagName = simpleTag.GetFirstWord(out index);
                atriesStr = simpleTag.Remove(0, index);
                atries = new HtmlAttries(atriesStr);
            }
        }
        private string getTagFulStr(HtmlTag tag)
        {
            string result;
            if (tag.IsLiteral() || tag.IsScriptLiteral())
                result = tag.ToString();
            else
            {
                result = new string(' ', tag.Level * 3) + tag.ToString();
                //literal and child control for newline
                //if next tag literal or child there is no newline
                //if (tag.Next != null)
                //    if (!tag.Next.IsLiteral() && tag.Level < tag.Next.Level)
                //        result += Environment.NewLine;

            }
            //recursive start
            if (tag.Children != null)
                foreach (var item in tag.Children)
                {
                    if (!tag.Next.IsLiteral()) result += Environment.NewLine;
                    result += getTagFulStr(item);
                }
            //recursive end
            if (!tag.IsLiteral())
            {
                //if next literal there in not and there is child new line
                if (tag.Children != null)
                    if (tag.Children.Count != 0)
                    {
                        if (tag.Next != null)
                            if (!tag.Next.IsLiteral())
                            {
                                result += Environment.NewLine;
                                result += new string(' ', tag.Level * 3);
                            }

                    }
                result += tag.CloseTag;
            }
            return result;
        }

    }
}
