# -*- coding: utf-8 -*-

from django.shortcuts import render_to_response
from django.http import HttpResponseRedirect
from fornever.views import meta_render
    
def index(request):
    return HttpResponseRedirect('/robocode/sniper/')

def sniper(request):
    return meta_render(request, 'robocode/sniper.html')
