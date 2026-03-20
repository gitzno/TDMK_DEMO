<script setup lang="ts">
import { h, resolveComponent } from "vue";
import type { TableColumn } from "@nuxt/ui";
import type { Row } from "@tanstack/vue-table";

// 1. Khai báo kiểu dữ liệu
type Telemetry = {
  id: number;
  value: number;
  createdAt: string;
};

const config = useRuntimeConfig();

const page = ref(1);
const pageSize = ref(10);

// Resolver cho các component Nuxt UI
const UButton = resolveComponent("UButton");
const UBadge = resolveComponent("UBadge");
const UDropdownMenu = resolveComponent("UDropdownMenu");

// 2. Fetch dữ liệu với useAsyncData
const { data: apiResponse, pending, refresh } = await useAsyncData(
  'reports-data',
  () => $fetch<any>(`${config.public.apiBase}/api/Telemetry/reports`, {
    params: { page: page.value, pageSize: pageSize.value }
  }),
  { watch: [page, pageSize], immediate: true }
);

const tableData = computed(() => apiResponse.value?.data || []);
const totalItems = computed(() => apiResponse.value?.total || 0);

// Ép nạp lại dữ liệu khi Client mount
onMounted(() => { refresh(); });

// 3. Định nghĩa cột (Sử dụng các thuộc tính color của Nuxt UI)
const columns: TableColumn<Telemetry>[] = [
  {
    accessorKey: "id",
    header: "ID",
    cell: ({ row }) => h('span', { class: 'font-mono text-neutral-500' }, `#${row.getValue("id")}`),
  },
  {
    accessorKey: "value",
    header: "Thông số",
    cell: ({ row }) => h(UBadge, {
      color: "neutral", // Bỏ màu xanh, dùng màu trung tính
      variant: "soft",
      size: "md",
      class: "font-semibold"
    }, () => `${row.getValue("value")} units`),
  },
  {
    accessorKey: "createdAt",
    header: "Thời gian",
    cell: ({ row }) => h('span', { class: 'text-neutral-600' },
      new Date(row.getValue("createdAt")).toLocaleString("vi-VN")
    ),
  },
  {
    id: "actions",
    cell: ({ row }) => h(UDropdownMenu, {
      content: { align: "end" },
      items: getRowItems(row),
    }, () => h(UButton, {
      icon: "i-lucide-ellipsis-vertical",
      color: "neutral",
      variant: "ghost",
    })),
  },
];

function getRowItems(row: Row<Telemetry>) {
  return [
    { label: "Xem chi tiết", icon: "i-lucide-eye", onSelect: () => navigateTo(`/reports/${row.original.id}`) },
    { label: "Xóa", icon: "i-lucide-trash", color: "error" as const, onSelect: () => deleteRecord(row.original.id) }
  ];
}

const deleteRecord = async (id: number) => {
  if (!confirm("Xác nhận xóa?")) return;
  await $fetch(`${config.public.apiBase}/api/Telemetry/${id}`, { method: 'DELETE' });
  refresh();
  toast.add({ title: "Đã xóa", color: "neutral" });
}
</script>

<template>
  <UContainer class="py-6 flex flex-col gap-6 min-h-screen">

    <div class="flex items-center justify-between">
      <div class="space-y-1">
        <h1 class="text-2xl font-semibold tracking-tight">Lịch sử thiết bị</h1>
        <p class="text-sm text-neutral-500">
          {{ pending ? 'Đang đồng bộ...' : `Tìm thấy ${totalItems} kết quả` }}
        </p>
      </div>
      <UButton
        icon="i-lucide-refresh-cw"
        color="neutral"
        variant="outline"
        :loading="pending"
        @click="refresh"
      > Làm mới </UButton>
    </div>

    <UCard :ui="{ body: { padding: 'p-0' } }">
      <UTable
        :data="tableData"
        :columns="columns"
        :loading="pending"
        class="w-full"
      >
        <template #loading-state>
          <div class="p-4 space-y-3">
            <USkeleton class="h-8 w-full" v-for="i in 5" :key="i" />
          </div>
        </template>

        <template #empty-state>
          <div class="py-12 flex flex-col items-center gap-2">
            <UIcon name="i-lucide-database-zap" class="w-8 h-8 text-neutral-300" />
            <p class="text-neutral-400 text-sm italic">Không có dữ liệu</p>
          </div>
        </template>
      </UTable>

      <template #footer v-if="totalItems > 0">
        <div class="flex items-center justify-between">
          <p class="text-xs text-neutral-500 font-medium">Trang {{ page }}</p>
          <UPagination
            v-model="page"
            :total="totalItems"
            :page-count="pageSize"
            color="neutral"
            size="sm"
          />
        </div>
      </template>
    </UCard>
  </UContainer>
</template>
