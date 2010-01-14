# -*- coding: utf-8 -*-

from django.shortcuts import render_to_response
from django.http import HttpResponseRedirect
from fornever.files.models import File
from fornever.files.forms import UploadFileForm
from fornever.settings import MEDIA_ROOT
from fornever.views import meta_render

def list(request):
    file_list = File.objects.all().order_by('date')
    return meta_render(request, 'files/file_list.html',
        {'file_list': file_list})
    
def upload(request):
    if request.method == 'POST':
        form = UploadFileForm(request.POST, request.FILES)
        if form.is_valid():
            form.save()
            return HttpResponseRedirect('/files/')
    else:
        form = UploadFileForm()
    return meta_render(request, 'files/file_upload.html', {'form': form})
