FROM node:18 AS react-builder
WORKDIR /app

COPY /project/package*.json ./
RUN npm install

COPY /project/. .
RUN npm run build


FROM node:18 AS vue-builder
WORKDIR /app

COPY project-vue/package*.json ./
RUN npm install

COPY /project-vue/. .
RUN npm run build


FROM nginx:latest
WORKDIR /

COPY --from=react-builder app/build /usr/share/nginx/html/react
COPY --from=vue-builder app/dist /usr/share/nginx/html/vue

COPY nginx.conf /etc/nginx/nginx.conf
COPY index.html /usr/share/nginx/html/