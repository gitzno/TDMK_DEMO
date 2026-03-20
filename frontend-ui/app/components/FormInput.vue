<script setup lang="ts">
// 1. Khai báo các tiện ích của Nuxt
const config = useRuntimeConfig();
const toast = useToast();
const isUploading = ref(false);
const selectedFile = ref<File | null>(null);

// 2. Danh sách 9 lựa chọn mã máy
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

// 3. Cấu hình các trường (Fields)
// Lưu ý: Trường 'file' sẽ được render thủ công qua Slot ở phần Template
const fields = [
  {
    name: "deviceId",
    type: "select",
    label: "Mã thiết bị",
    placeholder: "Chọn mã máy...",
    items: deviceOptions,
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
    label: "Hình ảnh hiện trường",
    required: true
  }
];

// 4. Hàm hứng file khi người dùng chọn hoặc chụp ảnh
const onFileChange = (e: any) => {
  const files = e.target.files;
  if (files && files[0]) {
    selectedFile.value = files[0];
  }
};

// 5. Hàm gửi dữ liệu lên API
const onSubmit = async (event: any) => {
  const formDataRaw = event.data;

  // Kiểm tra tính hợp lệ cơ bản
  if (formDataRaw.value === undefined || formDataRaw.value === null) {
    toast.add({ title: 'Vui lòng nhập giá trị Sensor', color: 'error' });
    return;
  }

  if (!selectedFile.value) {
    toast.add({ title: 'Vui lòng chụp hoặc chọn ảnh hiện trường', color: 'error' });
    return;
  }

  isUploading.value = true;

  try {
    const formData = new FormData();

    // Xử lý lấy DeviceId (hỗ trợ cả dạng Object của Select và String)
    const deviceIdValue = typeof formDataRaw.deviceId === 'object'
      ? formDataRaw.deviceId.value
      : formDataRaw.deviceId;

    // KHỚP CHÍNH XÁC KEY API: DeviceId, Value, Image
    formData.append('DeviceId', deviceIdValue || 'TDMK_01');
    formData.append('Value', String(formDataRaw.value));
    formData.append('Image', selectedFile.value);

    // Gửi request bằng $fetch
    await $fetch('/api/Telemetry', {
      method: 'POST',
      baseURL: config.public.apiBase,
      body: formData,
      // Không set Content-Type để trình duyệt tự tạo Multipart Boundary
    });

    toast.add({
      title: 'Gửi báo cáo thành công!',
      color: 'success',
      icon: 'i-lucide-check-circle'
    });

    // Reset file sau khi gửi thành công
    selectedFile.value = null;

  } catch (error: any) {
    console.error("Lỗi gửi Form:", error);
    const serverError = error.response?._data?.title || 'Gửi thất bại, vui lòng thử lại';
    toast.add({ title: serverError, color: 'error' });
  } finally {
    isUploading.value = false;
  }
};
</script>

<template>
  <UContainer class="py-10 flex justify-center">
    <UAuthForm
      title="Nhập liệu thiết bị"
      description="Chọn mã máy, nhập thông số và chụp ảnh"
      :fields="fields"
      :loading="isUploading"
      :submit-button="{
        label: 'Gửi báo cáo ngay',
        color: 'primary',
        class: 'w-full py-3 shadow-md font-bold',
      }"
      @submit="onSubmit"
    >
      <template #file-field="{ field }">
        <UFormGroup :label="field.label" :name="field.name" required class="mt-4">
          <div class="flex flex-col gap-2">
            <UInput
              type="file"
              icon="i-lucide-camera"
              accept="image/*"
              capture="environment"
              @change="onFileChange"
            />
            <p v-if="selectedFile" class="text-xs text-green-500 italic">
              Đã chọn: {{ selectedFile.name }}
            </p>
          </div>
        </UFormGroup>
      </template>
    </UAuthForm>
  </UContainer>

  <UNotifications />
</template>

<style scoped>
/* Thêm CSS nếu cần tùy chỉnh giao diện */
</style>
