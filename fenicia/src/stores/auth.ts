import type { UserResponse } from "@/types/Responses";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '');
  const user = ref<UserResponse | undefined>(undefined);
  const companyId = ref(localStorage.getItem('companyId') || '');

  const isAuthenticated = computed(() => !!token.value);

  function setAuth(newToken: string, userData: UserResponse) {
    token.value = newToken;
    user.value = userData;
    localStorage.setItem('token', newToken);
    localStorage.setItem('userId', user.value.id);
  }

  function setCompany(id: string) {
    companyId.value = id;
    localStorage.setItem('companyId', id);
  }

  function logout() {
    token.value = '';
    companyId.value = '';
    localStorage.clear();
  }

  return { token, user, companyId, isAuthenticated, setAuth, setCompany, logout };
});
