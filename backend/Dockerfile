FROM node:lts

WORKDIR /app

ADD package.json /app/
RUN yarn install

ADD . /app/
RUN yarn build

CMD node /app/dist/main.js