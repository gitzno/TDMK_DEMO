<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui";

const route = useRoute();

const items = computed<NavigationMenuItem[]>(() => [
  {
    label: "Nhập liệu",
    to: "/",
    icon: "i-lucide-book-open",
    active: route.path === "/",
  },
  {
    label: "Liệt kê",
    to: "/reports",
    icon: "i-lucide-box",
    active: route.path.startsWith("/reports"),
  },
  {
    label: "Biểu đồ",
    to: "/chart",
    icon: "i-lucide-box",
    active: route.path.startsWith("/chart"),
  },
]);
</script>

<template>
  <UHeader toggle-side="left">
    <template #title>
      <Logo class="h-6 w-auto" />
    </template>

    <UNavigationMenu :items="items" />

    <template #right>
      <UColorModeButton />
    </template>

    <template #body>
      <UNavigationMenu :items="items" orientation="vertical" class="-mx-2.5" />
    </template>
  </UHeader>
</template>
