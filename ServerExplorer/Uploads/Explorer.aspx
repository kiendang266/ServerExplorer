<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Explorer.aspx.cs" Inherits="NewAgeDesign.FileManager.Explorer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Manager</title>
    <script src="scripts/jsTree/jquery.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.core.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.widget.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.mouse.min.js" type="text/javascript"></script>    
    <script src="scripts/jquery.ui.selectable.min.js" type="text/javascript"></script>
    <script src="scripts/jsTree/jquery.jstree.js" type="text/javascript"></script>
    <script src="scripts/jquery.contextmenu.r2.packed.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.position.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.button.min.js" type="text/javascript"></script>
    <script src="scripts/jquery.ui.dialog.min.js" type="text/javascript"></script>    

    <script src="scripts/engine.js" type="text/javascript"></script>
    
    <link href="scripts/jquery.ui.theme.css" rel="stylesheet" type="text/css" />
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
   <div id="colTree">
        <h1>Carpetas</h1>
        <div id="divTree"></div>
    </div>
    <div id="colUpload">
        <iframe id="iframeUpload" src="ExplorerUploads.aspx" frameborder="0"></iframe>
    </div>
    <div id="colExplorer">
        <div id="thumbs"></div>

        <div id="resizeImage" title="Redimensionar" style="display:none">
            <input type="hidden" class="resizePath" value="" />
            <div class="resizeCol1">
                <img class="resizeThumb" src="" />
                <label class="resizeSize"></label>
            </div>
            <div class="resizeCol2">
                <label>Tamaño actual: <label class="resizeTamanioActual"></label></label><br /><br />
                <label>Nuevo tamaño: <input type="text" class="resizeTamanioX" onkeyup="resizeTamanioX_changed()" /> x <input type="text" class="resizeTamanioY" onkeyup="resizeTamanioX_changed()" /></label><br /><br />
                <input type="checkbox" class="resizeNuevaImagen" checked="checked" /> Nueva imágen
            </div>
            <div style="clear: both"></div>
        </div>
    </div>


    <div class="contextMenu" id="fileMenu">
      <ul>
        <li id="open"><img src="img/open.png" /> Abrir</li>
        <li id="delete"><img src="img/delete.png" /> Borrar</li>
      </ul>
    </div> 

    <div class="contextMenu" id="imgMenu">
      <ul>
        <li id="open"><img src="img/open.png" /> Abrir</li>
        <li id="resize"><img src="img/resize.png" /> Redimensionar</li>
        <li id="delete"><img src="img/delete.png" /> Borrar</li>
      </ul>
    </div> 
</body>
</html>
