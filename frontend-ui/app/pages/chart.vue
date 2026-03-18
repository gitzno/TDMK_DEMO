<script setup lang="ts">
import * as signalR from "@microsoft/signalr";
import { use } from "echarts/core";
import { CanvasRenderer } from "echarts/renderers";
import { LineChart } from "echarts/charts";
import { GridComponent, TooltipComponent, TitleComponent } from "echarts/components";
import VChart from "vue-echarts";

// Đăng ký các module cần thiết cho ECharts
use([CanvasRenderer, LineChart, GridComponent, TooltipComponent, TitleComponent]);

const config = useRuntimeConfig();

// 1. Khởi tạo dữ liệu biểu đồ (Mặc định 10 điểm dữ liệu trống)
const chartData = ref<{ time: string; value: number }[]>([]);

// 2. Cấu hình ECharts (Thuần Nuxt UI Style - Neutral)
const chartOptions = computed(() => ({
  animation: true,
  tooltip: { trigger: 'axis' },
  grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
  xAxis: {
    type: 'category',
    data: chartData.value.map(d => d.time),
    boundaryGap: false,
    axisLine: { lineStyle: { color: '#e5e5e5' } }
  },
  yAxis: {
    type: 'value',
    splitLine: { lineStyle: { color: '#f5f5f5' } }
  },
  series: [{
    name: 'Sensor Value',
    type: 'line',
    smooth: true,
    data: chartData.value.map(d => d.value),
    symbol: 'circle',
    symbolSize: 8,
    itemStyle: { color: '#00DC82' }, // Màu trung tính (Neutral)
    lineStyle: { width: 3, color: '#00DC82' },
    areaStyle: { color: 'rgba(23, 23, 23, 0.05)' }
  }]
}));

// 3. Lấy dữ liệu lịch sử ban đầu (10 bản ghi mới nhất)
const fetchHistory = async () => {
  const data = await $fetch<any>(`${config.public.apiBase}/api/Telemetry/reports`, {
    query: { page: 1, pageSize: 10 }
  });
  if (data?.data) {
    // Đảo ngược mảng để thời gian chạy từ trái sang phải
    chartData.value = data.data.reverse().map((item: any) => ({
      time: new Date(item.createdAt).toLocaleTimeString('vi-VN', { hour12: false }),
      value: item.value
    }));
  }
};

// 4. Kết nối SignalR
let connection: signalR.HubConnection | null = null;

const connectSignalR = () => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${config.public.apiBase}/hubs/monitor`)
    .withAutomaticReconnect()
    .build();

  // Lắng nghe sự kiện ReceiveData từ Server
  connection.on("ReceiveData", (newRecord: { value: number, timestamp: string }) => {
    // Thêm dữ liệu mới vào mảng
    chartData.value.push({
      time: newRecord.timestamp,
      value: newRecord.value
    });

    // Giữ lại tối đa 15 điểm trên biểu đồ để tránh bị rối
    if (chartData.value.length > 15) {
      chartData.value.shift();
    }
  });

  connection.start().catch(err => console.error("SignalR Error: ", err));
};

onMounted(async () => {
  await fetchHistory();
  connectSignalR();
});

onUnmounted(() => {
  if (connection) connection.stop();
});
</script>

<template>
  <UContainer class="py-6">
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-2xl font-semibold tracking-tight">Giám sát Real-time</h1>
        <p class="text-sm text-neutral-500 flex items-center gap-2">
          <span class="relative flex h-2 w-2">
            <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-neutral-400 opacity-75"></span>
            <span class="relative inline-flex rounded-full h-2 w-2 bg-neutral-500"></span>
          </span>
          Đang kết nối Socket...
        </p>
      </div>
      <UButton
        icon="i-lucide-history"
        color="neutral"
        variant="outline"
        label="Xem lịch sử"
        @click="navigateTo('/reports')"
      />
    </div>

    <UCard class="overflow-hidden">
      <template #header>
        <div class="flex justify-between items-center">
          <h3 class="font-medium">Biểu đồ thông số Sensor</h3>
          <UBadge v-if="chartData.length > 0" color="neutral" variant="soft">
            Hiện tại: {{ chartData[chartData.length - 1].value }}
          </UBadge>
        </div>
      </template>

      <div class="h-[400px] w-full">
        <v-chart class="chart" :option="chartOptions" autoresize />
      </div>
    </UCard>

    <div class="mt-4 flex gap-2">
       <UIcon name="i-lucide-info" class="text-neutral-400 w-4 h-4" />
       <p class="text-xs text-neutral-400">Dữ liệu sẽ tự động cập nhật mỗi khi có tín hiệu mới từ Hub.</p>
    </div>
  </UContainer>
</template>

<style scoped>
.chart {
  height: 100%;
}
</style>
