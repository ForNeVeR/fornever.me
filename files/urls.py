# -*- coding: utf-8 -*-

from django.conf.urls.defaults import *

urlpatterns = patterns('',
    (r'^$', 'fornever.files.views.list'),
    (r'^upload/$', 'fornever.files.views.upload'),
    
    (r'^sitemap_files.xml$', 'fornever.files.robots.sitemap_files_xml'),
)