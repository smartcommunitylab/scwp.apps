using DBHelper.DBModels;
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

  public class XmlToSqlite
  {
    private List<string> smart_check_12_numbers;
    private List<string> smart_check_12_colors;
    private List<string> smart_check_16_numbers;
    private List<string> smart_check_16_colors;
    private List<RouteInfo> routesInfo;
    private XDocument doc;


    public XmlToSqlite()
    {
      smart_check_12_numbers = new List<string>();
      smart_check_12_colors = new List<string>();
      smart_check_16_numbers = new List<string>();
      smart_check_16_colors = new List<string>();
      routesInfo = new List<RouteInfo>();
      doc = XDocument.Load("strings.xml");
    }

    public List<RouteInfo> ReadRoutesInfo()
    {
      foreach (XElement el in doc.Root.Descendants("string-array"))
      {
        List<XAttribute> name = el.Attributes("name").ToList();
        string sw = name.Count > 0 ? name[0].Value : null;
        switch (sw)
        {
          case "smart_check_12_numbers": StringArray(el, smart_check_12_numbers); break;
          case "smart_check_16_numbers": StringArray(el, smart_check_16_numbers); break;
        }
      }

      foreach (XElement el in doc.Root.Descendants("array"))
      {
        List<XAttribute> name = el.Attributes("name").ToList();
        string sw = name.Count > 0 ? name[0].Value : null;
        switch (sw)
        {
          case "smart_check_12_colors": StringArray(el, smart_check_12_colors); break;
          case "smart_check_16_colors": StringArray(el, smart_check_16_colors); break;
        }
      }

      for (int i = 0; i < smart_check_12_colors.Count; i++)
      {
        routesInfo.Add(new RouteInfo()
        {
          AgencyID = "12",
          RouteID = smart_check_12_numbers[i],
          Color = smart_check_12_colors[i]
        });
      }

      for (int i = 0; i < smart_check_16_colors.Count; i++)
      {
        routesInfo.Add(new RouteInfo()
        {
          AgencyID = "16",
          RouteID = smart_check_16_numbers[i],
          Color = smart_check_16_colors[i]
        });
      }
      return routesInfo;
    }

    private void StringArray(XElement node, List<string> array)
    {
      foreach (XElement item in node.Descendants("item"))
      {
        array.Add(item.Value);
      }
    }

    public List<RouteName> ReadRoutesName()
    {
      List<RouteName> routeName = new List<RouteName>();
      foreach (XElement el in doc.Root.Descendants("string"))
      {
        List<XAttribute> name = el.Attributes("name").ToList();
        string sw = name.Count > 0 ? name[0].Value : null;

        GroupCollection r = new Regex("^agency_(?<agency>.*)_route_(?<route>.*)$").Match(sw).Groups;

        routeName.Add(new RouteName()
        {
          AgencyID = r["agency"].Value,
          RouteID = r["route"].Value,
          Name = el.Value
        });
      }
      return routeName;
    }
  }
}
