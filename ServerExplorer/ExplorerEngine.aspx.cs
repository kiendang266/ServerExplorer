using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace ServerExplorer
{
    public partial class ExplorerEngine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var str7 = Request.Params["action"];
            if (str7 == null) return;
            if (str7 != "getfiles")
            {
                string str2;
                switch (str7)
                {
                    case "delete":
                    {
                        str2 = @"\" + Server.UrlDecode(Request.Params["file"]);
                        str2 = Server.MapPath(str2);
                        File.Delete(str2);
                        var path = Path.Combine(Path.Combine(Path.GetDirectoryName(str2), "_thumbs"), Path.GetFileName(str2));
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                        break;
                    case "imageSize":
                    {
                        
                        str2 = @"\" + Server.UrlDecode(Request.Params["file"]);
                        var image = Image.FromFile(Server.MapPath(str2));
                        var str5 = image.Width + " x " + image.Height;
                        Response.Write(ToJSON(str5));
                        Response.End();
                    }
                        break;
                    case "resizeImage":
                    {
                        str2 = @"\" + Server.UrlDecode(Request.Params["file"]);
                        str2 = Server.MapPath(str2);
                        int iWidth = Convert.ToInt32(Request.Params["x"]);
                        int iHeight = Convert.ToInt32(Request.Params["y"]);
                        bool flag = Convert.ToBoolean(Request.Params["n"]);
                        string str6 = str2;
                        if (flag)
                        {
                            while (File.Exists(str6))
                            {
                                str6 = Path.Combine(Path.GetDirectoryName(str6), "0" + Path.GetFileName(str6));
                            }
                        }
                        ResizeImage(str2, str6, iWidth, iHeight);
                        Response.Write(ToJSON("OK"));
                        Response.End();
                    }
                        break;
                }
            }
            else
            {
                var str = Request.Params["dir"] == "null"
                    ? @"\"
                    : @"\" + Server.UrlDecode(Request.Params["dir"]);

                str = Server.MapPath(str);
                var list = new List<FileData>();
                foreach (string str3 in Directory.GetFiles(str))
                {
                    GenerateThumb(str3);
                    list.Add(new FileData(str3));
                }
                Response.Write(ToJSON(list));
                Response.End();
            }
        }

        // Methods
        private void GenerateThumb(string filePath)
        {
            var source = new[] { ".jpg", ".png", ".bmp", ".gif" };
            if (source.Contains<string>(Path.GetExtension(filePath).ToLower()))
            {
                string path = Path.Combine(Path.GetDirectoryName(filePath), "_thumbs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = Path.Combine(path, Path.GetFileName(filePath));
                if (!File.Exists(str2))
                {
                    ResizeImage(filePath, str2, 100, 0x4b);
                }
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }

        public static string ResizeImage(string strImgPath, string strImgOutputPath, int iWidth, int iHeight)
        {
            string str;
            try
            {
                var flag = strImgPath.Equals(strImgOutputPath);
                if (flag)
                {
                    strImgOutputPath = strImgPath + "___.jpg";
                }
                var source = new[] { ".jpg", ".png", ".bmp", ".gif" };
                if (!source.Contains<string>(Path.GetExtension(strImgPath).ToLower()))
                {
                    throw new Exception("Extensi\x00f3n no soportada");
                }
                Stream responseStream = null;
                if (strImgPath.StartsWith("http"))
                {
                    var request = (HttpWebRequest)WebRequest.Create(strImgPath);
                    responseStream = (request.GetResponse()).GetResponseStream();
                }
                else
                {
                    responseStream = File.OpenRead(strImgPath);
                }
                var image = new Bitmap(responseStream);
                var size = new Size(iWidth, iHeight);
                var width = image.Width;
                int height = image.Height;
                float num3 = 0f;
                float num4 = 0f;
                float num5 = 0f;
                num4 = (size.Width) / ((float)width);
                num5 = (size.Height) / ((float)height);
                num3 = num5 < num4 ? num5 : num4;
                var num6 = (int)(width * num3);
                var num7 = (int)(height * num3);
                if ((num6 <= image.Width) || (num7 <= image.Height))
                {
                    var bitmap2 = new Bitmap(num6, num7);
                    Graphics graphics = Graphics.FromImage(bitmap2);
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, 0, 0, num6, num7);
                    graphics.Dispose();
                    ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
                    var parameter = new EncoderParameter(Encoder.Quality, 0x63L);
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = parameter;
                    bitmap2.Save(strImgOutputPath, encoderInfo, encoderParams);
                    bitmap2.Dispose();
                    graphics.Dispose();
                }
                else if (strImgPath != strImgOutputPath)
                {
                    File.Copy(strImgPath, strImgOutputPath);
                }
                image.Dispose();
                responseStream.Close();
                responseStream.Dispose();
                GC.Collect();
                if (flag)
                {
                    Thread.Sleep(100);
                    File.Delete(strImgPath);
                    File.Move(strImgOutputPath, strImgPath);
                }
                str = strImgPath;
            }
            catch (Exception)
            {
                throw;
            }
            return str;
        }

        public static string ToJSON(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
    }
}
