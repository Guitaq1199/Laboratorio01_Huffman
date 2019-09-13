using System.Web;
using System.Web.Mvc;

namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
