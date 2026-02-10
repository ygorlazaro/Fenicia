<script setup lang="ts">
import api from '@/api/client';
import { useAuthStore } from '@/stores/auth';
import { useLoadingStore } from '@/stores/loading';
import type { IPagination } from '@/types/IPagination';
import type { CompanyResponse, ModuleResponse } from '@/types/Responses';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import logo from "../assets/logo.jpeg";

const authStore = useAuthStore();
const loadingStore = useLoadingStore();
const companies = ref<CompanyResponse[]>([]);
const modules = ref<ModuleResponse[]>([]);
const currentCompany = ref<CompanyResponse | undefined>(undefined);

const router = useRouter();

const handleCurrentCompany = (company: CompanyResponse) => {
  currentCompany.value = company;

  authStore.setCompany(company.id);

  fetchModules();
}

const fetchCompanies = async () => {
  loadingStore.setLoading(true);

  try {
    const response = await api.get<IPagination<CompanyResponse>>('user/company');

    companies.value = response.data;
    currentCompany.value = response.data[0] ?? undefined;

    authStore.setCompany(currentCompany.value.id)

    fetchModules();

  } catch (error) {

  }
  finally {
    loadingStore.setLoading(false);
  }
}

const fetchModules = async () => {
  loadingStore.setLoading(true);

  try {
    const response = await api.get<ModuleResponse[]>('user/module');

    modules.value = response.data;
  } catch (error) {

  }
  finally {
    loadingStore.setLoading(false);
  }
}

const handleLogout = () => {
  authStore.logout();

  router.push("/");
}

fetchCompanies();

</script>
<template>
  <b-navbar>

    <template #brand>
      <b-navbar-item tag="router-link" :to="{ path: '/' }">
        <b-image :src="logo" rounded class="is-32x32 mb-4" />
      </b-navbar-item>
    </template>

    <template #start>
      <b-navbar-item href="#"> Dashboard </b-navbar-item>

      <b-navbar-dropdown :label="module.name" v-for="module in modules">
        <b-navbar-item :href="submodule.route" v-for="submodule in module.submodules""> {{ submodule.name }} </b-navbar-item>
          </b-navbar-dropdown>
      </template>

<template #end>
        <b-navbar-dropdown :label="currentCompany?.name ?? 'Selecione a empresa'">
          <b-navbar-item v-for="company in companies" href="#" @click="handleCurrentCompany(company)"> {{ company.name
          }}
        </b-navbar-item>
      </b-navbar-dropdown>

      <b-navbar-item tag="div">
        <a class="button is-light" @click="handleLogout" ref="/order"> Módulos </a>

        <div class="buttons">
          <a class="button is-light" @click="handleLogout"> Sair </a>
        </div>
      </b-navbar-item>
    </template>
  </b-navbar>
</template>
