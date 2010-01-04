# -*- coding: utf-8 -*-

from django.shortcuts import render_to_response

def login(request):
    from django.contrib.auth import authenticate, login
    
    try:
        username = request.POST['login']
        password = request.POST['password']
    except:
        return render_to_response('login.html')

    user = authenticate(username=username, password=password)
    if user is not None:
        login(request, user)
        return current_datetime(request)
    
    return render_to_response('login.html')
    
def logout(request):
    from django.contrib.auth import logout
    
    logout(request)
    return current_datetime(request)
        
def error_404(request):
    return render_to_response("404.html")
    
def error_500(request):
    return render_to_response("500.html")
    
def index(request):
    return render_to_response('index.html')
    
def html(request, filename):
    return render_to_response(filename)