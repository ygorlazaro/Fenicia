<script setup lang="ts">
import SignUp from '@/components/SignUp.vue';
import { ref } from 'vue';
import logo from "../assets/logo.jpeg";
import Login from '../components/Login.vue';

enum ViewEnum  {
  Login,
  SignUp
}

const currentView = ref(ViewEnum.Login)

</script>

<template>
  <main>
    <div class="is-flex is-flex-direction-column is-align-items-center">

      <b-image :src="logo" rounded class="is-128x128 mb-4" />

      <Login v-if="currentView === ViewEnum.Login" @on-navigate="currentView = ViewEnum.SignUp" />

      <SignUp v-else @on-navigate="currentView = ViewEnum.Login" />
    </div>
  </main>
</template>
