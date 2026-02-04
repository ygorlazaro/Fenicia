import { defineStore } from "pinia";
import { ref } from "vue";

export const useLoadingStore = defineStore('loading', () => {

  const isLoading = ref(false);

  function setLoading(value: boolean) {
    isLoading.value = value;
  }

  return {
    isLoading,
    setLoading
  }

});
