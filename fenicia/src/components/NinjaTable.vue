<script setup lang="ts">
import api from '@/api/client';
import { useLoadingStore } from '@/stores/loading';
import type { BadRequestType } from '@/types/BadRequestType';
import type { ProductCategoryResponse } from '@/types/basic/Responses';
import type { IPagination } from '@/types/IPagination';
import { useToast } from 'buefy';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

type NinjaTableColumn = {
  field: string;
  label: string;
  width?: string;
  numeric?: boolean;
}

const loadingStore = useLoadingStore();
const router = useRouter();
const toast = useToast();

const formErrors = ref<BadRequestType | undefined>(undefined);
let tableData = ref<ProductCategoryResponse[]>([])
const columns = ref<NinjaTableColumn[]>([{
  field: 'id',
  label: 'ID'
}, {
  field: 'name',
  label: 'Nome'
  }])

const fetchCategories = async () => {
  formErrors.value = undefined;
  try {
    loadingStore.setLoading(true);
    const { data } = await api.post<IPagination<ProductCategoryResponse>>('/productcategory');

    tableData = data.data;


  } catch (error: any) {

    const status = error.response?.status || error.status;

    if (status === 400) {
      const errorDetail = error.response.data as BadRequestType;
      toast.open({
        type: "is-danger",
        message: errorDetail.title || "Erro de validação",
        position: 'is-bottom-right'
      });
      formErrors.value = errorDetail;
    }

  } finally {
    loadingStore.setLoading(false);
  }
}

fetchCategories();
</script>

<template>
  <b-table :data="tableData" :columns="columns"></b-table>
</template>
