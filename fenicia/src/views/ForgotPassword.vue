<script setup lang="ts">
import api from '@/api/client';
import { useAuthStore } from '@/stores/auth';
import { useLoadingStore } from '@/stores/loading';
import type { BadRequestType } from '@/types/BadRequestType';
import type { ForgotPasswordRequest } from '@/types/Requests';
import type { TokenResponse } from '@/types/Responses';
import { useToast } from 'buefy';
import { ref } from 'vue';
import { useRouter } from 'vue-router';


const toast = useToast();
const router = useRouter();
const request = ref<ForgotPasswordRequest>({
  email: ''
});

const loadingStore = useLoadingStore();

const formErrors = ref<BadRequestType | undefined>(undefined);

const handleOnLogin = async () => {
  formErrors.value = undefined;
  try {
    loadingStore.setLoading(true);
    await api.post<TokenResponse>('/forgotpassword', request.value);

      toast.open("Verifique seu e-mail para recuperar a senha");

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
    <form class="card-content" @submit.prevent="handleOnLogin">

      <b-field label="Email" horizontal :type="{ 'is-danger': formErrors?.errors?.Email }"
        :message="formErrors?.errors?.Email">
        <b-input type="email" v-model="request.email" maxlength="256" rounded required />
      </b-field>

      <footer class="card-footer">
        <b-button native-type="submit" class="card-footer-item m-4" rounded type="is-primary"
          :loading="loadingStore.isLoading">
          Receber e-mail de recuperação
        </b-button>

      </footer>

      <RouterLink to="/forgot-password">
        Esqueci minha senha
      </RouterLink>
    </form>
  </div>
</template>
