using System;
using System.IO;
using System.Web.UI;

namespace ServerExplorer
{
    public partial class ExplorerUploads : System.Web.UI.Page
    {
        // Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string str = Server.MapPath(@"\" + dir.Text);
                string fileName = fu.FileName;
                while (File.Exists(Path.Combine(str, fileName)))
                {
                    fileName = "0" + fileName;
                }
                File.WriteAllBytes(Path.Combine(str, fileName), fu.FileBytes);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "refresh", "<script type='text/javascript'>this.parent.refreshThumbs();</script>", false);
            }
        }
    }
}
