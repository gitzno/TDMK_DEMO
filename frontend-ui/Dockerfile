# Giai đoạn 1: Build (Dùng Node 22 cho Nuxt 4)
FROM node:22-slim AS build
WORKDIR /src

# Cài đặt dependency
COPY package*.json ./
RUN npm install

# Copy code và build production
COPY . .
RUN npm run build

# Giai đoạn 2: Runtime (Nitro server)
FROM node:22-slim
WORKDIR /app

# Copy output từ bản build sang
COPY --from=build /src/.output ./

# Cấu hình môi trường mặc định cho Nuxt
ENV HOST=0.0.0.0
ENV PORT=3000
ENV NODE_ENV=production

EXPOSE 3000

# Chạy server Nitro
CMD ["node", "server/index.mjs"]
