<script setup lang="ts">
import SignUp from '@/components/SignUp.vue';
import { ref } from 'vue';
import Login from '../components/Login.vue';

enum ViewEnum  {
  Login,
  SignUp
}

const currentView = ref(ViewEnum.SignUp)

const onLogin = (loginRequest: any) => {
  console.log(loginRequest.value)
}
</script>

<template>
  <main>
    <Login @on-login="onLogin" v-if="currentView === ViewEnum.Login" @on-navigate="currentView = ViewEnum.SignUp" />

    <SignUp v-else @on-navigate="currentView = ViewEnum.Login"/>
  </main>
</template>
