FROM mcr.microsoft.com/dotnet/sdk:6.0

RUN echo "docker:x:990:" >> /etc/group \ 
    && echo "baseuser:x:1000:990:baseuser account:/home/baseuser:/bin/bash" >> /etc/passwd \
    && mkdir -p /home/baseuser \
    && chown baseuser:docker /home/baseuser

# This command assumes that you are in the folder where the SystemIntegrationTest
# project has been published to.
COPY ./. /app

WORKDIR /app

ENTRYPOINT ["pwsh", "main.ps1"]