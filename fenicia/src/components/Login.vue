<script setup lang="ts">
import api from '@/api/client';
import { useAuthStore } from '@/stores/auth';
import type { LoginRequest } from '@/types/Requests';
import type { TokenResponse } from '@/types/Responses';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const emit = defineEmits(['onLogin', 'onNavigate']);
const router = useRouter();

const request = ref<LoginRequest>({
  email: '',
  password: ''
});
const isLoading = ref(false);

const authStore = useAuthStore();

const handleOnLogin = async () => {
  try {
    isLoading.value = true;
    const { data } = await api.post<TokenResponse>('/token', request.value);

    authStore.setAuth(data.accessToken, data.user);

    router.push('/dashboard');

  } catch (error) {
    console.error("Erro ao cadastrar", error);
  } finally {
    isLoading.value = false;
  }
};

</script>

<template>
  <div class="card">
    <header class="card-header">
      <p class="card-header-title">Bem-vindo</p>
    </header>

    <div class="card-content">
      <b-field label="Email">
        <b-input type="email" v-model="request.email" maxlength="50" />
      </b-field>

      <b-field label="Senha">
        <b-input type="password" v-model="request.password" maxlength="48" />
      </b-field>

      <footer class="card-footer">
        <b-button class="card-footer-item" type="is-primary" @click="handleOnLogin">Entrar</b-button>
        <b-button class="card-footer-item" type="is-primary" @click="$emit('onNavigate')"">Cadastrar</b-button>
      </footer>

    </div>
  </div>
</template>

<style scoped></style>
