FROM composer:2 AS composer

WORKDIR /app
COPY composer.json ./
RUN composer install --no-dev --no-interaction --prefer-dist --optimize-autoloader

FROM php:8.3-cli-alpine AS runtime

RUN apk add --no-cache sqlite-dev \
 && docker-php-ext-install pdo pdo_sqlite

WORKDIR /app
COPY --from=composer /app/vendor ./vendor
COPY src ./src
COPY views ./views
COPY public ./public

ENV MOOD_DB_PATH=/data/mood.db

EXPOSE 8080
CMD ["php", "-S", "0.0.0.0:8080", "-t", "public"]
