import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import autoprefixer from 'autoprefixer'

// https://vitejs.dev/config/
export default defineConfig({
  base: './',
  build: {
    outDir: 'build',
  },
  css: {
    postcss: {
      plugins: [
        autoprefixer({}),
      ],
    },
  },
  optimizeDeps: {
    esbuildOptions: {
      loader: {
        '.js': 'jsx',
        '.ts': 'tsx',
      },
    },
  },
  plugins: [react()],
  resolve: {
    alias: [
      {
        find: 'src/',
        replacement: `${path.resolve(__dirname, 'src')}/`,
      },
    ],
    extensions: ['.mjs', '.js', '.ts', '.jsx', '.tsx', '.json', '.scss'],
  },
  server: {
    port: 3000,
  },
})
