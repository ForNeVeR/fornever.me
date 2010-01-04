# -*- coding: utf-8 -*-

from django.forms import ModelForm
from fornever.files.models import File

class UploadFileForm(ModelForm):
    class Meta:
        model = File
        #content = forms.FileField()
        #description = forms.CharField()