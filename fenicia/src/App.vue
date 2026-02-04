<script setup lang="ts">
import { RouterView } from 'vue-router';
import MainMenu from './components/MainMenu.vue';
import { useAuthStore } from './stores/auth';
import { useLoadingStore } from './stores/loading';

const loadingStore = useLoadingStore();
const authStore = useAuthStore();

</script>

<template>
  <header>
    <MainMenu v-if="authStore.isAuthenticated">
    </MainMenu>
  </header>

  <b-loading is-full-page v-model="loadingStore.isLoading" :can-cancel="true"></b-loading>

  <RouterView />
</template>

<style scoped>
</style>
