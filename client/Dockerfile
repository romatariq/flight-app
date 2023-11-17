FROM nginx:latest

COPY project/build /usr/share/nginx/html/react
COPY project-vue/dist /usr/share/nginx/html/vue

COPY nginx.conf /etc/nginx/nginx.conf
COPY index.html /usr/share/nginx/html/