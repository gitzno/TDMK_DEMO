<template>
  <div
    style="
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      align-items: center;
    "
  >
    <UPageHeader
      title="TDMK DEMO "
      description="Đây là DEMO cho ứng dụng quản lý báo cáo lỗi hãy nhập số liệu và chụp ảnh lỗi"
    />

    <div class="w-full flex flex-col items-center justify-center gap-6 p-4">
      <UPageCard class="w-full max-w-md">
        <FormInput />
      </UPageCard>
    </div>
  </div>
</template>
<script>
export default {};
</script>
<style lang=""></style>
