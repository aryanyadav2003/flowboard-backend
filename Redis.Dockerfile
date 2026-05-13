FROM alpine:latest
RUN apk add --no-cache redis python3
EXPOSE 6379
EXPOSE 80

# Run a dummy HTTP server on port 80 to pass Render's health check, 
# and start Redis server on port 6379.
CMD python3 -m http.server 80 & redis-server --protected-mode no
