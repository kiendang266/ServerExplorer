var lastPath = null;
var currentImg = null;

// Event that repositions layers when resizing window
function resizeWindow() {
    $('#colTree').css("height", $(window).height() - 40 + "px");
    $('#colUpload').css("width", ($(window).width() - 265) + "px");
    $('#colExplorer').css("height", $(window).height() - 92 + "px");
    $('#colExplorer').css("width", ($(window).width() - 265) + "px");
}


// Initialization. We generate the tree and control the event of resizing of the window
$(document).ready(function () {
    resizeWindow();
    window.onresize = resizeWindow;
    createTree();
    loadFiles(getUrlParam("id"));
});

function createTree() {
    // Settings up the tree - using $(selector).jstree(options);
    // All those configuration options are documented in the _docs folder
    $("#divTree")
		.jstree({
		    // the list of plugins to include
		    "plugins": ["themes", "json_data", "ui", "crrm", "contextmenu"],
		    // Plugin configuration

		    // I usually configure the plugin that handles the data first - in this case JSON as it is most common
		    "json_data": {
		        // I chose an ajax enabled tree - again - as this is most common, and maybe a bit more complex
		        // All the options are the same as jQuery's except for `data` which CAN (not should) be a function
		        "ajax": {
		            // the URL to fetch the data
		            "url": "scripts/jsTree/server.aspx",
		            // this function is executed in the instance's scope (this refers to the tree instance)
		            // the parameter is the node being loaded (may be -1, 0, or undefined when loading the root nodes)
		            "data": function (n) {
		                // the result is fed to the AJAX request `data` option
		                return {
		                    "operation": "get_children",
		                    "id": n.attr ? n.attr("id").replace("node_", "") : getUrlParam("id")
		                };
		            }
		        }
		    },
		    "contextmenu": {
		        "show_at_node": true,
		        "items": {
		            "create": {
		                "label": "Create",
		                "seperator_after": false,
		                "seperator_before": false
		            },
		            "remove": {
		                "label": "Delete",
		                "seperator_after": false,
		                "seperator_before": false
		            },
		            "rename": {
		                "label": "Rename",
		                "seperator_after": false,
		                "seperator_before": false,
		            },
		            "ccp": false
		        }
		    }
		})
        .bind("select_node.jstree", function (e, data) {
            console.log(data.inst._get_parent(data.rslt.obj));
            //console.log(data.inst._get_parent(data.rslt.obj).attr("id"));
            loadFiles(data.rslt.obj.attr("id"));
        })
        .bind("create.jstree", function (e, data) {
            $.post(
				"scripts/jsTree/server.aspx",
				{
				    "operation": "create_node",
				    "id": data.rslt.parent.attr("id").replace("node_", ""),
				    "position": data.rslt.position,
				    "title": data.rslt.name,
				    "type": data.rslt.obj.attr("rel")
				},
				function (r) {
				    if (r.status) {
				        $(data.rslt.obj).attr("id", r.id);
				    }
				    else {
				        $.jstree.rollback(data.rlbk);
				    }
				},
                "json"
			);
        })
		.bind("remove.jstree", function(e, data) {
            console.log(data.rslt.obj[0].parentNode);
//            if (confirm("Are you sure you want to delete the directory and all its contents?")) {
//                data.rslt.obj.each(function() {
//                    $.ajax({
//                        async: false,
//                        type: 'POST',
//                        url: "scripts/jsTree/server.aspx",
//                        data: {
//                            "operation": "remove_node",
//                            "id": this.id.replace("node_", "")
//                        },
//                        success: function(r) {
//                            if (!r.status) {
//                                data.inst.refresh();
//                            }
//                        },
//                        dataType: "json"
//                    });
//                });
//            } else {
                $.jstree.rollback(data.rlbk);
            //}
        })
		.bind("rename.jstree", function (e, data) {
		    $.post(
				"scripts/jsTree/server.aspx",
				{
				    "operation": "rename_node",
				    "id": data.rslt.obj.attr("id").replace("node_", ""),
				    "title": data.rslt.new_name
				},
				function(r) {
				    if (!r.status) {
				        $.jstree.rollback(data.rlbk);
				    }
				},
                "json"
			);
		});
};


