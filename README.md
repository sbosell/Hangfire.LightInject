# Hangfire.LightInject

Lightinject integration for Hangfire. Provides an implementation of the JobActivator class and registration extensions, allowing you to use LightInject container to resolve job type instances as well as control the lifetime of the all related dependencies.

NUGET: install-package Lucuma.HangfireLightInject

http://hangfire.io

http://www.lightinject.net/

http://github.com/sbosell/hangfire.lightinject

You can enable it via the extension:

     var container= new ServiceContainer();
     // registrations
     GlobalConfiguration.Configuration.UseLightInjectActivator(container);
            
