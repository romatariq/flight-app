import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

const BASE_URL = process.env.BASE_URL || '';

// https://vitejs.dev/config/
export default defineConfig({
  base: BASE_URL,
  plugins: [vue()],
  resolve: {
    alias: {
      //@ts-ignore
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
