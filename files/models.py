# -*- coding: utf-8 -*-

from django.db import models

class File(models.Model):
    content = models.FileField(upload_to='files/')
    date = models.DateField(auto_now_add=True)
    description = models.TextField()
    def __unicode__(self):
        return self.content.name
    def get_absolute_url(self):
        return self.content.url