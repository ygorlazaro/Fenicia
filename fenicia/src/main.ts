import './assets/main.css'

import Buefy from 'buefy'
import 'buefy/dist/css/buefy.css'
import { createPinia } from 'pinia'
import { createApp } from 'vue'

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(Buefy)

app.mount('#app')

app.config.errorHandler = err => console.error(err);
