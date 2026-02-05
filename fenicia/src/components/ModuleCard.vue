<script setup lang="ts">
import api from '@/api/client';
import { useLoadingStore } from '@/stores/loading';
import type { BadRequestType } from '@/types/BadRequestType';
import type { SubmoduleResponse } from '@/types/Responses';
import type { AxiosResponse } from 'axios';
import { useToast } from 'buefy';
import { ref } from 'vue';

const props = defineProps(['module']);
const loadingStore = useLoadingStore();
const toast = useToast();
const submodules = ref<SubmoduleResponse[]>([]);

const loadSubmodules = async () => {
  try {
    loadingStore.setLoading(true);
    const { data } = await api.get<AxiosResponse<SubmoduleResponse[]>>('/submodule/' + props.module.id);

    submodules.value = data;

  } catch (error: any) {

    const errorDetail = error.response.data as BadRequestType;

    console.log(error.response.data)
    toast.open({
      type: "is-danger",
      message: errorDetail.title || "Erro de validação",
      position: 'is-bottom-right'
    });

  } finally {
    loadingStore.setLoading(false);
  }
};

loadSubmodules();

</script>

<template>
  <div class="card is-medium cell">
    <div class="card-image">
      <figure class="image is-4by3">
        <img src="https://bulma.io/assets/images/placeholders/1280x960.png" alt="Placeholder image" />
      </figure>
    </div>
    <div class="card-content">
      <div class="media">
        <div class="media-left">

          <figure class="image is-48x48">
            <img src="https://bulma.io/assets/images/placeholders/96x96.png" alt="Placeholder image" />
          </figure>
        </div>
        <div class="media-content">
          <p class="title is-4">{{ module.name }}</p>
          <p class="subtitle is-6">R$ {{ module.amount }}/mês</p>
        </div>
      </div>

      <div class="content">
        <ul>
          <li v-for="submodule in submodules">
            {{ submodule.name }}
          </li>
        </ul>
        <br />
        <time datetime="2016-1-1">11:09 PM - 1 Jan 2016</time>
      </div>
    </div>
  </div>
</template>
