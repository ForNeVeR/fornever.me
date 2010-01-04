# -*- coding: utf-8 -*-

from django.shortcuts import render_to_response
from django.http import HttpResponseRedirect
    
def index(request):
    return HttpResponseRedirect('/robocode/sniper/')

def sniper(request):
    return render_to_response('robocode/sniper.html')
