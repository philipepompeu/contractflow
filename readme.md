docker run -d --name pgweb \
  -p 8081:8081 \
  -e PGWEB_LISTEN=0.0.0.0 \
  -e PGWEB_SESSIONS=1 \
  sosedoff/pgweb