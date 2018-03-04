function PrintDiv(divName,cssPath,width,height, title)
{
    var HTML = document.getElementById(divName).innerHTML;
    PrintHTML(HTML,cssPath,width,height, title)   
}

function PrintHTML(HTML,cssPath,width,height, title)
{
    var Prop="resizable=no,scrollbars=yes,menubar=no,toolbar=no,status=no,height="+height+",width="+width;
      
    var NewWin = window.open("","",Prop);
    NewWin.document.open();
    NewWin.document.write('<html><head><title>' + title + '</title>');
    NewWin.document.write('<LINK href="'+cssPath+'" type="text/css" rel="stylesheet">');
    NewWin.document.write('</head>');
    NewWin.document.write('<body onLoad="self.print()"><center>');
    NewWin.document.write(HTML);
    NewWin.document.write('</center></body></html>');
    NewWin.document.close(); 
    NewWin.focus(); 
}

function PrintCV() {
    var handlerPath = 'GetCvHTML.ashx';
    var HTML = ($.ajax({ url: handlerPath, async: false }).responseText);
    PrintHTML(HTML, '', '740px', '500px', 'קורות חיים - רובי כהן')
}

/*
GetCvHTML.ashx:
---------------
public void ProcessRequest (HttpContext context) {
    string strCV = string.Empty;
    using (StreamReader reader = new StreamReader(context.Server.MapPath("Resources/RobyCohenCV.htm"), Encoding.UTF8))
            strCV = reader.ReadToEnd();

    context.Response.Write(strCV);
    context.Response.End();
}
*/