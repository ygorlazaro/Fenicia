<script setup lang="ts">
import api from '@/api/client';
import { useAuthStore } from '@/stores/auth';
import type { IPagination } from '@/types/IPagination';
import type { CompanyResponse, ModuleResponse } from '@/types/Responses';
import { ref } from 'vue';

const authStore = useAuthStore();
const isLoading = ref(false)
const companies = ref<CompanyResponse[]>([]);
const modules = ref<ModuleResponse[]>([]);
const currentCompany = ref<CompanyResponse | undefined>(undefined);


const handleCurrentCompany = (company: CompanyResponse) => {
  currentCompany.value = company;

  authStore.setCompany(company.id);

  fetchModules();
}

const fetchCompanies = async () => {
  isLoading.value = true;

  try {
    const response = await api.get<IPagination<CompanyResponse>>('user/company');

    companies.value = response.data;
    currentCompany.value = response.data[0] ?? undefined;

    fetchModules();

  } catch (error) {

  }
  finally {
    isLoading.value = false;
  }
}

const fetchModules = async () => {
  isLoading.value = true;

  try {
    const response = await api.get<ModuleResponse[]>('user/module');

    modules.value = response.data;
  } catch (error) {

  }
  finally {
    isLoading.value = false;
  }
}

fetchCompanies();

</script>

<template>
  <section>
    <b-navbar>
      <template #brand>
        <b-navbar-item tag="router-link" :to="{ path: '/' }">
          <!-- <img src="/src/assets/buefy.png" alt="Lightweight UI components for Vue.js based on Bulma" /> -->
        </b-navbar-item>
      </template>
      <template #start>
        <b-navbar-item href="#"> Dashboard </b-navbar-item>

        <div v-for="module in modules">
          <b-navbar-dropdown :label="module.name">
            <b-navbar-item :href="submodule.route" v-for="submodule in module.submodules""> {{ submodule.name }} </b-navbar-item>
          </b-navbar-dropdown>
        </div>
      </template>

      <template #end>
        <b-navbar-dropdown :label="currentCompany?.name ?? 'Selecione a empresa'">
          <b-navbar-item v-for="company in companies" href="#" @click="handleCurrentCompany(company)"> {{ company.name }}
          </b-navbar-item>
        </b-navbar-dropdown>

        <b-navbar-item tag="div">
          <div class="buttons">
            <a class="button is-light"> Sair </a>
          </div>
        </b-navbar-item>
      </template>
    </b-navbar>
  </section>
</template>
