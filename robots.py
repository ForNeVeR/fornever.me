# -*- coding: utf-8 -*-

from django.http import HttpResponse

def robots_txt(request):
    from django.http import HttpResponse
    return HttpResponse('''User-agent: *
Disallow:
Allow: *
Sitemap: http://fornever.no-ip.org/sitemap.xml''')

def sitemap_xml(request):
    return HttpResponse('''<?xml version="1.0" encoding="UTF-8"?>
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

def sitemap_main_xml(request):
    return HttpResponse('''<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <url>
        <loc>http://fornever.no-ip.org/</loc>
        <changefreq>daily</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>http://fornever.no-ip.org/about.html</loc>
        <changefreq>weekly</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>http://fornever.no-ip.org/programming.html</loc>
        <changefreq>daily</changefreq>
        <priority>1.0</priority>
    </url>
</urlset>''')