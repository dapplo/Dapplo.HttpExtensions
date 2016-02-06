# xmldoc2md.ps1
# By Jaime Olivares
# URL: http://github.com/jaime-olivares/xmldoc2md

param (
    [string]$xml = $(throw "-xml is required."),
    [string]$xsl = $(throw "-xsl is required."),
    [string]$output = $(throw "-output is required.")
)

$TemplateFile = $xsl
if(!([System.IO.Path]::IsPathRooted($xsl))){
	$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $xsl)
}

$XmlDoc = $xml
if(!([System.IO.Path]::IsPathRooted($xml))){
	$XmlDoc = [System.IO.Path]::Combine($PSScriptRoot, $xml)
}

$MarkdownOutput = $output
if(!([System.IO.Path]::IsPathRooted($output))){
	$MarkdownOutput = [System.IO.Path]::Combine($PSScriptRoot, $output)
}

# var = new XslCompiledTransform(true);
$xslt = New-Object -TypeName "System.Xml.Xsl.XslCompiledTransform"

# xslt.Load(stylesheet);
$xslt.Load($TemplateFile)

# xslt.Transform(sourceFile, null, sw);
$xslt.Transform($XmlDoc, $MarkdownOutput)
