FROM jelastic/dotnet:7.0.404 AS base
WORKDIR /home/jelastic/APP/ROOT
EXPOSE 22 80

COPY app/publish/** .

#ENTRYPOINT ["/bin/sh"]
#CMD ASPNETCORE_URLS=http://*:$PORT dotnet Pantry.Service.dll