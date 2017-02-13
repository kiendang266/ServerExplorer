using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace ServerExplorer.scripts.jsTree
{
    public partial class server : Page
    {
        private string CreateNode(HttpRequest request)
        {
            string str = request.Params["id"];
            string str2 = request.Params["position"];
            string str3 = request.Params["title"];
            string str4 = request.Params["type"];
            Directory.CreateDirectory(Path.Combine(Server.MapPath(@"\" + str), str3));
            return ToJSON(new AjaxResponse(true, Path.Combine(str, str3)));
        }

        private string GetChildren(HttpRequest request)
        {
            string str = @"\" + Server.UrlDecode(Request.Params["id"]);
            if (str == @"\null")
            {
                str = "";
            }
            string oldValue = Server.MapPath("/");
            string path = Server.MapPath(@"\" + str);
            var list = new List<TreeNode>();
            foreach (string str4 in Directory.GetDirectories(path))
            {
                string name = new DirectoryInfo(str4).Name;
                if (!name.StartsWith("_") && !name.Equals(".svn"))
                {
                    var item = new TreeNode();
                    var node2 = new attrTreeNode
                    {
                        id = str4.Replace(oldValue, "")
                    };
                    item.attr = node2;
                    item.data = name;
                    item.state = "closed";
                    list.Add(item);
                }
            }
            // thêm thuộc tính parrent vào để check về sau
            return ToJSON(list.ToArray());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string s = string.Empty;
            string str2 = base.Request.Params["operation"];
            if (str2 != null)
            {
                if (!(str2 == "get_children"))
                {
                    if (str2 == "create_node")
                    {
                        s = CreateNode(Request);
                    }
                    else if (str2 == "remove_node")
                    {
                        s = RemoveNode(Request);
                    }
                    else if (str2 == "rename_node")
                    {
                        s = RenameNode(Request);
                    }
                }
                else
                {
                    s = GetChildren(Request);
                }
            }
            Response.Write(s);
            Response.End();
        }

        private string RemoveNode(HttpRequest request)
        {
            string str = request.Params["id"];
            string path = Server.MapPath(@"\" + str);
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                Directory.Delete(path, true);
            }
            return ToJSON(new AjaxResponse(true, null));
        }

        private string RenameNode(HttpRequest request)
        {
            string str = request.Params["id"];
            string str2 = request.Params["title"];
            string path = Server.MapPath(@"\" + str);
            var info = new DirectoryInfo(path);
            string destDirName = Path.Combine(info.Parent.FullName, str2);
            if (string.Equals(path, destDirName))
                return ToJSON(new AjaxResponse(false, null));
            else
            {
                Directory.Move(path, destDirName);
                return ToJSON(new AjaxResponse(true, null));
            }
        }

        public static string ToJSON(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct AjaxResponse
        {
            public bool status;
            public string id;
            public AjaxResponse(bool s, string id)
            {
                status = s;
                this.id = id;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct attrTreeNode
        {
            public string id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TreeNode
        {
            public string data;
            public string state;
            public attrTreeNode attr;
        }
    }
}
