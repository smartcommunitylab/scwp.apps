using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace xmlToSqlite
{
    public struct Strings
    {
        public string agency;
        public string route;
        public string content;
    }
    public class XmlToSqlite
    {
        public List<string> smart_check_12_numbers;
        public List<string> smart_check_12_colors;
        public List<string> smart_check_16_numbers;
        public List<string> smart_check_16_colors;
        public List<Strings> strings = new List<Strings>();

        public XmlToSqlite()
        {
            smart_check_12_numbers = new List<string>();
            smart_check_12_colors = new List<string>();
            smart_check_16_numbers = new List<string>();
            smart_check_16_colors = new List<string>();
            strings = new List<Strings>();
            XDocument doc = XDocument.Load("strings.xml");

            foreach (XNode el in doc.DescendantNodes)
            {
                switch (el Name)
                {
                    case "string-array": StringArray(el); break;
                    case "array": ColorArray(el); break;
                    case "string": StringDictionary(el); break;
                }
            }

        }
        private void StringDictionary(XmlNode el)
        {
            string name = el.Attributes["name"].InnerText;
            GroupCollection r = new Regex("^agency_(?<agency>.*)_route_(?<route>.*)$").Match(name).Groups;

            strings.Add(new Strings()
            {
                agency = r["agency"].Value,
                route = r["route"].Value,
                content = el.InnerText
            });
        }

        private void ColorArray(XmlNode node)
        {
            List<string> temp = null;
            switch (node.Attributes["name"].InnerText)
            {
                case "smart_check_12_colors": temp = smart_check_12_colors; break;
                case "smart_check_16_colors": temp = smart_check_16_colors; break;
            }

            foreach (XmlNode el in node.ChildNodes)
            {
                if (el.Name == "item" && temp != null)
                {
                    temp.Add(el.InnerText);
                }
            }
        }

        private void StringArray(XmlNode node)
        {
            List<string> temp = null;
            switch (node.Attributes["name"].InnerText)
            {
                case "smart_check_12_numbers": temp = smart_check_12_numbers; break;
                case "smart_check_16_numbers": temp = smart_check_16_numbers; break;
            }

            foreach (XmlNode el in node.ChildNodes)
            {
                if (el.Name == "item" && temp != null)
                {
                    temp.Add(el.InnerText);
                }
            }
        }
    }
}
