# -*- coding: utf-8 -*-

from django.conf.urls.defaults import *

urlpatterns = patterns('',
    (r'^$', 'fornever.robocode.views.index'),
    (r'^sniper/$', 'fornever.robocode.views.sniper'),
    
    (r'^sitemap_robocode.xml$', 'fornever.robocode.robots.sitemap_robocode_xml'),
)