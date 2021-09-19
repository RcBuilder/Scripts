WK HTML to PDF/IMG - CLI Converter

about
-----
a CLI tool to convert HTML or URI into PDF or Image. 

sources
-------
https://wkhtmltopdf.org/
https://wkhtmltopdf.org/usage/wkhtmltopdf.txt

files
-----
wkhtmltopdf.exe
wkhtmltoimage.exe

capabilities
------------
convert url to pdf or image
convert html to pdf or image

commands
--------
https://wkhtmltopdf.org/usage/wkhtmltopdf.txt
> wkhtmltopdf.exe <options> <source> <target>   // create PDF file from URL source     
> wkhtmltopdf.exe <options> <source> -          // create PDF stream from URL source 
> wkhtmltopdf.exe <options> - -                 // create PDF stream from HTML content

options
-------
https://wkhtmltopdf.org/usage/wkhtmltopdf.txt
--collate
--dpi <dpi>
--grayscale
--margin-bottom <unit>
--margin-left <unit>
--margin-right <unit>
--margin-top <unit>
--orientation <Landscape|Portrait> 
--page-size <A5|A4|A3|Letter|etc>
--page-height <unit>
--page-width <unit>
--quiet
--image-dpi <int>
--image-quality <int>
--no-pdf-compression
--title <string> 
--outline 
--no-outline  
--custom-header <name> <value>
--encoding <encoding>
--disable-external-links
--enable-forms
--images 
--no-images
--disable-javascript
--enable-local-file-access

implementations
---------------
PdfManager.cs

using
-----
> wkhtmltopdf https://example.com output.pdf	// convert url to pdf file
> wkhtmltoimage https://example.com output.jpg  // convert url to jpg image
> wkhtmltopdf https://example.com -				// convert url as pdf stream  
> wkhtmltopdf - -								// convert html as pdf stream
  <p>hello world!</p>
> wkhtmltopdf --enable-local-file-access --quiet - -
  <img src='C:\\sample.jpg' />
> wkhtmltopdf --quiet - -
  <img src='https://picsum.photos/id/1011/600/300' />
> wkhtmltopdf --encoding windows-1255 --quiet - -
  <p>ωμεν</p>