function refreshThumbsDelayed() {
    setTimeout(refreshThumbs, 500);
}

function refreshThumbs() {
    loadFiles(lastPath);
}

// Event thrown when a tree directory is selected
function loadFiles(currentPath) {
    lastPath = currentPath;

    $("#iframeUpload").contents().find("input[type=text]").val(currentPath);

    $('#thumbs').html("<div class='thumbsLoader'><p>Searching files...<p><img src='/img/ajax-loader.gif' /></div>");

    $.ajax({
        url: 'ExplorerEngine.aspx',
        data: "action=getfiles&dir=" + currentPath,
        dataType: "json",
        cache: false,
        success: showFiles
    });
    currentImg = null;
}


// Displays the files received on the right side
function showFiles(data) {
    var html = "";
    for (var i = 0; i < data.length; i++) {
        var fileData = data[i];
        html += GenerateFileHTML(fileData);
    }
    $('#thumbs').html(html).selectable({
        filter: 'div.thumb',
        selected: function (event, ui) {
            currentImg = $(ui.selected).attr("file").replaceAll("\\", "/");
            sendFileToCKEditor(currentImg);
        }
    });
    createFileContextMenu();
}


String.prototype.replaceAll = function (toReplace, txtReplace) {
    var temp = this;
    var temp2 = this;
    do {
        temp2 = temp;
        temp = temp2.replace(toReplace, txtReplace);
    } while (temp != temp2);
    return temp;
}


// Genera el HTML de un fichero
function GenerateFileHTML(fileData) {
    var html = "<div ext='" + getFileExtension(fileData.FileName) + "' class='thumb' file='" + fileData.RelativePath + "'>" +
               "    <div class='thumb_image'><img src='" + fileData.RelativeThumbPath + "' /></div>" +
               "    <span class='thumb_name'>" + fileData.FileName + "</span>" +
               "    <span class='thumb_size'>" + fileData.KB + " KB</span>" +
               "</div>";
    return html;
}



