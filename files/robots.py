# -*- coding: utf-8 -*-

from django.http import HttpResponse

def sitemap_files_xml(request):
    response = HttpResponse('''<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <url>
        <loc>http://fornever.no-ip.org/files/</loc>
        <changefreq>daily</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>http://fornever.no-ip.org/files/upload/</loc>
        <changefreq>weekly</changefreq>
        <priority>0.5</priority>
    </url>
</urlset>''')
    response['Content-Type'] = 'application/xml; charset=utf-8'
    return response