// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: ["@nuxt/ui"],
  devtools: {
    enabled: true,
  },

  css: ["~/assets/css/main.css"],

  routeRules: {
    "/": { prerender: true },
  },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || "https://localhost:7287",
      hubUrl:
        process.env.NUXT_PUBLIC_HUB_URL ||
        "https://localhost:7287/hubs/monitor",
    },
  },
  compatibilityDate: "2025-01-15",
});
