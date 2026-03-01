# ── Build Stage ───────────────────────────────────────────────────────────────
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files first — cached layer
COPY package*.json .
RUN npm install

# Copy source and build
COPY . .
RUN npm run build

# ── Runtime Stage ─────────────────────────────────────────────────────────────
# Nginx serves the static Vue build files
FROM nginx:alpine AS runtime

# Copy the built Vue app to nginx's serve folder
COPY --from=build /app/dist /usr/share/nginx/html

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]