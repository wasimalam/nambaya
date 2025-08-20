FROM mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04

LABEL maintainer="Mahmoud Zalt <mahmoud@zalt.me>"

# Create config directory
# an set it as WORKDIR

#WORKDIR /usr/src/app

# Bundle app source
#COPY . /usr/src/app
#RUN ls -al
#RUN whoami
#RUN chmod +x /usr/src/app/create_table.sh

ENV MSSQL_DATABASE=$MSSQL_DATABASE
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=$MSSQL_PASSWORD

VOLUME /var/opt/mssql

EXPOSE 1433

CMD [ "/opt/mssql/bin/sqlservr" ]
#CMD /bin/bash ./entrypoint.sh
