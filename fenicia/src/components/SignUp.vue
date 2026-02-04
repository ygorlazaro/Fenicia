<script setup lang="ts">
import api from '@/api/client';
import { useLoadingStore } from '@/stores/loading';
import type { BadRequestType } from '@/types/BadRequestType';
import type { SignUpRequest } from '@/types/Requests';
import type { UserResponse } from '@/types/Responses';
import { useToast } from 'buefy';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const emit = defineEmits(['onLogin', 'onNavigate']);
const router = useRouter();
const loadingStore = useLoadingStore();
const toast = useToast();
const formErrors = ref<BadRequestType | undefined>(undefined);

const request = ref<SignUpRequest>({
  email: '',
  password: '',
  company: {
    cnpj: '',
    name: ''
  },
  name: ''
});

const handleOnSignup = async () => {
  formErrors.value = undefined;
  try {
    loadingStore.setLoading(true);
    const { data } = await api.post<UserResponse>('/register', request.value);

    router.push('/');

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
};

</script>

<template>
  <div class="card">

    <form class="card-content" @submit.prevent="handleOnSignup">
      <b-field label="Nome" horizontal :type="{ 'is-danger': formErrors?.errors?.Name }"
        :message="formErrors?.errors?.Name">
        <b-input type="text" v-model="request.name" maxlength="48" rounded required />
      </b-field>

      <b-field label="Email" horizontal :type="{ 'is-danger': formErrors?.errors?.Email }"
        :message="formErrors?.errors?.Email">
        <b-input type="email" v-model="request.email" maxlength="48" rounded required />
      </b-field>

      <b-field label="Senha" horizontal :type="{ 'is-danger': formErrors?.errors?.Password }"
        :message="formErrors?.errors?.Password">
        <b-input type="password" v-model="request.password" maxlength="200" rounded required />
      </b-field>

      <b-field label="Nome da empresa" horizontal :type="{ 'is-danger': formErrors?.errors?.Company?.Name }"
        :message="formErrors?.errors?.Company.Name">
        <b-input type="text" v-model="request.company.name" maxlength="200" rounded required />
      </b-field>

      <b-field label="CNPJ" horizontal :type="{ 'is-danger': formErrors?.errors?.Company?.Cnpj }"
        :message="formErrors?.errors?.Company.Cnpj">
        <b-input type="text" v-model="request.company.cnpj" maxlength="14" rounded required />
      </b-field>

      <footer class="card-footer">
        <b-button class="card-footer-item m-4" rounded type="is-primary" :loading="loadingStore.isLoading"
          @click="handleOnSignup">Cadastrar</b-button>
        <b-button class="card-footer-item m-4" rounded type="is-primary" @click="$emit('onNavigate')">Login</b-button>
      </footer>

    </form>
  </div>
</template>

<style scoped></style>
