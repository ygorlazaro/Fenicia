<script setup lang="ts">
import type { LoginRequest } from '@/types/LoginRequest';
import { ref } from 'vue';

const emit = defineEmits(['onLogin', 'onNavigate']);

const request = ref<LoginRequest>({
  email: '',
  password: ''
});

const handleOnLogin = () => {
  emit('onLogin', request);
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
