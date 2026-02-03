<script lang="ts">
import { ref } from 'vue';
import { RouterLink, RouterView } from 'vue-router';

export default {
  setup() {
    const isLogged = ref(false);

    return {
      isLogged

    }
  }
}
</script>

<template>
  <header>
    <div v-if="isLogged">
      <nav>
        <RouterLink to="/">Home</RouterLink>
        <RouterLink to="/about">About</RouterLink>
      </nav>
    </div>
  </header>

  <RouterView />
</template>

<style scoped>
</style>
