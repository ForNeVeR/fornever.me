# Run this script as administrator (due to restrictions with IE API in PowerShell).
# Do not allow it to download any cookies if it will ask.

# Warning! Check the resulting script manually for SQL injections!

if (Test-Path Quotes.sql) {
    Remove-Item Quotes.sql
}

$address = 'http://legendspbem.angelfire.com/eviloverlordlist.html'
$selector = 'p.MsoNormal'

$page = Invoke-WebRequest $address
$elements = $page.ParsedHtml.querySelectorAll($selector)

# Foreach crashes the PowerShell here for me, so use for
for ($i = 0; $i -lt $elements.length; ++$i) {
    $element = $elements[$i]
    $quote = $element.innerText.Replace("'", "''")
    @"
insert into Quotations (Source, SourceUrl, Text)
values (N'The Evil Overlord List', N'http://legendspbem.angelfire.com/eviloverlordlist.html', N'$quote')
"@ | Out-File Quotes.sql -Encoding utf8 -Append
}
