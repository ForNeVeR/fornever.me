# -*- coding: utf-8 -*-

from django.http import HttpResponse

def robots_txt(request):
    from django.http import HttpResponse
    response = HttpResponse('''User-agent: *
Allow: /
Disallow: /admin
Sitemap: http://fornever.no-ip.org/sitemap.xml''')
    response['Content-Type'] = 'text/plain; charset=utf-8'
    return response

def sitemap_xml(request):
    response = HttpResponse('''<?xml version="1.0" encoding="UTF-8"?>
<sitemapindex xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <sitemap>
        <loc>http://fornever.no-ip.org/sitemap_main.xml</loc>
    </sitemap>
    <sitemap>
        <loc>http://fornever.no-ip.org/files/sitemap_files.xml</loc>
    </sitemap>
    <sitemap>
        <loc>http://fornever.no-ip.org/robocode/sitemap_robocode.xml</loc>
    </sitemap>
</sitemapindex>''')
    response['Content-Type'] = 'application/xml; charset=utf-8'
    return response

def sitemap_main_xml(request):
    response = HttpResponse('''<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <url>
        <loc>http://fornever.no-ip.org/</loc>
        <changefreq>daily</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>http://fornever.no-ip.org/contact/</loc>
        <changefreq>weekly</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>http://fornever.no-ip.org/programming/</loc>
        <changefreq>daily</changefreq>
        <priority>1.0</priority>
    </url>
</urlset>''')
    response['Content-Type'] = 'application/xml; charset=utf-8'
    return response