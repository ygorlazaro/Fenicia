import { useAuthStore } from '@/stores/auth';
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5144'
});

api.interceptors.request.use((config) => {
  const authStore = useAuthStore();

  if (authStore.token) {
    config.headers.Authorization = `Bearer ${authStore.token}`;
  }

  if (authStore.companyId) {
    config.headers['x-company'] = authStore.companyId;
  }

  return config;
});

api.interceptors.response.use(
  res => res,
  error => {
    if (error.response?.status === 401) {
      const authStore = useAuthStore();
      authStore.logout();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
