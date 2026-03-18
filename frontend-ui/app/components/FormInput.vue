<script setup lang="ts">
const config = useRuntimeConfig();
const toast = useToast();
const isUploading = ref(false);

// 1. Danh sách 9 lựa chọn cố định
const deviceOptions = [
  { label: "TDMK_01", value: "TDMK_01" },
  { label: "TDMK_02", value: "TDMK_02" },
  { label: "TDMK_03", value: "TDMK_03" },
  { label: "SEEV_01", value: "SEEV_01" },
  { label: "SEEV_02", value: "SEEV_02" },
  { label: "SEEV_03", value: "SEEV_03" },
  { label: "SUNTORY_01", value: "SUNTORY_01" },
  { label: "SUNTORY_02", value: "SUNTORY_02" },
  { label: "SUNTORY_03", value: "SUNTORY_03" },
];

// 2. Cấu hình Fields
const fields = [
  {
    name: "deviceId",
    type: "select", // Chuyển từ text sang select
    label: "Mã thiết bị",
    placeholder: "Chọn mã máy...",
    items: deviceOptions, // Đổ 9 lựa chọn vào đây
    required: true,
    icon: "i-lucide-monitor-speaker",
  },
  {
    name: "value",
    type: "number",
    label: "Giá trị Sensor",
    placeholder: "Ví dụ: 2.5",
    required: true,
    icon: "i-lucide-gauge",
  },
  {
    name: "file",
    type: "file",
    label: "Hình ảnh hiện trường",
    required: true,
    inputProps: {
      accept: "image/*",
      capture: "environment",
    },
  },
];

// 3. Hàm xử lý gửi dữ liệu (Giữ nguyên logic FormData)
const onSubmit = async (event: any) => {
  // Nuxt UI v3 truyền event, dữ liệu nằm trong event.data
  const formDataRaw = event.data;

  // 1. Kiểm tra giá trị Sensor
  if (formDataRaw.value === undefined || formDataRaw.value === null) {
    toast.add({ title: 'Thiếu giá trị', color: 'error' });
    return;
  }

  isUploading.value = true;

  try {
    const formData = new FormData();

    // 2. LẤY GIÁ TRỊ TỪ SELECT (Sửa lỗi DeviceId là Object)
    // Nếu deviceId là Object { label: '...', value: 'TDMK_01' }
    const deviceIdValue = typeof formDataRaw.deviceId === 'object'
      ? formDataRaw.deviceId.value
      : formDataRaw.deviceId;

    formData.append('DeviceId', deviceIdValue || 'TDMK_01');
    formData.append('Value', String(formDataRaw.value));

    // 3. Xử lý File
    if (formDataRaw.file) {
      const file = formDataRaw.file instanceof FileList ? formDataRaw.file[0] : formDataRaw.file;
      if (file) {
        formData.append('Image', file);
      }
    }

    // 4. Gửi API
    await $fetch(`${config.public.apiBase}/api/Telemetry`, {
      method: 'POST',
      body: formData
    });

    toast.add({ title: 'Gửi thành công!', color: 'neutral', icon: 'i-lucide-check' });

  } catch (error) {
    console.error("Lỗi gửi Form:", error);
    toast.add({ title: 'Gửi thất bại', color: 'error' });
  } finally {
    isUploading.value = false;
  }
};
</script>

<template>
  <UContainer class="py-10 flex justify-center">
    <UAuthForm
      title="Nhập liệu thiết bị"
      description="Vui lòng chọn mã máy và nhập thông số"
      :fields="fields"
      :loading="isUploading"
      :submit-button="{
        label: 'Gửi báo cáo',
        color: 'neutral',
        class: 'w-full py-3 shadow-none font-bold',
      }"
      @submit="onSubmit"
    />
  </UContainer>
</template>