function createFileContextMenu() {
    $('div.thumb[ext!=jpg][ext!=JPG][ext!=bmp][ext!=BMP][ext!=png][ext!=PNG]').contextMenu("fileMenu", {
        menuStyle: {
            listStyle: 'none',
            padding: '1px',
            margin: '0px',
            backgroundColor: '#fff',
            border: '1px solid #999',
            width: '120px',
            fontSize: '10px'
        },
        itemStyle: {
            margin: '0px',
            color: '#000',
            display: 'block',
            cursor: 'default',
            padding: '3px',
            border: '1px solid #fff',
            backgroundColor: 'transparent',
            fontSize: '10px'
        },
        itemHoverStyle: {
            border: '1px solid #0a246a',
            backgroundColor: '#b6bdd2'
        },
        bindings: {
            'delete': function (t) {
                var file = $(t).attr("file");
                if (confirm("Are you sure you want to delete the file?")) {
                    $.ajax({
                        url: 'ExplorerEngine.aspx',
                        data: "action=delete&file=" + file,
                        dataType: "json",
                        cache: false,
                        success: refreshThumbsDelayed
                    });
                }
            },
            'open': function (t) {
                var file = $(t).attr("file");
                window.open(file, "img");
            }
        }
    });


    $('div.thumb[ext=jpg],[ext=JPG],div.thumb[ext=bmp],[ext=BMP],div.thumb[ext=png],[ext=PNG]').contextMenu("imgMenu", {
        menuStyle: {
            listStyle: 'none',
            padding: '1px',
            margin: '0px',
            backgroundColor: '#fff',
            border: '1px solid #999',
            width: '120px',
            fontSize: '10px'
        },
        itemStyle: {
            margin: '0px',
            color: '#000',
            display: 'block',
            cursor: 'default',
            padding: '3px',
            border: '1px solid #fff',
            backgroundColor: 'transparent',
            fontSize: '10px'
        },
        itemHoverStyle: {
            border: '1px solid #0a246a',
            backgroundColor: '#b6bdd2'
        },
        bindings: {
            'delete': function (t) {
                var file = $(t).attr("file");
                if (confirm("Are you sure you want to delete the file?")) {
                    $.ajax({
                        url: 'ExplorerEngine.aspx',
                        data: "action=delete&file=" + file,
                        dataType: "json",
                        cache: false,
                        success: refreshThumbsDelayed
                    });
                }
            },
            'open': function (t) {
                var file = $(t).attr("file");
                window.open(file, "img");
            },
            'resize': function (t) {
                var file = $(t).attr("file");
                var tb = $(t).find(".thumb_image img").attr("src");

                $('#resizeImage').find(".resizePath").val(file);
                $('#resizeImage').find(".resizeThumb").attr("src", tb);

                $('#resizeImage').dialog({
                    modal: true,
                    resizable: false,
                    width: '380px',
                    buttons: {
                        OK: function () {
                            $(this).dialog("close");
                            var x = $('.resizeTamanioX').val();
                            var y = $('.resizeTamanioY').val();
                            var n = $('.resizeNuevaImagen:checked').length > 0;
                            $.ajax({
                                url: 'ExplorerEngine.aspx',
                                data: "action=resizeImage&file=" + file + "&x=" + x + "&y=" + y + "&n=" + n,
                                dataType: "json",
                                cache: false,
                                success: refreshThumbsDelayed
                            });
                        },
                        Cancel: function () {
                            $(this).dialog("close");
                        }
                    }
                }).dialog('option', 'position', 'center');

                $.ajax({
                    url: 'ExplorerEngine.aspx',
                    data: "action=imageSize&file=" + file,
                    dataType: "json",
                    cache: false,
                    success: getImageSize
                });
            }
        }
    });
}


function getImageSize(data) {
    $('.resizeTamanioActual').html(data);
}

function resizeTamanioX_changed() {
    var origX = parseInt($('.resizeTamanioActual').html().split(' ')[0]);
    var origY = parseInt($('.resizeTamanioActual').html().split(' ')[2]);

    var targX = parseInt($('.resizeTamanioX').val());
    var targY = parseInt($('.resizeTamanioY').val());

    if (isNaN(targX)) return;

    var newY = parseInt(targX * origY / origX);
    $('.resizeTamanioY').val(newY);
}

function resizeTamanioY_changed() {
    var origX = parseInt($('.resizeTamanioActual').html().split(' ')[0]);
    var origY = parseInt($('.resizeTamanioActual').html().split(' ')[2]);

    var targX = parseInt($('.resizeTamanioX').val());
    var targY = parseInt($('.resizeTamanioY').val());

    if (isNaN(targY)) return;

    var newX = parseInt(targY * origX / origY);
    $('.resizeTamanioX').val(newX);
}

function getFileDirectory(filePath) {
    filePath = filePath.replaceAll("\\", "/");
    return filePath.substring(0, filePath.lastIndexOf('/'));
}

function getFileExtension(filePath) {
    return filePath.substring(filePath.lastIndexOf('.') + 1);
}

/* INTEGRATION WITH CKEditor */

function sendFileToCKEditor(fileUrl) {
    var funcNum = getUrlParam('CKEditorFuncNum');
    if (funcNum != null) {
        if (confirm("Do you want to select this image?")) {
            window.opener.CKEDITOR.tools.callFunction(funcNum, fileUrl);
            window.close();
        }
    }
}

// Helper function to get parameters from the query string.
function getUrlParam(paramName) {
    var reParam = new RegExp('(?:[\?&]|&amp;)' + paramName + '=([^&]+)', 'i');
    var match = window.location.search.match(reParam);
    return (match && match.length > 1) ? match[1] : null;
}