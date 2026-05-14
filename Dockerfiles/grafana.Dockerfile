FROM grafana/grafana:10.2.2
COPY ./prometheus-connector.yml  /etc/grafana/provisioning/datasources/
COPY ./dashboards.yml  /etc/grafana/provisioning/dashboards/
COPY ./dashboards/  /var/lib/grafana/dashboards/
