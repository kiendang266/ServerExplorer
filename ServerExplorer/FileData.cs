using System;
using System.IO;
using System.Web;

namespace ServerExplorer
{
    public class FileData
    {
        // Fields
        public string Extension;
        public string FileName;
        public int KB;
        public string RelativePath;
        public string RelativeThumbPath;

        // Methods
        public FileData()
        {
        }

        public FileData(string filePath)
        {
            this.FileName = Path.GetFileName(filePath);
            this.Extension = Path.GetExtension(filePath).TrimStart(new char[] { '.' }).ToLower();
            this.KB = Convert.ToInt32((long)(new FileInfo(filePath).Length / 0x400L));
            // Check kỹ phần này để lấy thư mục gốc
            string fileDirectory = Path.GetDirectoryName(filePath);
            string str;
            if (string.Equals(fileDirectory + @"\", HttpContext.Current.Server.MapPath("/")))
                str = "/";
            else
                str = "/" + fileDirectory.Replace(HttpContext.Current.Server.MapPath("/"), "").Replace(@"\", "/");
            //string directoryName = Path.GetDirectoryName(filePath).Replace(HttpContext.Current.Server.MapPath("/"), "").Replace(@"\", "/");
            //string str = "/" + Path.GetDirectoryName(filePath).Replace(HttpContext.Current.Server.MapPath("/"), "").Replace(@"\", "/");
            this.RelativePath = Path.Combine(str, Path.GetFileName(filePath));
            this.RelativeThumbPath = Path.Combine(Path.Combine(str, "_thumbs"), Path.GetFileName(filePath));
            if (!File.Exists(HttpContext.Current.Server.MapPath(this.RelativeThumbPath)))
            {
                this.RelativeThumbPath = "img/filetypes/" + Path.GetExtension(filePath).TrimStart(new char[] { '.' }) + ".png";
            }
            if (!File.Exists(HttpContext.Current.Server.MapPath(this.RelativeThumbPath)))
            {
                this.RelativeThumbPath = "img/filetypes/_blank.png";
            }
        }

    }
}
