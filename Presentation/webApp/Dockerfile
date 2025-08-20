# STEP 1 build static website
FROM node:alpine as builder

RUN apk update && apk add --no-cache make git

# Create app directory

WORKDIR /app

# Install app dependencies

COPY package.json package-lock.json /app/
RUN cd /app && npm set progress=false && npm install

# Copy project files into the docker image

ENV NODE_ENV production
ENV NODE_OPTIONS "--max-old-space-size=4096"

COPY .  /app

RUN cd /app && npm run build

# STEP 2 build a small nginx image with static website
FROM nginx:alpine

## Remove default nginx website

RUN rm -rf /usr/share/nginx/html/*

## From 'builder' copy website to default nginx public folder

COPY --from=builder /app/dist /usr/share/nginx/html
COPY --from=builder /app/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
