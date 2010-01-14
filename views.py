# -*- coding: utf-8 -*-

from django.http import HttpResponseRedirect
from django.shortcuts import render_to_response

def meta_render(request, template, context={}):
    context.update({'username': request.user.username})
    return render_to_response(template, context)

def index(request):
    return meta_render(request, 'index.html')
    
def contact(request):
    return meta_render(request, 'contact.html')
    
def programming(request):
    return meta_render(request, 'programming.html')

def login(request):
    from django.contrib.auth import authenticate, login
    
    try:
        username = request.POST['login']
        password = request.POST['password']
    except:
        return  meta_render(request, 'login.html')

    user = authenticate(username=username, password=password)
    if user is not None:
        login(request, user)
        return HttpResponseRedirect('/')
    
    return  meta_render(request, 'login.html')
    
def logout(request):
    from django.contrib.auth import logout
    
    logout(request)
    return HttpResponseRedirect('/')
    
def error_404(request):
    return  meta_render(request, '404.html')
    
def error_500(request):
    return  meta_render(request, '500.html')
