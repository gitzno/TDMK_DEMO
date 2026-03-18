<script setup lang="ts">
import { h, resolveComponent } from "vue";

// 1. Lấy ID từ URL và cấu hình
const route = useRoute()
const config = useRuntimeConfig()
const reportId = route.params.id

// 2. Định nghĩa kiểu dữ liệu khớp API của Thụy
type TelemetryDetail = {
  id: number
  value: number
  fullImageUrl: string
  createdAt: string
  deviceId?: string
}

// 3. Gọi API lấy dữ liệu (Lazy để không chặn việc render giao diện)
const { data: report, pending, error, refresh } = await useAsyncData(
  `report-detail-${reportId}`,
  () => $fetch<TelemetryDetail>(`${config.public.apiBase}/api/Telemetry/${reportId}`),
  { immediate: true }
)

// Đảm bảo dữ liệu mới nhất khi mount tại Client
onMounted(() => {
  refresh()
})

const goBack = () => navigateTo('/reports')
</script>

<template>
  <UContainer class="py-6">
    <div class="flex items-center gap-4 mb-8">
      <UButton
        icon="i-lucide-arrow-left"
        color="neutral"
        variant="ghost"
        @click="goBack"
      />
      <div>
        <h1 class="text-2xl font-semibold tracking-tight">Báo cáo chi tiết</h1>
        <p class="text-sm text-neutral-500">Mã định danh: #{{ reportId }}</p>
      </div>
    </div>

    <div v-if="pending" class="space-y-6">
      <USkeleton class="h-80 w-full" />
      <USkeleton class="h-32 w-full" />
    </div>

    <div v-else-if="error || !report" class="py-20 flex flex-col items-center">
      <UIcon name="i-lucide-circle-alert" class="w-10 h-10 text-neutral-400 mb-4" />
      <p class="text-neutral-500 mb-4">Dữ liệu không tồn tại hoặc lỗi kết nối.</p>
      <UButton label="Quay lại danh sách" color="neutral" variant="outline" @click="goBack" />
    </div>

    <div v-else class="space-y-8">

      <UCard :ui="{ body: { padding: 'p-0' } }">
        <div class="flex flex-col">
          <img
            :src="report.fullImageUrl"
            class="w-full h-auto object-contain max-h-[600px]"
            alt="Telemetry Image"
          />
        </div>

        <template #footer>
          <div class="flex justify-between items-center">
            <span class="text-sm font-medium text-neutral-500 uppercase tracking-wider">Thông số đo</span>
            <UBadge color="neutral" variant="soft" size="lg" class="font-bold">
              {{ report.value }} Units
            </UBadge>
          </div>
        </template>
      </UCard>

      <UCard>
        <template #header>
          <h3 class="font-semibold text-neutral-900">Thông tin hệ thống</h3>
        </template>

        <div class="divide-y divide-neutral-100">
          <div class="flex justify-between py-4">
            <span class="text-neutral-500">Thiết bị nguồn</span>
            <span class="font-mono font-medium">{{ report.deviceId || 'TDMK_01' }}</span>
          </div>

          <div class="flex justify-between py-4">
            <span class="text-neutral-500">Giá trị Sensor</span>
            <span class="font-bold">{{ report.value }}</span>
          </div>

          <div class="flex justify-between py-4">
            <span class="text-neutral-500">Thời gian ghi nhận</span>
            <span>{{ report.createdAt }}</span>
          </div>
        </div>
      </UCard>

      <div class="flex flex-col gap-3">
        <UButton
          label="Tải ảnh về máy"
          icon="i-lucide-download"
          color="neutral"
          variant="outline"
          block
          @click="() => window.open(report.fullImageUrl, '_blank')"
        />
        <UButton
          label="Chia sẻ báo cáo"
          icon="i-lucide-share-2"
          color="neutral"
          variant="ghost"
          block
        />
      </div>
    </div>
  </UContainer>
</template>

<style scoped>
img {
  @apply block mx-auto;
}
</style>
