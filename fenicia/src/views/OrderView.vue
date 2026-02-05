<script setup lang="ts">
import api from '@/api/client';
import ModuleCard from '@/components/ModuleCard.vue';
import { useLoadingStore } from '@/stores/loading';
import type { BadRequestType } from '@/types/BadRequestType';
import type { ModuleResponse } from '@/types/Responses';
import type { AxiosResponse } from 'axios';
import { useToast } from 'buefy';
import { ref } from 'vue';

const loadingStore = useLoadingStore();
const toast = useToast();
const modules = ref<ModuleResponse[]>([]);

const loadModules = async () => {
  try {
    loadingStore.setLoading(true);
    const { data } = await api.get<AxiosResponse<ModuleResponse[]>>('/module?page=1&perPage=20');

    modules.value = data.data;

  } catch (error: any) {

    const status = error.response?.status || error.status;

    if (status === 400) {
      const errorDetail = error.response.data as BadRequestType;
      toast.open({
        type: "is-danger",
        message: errorDetail.title || "Erro de validação",
        position: 'is-bottom-right'
      });
    }

  } finally {
    loadingStore.setLoading(false);
  }
};

loadModules();

</script>

<template>

  <div class="container">

    <h1 class="title">Módulos disponíveis</h1>
    <h2 class="subtitle">Assine o módulo que falta para a sua empresa!</h2>

    <div class="grid is-col-min-12">
      <ModuleCard v-for="module in modules" :module="module" />
    </div>
  </div>
</template>
