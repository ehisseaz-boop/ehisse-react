import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    strictPort: true,
    // Bütün /api və /Image sorğuları arxa planda backend-ə ötürülür.
    // Brauzerin URL-i həmişə localhost:5175 qalır, cookie-lər də eyni origin-də işləyir.
    proxy: {
      '/api': { target: 'http://localhost:5000', changeOrigin: true },
      '/Image': { target: 'http://localhost:5000', changeOrigin: true },
      '/images': { target: 'http://localhost:5000', changeOrigin: true },
    },
  },
})
